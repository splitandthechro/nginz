// /**
//   * Copyright (c) 2015, GruntTheDivine All rights reserved.
//
//   * Redistribution and use in source and binary forms, with or without modification,
//   * are permitted provided that the following conditions are met:
//   * 
//   *  * Redistributions of source code must retain the above copyright notice, this list
//   *    of conditions and the following disclaimer.
//   * 
//   *  * Redistributions in binary form must reproduce the above copyright notice, this
//   *    list of conditions and the following disclaimer in the documentation and/or
//   *    other materials provided with the distribution.
//
//   * Neither the name of the copyright holder nor the names of its contributors may be
//   * used to endorse or promote products derived from this software without specific
//   * prior written permission.
//   * 
//   * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
//   * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
//   * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT
//   * SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
//   * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
//   * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR
//   * BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
//   * CONTRACT ,STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
//   * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
//   * DAMAGE.
// /**
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Iodine.Runtime;

namespace Iodine.Compiler
{
	public delegate IodineModule ModuleResolveHandler (string name);

	/// <summary>
	/// Represent v
	/// </summary>
	public class IodineContext
	{
		public readonly ErrorLog ErrorLog;
		public readonly VirtualMachine VirtualMachine;
		public readonly IodineConfiguration Configuration; // Virtual machine configuration 

		/*
		 * Where we can search for modules
		 */
		public readonly List<string> SearchPath = new List<string> ();

		public bool AllowBuiltins { 
			set;
			get;
		}

		public bool ShouldOptimize {
			set;
			get;
		}

		private Dictionary<string, IodineModule> moduleCache = new Dictionary<string, IodineModule> ();

		private ModuleResolveHandler _resolveModule;

		/// <summary>
		/// Occurs before a module is resolved
		/// </summary>
		public event ModuleResolveHandler ResolveModule {
			add {
				_resolveModule += value;
			}
			remove {
				_resolveModule -= value;
			}
		}

		public IodineContext ()
			: this (new IodineConfiguration ())
		{
			string exeDir = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);
			string iodinePath = Environment.GetEnvironmentVariable ("IODINE_PATH");
			SearchPath.Add (Environment.CurrentDirectory);
			SearchPath.Add (Path.Combine (exeDir, "modules"));
			SearchPath.Add (Path.Combine (exeDir, "extensions"));
			if (iodinePath != null) {
				SearchPath.AddRange (iodinePath.Split (':'));
			}
			ShouldOptimize = true;
			AllowBuiltins = true;
		}

		public IodineContext (IodineConfiguration config)
		{
			Configuration = config;
			ErrorLog = new ErrorLog ();
			VirtualMachine = new VirtualMachine (this);
		}

		/// <summary>
		/// Invokes an IodineObject (Calling its __invoke__ method) under this
		/// context 
		/// </summary>
		/// <param name="obj">The object to invoke.</param>
		/// <param name="args">Arguments.</param>
		public IodineObject Invoke (IodineObject obj, IodineObject[] args)
		{
			return obj.Invoke (VirtualMachine, args);
		}

		/// <summary>
		/// Loads an Iodine module.
		/// </summary>
		/// <returns>A compiled Iodine module.</returns>
		/// <param name="name">The module's name.</param>
		public IodineModule LoadModule (string name)
		{
			if (moduleCache.ContainsKey (name)) {
				return moduleCache [name];
			}

			if (_resolveModule != null) {
				foreach (Delegate del in _resolveModule.GetInvocationList ()) {
					ModuleResolveHandler handler = del as ModuleResolveHandler;
					IodineModule result = handler (name);
					if (result != null) {
						return result;
					}
				}
			}

			IodineModule module = LoadIodineModule (name);

			if (module == null) {
				module = LoadExtensionModule (name);
			}

			if (module != null) {
				moduleCache [name] = module;
			}

			return module;
		}

		public static IodineContext Create ()
		{
			return new IodineContext ();
		}

		private IodineModule LoadIodineModule (string name) 
		{
			string modulePath = FindModuleSource (name);
			if (modulePath != null) {
				SourceUnit source = SourceUnit.CreateFromFile (modulePath);
				return source.Compile (this);
			}
			return null;
		}

		private IodineModule LoadExtensionModule (string name)
		{
			string extPath = FindExtension (name);
			if (extPath != null) {
				return LoadDll (name, extPath);
			}
			return null;
		}

		private static IodineModule LoadDll (string module, string dll)
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

		private string FindModuleSource (string moduleName)
		{
			foreach (string path in SearchPath) {
				string expectedName = Path.Combine (path, moduleName + ".id");
				if (File.Exists (expectedName)) {
					return expectedName;
				}
			}
			// Module not found!
			return null;
		}

		private string FindExtension (string extensionName)
		{
			foreach (string path in SearchPath) {
				string expectedName = Path.Combine (path, extensionName + ".dll");
				if (File.Exists (expectedName)) {
					return expectedName;
				}
			}
			// Extension not found!
			return null;
		}
	}
}

