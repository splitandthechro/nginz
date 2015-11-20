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
using System.Text;
using System.Collections.Generic;
using Iodine.Compiler.Ast;

namespace Iodine.Compiler
{
	public sealed class Parser
	{
		private TokenStream tokenStream;

		public Parser (TokenStream tokenStream)
		{
			this.tokenStream = tokenStream;
		}

		public AstRoot Parse ()
		{
			try {
				AstRoot root = new AstRoot (tokenStream.Location);
				while (!tokenStream.EndOfStream) {
					root.Add (ParseStatement (tokenStream));
				}
				return root;
			} catch (Exception) {
				//this.tokenStream.ErrorLog.AddError (ErrorType.ParserError, "");
				return new AstRoot (tokenStream.Location);
			}
		}

		private static AstNode ParseClass (TokenStream stream)
		{
			stream.Expect (TokenClass.Keyword, "class");
			string name = stream.Expect (TokenClass.Identifier).Value;

			List<string> baseClass = new List<string> ();
			if (stream.Accept (TokenClass.Colon)) {
				do {
					baseClass.Add (ParseClassName (stream));
				} while (stream.Accept (TokenClass.Comma));
			}

			ClassDeclaration clazz = new ClassDeclaration (stream.Location, name, baseClass);

			stream.Expect (TokenClass.OpenBrace);

			while (!stream.Match (TokenClass.CloseBrace)) {
				if (stream.Match (TokenClass.Keyword, "func") || stream.Match (TokenClass.Operator,
					    "@")) {
					FunctionDeclaration func = ParseFunction (stream, false, clazz) as FunctionDeclaration;
					if (func.Name == name) {
						clazz.Constructor = func;
					} else {
						clazz.Add (func);
					}
				} else {
					clazz.Add (ParseStatement (stream));
				}
			}

			stream.Expect (TokenClass.CloseBrace);

			return clazz;
		}
			
		private static AstNode ParseEnum (TokenStream stream)
		{
			stream.Expect (TokenClass.Keyword, "enum");
			string name = stream.Expect (TokenClass.Identifier).Value;
			EnumDeclaration decl = new EnumDeclaration (stream.Location, name);

			stream.Expect (TokenClass.OpenBrace);
			int defaultVal = -1;

			while (!stream.Match (TokenClass.CloseBrace)) {
				string ident = stream.Expect (TokenClass.Identifier).Value;
				if (stream.Accept (TokenClass.Operator, "=")) {
					string val = stream.Expect (TokenClass.IntLiteral).Value;
					int numVal = 0;
					if (val != "") {
						numVal = Int32.Parse (val);
					}
					decl.Items [ident] = numVal;
				} else {
					decl.Items [ident] = defaultVal--;
				}
				if (!stream.Accept (TokenClass.Comma)) {
					break;
				}
			}

			stream.Expect (TokenClass.CloseBrace);

			return decl;
		}

		private static AstNode ParseInterface (TokenStream stream)
		{
			stream.Expect (TokenClass.Keyword, "interface");
			string name = stream.Expect (TokenClass.Identifier).Value;

			InterfaceDeclaration contract = new InterfaceDeclaration (stream.Location, name);

			stream.Expect (TokenClass.OpenBrace);

			while (!stream.Match (TokenClass.CloseBrace)) {
				if (stream.Match (TokenClass.Keyword, "func")) {
					FunctionDeclaration func = ParseFunction (stream, true) as FunctionDeclaration;
					contract.Add (func);
				} else {
					stream.ErrorLog.AddError (ErrorType.ParserError, stream.Location, 
						"Interface may only contain function prototypes!");
				}
				while (stream.Accept (TokenClass.SemiColon));
			}

			stream.Expect (TokenClass.CloseBrace);

			return contract;
		}

		private static string ParseClassName (TokenStream stream)
		{
			StringBuilder ret = new StringBuilder ();
			do {
				string attr = stream.Expect (TokenClass.Identifier).Value;
				ret.Append (attr);
				if (stream.Match (TokenClass.Operator, "."))
					ret.Append ('.');
			} while (stream.Accept (TokenClass.Operator, "."));
			return ret.ToString ();
		}

		private static UseStatement ParseUse (TokenStream stream)
		{
			stream.Expect (TokenClass.Keyword, "use");
			bool relative = stream.Accept (TokenClass.Operator, ".");
			string ident = "";
			if (!stream.Match (TokenClass.Operator, "*"))
				ident = ParseModuleName (stream);
			if (stream.Match (TokenClass.Keyword, "from") || stream.Match (TokenClass.Comma) ||
				stream.Match (TokenClass.Operator, "*")) {
				List<string> items = new List<string> ();
				bool wildcard = false;
				if (!stream.Accept (TokenClass.Operator, "*")) {
					items.Add (ident);
					stream.Accept (TokenClass.Comma);
					while (!stream.Match (TokenClass.Keyword, "from")) {
						Token item = stream.Expect (TokenClass.Identifier);
						items.Add (item.Value);
						if (!stream.Accept (TokenClass.Comma)) {
							break;
						}
					}
				} else {
					wildcard = true;
				}
				stream.Expect (TokenClass.Keyword, "from");

				relative = stream.Accept (TokenClass.Operator, ".");
				string module = ParseModuleName (stream);
				return new UseStatement (stream.Location, module, items, wildcard, relative);
			}
			return new UseStatement (stream.Location, ident, relative);
		}

