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
using System.Linq;
using System.Collections.Generic;
using Iodine.Compiler;
using Iodine.Compiler.Ast;
using Iodine.Runtime;

namespace Iodine.Compiler
{
	/// <summary>
	/// Reponsible for compiling all code inside a function body
	/// </summary>
	class FunctionCompiler : IodineAstVisitor
	{
		private SymbolTable symbolTable;
		private IodineMethod methodBuilder;
		private Stack<IodineLabel> breakLabels = new Stack<IodineLabel> ();
		private Stack<IodineLabel> continueLabels = new Stack<IodineLabel> ();

		public FunctionCompiler (SymbolTable symbolTable, IodineMethod methodBuilder)
		{
			this.symbolTable = symbolTable;
			this.methodBuilder = methodBuilder;
		}

		public FunctionCompiler (SymbolTable symbolTable, IodineMethod methodBuilder,
		                         Stack<IodineLabel> breakLabels, Stack<IodineLabel> continueLabels)
		{
			this.symbolTable = symbolTable;
			this.methodBuilder = methodBuilder;
			this.breakLabels = breakLabels;
			this.continueLabels = continueLabels;
		}

		public void Accept (AstNode ast)
		{
			ast.VisitChildren (this);
		}

		public override void Accept (AstRoot ast)
		{
			ast.VisitChildren (this);
		}

		public override void Accept (Expression expr)
		{
			expr.VisitChildren (this);
			methodBuilder.EmitInstruction (expr.Location, Opcode.Pop);
		}

		public override void Accept (Statement stmt)
		{
			stmt.VisitChildren (this);
		}

		public override void Accept (BinaryExpression binop)
		{
			if (binop.Operation == BinaryOperation.Assign) {
				binop.Right.Visit (this);
				if (binop.Left is NameExpression) {
					NameExpression ident = (NameExpression)binop.Left;
					Symbol sym = symbolTable.GetSymbol (ident.Value);
					if (sym.Type == SymbolType.Local) {
						methodBuilder.EmitInstruction (ident.Location, Opcode.StoreLocal, sym.Index);
						methodBuilder.EmitInstruction (ident.Location, Opcode.LoadLocal, sym.Index);
					} else {
						int globalIndex = methodBuilder.Module.DefineConstant (new IodineName (ident.Value));
						methodBuilder.EmitInstruction (ident.Location, Opcode.StoreGlobal, globalIndex);
						methodBuilder.EmitInstruction (ident.Location, Opcode.LoadGlobal, globalIndex);
					}
				} else if (binop.Left is GetExpression) {
					GetExpression getattr = binop.Left as GetExpression;
					getattr.Target.Visit (this);
					int attrIndex = methodBuilder.Module.DefineConstant (new IodineName (getattr.Field));
					methodBuilder.EmitInstruction (getattr.Location, Opcode.StoreAttribute, attrIndex);
					getattr.Target.Visit (this);
					methodBuilder.EmitInstruction (getattr.Location, Opcode.LoadAttribute, attrIndex);
				} else if (binop.Left is IndexerExpression) {
					IndexerExpression indexer = binop.Left as IndexerExpression;
					indexer.Target.Visit (this);
					indexer.Index.Visit (this);
					methodBuilder.EmitInstruction (indexer.Location, Opcode.StoreIndex);
					binop.Left.Visit (this);
				}
			} else if (binop.Operation == BinaryOperation.InstanceOf) {
				binop.Right.Visit (this);
				binop.Left.Visit (this);
				methodBuilder.EmitInstruction (binop.Location, Opcode.InstanceOf);
			} else if (binop.Operation == BinaryOperation.NotInstanceOf) {
				binop.Right.Visit (this);
				binop.Left.Visit (this);
				methodBuilder.EmitInstruction (binop.Location, Opcode.InstanceOf);
				methodBuilder.EmitInstruction (binop.Location, Opcode.UnaryOp, (int)UnaryOperation.BoolNot);
			}  else if (binop.Operation == BinaryOperation.DynamicCast) {
				binop.Right.Visit (this);
				binop.Left.Visit (this);
				methodBuilder.EmitInstruction (binop.Location, Opcode.DynamicCast);
			}  else if (binop.Operation == BinaryOperation.NullCoalescing) {
				binop.Right.Visit (this);
				binop.Left.Visit (this);
				methodBuilder.EmitInstruction (binop.Location, Opcode.NullCoalesce);
			} else {
				IodineLabel shortCircuitTrueLabel = methodBuilder.CreateLabel ();
				IodineLabel shortCircuitFalseLabel = methodBuilder.CreateLabel ();
				IodineLabel endLabel = methodBuilder.CreateLabel ();
				binop.Left.Visit (this);
				/*
				 * Short circuit evaluation 
				 */
				switch (binop.Operation) {
				case BinaryOperation.BoolAnd:
					methodBuilder.EmitInstruction (binop.Location, Opcode.Dup);
					methodBuilder.EmitInstruction (binop.Location, Opcode.JumpIfFalse,
						shortCircuitFalseLabel);
					break;
				case BinaryOperation.BoolOr:
					methodBuilder.EmitInstruction (binop.Location, Opcode.Dup);
					methodBuilder.EmitInstruction (binop.Location, Opcode.JumpIfTrue,
						shortCircuitTrueLabel);
					break;
				}
				binop.Right.Visit (this);
				methodBuilder.EmitInstruction (binop.Location, Opcode.BinOp, (int)binop.Operation);
				methodBuilder.EmitInstruction (binop.Location, Opcode.Jump, endLabel);
				methodBuilder.MarkLabelPosition (shortCircuitTrueLabel);
				methodBuilder.EmitInstruction (binop.Location, Opcode.Pop);
				methodBuilder.EmitInstruction (binop.Location, Opcode.LoadTrue);
				methodBuilder.EmitInstruction (binop.Location, Opcode.Jump, endLabel);
				methodBuilder.MarkLabelPosition (shortCircuitFalseLabel);
				methodBuilder.EmitInstruction (binop.Location, Opcode.Pop);
				methodBuilder.EmitInstruction (binop.Location, Opcode.LoadFalse);
				methodBuilder.MarkLabelPosition (endLabel);
			}
		}

