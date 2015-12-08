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
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Iodine.Runtime;

namespace Iodine.Engine
{
	static class MethodWrapper
	{
		public static InternalMethodCallback Create (TypeRegistry registry, 
			MethodInfo info,
			object self = null)
		{
			return new InternalMethodCallback (((VirtualMachine vm,
				IodineObject @this, IodineObject[] arguments) => {
				Type[] types = info.GetParameters ().Select (p => p.ParameterType).ToArray ();
				int i = 0;
				object[] objects = arguments.Select (p => registry.ConvertToNativeObject (p,
					types [i++])).ToArray ();
				return registry.ConvertToIodineObject (info.Invoke (self, objects));
			}), null);
		}

		public static InternalMethodCallback Create (TypeRegistry registry,
			IEnumerable<MethodInfo> info,
			object self = null)
		{
			return new InternalMethodCallback (((VirtualMachine vm,
				IodineObject @this, IodineObject[] arguments) => {
				var suitableOverloads = info.Where (p => p.GetParameters ().Length == arguments.Length);

				foreach (MethodInfo overload in suitableOverloads) {
					var types = overload.GetParameters ().Select (p => p.ParameterType).ToArray ();
					object[] objects = new object[arguments.Length];
					bool mappingExists = true;
					for (int i = 0; i < arguments.Length; i++) {
						if (!registry.TypeMappingExists (arguments [i].TypeDef, types [i])) {
							mappingExists = false;
							break;
						}
						objects [i] = registry.ConvertToNativeObject (arguments [i], types [i]);
					}

					if (mappingExists) {
						return registry.ConvertToIodineObject (overload.Invoke (self, objects));
					}
				}
				// No suitable overload found
				Console.WriteLine ("No suitable overload found!");
				return null;
			}), null);
		}
	
	}
}

