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
using System.Linq;
using System.Collections.Generic;

namespace Iodine.Runtime
{
	[IodineBuiltinModule ("io")]
	public class IOModule : IodineModule
	{
		class IodineDirectory : IodineObject
		{
			public readonly static IodineTypeDefinition DirectoryTypeDef = new IodineTypeDefinition ("Directory");

			public IodineDirectory ()
				: base (DirectoryTypeDef)
			{
				SetAttribute ("separator", new IodineString (Path.DirectorySeparatorChar.ToString ()));
				SetAttribute ("getFiles", new InternalMethodCallback (listFiles, this));
				SetAttribute ("getDirectories", new InternalMethodCallback (listDirectories, this));
				SetAttribute ("remove", new InternalMethodCallback (remove, this));
				SetAttribute ("removeTree", new InternalMethodCallback (removeTree, this));
				SetAttribute ("exists", new InternalMethodCallback (exists, this));
				SetAttribute ("create", new InternalMethodCallback (create, this));
				SetAttribute ("copy", new InternalMethodCallback (copy, this));
			}

			private IodineObject listFiles (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				if (args.Length <= 0) {
					vm.RaiseException (new IodineArgumentException (1));
					return null;
				}

				if (!(args [0] is IodineString)) {
					vm.RaiseException (new IodineTypeException ("Str"));
					return null;
				}

				if (!Directory.Exists (args [0].ToString ())) {
					vm.RaiseException (new IodineIOException ("Directory '" + args [0].ToString () +
					"' does not exist!"));
					return null;
				}

				IodineList ret = new IodineList (new IodineObject[]{ });

				foreach (string dir in Directory.GetFiles (args[0].ToString ())) {
					ret.Add (new IodineString (dir));
				}
				return ret;
			}

			private IodineObject listDirectories (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				if (args.Length <= 0) {
					vm.RaiseException (new IodineArgumentException (1));
					return null;
				}

				if (!(args [0] is IodineString)) {
					vm.RaiseException (new IodineTypeException ("Str"));
					return null;
				}

				if (!Directory.Exists (args [0].ToString ())) {
					vm.RaiseException (new IodineIOException ("Directory '" + args [0].ToString () +
					"' does not exist!"));
					return null;
				}

				IodineList ret = new IodineList (new IodineObject[]{ });

				foreach (string dir in Directory.GetDirectories (args[0].ToString ())) {
					ret.Add (new IodineString (dir));
				}
				return ret;
			}

			private IodineObject remove (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				if (args.Length <= 0) {
					vm.RaiseException (new IodineArgumentException (1));
					return null;
				}

				if (!(args [0] is IodineString)) {
					vm.RaiseException (new IodineTypeException ("Str"));
					return null;
				}

				if (!Directory.Exists (args [0].ToString ())) {
					vm.RaiseException (new IodineIOException ("Directory '" + args [0].ToString () +
					"' does not exist!"));
					return null;
				}

				Directory.Delete (args [0].ToString ());

				return null;
			}

			private IodineObject removeTree (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				if (args.Length <= 0) {
					vm.RaiseException (new IodineArgumentException (1));
					return null;
				}

				if (!(args [0] is IodineString)) {
					vm.RaiseException (new IodineTypeException ("Str"));
					return null;
				}

				if (!Directory.Exists (args [0].ToString ())) {
					vm.RaiseException (new IodineIOException ("Directory '" + args [0].ToString () +
					"' does not exist!"));
					return null;
				}

				rmDir (args [0].ToString ());

				return null;
			}

			private IodineObject exists (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				if (args.Length <= 0) {
					vm.RaiseException (new IodineArgumentException (1));
					return null;
				}

				if (!(args [0] is IodineString)) {
					vm.RaiseException (new IodineTypeException ("Str"));
					return null;
				}

				return IodineBool.Create (Directory.Exists (args [0].ToString ()));
			}

			private IodineObject create (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				if (args.Length <= 0) {
					vm.RaiseException (new IodineArgumentException (1));
					return null;
				}

				if (!(args [0] is IodineString)) {
					vm.RaiseException (new IodineTypeException ("Str"));
					return null;
				}
				Directory.CreateDirectory (args [0].ToString ());
				return null;
			}

			private IodineObject copy (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				if (args.Length <= 1) {
					vm.RaiseException (new IodineArgumentException (2));
					return null;
				}

				if (!(args [0] is IodineString) || !(args [1] is IodineString)) {
					vm.RaiseException (new IodineTypeException ("Str"));
					return null;
				}
				bool recurse = false;
				if (args.Length >= 3) {
					if (!(args [2] is IodineBool)) {
						vm.RaiseException (new IodineTypeException ("Bool"));
						return null;
					}
					recurse = ((IodineBool)args [2]).Value;
				}
				bool res = copyDir (args [0].ToString (), args [1].ToString (), recurse);
				if (!res) {
					vm.RaiseException (new IodineIOException ("Could not find directory " + args [0].ToString ()));
				}
				return null;
			}

			private static bool copyDir (string src, string dest, bool recurse)
			{
				DirectoryInfo dir = new DirectoryInfo (src);
				DirectoryInfo[] dirs = dir.GetDirectories ();

				if (!dir.Exists) {
					return false;
				}

				if (!Directory.Exists (dest)) {
					Directory.CreateDirectory (dest);
				}

				FileInfo[] files = dir.GetFiles ();
				foreach (FileInfo file in files) {
					string temppath = Path.Combine (dest, file.Name);
					file.CopyTo (temppath, false);
				}

				if (recurse) {
					foreach (DirectoryInfo subdir in dirs) {
						string temppath = Path.Combine (dest, subdir.Name);
						copyDir (subdir.FullName, temppath, recurse);
					}
				}
				return true;
			}