		private static string ParseModuleName (TokenStream stream)
		{
			Token initIdent = stream.Expect (TokenClass.Identifier);

			if (stream.Match (TokenClass.Operator, ".")) {
				StringBuilder accum = new StringBuilder ();
				accum.Append (initIdent.Value);
				while (stream.Accept (TokenClass.Operator, ".")) {
					Token ident = stream.Expect (TokenClass.Identifier);
					accum.Append (Path.DirectorySeparatorChar);
					accum.Append (ident.Value);
				}
				return accum.ToString ();

			} else {
				return initIdent.Value;
			}
		}

		private static AstNode ParseFunction (TokenStream stream, bool prototype = false,
			ClassDeclaration cdecl = null)
		{
			if (stream.Accept (TokenClass.Operator, "@")) {
				/*
				 * Function decorators in the form of 
				 * @myDecorator
				 * func foo () {
				 * }
				 * are merely syntatic sugar for
				 * func foo () {
				 * }
				 * foo = myDecorator (foo)
				 */
				AstNode expr = ParseExpression (stream); // Decorator expression 
				/* This is the original function which is to be decorated */
				FunctionDeclaration idecl = ParseFunction (stream, prototype, cdecl) as FunctionDeclaration;
				/* We must construct an arglist which will be passed to the decorator */
				ArgumentList args = new ArgumentList (stream.Location);
				args.Add (new NameExpression (stream.Location, idecl.Name));
				/*
				 * Since two values can not be returned, we must return a single node containing both
				 * the function declaration and call to the decorator 
				 */
				AstRoot nodes = new AstRoot (stream.Location);
				nodes.Add (idecl);
				nodes.Add (new Expression (stream.Location, new BinaryExpression (stream.Location,
					BinaryOperation.Assign,
					new NameExpression (stream.Location, idecl.Name),
					new CallExpression (stream.Location, expr, args))));
				return nodes;
			}
			stream.Expect (TokenClass.Keyword, "func");
			bool isInstanceMethod;
			bool isVariadic;
			bool hasKeywordArgs;
			Token ident = stream.Expect (TokenClass.Identifier);
			List<string> parameters = ParseFuncParameters (stream,
				out isInstanceMethod,
				out isVariadic,
				out hasKeywordArgs);

			FunctionDeclaration decl = new FunctionDeclaration (stream.Location, ident != null ?
				ident.Value : "",
				isInstanceMethod,
				isVariadic,
				hasKeywordArgs,
				parameters);
			
			if (!prototype) {

				if (stream.Accept (TokenClass.Operator, "=>")) {
					decl.Add (new ReturnStatement (stream.Location, ParseExpression (stream)));
				} else {
					stream.Expect (TokenClass.OpenBrace);
					CodeBlock scope = new CodeBlock (stream.Location);

					if (stream.Match (TokenClass.Keyword, "super")) {
						scope.Add (ParseSuperCall (stream, cdecl));
					}

					while (!stream.Match (TokenClass.CloseBrace)) {
						scope.Add (ParseStatement (stream));
					}

					decl.Add (scope);
					stream.Expect (TokenClass.CloseBrace);
				}
			}
			return decl;
		}

		private static List<string> ParseFuncParameters (TokenStream stream,
			out bool isInstanceMethod,
			out bool isVariadic,
			out bool hasKeywordArgs)
		{
			isVariadic = false;
			hasKeywordArgs = false;
			isInstanceMethod = false;
			List<string> ret = new List<string> ();
			stream.Expect (TokenClass.OpenParan);
			if (stream.Accept (TokenClass.Keyword, "self")) {
				isInstanceMethod = true;
				if (!stream.Accept (TokenClass.Comma)) {
					stream.Expect (TokenClass.CloseParan);
					return ret;
				}
			}
			while (!stream.Match (TokenClass.CloseParan)) {
				if (!hasKeywordArgs && stream.Accept (TokenClass.Operator, "*")) {
					if (stream.Accept (TokenClass.Operator, "*")) {
						hasKeywordArgs = true;
						Token ident = stream.Expect (TokenClass.Identifier);
						ret.Add (ident.Value);
					} else {
						isVariadic = true;
						Token ident = stream.Expect (TokenClass.Identifier);
						ret.Add (ident.Value);
					}
				} else {
					if (hasKeywordArgs) {
						stream.ErrorLog.AddError (ErrorType.ParserError, stream.Location,
							"Argument after keyword arguments!");
					}
					if (isVariadic) {
						stream.ErrorLog.AddError (ErrorType.ParserError, stream.Location,
							"Argument after params keyword!");
					}
					Token param = stream.Expect (TokenClass.Identifier);
					ret.Add (param.Value);
				}
				if (!stream.Accept (TokenClass.Comma)) {
					break;
				}
			}
			stream.Expect (TokenClass.CloseParan);
			return ret;
		}