		public override void Accept (UnaryExpression unaryop)
		{
			unaryop.VisitChildren (this);
			methodBuilder.EmitInstruction (unaryop.Location, Opcode.UnaryOp, (int)unaryop.Operation);
		}

		public override void Accept (NameExpression ident)
		{
			if (symbolTable.IsSymbolDefined (ident.Value)) {
				Symbol sym = symbolTable.GetSymbol (ident.Value);
				if (sym.Type == SymbolType.Local) {
					methodBuilder.EmitInstruction (ident.Location, Opcode.LoadLocal, sym.Index);
				} else {
					methodBuilder.EmitInstruction (ident.Location, Opcode.LoadGlobal,
						methodBuilder.Module.DefineConstant (new IodineName (ident.Value)));
				}
			} else {
				methodBuilder.EmitInstruction (ident.Location, Opcode.LoadGlobal,
					methodBuilder.Module.DefineConstant (new IodineName (ident.Value)));
			}
		}

		public override void Accept (CallExpression call)
		{
			call.Arguments.Visit (this);
			call.Target.Visit (this);
			if (call.Arguments.Packed) {
				methodBuilder.EmitInstruction (call.Target.Location, Opcode.InvokeVar, 
					call.Arguments.Children.Count - 1);
			} else {
				methodBuilder.EmitInstruction (call.Target.Location, Opcode.Invoke, 
					call.Arguments.Children.Count);
			}
			
		}

		public override void Accept (ArgumentList arglist)
		{
			arglist.VisitChildren (this);
		}

