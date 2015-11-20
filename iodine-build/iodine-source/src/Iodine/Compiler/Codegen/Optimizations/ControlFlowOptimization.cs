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
using Iodine.Runtime;

namespace Iodine.Compiler
{
	public class ControlFlowOptimization : IBytecodeOptimization
	{
		class ReachableRegion
		{
			public int Start { private set; get; }

			public int End { private set; get; }

			public int Size {
				get {
					return this.End - this.Start;
				}
			}

			public ReachableRegion (int start, int end)
			{
				this.Start = start;
				this.End = end;
			}
		}

		public void PerformOptimization (IodineMethod method)
		{
			List <ReachableRegion> regions = new List<ReachableRegion> ();
			int reachableSize = 0;
			FindRegion (method, regions, 0);
			foreach (ReachableRegion region in regions) {
				reachableSize += region.Size + 1;
			}
			Instruction[] oldInstructions = method.Body.ToArray ();
			Instruction[] newInstructions = new Instruction[method.Body.Count];
			int next = 0;
			for (int i = 0; i < method.Body.Count; i++) {
				if (IsReachable (regions, i)) {
					newInstructions [next++] = oldInstructions [i];
				} else {
					ShiftLabels (next, oldInstructions);
					ShiftLabels (next, newInstructions);
				}
			}
			method.Body.Clear ();
			method.Body.AddRange (newInstructions);
		}

		private void FindRegion (IodineMethod method, List<ReachableRegion> regions, int start)
		{
			if (IsReachable (regions, start)) {
				return;
			}

			for (int i = start; i < method.Body.Count; i++) {
				Instruction ins = method.Body [i];

				if (ins.OperationCode == Opcode.Jump) {
					regions.Add (new ReachableRegion (start, i));
					FindRegion (method, regions, ins.Argument);
					return;
				} else if (ins.OperationCode == Opcode.JumpIfTrue ||
				           ins.OperationCode == Opcode.JumpIfFalse ||
				           ins.OperationCode == Opcode.PushExceptionHandler) {
					regions.Add (new ReachableRegion (start, i));
					FindRegion (method, regions, i + 1);
					FindRegion (method, regions, ins.Argument);
					return;
				} else if (ins.OperationCode == Opcode.Return) {
					regions.Add (new ReachableRegion (start, i));
					return;
				}
			}
			regions.Add (new ReachableRegion (start, method.Body.Count));
		}

		private void ShiftLabels (int start, Instruction[] instructions)
		{
			for (int i = 0; i < instructions.Length; i++) {
				Instruction ins = instructions [i];
				if (ins.OperationCode == Opcode.Jump || ins.OperationCode == Opcode.JumpIfFalse ||
				    ins.OperationCode == Opcode.JumpIfTrue ||
				    ins.OperationCode == Opcode.PushExceptionHandler) {

					if (ins.Argument > start) {
						instructions [i] = new Instruction (ins.Location, ins.OperationCode,
							ins.Argument - 1);
					}
				}

			}
		}

		private bool IsReachable (List<ReachableRegion> regions, int addr)
		{
			foreach (ReachableRegion region in regions) {
				if (region.Start <= addr && addr <= region.End) {
					return true;
				}
			}
			return false;
		}
	}
}

