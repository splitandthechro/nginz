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
	public class IodineByteArray : IodineObject
	{
		public static readonly IodineTypeDefinition TypeDefinition = new ByteArrayTypeDef ();

		class ByteArrayTypeDef : IodineTypeDefinition
		{
			public ByteArrayTypeDef ()
				: base ("ByteArray")
			{
			}

			public override IodineObject Invoke (VirtualMachine vm, IodineObject[] args)
			{
				if (args.Length <= 0) {
					vm.RaiseException (new IodineArgumentException (1));
					return null;
				}
				IodineInteger size = args [0] as IodineInteger;
				return new IodineByteArray (new byte [size.Value]);
			}
		}

		public byte[] Array { private set; get; }

		private int iterIndex = 0;

		public IodineByteArray (byte[] bytes)
			: base (TypeDefinition)
		{
			Array = bytes;
			SetAttribute ("getSize", new InternalMethodCallback (getSize, this));
		}

		public override IodineObject Len (VirtualMachine vm)
		{
			return new IodineInteger (Array.Length);
		}

		public override void SetIndex (VirtualMachine vm, IodineObject key, IodineObject value)
		{
			IodineInteger index = key as IodineInteger;
			IodineInteger val = value as IodineInteger;
			if (index.Value < Array.Length)
				Array [(int)index.Value] = (byte)(val.Value & 0xFF);
			else
				vm.RaiseException (new IodineIndexException ());
		}

		public override IodineObject GetIndex (VirtualMachine vm, IodineObject key)
		{
			IodineInteger index = key as IodineInteger;
			if (index.Value < Array.Length)
				return new IodineInteger (Array [(int)index.Value]);
			vm.RaiseException (new IodineIndexException ());
			return null;
		}

		public override IodineObject IterGetCurrent (VirtualMachine vm)
		{
			return new IodineInteger (Array [iterIndex - 1]);
		}

		public override bool IterMoveNext (VirtualMachine vm)
		{
			if (iterIndex >= Array.Length)
				return false;
			iterIndex++;
			return true;
		}

		public override void IterReset (VirtualMachine vm)
		{
			iterIndex = 0;
		}

		private IodineObject getSize (VirtualMachine vm, IodineObject self, IodineObject[] arguments)
		{
			return new IodineInteger (Array.Length);
		}

		public override int GetHashCode ()
		{
			return Array.GetHashCode ();
		}

		public override string ToString ()
		{
			StringBuilder accum = new StringBuilder ();
			foreach (byte b in this.Array) {
				accum.Append (b.ToString ("x2"));
			}
			return accum.ToString ();
		}
	}
}

