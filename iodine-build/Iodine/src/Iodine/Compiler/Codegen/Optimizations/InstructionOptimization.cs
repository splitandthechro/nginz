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
using Iodine.Runtime;

namespace Iodine.Compiler
{
	class InstructionOptimization : IBytecodeOptimization
	{
		public void PerformOptimization (IodineMethod method)
		{
			while (PerformMethodOptimization (method) > 0)
				;
		}

		private int PerformMethodOptimization (IodineMethod method)
		{
			int removed = 0;
			Instruction[] oldInstructions = method.Body.ToArray ();
			Instruction[] newInstructions = new Instruction[method.Body.Count];
			int next = 0;
			Instruction last = new Instruction ();
			for (int i = 0; i < method.Body.Count; i++) {
				Instruction curr = oldInstructions [i];
				if (i != 0 && curr.OperationCode == Opcode.Pop) {
					if (last.OperationCode == Opcode.LoadLocal || last.OperationCode == Opcode.LoadGlobal
					    || last.OperationCode == Opcode.LoadNull) {
						oldInstructions [i] = new Instruction (curr.Location, Opcode.Nop, 0);
						oldInstructions [i - 1] = new Instruction (curr.Location, Opcode.Nop, 0);
						removed++;
					}
				} else if (curr.OperationCode == Opcode.Jump && curr.Argument == i + 1) {
					oldInstructions [i] = new Instruction (curr.Location, Opcode.Nop, 0);
					removed++;
				}
				last = curr;
			}
			for (int i = 0; i < oldInstructions.Length; i++) {
				Instruction curr = oldInstructions [i];
				if (curr.OperationCode == Opcode.Nop) {
					ShiftLabels (next, newInstructions);
					ShiftLabels (next, oldInstructions);
				} else {
					newInstructions [next++] = curr;
				}
			}
			method.Body.Clear ();
			method.Body.AddRange (newInstructions);
			return removed;
		}

		private void ShiftLabels (int start, Instruction[] instructions)
		{
			for (int i = 0; i < instructions.Length; i++) {
				Instruction ins = instructions [i];
				if (ins.OperationCode == Opcode.Jump ||
					ins.OperationCode == Opcode.JumpIfFalse ||
				    ins.OperationCode == Opcode.JumpIfTrue ||
				    ins.OperationCode == Opcode.PushExceptionHandler) {

					if (ins.Argument > start) {
						instructions [i] = new Instruction (ins.Location, ins.OperationCode,
							ins.Argument - 1);
					}
				}

			}
		}
	}
}