		public override void Accept (KeywordArgumentList kwargs)
		{
			for (int i = 0; i < kwargs.Keywords.Count; i++) {
				string kw = kwargs.Keywords [i];
				AstNode val = kwargs.Children [i];
				methodBuilder.EmitInstruction (kwargs.Location, Opcode.LoadConst, methodBuilder.Module.DefineConstant (
					new IodineString (kw)));
				val.Visit (this);
				methodBuilder.EmitInstruction (kwargs.Location, Opcode.BuildTuple, 2);
			}
			methodBuilder.EmitInstruction (kwargs.Location, Opcode.BuildList, kwargs.Keywords.Count);
			methodBuilder.EmitInstruction (kwargs.Location, Opcode.LoadGlobal, methodBuilder.Module.DefineConstant (
				new IodineName ("HashMap")));
			methodBuilder.EmitInstruction (kwargs.Location, Opcode.Invoke, 1);
		}

		public override void Accept (GetExpression getAttr)
		{
			getAttr.Target.Visit (this);
			methodBuilder.EmitInstruction (getAttr.Location, Opcode.LoadAttribute,
				methodBuilder.Module.DefineConstant (new IodineName (getAttr.Field)));
		}

		public override void Accept (IntegerExpression integer)
		{
			methodBuilder.EmitInstruction (integer.Location, Opcode.LoadConst, 
				methodBuilder.Module.DefineConstant (new IodineInteger (integer.Value)));
		}

		public override void Accept (FloatExpression num)
		{
			methodBuilder.EmitInstruction (num.Location, Opcode.LoadConst, 
				methodBuilder.Module.DefineConstant (new IodineFloat (num.Value)));
		}

		public override void Accept (IfStatement ifStmt)
		{
			IodineLabel elseLabel = methodBuilder.CreateLabel ();
			IodineLabel endLabel = methodBuilder.CreateLabel ();
			ifStmt.Condition.Visit (this);
			methodBuilder.EmitInstruction (ifStmt.Body.Location, Opcode.JumpIfFalse, elseLabel);
			ifStmt.Body.Visit (this);
			methodBuilder.EmitInstruction (ifStmt.ElseBody.Location, Opcode.Jump, endLabel);
			methodBuilder.MarkLabelPosition (elseLabel);
			ifStmt.ElseBody.Visit (this);
			methodBuilder.MarkLabelPosition (endLabel);
		}

		public override void Accept (WhileStatement whileStmt)
		{
			IodineLabel whileLabel = methodBuilder.CreateLabel ();
			IodineLabel breakLabel = methodBuilder.CreateLabel ();
			breakLabels.Push (breakLabel);
			continueLabels.Push (whileLabel);
			methodBuilder.MarkLabelPosition (whileLabel);
			whileStmt.Condition.Visit (this);
			methodBuilder.EmitInstruction (whileStmt.Condition.Location, Opcode.JumpIfFalse,
				breakLabel);
			whileStmt.Body.Visit (this);
			methodBuilder.EmitInstruction (whileStmt.Body.Location, Opcode.Jump, whileLabel);
			methodBuilder.MarkLabelPosition (breakLabel);
			breakLabels.Pop ();
			continueLabels.Pop ();
		}

		public override void Accept (WithStatement withStmt)
		{
			symbolTable.NextScope ();
			withStmt.Expression.Visit (this);
			methodBuilder.EmitInstruction (withStmt.Location, Opcode.BeginWith);
			withStmt.Body.Visit (this);
			methodBuilder.EmitInstruction (withStmt.Location, Opcode.EndWith);
			symbolTable.LeaveScope ();
		}

		public override void Accept (DoStatement doStmt)
		{
			IodineLabel doLabel = methodBuilder.CreateLabel ();
			IodineLabel breakLabel = methodBuilder.CreateLabel ();
			breakLabels.Push (breakLabel);
			continueLabels.Push (doLabel);
			methodBuilder.MarkLabelPosition (doLabel);
			doStmt.Body.Visit (this);
			doStmt.Condition.Visit (this);
			methodBuilder.EmitInstruction (doStmt.Condition.Location, Opcode.JumpIfTrue,
				doLabel);
			methodBuilder.MarkLabelPosition (breakLabel);
			breakLabels.Pop ();
			continueLabels.Pop ();
		}

