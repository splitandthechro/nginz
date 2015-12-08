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
using System.Security.Cryptography;
using Iodine.Compiler;

namespace Iodine.Runtime
{
	[IodineBuiltinModule ("reflection")]
	public class ReflectionModule : IodineModule
	{
		class IodineInstruction : IodineObject
		{
			public static readonly IodineTypeDefinition TypeDefinition = new IodineTypeDefinition ("Instruction");

			public readonly Instruction Instruction;

			private IodineMethod parentMethod;

			public IodineInstruction (IodineMethod method, Instruction instruction)
				: base (TypeDefinition)
			{
				Instruction = instruction;
				parentMethod = method;
				SetAttribute ("opcode", new IodineInteger ((long)instruction.OperationCode));
				SetAttribute ("immediate", new IodineInteger (instruction.Argument));
			}

			public override string ToString ()
			{
				Instruction ins = this.Instruction;
				switch (this.Instruction.OperationCode) {
				case Opcode.BinOp:
					return ((BinaryOperation)ins.Argument).ToString ();
				case Opcode.UnaryOp:
					return ((UnaryOperation)ins.Argument).ToString ();
				case Opcode.LoadConst:
				case Opcode.Invoke:
				case Opcode.BuildList:
				case Opcode.LoadLocal:
				case Opcode.StoreLocal:
				case Opcode.Jump:
				case Opcode.JumpIfTrue:
				case Opcode.JumpIfFalse:
					return String.Format ("{0} {1}", ins.OperationCode, ins.Argument);
				case Opcode.StoreAttribute:
				case Opcode.LoadAttribute:
				case Opcode.LoadGlobal:
				case Opcode.StoreGlobal:
					return String.Format ("{0} {1} ({2})", ins.OperationCode, ins.Argument, 
						parentMethod.Module.ConstantPool[ins.Argument].ToString ());
				default:
					return ins.OperationCode.ToString ();
				}
			}
		}
		public ReflectionModule ()
			: base ("reflection")
		{
			SetAttribute ("getBytecode", new InternalMethodCallback (getBytecode, this));
			SetAttribute ("hasAttribute", new InternalMethodCallback (hasAttribute, this));
			SetAttribute ("setAttribute", new InternalMethodCallback (setAttribute, this));
			SetAttribute ("getAttributes", new InternalMethodCallback (getAttributes, this));
			SetAttribute ("getInterfaces", new InternalMethodCallback (getInterfaces, this));
			SetAttribute ("loadModule", new InternalMethodCallback (loadModule, this));
			SetAttribute ("compileModule", new InternalMethodCallback (compileModule, this));
		}

		private IodineObject hasAttribute (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length < 2) {
				vm.RaiseException (new IodineArgumentException (2));
				return null;
			}
			IodineObject o1 = args [0];
			IodineString str = args [1] as IodineString;
			if (str == null) {
				vm.RaiseException (new IodineTypeException ("Str"));
				return null;
			}
			return IodineBool.Create (o1.HasAttribute (str.Value));
		}

		private IodineObject getAttributes (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length < 1) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}
			IodineObject o1 = args [0];
			IodineHashMap map = new IodineHashMap ();
			foreach (string key in o1.Attributes.Keys) {
				map.Set (new IodineString (key), o1.Attributes [key]);
			}
			return map;
		}

		private IodineObject getInterfaces (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length < 1) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}
			IodineObject o1 = args [0];
			IodineList list = new IodineList (o1.Interfaces.ToArray ());
			return list;
		}

		private IodineObject setAttribute (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length < 3) {
				vm.RaiseException (new IodineArgumentException (2));
				return null;
			}
			IodineObject o1 = args [0];
			IodineString str = args [1] as IodineString;
			if (str == null) {
				vm.RaiseException (new IodineTypeException ("Str"));
				return null;
			}
			o1.SetAttribute (str.Value, args [2]);
			return null;
		}

		private IodineObject loadModule (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			IodineString pathStr = args [0] as IodineString;
			IodineModule module = vm.Context.LoadModule (pathStr.Value);
			module.Initializer.Invoke (vm, new IodineObject[] { });
			return module;
		}

		private IodineObject compileModule (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			IodineString source = args [0] as IodineString;
			SourceUnit unit = SourceUnit.CreateFromSource (source.Value);
			return unit.Compile (vm.Context);
		}

		private IodineObject getBytecode (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			IodineMethod method = args [0] as IodineMethod;

			if (method == null && args [0] is IodineClosure) {
				method = ((IodineClosure)args [0]).Target;
			}

			IodineList ret = new IodineList (new IodineObject[] { });

			foreach (Instruction ins in method.Body) {
				ret.Add (new IodineInstruction (method, ins));
			}
			return ret;
		}
	}
}

