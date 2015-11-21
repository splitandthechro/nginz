// /**
//   * Copyright (c) 2015, GruntTheDivine All rights reserved.
//
//   * Redistribution and use in source and binary forms, with or without modification,
//   * are permitted provided that the following conditions are met:
//   * 
//   *  * Redistributions of source code must retain the above copyright notice, this list
//   *    of conditions and the following disclaimer.
//   * 
//   *  * Redistributions in binary form must reproduce the above copyright notice, this
//   *    list of conditions and the following disclaimer in the documentation and/or
//   *    other materials provided with the distribution.
//
//   * Neither the name of the copyright holder nor the names of its contributors may be
//   * used to endorse or promote products derived from this software without specific
//   * prior written permission.
//   * 
//   * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
//   * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
//   * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT
//   * SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
//   * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
//   * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR
//   * BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
//   * CONTRACT ,STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
//   * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
//   * DAMAGE.
// /**
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using Iodine.Runtime;
using Iodine.Compiler;

namespace Iodine.Runtime.Debug
{
	public class DebugSession
	{
		class DebugResponse 
		{
			public readonly Location Location;
			public readonly StackFrame Frame;

			public DebugResponse (Location location, StackFrame frame)
			{
				Location = location;
				Frame = frame;
			}
		}

		private VirtualMachine virtualMachine;
		private NetworkStream baseStream;
		private StreamReader requestStream;
		private StreamWriter responseStream;


		private Dictionary<string, string[]> fileCache = new Dictionary<string, string[]> ();

		public DebugSession (VirtualMachine vm, Socket socket)
		{
			virtualMachine = vm;
			baseStream = new NetworkStream (socket);
			requestStream = new StreamReader (baseStream);
			responseStream = new StreamWriter (baseStream);
		}

		public void Connect ()
		{
			while (true) {
				string command = requestStream.ReadLine ();
				SendResponse (InterpretCommand (command));
			}
		}

		private void SendResponse (DebugResponse response)
		{
			if (!fileCache.ContainsKey (response.Location.File)) {
				fileCache [response.Location.File] = File.ReadAllLines (response.Location.File);
			}

			StringBuilder sb = new StringBuilder ();
			sb.AppendFormat ("Line:{0};", response.Location.Line);
			sb.AppendFormat ("File:{0}", response.Location.File);
			sb.AppendFormat ("!{0}", fileCache [response.Location.File] [response.Location.Line - 1]);
			responseStream.WriteLine (sb.ToString ());
			responseStream.Flush ();
		}

		private DebugResponse InterpretCommand (string command)
		{
			string[] args = command.Split (' ');
			switch (args [0]) {
			case "s":
			case "step":
				return Step ();
			case "d":
			case "down":
				return Down ();
			case "n":
			case "next":
				return Next ();
			case "c":
			case "continue":
				virtualMachine.SetTrace (null);
				return null;
			}
			return null;
		}

		private DebugResponse Step ()
		{
			ManualResetEvent done = new ManualResetEvent (false);
			DebugResponse response = null;
			virtualMachine.SetTrace (((TraceType type,
				VirtualMachine vm,
				StackFrame frame,
				Location location) =>  {
				if (type == TraceType.Line) {
					response = new DebugResponse (location, frame);
					done.Set ();
					return true;
				}
				return false;
			}));
			virtualMachine.ContinueExecution ();
			done.WaitOne ();

			return response;
		}

		private DebugResponse Next ()
		{
			ManualResetEvent done = new ManualResetEvent (false);
			DebugResponse response = null;
			StackFrame currentFrame = virtualMachine.Top;
			virtualMachine.SetTrace (((TraceType type,
				VirtualMachine vm,
				StackFrame frame,
				Location location) =>  {
				if (type == TraceType.Line && frame == currentFrame) {
					response = new DebugResponse (location, frame);
					done.Set ();
					return true;
				}
				return false;
			}));
			virtualMachine.ContinueExecution ();
			done.WaitOne ();

			return response;
		}

		private DebugResponse Down ()
		{
			ManualResetEvent done = new ManualResetEvent (false);
			DebugResponse response = null;
			StackFrame currentFrame = virtualMachine.Top;
			virtualMachine.SetTrace (((TraceType type,
				VirtualMachine vm,
				StackFrame frame,
				Location location) =>  {
				if (type == TraceType.Line && frame.Parent == currentFrame) {
					response = new DebugResponse (location, frame);
					done.Set ();
					return true;
				}
				return false;
			}));
			virtualMachine.ContinueExecution ();
			done.WaitOne ();
			return response;
		}
	}
}

