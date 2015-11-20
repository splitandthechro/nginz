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
using System.Text;
using System.Collections.Generic;

namespace Iodine.Compiler
{
	/// <summary>
	/// Iodine lexer class, tokenizes our source into a list of Token objects represented as a TokenStream object.
	/// </summary>
	public sealed class Tokenizer
	{
		private int position;
		private int sourceLen;
		private string source;
		private string file;
		private ErrorLog errorLog;
		private Location location;

		public Tokenizer (ErrorLog errorLog, string source, string file = "")
		{
			this.errorLog = errorLog;
			this.source = source;
			this.file = file;
			position = 0;
			sourceLen = source.Length;
			location = new Location (0, 0, file);
		}

		public TokenStream Scan ()
		{
			TokenStream retStream = new TokenStream (errorLog);
			EatWhiteSpaces ();
			while (PeekChar () != -1) {
				Token nextToken = NextToken ();
				if (nextToken != null)
					retStream.AddToken (nextToken);
				EatWhiteSpaces ();
			}
			return retStream;
		}

		private Token NextToken ()
		{
			char ch = (char)PeekChar ();
			switch (ch) {
			case '#':
				return ReadComment ();
			case '\'':
			case '"':
				return ReadStringLiteral ();
			case '_':
				return ReadIdentifier ();
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
				return ReadNumber ();
			case '+':
			case '-':
			case '*':
			case '/':
			case '.':
			case '=':
			case '<':
			case '>':
			case '~':
			case '!':
			case '&':
			case '^':
			case '|':
			case '%':
			case '@':
			case '?':
				return ReadOperator ();
			case '{':
				ReadChar ();
				return new Token (TokenClass.OpenBrace, "{", location);
			case '}':
				ReadChar ();
				return new Token (TokenClass.CloseBrace, "}", location);
			case '(':
				ReadChar ();
				return new Token (TokenClass.OpenParan, "(", location);
			case ')':
				ReadChar ();
				return new Token (TokenClass.CloseParan, ")", location);
			case '[':
				ReadChar ();
				return new Token (TokenClass.OpenBracket, "[", location);
			case ']':
				ReadChar ();
				return new Token (TokenClass.CloseBracket, "]", location);
			case ';':
				ReadChar ();
				return new Token (TokenClass.SemiColon, ";", location);
			case ':':
				ReadChar ();
				return new Token (TokenClass.Colon, ":", location);
			case ',':
				ReadChar ();
				return new Token (TokenClass.Comma, ",", location);
			default:
				if (char.IsLetter (ch)) {
					return ReadIdentifier ();
				}
				errorLog.AddError (ErrorType.LexerError, location, "Unexpected '{0}'", 
					(char)ReadChar ());
				
				return null;
			}
		}

		private Token ReadComment ()
		{
			int ch = 0;
			do {
				ch = ReadChar ();
			} while (ch != -1 && ch != '\n');

			return null;
		}

		private Token ReadNumber ()
		{
			StringBuilder accum = new StringBuilder ();
			char ch = (char)PeekChar ();
			if (ch == '0' && PeekChar (1) == 'x')
				return ReadHexNumber (accum);
			do {
				if (ch == '.')
					return ReadFloat (accum);
				accum.Append ((char)ReadChar ());
				ch = (char)PeekChar ();
			} while (char.IsDigit (ch) || ch == '.');
			return new Token (TokenClass.IntLiteral, accum.ToString (), location);
		}

		private Token ReadHexNumber (StringBuilder accum)
		{
			ReadChar (); // 0
			ReadChar (); // x
			while (IsHexNumber ((char)PeekChar ())) {
				accum.Append ((char)ReadChar ());
			}

			return new Token (TokenClass.IntLiteral, Int32.Parse (accum.ToString (),
				System.Globalization.NumberStyles.HexNumber).ToString (), location);
		}

		private static bool IsHexNumber (char c)
		{
			return "ABCDEFabcdef0123456789".Contains (c.ToString ());
		}

		private Token ReadFloat (StringBuilder buffer)
		{
			ReadChar (); // .
			buffer.Append (".");
			char ch = (char)PeekChar ();
			do {
				buffer.Append ((char)ReadChar ());
				ch = (char)PeekChar ();
			} while (char.IsDigit (ch));
			return new Token (TokenClass.FloatLiteral, buffer.ToString (), location);
		}

