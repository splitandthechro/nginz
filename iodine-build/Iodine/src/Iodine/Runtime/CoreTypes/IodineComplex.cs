// /**
//   * Copyright (c) 2015, GruntTheDivine All rights reserved.
//
//   * Redistribution and use in source and binary forms, with or without modification,
//   * are permitted provided that the following conditions are met:
//   * 
//   *  * Redistributions of source code must retain the above copyright notice, this list
//   *    of conditions and the following disclaimer.
//   * 
//   *  * Redistributions in binary form must reproduce the above copyright notice, this
//   *    list of conditions and the following disclaimer in the documentation and/or
//   *    other materials provided with the distribution.
//
//   * Neither the name of the copyright holder nor the names of its contributors may be
//   * used to endorse or promote products derived from this software without specific
//   * prior written permission.
//   * 
//   * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
//   * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
//   * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT
//   * SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
//   * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
//   * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR
//   * BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
//   * CONTRACT ,STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
//   * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
//   * DAMAGE.
// /**
using System;
using System.Numerics;

namespace Iodine.Runtime
{
	public class IodineComplex : IodineObject
	{
		public static readonly IodineTypeDefinition TypeDefinition = new IodineComplexTypeDef ();

		class IodineComplexTypeDef : IodineTypeDefinition
		{
			public IodineComplexTypeDef ()
				: base ("Complex")
			{
			}

			public override IodineObject Invoke (VirtualMachine vm, IodineObject[] arguments)
			{
				return base.Invoke (vm, arguments);
			}
		}

		public readonly Complex Value;

		public IodineComplex (Complex complex)
			: base (TypeDefinition)
		{
			Value = complex;
		}

		public override IodineObject Add (VirtualMachine vm, IodineObject left)
		{
			Complex leftComplex;
			if (left is IodineInteger) {
				leftComplex = new Complex (((IodineInteger)left).Value, 0);
			} else if (left is IodineFloat) {
				leftComplex = new Complex (((IodineFloat)left).Value, 0);
			} else if (left is IodineComplex) {
				leftComplex = ((IodineComplex)left).Value;
			} else {
				vm.RaiseException (new IodineTypeException ("Complex"));
				return null;
			}
			return new IodineComplex (Value + leftComplex);
		}

		public override IodineObject Sub (VirtualMachine vm, IodineObject left)
		{
			Complex leftComplex;
			if (left is IodineInteger) {
				leftComplex = new Complex (((IodineInteger)left).Value, 0);
			} else if (left is IodineFloat) {
				leftComplex = new Complex (((IodineFloat)left).Value, 0);
			} else if (left is IodineComplex) {
				leftComplex = ((IodineComplex)left).Value;
			} else {
				vm.RaiseException (new IodineTypeException ("Complex"));
				return null;
			}
			return new IodineComplex (Value - leftComplex);
		}

		public override IodineObject Mul (VirtualMachine vm, IodineObject left)
		{
			Complex leftComplex;
			if (left is IodineInteger) {
				leftComplex = new Complex (((IodineInteger)left).Value, 0);
			} else if (left is IodineFloat) {
				leftComplex = new Complex (((IodineFloat)left).Value, 0);
			} else if (left is IodineComplex) {
				leftComplex = ((IodineComplex)left).Value;
			} else {
				vm.RaiseException (new IodineTypeException ("Complex"));
				return null;
			}
			return new IodineComplex (Value * leftComplex);
		}

		public override IodineObject Div (VirtualMachine vm, IodineObject left)
		{
			Complex leftComplex;
			if (left is IodineInteger) {
				leftComplex = new Complex (((IodineInteger)left).Value, 0);
			} else if (left is IodineFloat) {
				leftComplex = new Complex (((IodineFloat)left).Value, 0);
			} else if (left is IodineComplex) {
				leftComplex = ((IodineComplex)left).Value;
			} else {
				vm.RaiseException (new IodineTypeException ("Complex"));
				return null;
			}
			return new IodineComplex (Value / leftComplex);
		}
	}
}