		private static AstNode ParseStatement (TokenStream stream) 
		{
			if (stream.Match (TokenClass.Keyword)) {
				switch (stream.Current.Value) {
				case "class":
					return ParseClass (stream);
				case "enum":
					return ParseEnum (stream);
				case "interface":
					return ParseInterface (stream);
				case "func":
					return ParseFunction (stream);
				case "if":
					return ParseIf (stream);
				case "given":
					return ParseGiven (stream);
				case "for":
					return ParseFor (stream);
				case "foreach":
					return ParseForeach (stream);
				case "with":
					return ParseWith (stream);
				case "while":
					return ParseWhile (stream);
				case "do":
					return ParseDoWhile (stream);
				case "use":
					return ParseUse (stream);
				case "return":
					return ParseReturn (stream);
				case "raise":
					return ParseRaise (stream);
				case "yield":
					return ParseYield (stream);
				case "try":
					return ParseTryExcept (stream);
				case "break":
					stream.Accept (TokenClass.Keyword);
					return new BreakStatement (stream.Location);
				case "continue":
					stream.Accept (TokenClass.Keyword);
					return new ContinueStatement (stream.Location);
				case "super":
					stream.ErrorLog.AddError (ErrorType.ParserError, stream.Location,
						"super () constructor must be called first!");
					return ParseSuperCall (stream, new ClassDeclaration (stream.Location, "", null));
				}
			}
			if (stream.Match (TokenClass.OpenBrace)) {
				return ParseBlock (stream);
			} else if (stream.Accept (TokenClass.SemiColon)) {
				return new Statement (stream.Location);
			} else if (stream.Match (TokenClass.Operator, "@")) {
				return ParseFunction (stream);
			} else {
				AstNode node = ParseExpression (stream);
				if (node == null) {
					stream.MakeError ();
				}
				return new Expression (stream.Location, node);
			}
		}

		private static AstNode ParseBlock (TokenStream stream)
		{
			CodeBlock ret = new CodeBlock (stream.Location);
			stream.Expect (TokenClass.OpenBrace);

			while (!stream.Match (TokenClass.CloseBrace)) {
				ret.Add (ParseStatement (stream));
			}

			stream.Expect (TokenClass.CloseBrace);
			return ret;
		}

		private static AstNode ParseTryExcept (TokenStream stream)
		{
			TryExceptStatement retVal = null;
			stream.Expect (TokenClass.Keyword, "try");
			AstNode tryBody = ParseStatement (stream);
			AstNode typeList = new ArgumentList (stream.Location);
			stream.Expect (TokenClass.Keyword, "except");
			if (stream.Accept (TokenClass.OpenParan)) {
				Token ident = stream.Expect (TokenClass.Identifier);
				if (stream.Accept (TokenClass.Operator, "as")) {
					typeList = ParseTypeList (stream);
				}
				stream.Expect (TokenClass.CloseParan);
				retVal = new TryExceptStatement (stream.Location, ident.Value);
			} else {
				retVal = new TryExceptStatement (stream.Location, null);
			}
			retVal.Add (tryBody);
			retVal.Add (ParseStatement (stream));
			retVal.Add (typeList);
			return retVal;
		}

		private static ArgumentList ParseTypeList (TokenStream stream)
		{
			ArgumentList argList = new ArgumentList (stream.Location);
			while (!stream.Match (TokenClass.CloseParan)) {
				argList.Add (ParseExpression (stream));
				if (!stream.Accept (TokenClass.Comma)) {
					break;
				}
			}
			return argList;
		}

		private static AstNode ParseGiven (TokenStream stream)
		{
			GivenStatement switchStmt = new GivenStatement (stream.Location);
			stream.Expect (TokenClass.Keyword, "given");
			stream.Expect (TokenClass.OpenParan);
			switchStmt.Add (ParseExpression (stream));
			stream.Expect (TokenClass.CloseParan);
			stream.Expect (TokenClass.OpenBrace);
			AstNode defaultBlock = new AstRoot (stream.Location);
			AstRoot caseStatements = new AstRoot (stream.Location);
			while (!stream.EndOfStream && !stream.Match (TokenClass.CloseBrace)) {
				caseStatements.Add (ParseWhen (stream));
				if (stream.Accept (TokenClass.Keyword, "default")) {
					defaultBlock = ParseStatement (stream); 
				}
			}
			switchStmt.Add (caseStatements);
			switchStmt.Add (defaultBlock);
			stream.Expect (TokenClass.CloseBrace);
			return switchStmt;
		}

