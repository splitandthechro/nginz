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

#if COMPILE_EXTRAS

using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Net.Security;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Iodine.Runtime;

namespace Iodine.Modules.Extras
{
	[IodineBuiltinModule ("socket")]
	internal class SocketModule : IodineModule
	{
		public class IodineProtocolType : IodineObject
		{
			private static IodineTypeDefinition SocketProtocalTypeTypeDef = new IodineTypeDefinition ("Socket");

			public ProtocolType Type { private set; get; }

			public IodineProtocolType (ProtocolType protoType)
				: base (SocketProtocalTypeTypeDef)
			{
				this.Type = protoType;
			}
		}

		public class IodineSocketType : IodineObject
		{
			private static IodineTypeDefinition SocketTypeTypeDef = new IodineTypeDefinition ("Socket");

			public SocketType Type { private set; get; }

			public IodineSocketType (SocketType sockType)
				: base (SocketTypeTypeDef)
			{
				this.Type = sockType;
			}
		}

		public class IodineSocketException : IodineException
		{
			
		}

		public class IodineSocket : IodineObject
		{
			private static IodineTypeDefinition SocketTypeDef = new IodineTypeDefinition ("Socket");

			public Socket Socket { private set; get; }

			private System.IO.Stream stream;
			private string host;

			private static bool ValidateServerCertificate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
			{
				return true;
			}

			public IodineSocket (Socket sock)
				: base (SocketTypeDef)
			{
				Socket = sock;
				SetAttribute ("connect", new InternalMethodCallback (connect, this));
				SetAttribute ("connectSsl", new InternalMethodCallback (connectSsl, this));
				SetAttribute ("send", new InternalMethodCallback (send, this));
				SetAttribute ("bind", new InternalMethodCallback (bind, this));
				SetAttribute ("accept", new InternalMethodCallback (accept, this));
				SetAttribute ("acceptSsl", new InternalMethodCallback (acceptSsl, this));
				SetAttribute ("listen", new InternalMethodCallback (listen, this));
				SetAttribute ("receive", new InternalMethodCallback (receive, this));
				SetAttribute ("receiveRaw", new InternalMethodCallback (receiveRaw, this));
				SetAttribute ("getBytesAvailable", new InternalMethodCallback (getBytesAvailable, this));
				SetAttribute ("getStream", new InternalMethodCallback (getStream, this));
				SetAttribute ("close", new InternalMethodCallback (close, this));
				SetAttribute ("setHost", new InternalMethodCallback (setHost, this));
				SetAttribute ("isConnected", new InternalMethodCallback (connected, this));
				host = string.Empty;
			}

			public IodineSocket (SocketType sockType, ProtocolType protoType)
				: this (new Socket (AddressFamily.InterNetwork, sockType, protoType))
			{
			}

			private IodineObject connected (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				try {
					var result = !((Socket.Poll (1000, SelectMode.SelectRead)
					             && (Socket.Available == 0)) || !Socket.Connected);
					return IodineBool.Create (result);
				} catch {
					return IodineBool.False;
				}
			}

			private IodineObject setHost (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				if (args.Length <= 0) {
					vm.RaiseException (new IodineArgumentException (1));
					return null;
				}
				IodineString hostObj = args [0] as IodineString;

				if (hostObj == null) {
					vm.RaiseException (new IodineTypeException ("Str"));
				}

				host = hostObj.ToString ();
				return null;
			}

			private IodineObject close (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				Socket.Shutdown (SocketShutdown.Both);
				Socket.Close ();
				return null;
			}

			private IodineObject bind (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				if (args.Length < 2) {
					vm.RaiseException (new IodineArgumentException (2));
					return null;
				}

				IodineString ipAddrStr = args [0] as IodineString;
				IodineInteger portObj = args [1] as IodineInteger;

				if (ipAddrStr == null) {
					vm.RaiseException (new IodineTypeException ("Str"));
					return null;
				} else if (portObj == null) {
					vm.RaiseException (new IodineTypeException ("Int"));
					return null;
				}

				IPAddress ipAddr;
				int port = (int)portObj.Value;
				EndPoint endPoint = null;
				if (!IPAddress.TryParse (ipAddrStr.ToString (), out ipAddr)) {
					endPoint = new IPEndPoint (DnsLookUp (ipAddrStr.ToString ()), port);
				} else {
					endPoint = new IPEndPoint (ipAddr, port);
				}
				try {
					Socket.Bind (endPoint);
				} catch {
					vm.RaiseException ("Could not bind to socket!");
					return null;
				}
				return null;
			}

			private IodineObject listen (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				IodineInteger backlogObj = args [0] as IodineInteger;
				try {
					int backlog = (int)backlogObj.Value;
					Socket.Listen (backlog);
				} catch {
					vm.RaiseException ("Could not listen to socket!");
					return null;
				}
				return null;
			}

			private IodineSocket accept (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				IodineSocket sock = new IodineSocket (Socket.Accept ());
				sock.stream = new NetworkStream (sock.Socket);
				return sock;
			}

			private IodineSocket acceptSsl (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				IodineSocket sock = new IodineSocket (this.Socket.Accept ());
				sock.stream = new SslStream (new NetworkStream (Socket), false, ValidateServerCertificate, null);
				// I have no idea what I'm doing lol
				((SslStream)sock.stream).AuthenticateAsClient (host);
				return sock;
			}

