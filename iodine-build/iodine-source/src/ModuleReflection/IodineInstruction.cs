using System;
using Iodine.Runtime;
using Iodine.Compiler;

namespace ModuleReflection
{
	public class IodineInstruction : IodineObject
	{
		private static IodineTypeDefinition InstructionTypeDef = new IodineTypeDefinition ("Instruction");

		public Instruction Instruction
		{ private set; get; }

		private IodineMethod parentMethod;

		public IodineInstruction (IodineMethod method, Instruction instruction)
			: base (InstructionTypeDef)
		{
			this.Instruction = instruction;
			SetAttribute ("opcode", new IodineInteger ((long)instruction.OperationCode));
			SetAttribute ("immediate", new IodineInteger (instruction.Argument));
			this.parentMethod = method;
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
}

