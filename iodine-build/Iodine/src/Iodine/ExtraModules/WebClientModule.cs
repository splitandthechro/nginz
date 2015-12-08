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
using Iodine.Runtime;

namespace Iodine.Modules.Extras
{
	[IodineBuiltinModule ("webclient")]
	internal class WebClientModule : IodineModule
	{
		public class IodineWebClient : IodineObject
		{
			private static IodineTypeDefinition WebClientTypeDef =
				new IodineTypeDefinition ("WebClient");

			private WebClient client;

			public IodineWebClient ()
				: base (WebClientTypeDef)
			{
				SetAttribute ("downloadString", new InternalMethodCallback (downloadString, this));
				SetAttribute ("downloadRaw", new InternalMethodCallback (downloadRaw, this));
				SetAttribute ("downloadFile", new InternalMethodCallback (downloadFile, this));
				SetAttribute ("uploadFile", new InternalMethodCallback (uploadFile, this));
				WebProxy proxy = new WebProxy ();
				client = new WebClient ();
				client.Proxy = proxy;
			}

			private IodineObject downloadString (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
				IodineString uri = args [0] as IodineString;
				string data;
				try {
					data = this.client.DownloadString (uri.ToString ());
				} catch (Exception e) {
					vm.RaiseException (e.Message);
					return null;
				}
				return new IodineString (data);
			}

			private IodineObject downloadRaw (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
				IodineString uri = args [0] as IodineString;
				byte[] data;
				try {
					data = client.DownloadData (uri.ToString ());
				} catch (Exception e) {
					vm.RaiseException (e.Message);
					return null;
				}
				return new IodineByteArray (data);
			}

			private IodineObject downloadFile (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
				IodineString uri = args [0] as IodineString;
				IodineString file = args [1] as IodineString;

				try {
					client.DownloadFile (uri.ToString (), file.ToString ());
				} catch (Exception e) {
					vm.RaiseException (e.Message);
				}
				return null;

			}

			private IodineObject uploadFile (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
				IodineString uri = args [0] as IodineString;
				IodineString file = args [1] as IodineString;
				client.UploadFile (uri.ToString (), file.ToString ());
				return null;
			}
		}

		public WebClientModule () : base ("webclient")
		{
			SetAttribute ("WebClient", new InternalMethodCallback (webclient, this));
			SetAttribute ("disableCertificateCheck", new InternalMethodCallback (disableCertCheck, this));
		}

		private IodineObject webclient (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			return new IodineWebClient ();
		}

		private IodineObject disableCertCheck (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true; 
			return null;
		}
	}
}

#endif