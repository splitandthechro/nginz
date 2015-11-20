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
using System.Threading;
using System.Security.Cryptography;

namespace Iodine.Runtime
{
	[IodineBuiltinModule ("threading")]
	public class ThreadingModule : IodineModule
	{
		class IodineThread : IodineObject
		{
			class ThreadTypeDefinition : IodineTypeDefinition
			{
				public ThreadTypeDefinition ()
					: base ("Thread")
				{
				}

				public override IodineObject Invoke (VirtualMachine vm, IodineObject[] args)
				{
					if (args.Length <= 0) {
						vm.RaiseException (new IodineArgumentException (1));
						return null;
					}
					IodineObject func = args [0];
					VirtualMachine newVm = new VirtualMachine (vm.Configuration, vm.Globals);

					Thread t = new Thread (() => {
						try {
							func.Invoke (newVm, new IodineObject[] { }); 
						} catch (UnhandledIodineExceptionException ex) {
							vm.RaiseException (ex.OriginalException);
						}
					});
					return new IodineThread (t);
				}
			}

			public static readonly IodineTypeDefinition TypeDefinition = new ThreadTypeDefinition ();

			public Thread Value { private set; get; }

			public IodineThread (Thread t)
				: base (TypeDefinition)
			{
				Value = t;
				SetAttribute ("start", new InternalMethodCallback (start, this));
				SetAttribute ("abort", new InternalMethodCallback (abort, this));
				SetAttribute ("isAlive", new InternalMethodCallback (isAlive, this));
			}

			private IodineObject start (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				Value.Start ();
				return null;
			}

			private IodineObject abort (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				Value.Abort ();
				return null;
			}

			private IodineObject isAlive (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				return IodineBool.Create (Value.IsAlive);
			}
		}

		class IodineLock : IodineObject
		{
			public static readonly IodineTypeDefinition TypeDefinition = new LockTypeDefinition ();

			class LockTypeDefinition : IodineTypeDefinition
			{
				public LockTypeDefinition ()
					: base ("Lock")
				{
				}

				public override IodineObject Invoke (VirtualMachine vm, IodineObject[] args)
				{
					return new IodineLock ();
				}
			}

			private volatile bool _lock = false;

			public IodineLock ()
				: base (TypeDefinition)
			{
				SetAttribute ("aquire", new InternalMethodCallback (acquire, this));
				SetAttribute ("release", new InternalMethodCallback (release, this));
				SetAttribute ("locked", new InternalMethodCallback (locked, this));
			}

			private IodineObject acquire (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				while (_lock)
					;
				_lock = true;
				return null;
			}

			private IodineObject release (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				_lock = false;
				return null;
			}

			private IodineObject locked (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				return IodineBool.Create (_lock);
			}
		}

		class IodineSemaphore : IodineObject
		{
			public static readonly IodineTypeDefinition TypeDefinition = new SemaphoreTypeDefinition ();

			class SemaphoreTypeDefinition : IodineTypeDefinition
			{
				public SemaphoreTypeDefinition ()
					: base ("Semaphore")
				{
				}

				public override IodineObject Invoke (VirtualMachine vm, IodineObject[] args)
				{
					if (args.Length == 0) {
						return new IodineSemaphore (1);
					}
					IodineInteger semaphore = args [0] as IodineInteger;
					if (semaphore == null) {
						vm.RaiseException (new IodineTypeException ("Integer"));
						return null;
					}
					return new IodineSemaphore ((int)semaphore.Value);
				}
			}

			private volatile int semaphore = 1;

			public IodineSemaphore (int semaphore)
				: base (TypeDefinition)
			{
				this.semaphore = semaphore;
				SetAttribute ("aquire", new InternalMethodCallback (acquire, this));
				SetAttribute ("release", new InternalMethodCallback (release, this));
				SetAttribute ("locked", new InternalMethodCallback (locked, this));
			}

			private IodineObject acquire (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				semaphore--;
				while (semaphore < 0)
					;
				return null;
			}

			private IodineObject release (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				semaphore++;
				return null;
			}

			private IodineObject locked (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				return IodineBool.Create (semaphore < 0);
			}
		}

		public ThreadingModule ()
			: base ("threading")
		{
			SetAttribute ("Thread", IodineThread.TypeDefinition);
			SetAttribute ("Lock", IodineLock.TypeDefinition);
			SetAttribute ("Semaphore", IodineSemaphore.TypeDefinition);
			SetAttribute ("sleep", new InternalMethodCallback (sleep, this));
		}

		private IodineObject sleep (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
			}
			IodineInteger time = args [0] as IodineInteger;
			System.Threading.Thread.Sleep ((int)time.Value);
			return null;
		}
	}
}

