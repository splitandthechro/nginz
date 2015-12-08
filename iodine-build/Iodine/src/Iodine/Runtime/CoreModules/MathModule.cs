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
using System.IO;
using System.Reflection;

namespace Iodine.Runtime
{
	[IodineBuiltinModule ("math")]
	public class MathModule : IodineModule
	{
		public MathModule ()
			: base ("math")
		{
			SetAttribute ("PI", new IodineFloat (Math.PI));
			SetAttribute ("E", new IodineFloat (Math.E));
			SetAttribute ("pow", new InternalMethodCallback (pow, this));
			SetAttribute ("sin", new InternalMethodCallback (sin, this));
			SetAttribute ("cos", new InternalMethodCallback (cos, this));
			SetAttribute ("tan", new InternalMethodCallback (tan, this));
			SetAttribute ("asin", new InternalMethodCallback (asin, this));
			SetAttribute ("acos", new InternalMethodCallback (acos, this));
			SetAttribute ("atan", new InternalMethodCallback (atan, this));
			SetAttribute ("abs", new InternalMethodCallback (abs, this));
			SetAttribute ("sqrt", new InternalMethodCallback (sqrt, this));
			SetAttribute ("floor", new InternalMethodCallback (floor, this));
			SetAttribute ("ceiling", new InternalMethodCallback (ceiling, this));
			SetAttribute ("log", new InternalMethodCallback (log, this));
		}

		private IodineObject pow (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length <= 1) {
				vm.RaiseException (new IodineArgumentException (2));
				return null;
			}

			double a1 = 0;
			double a2 = 0;
			if (args [0] is IodineInteger) {
				a1 = (double)((IodineInteger)args [0]).Value;
			} else if (args [0] is IodineFloat) {
				a1 = ((IodineFloat)args [0]).Value;
			} else {
				vm.RaiseException (new IodineTypeException ("Float"));
				return null;
			}

			if (args [1] is IodineInteger) {
				a2 = (double)((IodineInteger)args [1]).Value;
			} else if (args [1] is IodineFloat) {
				a2 = ((IodineFloat)args [1]).Value;
			} else {
				vm.RaiseException (new IodineTypeException ("Float"));
				return null;
			}
			return new IodineFloat (Math.Pow (a1, a2));
		}

		private IodineObject sin (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}

			double input = 0;
			if (args [0] is IodineInteger) {
				input = (double)((IodineInteger)args [0]).Value;
			} else if (args [0] is IodineFloat) {
				input = ((IodineFloat)args [0]).Value;
			} else {
				vm.RaiseException (new IodineTypeException ("Float"));
				return null;
			}

			return new IodineFloat (Math.Sin (input));
		}

		private IodineObject cos (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}

			double input = 0;
			if (args [0] is IodineInteger) {
				input = (double)((IodineInteger)args [0]).Value;
			} else if (args [0] is IodineFloat) {
				input = ((IodineFloat)args [0]).Value;
			} else {
				vm.RaiseException (new IodineTypeException ("Float"));
				return null;
			}

			return new IodineFloat (Math.Cos (input));
		}

		private IodineObject tan (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}

			double input = 0;
			if (args [0] is IodineInteger) {
				input = (double)((IodineInteger)args [0]).Value;
			} else if (args [0] is IodineFloat) {
				input = ((IodineFloat)args [0]).Value;
			} else {
				vm.RaiseException (new IodineTypeException ("Float"));
				return null;
			}

			return new IodineFloat (Math.Tan (input));
		}

		private IodineObject asin (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}

			double input = 0;
			if (args [0] is IodineInteger) {
				input = (double)((IodineInteger)args [0]).Value;
			} else if (args [0] is IodineFloat) {
				input = ((IodineFloat)args [0]).Value;
			} else {
				vm.RaiseException (new IodineTypeException ("Float"));
				return null;
			}

			return new IodineFloat (Math.Asin (input));
		}

		private IodineObject acos (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}

			double input = 0;
			if (args [0] is IodineInteger) {
				input = (double)((IodineInteger)args [0]).Value;
			} else if (args [0] is IodineFloat) {
				input = ((IodineFloat)args [0]).Value;
			} else {
				vm.RaiseException (new IodineTypeException ("Float"));
				return null;
			}

			return new IodineFloat (Math.Acos (input));
		}

		private IodineObject atan (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}

			double input = 0;
			if (args [0] is IodineInteger) {
				input = (double)((IodineInteger)args [0]).Value;
			} else if (args [0] is IodineFloat) {
				input = ((IodineFloat)args [0]).Value;
			} else {
				vm.RaiseException (new IodineTypeException ("Float"));
				return null;
			}

			return new IodineFloat (Math.Atan (input));
		}

		private IodineObject abs (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}

			double input = 0;
			if (args [0] is IodineInteger) {
				input = (double)((IodineInteger)args [0]).Value;
			} else if (args [0] is IodineFloat) {
				input = ((IodineFloat)args [0]).Value;
			} else {
				vm.RaiseException (new IodineTypeException ("Float"));
				return null;
			}

			return new IodineFloat (Math.Abs (input));
		}

		private IodineObject sqrt (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}

			double input = 0;
			if (args [0] is IodineInteger) {
				input = (double)((IodineInteger)args [0]).Value;
			} else if (args [0] is IodineFloat) {
				input = ((IodineFloat)args [0]).Value;
			} else {
				vm.RaiseException (new IodineTypeException ("Float"));
				return null;
			}

			return new IodineFloat (Math.Sqrt (input));
		}

		private IodineObject floor (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}

			double input = 0;
			if (args [0] is IodineInteger) {
				input = (double)((IodineInteger)args [0]).Value;
			} else if (args [0] is IodineFloat) {
				input = ((IodineFloat)args [0]).Value;
			} else {
				vm.RaiseException (new IodineTypeException ("Float"));
				return null;
			}

			return new IodineFloat (Math.Floor (input));
		}

		private IodineObject ceiling (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}

			double input = 0;
			if (args [0] is IodineInteger) {
				input = (double)((IodineInteger)args [0]).Value;
			} else if (args [0] is IodineFloat) {
				input = ((IodineFloat)args [0]).Value;
			} else {
				vm.RaiseException (new IodineTypeException ("Float"));
				return null;
			}

			return new IodineFloat (Math.Ceiling (input));
		}

		private IodineObject log (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}

			double input = 0;
			if (args [0] is IodineInteger) {
				input = (double)((IodineInteger)args [0]).Value;
			} else if (args [0] is IodineFloat) {
				input = ((IodineFloat)args [0]).Value;
			} else {
				vm.RaiseException (new IodineTypeException ("Float"));
				return null;
			}

			return new IodineFloat (Math.Log (input));
		}
	}
}

