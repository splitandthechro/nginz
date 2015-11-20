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
	public class IodineStringBuilder : IodineObject
	{
		public readonly static IodineTypeDefinition TypeDefinition = new StringBuilderTypeDef ();

		class StringBuilderTypeDef : IodineTypeDefinition
		{
			public StringBuilderTypeDef ()
				: base ("StringBuffer")
			{
			}

			public override IodineObject Invoke (VirtualMachine vm, IodineObject[] args)
			{
				return new IodineStringBuilder ();
			}
		}

		private StringBuilder buffer = new StringBuilder ();

		public IodineStringBuilder ()
			: base (TypeDefinition) {
			SetAttribute ("clear", new InternalMethodCallback (clear, null));
			SetAttribute ("append", new InternalMethodCallback (append, null));
		}

		public override string ToString ()
		{
			return buffer.ToString ();
		}

		public override IodineObject Len (VirtualMachine vm)
		{
			return new IodineInteger (buffer.Length);
		}

		public override IodineObject ToString (VirtualMachine vm)
		{
			return new IodineString (buffer.ToString ());
		}

		private IodineObject append (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			foreach (IodineObject obj in args) {
				buffer.Append (obj.ToString (vm));
			}
			return null;
		}

		private IodineObject clear (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			buffer.Clear ();
			return null;
		}


	}
}

