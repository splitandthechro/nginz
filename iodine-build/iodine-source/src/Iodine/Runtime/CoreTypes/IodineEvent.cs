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
using System.Linq;
using System.Collections.Generic;
using Iodine.Compiler;

namespace Iodine.Runtime
{
	public class IodineEventEmitter : IodineObject
	{
		public static readonly IodineTypeDefinition TypeDefinition = new EventEmitterTypeDef ();

		class EventEmitterTypeDef : IodineTypeDefinition
		{
			public EventEmitterTypeDef ()
				: base ("EventEmitter")
			{
			}

			public override IodineObject Invoke (VirtualMachine vm, IodineObject[] args)
			{
				return new IodineEventEmitter ();
			}

			public override void Inherit (VirtualMachine vm, IodineObject self, IodineObject[] arguments)
			{
				self.Base = new IodineEventEmitter ();
			}
		}

		/*
		 * This is ugly
		 */
		private Dictionary<string, List<IodineObject>> listeners = new Dictionary<string,
			List<IodineObject>> ();

		public IodineEventEmitter ()
			: base (TypeDefinition)
		{
			SetAttribute ("on", new InternalMethodCallback (on, this));
			SetAttribute ("emit", new InternalMethodCallback (emit, this));
		}

		public override string ToString ()
		{
			return string.Format ("EventEmitter ()");
		}

		private IodineObject emit (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length < 1) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}
			IodineString e = args [0] as IodineString;
			if (e == null) {
				vm.RaiseException (new IodineTypeException ("Str"));
				return null;
			}
			if (listeners.ContainsKey (e.Value)) {
				foreach (IodineObject obj in listeners [e.Value]) {
					obj.Invoke (vm, args.Skip (1).ToArray ());
				}
			}
			return null;
		}

		private IodineObject on (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length < 2) {
				vm.RaiseException (new IodineArgumentException (2));
				return null;
			}
			IodineString e = args [0] as IodineString;
			if (e == null) {
				vm.RaiseException (new IodineTypeException ("Str"));
				return null;
			}
			if (!listeners.ContainsKey (e.Value)) {
				listeners.Add (e.Value, new List<IodineObject> ());
			}
			listeners [e.Value].Add (args [1]);
			return null;
		}
	}
}

