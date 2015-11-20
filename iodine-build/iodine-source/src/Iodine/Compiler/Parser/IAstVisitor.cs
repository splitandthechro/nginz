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
using Iodine.Compiler.Ast;

namespace Iodine.Compiler
{
	public interface IAstVisitor
	{
		void Accept (AstRoot ast);
		void Accept (Expression expr);
		void Accept (Statement stmt);
		void Accept (BinaryExpression binop);
		void Accept (UnaryExpression unaryop);
		void Accept (NameExpression ident);
		void Accept (CallExpression call);
		void Accept (ArgumentList arglist);
		void Accept (KeywordArgumentList kwargs);
		void Accept (GetExpression getAttr);
		void Accept (IntegerExpression integer);
		void Accept (IfStatement ifStmt);
		void Accept (WhileStatement whileStmt);
		void Accept (DoStatement doStmt);
		void Accept (ForStatement forStmt);
		void Accept (ForeachStatement foreachStmt);
		void Accept (GivenStatement switchStmt);
		void Accept (WhenStatement caseStmt);
		void Accept (FunctionDeclaration funcDecl);
		void Accept (CodeBlock scope);
		void Accept (StringExpression stringConst);
		void Accept (UseStatement useStmt);
		void Accept (InterfaceDeclaration interfaceDecl);
		void Accept (ClassDeclaration classDecl);
		void Accept (ReturnStatement returnStmt);
		void Accept (YieldStatement yieldStmt);
		void Accept (IndexerExpression indexer);
		void Accept (ListExpression list);
		void Accept (HashExpression hash);
		void Accept (SelfStatement self);
		void Accept (TrueExpression ntrue);
		void Accept (FalseExpression nfalse);
		void Accept (NullExpression nil);
		void Accept (LambdaExpression lambda);
		void Accept (TryExceptStatement tryCatch);
		void Accept (WithStatement with);
		void Accept (BreakStatement brk);
		void Accept (ContinueStatement cont);
		void Accept (TupleExpression tuple);
		void Accept (FloatExpression dec);
		void Accept (SuperCallExpression super);
		void Accept (EnumDeclaration enumDecl);
		void Accept (RaiseStatement raise);
		void Accept (MatchExpression match);
		void Accept (CaseExpression caseExpr);
		void Accept (ListCompExpression list);
		void Accept (TernaryExpression ifExpr);
	}
}

