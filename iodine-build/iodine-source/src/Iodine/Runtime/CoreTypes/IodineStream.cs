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
using System.Text;
using System.Collections.Generic;

namespace Iodine.Runtime
{
	public class IodineStream : IodineObject
	{
		private static readonly IodineTypeDefinition FileTypeDef = new IodineTypeDefinition ("File");

		public bool Closed { set; get; }

		public Stream File { private set; get; }

		public bool CanRead { private set; get; }

		public bool CanWrite { private set; get; }

		public IodineStream (Stream file, bool canWrite, bool canRead)
			: base (FileTypeDef)
		{
			File = file;
			CanRead = canRead;
			CanWrite = canWrite;
			SetAttribute ("write", new InternalMethodCallback (write, this));
			SetAttribute ("writeBytes", new InternalMethodCallback (writeBytes, this));
			SetAttribute ("read", new InternalMethodCallback (read, this));
			SetAttribute ("readByte", new InternalMethodCallback (readByte, this));
			SetAttribute ("readBytes", new InternalMethodCallback (readBytes, this));
			SetAttribute ("readLine", new InternalMethodCallback (readLine, this));
			SetAttribute ("tell", new InternalMethodCallback (readLine, this));
			SetAttribute ("getSize", new InternalMethodCallback (getSize, this));
			SetAttribute ("close", new InternalMethodCallback (close, this));
			SetAttribute ("flush", new InternalMethodCallback (flush, this));
			SetAttribute ("readAllText", new InternalMethodCallback (readAllText, this));
			SetAttribute ("readAllBytes", new InternalMethodCallback (readAllBytes, this));
		}


		public override IodineObject Len (VirtualMachine vm)
		{
			return new IodineInteger (File.Length);
		}

		public override void Exit (VirtualMachine vm)
		{
			if (!Closed) {
				File.Close ();
				File.Dispose ();
			}
		}

		private IodineObject write (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (Closed) { 
				vm.RaiseException (new IodineIOException ("Stream has been closed!"));
				return null;
			}

			if (!CanWrite) {
				vm.RaiseException (new IodineIOException ("Can not write to stream!"));
				return null;
			}

			foreach (IodineObject obj in args) {
				if (obj is IodineString) {
					write (obj.ToString ());
				} else if (obj is IodineBytes) {
					IodineBytes arr = obj as IodineBytes;
					File.Write (arr.Value, 0, arr.Value.Length);
				} else if (obj is IodineInteger) {
					IodineInteger intVal = obj as IodineInteger;
					write ((byte)intVal.Value);
				} else if (obj is IodineByteArray) {
					IodineByteArray arr = obj as IodineByteArray;
					File.Write (arr.Array, 0, arr.Array.Length);
				} else {
					vm.RaiseException (new IodineTypeException (""));
				}
			}
			return null;
		}

		private IodineObject writeBytes (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (Closed) { 
				vm.RaiseException (new IodineIOException ("Stream has been closed!"));
				return null;
			}

			if (!CanWrite) {
				vm.RaiseException (new IodineIOException ("Can not write to stream!"));
				return null;
			}

			IodineByteArray arr = args [0] as IodineByteArray;
			File.Write (arr.Array, 0, arr.Array.Length);

			return null;
		}

		private IodineObject read (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (Closed) { 
				vm.RaiseException ("Stream has been closed!");
				return null;
			}

			if (!CanRead) {
				vm.RaiseException ("Stream is not open for reading!");
				return null;
			}

			if (args [0] is IodineInteger) {
				IodineInteger intv = args [0] as IodineInteger;
				byte[] buf = new byte[(int)intv.Value];
				File.Read (buf, 0, buf.Length);
				return new IodineString (Encoding.UTF8.GetString (buf));
			}
			vm.RaiseException (new IodineTypeException ("Int"));
			return null;
		}

		private IodineObject readByte (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (Closed) { 
				vm.RaiseException ("Stream has been closed!");
				return null;
			}

			if (!CanWrite) {
				vm.RaiseException ("Stream is not open for reading!");
				return null;
			}

			return new IodineInteger (File.ReadByte ());
		}

		private IodineObject readBytes (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (Closed) { 
				vm.RaiseException ("Stream has been closed!");
				return null;
			}

			if (!CanRead) {
				vm.RaiseException ("Stream is not open for reading!");
				return null;
			}

			if (args [0] is IodineInteger) {
				IodineInteger intv = args [0] as IodineInteger;
				byte[] buf = new byte[(int)intv.Value];
				File.Read (buf, 0, buf.Length);
				return new IodineBytes (buf);
			}
			vm.RaiseException (new IodineTypeException ("Int"));
			return null;
		}

		private IodineObject readLine (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (Closed) { 
				vm.RaiseException ("Stream has been closed!");
				return null;
			}

			if (!CanWrite) {
				vm.RaiseException ("Stream is not open for reading!");
				return null;
			}

			return new IodineString (readLine ());
		}

		private IodineObject tell (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (this.Closed) { 
				vm.RaiseException ("Stream has been closed!");
			}
			return new IodineInteger (File.Position);
		}

		private IodineObject getSize (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (this.Closed) { 
				vm.RaiseException ("Stream has been closed!");
			}
			return new IodineInteger (File.Length);
		}

		private IodineObject close (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (this.Closed) { 
				vm.RaiseException ("Stream has been closed!");
			}
			File.Close ();
			return null;
		}

		private IodineObject flush (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (this.Closed) { 
				vm.RaiseException ("Stream has been closed!");
			}
			File.Flush ();
			return null;
		}

		private IodineObject readAllText (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (this.Closed) { 
				vm.RaiseException ("Stream has been closed!");
			}

			StringBuilder builder = new StringBuilder ();
			int ch = 0;
			while ((ch = File.ReadByte ()) != -1) {
				builder.Append ((char)ch);
			}
			return new IodineString (builder.ToString ());
		}

		private IodineObject readAllBytes (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (Closed) { 
				vm.RaiseException ("Stream has been closed!");
			}

			List<byte> bytes = new List<byte> (File.CanSeek ? (int)File.Length : 256);
			int ch = 0;
			while ((ch = File.ReadByte ()) != -1) {
				bytes.Add ((byte)ch);
			}
			return new IodineBytes (bytes.ToArray ());
		}

		private void write (string str)
		{
			foreach (char c in str) {
				File.WriteByte ((byte)c);
			}
		}

		public void write (byte b)
		{
			File.WriteByte (b);
		}

		public string readLine ()
		{
			StringBuilder builder = new StringBuilder ();
			int ch = 0;
			while ((ch = File.ReadByte ()) != '\n' && ch != '\r' && ch != -1) {
				builder.Append ((char)ch);
			}
			return builder.ToString ();
		}
	}
}