		public override void Accept (ForStatement forStmt)
		{
			IodineLabel forLabel = methodBuilder.CreateLabel ();
			IodineLabel breakLabel = methodBuilder.CreateLabel ();
			IodineLabel skipAfterThought = methodBuilder.CreateLabel ();
			breakLabels.Push (breakLabel);
			continueLabels.Push (forLabel);
			forStmt.Initializer.Visit (this);
			methodBuilder.EmitInstruction (forStmt.Location, Opcode.Jump, skipAfterThought);
			methodBuilder.MarkLabelPosition (forLabel);
			forStmt.AfterThought.Visit (this);
			methodBuilder.MarkLabelPosition (skipAfterThought);
			forStmt.Condition.Visit (this);
			methodBuilder.EmitInstruction (forStmt.Condition.Location, Opcode.JumpIfFalse, breakLabel);
			forStmt.Body.Visit (this);
			forStmt.AfterThought.Visit (this);
			methodBuilder.EmitInstruction (forStmt.AfterThought.Location, Opcode.Jump, skipAfterThought);
			methodBuilder.MarkLabelPosition (breakLabel);
			breakLabels.Pop ();
			continueLabels.Pop ();
		}

		public override void Accept (ForeachStatement foreachStmt)
		{
			IodineLabel foreachLabel = methodBuilder.CreateLabel ();
			IodineLabel breakLabel = methodBuilder.CreateLabel ();
			breakLabels.Push (breakLabel);
			continueLabels.Push (foreachLabel);
			foreachStmt.Iterator.Visit (this);
			int tmp = methodBuilder.CreateTemporary (); 
			methodBuilder.EmitInstruction (foreachStmt.Iterator.Location, Opcode.Dup);
			methodBuilder.EmitInstruction (foreachStmt.Iterator.Location, Opcode.StoreLocal, tmp);
			methodBuilder.EmitInstruction (foreachStmt.Iterator.Location, Opcode.IterReset);
			methodBuilder.MarkLabelPosition (foreachLabel);
			methodBuilder.EmitInstruction (foreachStmt.Iterator.Location, Opcode.LoadLocal, tmp);
			methodBuilder.EmitInstruction (foreachStmt.Iterator.Location, Opcode.IterMoveNext);
			methodBuilder.EmitInstruction (foreachStmt.Iterator.Location, Opcode.JumpIfFalse,
				breakLabel);
			methodBuilder.EmitInstruction (foreachStmt.Iterator.Location, Opcode.LoadLocal, tmp);
			methodBuilder.EmitInstruction (foreachStmt.Iterator.Location, Opcode.IterGetNext);
			methodBuilder.EmitInstruction (foreachStmt.Iterator.Location, Opcode.StoreLocal,
				symbolTable.GetSymbol
				(foreachStmt.Item).Index);
			foreachStmt.Body.Visit (this);
			methodBuilder.EmitInstruction (foreachStmt.Body.Location, Opcode.Jump, foreachLabel);
			methodBuilder.MarkLabelPosition (breakLabel);
			breakLabels.Pop ();
			continueLabels.Pop ();
		}

		public override void Accept (GivenStatement switchStmt)
		{
			foreach (AstNode node in switchStmt.WhenStatements.Children) {
				WhenStatement caseStmt = node as WhenStatement;
				caseStmt.Values.Visit (this);
				caseStmt.Body.Visit (this);
			}
			switchStmt.GivenValue.Visit (this);
			methodBuilder.EmitInstruction (switchStmt.Location, Opcode.SwitchLookup, switchStmt.WhenStatements.Children.Count);
			IodineLabel endLabel = methodBuilder.CreateLabel ();
			methodBuilder.EmitInstruction (switchStmt.Location, Opcode.JumpIfTrue, endLabel);
			switchStmt.DefaultStatement.Visit (this);
			methodBuilder.MarkLabelPosition (endLabel);
		}

