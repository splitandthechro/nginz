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
using Iodine.Compiler;

namespace Iodine.Runtime
{
	/*
	 * Should I move all these classes into seperate files? Make a subdirectory for them?
	 * TODO: Decide
	 */
	public class IodineException : IodineObject
	{
		public static readonly IodineTypeDefinition TypeDefinition = new ExceptionTypeDef ();

		class ExceptionTypeDef : IodineTypeDefinition
		{
			public ExceptionTypeDef ()
				: base ("Exception")
			{
			}

			public override IodineObject Invoke (VirtualMachine vm, IodineObject[] args)
			{
				if (args.Length <= 0) {
					vm.RaiseException (new IodineArgumentException (1));
				}
				return new IodineException ("{0}", args [0].ToString ());
			}

		}

		public readonly string Message;

		public SourceLocation Location { set; get; }

		public IodineException ()
			: base (TypeDefinition)
		{
		}

		public IodineException (string format, params object[] args)
			: base (TypeDefinition)
		{
			Message = String.Format (format, args);
			SetAttribute ("message", new IodineString (this.Message));
		}

		public IodineException (IodineTypeDefinition typeDef, string format, params object[] args)
			: base (typeDef)
		{
			Message = String.Format (format, args);
			SetAttribute ("message", new IodineString (this.Message));
		}
			
	}

	public class IodineStackOverflow : IodineException 
	{
		public IodineStackOverflow ()
			: base ("StackOverflow")
		{
			SetAttribute ("message", new IodineString ("Stack overflow"));
		}
	}

	public class IodineTypeException : IodineException
	{
		public static new readonly IodineTypeDefinition TypeDefinition = new TypeExceptionTypeDef ();

		class TypeExceptionTypeDef : IodineTypeDefinition
		{
			public TypeExceptionTypeDef ()
				: base ("TypeException")
			{
			}

			public override IodineObject Invoke (VirtualMachine vm, IodineObject[] args)
			{
				if (args.Length <= 0) {
					vm.RaiseException (new IodineArgumentException (1));
				}
				return new IodineTypeException (args [0].ToString ());
			}
		}

		public IodineTypeException (string expectedType)
			: base (TypeDefinition, "Expected type '{0}'", expectedType)
		{
			Base = new IodineException ();
		}
	}

	public class IodineTypeCastException : IodineException
	{
		public static new readonly IodineTypeDefinition TypeDefinition = new TypeCastExceptionTypeDef ();

		class TypeCastExceptionTypeDef : IodineTypeDefinition
		{
			public TypeCastExceptionTypeDef ()
				: base ("TypeCastException")
			{
			}

			public override IodineObject Invoke (VirtualMachine vm, IodineObject[] args)
			{
				if (args.Length <= 0) {
					vm.RaiseException (new IodineArgumentException (1));
				}
				return new IodineTypeCastException (args [0].ToString ());
			}
		}

		public IodineTypeCastException (string expectedType)
			: base (TypeDefinition, "Could not convert to type '{0}'!", expectedType)
		{
			this.Base = new IodineException ();
		}
	}

	public class IodineIndexException : IodineException
	{
		public static new readonly IodineTypeDefinition TypeDefinition = new IndexExceptionTypeDef ();

		class IndexExceptionTypeDef : IodineTypeDefinition
		{
			public IndexExceptionTypeDef ()
				: base ("IndexException")
			{
			}

			public override IodineObject Invoke (VirtualMachine vm, IodineObject[] args)
			{
				return new IodineIndexException ();
			}
		}

		public IodineIndexException ()
			: base (TypeDefinition, "Index out of range!")
		{
			Base = new IodineException ();
		}
	}

	public class IodineKeyNotFound : IodineException
	{
		public static new readonly IodineTypeDefinition TypeDefinition = new KeyNotFoundTypeDef ();

		class KeyNotFoundTypeDef : IodineTypeDefinition
		{
			public KeyNotFoundTypeDef ()
				: base ("KeyNotFoundException")
			{
			}

			public override IodineObject Invoke (VirtualMachine vm, IodineObject[] args)
			{
				return new IodineKeyNotFound ();
			}
		}

		public IodineKeyNotFound ()
			: base (TypeDefinition, "Key not found!")
		{
			this.Base = new IodineException ();
		}
	}

	public class IodineAttributeNotFoundException : IodineException
	{
		public static new readonly IodineTypeDefinition TypeDefinition = new AttributeNotFoundExceptionTypeDef ();

