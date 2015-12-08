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
using System.Collections.Generic;
using Iodine.Compiler.Ast;
using Iodine.Runtime;

namespace Iodine.Compiler
{
	public class IodineCompiler
	{
		private static List<IBytecodeOptimization> Optimizations = new List<IBytecodeOptimization> ();

		static IodineCompiler ()
		{
			Optimizations.Add (new ControlFlowOptimization ());
			Optimizations.Add (new InstructionOptimization ());
		}

		private IodineContext context;
		private SymbolTable symbolTable;
		private AstRoot root;

		private IodineCompiler (IodineContext context, SymbolTable symbolTable, AstRoot root)
		{
			this.context = context;
			this.symbolTable = symbolTable;
			this.root = root;
		}

		public static IodineCompiler CreateCompiler (IodineContext context, AstRoot root)
		{
			SemanticAnalyser analyser = new SemanticAnalyser (context.ErrorLog);
			SymbolTable table = analyser.Analyse (root);
			return new IodineCompiler (context, table, root);
		}

		public IodineModule Compile (string moduleName)
		{
			IodineModule module = new IodineModule (moduleName);

			ModuleCompiler compiler = new ModuleCompiler (symbolTable, module);
			root.Visit (compiler);
			module.Initializer.FinalizeLabels ();
			if (context.ShouldOptimize) {
				OptimizeObject (module);
			}
			return module;
		}

		private void OptimizeObject (IodineObject obj)
		{
			foreach (IodineObject attr in obj.Attributes.Values) {
				if (attr is IodineMethod) {
					IodineMethod method = attr as IodineMethod;
					foreach (IBytecodeOptimization opt in Optimizations) {
						opt.PerformOptimization (method);
					}
				}
			}
		}
	}
}

