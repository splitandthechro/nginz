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

namespace Iodine.Runtime
{
	public class IodineProperty : IodineObject, IIodineProperty
	{
		public readonly static IodineTypeDefinition TypeDefinition = new IodineTypeDefinition ("Property");

		public readonly IodineObject Setter;
		public readonly IodineObject Getter;

		private IodineObject self;

		public IodineProperty (IodineObject getter, IodineObject setter, IodineObject self)
			: base (TypeDefinition) {
			Setter = setter;
			Getter = getter;
			this.self = self;
		}

		public IodineObject Set (VirtualMachine vm, IodineObject value)
		{
			if (Setter is IodineMethod) {
				return vm.InvokeMethod ((IodineMethod)Setter, self, new IodineObject[] { value });
			} else if (Setter is IodineInstanceMethodWrapper) {
				return vm.InvokeMethod (((IodineInstanceMethodWrapper)Setter).Method, self,
					new IodineObject[] { value });
			}
			return Setter.Invoke (vm, new IodineObject[] { value });
		}

		public IodineObject Get (VirtualMachine vm)
		{
			if (Getter is IodineMethod) {
				return vm.InvokeMethod ((IodineMethod)Getter, self, new IodineObject[0]);
			} else if (Getter is IodineInstanceMethodWrapper) {
				return vm.InvokeMethod (((IodineInstanceMethodWrapper)Getter).Method, self,
					new IodineObject[0]);
			}
			return Getter.Invoke (vm, new IodineObject[0]);
		}
	}
}

