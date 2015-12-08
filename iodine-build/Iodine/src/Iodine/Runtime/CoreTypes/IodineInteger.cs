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
using System.Globalization;
using Iodine.Compiler;

namespace Iodine.Runtime
{
	public sealed class IodineInteger : IodineObject
	{
		public static readonly IodineTypeDefinition TypeDefinition = new IntTypeDef ();

		class IntTypeDef : IodineTypeDefinition
		{
			public IntTypeDef ()
				: base ("Int")
			{
			}

			public override IodineObject Invoke (VirtualMachine vm, IodineObject[] args)
			{
				if (args.Length <= 0) {
					vm.RaiseException (new IodineArgumentException (1));
				}

				if (args [0] is IodineFloat) {
					IodineFloat fp = args [0] as IodineFloat;
					return new IodineInteger ((long)fp.Value);
				}

				long value;
				NumberStyles style = NumberStyles.AllowLeadingSign;

				if (args.Length > 1) {
					IodineInteger basen = args [1] as IodineInteger;
					switch (basen.Value) {
					case 16:
						style = NumberStyles.HexNumber;
						break;
					}
				}

				if (!Int64.TryParse (args [0].ToString (), style, null, out value)) {
					vm.RaiseException (new IodineTypeCastException ("Int"));
					return null;
				} else {
					return new IodineInteger (value);
				}
			}
		}

		public readonly long Value;

		public IodineInteger (long val)
			: base (TypeDefinition)
		{
			Value = val;
		}

		#region Operator implementations
		public override IodineObject Add (VirtualMachine vm, IodineObject right)
		{
			IodineInteger intVal = right as IodineInteger;
			if (intVal == null) {
				vm.RaiseException (new IodineTypeException ("Right hand side must be of type Int!"));
				return null;
			}
			return new IodineInteger (Value + intVal.Value);
		}

		public override IodineObject Sub (VirtualMachine vm, IodineObject right)
		{
			IodineInteger intVal = right as IodineInteger;
			if (intVal == null) {
				vm.RaiseException (new IodineTypeException ("Right hand side must be of type Int!"));
				return null;
			}
			return new IodineInteger (Value - intVal.Value);
		}

		public override IodineObject Mul (VirtualMachine vm, IodineObject right)
		{
			IodineInteger intVal = right as IodineInteger;
			if (intVal == null) {
				vm.RaiseException (new IodineTypeException ("Right hand side must be of type Int!"));
				return null;
			}
			return new IodineInteger (Value * intVal.Value);
		}

		public override IodineObject Div (VirtualMachine vm, IodineObject right)
		{
			IodineInteger intVal = right as IodineInteger;
			if (intVal == null) {
				vm.RaiseException (new IodineTypeException ("Right hand side must be of type Int!"));
				return null;
			}
			return new IodineInteger (Value / intVal.Value);
		}

		public override IodineObject Mod (VirtualMachine vm, IodineObject right)
		{
			IodineInteger intVal = right as IodineInteger;
			if (intVal == null) {
				vm.RaiseException (new IodineTypeException ("Right hand side must be of type Int!"));
				return null;
			}
			return new IodineInteger (Value % intVal.Value);
		}

		public override IodineObject And (VirtualMachine vm, IodineObject right)
		{
			IodineInteger intVal = right as IodineInteger;
			if (intVal == null) {
				vm.RaiseException (new IodineTypeException ("Right hand side must be of type Int!"));
				return null;
			}
			return new IodineInteger (Value & intVal.Value);
		}

		public override IodineObject Or (VirtualMachine vm, IodineObject right)
		{
			IodineInteger intVal = right as IodineInteger;
			if (intVal == null) {
				vm.RaiseException (new IodineTypeException ("Right hand side must be of type Int!"));
				return null;
			}
			return new IodineInteger (Value | intVal.Value);
		}

		public override IodineObject Xor (VirtualMachine vm, IodineObject right)
		{
			IodineInteger intVal = right as IodineInteger;
			if (intVal == null) {
				vm.RaiseException (new IodineTypeException ("Right hand side must be of type Int!"));
				return null;
			}
			return new IodineInteger (Value ^ intVal.Value);
		}

