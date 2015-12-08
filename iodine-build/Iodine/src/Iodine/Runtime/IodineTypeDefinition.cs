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
	/// <summary>
	/// Base class for all custom Iodine types, the equivalent of C#'s Type class or
	/// Java's Class class. 
	/// </summary>
	public class IodineTypeDefinition : IodineObject
	{
		private static IodineTypeDefinition TypeDefinition = new IodineTypeDefinition ("TypeDef");

		public readonly string Name;

		public IodineTypeDefinition (string name)
			: base (TypeDefinition)
		{
			Name = name;
			Attributes ["__name__"] = new IodineString (name);
		}

		public override IodineObject Invoke (VirtualMachine vm, IodineObject[] arguments)
		{
			return base.Invoke (vm, arguments);
		}

		public override bool IsCallable ()
		{
			return true;
		}

		public override string ToString ()
		{
			return Name;
		}
			
		public virtual void Inherit (VirtualMachine vm, IodineObject self, IodineObject[] arguments)
		{
			IodineObject obj = this.Invoke (vm, arguments);
			foreach (string attr in Attributes.Keys) {
				if (!self.HasAttribute (attr))
					self.SetAttribute (attr, Attributes [attr]);
				obj.SetAttribute (attr, Attributes [attr]);
			}
			self.SetAttribute ("__super__", obj);
			self.Base = obj;
		}

		public IodineObject BindAttributes (IodineObject obj)
		{
			foreach (KeyValuePair<string, IodineObject> kv in Attributes) {
				if (!obj.HasAttribute (kv.Key))
					obj.SetAttribute (kv.Key, kv.Value);
			}
			return obj;
		}

	}
}

