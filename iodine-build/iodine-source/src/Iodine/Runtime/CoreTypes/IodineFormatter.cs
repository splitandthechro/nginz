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

namespace Iodine.Runtime
{
	public class IodineFormatter : IodineObject
	{
		public static IodineTypeDefinition FormatterTypeDef = new IodineTypeDefinition ("Formatter");

		public IodineFormatter ()
			: base (FormatterTypeDef)
		{
		}

		public string Format (VirtualMachine vm, string format, IodineObject[] args)
		{
			StringBuilder accum = new StringBuilder ();
			int nextArg = 0;
			int pos = 0;
			while (pos < format.Length) {
				if (format [pos] == '{') {
					string substr = format.Substring (pos + 1);
					if (substr.IndexOf ('}') == -1) {
						return null;
					}
					substr = substr.Substring (0, substr.IndexOf ('}'));
					pos += substr.Length + 2;
					if (substr.Length == 0) {
						accum.Append (args [nextArg++].ToString ());
					} else {
						int index = 0;
						string indexStr = "";
						string specifier = "";

						if (substr.IndexOf (':') == -1) {
							indexStr = substr;
						} else {
							indexStr = substr.Substring (0, substr.IndexOf (":"));
							specifier = substr.Substring (substr.IndexOf (":") + 1);
						}

						if (indexStr == "") {
							index = nextArg++;
						} else if (!Int32.TryParse (indexStr, out index)) {
							return null;
						}

						accum.Append (formatObj (args [index], specifier));

					}
				} else {
					accum.Append (format [pos++]);
				}
			}
			return accum.ToString ();
		}

		private string formatObj (IodineObject obj, string specifier)
		{
			if (specifier.Length == 0) {
				return obj.ToString ();
			}
			char type = specifier [0];
			string args = specifier.Substring (1);
			switch (char.ToLower (type)) {
			case 'd':
				{
					IodineInteger intObj = obj as IodineInteger;
					int pad = args.Length == 0 ? 0 : int.Parse (args);
					if (intObj == null)
						return null;
					return intObj.Value.ToString (type.ToString () + pad);
				}
			case 'x':
				{
					IodineInteger intObj = obj as IodineInteger;
					int pad = args.Length == 0 ? 0 : int.Parse (args);
					if (intObj == null)
						return null;
					return intObj.Value.ToString (type.ToString () + pad);
				}
			default:
				return null;
			}
		}
	}
}

