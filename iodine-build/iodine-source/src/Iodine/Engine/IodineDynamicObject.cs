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
using System.Dynamic;
using Iodine.Runtime;

namespace Iodine
{
	public class IodineDynamicObject : DynamicObject
	{
		private IodineObject internalObject;
		private VirtualMachine internalVm;

		internal IodineDynamicObject (IodineObject obj, VirtualMachine vm)
		{
			internalObject = obj;
			internalVm = vm;
		}

		public override bool TryGetMember (GetMemberBinder binder, out object result)
		{
			if (internalObject.HasAttribute (binder.Name)) {
				IodineObject obj = internalObject.GetAttribute (binder.Name);
				if (!IodineTypeConverter.Instance.ConvertToPrimative (obj, out result)) {
					result = new IodineDynamicObject (obj, internalVm);
				}
				return true;
			}
			result = null;
			return true;
		}

		public override bool TrySetMember (SetMemberBinder binder, object value)
		{
			IodineObject val = null;
			if (!IodineTypeConverter.Instance.ConvertFromPrimative (value, out val)) {
				if (value is IodineObject) {
					val = (IodineObject)value;
				} else {
					return false;
				}
			}
			internalObject.SetAttribute (binder.Name, val);
			return true;
		}

		public override bool TryInvoke (InvokeBinder binder, object[] args, out object result)
		{
			IodineObject[] arguments = new IodineObject[args.Length];
			for (int i = 0; i < args.Length; i++) {
				IodineObject val = null;
				if (!IodineTypeConverter.Instance.ConvertFromPrimative (args [i], out val)) {
					if (args [i] is IodineObject) {
						val = (IodineObject)args [i];
					} else {
						result = null;
						return false;
					}
				}
				arguments [i] = val;
			}
			IodineObject returnVal = internalObject.Invoke (internalVm, arguments);
			if (!IodineTypeConverter.Instance.ConvertToPrimative (returnVal, out result)) {
				result = new IodineDynamicObject (returnVal, internalVm);
			}
			return true;
		}

		public override string ToString ()
		{
			return internalObject.Represent (internalVm).ToString ();
		}
	}
}

