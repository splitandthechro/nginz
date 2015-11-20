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
using System.Linq;

namespace Iodine.Runtime
{
	public sealed class IodineConfiguration
	{
		public int StackLimit {
			set;
			get;
		}

		public bool RestrictExtensions {
			set;
			get;
		}

		public int ThreadLimit {
			get;
			set;
		}

		public IodineConfiguration ()
		{
			// Defaults
			ThreadLimit = 1024;
			StackLimit = 8192;
			RestrictExtensions = false;
		}


		public void SetField (string name, string value)
		{
			switch (name) {
			case "stacklimit":
				StackLimit = Int32.Parse (value);
				break;
			case "threadlimit":
				ThreadLimit = Int32.Parse (value);
				break;
			case "restrictextensions":
				RestrictExtensions = value.ToLower () == "true";
				break;
			}
		}

		public static IodineConfiguration Load (string path)
		{
			IodineConfiguration config = new IodineConfiguration ();

			string[] lines = File.ReadAllLines (path);
			var configLines = lines.Where (p => p.Trim () != "" && !p.StartsWith ("#"));
			foreach (string configLine in configLines) {
				string line = configLine.Trim ();
				if (line.Contains (" ")) {
					string key = line.Substring (0, line.IndexOf (" "));
					string value = line.Substring (line.IndexOf (" ")).Trim ();
					config.SetField (key, value);
				}
			}
			return config;
		}
	}
}

