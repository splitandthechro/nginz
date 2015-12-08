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
using Iodine.Compiler;

namespace Iodine.Runtime
{
	public class IodineHashMap : IodineObject
	{
		public static readonly IodineTypeDefinition TypeDefinition = new MapTypeDef ();

		class MapTypeDef : IodineTypeDefinition
		{
			public MapTypeDef ()
				: base ("HashMap")
			{
			}

			public override IodineObject Invoke (VirtualMachine vm, IodineObject[] args)
			{
				if (args.Length >= 1) {
					IodineList inputList = args [0] as IodineList;
					IodineHashMap ret = new IodineHashMap ();
					if (inputList != null) {
						foreach (IodineObject item in inputList.Objects) {
							IodineTuple kv = item as IodineTuple;
							if (kv != null) {
								ret.Set (kv.Objects [0], kv.Objects [1]);
							}
						}
					} 
					return ret;
				}
				return new IodineHashMap ();
			}
		}

		private int iterIndex = 0;

		public Dictionary <int, IodineObject> Dict { private set; get; }

		public Dictionary <int, IodineObject> Keys { private set; get; }

		public IodineHashMap ()
			: base (TypeDefinition)
		{
			Dict = new Dictionary<int, IodineObject> ();
			Keys = new Dictionary<int, IodineObject> ();
			SetAttribute ("contains", new InternalMethodCallback (contains, this));
			SetAttribute ("getSize", new InternalMethodCallback (getSize, this));
			SetAttribute ("clear", new InternalMethodCallback (clear, this));
			SetAttribute ("set", new InternalMethodCallback (set, this));
			SetAttribute ("get", new InternalMethodCallback (get, this));
			SetAttribute ("remove", new InternalMethodCallback (remove, this));
		}

		public override IodineObject Len (VirtualMachine vm)
		{
			return new IodineInteger (Keys.Count);
		}

		public override IodineObject GetIndex (VirtualMachine vm, IodineObject key)
		{
			int hash = key.GetHashCode ();
			if (!Dict.ContainsKey (hash)) {
				vm.RaiseException (new IodineKeyNotFound ());
				return null;
			}
			return Dict [key.GetHashCode ()];
		}

		public override void SetIndex (VirtualMachine vm, IodineObject key, IodineObject value)
		{
			Dict [key.GetHashCode ()] = value;
			Keys [key.GetHashCode ()] = key;
		}

		public override IodineObject Equals (VirtualMachine vm, IodineObject right)
		{
			IodineHashMap hash = right as IodineHashMap;
			if (hash == null) {
				vm.RaiseException (new IodineTypeException ("HashMap"));
				return null;
			}
			return IodineBool.Create (compareTo (hash));
		}

		public override int GetHashCode ()
		{
			int accum = 17;
			unchecked {
				foreach (int key in Dict.Keys) {
					accum += 529 * key;
					IodineObject obj = Dict [key];
					if (obj != null) {
						accum += 529 * obj.GetHashCode ();
					}
				}
			}
			return accum;
		}

		public override IodineObject IterGetCurrent (VirtualMachine vm)
		{
			IodineObject[] keys = new IodineObject[Keys.Count];
			Keys.Values.CopyTo (keys, 0);
			return keys [iterIndex - 1];
		}

		public override bool IterMoveNext (VirtualMachine vm)
		{
			if (iterIndex >= Dict.Keys.Count)
				return false;
			iterIndex++;
			return true;
		}

		public override void IterReset (VirtualMachine vm)
		{
			iterIndex = 0;
		}

		public void Set (IodineObject key, IodineObject val)
		{
			Dict [key.GetHashCode ()] = val;
			Keys [key.GetHashCode ()] = key;
		}

		public IodineObject Get (IodineObject key)
		{
			return Dict [key.GetHashCode ()];
		}

		private bool compareTo (IodineHashMap hash)
		{
			if (hash.Keys.Count != this.Keys.Count)
				return false;
			foreach (int key in Keys.Keys) {
				if (!hash.Keys.ContainsKey (key))
					return false;
				if (hash.Dict [key].GetHashCode () != Dict [key].GetHashCode ())
					return false;
			}
			return true;
		}

		private IodineObject contains (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}
			return IodineBool.Create (Dict.ContainsKey (args [0].GetHashCode ()));
		}

		private IodineObject getSize (VirtualMachine vm, IodineObject self, IodineObject[] arguments)
		{
			return new IodineInteger (Dict.Count);
		}

		private IodineObject clear (VirtualMachine vm, IodineObject self, IodineObject[] arguments)
		{
			Dict.Clear ();
			Keys.Clear ();
			return null;
		}

		private IodineObject set (VirtualMachine vm, IodineObject self, IodineObject[] arguments)
		{
			if (arguments.Length >= 2) {
				IodineObject key = arguments [0];
				IodineObject val = arguments [1];
				Dict [key.GetHashCode ()] = val;
				Keys [key.GetHashCode ()] = key;
				return null;
			}
			vm.RaiseException (new IodineArgumentException (2));
			return null;
		}

		private IodineObject get (VirtualMachine vm, IodineObject self, IodineObject[] arguments)
		{
			if (arguments.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			} else if (arguments.Length == 1) {
				int hash = arguments [0].GetHashCode ();
				if (Dict.ContainsKey (hash)) {
					return Dict [hash];
				}
				vm.RaiseException (new IodineKeyNotFound ());
				return null;
			} else {
				int hash = arguments [0].GetHashCode ();
				if (Dict.ContainsKey (hash)) {
					return Dict [hash];
				}
				return arguments [1];
			}
		}

		private IodineObject remove (VirtualMachine vm, IodineObject self, IodineObject[] arguments)
		{
			if (arguments.Length >= 1) {
				IodineObject key = arguments [0];
				int hash = key.GetHashCode ();
				if (!Dict.ContainsKey (hash)) {
					vm.RaiseException (new IodineKeyNotFound ());
					return null;
				}
				Keys.Remove (hash);
				Dict.Remove (hash);
				return null;
			}
			vm.RaiseException (new IodineArgumentException (2));
			return null;
		}
	}
}

