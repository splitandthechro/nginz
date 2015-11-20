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
	public class PatternCompiler : IAstVisitor
	{
		private IAstVisitor parentVisitor;
		private ErrorLog errorLog;
		private IodineMethod methodBuilder;
		private SymbolTable symbolTable;
		private int temporary;

		public PatternCompiler (ErrorLog errorLog,
			SymbolTable symbolTable,
			IodineMethod methodBuilder,
			int temporary,
			IAstVisitor parent)
		{
			parentVisitor = parent;
			this.errorLog = errorLog;
			this.methodBuilder = methodBuilder;
			this.symbolTable = symbolTable;
			this.temporary = temporary;
		}

		public void Accept (AstNode ast)
		{
			ast.Visit (parentVisitor);
		}

		public void Accept (AstRoot ast)
		{
			ast.VisitChildren (this);
		}

		public void Accept (Expression expr)
		{
		}

		public void Accept (Statement stmt)
		{
		}

		public void Accept (BinaryExpression pattern)
		{
			IodineLabel shortCircuitTrueLabel = methodBuilder.CreateLabel ();
			IodineLabel shortCircuitFalseLabel = methodBuilder.CreateLabel ();
			IodineLabel endLabel = methodBuilder.CreateLabel ();
			pattern.Left.Visit (this);

			/*
				 * Short circuit evaluation 
				 */
			switch (pattern.Operation) {
			case BinaryOperation.And:
				methodBuilder.EmitInstruction (pattern.Location, Opcode.Dup);
				methodBuilder.EmitInstruction (pattern.Location, Opcode.JumpIfFalse,
					shortCircuitFalseLabel);
				break;
			case BinaryOperation.Or:
				methodBuilder.EmitInstruction (pattern.Location, Opcode.Dup);
				methodBuilder.EmitInstruction (pattern.Location, Opcode.JumpIfTrue,
					shortCircuitTrueLabel);
				break;
			}
			pattern.Right.Visit (this);

			methodBuilder.EmitInstruction (pattern.Location, Opcode.BinOp, (int)pattern.Operation);
			methodBuilder.EmitInstruction (pattern.Location, Opcode.Jump, endLabel);
			methodBuilder.MarkLabelPosition (shortCircuitTrueLabel);
			methodBuilder.EmitInstruction (pattern.Location, Opcode.Pop);
			methodBuilder.EmitInstruction (pattern.Location, Opcode.LoadTrue);
			methodBuilder.EmitInstruction (pattern.Location, Opcode.Jump, endLabel);
			methodBuilder.MarkLabelPosition (shortCircuitFalseLabel);
			methodBuilder.EmitInstruction (pattern.Location, Opcode.Pop);
			methodBuilder.EmitInstruction (pattern.Location, Opcode.LoadFalse);
			methodBuilder.MarkLabelPosition (endLabel);
		}

		public void Accept (UnaryExpression unaryop)
		{
			unaryop.Visit (parentVisitor);
			methodBuilder.EmitInstruction (unaryop.Location, Opcode.LoadLocal, temporary);
			methodBuilder.EmitInstruction (unaryop.Location, Opcode.BinOp, (int)BinaryOperation.Equals);
		}

		public void Accept (NameExpression ident)
		{
			if (ident.Value == "_") {
				methodBuilder.EmitInstruction (ident.Location, Opcode.LoadTrue);
			} else {
				methodBuilder.EmitInstruction (ident.Location, Opcode.LoadLocal, temporary);
				methodBuilder.EmitInstruction (ident.Location, Opcode.StoreLocal,
					symbolTable.GetSymbol (ident.Value).Index);
				methodBuilder.EmitInstruction (ident.Location, Opcode.LoadTrue);
			}
		}

		public void Accept (CallExpression call)
		{
			call.Visit (parentVisitor);
			methodBuilder.EmitInstruction (call.Location, Opcode.LoadLocal, temporary);
			methodBuilder.EmitInstruction (call.Location, Opcode.BinOp, (int)BinaryOperation.Equals);
		}

		public void Accept (ArgumentList arglist)
		{
			arglist.Visit (parentVisitor);
		}

		public void Accept (KeywordArgumentList kwargs)
		{
			kwargs.Visit (parentVisitor);
		}

		public void Accept (GetExpression getAttr)
		{
			getAttr.Visit (parentVisitor);
			methodBuilder.EmitInstruction (getAttr.Location, Opcode.LoadLocal, temporary);
			methodBuilder.EmitInstruction (getAttr.Location, Opcode.BinOp, (int)BinaryOperation.Equals);
		}

		public void Accept (IntegerExpression integer)
		{
			integer.Visit (parentVisitor);
			methodBuilder.EmitInstruction (integer.Location, Opcode.LoadLocal, temporary);
			methodBuilder.EmitInstruction (integer.Location, Opcode.BinOp, (int)BinaryOperation.Equals);
		}

		public void Accept (FloatExpression num)
		{
			num.Visit (parentVisitor);
			methodBuilder.EmitInstruction (num.Location, Opcode.LoadLocal, temporary);
			methodBuilder.EmitInstruction (num.Location, Opcode.BinOp, (int)BinaryOperation.Equals);
		}

		public void Accept (IfStatement ifStmt)
		{
		}

		public void Accept (WhileStatement whileStmt)
		{
		}

		public void Accept (WithStatement withStmt)
		{
		}

		public void Accept (DoStatement doStmt)
		{
		}

		public void Accept (ForStatement forStmt)
		{
		}

		public void Accept (ForeachStatement foreachStmt)
		{
		}

		public void Accept (GivenStatement switchStmt)
		{
		}

		public void Accept (WhenStatement caseStmt)
		{
		}

		public void Accept (FunctionDeclaration funcDecl)
		{
		}

		public void Accept (CodeBlock scope)
		{
		}

		public void Accept (StringExpression str)
		{
			str.Visit (parentVisitor);
			methodBuilder.EmitInstruction (str.Location, Opcode.LoadLocal, temporary);
			methodBuilder.EmitInstruction (str.Location, Opcode.BinOp, (int)BinaryOperation.Equals);
		}

		public void Accept (UseStatement useStmt)
		{

		}

		public void Accept (ClassDeclaration classDecl)
		{
		}

		public void Accept (InterfaceDeclaration contractDecl)
		{
		}

		public void Accept (EnumDeclaration enumDecl)
		{
		}

		public void Accept (ReturnStatement returnStmt)
		{
		}

		public void Accept (YieldStatement yieldStmt)
		{
		}

		public void Accept (IndexerExpression indexer)
		{
		}

		public void Accept (ListExpression list)
		{
		}

		public void Accept (HashExpression hash)
		{
		}

		public void Accept (SelfStatement self)
		{
			self.Visit (parentVisitor);
		}

		public void Accept (TrueExpression ntrue)
		{
			ntrue.Visit (parentVisitor);
			methodBuilder.EmitInstruction (ntrue.Location, Opcode.LoadLocal, temporary);
			methodBuilder.EmitInstruction (ntrue.Location, Opcode.BinOp, (int)BinaryOperation.Equals);
		}

		public void Accept (FalseExpression nfalse)
		{
			nfalse.Visit (parentVisitor);
			methodBuilder.EmitInstruction (nfalse.Location, Opcode.LoadLocal, temporary);
			methodBuilder.EmitInstruction (nfalse.Location, Opcode.BinOp, (int)BinaryOperation.Equals);
		}

		public void Accept (NullExpression nil)
		{
			nil.Visit (parentVisitor);
			methodBuilder.EmitInstruction (nil.Location, Opcode.LoadLocal, temporary);
			methodBuilder.EmitInstruction (nil.Location, Opcode.BinOp, (int)BinaryOperation.Equals);
		}

		public void Accept (LambdaExpression lambda)
		{
			lambda.Visit (parentVisitor);
		}


		public void Accept (TryExceptStatement tryExcept)
		{
		}

		public void Accept (RaiseStatement raise)
		{
		}

		public void Accept (TupleExpression tuple)
		{
			IodineLabel startLabel = methodBuilder.CreateLabel ();
			IodineLabel endLabel = methodBuilder.CreateLabel ();
			int item = methodBuilder.CreateTemporary ();

			PatternCompiler compiler = new PatternCompiler (errorLog, symbolTable, methodBuilder,
				item,
				parentVisitor);
			
			for (int i = 0; i < tuple.Children.Count; i++) {
				if (tuple.Children [i] is NameExpression &&
					((NameExpression)tuple.Children [i]).Value == "_")
					continue;
				methodBuilder.EmitInstruction (tuple.Location, Opcode.LoadLocal, temporary);
				methodBuilder.EmitInstruction (tuple.Location, Opcode.LoadConst,
					methodBuilder.Module.DefineConstant (new IodineInteger (i)));
				methodBuilder.EmitInstruction (tuple.Location, Opcode.LoadIndex);
				methodBuilder.EmitInstruction (tuple.Location, Opcode.StoreLocal, item);
				tuple.Children [i].Visit (compiler);
				methodBuilder.EmitInstruction (tuple.Location, Opcode.JumpIfFalse, endLabel);
			}
			methodBuilder.EmitInstruction (tuple.Location, Opcode.LoadTrue);
			methodBuilder.EmitInstruction (tuple.Location, Opcode.Jump, startLabel);

			methodBuilder.MarkLabelPosition (endLabel);
			methodBuilder.EmitInstruction (tuple.Location, Opcode.LoadFalse);

			methodBuilder.MarkLabelPosition (startLabel);
		}

		public void Accept (SuperCallExpression super)
		{
		}

		public void Accept (BreakStatement brk)
		{
		}

		public void Accept (ContinueStatement cont)
		{
		}

		public void Accept (MatchExpression match)
		{
		}

		public void Accept (CaseExpression caseExpr)
		{
		}

		public void Accept (ListCompExpression list)
		{
		}

		public void Accept (TernaryExpression ifExpr)
		{
		}
	}
}