		public override void Accept (WhenStatement caseStmt)
		{
		}

		public override void Accept (FunctionDeclaration funcDecl)
		{
			symbolTable.NextScope ();
			IodineMethod anonMethod = new IodineMethod (methodBuilder, methodBuilder.Module, null, funcDecl.InstanceMethod, 
				                          funcDecl.Parameters.Count, methodBuilder.LocalCount);
			FunctionCompiler compiler = new FunctionCompiler (symbolTable, anonMethod);
			for (int i = 0; i < funcDecl.Parameters.Count; i++) {
				anonMethod.Parameters [funcDecl.Parameters [i]] = symbolTable.GetSymbol
					(funcDecl.Parameters [i]).Index;
			}
			funcDecl.Children [0].Visit (compiler);
			anonMethod.EmitInstruction (funcDecl.Location, Opcode.LoadNull);
			anonMethod.Variadic = funcDecl.Variadic;
			anonMethod.AcceptsKeywordArgs = funcDecl.AcceptsKeywordArgs;
			anonMethod.FinalizeLabels ();
			methodBuilder.EmitInstruction (funcDecl.Location, Opcode.LoadConst,
				methodBuilder.Module.DefineConstant (anonMethod));
			methodBuilder.EmitInstruction (funcDecl.Location, Opcode.BuildClosure);
			methodBuilder.EmitInstruction (funcDecl.Location, Opcode.StoreLocal, symbolTable.GetSymbol (funcDecl.Name).Index);
			symbolTable.LeaveScope ();
		}

		public override void Accept (CodeBlock scope)
		{
			symbolTable.NextScope ();

			FunctionCompiler scopeCompiler = new FunctionCompiler (symbolTable, methodBuilder,
				                                 breakLabels, continueLabels);
			foreach (AstNode node in scope) {
				node.Visit (scopeCompiler);
			}
			symbolTable.LeaveScope ();
		}

		public override void Accept (StringExpression str)
		{
			str.VisitChildren (this);
			methodBuilder.EmitInstruction (str.Location, Opcode.LoadConst, 
				methodBuilder.Module.DefineConstant (new IodineString (str.Value)));
			if (str.Children.Count != 0) {
				methodBuilder.EmitInstruction (str.Location, Opcode.LoadAttribute,
					methodBuilder.Module.DefineConstant (new IodineName ("format")));
				methodBuilder.EmitInstruction (str.Location, Opcode.Invoke, str.Children.Count);
			}
		}

		public override void Accept (UseStatement useStmt)
		{
			
		}

		public override void Accept (ClassDeclaration classDecl)
		{
			ModuleCompiler compiler = new ModuleCompiler (symbolTable, methodBuilder.Module);
			IodineClass clazz = compiler.CompileClass (classDecl);
			methodBuilder.EmitInstruction (classDecl.Location, Opcode.LoadConst,
				methodBuilder.Module.DefineConstant (clazz));
			methodBuilder.EmitInstruction (classDecl.Location, Opcode.StoreLocal,
				symbolTable.GetSymbol (classDecl.Name).Index);
		}

		public override void Accept (InterfaceDeclaration contractDecl)
		{
			IodineInterface contract = new IodineInterface (contractDecl.Name);
			foreach (AstNode node in contractDecl.Children) {
				FunctionDeclaration decl = node as FunctionDeclaration;
				contract.AddMethod (new IodineMethod (methodBuilder.Module, decl.Name, decl.InstanceMethod,
					decl.Parameters.Count, 0));
			}
			methodBuilder.EmitInstruction (contractDecl.Location, Opcode.LoadConst,
				methodBuilder.Module.DefineConstant (contract));
			methodBuilder.EmitInstruction (contractDecl.Location, Opcode.StoreLocal,
				symbolTable.GetSymbol (contractDecl.Name).Index);
		}

