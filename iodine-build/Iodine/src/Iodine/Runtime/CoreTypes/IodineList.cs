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
using System.Text;
using System.Collections.Generic;
using Iodine.Compiler;

namespace Iodine.Runtime
{
	public class IodineList : IodineObject
	{
		public static readonly IodineTypeDefinition TypeDefinition = new ListTypeDef ();

		class ListTypeDef : IodineTypeDefinition
		{
			public ListTypeDef ()
				: base ("List")
			{
			}

			public override IodineObject Invoke (VirtualMachine vm, IodineObject[] args)
			{
				IodineList list = new IodineList (new IodineObject[0]);
				if (args.Length > 0) {
					foreach (IodineObject collection in args) {
						collection.IterReset (vm);
						while (collection.IterMoveNext (vm)) {
							IodineObject o = collection.IterGetCurrent (vm);
							list.Add (o);
						}
					}
				}
				return list;
			}
		}

		private int iterIndex = 0;

		public List<IodineObject> Objects { private set; get; }

		public IodineList (List<IodineObject> list)
			: base (TypeDefinition)
		{
			SetAttribute ("getSize", new InternalMethodCallback (getSize, this));
			SetAttribute ("add", new InternalMethodCallback (add, this));
			SetAttribute ("addRange", new InternalMethodCallback (add, this));
			SetAttribute ("remove", new InternalMethodCallback (remove, this));
			SetAttribute ("removeAt", new InternalMethodCallback (removeAt, this));
			SetAttribute ("contains", new InternalMethodCallback (contains, this));
			SetAttribute ("splice", new InternalMethodCallback (splice, this));
			SetAttribute ("clear", new InternalMethodCallback (clear, this));
			Objects = list;
		}

		public IodineList (IodineObject[] items)
			: this (new List<IodineObject> (items))
		{
		}

		public override IodineObject Len (VirtualMachine vm)
		{
			return new IodineInteger (Objects.Count);
		}

		public override IodineObject GetIndex (VirtualMachine vm, IodineObject key)
		{
			IodineInteger index = key as IodineInteger;
			if (index == null) {
				vm.RaiseException (new IodineTypeException ("Int"));
				return null;
			}

			if (index.Value < Objects.Count)
				return Objects [(int)index.Value];
			vm.RaiseException (new IodineIndexException ());
			return null;
		}

		public override void SetIndex (VirtualMachine vm, IodineObject key, IodineObject value)
		{
			IodineInteger index = key as IodineInteger;
			if (index == null) {
				vm.RaiseException (new IodineTypeException ("Int"));
				return;
			}

			if (index.Value < Objects.Count)
				this.Objects [(int)index.Value] = value;
			else
				vm.RaiseException (new IodineIndexException ());
		}

		public override IodineObject Add (VirtualMachine vm, IodineObject right)
		{
			IodineList list = new IodineList (Objects.ToArray ());
			right.IterReset (vm);
			while (right.IterMoveNext (vm)) {
				IodineObject o = right.IterGetCurrent (vm);
				list.Add (o);
			}
			return list;
		}

		public override IodineObject Equals (VirtualMachine vm, IodineObject right)
		{
			IodineList listVal = right as IodineList;
			if (listVal == null) {
				vm.RaiseException (new IodineTypeException ("List"));
				return null;
			}
			return IodineBool.Create (compare (this, listVal));
		}

		public override IodineObject IterGetCurrent (VirtualMachine vm)
		{
			return Objects [iterIndex - 1];
		}

		public override bool IterMoveNext (VirtualMachine vm)
		{
			if (iterIndex >= Objects.Count)
				return false;
			iterIndex++;
			return true;
		}

		public override void IterReset (VirtualMachine vm)
		{
			iterIndex = 0;
		}

		public override IodineObject Represent (VirtualMachine vm)
		{
			string repr = String.Join (", ", Objects.Select (p => p.Represent (vm).ToString ()));
			return new IodineString (String.Format ("[{0}]", repr));
		}

