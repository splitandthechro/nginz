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
using System.Collections.Generic;
using Iodine.Compiler.Ast;

namespace Iodine.Compiler
{
	internal class FunctionAnalyser : IodineAstVisitor
	{
		private ErrorLog errorLog;
		private SymbolTable symbolTable;

		public FunctionAnalyser (ErrorLog errorLog, SymbolTable symbolTable)
		{
			this.errorLog = errorLog;
			this.symbolTable = symbolTable;
		}

		public override void Accept (UseStatement useStmt)
		{
			errorLog.AddError (ErrorType.ParserError, useStmt.Location,
				"use statement not valid inside function body!");
		}

		public override void Accept (BinaryExpression binop)
		{
			if (binop.Operation == BinaryOperation.Assign) {
				if (binop.Left is NameExpression) {
					NameExpression ident = (NameExpression)binop.Left;
					if (!this.symbolTable.IsSymbolDefined (ident.Value)) {
						this.symbolTable.AddSymbol (ident.Value);
					}
				}
			}
			binop.VisitChildren (this);
		}

		public override void Accept (InterfaceDeclaration interfaceDecl)
		{
			symbolTable.AddSymbol (interfaceDecl.Name);
		}

		public override void Accept (ClassDeclaration classDecl)
		{
			symbolTable.AddSymbol (classDecl.Name);
			RootAnalyser visitor = new RootAnalyser (errorLog, symbolTable);
			classDecl.Visit (visitor);
		}

		public override void Accept (EnumDeclaration enumDecl)
		{
			symbolTable.AddSymbol (enumDecl.Name);
		}

		public override void Accept (FunctionDeclaration funcDecl)
		{
			symbolTable.AddSymbol (funcDecl.Name);
			FunctionAnalyser visitor = new FunctionAnalyser (errorLog, symbolTable);
			symbolTable.BeginScope ();

			foreach (string param in funcDecl.Parameters) {
				symbolTable.AddSymbol (param);
			}

			funcDecl.Children [0].Visit (visitor);
			symbolTable.EndScope ();
		}

		public override void Accept (ForeachStatement foreachStmt)
		{
			symbolTable.AddSymbol (foreachStmt.Item);
			foreachStmt.Iterator.Visit (this);
			foreachStmt.Body.Visit (this);
		}

		public override void Accept (LambdaExpression lambda)
		{
			symbolTable.BeginScope ();
			foreach (string param in lambda.Parameters) {
				symbolTable.AddSymbol (param);
			}
			FunctionAnalyser visitor = new FunctionAnalyser (errorLog, symbolTable);
			lambda.Children [0].Visit (visitor);
			symbolTable.EndScope ();
		}

		public override void Accept (CodeBlock scope)
		{
			symbolTable.BeginScope ();
			scope.VisitChildren (this);
			symbolTable.EndScope ();
		}

		public override void Accept (TryExceptStatement tryExcept)
		{
			tryExcept.TryBody.Visit (this);
			if (tryExcept.ExceptionIdentifier != null) {
				symbolTable.AddSymbol (tryExcept.ExceptionIdentifier);
			}
			tryExcept.ExceptBody.Visit (this);
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
		}

		public override void Accept (RaiseStatement raise)
		{
			raise.VisitChildren (this);
		}

		public override void Accept (SuperCallExpression super)
		{
			super.VisitChildren (this);
		}

		public override void Accept (ReturnStatement returnStmt)
		{
			returnStmt.VisitChildren (this);
		}

		public override void Accept (YieldStatement yieldStmt)
		{
			yieldStmt.VisitChildren (this);
		}

		public override void Accept (ListExpression list)
		{
			list.VisitChildren (this);
		}

		public override void Accept (HashExpression hash)
		{
			hash.VisitChildren (this);
		}

		public override void Accept (IndexerExpression indexer)
		{
			indexer.VisitChildren (this);
		}

		public override void Accept (TupleExpression tuple)
		{
			tuple.VisitChildren (this);
		}

		public override void Accept (UnaryExpression unaryop)
		{
			unaryop.VisitChildren (this);
		}

		public override void Accept (CallExpression call)
		{
			call.VisitChildren (this);
		}

		public override void Accept (ArgumentList arglist)
		{
			arglist.VisitChildren (this);
		}

		public override void Accept (KeywordArgumentList kwargs)
		{
			kwargs.VisitChildren (this);
		}

		public override void Accept (GetExpression getAttr)
		{
			getAttr.VisitChildren (this);
		}

		public override void Accept (IfStatement ifStmt)
		{
			ifStmt.VisitChildren (this);
		}

		public override void Accept (GivenStatement switchStmt)
		{
			switchStmt.VisitChildren (this);
		}

		public override void Accept (WhenStatement caseStmt)
		{
			caseStmt.VisitChildren (this);
		}

		public override void Accept (WithStatement withStmt)
		{
			symbolTable.BeginScope ();
			withStmt.VisitChildren (this);
			symbolTable.EndScope ();
		}

		public override void Accept (WhileStatement whileStmt)
		{
			whileStmt.VisitChildren (this);
		}

		public override void Accept (DoStatement doStmt)
		{
			doStmt.VisitChildren (this);
		}

		public override void Accept (ForStatement forStmt)
		{
			forStmt.VisitChildren (this);
		}

		public override void Accept (MatchExpression match)
		{
			match.VisitChildren (this);
		}

		public override void Accept (TernaryExpression ifExpr)
		{
			ifExpr.VisitChildren (this);
		}

		public override void Accept (CaseExpression caseExpr)
		{
			PatternAnalyzer analyzer = new PatternAnalyzer (errorLog, symbolTable, this);
			caseExpr.Pattern.Visit (analyzer);
			if (caseExpr.Condition != null) {
				caseExpr.Condition.Visit (this);
			}
			caseExpr.Value.Visit (this);
		}
			
		public override void Accept (ListCompExpression list)
		{
			symbolTable.BeginScope ();
			symbolTable.AddSymbol (list.Identifier);
			list.VisitChildren (this);
			symbolTable.EndScope ();
		}
	}
}