		private Token ReadStringLiteral ()
		{
			StringBuilder accum = new StringBuilder ();
			int delimiter = ReadChar ();
			int ch = (char)PeekChar ();
			while (ch != delimiter && ch != -1) {
				if (ch == '\\') {
					ReadChar ();
					accum.Append (ParseEscapeCode ());
				} else {
					accum.Append ((char)ReadChar ());
				}
				ch = PeekChar ();
			}
			if (ReadChar () == -1) {
				errorLog.AddError (ErrorType.LexerError, location, "Unterminated string literal!");
			}
			return new Token (ch == '"' ? 
				TokenClass.InterpolatedStringLiteral :
				TokenClass.StringLiteral,
				accum.ToString (),
				location);
		}

		private char ParseEscapeCode ()
		{
			char escape = (char)ReadChar ();
			switch (escape) {
			case '"':
				return '"';
			case 'n':
				return '\n';
			case 'b':
				return '\b';
			case 'r':
				return '\r';
			case 't':
				return '\t';
			case 'f':
				return '\f';
			case '\\':
				return '\\';
			}
			errorLog.AddError (ErrorType.LexerError, location, "Unrecognized escape sequence");
			return '\0';
		}

		private Token ReadIdentifier ()
		{
			StringBuilder accum = new StringBuilder ();
			char ch = (char)PeekChar ();
			do {
				accum.Append ((char)ReadChar ());
				ch = (char)PeekChar ();
			} while (char.IsLetterOrDigit (ch) || ch == '_');

			string final = accum.ToString ();

			switch (final) {
			case "if":
			case "else":
			case "while":
			case "do":
			case "for":
			case "func":
			case "class":
			case "use":
			case "self":
			case "foreach":
			case "in":
			case "true":
			case "false":
			case "null":
			case "lambda":
			case "try":
			case "except":
			case "break":
			case "from":
			case "continue":
			case "super":
			case "enum":
			case "raise":
			case "interface":
			case "given":
			case "case":
			case "yield":
			case "default":
			case "return":
			case "match":
			case "when":
			case "with":
				return new Token (TokenClass.Keyword, accum.ToString (), location);
			case "is":
			case "isnot":
			case "as":
				return new Token (TokenClass.Operator, accum.ToString (), location);
			default:
				return new Token (TokenClass.Identifier, accum.ToString (), location);
			}
		}

		private Token ReadOperator ()
		{
			char op = (char)ReadChar ();
			string nextTwoChars = op + ((char)PeekChar ()).ToString ();
			string nextThreeChars = op + ((char)PeekChar ()).ToString () + ((char)PeekChar (1)).ToString ();

			switch (nextThreeChars) {
			case "<<=":
				ReadChar ();
				ReadChar ();
				return new Token (TokenClass.Operator, nextThreeChars, location);
			case ">>=":
				ReadChar ();
				ReadChar ();
				return new Token (TokenClass.Operator, nextThreeChars, location);
			case "...":
				ReadChar ();
				ReadChar ();
				return new Token (TokenClass.Operator, nextThreeChars, location);
			}

			switch (nextTwoChars) {
			case ">>":
			case "<<":
			case "&&":
			case "||":
			case "==":
			case "!=":
			case "=>":
			case "<=":
			case ">=":
			case "+=":
			case "-=":
			case "*=":
			case "/=":
			case "%=":
			case "^=":
			case "&=":
			case "|=":
			case "??":
			case "..":
			case "|>":
				ReadChar ();
				return new Token (TokenClass.Operator, nextTwoChars, location);
			default:
				return new Token (TokenClass.Operator, op.ToString (), location);
			}
		}

		private void EatWhiteSpaces ()
		{
			while (char.IsWhiteSpace ((char)PeekChar ())) {
				ReadChar ();
			}
		}

		private bool MatchString (string str)
		{
			for (int i = 0; i < str.Length; i++) {
				if (PeekChar (i) != str [i]) {
					return false;
				}
			}
			return true;
		}

		private void ReadChars (int n)
		{
			for (int i = 0; i < n; i++) {
				ReadChar ();
			}
		}

		private int ReadChar ()
		{
			if (position >= sourceLen) {
				return -1;
			}

			if (source [position] == '\n') {
				location = new Location (location.Line + 1, 0, this.file); 
			} else {
				location = new Location (location.Line, location.Column + 1,
					this.file); 
			}
			return source [position++];
		}

		private int PeekChar ()
		{
			return PeekChar (0);
		}

		private int PeekChar (int n)
		{
			if (position + n >= sourceLen) {
				return -1;
			}
			return source [position + n];
		}
	}
}