			private static bool rmDir (string target)
			{
				DirectoryInfo dir = new DirectoryInfo (target);
				DirectoryInfo[] dirs = dir.GetDirectories ();

				if (!dir.Exists) {
					return false;
				}

				FileInfo[] files = dir.GetFiles ();
				foreach (FileInfo file in files) {
					string temppath = Path.Combine (target, file.Name);
					File.Delete (temppath);
				}

				foreach (DirectoryInfo subdir in dirs) {
					string temppath = Path.Combine (target, subdir.Name);
					rmDir (temppath);
				}
				Directory.Delete (target);
				return true;
			}
		}


		class IodineFile : IodineObject
		{
			public readonly static IodineTypeDefinition FileTypeDef = new IodineTypeDefinition ("File");

			public IodineFile ()
				: base (FileTypeDef)
			{
				SetAttribute ("join", new InternalMethodCallback (join, this));
				SetAttribute ("remove", new InternalMethodCallback (remove, this));
				SetAttribute ("exists", new InternalMethodCallback (exists, this));
				SetAttribute ("getNameWithoutExt", new InternalMethodCallback (getNameWithoutExt, this));
				SetAttribute ("getName", new InternalMethodCallback (getName, this));
				SetAttribute ("copy", new InternalMethodCallback (copy, this));
			}

			private IodineObject join (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				string[] paths = args.Select (p => p.ToString ()).ToArray ();
				return new IodineString (Path.Combine (paths));
			}

			private IodineObject remove (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				if (args.Length <= 0) {
					vm.RaiseException (new IodineArgumentException (1));
					return null;
				}

				if (!(args [0] is IodineString)) {
					vm.RaiseException (new IodineTypeException ("Str"));
					return null;
				}

				if (!File.Exists (args [0].ToString ())) {
					vm.RaiseException (new IodineIOException ("File '" + args [0].ToString () +
					"' does not exist!"));
					return null;
				}

				File.Delete (args [0].ToString ());

				return null;
			}

			private IodineObject exists (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				if (args.Length <= 0) {
					vm.RaiseException (new IodineArgumentException (1));
					return null;
				}

				if (!(args [0] is IodineString)) {
					vm.RaiseException (new IodineTypeException ("Str"));
					return null;
				}

				return IodineBool.Create (File.Exists (args [0].ToString ()));
			}

			private IodineObject getNameWithoutExt (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				if (args.Length <= 0) {
					vm.RaiseException (new IodineArgumentException (1));
					return null;
				}

				if (!(args [0] is IodineString)) {
					vm.RaiseException (new IodineTypeException ("Str"));
					return null;
				}
				return new IodineString (Path.GetFileNameWithoutExtension (args [0].ToString ()));
			}

			private IodineObject getName (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				if (args.Length <= 0) {
					vm.RaiseException (new IodineArgumentException (1));
					return null;
				}

				if (!(args [0] is IodineString)) {
					vm.RaiseException (new IodineTypeException ("Str"));
					return null;
				}
				return new IodineString (Path.GetFileName (args [0].ToString ()));
			}

			private IodineObject copy (VirtualMachine vm, IodineObject self, IodineObject[] args)
			{
				if (args.Length <= 0) {
					vm.RaiseException (new IodineArgumentException (1));
					return null;
				}

				if (!(args [0] is IodineString) || !(args [1] is IodineString)) {
					vm.RaiseException (new IodineTypeException ("Str"));
					return null;
				}

				if (!File.Exists (args [0].ToString ())) {
					vm.RaiseException (new IodineIOException ("File '" + args [0].ToString () +
					"' does not exist!"));
					return null;
				}

				File.Copy (args [0].ToString (), args [1].ToString (), true);
				return null;
			}
		}

		public IOModule ()
			: base ("io")
		{
			SetAttribute ("Directory", new IodineDirectory ());
			SetAttribute ("File", new IodineFile ());
			SetAttribute ("getCreationTime", new InternalMethodCallback (getModifiedTime, this));
			SetAttribute ("getModifiedTime", new InternalMethodCallback (getCreationTime, this));
		}

		private IodineObject getModifiedTime (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}
			if (!(args [0] is IodineString)) {
				vm.RaiseException (new IodineTypeException ("Str"));
				return null;

			}
			if (!File.Exists (args [0].ToString ())) {
				vm.RaiseException (new IodineIOException ("File '" + args [0].ToString () +
				"' does not exist!"));
				return null;
			}
			return new DateTimeModule.IodineTimeStamp (File.GetLastAccessTime (args [0].ToString ()));
		}

		private IodineObject getCreationTime (VirtualMachine vm, IodineObject self, IodineObject[] args)
		{
			if (args.Length <= 0) {
				vm.RaiseException (new IodineArgumentException (1));
				return null;
			}
			if (!(args [0] is IodineString)) {
				vm.RaiseException (new IodineTypeException ("Str"));
				return null;
			}
			if (!File.Exists (args [0].ToString ())) {
				vm.RaiseException (new IodineIOException ("File '" + args [0].ToString () +
				"' does not exist!"));
				return null;
			}
			return new DateTimeModule.IodineTimeStamp (File.GetCreationTime (args [0].ToString ()));
		}
	}
}

