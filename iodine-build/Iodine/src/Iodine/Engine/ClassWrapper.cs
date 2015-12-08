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
using Iodine.Runtime;

namespace Iodine.Engine
{
	class ClassWrapper : IodineTypeDefinition
	{
		private Type type;
		private TypeRegistry typeRegistry;

		private ClassWrapper (TypeRegistry registry, Type type, string name)
			: base (name)
		{
			typeRegistry = registry;
			this.type = type;
		}

		public override IodineObject Invoke (VirtualMachine vm, IodineObject[] arguments)
		{
			int i = 0;

			var suitableOverload = type.GetConstructors ().Where (p => p.GetParameters ().Length ==
				arguments.Length).
				FirstOrDefault ();
			
			Type[] types = suitableOverload.GetParameters ().Select (p => p.ParameterType).ToArray ();

			object[] objects = arguments.Select (p => typeRegistry.ConvertToNativeObject (p,
				types [i++])).ToArray ();
			
			return ObjectWrapper.CreateFromObject (typeRegistry, this, suitableOverload.Invoke (objects));
		}

		public static ClassWrapper CreateFromType (TypeRegistry registry, Type type, string name)
		{
			ClassWrapper wrapper = new ClassWrapper (registry, type, name);

			foreach (MemberInfo info in type.GetMembers (BindingFlags.Public | BindingFlags.Static)) {
				switch (info.MemberType) {
				case MemberTypes.Method:
					if (!wrapper.HasAttribute (info.Name)) {
						
						wrapper.SetAttribute (info.Name, CreateMultiMethod (registry, type, info.Name));
					}
					break;
				case MemberTypes.Field:
					wrapper.SetAttribute (info.Name, FieldWrapper.Create (registry, (FieldInfo)info));
					break;
				case MemberTypes.Property:
					wrapper.SetAttribute (info.Name, PropertyWrapper.Create (registry, (PropertyInfo)info));
					break;
				}
			}

			registry.AddTypeMapping (type, wrapper, new ObjectTypeMapping (wrapper, registry));

			return wrapper;
		}

		private static InternalMethodCallback CreateMultiMethod (TypeRegistry registry,
			Type type,
			string name)
		{
			var methods = type.GetMembers (BindingFlags.Public | BindingFlags.Static)
				.Where (p => p.Name == name && p.MemberType == MemberTypes.Method)
				.Select (p => (MethodInfo)p);
			return MethodWrapper.Create (registry, methods, null); 
		}
	}
}