		public void Add (IodineObject obj)
		{
			Objects.Add (obj);
		}

		private bool compare (IodineList list1, IodineList list2)
		{
			if (list1.Objects.Count != list2.Objects.Count)
				return false;
			for (int i = 0; i < list1.Objects.Count; i++) {
				if (list1.Objects [i].GetHashCode () != list2.Objects [i].GetHashCode ())
					return false;
			}
			return true;
		}

		private IodineObject getSize (VirtualMachine vm, IodineObject self, IodineObject[] arguments)
		{
			return new IodineInteger (((IodineList)self).Objects.Count);
		}

		private IodineObject add (VirtualMachine vm, IodineObject self, IodineObject[] arguments)
		{
			if (arguments.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}
			IodineList list = self as IodineList;
			foreach (IodineObject obj in arguments) {
				list.Add (obj);
			}
			return null;
		}

		private IodineObject addRange (VirtualMachine vm, IodineObject self, IodineObject[] arguments)
		{
			if (arguments.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}
			IodineObject collection = arguments [0];
			collection.IterReset (vm);
			while (collection.IterMoveNext (vm)) {
				IodineObject o = collection.IterGetCurrent (vm);
				Add (o);
			}
			return null;
		}

		private IodineObject remove (VirtualMachine vm, IodineObject self, IodineObject[] arguments)
		{
			if (arguments.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}
			IodineObject key = arguments [0];
			if (Objects.Contains (key))
				Objects.Remove (key);
			else
				vm.RaiseException (new IodineKeyNotFound ());
			return null;
		}

		private IodineObject removeAt (VirtualMachine vm, IodineObject self, IodineObject[] arguments)
		{
			if (arguments.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}
			IodineInteger index = arguments [0] as IodineInteger;
			if (index != null)
				Objects.RemoveAt ((int)index.Value);
			else
				vm.RaiseException (new IodineTypeException ("Int"));
			return null;
		}

		private IodineObject contains (VirtualMachine vm, IodineObject self, IodineObject[] arguments)
		{
			if (arguments.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}
			IodineObject key = arguments [0];
			int hashCode = key.GetHashCode ();
			bool found = false;
			foreach (IodineObject obj in Objects) {
				if (obj.GetHashCode () == hashCode) {
					found = true;
				}
			}

			return IodineBool.Create (found);
		}

		private IodineObject splice (VirtualMachine vm, IodineObject self, IodineObject[] arguments)
		{
			if (arguments.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}

			int start = 0;
			int end = Objects.Count;

			IodineInteger startInt = arguments [0] as IodineInteger;
			if (startInt == null) {
				vm.RaiseException (new IodineTypeException ("Int"));
				return null;
			}
			start = (int)startInt.Value;

			if (arguments.Length >= 2) {
				IodineInteger endInt = arguments [1] as IodineInteger;
				if (endInt == null) {
					vm.RaiseException (new IodineTypeException ("Int"));
					return null;
				}
				end = (int)endInt.Value;
			}

			if (start < 0)
				start = this.Objects.Count - start;
			if (end < 0)
				end = this.Objects.Count - end;

			IodineList retList = new IodineList (new IodineObject[]{ });

			for (int i = start; i < end; i++) {
				if (i < 0 || i > this.Objects.Count) {
					vm.RaiseException (new IodineIndexException ());
					return null;
				}
				retList.Add (Objects [i]);
			}

			return retList;
		}

		private IodineObject clear (VirtualMachine vm, IodineObject self, IodineObject[] arguments)
		{
			Objects.Clear ();
			return null;
		}

		public override int GetHashCode ()
		{
			int accum = 17;
			unchecked {
				foreach (IodineObject obj in Objects) {
					if (obj != null) {
						accum += 529 * obj.GetHashCode ();
					}
				}
			}
			return accum;
		}
	}
}
