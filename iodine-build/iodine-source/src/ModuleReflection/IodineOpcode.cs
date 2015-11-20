using System;
using Iodine.Runtime;

namespace ModuleReflection
{
	public class IodineOpcode : IodineObject
	{
		public static readonly IodineTypeDefinition OpcodeTypeDef = new IodineTypeDefinition ("Opcode");

		static IodineOpcode ()
		{
			OpcodeTypeDef.SetAttribute ("BIN_OP", new IodineOpcode (Opcode.BinOp));
			OpcodeTypeDef.SetAttribute ("BUILD_CLOSURE", new IodineOpcode (Opcode.BuildClosure));
			OpcodeTypeDef.SetAttribute ("BUILD_LIST", new IodineOpcode (Opcode.BuildList));
			OpcodeTypeDef.SetAttribute ("BUILD_TUPLE", new IodineOpcode (Opcode.BuildTuple));
			OpcodeTypeDef.SetAttribute ("DUP", new IodineOpcode (Opcode.Dup));
			OpcodeTypeDef.SetAttribute ("DUP3", new IodineOpcode (Opcode.Dup3));
			OpcodeTypeDef.SetAttribute ("INVOKE", new IodineOpcode (Opcode.Invoke));
			OpcodeTypeDef.SetAttribute ("ITER_GET_NEXT", new IodineOpcode (Opcode.IterGetNext));
			OpcodeTypeDef.SetAttribute ("ITER_MOVE_NEXT", new IodineOpcode (Opcode.IterMoveNext));
			OpcodeTypeDef.SetAttribute ("ITER_RESET", new IodineOpcode (Opcode.IterReset));
			OpcodeTypeDef.SetAttribute ("JUMP", new IodineOpcode (Opcode.Jump));
			OpcodeTypeDef.SetAttribute ("JUMP_IF_FALSE", new IodineOpcode (Opcode.JumpIfFalse));
			OpcodeTypeDef.SetAttribute ("JUMP_IF_TRUE", new IodineOpcode (Opcode.JumpIfTrue));
			OpcodeTypeDef.SetAttribute ("LOAD_ATTRIBUTE", new IodineOpcode (Opcode.LoadAttribute));
			OpcodeTypeDef.SetAttribute ("LOAD_CONST", new IodineOpcode (Opcode.LoadConst));
			OpcodeTypeDef.SetAttribute ("LOAD_EXCEPTION", new IodineOpcode (Opcode.LoadException));
			OpcodeTypeDef.SetAttribute ("LOAD_FALSE", new IodineOpcode (Opcode.LoadFalse));
			OpcodeTypeDef.SetAttribute ("LOAD_GLOBAL", new IodineOpcode (Opcode.LoadGlobal));
			OpcodeTypeDef.SetAttribute ("LOAD_INDEX", new IodineOpcode (Opcode.LoadIndex));
			OpcodeTypeDef.SetAttribute ("LOAD_LOCAL", new IodineOpcode (Opcode.LoadLocal));
			OpcodeTypeDef.SetAttribute ("LOAD_NULL", new IodineOpcode (Opcode.LoadNull));
			OpcodeTypeDef.SetAttribute ("LOAD_SELF", new IodineOpcode (Opcode.LoadSelf));
			OpcodeTypeDef.SetAttribute ("LOAD_TRUE", new IodineOpcode (Opcode.LoadTrue));
			OpcodeTypeDef.SetAttribute ("STORE_LOCAL", new IodineOpcode (Opcode.StoreLocal));
			OpcodeTypeDef.SetAttribute ("UNARY_OP", new IodineOpcode (Opcode.UnaryOp));
		}

		public Opcode OperationCode
		{ private set; get; }

		public IodineOpcode (Opcode opcode)
			: base (OpcodeTypeDef)
		{
			this.OperationCode = opcode;
		}
	}
}

