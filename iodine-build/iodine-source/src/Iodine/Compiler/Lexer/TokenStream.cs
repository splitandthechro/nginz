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

namespace Iodine.Compiler
{
	public sealed class TokenStream
	{
		private ErrorLog errorLog;
		private List<Token> tokens = new List<Token> ();

		public bool EndOfStream {
			get {
				return tokens.Count <= Position;
			}
		}

		public int Position { private set; get; }

		public Token Current {
			get {
				return PeekToken ();
			}
		}

		public Location Location {
			get {
				if (PeekToken () != null)
					return PeekToken ().Location;
				else if (tokens.Count == 0) {
					return new Location (0, 0, "");
				}
				return PeekToken (-1).Location;

			}
		}

		public ErrorLog ErrorLog {
			get {
				return errorLog;
			}
		}

		public TokenStream (ErrorLog errorLog)
		{
			this.errorLog = errorLog;
		}

		public void AddToken (Token token)
		{
			tokens.Add (token);
		}

		public bool Match (TokenClass clazz)
		{
			return PeekToken () != null && PeekToken ().Class == clazz;
		}

		public bool Match (TokenClass clazz1, TokenClass clazz2)
		{
			return PeekToken () != null && PeekToken ().Class == clazz1 &&
				PeekToken (1) != null &&
				PeekToken (1).Class == clazz2;
		}

		public bool Match (TokenClass clazz, string val)
		{
			return PeekToken () != null &&
				PeekToken ().Class == clazz &&
				PeekToken ().Value == val;
		}

		public bool Accept (TokenClass clazz)
		{
			if (PeekToken () != null && PeekToken ().Class == clazz) {
				ReadToken ();
				return true;
			}
			return false;
		}

		public bool Accept (TokenClass clazz, ref Token token)
		{
			if (PeekToken () != null && PeekToken ().Class == clazz) {
				token = ReadToken ();
				return true;
			}
			return false;
		}

		public bool Accept (TokenClass clazz, string val)
		{
			if (PeekToken () != null && PeekToken ().Class == clazz && PeekToken ().Value == val) {
				ReadToken ();
				return true;
			}
			return false;
		}

		public Token Expect (TokenClass clazz)
		{
			Token ret = null;
			if (Accept (clazz, ref ret)) {
				return ret;
			}
			Token offender = ReadToken ();
			if (offender != null) {
				errorLog.AddError (ErrorType.ParserError, offender.Location,
					"Unexpected '{0}' (Expected '{1}')",
					offender.ToString (),
					Token.ClassToString (clazz));
			} else {
				errorLog.AddError (ErrorType.ParserError, offender.Location,
					"Unexpected end of file (Expected {0})",
					Token.ClassToString (clazz));
				throw new Exception ("");
			}
			return new Token (clazz, "", Location);
		}

		public Token Expect (TokenClass clazz, string val)
		{
			Token ret = PeekToken ();
			if (Accept (clazz, val)) {
				return ret;
			}
			Token offender = ReadToken ();
			if (offender != null) {
				errorLog.AddError (ErrorType.ParserError, offender.Location, 
					"Unexpected '{0}' (Expected '{1}')", offender.ToString (), Token.ClassToString (
					clazz));
			} else {
				errorLog.AddError (ErrorType.ParserError, offender.Location, 
					"Unexpected end of file (Expected {0})", Token.ClassToString (clazz));
				throw new Exception ("");
			}
			return new Token (clazz, "", Location);
		}

		public void MakeError ()
		{
			errorLog.AddError (ErrorType.ParserError, PeekToken ().Location, "Unexpected {0}",
				ReadToken ().ToString ());
		}

		private Token PeekToken ()
		{
			return PeekToken (0);
		}

		private Token PeekToken (int n)
		{
			if (Position + n < tokens.Count) {
				return tokens [Position + n];
			}
			return null;
		}

		public Token ReadToken ()
		{
			if (Position >= tokens.Count) {
				return null;
			}
			return tokens [Position++];
		}
	}
}

