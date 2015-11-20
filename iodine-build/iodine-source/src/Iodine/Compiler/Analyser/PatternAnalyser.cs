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
	public sealed class PatternAnalyzer : IAstVisitor
	{
		private ErrorLog errorLog;
		private SymbolTable symbolTable;
		private IAstVisitor parentVisitor;

		public PatternAnalyzer (ErrorLog errorLog, SymbolTable symbolTable, IAstVisitor parent)
		{
			parentVisitor = parent;
			this.symbolTable = symbolTable;
			this.errorLog = errorLog;
		}

		public void Accept (AstNode ast)
		{
			ast.Visit (parentVisitor);
		}

		public void Accept (AstRoot ast)
		{
		}

		public void Accept (Expression expr)
		{
		}

		public void Accept (Statement stmt)
		{
		}

		public void Accept (BinaryExpression pattern)
		{
			switch (pattern.Operation) {
			case BinaryOperation.Or:
			case BinaryOperation.And:
				pattern.Left.Visit (this);
				pattern.Right.Visit (this);
				break;
			default:
				errorLog.AddError (ErrorType.ParserError, pattern.Location, 
					"Binary operator can not be used on patterns!");
				break;
			}
		}

		public void Accept (UnaryExpression unaryop)
		{
			errorLog.AddError (ErrorType.ParserError, unaryop.Location,
				"Unary operators can not be used on patterns!");
		}

		public void Accept (NameExpression ident)
		{
			symbolTable.AddSymbol (ident.Value);
		}

		public void Accept (CallExpression call)
		{
			call.Visit (parentVisitor);
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
		}

		public void Accept (IntegerExpression integer)
		{
		}

		public void Accept (FloatExpression num)
		{
		}

		public void Accept (IfStatement ifStmt)
		{
			errorLog.AddError (ErrorType.ParserError, ifStmt.Location,
				"statement can not exist inside pattern!");
		}

		public void Accept (WhileStatement whileStmt)
		{
			errorLog.AddError (ErrorType.ParserError, whileStmt.Location,
				"statement can not exist inside pattern!");
		}

		public void Accept (WithStatement withStmt)
		{
			errorLog.AddError (ErrorType.ParserError, withStmt.Location,
				"statement can not exist inside pattern!");
		}

		public void Accept (DoStatement doStmt)
		{
			errorLog.AddError (ErrorType.ParserError, doStmt.Location,
				"statement can not exist inside pattern!");
		}

		public void Accept (ForStatement forStmt)
		{
			errorLog.AddError (ErrorType.ParserError, forStmt.Location,
				"statement can not exist inside pattern!");
		}

		public void Accept (ForeachStatement foreachStmt)
		{
			errorLog.AddError (ErrorType.ParserError, foreachStmt.Location,
				"statement can not exist inside pattern!");
		}

		public void Accept (GivenStatement switchStmt)
		{
			errorLog.AddError (ErrorType.ParserError, switchStmt.Location,
				"statement can not exist inside pattern!");
		}

		public void Accept (WhenStatement caseStmt)
		{
			errorLog.AddError (ErrorType.ParserError, caseStmt.Location,
				"statement can not exist inside pattern!");
		}

		public void Accept (FunctionDeclaration funcDecl)
		{
			errorLog.AddError (ErrorType.ParserError, funcDecl.Location,
				"statement can not exist inside pattern!");
		}

		public void Accept (CodeBlock scope)
		{
			errorLog.AddError (ErrorType.ParserError, scope.Location,
				"statement can not exist inside pattern!");
		}

		public void Accept (StringExpression str)
		{
			str.Visit (parentVisitor);
		}

		public void Accept (UseStatement useStmt)
		{
			errorLog.AddError (ErrorType.ParserError, useStmt.Location,
				"statement can not exist inside pattern!");
		}

		public void Accept (ClassDeclaration classDecl)
		{
			errorLog.AddError (ErrorType.ParserError, classDecl.Location,
				"statement can not exist inside pattern!");
		}

		public void Accept (InterfaceDeclaration contractDecl)
		{
			errorLog.AddError (ErrorType.ParserError, contractDecl.Location,
				"statement can not exist inside pattern!");
		}

		public void Accept (EnumDeclaration enumDecl)
		{
			errorLog.AddError (ErrorType.ParserError, enumDecl.Location,
				"statement can not exist inside pattern!");
		}

		public void Accept (ReturnStatement returnStmt)
		{
			errorLog.AddError (ErrorType.ParserError, returnStmt.Location,
				"statement can not exist inside pattern!");
		}

		public void Accept (YieldStatement yieldStmt)
		{
			errorLog.AddError (ErrorType.ParserError, yieldStmt.Location,
				"statement can not exist inside pattern!");
		}

		public void Accept (IndexerExpression indexer)
		{
			indexer.Visit (parentVisitor);
		}

		public void Accept (ListExpression list)
		{
			list.Visit (this);
		}

		public void Accept (HashExpression hash)
		{
			hash.Visit (parentVisitor);
		}

		public void Accept (SelfStatement self)
		{
			self.Visit (parentVisitor);
		}

		public void Accept (TrueExpression ntrue)
		{
			ntrue.Visit (parentVisitor);
		}

		public void Accept (FalseExpression nfalse)
		{
			nfalse.Visit (parentVisitor);
		}

		public void Accept (NullExpression nil)
		{
			nil.Visit (parentVisitor);
		}

		public void Accept (LambdaExpression lambda)
		{
			lambda.Visit (parentVisitor);
		}

		public void Accept (TernaryExpression ifExpr)
		{
			errorLog.AddError (ErrorType.ParserError, ifExpr.Location,
				"Expression can not exist inside pattern!");
		}

		public void Accept (ListCompExpression list)
		{
			errorLog.AddError (ErrorType.ParserError, list.Location,
				"List can not exist inside pattern!");
		}

		public void Accept (TryExceptStatement tryExcept)
		{
			errorLog.AddError (ErrorType.ParserError, tryExcept.Location,
				"statement can not exist inside pattern!");
		}

		public void Accept (RaiseStatement raise)
		{
			errorLog.AddError (ErrorType.ParserError, raise.Location,
				"statement can not exist inside pattern!");
		}

		public void Accept (TupleExpression tuple)
		{
			tuple.VisitChildren (this);
		}

		public void Accept (SuperCallExpression super)
		{
			errorLog.AddError (ErrorType.ParserError, super.Location,
				"statement can not exist inside pattern!");
		}

		public void Accept (BreakStatement brk)
		{
			errorLog.AddError (ErrorType.ParserError, brk.Location,
				"statement can not exist inside pattern!");
		}

		public void Accept (ContinueStatement cont)
		{
			errorLog.AddError (ErrorType.ParserError, cont.Location,
				"statement can not exist inside pattern!");
		}

		public void Accept (MatchExpression match)
		{
			errorLog.AddError (ErrorType.ParserError, match.Location,
				"match expression can not exist inside pattern!");
		}

		public void Accept (CaseExpression caseExpr)
		{
			errorLog.AddError (ErrorType.ParserError, caseExpr.Location,
				"match expression can not exist inside pattern!");
		}
	}
}

