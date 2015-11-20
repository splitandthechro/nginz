/**
  * Copyright (c) 2015, GruntTheDivine All rights reserved.

  * Redistribution and use in source and binary forms, with or without modification,
  * are permitted provided that the following conditions are met:
  * 
  *  * Redistributions of source code must retain the above copyright notice, this list
  *    of conditions and the following disclaimer.
  * 
  *  * Redistributions in binary form must reproduce the above copyright notice, this
  *    list of conditions and the following disclaimer in the documentation and/or
  *    other materials provided with the distribution.

  * Neither the name of the copyright holder nor the names of its contributors may be
  * used to endorse or promote products derived from this software without specific
  * prior written permission.
  * 
  * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
  * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
  * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT
  * SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
  * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
  * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR
  * BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
  * CONTRACT ,STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
  * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
  * DAMAGE.
**/

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Iodine.Compiler;
using Iodine.Compiler.Ast;

namespace Iodine.Runtime
{
	public class IodineModule : IodineObject
	{
		public static readonly List<IodineObject> SearchPaths = new List<IodineObject> ();
		private static readonly IodineTypeDefinition ModuleTypeDef = new IodineTypeDefinition ("Module");

		static IodineModule ()
		{
			SearchPaths.Add (new IodineString (Environment.CurrentDirectory));
			SearchPaths.Add (new IodineString (Path.Combine (Path.GetDirectoryName (
				Assembly.GetEntryAssembly ().Location), "modules")));
			if (Environment.GetEnvironmentVariable ("IODINE_PATH") != null) {
				foreach (string path in Environment.GetEnvironmentVariable ("IODINE_PATH").Split (
					Path.PathSeparator)) {
					SearchPaths.Add (new IodineString (path));
				}
			}
		}

		public string Name { set; get; }

		public IList<IodineObject> ConstantPool {
			get {
				return this.constantPool;
			}
		}

		public IList<string> Imports { private set; get; }

		public bool ExistsInGlobalNamespace {
			protected set;
			get;
		}

		public IodineMethod Initializer { set; get; }

		private List<IodineObject> constantPool = new List<IodineObject> ();

		public IodineModule (string name)
			: base (ModuleTypeDef)
		{
			Name = name;
			Imports = new List<string> ();
			Initializer = new IodineMethod (this, "__init__", false, 0, 0);
			Attributes ["__init__"] = Initializer;
		}

		public void AddMethod (IodineMethod method)
		{
			Attributes [method.Name] = method;
		}

		public int DefineConstant (IodineObject obj)
		{
			constantPool.Add (obj);
			return constantPool.Count - 1;
		}

		public override IodineObject Invoke (VirtualMachine vm, IodineObject[] arguments)
		{
			Initializer.Invoke (vm, arguments);
			return null;
		}

		public override string ToString ()
		{
			return string.Format ("<Module {0}>", Name);
		}

		public static IodineModule CompileModule (ErrorLog errorLog, string file)
		{

			if (FindModule (file) != null) {
				Tokenizer lexer = new Tokenizer (errorLog, File.ReadAllText (FindModule (file)), file);
				TokenStream tokenStream = lexer.Scan ();
				if (errorLog.ErrorCount > 0)
					return null;
				Parser parser = new Parser (tokenStream);
				AstRoot root = parser.Parse ();
				if (errorLog.ErrorCount > 0)
					return null;
				SemanticAnalyser analyser = new SemanticAnalyser (errorLog);
				SymbolTable symbolTable = analyser.Analyse (root);
				if (errorLog.ErrorCount > 0)
					return null;
				IodineCompiler compiler = new IodineCompiler (errorLog, symbolTable, Path.GetFullPath (file));
				IodineModule module = new IodineModule (Path.GetFileNameWithoutExtension (file));
				compiler.CompileAst (module, root);
				if (errorLog.ErrorCount > 0)
					return null;
				return module;
			} else {
				errorLog.AddError (ErrorType.ParserError, new Location (0, 0, file), 
					"Could not find module {0}", file);
				return null;
			}
		}