		public override IodineObject LeftShift (VirtualMachine vm, IodineObject right)
		{
			IodineInteger intVal = right as IodineInteger;
			if (intVal == null) {
				vm.RaiseException (new IodineTypeException ("Right hand side must be of type Int!"));
				return null;
			}
			return new IodineInteger (Value << (int)intVal.Value);
		}

		public override IodineObject RightShift (VirtualMachine vm, IodineObject right)
		{
			IodineInteger intVal = right as IodineInteger;
			if (intVal == null) {
				vm.RaiseException (new IodineTypeException ("Right hand side must be of type Int!"));
				return null;
			}
			return new IodineInteger (Value >> (int)intVal.Value);
		}

		public override IodineObject Equals (VirtualMachine vm, IodineObject right)
		{
			IodineInteger intVal = right as IodineInteger;
			if (intVal == null) {
				vm.RaiseException (new IodineTypeException ("Right hand side must be of type Int!"));
				return null;
			}
			return IodineBool.Create (Value == intVal.Value);
		}

		public override IodineObject NotEquals (VirtualMachine vm, IodineObject right)
		{
			IodineInteger intVal = right as IodineInteger;
			if (intVal == null) {
				vm.RaiseException (new IodineTypeException ("Right hand side must be of type Int!"));
				return null;
			}
			return IodineBool.Create (Value != intVal.Value);
		}

		public override IodineObject GreaterThan (VirtualMachine vm, IodineObject right)
		{
			IodineInteger intVal = right as IodineInteger;
			if (intVal == null) {
				vm.RaiseException (new IodineTypeException ("Right hand side must be of type Int!"));
				return null;
			}
			return IodineBool.Create (Value > intVal.Value);
		}

		public override IodineObject GreaterThanOrEqual (VirtualMachine vm, IodineObject right)
		{
			IodineInteger intVal = right as IodineInteger;
			if (intVal == null) {
				vm.RaiseException (new IodineTypeException ("Right hand side must be of type Int!"));
			}
			return IodineBool.Create (Value >= intVal.Value);
		}

		public override IodineObject LessThan (VirtualMachine vm, IodineObject right)
		{
			IodineInteger intVal = right as IodineInteger;
			if (intVal == null) {
				vm.RaiseException (new IodineTypeException ("Right hand side must be of type Int!"));
				return null;
			}
			return IodineBool.Create (Value < intVal.Value);
		}

		public override IodineObject LessThanOrEqual (VirtualMachine vm, IodineObject right)
		{
			IodineInteger intVal = right as IodineInteger;
			if (intVal == null) {
				vm.RaiseException (new IodineTypeException ("Right hand side must be of type Int!"));
				return null;
			}
			return IodineBool.Create (Value <= intVal.Value);
		}

		public override IodineObject HalfRange (VirtualMachine vm, IodineObject right)
		{
			IodineInteger intVal = right as IodineInteger;
			if (intVal == null) {
				vm.RaiseException (new IodineTypeException ("Right hand side must be of type Int!"));
				return null;
			}
			return new IodineRange (Value, intVal.Value, 1);
		}

		public override IodineObject ClosedRange (VirtualMachine vm, IodineObject right)
		{
			IodineInteger intVal = right as IodineInteger;
			if (intVal == null) {
				vm.RaiseException (new IodineTypeException ("Right hand side must be of type Int!"));
				return null;
			}
			return new IodineRange (Value, intVal.Value + 1, 1);
		}
		#endregion

		public override IodineObject PerformUnaryOperation (VirtualMachine vm, UnaryOperation op)
		{
			switch (op) {
			case UnaryOperation.Not:
				return new IodineInteger (~Value);
			case UnaryOperation.Negate:
				return new IodineInteger (-Value);
			}
			return null;
		}

		public override bool IsTrue ()
		{
			return Value != 0;
		}

		public override string ToString ()
		{
			return Value.ToString ();
		}

		public override int GetHashCode ()
		{
			return Value.GetHashCode ();
		}
	}
}