		private static AstNode ParseWhen (TokenStream stream)
		{
			WhenStatement caseStmt = new WhenStatement (stream.Location);
			stream.Expect (TokenClass.Keyword, "when");
			AstNode value = ParseExpression (stream);
			AstNode body = ParseStatement (stream);
			AstNode lambda = new LambdaExpression (body.Location, false, false, false,
				new System.Collections.Generic.List<string> ());
			lambda.Add (body);
			caseStmt.Add (value);
			caseStmt.Add (lambda);
			return caseStmt;
		}

		private static AstNode ParseIf (TokenStream stream)
		{
			IfStatement ifStmt = new IfStatement (stream.Location);
			stream.Expect (TokenClass.Keyword, "if");
			stream.Expect (TokenClass.OpenParan);
			ifStmt.Add (ParseExpression (stream));
			stream.Expect (TokenClass.CloseParan);
			ifStmt.Add (ParseStatement (stream));
			if (stream.Accept (TokenClass.Keyword, "else")) {
				ifStmt.Add (ParseStatement (stream));
			} else {
				ifStmt.Add (new CodeBlock (stream.Location));
			}
			return ifStmt;
		}

		private static AstNode ParseFor (TokenStream stream)
		{
			ForStatement ret = new ForStatement (stream.Location);
			stream.Expect (TokenClass.Keyword, "for");
			stream.Expect (TokenClass.OpenParan);
			ret.Add (new Expression (stream.Location, ParseExpression (stream)));
			stream.Expect (TokenClass.SemiColon);
			ret.Add (ParseExpression (stream));
			stream.Expect (TokenClass.SemiColon);
			ret.Add (new Expression (stream.Location, ParseExpression (stream)));
			stream.Expect (TokenClass.CloseParan);
			ret.Add (ParseStatement (stream));
			return ret;
		}

		private static AstNode ParseForeach (TokenStream stream)
		{
			stream.Expect (TokenClass.Keyword, "foreach");
			stream.Expect (TokenClass.OpenParan);
			Token identifier = stream.Expect (TokenClass.Identifier);
			stream.Expect (TokenClass.Keyword, "in");
			AstNode expr = ParseExpression (stream);
			stream.Expect (TokenClass.CloseParan);
			AstNode body = ParseStatement (stream);
			return new ForeachStatement (stream.Location, identifier.Value, expr, body);
		}

		private static AstNode ParseDoWhile (TokenStream stream)
		{
			DoStatement ret = new DoStatement (stream.Location);
			stream.Expect (TokenClass.Keyword, "do");
			ret.Add (ParseStatement (stream));
			stream.Expect (TokenClass.Keyword, "while");
			stream.Expect (TokenClass.OpenParan);
			ret.Add (ParseExpression (stream));
			stream.Expect (TokenClass.CloseParan);
			return ret;
		}

		private static AstNode ParseWhile (TokenStream stream)
		{
			WhileStatement ret = new WhileStatement (stream.Location);
			stream.Expect (TokenClass.Keyword, "while");
			stream.Expect (TokenClass.OpenParan);
			ret.Add (ParseExpression (stream));
			stream.Expect (TokenClass.CloseParan);
			ret.Add (ParseStatement (stream));
			return ret;
		}
			
		private static AstNode ParseWith (TokenStream stream)
		{
			WithStatement ret = new WithStatement (stream.Location);
			stream.Expect (TokenClass.Keyword, "with");
			stream.Expect (TokenClass.OpenParan);
			ret.Add (ParseExpression (stream));
			stream.Expect (TokenClass.CloseParan);
			ret.Add (ParseStatement (stream));
			return ret;
		}

		private static AstNode ParseRaise (TokenStream stream)
		{
			stream.Expect (TokenClass.Keyword, "raise");
			return new RaiseStatement (stream.Location, ParseExpression (stream));
		}

		private static AstNode ParseReturn (TokenStream stream)
		{
			stream.Expect (TokenClass.Keyword, "return");
			if (stream.Accept (TokenClass.SemiColon)) {
				return new ReturnStatement (stream.Location, new CodeBlock (stream.Location));
			} else {
				return new ReturnStatement (stream.Location, ParseExpression (stream));
			}
		}

		private static AstNode ParseYield (TokenStream stream)
		{
			stream.Expect (TokenClass.Keyword, "yield");
			return new YieldStatement (stream.Location, ParseExpression (stream));
		}

		private static AstNode ParseExpression (TokenStream stream)
		{
			return ParseAssign (stream);
		}