		public override void Accept (EnumDeclaration enumDecl)
		{
			IodineEnum ienum = new IodineEnum (enumDecl.Name);
			foreach (string name in enumDecl.Items.Keys) {
				ienum.AddItem (name, enumDecl.Items [name]);
			}
			methodBuilder.EmitInstruction (enumDecl.Location, Opcode.LoadConst,
				methodBuilder.Module.DefineConstant (ienum));
			methodBuilder.EmitInstruction (enumDecl.Location, Opcode.StoreLocal,
				symbolTable.GetSymbol (enumDecl.Name).Index);
		}

		public override void Accept (ReturnStatement returnStmt)
		{
			returnStmt.VisitChildren (this);
			methodBuilder.EmitInstruction (returnStmt.Location, Opcode.Return);
		}

		public override void Accept (YieldStatement yieldStmt)
		{
			yieldStmt.VisitChildren (this);
			methodBuilder.Generator = true;
			methodBuilder.EmitInstruction (yieldStmt.Location, Opcode.Yield);
		}

		public override void Accept (IndexerExpression indexer)
		{
			indexer.Target.Visit (this);
			indexer.Index.Visit (this);
			methodBuilder.EmitInstruction (indexer.Location, Opcode.LoadIndex);
		}

		public override void Accept (ListExpression list)
		{
			list.VisitChildren (this);
			methodBuilder.EmitInstruction (list.Location, Opcode.BuildList, list.Children.Count);
		}

		public override void Accept (HashExpression hash)
		{
			hash.VisitChildren (this);
			methodBuilder.EmitInstruction (hash.Location, Opcode.BuildHash, hash.Children.Count / 2);
		}

		public override void Accept (SelfStatement self)
		{
			methodBuilder.EmitInstruction (self.Location, Opcode.LoadSelf);
		}

		public override void Accept (TrueExpression ntrue)
		{
			methodBuilder.EmitInstruction (ntrue.Location, Opcode.LoadTrue);
		}

		public override void Accept (FalseExpression nfalse)
		{
			methodBuilder.EmitInstruction (nfalse.Location, Opcode.LoadFalse);
		}

		public override void Accept (NullExpression nil)
		{
			methodBuilder.EmitInstruction (nil.Location, Opcode.LoadNull);
		}

		public override void Accept (LambdaExpression lambda)
		{
			symbolTable.NextScope ();

			int locals = methodBuilder.LocalCount > 0 ? methodBuilder.LocalCount : symbolTable.CurrentScope.SymbolCount;
			IodineMethod anonMethod = new IodineMethod (methodBuilder, methodBuilder.Module, null, lambda.InstanceMethod, 
				                          lambda.Parameters.Count, locals);
			FunctionCompiler compiler = new FunctionCompiler (symbolTable, anonMethod);
			for (int i = 0; i < lambda.Parameters.Count; i++) {
				anonMethod.Parameters [lambda.Parameters [i]] = symbolTable.GetSymbol
					(lambda.Parameters [i]).Index;
			}
			lambda.Children [0].Visit (compiler);
			anonMethod.EmitInstruction (lambda.Location, Opcode.LoadNull);
			anonMethod.Variadic = lambda.Variadic;
			anonMethod.FinalizeLabels ();
			methodBuilder.EmitInstruction (lambda.Location, Opcode.LoadConst,
				methodBuilder.Module.DefineConstant (anonMethod));
			if (methodBuilder.LocalCount > 0) {
				methodBuilder.EmitInstruction (lambda.Location, Opcode.BuildClosure);
			}
			symbolTable.LeaveScope ();
		}


