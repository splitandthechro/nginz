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
using Iodine.Runtime;
using Iodine.Compiler.Ast;

namespace Iodine.Compiler
{
	/// <summary>
	/// Represents a unit of Iodine code (Typically a file or a string of code)
	/// </summary>
	public sealed class SourceUnit
	{
		public readonly string Text;
		public readonly string Path;

		public bool HasPath {
			get { 
				return Path != null;
			}
		}

		private SourceUnit (string source, string path = null)
		{
			Text = source;
			Path = path;
		}

		public static SourceUnit CreateFromFile (string path)
		{
			return new SourceUnit (File.ReadAllText (path), 
				System.IO.Path.GetFullPath (path));
		}

		public static SourceUnit CreateFromSource (string source)
		{
			return new SourceUnit (source);
		}

		public IodineModule Compile (IodineContext context)
		{
			string moduleName = Path == null ? "__anonymous__" :
				System.IO.Path.GetFileNameWithoutExtension (Path);
			
			if (HasPath) {
				string wd = System.IO.Path.GetDirectoryName (Path);
				string depPath = System.IO.Path.Combine (wd, ".deps");

				if (!context.SearchPath.Contains (wd)) {
					context.SearchPath.Add (wd);
				}

				if (!context.SearchPath.Contains (depPath)) {
					context.SearchPath.Add (depPath);
				}
			}
			Parser parser = Parser.CreateParser (context, this);
			AstRoot root = parser.Parse ();
			IodineCompiler compiler = IodineCompiler.CreateCompiler (context, root);
			return compiler.Compile (moduleName);
		}
	}
}