		private static AstNode ParseAssign (TokenStream stream)
		{
			AstNode expr = ParseTernaryIfElse (stream);
			while (stream.Match (TokenClass.Operator)) {
				switch (stream.Current.Value) {
				case "=":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.Assign,
						expr, ParseTernaryIfElse (stream));
					continue;
				case "+=":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.Assign, expr,
						new BinaryExpression (stream.Location,
							BinaryOperation.Add, expr,
							ParseTernaryIfElse (stream)));
					continue;
				case "-=":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.Assign, expr,
						new BinaryExpression (stream.Location,
							BinaryOperation.Sub,
							expr,
							ParseTernaryIfElse (stream)));
					continue;
				case "*=":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.Assign, expr,
						new BinaryExpression (stream.Location,
							BinaryOperation.Mul,
							expr,
							ParseTernaryIfElse (stream)));
					continue;
				case "/=":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.Assign, expr,
						new BinaryExpression (stream.Location,
							BinaryOperation.Div,
							expr,
							ParseTernaryIfElse (stream)));
					continue;
				case "%=":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.Assign, expr,
						new BinaryExpression (stream.Location,
							BinaryOperation.Mod,
							expr,
							ParseTernaryIfElse (stream)));
					continue;
				case "^=":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.Assign, expr,
						new BinaryExpression (stream.Location,
							BinaryOperation.Xor,
							expr, 
							ParseTernaryIfElse (stream)));
					continue;
				case "&=":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.Assign, expr,
						new BinaryExpression (stream.Location,
							BinaryOperation.And,
							expr,
							ParseTernaryIfElse (stream)));
					continue;
				case "|=":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.Assign, expr,
						new BinaryExpression (stream.Location,
							BinaryOperation.Or,
							expr,
							ParseTernaryIfElse (stream)));
					continue;
				case "<<=":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.Assign, expr,
						new BinaryExpression (stream.Location,
							BinaryOperation.LeftShift,
							expr,
							ParseTernaryIfElse (stream)));
					continue;
				case ">>=":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.Assign, expr,
						new BinaryExpression (stream.Location,
							BinaryOperation.RightShift,
							expr,
							ParseTernaryIfElse (stream)));
					continue;
				default:
					break;
				}
				break;
			}
			return expr;
		}

		private static AstNode ParseTernaryIfElse (TokenStream stream)
		{
			AstNode expr = ParsePipeline (stream);
			while (stream.Accept (TokenClass.Keyword, "when")) {
				AstNode condition = ParseExpression (stream);
				stream.Expect (TokenClass.Keyword, "else");
				AstNode altValue = ParseTernaryIfElse (stream);
				expr = new TernaryExpression (expr.Location, condition, expr, altValue);
			}
			return expr;
		}

		private static AstNode ParsePipeline (TokenStream stream)
		{
			AstNode expr = ParseRange (stream);
			while (stream.Accept (TokenClass.Operator, "|>")) {
				CallExpression call = ParseRange (stream) as CallExpression;
				if (call == null) {
					stream.ErrorLog.AddError (ErrorType.ParserError, stream.Location, 
						"Right value must be function call");
				} else {
					call.Arguments.Children.Insert (0, expr);
					expr = call;
				}
			}
			return expr;
		}

		private static AstNode ParseRange (TokenStream stream)
		{
			AstNode expr = ParseBoolOr (stream);
			while (stream.Match (TokenClass.Operator)) {
				switch (stream.Current.Value) {
				case "...":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.ClosedRange, expr,
						ParseBoolOr (stream));
					continue;
				case "..":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.HalfRange, expr,
						ParseBoolOr (stream));
					continue;
				default:
					break;
				}
				break;
			}
			return expr;
		}

		private static AstNode ParseBoolOr (TokenStream stream) 
		{
			AstNode expr = ParseBoolAnd (stream);
			while (stream.Match (TokenClass.Operator)) {
				switch (stream.Current.Value) {
				case "||":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.BoolOr, expr,
						ParseBoolAnd (stream));
					continue;
				case "??":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.NullCoalescing, expr,
						ParseBoolAnd (stream));
					continue;
				default:
					break;
				}
				break;
			}
			return expr;
		}

		private static AstNode ParseBoolAnd (TokenStream stream)
		{
			AstNode expr = ParseOr (stream);
			while (stream.Accept (TokenClass.Operator, "&&")) {
				expr = new BinaryExpression (stream.Location, BinaryOperation.BoolAnd, expr, ParseOr (stream));
			}
			return expr;
		}

		public static AstNode ParseOr (TokenStream stream)
		{
			AstNode expr = ParseXor (stream);
			while (stream.Accept (TokenClass.Operator, "|")) {
				expr = new BinaryExpression (stream.Location, BinaryOperation.Or, expr, ParseXor (stream));
			}
			return expr;
		}

		public static AstNode ParseXor (TokenStream stream)
		{
			AstNode expr = ParseAnd (stream);
			while (stream.Accept (TokenClass.Operator, "^")) {
				expr = new BinaryExpression (stream.Location, BinaryOperation.Xor, expr, ParseAnd (stream));
			}
			return expr;
		}

		public static AstNode ParseAnd (TokenStream stream)
		{
			AstNode expr = ParseEquals (stream);
			while (stream.Accept (TokenClass.Operator, "&")) {
				expr = new BinaryExpression (stream.Location, BinaryOperation.And, expr,
					ParseEquals (stream));
			}
			return expr;
		}

		public static AstNode ParseEquals (TokenStream stream)
		{
			AstNode expr = ParseRelationalOp (stream);
			while (stream.Match (TokenClass.Operator)) {
				switch (stream.Current.Value) {
				case "==":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.Equals, expr,
						ParseRelationalOp (stream));
					continue;
				case "!=":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.NotEquals, expr,
						ParseRelationalOp (stream));
					continue;
				default:
					break;
				}
				break;
			}
			return expr;
		}

		public static AstNode ParseRelationalOp (TokenStream stream)
		{
			AstNode expr = ParseBitshift (stream);
			while (stream.Match (TokenClass.Operator)) {
				switch (stream.Current.Value) {
				case ">":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.GreaterThan, expr,
						ParseBitshift (stream));
					continue;
				case "<":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.LessThan, expr,
						ParseBitshift (stream));
					continue;
				case ">=":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.GreaterThanOrEqu, expr,
						ParseBitshift (stream));
					continue;
				case "<=":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.LessThanOrEqu, expr,
						ParseBitshift (stream));
					continue;
				case "is":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.InstanceOf, expr,
						ParseBitshift (stream));
					continue;
				case "isnot":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.NotInstanceOf, expr,
						ParseBitshift (stream));
					continue;
				case "as":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.DynamicCast, expr,
						ParseBitshift (stream));
					continue;
				default:
					break;
				}
				break;
			}
			return expr;
		}

		public static AstNode ParseBitshift (TokenStream stream)
		{
			AstNode expr = ParseAdditive (stream);
			while (stream.Match (TokenClass.Operator)) {
				switch (stream.Current.Value) {
				case "<<":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.LeftShift, expr,
						ParseAdditive (stream));
					continue;
				case ">>":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.RightShift, expr,
						ParseAdditive (stream));
					continue;
				default:
					break;
				}
				break;
			}
			return expr;
		}

		public static AstNode ParseAdditive (TokenStream stream)
		{
			AstNode expr = ParseMultiplicative (stream);
			while (stream.Match (TokenClass.Operator)) {
				switch (stream.Current.Value) {
				case "+":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.Add, expr,
						ParseMultiplicative (stream));
					continue;
				case "-":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.Sub, expr,
						ParseMultiplicative (stream));
					continue;
				default:
					break;
				}
				break;
			}
			return expr;
		}

		public static AstNode ParseMultiplicative (TokenStream stream)
		{
			AstNode expr = ParseUnary (stream);
			while (stream.Match (TokenClass.Operator)) {
				switch (stream.Current.Value) {
				case "*":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.Mul, expr,
						ParseUnary (stream));
					continue;
				case "/":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.Div, expr,
						ParseUnary (stream));
					continue;
				case "%":
					stream.Accept (TokenClass.Operator);
					expr = new BinaryExpression (stream.Location, BinaryOperation.Mod, expr,
						ParseUnary (stream));
					continue;
				default:
					break;
				}
				break;
			}
			return expr;
		}

		public static AstNode ParseUnary (TokenStream stream)
		{
			if (stream.Match (TokenClass.Operator)) {
				switch (stream.Current.Value) {
				case "-":
					stream.Accept (TokenClass.Operator);
					return new UnaryExpression (stream.Location, UnaryOperation.Negate, ParseUnary (
						stream));
				case "~":
					stream.Accept (TokenClass.Operator);
					return new UnaryExpression (stream.Location, UnaryOperation.Not, ParseUnary (
						stream));
				case "!":
					stream.Accept (TokenClass.Operator);
					return new UnaryExpression (stream.Location, UnaryOperation.BoolNot, ParseUnary (
						stream));

				}
			}
			return ParseCallSubscriptAccess (stream);
		}

		public static AstNode ParseCallSubscriptAccess (TokenStream stream)
		{
			return ParseCallSubscriptAccess (stream, ParseTerm (stream));
		}

		public static AstNode ParseCallSubscriptAccess (TokenStream stream, AstNode lvalue)
		{
			if (stream.Match (TokenClass.OpenParan)) {
				return ParseCallSubscriptAccess (stream, new CallExpression (stream.Location, lvalue,
					ParseArgumentList (stream)));
			} else if (stream.Match (TokenClass.OpenBracket)) {
				return ParseCallSubscriptAccess (stream, ParseIndexer (lvalue, stream));
			} else if (stream.Match (TokenClass.Operator, ".")) {
				return ParseCallSubscriptAccess (stream, ParseGet (lvalue, stream));
			}
			return lvalue;
		}

		public static AstNode ParseTerm (TokenStream stream)
		{
			switch (stream.Current.Class) {
			case TokenClass.Identifier:
				return new NameExpression (stream.Location, stream.ReadToken ().Value);
			case TokenClass.IntLiteral:
				return new IntegerExpression (stream.Location, long.Parse ( 
					stream.ReadToken ().Value));
			case TokenClass.FloatLiteral:
				return new FloatExpression (stream.Location, double.Parse (
					stream.ReadToken ().Value));
			case TokenClass.InterpolatedStringLiteral:
				AstNode val = ParseString (stream.Location, stream.ReadToken ().Value);
				if (val == null) {
					stream.MakeError ();
					return new StringExpression (stream.Location, "");
				}
				return val;
			case TokenClass.StringLiteral:
				return new StringExpression (stream.Location, stream.ReadToken ().Value);
			case TokenClass.OpenBracket:
				return ParseList (stream);
			case TokenClass.OpenBrace:
				return ParseHash (stream);
			case TokenClass.OpenParan:
				stream.ReadToken ();
				AstNode expr = ParseExpression (stream);
				if (stream.Accept (TokenClass.Comma)) {
					return ParseTuple (expr, stream);
				}
				stream.Expect (TokenClass.CloseParan);
				return expr;
			case TokenClass.Keyword:
				switch (stream.Current.Value) {
				case "self":
					stream.ReadToken ();
					return new SelfStatement (stream.Location);
				case "true":
					stream.ReadToken ();
					return new TrueExpression (stream.Location);
				case "false":
					stream.ReadToken ();
					return new FalseExpression (stream.Location);
				case "null":
					stream.ReadToken ();
					return new NullExpression (stream.Location);
				case "lambda":
					return ParseLambda (stream);
				case "match":
					return ParseMatch (stream);
				}
				break;
			}
		
			stream.MakeError ();
			return null;
		}

		private static AstNode ParseMatch (TokenStream stream)
		{
			MatchExpression expr = new MatchExpression (stream.Location);
			stream.Expect (TokenClass.Keyword, "match");
			expr.Add (ParseExpression (stream));
			stream.Expect (TokenClass.OpenBrace);
			while (stream.Accept (TokenClass.Keyword, "case")) {
				AstNode condition = null;
				AstNode pattern = ParsePattern (stream);
				if (stream.Accept (TokenClass.Keyword, "when")) {
					condition = ParseExpression (stream);
				}
				stream.Expect (TokenClass.Operator, "=>");
				AstNode value = ParseExpression (stream);
				expr.Children.Add (new CaseExpression (pattern.Location, pattern, condition, value));
			}
			stream.Expect (TokenClass.CloseBrace);
			return expr;
		}

		private static AstNode ParsePattern (TokenStream stream)
		{
			return ParsePatternOr (stream);
		}

		private static AstNode ParsePatternOr (TokenStream stream)
		{
			AstNode expr = ParsePatternAnd (stream);
			while (stream.Match (TokenClass.Operator, "|")) {
				stream.Accept (TokenClass.Operator);
				expr = new BinaryExpression (stream.Location, BinaryOperation.Or, expr,
					ParsePatternAnd (stream));
			}
			return expr;
		}

		private static AstNode ParsePatternAnd (TokenStream stream)
		{
			AstNode expr = ParsePatternTerm (stream);
			while (stream.Match (TokenClass.Operator, "&")) {
				stream.Accept (TokenClass.Operator);
				expr = new BinaryExpression (stream.Location, BinaryOperation.And, expr,
					ParsePatternTerm (stream));
			}
			return expr;
		}

		private static AstNode ParsePatternTerm (TokenStream stream)
		{
			return ParseTerm (stream);
		}

		private static AstNode ParseLambda (TokenStream stream)
		{
			stream.Expect (TokenClass.Keyword, "lambda");
			bool isInstanceMethod;
			bool isVariadic;
			bool acceptsKwargs;

			List<string> parameters = ParseFuncParameters (stream,
				out isInstanceMethod,
				out isVariadic,
				out acceptsKwargs);

			LambdaExpression decl = new LambdaExpression (stream.Location, isInstanceMethod, isVariadic, acceptsKwargs, parameters);
			if (stream.Accept (TokenClass.Operator, "=>"))
				decl.Add (new ReturnStatement (stream.Location, ParseExpression (stream)));
			else
				decl.Add (ParseStatement (stream));
			return decl;
		}

		private static AstNode ParseIndexer (AstNode lvalue, TokenStream stream)
		{
			stream.Expect (TokenClass.OpenBracket);
			AstNode index = ParseExpression (stream);
			stream.Expect (TokenClass.CloseBracket);
			return new IndexerExpression (stream.Location, lvalue, index);
		}

		private static AstNode ParseGet (AstNode lvalue, TokenStream stream)
		{
			stream.Expect (TokenClass.Operator, ".");
			Token ident = stream.Expect (TokenClass.Identifier);
			return new GetExpression (stream.Location, lvalue, ident.Value);
		}

		public static SuperCallExpression ParseSuperCall (TokenStream stream, ClassDeclaration parent)
		{
			SuperCallExpression ret = new SuperCallExpression (stream.Location);
			stream.Expect (TokenClass.Keyword, "super");
			ret.Parent = parent;
			ret.Add (ParseArgumentList (stream));
			while (stream.Accept (TokenClass.SemiColon))
				;
			return ret;
		}

		private static AstNode ParseArgumentList (TokenStream stream)
		{
			ArgumentList argList = new ArgumentList (stream.Location);
			stream.Expect (TokenClass.OpenParan);
			KeywordArgumentList kwargs = null;
			while (!stream.Match (TokenClass.CloseParan)) {
				if (stream.Accept (TokenClass.Operator, "*")) {
					argList.Packed = true;
					argList.Add (ParseExpression (stream));
					break;
				}
				AstNode arg = ParseExpression (stream);
				if (stream.Accept (TokenClass.Colon)) {
					if (kwargs == null) {
						kwargs = new KeywordArgumentList (arg.Location);
					}
					NameExpression ident = arg as NameExpression;
					AstNode val = ParseExpression (stream);
					if (ident == null) {
						stream.ErrorLog.AddError (ErrorType.ParserError, arg.Location,
							"Keyword must be a valid identifier");
					} else {
						kwargs.Add (ident.Value, val);
					}
				} else
					argList.Add (arg);
				if (!stream.Accept (TokenClass.Comma)) {
					break;
				}

			}
			if (kwargs != null) {
				argList.Add (kwargs);
			}
			stream.Expect (TokenClass.CloseParan);
			return argList;

		}

		private static AstNode ParseList (TokenStream stream)
		{
			stream.Expect (TokenClass.OpenBracket);
			ListExpression ret = new ListExpression (stream.Location);
			while (!stream.Match (TokenClass.CloseBracket)) {
				AstNode expr = ParseExpression (stream);
				if (stream.Accept (TokenClass.Keyword, "for")) {
					string ident = stream.Expect (TokenClass.Identifier).Value;
					stream.Expect (TokenClass.Keyword, "in");
					AstNode iterator = ParseExpression (stream);
					AstNode predicate = null;
					if (stream.Accept (TokenClass.Keyword, "when")) {
						predicate = ParseExpression (stream);
					}
					stream.Expect (TokenClass.CloseBracket);
					return new ListCompExpression (expr.Location, expr, ident, iterator, predicate);
				}
				ret.Add (expr);
				if (!stream.Accept (TokenClass.Comma)) {
					break;
				}
			}
			stream.Expect (TokenClass.CloseBracket);
			return ret;
		}

		private static AstNode ParseHash (TokenStream stream)
		{
			stream.Expect (TokenClass.OpenBrace);
			HashExpression ret = new HashExpression (stream.Location);
			while (!stream.Match (TokenClass.CloseBrace)) {
				ret.Add (ParseExpression (stream));
				stream.Expect (TokenClass.Colon);
				ret.Add (ParseExpression (stream));
				if (!stream.Accept (TokenClass.Comma)) {
					break;
				}
			}
			stream.Expect (TokenClass.CloseBrace);
			return ret;
		}

		private static AstNode ParseTuple (AstNode firstVal, TokenStream stream)
		{
			TupleExpression tuple = new TupleExpression (stream.Location);
			tuple.Add (firstVal);
			while (!stream.Match (TokenClass.CloseParan)) {
				tuple.Add (ParseExpression (stream));
				if (!stream.Accept (TokenClass.Comma)) {
					break;
				}
			}
			stream.Expect (TokenClass.CloseParan);
			return tuple;
		}

		private static AstNode ParseString (Location loc, string str)
		{
			int pos = 0;
			string accum = "";
			List<string> vars = new List<string> ();
			while (pos < str.Length) {
				if (str [pos] == '#' && str.Length != pos + 1 && str [pos + 1] == '{') {
					string substr = str.Substring (pos + 2);
					if (substr.IndexOf ('}') == -1)
						return null;
					substr = substr.Substring (0, substr.IndexOf ('}'));
					pos += substr.Length + 3;
					vars.Add (substr);
					accum += "{}";

				} else {
					accum += str [pos++];
				}
			}
			StringExpression ret = new StringExpression (loc, accum);

			foreach (string name in vars) {
				ret.Add (new NameExpression (loc, name));
			}
			return ret;
		}
	}
}

