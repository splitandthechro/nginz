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
using System.Text;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Iodine.Compiler;

namespace Iodine.Runtime
{
	/// <summary>
	/// Represents a single frame (Activation record) for an Iodine method
	/// </summary>
	public class StackFrame
	{
		public readonly int LocalCount;
		public readonly IodineMethod Method;
		public readonly IodineObject Self;
		public readonly LinkedStack<IodineObject> DisposableObjects = new LinkedStack<IodineObject> ();
		public readonly LinkedStack<IodineExceptionHandler> ExceptionHandlers = new LinkedStack<IodineExceptionHandler> ();

		public volatile bool AbortExecution = false;

		public bool Yielded { set; get; }

		public SourceLocation Location { set; get; }

		public int InstructionPointer { set; get; }

		public StackFrame Parent { private set; get; }

		public IodineModule Module {
			get { return Method.Module; }
		}

		private LinkedStack<IodineObject> stack = new LinkedStack<IodineObject> ();
		private IodineObject[] locals;
		private IodineObject[] parentLocals = null;

		public StackFrame (IodineMethod method, StackFrame parent, IodineObject self, int localCount)
		{
			LocalCount = localCount;
			locals = new IodineObject[localCount];
			parentLocals = locals;
			Method = method;
			Self = self;
			Parent = parent;
		}

		public StackFrame (IodineMethod method, StackFrame parent, IodineObject self, int localCount,
		                   IodineObject[] locals) : this (method, parent, self, localCount)
		{
			parentLocals = locals;
			this.locals = new IodineObject[localCount];
			for (int i = 0; i < localCount; i++) {
				this.locals [i] = locals [i]; 
			}
		}

		#if DOTNET_45
		[MethodImpl (MethodImplOptions.AggressiveInlining)]
		#endif
		internal void StoreLocal (int index, IodineObject obj)
		{
			if (parentLocals [index] != null) {
				parentLocals [index] = obj;
			}
			locals [index] = obj;
		}

		#if DOTNET_45
		[MethodImpl (MethodImplOptions.AggressiveInlining)]
		#endif
		internal IodineObject LoadLocal (int index)
		{
			return locals [index];
		}

		#if DOTNET_45
		[MethodImpl (MethodImplOptions.AggressiveInlining)]
		#endif
		internal void Push (IodineObject obj)
		{
			stack.Push (obj ?? IodineNull.Instance);
		}

		#if DOTNET_45
		[MethodImpl (MethodImplOptions.AggressiveInlining)]
		#endif
		internal IodineObject Pop ()
		{
			return stack.Pop ();
		}

		#if DOTNET_45
		[MethodImpl (MethodImplOptions.AggressiveInlining)]
		#endif
		internal StackFrame Duplicate (StackFrame top, int localCount)
		{
			if (localCount > LocalCount) {
				IodineObject[] oldLocals = locals;
				locals = new IodineObject[localCount];
				Array.Copy (oldLocals, locals, oldLocals.Length);
			}
			return new StackFrame (Method, top, Self, Math.Max (LocalCount, localCount), locals);
		}
	}

	public class NativeStackFrame : StackFrame
	{
		public InternalMethodCallback NativeMethod { private set; get; }

		public NativeStackFrame (InternalMethodCallback method, StackFrame parent)
			: base (null, parent, null, 0)
		{
			this.NativeMethod = method;
		}
	}
}