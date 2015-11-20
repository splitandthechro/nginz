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
using System.Collections.Generic;

namespace Iodine.Runtime
{
	public class IodineLabel
	{
		public int _Position;
		public int _LabelID;

		public IodineLabel (int labelID)
		{
			_LabelID = labelID;
			_Position = 0;
		}
	}

	public class IodineInstanceMethodWrapper : IodineObject
	{
		private static readonly IodineTypeDefinition InstanceTypeDef = new IodineTypeDefinition ("InstanceMethod");

		public IodineMethod Method { private set; get; }

		public IodineObject Self { private set; get; }

		public IodineInstanceMethodWrapper (IodineObject self, IodineMethod method)
			: base (InstanceTypeDef)
		{
			Method = method;
			Self = self;
		}

		public override bool IsCallable ()
		{
			return true;
		}

		public override IodineObject Invoke (VirtualMachine vm, IodineObject[] arguments)
		{
			if (Method.Generator)
				return new IodineGenerator (vm.Top, this, arguments);
			return vm.InvokeMethod (Method, Self, arguments);
		}
	}

	public class IodineMethod : IodineObject
	{
		private static readonly IodineTypeDefinition MethodTypeDef = new IodineTypeDefinition ("Method");
		private static int nextLabelID = 0;

		private Dictionary<int, IodineLabel> labelReferences = new Dictionary<int, IodineLabel> ();
		protected List<Instruction> instructions = new List<Instruction> ();
		private IodineMethod parent = null;

		public List<Instruction> Body {
			get {
				return this.instructions;
			}
		}

		public string Name { private set; get; }

		public Dictionary <string, int> Parameters { private set; get; }

		public int ParameterCount { private set; get; }

		public int LocalCount { private set; get; }

		public bool Variadic { set; get; }

		public bool AcceptsKeywordArgs { set; get; }

		public bool Generator { set; get; }

		public IodineModule Module { private set; get; }

		public bool InstanceMethod { private set; get; }

		public IodineMethod (IodineModule module, string name, bool isInstance, int parameterCount,
		                     int localCount) : base (MethodTypeDef)
		{
			Name = name;
			ParameterCount = parameterCount;
			Module = module;
			LocalCount = localCount;
			InstanceMethod = isInstance;
			Parameters = new Dictionary<string, int> ();
		}

		public IodineMethod (IodineMethod parent, IodineModule module, string name, bool isInstance, int parameterCount,
		                     int localCount) : this (module, name, isInstance, parameterCount, localCount)
		{
			this.parent = parent;
		}

		public void EmitInstruction (Location loc, Opcode opcode)
		{
			instructions.Add (new Instruction (loc, opcode));
		}

		public void EmitInstruction (Location loc, Opcode opcode, int arg)
		{
			instructions.Add (new Instruction (loc, opcode, arg));
		}

		public void EmitInstruction (Location loc, Opcode opcode, IodineLabel label)
		{
			labelReferences [instructions.Count] = label;
			instructions.Add (new Instruction (loc, opcode, 0));
		}

		public int CreateTemporary ()
		{
			if (parent != null)
				parent.CreateTemporary ();
			return LocalCount++;
		}

		public IodineLabel CreateLabel ()
		{
			return new IodineLabel (nextLabelID++);
		}

		public void MarkLabelPosition (IodineLabel label)
		{
			label._Position = instructions.Count;
		}

		public void FinalizeLabels ()
		{
			foreach (int position in labelReferences.Keys) {
				instructions [position] = new Instruction (instructions [position].Location,
					instructions [position].OperationCode,
					labelReferences [position]._Position);
			}
		}

		public override bool IsCallable ()
		{
			return true;
		}

		public override IodineObject Invoke (VirtualMachine vm, IodineObject[] arguments)
		{
			if (Generator) {
				return new IodineGenerator (vm.Top, this, arguments);
			}
			return vm.InvokeMethod (this, null, arguments);
		}

		public override string ToString ()
		{
			return string.Format ("<Function {0}>", Name);
		}
	}
}