		public override void Accept (TryExceptStatement tryExcept)
		{
			IodineLabel exceptLabel = methodBuilder.CreateLabel ();
			IodineLabel endLabel = methodBuilder.CreateLabel ();
			methodBuilder.EmitInstruction (tryExcept.Location, Opcode.PushExceptionHandler, exceptLabel);
			tryExcept.TryBody.Visit (this);
			methodBuilder.EmitInstruction (tryExcept.TryBody.Location, Opcode.PopExceptionHandler);
			methodBuilder.EmitInstruction (tryExcept.TryBody.Location, Opcode.Jump, endLabel);
			methodBuilder.MarkLabelPosition (exceptLabel);
			tryExcept.TypeList.Visit (this);
			if (tryExcept.TypeList.Children.Count > 0) {
				methodBuilder.EmitInstruction (tryExcept.ExceptBody.Location, Opcode.BeginExcept,
					tryExcept.TypeList.Children.Count);
			}
			if (tryExcept.ExceptionIdentifier != null) {
				methodBuilder.EmitInstruction (tryExcept.ExceptBody.Location, Opcode.LoadException);
				methodBuilder.EmitInstruction (tryExcept.ExceptBody.Location, Opcode.StoreLocal,
					symbolTable.GetSymbol (tryExcept.ExceptionIdentifier).Index);
			}
			tryExcept.ExceptBody.Visit (this);
			methodBuilder.MarkLabelPosition (endLabel);
		}

		public override void Accept (RaiseStatement raise)
		{
			raise.Value.Visit (this);
			methodBuilder.EmitInstruction (raise.Location, Opcode.Raise);
		}

		public override void Accept (TupleExpression tuple)
		{
			tuple.VisitChildren (this);
			methodBuilder.EmitInstruction (tuple.Location, Opcode.BuildTuple, tuple.Children.Count);
		}

		public override void Accept (SuperCallExpression super)
		{
			string[] subclass = super.Parent.Base [0].Split ('.');
			super.Arguments.Visit (this);
			methodBuilder.EmitInstruction (super.Location, Opcode.LoadGlobal,
				methodBuilder.Module.DefineConstant (new IodineName (subclass [0])));
			for (int i = 1; i < subclass.Length; i++) {
				methodBuilder.EmitInstruction (super.Location, Opcode.LoadAttribute,
					methodBuilder.Module.DefineConstant (new IodineName (subclass [0])));
			}
			methodBuilder.EmitInstruction (super.Location, Opcode.InvokeSuper,
				super.Arguments.Children.Count);
			for (int i = 1; i < super.Parent.Base.Count; i++) {
				string[] contract = super.Parent.Base [i].Split ('.');
				methodBuilder.EmitInstruction (super.Location, Opcode.LoadGlobal,
					methodBuilder.Module.DefineConstant (new IodineName (contract [0])));
				for (int j = 1; j < contract.Length; j++) {
					methodBuilder.EmitInstruction (super.Location, Opcode.LoadAttribute,
						methodBuilder.Module.DefineConstant (new IodineName (contract [0])));
				}
				methodBuilder.EmitInstruction (super.Location, Opcode.InvokeSuper, 0);
			}
		}

		public override void Accept (BreakStatement brk)
		{
			methodBuilder.EmitInstruction (brk.Location, Opcode.Jump, breakLabels.Peek ());
		}

		public override void Accept (ContinueStatement cont)
		{
			methodBuilder.EmitInstruction (cont.Location, Opcode.Jump, continueLabels.Peek ());
		}

		public override void Accept (MatchExpression match)
		{
			AstNode value = match.Children [0];
			value.Visit (this);
			int temporary = methodBuilder.CreateTemporary ();
			methodBuilder.EmitInstruction (match.Location, Opcode.StoreLocal, temporary);
			PatternCompiler compiler = new PatternCompiler (symbolTable, methodBuilder,
				temporary,
				this);
			IodineLabel nextLabel = methodBuilder.CreateLabel ();
			IodineLabel endLabel = methodBuilder.CreateLabel ();
			for (int i = 1; i < match.Children.Count; i++) {
				if (i > 1) {
					methodBuilder.MarkLabelPosition (nextLabel);
					nextLabel = methodBuilder.CreateLabel ();
				}
				CaseExpression clause = match.Children [i] as CaseExpression;
				clause.Pattern.Visit (compiler);
				methodBuilder.EmitInstruction (match.Location, Opcode.JumpIfFalse, nextLabel);
				if (clause.Condition != null) {
					clause.Condition.Visit (this);
					methodBuilder.EmitInstruction (match.Location, Opcode.JumpIfFalse, nextLabel);
				}
				clause.Value.Visit (this);
				methodBuilder.EmitInstruction (match.Location, Opcode.Jump, endLabel);
			}
			methodBuilder.MarkLabelPosition (endLabel);
		}


