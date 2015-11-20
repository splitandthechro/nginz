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
using System.Linq;
using Iodine.Compiler;

namespace Iodine.Runtime
{
	public class IodineBytes : IodineObject
	{
		public static readonly IodineTypeDefinition TypeDefinition = new BytesTypeDef ();

		class BytesTypeDef : IodineTypeDefinition
		{
			public BytesTypeDef ()
				: base ("Bytes")
			{
			}

			public override IodineObject Invoke (VirtualMachine vm, IodineObject[] args)
			{
				if (args.Length <= 0) {
					vm.RaiseException (new IodineArgumentException (1));
				}
				return new IodineBytes (args [0].ToString ());
			}
		}

		private int iterIndex = 0;

		public byte[] Value { private set; get; }

		public IodineBytes ()
			: base (TypeDefinition)
		{
		}

		public IodineBytes (byte[] val)
			: this ()
		{
			Value = val;
		}

		public IodineBytes (string val)
			: this ()
		{
			Value = Encoding.ASCII.GetBytes (val);
		}

		public override IodineObject Len (VirtualMachine vm)
		{
			return new IodineInteger (Value.Length);
		}

		public override IodineObject Add (VirtualMachine vm, IodineObject right)
		{
			IodineBytes str = right as IodineBytes;
			if (str == null) {
				vm.RaiseException ("Right hand value must be of type Bytes!");
				return null;
			}
			byte[] newArr = new byte[str.Value.Length + Value.Length];
			Array.Copy (Value, newArr, Value.Length);
			Array.Copy (str.Value, 0, newArr, Value.Length, str.Value.Length);
			return new IodineBytes (newArr);
		}

		public override IodineObject Equals (VirtualMachine vm, IodineObject right)
		{
			IodineBytes str = right as IodineBytes;
			if (str == null) {
				return base.Equals (vm, right);
			}
			return IodineBool.Create (Enumerable.SequenceEqual<byte> (str.Value, Value));
		}

		public override IodineObject NotEquals (VirtualMachine vm, IodineObject right)
		{
			IodineBytes str = right as IodineBytes;
			if (str == null) {
				return base.NotEquals (vm, right);
			}
			return IodineBool.Create (!Enumerable.SequenceEqual<byte> (str.Value, Value));
		}

		public override string ToString ()
		{
			return Encoding.ASCII.GetString (Value);
		}

		public override int GetHashCode ()
		{
			return Value.GetHashCode ();
		}

		public override IodineObject GetIndex (VirtualMachine vm, IodineObject key)
		{
			IodineInteger index = key as IodineInteger;
			if (index == null) {
				vm.RaiseException (new IodineTypeException ("Int"));
				return null;
			}
			if (index.Value >= this.Value.Length) {
				vm.RaiseException (new IodineIndexException ());
				return null;
			}
			return new IodineInteger ((long)Value [(int)index.Value]);
		}

		public override IodineObject IterGetCurrent (VirtualMachine vm)
		{
			return new IodineInteger ((long)Value [iterIndex - 1]);
		}

		public override bool IterMoveNext (VirtualMachine vm)
		{
			if (iterIndex >= Value.Length) {
				return false;
			}
			iterIndex++;
			return true;
		}

		public override void IterReset (VirtualMachine vm)
		{
			iterIndex = 0;
		}
	}
}

