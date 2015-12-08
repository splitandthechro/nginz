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
using System.Security.Cryptography;

namespace Iodine.Runtime
{
	[IodineBuiltinModule ("hash")]
	public class HashModule : IodineModule
	{
		public HashModule ()
			: base ("hash")
		{
			SetAttribute ("sha1", new InternalMethodCallback (sha1, this));
			SetAttribute ("sha256", new InternalMethodCallback (sha256, this));
			SetAttribute ("sha512", new InternalMethodCallback (sha512, this));
			SetAttribute ("md5", new InternalMethodCallback (md5, this));
		}

		private IodineObject sha256 (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}

			byte[] bytes = new byte[]{};
			byte[] hash = null;

			SHA256Managed hashstring = new SHA256Managed();

			if (args[0] is IodineString) {
				bytes = System.Text.Encoding.UTF8.GetBytes (args[0].ToString ());
				hash = hashstring.ComputeHash(bytes);
			} else if (args[0] is IodineByteArray) {
				bytes = ((IodineByteArray)args[0]).Array;
				hash = hashstring.ComputeHash(bytes);
			} else if (args[0] is IodineStream) {
				hash = hashstring.ComputeHash(((IodineStream)args[0]).File);
			} else {
				vm.RaiseException (new IodineTypeException ("Str"));
				return null;
			}

			return new IodineByteArray (hash);
		}

		private IodineObject sha1 (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}

			byte[] bytes = new byte[]{};
			byte[] hash = null;

			SHA1Managed hashstring = new SHA1Managed();

			if (args[0] is IodineString) {
				bytes = System.Text.Encoding.UTF8.GetBytes (args[0].ToString ());
				hash = hashstring.ComputeHash(bytes);
			} else if (args[0] is IodineByteArray) {
				bytes = ((IodineByteArray)args[0]).Array;
				hash = hashstring.ComputeHash(bytes);
			} else if (args[0] is IodineStream) {
				hash = hashstring.ComputeHash(((IodineStream)args[0]).File);
			} else {
				vm.RaiseException (new IodineTypeException ("Str"));
				return null;
			}

			return new IodineByteArray (hash);
		}

		private IodineObject sha512 (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}

			byte[] bytes = new byte[]{};
			byte[] hash = null;

			SHA512Managed hashstring = new SHA512Managed();

			if (args[0] is IodineString) {
				bytes = System.Text.Encoding.UTF8.GetBytes (args[0].ToString ());
				hash = hashstring.ComputeHash(bytes);
			} else if (args[0] is IodineByteArray) {
				bytes = ((IodineByteArray)args[0]).Array;
				hash = hashstring.ComputeHash(bytes);
			} else if (args[0] is IodineStream) {
				hash = hashstring.ComputeHash(((IodineStream)args[0]).File);
			} else {
				vm.RaiseException (new IodineTypeException ("Str"));
				return null;
			}

			return new IodineByteArray (hash);
		}

		private IodineObject md5 (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}

			byte[] bytes = new byte[]{};
			byte[] hash = null;

			MD5 hashstring = MD5.Create ();

			if (args[0] is IodineString) {
				bytes = System.Text.Encoding.UTF8.GetBytes (args[0].ToString ());
				hash = hashstring.ComputeHash(bytes);
			} else if (args[0] is IodineByteArray) {
				bytes = ((IodineByteArray)args[0]).Array;
				hash = hashstring.ComputeHash(bytes);
			} else if (args[0] is IodineStream) {
				hash = hashstring.ComputeHash(((IodineStream)args[0]).File);
			} else {
				vm.RaiseException (new IodineTypeException ("Str"));
				return null;
			}

			return new IodineByteArray (hash);
		}
	}
}