			private IodineObject connect (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				IodineString ipAddrStr = args [0] as IodineString;
				IodineInteger portObj = args [1] as IodineInteger;
				IPAddress ipAddr;
				int port = (int)portObj.Value;

				EndPoint endPoint = null;
				if (!IPAddress.TryParse (ipAddrStr.ToString (), out ipAddr)) {
					endPoint = new IPEndPoint (DnsLookUp (ipAddrStr.ToString ()), port);
				} else {
					endPoint = new IPEndPoint (ipAddr, port);
				}

				try {
					Socket.Connect (endPoint);
					stream = new NetworkStream (this.Socket);
				} catch (Exception ex) {
					vm.RaiseException ("Could not connect to socket! (Reason: {0})", ex.Message);
					return null;
				}

				return null;
			}

			private IodineObject connectSsl (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				IodineString ipAddrStr = args [0] as IodineString;
				IodineInteger portObj = args [1] as IodineInteger;
				IPAddress ipAddr;
				int port = (int)portObj.Value;

				EndPoint endPoint = null;
				if (!IPAddress.TryParse (ipAddrStr.ToString (), out ipAddr)) {
					endPoint = new IPEndPoint (DnsLookUp (ipAddrStr.ToString ()), port);
				} else {
					endPoint = new IPEndPoint (ipAddr, port);
				}

				try {
					Socket.Connect (endPoint);
				} catch {
					vm.RaiseException ("Could not connect to socket!");
					return null;
				}

				try {
					stream = new SslStream (new NetworkStream (Socket), false, ValidateServerCertificate, null);
				} catch (Exception e) {
					vm.RaiseException (e.Message);
					return null;
				}

				try {
					((SslStream)stream).AuthenticateAsClient (host);
				} catch (Exception e) {
					vm.RaiseException (e.Message);
					return null;
				}

				return null;
			}

			private IodineObject getStream (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				return new IodineStream (stream, true, true);
			}

			private IodineObject getBytesAvailable (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				return new IodineInteger (Socket.Available);
			}

			private IodineObject send (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				foreach (IodineObject obj in args) {
					if (obj is IodineInteger) {
						stream.WriteByte ((byte)((IodineInteger)obj).Value);
						stream.Flush ();
					} else if (obj is IodineString) {
						var buf = Encoding.UTF8.GetBytes (obj.ToString ());
						stream.Write (buf, 0, buf.Length);
						stream.Flush ();
					}
				}
				return null;
			}

			private IodineByteArray receiveRaw (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				IodineInteger n = args [0] as IodineInteger;
				byte[] buf = new byte[n.Value];
				for (int i = 0; i < n.Value; i++)
					buf [i] = (byte)stream.ReadByte ();
				return new IodineByteArray (buf);
			}

			private IodineObject receive (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				IodineInteger n = args [0] as IodineInteger;
				StringBuilder accum = new StringBuilder ();
				for (int i = 0; i < n.Value; i++) {
					int b = stream.ReadByte ();
					if (b != -1)
						accum.Append ((char)b);
				}
				return new IodineString (accum.ToString ());
			}

			private IodineObject readLine (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				StringBuilder accum = new StringBuilder ();
				int b = stream.ReadByte ();
				while (b != -1 && b != '\n') {
					accum.Append ((char)b);
					b = stream.ReadByte ();
				}
				return new IodineString (accum.ToString ());
			}

			/*
			 * I have no idea why, but for some reason using DnsEndPoint for establishing a 
			 * socket connection throws a FeatureNotImplemented exception on Mono 4.0.3 so
			 * this will have to do 
			 */
			private static IPAddress DnsLookUp (string host)
			{
				IPHostEntry entries = Dns.GetHostEntry (host);
				return entries.AddressList [0];
			}
		}

		public SocketModule ()
			: base ("socket")
		{
			SetAttribute ("SOCK_DGRAM", new IodineSocketType (SocketType.Dgram));
			SetAttribute ("SOCK_RAW", new IodineSocketType (SocketType.Raw));
			SetAttribute ("SOCK_RDM", new IodineSocketType (SocketType.Rdm));
			SetAttribute ("SOCK_SEQPACKET", new IodineSocketType (SocketType.Seqpacket));
			SetAttribute ("SOCK_STREAM", new IodineSocketType (SocketType.Stream));
			SetAttribute ("PROTO_TCP", new IodineProtocolType (ProtocolType.Tcp));
			SetAttribute ("PROTO_IP", new IodineProtocolType (ProtocolType.IP));
			SetAttribute ("PROTO_IPV4", new IodineProtocolType (ProtocolType.IPv4));
			SetAttribute ("PROTO_IPV6", new IodineProtocolType (ProtocolType.IPv6));
			SetAttribute ("PROTO_UDP", new IodineProtocolType (ProtocolType.Udp));
			SetAttribute ("socket", new InternalMethodCallback (socket, this));
		}

		private IodineObject socket (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			IodineSocketType sockType = args [0] as IodineSocketType;
			IodineProtocolType protoType = args [1] as IodineProtocolType;
			return new IodineSocket (sockType.Type, protoType.Type);
		}
	}
}


#endif