		public override void Accept (ListCompExpression list)
		{
			IodineLabel foreachLabel = methodBuilder.CreateLabel ();
			IodineLabel breakLabel = methodBuilder.CreateLabel ();
			IodineLabel predicateSkip = methodBuilder.CreateLabel ();
			int tmp = methodBuilder.CreateTemporary (); 
			int set = methodBuilder.CreateTemporary ();
			methodBuilder.EmitInstruction (list.Iterator.Location, Opcode.BuildList, 0);
			methodBuilder.EmitInstruction (list.Iterator.Location, Opcode.StoreLocal, set);
			symbolTable.NextScope ();
			list.Iterator.Visit (this);
			methodBuilder.EmitInstruction (list.Iterator.Location, Opcode.Dup);
			methodBuilder.EmitInstruction (list.Iterator.Location, Opcode.StoreLocal, tmp);
			methodBuilder.EmitInstruction (list.Iterator.Location, Opcode.IterReset);
			methodBuilder.MarkLabelPosition (foreachLabel);
			methodBuilder.EmitInstruction (list.Iterator.Location, Opcode.LoadLocal, tmp);
			methodBuilder.EmitInstruction (list.Iterator.Location, Opcode.IterMoveNext);
			methodBuilder.EmitInstruction (list.Iterator.Location, Opcode.JumpIfFalse,
				breakLabel);
			methodBuilder.EmitInstruction (list.Iterator.Location, Opcode.LoadLocal, tmp);
			methodBuilder.EmitInstruction (list.Iterator.Location, Opcode.IterGetNext);
			methodBuilder.EmitInstruction (list.Iterator.Location, Opcode.StoreLocal,
				symbolTable.GetSymbol
				(list.Identifier).Index);
			if (list.Predicate != null) {
				list.Predicate.Visit (this);
				methodBuilder.EmitInstruction (list.Iterator.Location, Opcode.JumpIfFalse, predicateSkip);
			}
			list.Expression.Visit (this);
			methodBuilder.EmitInstruction (list.Iterator.Location, Opcode.LoadLocal, set);
			methodBuilder.EmitInstruction (list.Iterator.Location, Opcode.LoadAttribute,
				methodBuilder.Module.DefineConstant (new IodineName ("add")));
			methodBuilder.EmitInstruction (list.Iterator.Location, Opcode.Invoke, 1);
			methodBuilder.EmitInstruction (list.Iterator.Location, Opcode.Pop);
			if (list.Predicate != null) {
				methodBuilder.MarkLabelPosition (predicateSkip);
			}
			methodBuilder.EmitInstruction (list.Expression.Location, Opcode.Jump, foreachLabel);
			methodBuilder.MarkLabelPosition (breakLabel);
			methodBuilder.EmitInstruction (list.Iterator.Location, Opcode.LoadLocal, set);
			symbolTable.LeaveScope ();
		}

		public override void Accept (TernaryExpression ifExpr)
		{
			IodineLabel elseLabel = methodBuilder.CreateLabel ();
			IodineLabel endLabel = methodBuilder.CreateLabel ();
			ifExpr.Condition.Visit (this);
			methodBuilder.EmitInstruction (ifExpr.Expression.Location, Opcode.JumpIfFalse, elseLabel);
			ifExpr.Expression.Visit (this);
			methodBuilder.EmitInstruction (ifExpr.ElseExpression.Location, Opcode.Jump, endLabel);
			methodBuilder.MarkLabelPosition (elseLabel);
			ifExpr.ElseExpression.Visit (this);
			methodBuilder.MarkLabelPosition (endLabel);
		}

		public override void Accept (CaseExpression caseExpr)
		{
		}
	}
}