		public static IodineModule CompileModuleFromSource (ErrorLog errorLog, string source)
		{
			Tokenizer lexer = new Tokenizer (errorLog, source);
			TokenStream tokenStream = lexer.Scan ();
			if (errorLog.ErrorCount > 0)
				return null;
			Parser parser = new Parser (tokenStream);
			AstRoot root = parser.Parse ();
			if (errorLog.ErrorCount > 0)
				return null;
			SemanticAnalyser analyser = new SemanticAnalyser (errorLog);
			SymbolTable symbolTable = analyser.Analyse (root);
			if (errorLog.ErrorCount > 0)
				return null;
			IodineCompiler compiler = new IodineCompiler (errorLog, symbolTable, "");
			IodineModule module = new IodineModule ("");

			compiler.CompileAst (module, root);
			if (errorLog.ErrorCount > 0)
				return null;
			return module;
		}

		public static IodineModule LoadModule (ErrorLog errLog, string path)
		{
			if (FindExtension (path) != null) {
				return LoadExtensionModule (Path.GetFileNameWithoutExtension (path), 
					FindExtension (path));
			} else if (FindModule (path) != null) {
				string fullPath = FindModule (path);
				string dir = Path.GetDirectoryName (fullPath);
				if (!ContainsPath (dir)) {
					SearchPaths.Add (new IodineString (dir));
					string depPath = Path.Combine (dir, ".deps");
					if (!ContainsPath (depPath)) {
						SearchPaths.Add (new IodineString (depPath));
					}
				}
				return CompileModule (errLog, FindModule (path));
			} else if (BuiltInModules.Modules.ContainsKey (path)) {
				return BuiltInModules.Modules [path];
			}
			return null;
		}

		private static IodineModule LoadExtensionModule (string module, string dll)
		{
			Assembly extension = Assembly.Load (AssemblyName.GetAssemblyName (dll));

			foreach (Type type in extension.GetTypes ()) {
				if (type.IsDefined (typeof(IodineBuiltinModule), false)) {
					IodineBuiltinModule attr = (IodineBuiltinModule)type.GetCustomAttributes (
						typeof(IodineBuiltinModule), false).First ();
					if (attr.Name == module) {
						return (IodineModule)type.GetConstructor (new Type[] { }).Invoke (new object[]{ });
					}
				}
			}
			return null;
		}

		private static bool ContainsPath (string path)
		{
			foreach (IodineObject obj in SearchPaths) {
				if (obj.ToString () == path) {
					return true;
				}
			}
			return false;
		}

		private static string FindModule (string name)
		{
			if (File.Exists (name)) {
				return name;
			}
			if (File.Exists (name + ".id")) {
				return name + ".id";
			}

			foreach (IodineObject obj in SearchPaths) {
				string dir = obj.ToString ();
				string expectedName = Path.Combine (dir, name + ".id");
				if (File.Exists (expectedName)) {
					return expectedName;
				}
			}

			return null;
		}

		private static string FindExtension (string name)
		{
			if (File.Exists (name) && name.EndsWith (".dll")) {
				return name;
			}
			if (File.Exists (name + ".dll")) {
				return name + ".dll";
			}

			string exePath = Path.Combine (Path.GetDirectoryName (Assembly.GetEntryAssembly ().Location), "extensions");

			if (Directory.Exists (exePath)) {
				foreach (string file in Directory.GetFiles (exePath)) {
					string fname = Path.GetFileName (file);
					if (fname == name || fname == name + ".dll") {
						return file;
					}
				}
			}
			return null;
		}

		private static bool CanWrite (string folderPath)
		{
			try {
				return true;
			} catch (UnauthorizedAccessException) {
				return false;
			}
		}
	}
}
