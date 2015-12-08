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
using Iodine.Compiler;

namespace Iodine.Runtime
{
	public class IodineFloat : IodineObject
	{
		public static readonly IodineTypeDefinition TypeDefinition = new FloatTypeDef ();

		class FloatTypeDef : IodineTypeDefinition
		{
			public FloatTypeDef ()
				: base ("Float")
			{
			}

			public override IodineObject Invoke (VirtualMachine vm, IodineObject[] args)
			{
				if (args.Length <= 0) {
					vm.RaiseException (new IodineArgumentException (1));
				}

				return new IodineFloat (Double.Parse (args [0].ToString ()));
			}
		}

		public double Value { private set; get; }

		public IodineFloat (double val)
			: base (TypeDefinition)
		{
			Value = val;
		}

		public override IodineObject Add (VirtualMachine vm, IodineObject right)
		{
			double floatVal;
			if (!(TryConvertToFloat (right, out floatVal))) {
				vm.RaiseException (new IodineTypeException (
					"Right hand value expected to be of type Float"));
				return null;
			}
			return new IodineFloat (Value + floatVal);
		}

		public override IodineObject Sub (VirtualMachine vm, IodineObject right)
		{
			double floatVal;
			if (!(TryConvertToFloat (right, out floatVal))) {
				vm.RaiseException (new IodineTypeException (
					"Right hand value expected to be of type Float"));
				return null;
			}
			return new IodineFloat (Value - floatVal);
		}

		public override IodineObject Div (VirtualMachine vm, IodineObject right)
		{
			double floatVal;
			if (!(TryConvertToFloat (right, out floatVal))) {
				vm.RaiseException (new IodineTypeException (
					"Right hand value expected to be of type Float"));
				return null;
			}
			return new IodineFloat (Value / floatVal);
		}

		public override IodineObject Mod (VirtualMachine vm, IodineObject right)
		{
			double floatVal;
			if (!(TryConvertToFloat (right, out floatVal))) {
				vm.RaiseException (new IodineTypeException (
					"Right hand value expected to be of type Float"));
				return null;
			}
			return new IodineFloat (Value % floatVal);
		}

		public override IodineObject Equals (VirtualMachine vm, IodineObject right)
		{
			double floatVal;
			if (!(TryConvertToFloat (right, out floatVal))) {
				vm.RaiseException (new IodineTypeException (
					"Right hand value expected to be of type Float"));
				return null;
			}
			return IodineBool.Create (Value == floatVal);
		}

		public override IodineObject NotEquals (VirtualMachine vm, IodineObject right)
		{
			double floatVal;
			if (!(TryConvertToFloat (right, out floatVal))) {
				vm.RaiseException (new IodineTypeException (
					"Right hand value expected to be of type Float"));
				return null;
			}
			return IodineBool.Create (Value != floatVal);
		}


		public override IodineObject GreaterThan (VirtualMachine vm, IodineObject right)
		{
			double floatVal;
			if (!(TryConvertToFloat (right, out floatVal))) {
				vm.RaiseException (new IodineTypeException (
					"Right hand value expected to be of type Float"));
				return null;
			}
			return IodineBool.Create (Value > floatVal);
		}


		public override IodineObject GreaterThanOrEqual (VirtualMachine vm, IodineObject right)
		{
			double floatVal;
			if (!(TryConvertToFloat (right, out floatVal))) {
				vm.RaiseException (new IodineTypeException (
					"Right hand value expected to be of type Float"));
				return null;
			}
			return IodineBool.Create (Value >= floatVal);
		}
			
		public override IodineObject LessThan (VirtualMachine vm, IodineObject right)
		{
			double floatVal;
			if (!(TryConvertToFloat (right, out floatVal))) {
				vm.RaiseException (new IodineTypeException (
					"Right hand value expected to be of type Float"));
				return null;
			}
			return IodineBool.Create (Value < floatVal);
		}
			
		public override IodineObject LessThanOrEqual (VirtualMachine vm, IodineObject right)
		{
			double floatVal;
			if (!(TryConvertToFloat (right, out floatVal))) {
				vm.RaiseException (new IodineTypeException (
					"Right hand value expected to be of type Float"));
				return null;
			}
			return IodineBool.Create (Value <= floatVal);
		}

		public override IodineObject PerformUnaryOperation (VirtualMachine vm, UnaryOperation op)
		{
			switch (op) {
			case UnaryOperation.Negate:
				return new IodineFloat (-Value);
			}
			return null;
		}

		private bool TryConvertToFloat (IodineObject obj, out double result)
		{
			if (obj is IodineFloat) {
				result = ((IodineFloat)obj).Value;
				return true;
			} else if (obj is IodineInteger) {
				result = (double)((IodineInteger)obj).Value;
				return true;
			}
			result = 0;
			return false;
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