		class AttributeNotFoundExceptionTypeDef : IodineTypeDefinition
		{
			public AttributeNotFoundExceptionTypeDef ()
				: base ("AttributeNotFoundException")
			{
			}
		}

		public IodineAttributeNotFoundException (string name)
			: base (TypeDefinition, "Attribute '{0}' not found!", name)
		{
			Base = new IodineException ();
		}
	}

	public class IodineInternalErrorException : IodineException
	{
		public static new readonly IodineTypeDefinition TypeDefinition = new InternalErrorExceptionTypeDef ();

		class InternalErrorExceptionTypeDef : IodineTypeDefinition
		{
			public InternalErrorExceptionTypeDef ()
				: base ("InternalException")
			{
			}
		}

		public IodineInternalErrorException (Exception ex)
			: base (TypeDefinition, "Internal exception: {0}\n Inner Exception: ",
			        ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message)
		{
			Base = new IodineException ();
		}
	}

	public class IodineArgumentException : IodineException
	{
		public static new readonly IodineTypeDefinition TypeDefinition = new ArgumentExceptionTypeDef ();

		class ArgumentExceptionTypeDef : IodineTypeDefinition
		{
			public ArgumentExceptionTypeDef ()
				: base ("ArgumentException")
			{
			}
		}

		public IodineArgumentException (int argCount)
			: base (TypeDefinition, "Expected {0} or more arguments!", argCount)
		{
			Base = new IodineException ();
		}
	}

	public class IodineIOException : IodineException
	{
		public static new readonly IodineTypeDefinition TypeDefinition = new IOExceptionTypeDef ();

		class IOExceptionTypeDef : IodineTypeDefinition
		{
			public IOExceptionTypeDef ()
				: base ("IOException")
			{
			}
		}

		public IodineIOException (string msg)
			: base (TypeDefinition, msg)
		{
			Base = new IodineException ();
		}
	}

	public class IodineSyntaxException : IodineException
	{
		public static new readonly IodineTypeDefinition TypeDefinition = new SyntaxExceptionTypeDef ();

		class SyntaxExceptionTypeDef : IodineTypeDefinition
		{
			public SyntaxExceptionTypeDef ()
				: base ("SynaxErrorException")
			{
			}
		}

		public IodineSyntaxException (ErrorLog errorLog)
			: base (TypeDefinition, "Syntax error")
		{
			Base = new IodineException ();
			IodineObject[] errors = new IodineObject[errorLog.ErrorCount];
			int i = 0;
			foreach (Error error in errorLog.Errors) {
				SourceLocation loc = error.Location;
				string text = String.Format ("{0} ({1}:{2}) error: {3}", Path.GetFileName (loc.File),
					              loc.Line, loc.Column, error.Text);
				errors [i++] = new IodineString (text);
			}
			SetAttribute ("errors", new IodineTuple (errors));
		}
	}

	public class IodineNotSupportedException : IodineException
	{
		public static new readonly IodineTypeDefinition TypeDefinition = new NotSupportedExceptionTypeDef ();

		class NotSupportedExceptionTypeDef : IodineTypeDefinition
		{
			public NotSupportedExceptionTypeDef ()
				: base ("NotSupportedException")
			{
			}

			public override IodineObject Invoke (VirtualMachine vm, IodineObject[] args)
			{
				return new IodineIndexException ();
			}
		}

		public IodineNotSupportedException ()
			: base (TypeDefinition, "The requested feature is not supported!")
		{
			Base = new IodineException ();
		}

		public IodineNotSupportedException (string message)
			: base (TypeDefinition, message)
		{
			Base = new IodineException ();
		}
	}

	public class UnhandledIodineExceptionException : Exception
	{
		public IodineObject OriginalException { private set; get; }

		public StackFrame Frame { private set; get; }

		public UnhandledIodineExceptionException (StackFrame frame, IodineObject original)
		{
			OriginalException = original;
			Frame = frame;
		}

		public void PrintStack ()
		{
			StackFrame top = Frame;
			Console.WriteLine ("Stack trace:");
			Console.WriteLine ("------------");
			while (top != null) {
				Console.WriteLine (" at {0} (Module: {1}, Line: {2})", top.Method.Name, top.Module.Name,
					top.Location != null ?
					top.Location.Line + 1 : 
					0);
				
				top = top.Parent;
			}
		}
	}
}

