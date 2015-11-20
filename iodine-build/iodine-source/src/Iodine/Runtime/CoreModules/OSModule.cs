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
using System.Diagnostics;

namespace Iodine.Runtime
{
	[IodineBuiltinModule ("os")]
	public class OSModule : IodineModule
	{
		class IodineProc : IodineObject
		{
			public static readonly IodineTypeDefinition ProcTypeDef = new IodineTypeDefinition ("Proc");

			public Process Value { private set; get; }

			public IodineProc (Process proc)
				: base (ProcTypeDef)
			{
				Value = proc;
				SetAttribute ("id", new IodineInteger (proc.Id));
				SetAttribute ("name", new IodineString (proc.ProcessName));
				SetAttribute ("kill", new InternalMethodCallback (kill, this));
			}

			private IodineObject kill (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				Value.Kill ();
				return null;
			}
		}

		public OSModule ()
			: base ("os")
		{
			SetAttribute ("userDir", new IodineString (Environment.GetFolderPath (
				Environment.SpecialFolder.UserProfile)));
			SetAttribute ("envSep", new IodineString (Path.PathSeparator.ToString ()));
			SetAttribute ("getProcList", new InternalMethodCallback (getProcList, this));
			SetAttribute ("getEnv", new InternalMethodCallback (getEnv, this));
			SetAttribute ("setEnv", new InternalMethodCallback (setEnv, this));
			SetAttribute ("spawn", new InternalMethodCallback (spawn, this));
		}

		private IodineObject getProcList (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			IodineList list = new IodineList (new IodineObject[] { });
			foreach (Process proc in Process.GetProcesses ()) {
				list.Add (new IodineProc (proc));
			}
			return list;
		}

		private IodineObject getEnv (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
			}
			IodineString str = args [0] as IodineString;

			if (str == null) {
				vm.RaiseException (new IodineTypeException ("Str"));
				return null;
			}
			if (Environment.GetEnvironmentVariable (str.Value) != null)
				return new IodineString (Environment.GetEnvironmentVariable (str.Value));
			else
				return null;
		}

		private IodineObject setEnv (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
			}
			IodineString str = args [0] as IodineString;
			Environment.SetEnvironmentVariable (str.Value, args [1].ToString (), EnvironmentVariableTarget.User);
			return null;
		}

		private IodineObject spawn (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
			}

			IodineString str = args [0] as IodineString;
			string cmdArgs = "";
			bool wait = true;

			if (str == null) {
				vm.RaiseException (new IodineTypeException ("Str"));
				return null;
			}

			if (args.Length >= 2) {
				IodineString cmdArgsObj = args [1] as IodineString;
				if (cmdArgsObj == null) {
					vm.RaiseException (new IodineTypeException ("Str"));
					return null;
				}
				cmdArgs = cmdArgsObj.Value;
			}

			if (args.Length >= 3) {
				IodineBool waitObj = args [2] as IodineBool;
				if (waitObj == null) {
					vm.RaiseException (new IodineTypeException ("Bool"));
					return null;
				}
				wait = waitObj.Value;
			}

			ProcessStartInfo info = new ProcessStartInfo (str.Value, cmdArgs);
			info.UseShellExecute = false;
			Process proc = Process.Start (info);
			if (wait) {
				proc.WaitForExit ();
			}
			return new IodineInteger (proc.ExitCode);
		}
	}
}

