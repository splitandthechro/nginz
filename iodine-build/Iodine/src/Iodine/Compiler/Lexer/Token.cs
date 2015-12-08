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

namespace Iodine.Compiler
{
	public enum TokenClass
	{
		Identifier,
		StringLiteral,
		InterpolatedStringLiteral,
		IntLiteral,
		FloatLiteral,
		Keyword,
		OpenParan,
		CloseParan,
		OpenBrace,
		CloseBrace,
		OpenBracket,
		CloseBracket,
		SemiColon,
		Colon,
		Operator,
		Comma
	}

	/// <summary>
	/// Token.
	/// </summary>
	public sealed class Token
	{
		public readonly string Value;
		public readonly TokenClass Class;
		public readonly SourceLocation Location;

		/// <summary>
		/// Initializes a new instance of the <see cref="Iodine.Compiler.Token"/> class.
		/// </summary>
		/// <param name="clazz">Token class.</param>
		/// <param name="value">Value.</param>
		/// <param name="location">Location.</param>
		public Token (TokenClass clazz, string value, SourceLocation location)
		{
			Class = clazz;
			Value = value;
			Location = location;
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString ()
		{
			return Value.ToString ();
		}

		/// <summary>
		/// Converts a TokenClass enum to its string representation
		/// </summary>
		/// <returns>The string representation of the TokenClass enum.</returns>
		/// <param name="clazz">Token class.</param>
		public static string ClassToString (TokenClass clazz)
		{
			switch (clazz) {
			case TokenClass.CloseBrace:
				return "}";
			case TokenClass.OpenBrace:
				return "{";
			case TokenClass.CloseParan:
				return ")";
			case TokenClass.OpenParan:
				return "(";
			case TokenClass.Comma:
				return ",";
			case TokenClass.OpenBracket:
				return "[";
			case TokenClass.CloseBracket:
				return "]";
			case TokenClass.SemiColon:
				return ";";
			case TokenClass.Colon:
				return ":";
			default:
				return clazz.ToString ();
			}
		}
	}
}

