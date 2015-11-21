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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Iodine.Runtime;

namespace Iodine.Runtime.Debug
{
	public class DebugServer
	{
		private VirtualMachine virtualMachine;
		private TcpListener debugServer;

		public DebugServer (VirtualMachine vm)
		{
			virtualMachine = vm;
		}

		public void Start (IPEndPoint endpoint)
		{
			debugServer = new TcpListener (endpoint);
			debugServer.Start ();
			Listen ();
		}

		private void Listen ()
		{
			while (true) {
				Socket socket = debugServer.AcceptSocket ();
				DebugSession session = new DebugSession (virtualMachine, socket);
				Thread thread = new Thread (() => {
					session.Connect ();
				});
				thread.Start ();
			}
		}
	}
}

