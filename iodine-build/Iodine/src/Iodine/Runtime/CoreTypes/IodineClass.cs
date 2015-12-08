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
using System.Collections.Generic;

namespace Iodine.Runtime
{
	public class IodineClass : IodineTypeDefinition
	{
		private bool initializerInvoked = false;

		public IodineMethod Initializer { private set; get; }

		public IodineMethod Constructor { private set; get; }

		public IodineClass (string name, IodineMethod initializer, IodineMethod constructor)
			: base (name)
		{
			Constructor = constructor;
			Initializer = initializer;
		}
			
		public void AddInstanceMethod (IodineMethod method)
		{
			SetAttribute (method.Name, method);
		}

		public override IodineObject GetAttribute (VirtualMachine vm, string name)
		{
			if (!initializerInvoked) {
				initializerInvoked = true;
				Initializer.Invoke (vm, new IodineObject[] { });
			}
			return base.GetAttribute (vm, name);
		}

		public override void SetAttribute (VirtualMachine vm, string name, IodineObject value)
		{
			if (!initializerInvoked) {
				initializerInvoked = true;
				Initializer.Invoke (vm, new IodineObject[] { });
			}
			base.SetAttribute (vm, name, value);
		}

		public override bool IsCallable ()
		{
			return true;
		}

		public override IodineObject Invoke (VirtualMachine vm, IodineObject[] arguments)
		{
			if (!initializerInvoked) {
				initializerInvoked = true;
				Initializer.Invoke (vm, new IodineObject[] { });
			}
			IodineObject obj = new IodineObject (this);
			BindAttributes (obj);
			vm.InvokeMethod (Constructor, obj, arguments);
			return obj;
		}

		public override void Inherit (VirtualMachine vm, IodineObject self, IodineObject[] arguments)
		{
			IodineObject obj = Invoke (vm, arguments);

			foreach (KeyValuePair<string, IodineObject> kv in Attributes) {
				if (!self.HasAttribute (kv.Key))
					self.SetAttribute (kv.Key, kv.Value);
				if (!obj.HasAttribute (kv.Key))
					obj.SetAttribute (kv.Key, kv.Value);
			}
			self.SetAttribute ("__super__", obj);
			self.Base = obj;
		}

		public override IodineObject Represent (VirtualMachine vm)
		{
			return new IodineString (string.Format ("<Class {0}>", Name));
		}

		public override string ToString ()
		{
			return Name;
		}
	}
}

