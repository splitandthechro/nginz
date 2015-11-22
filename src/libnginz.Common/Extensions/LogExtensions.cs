using System;
using System.Diagnostics;
using Iodine.Runtime;

namespace nginz.Common
{
	/// <summary>
	/// Log extensions.
	/// </summary>
	public static class LogExtensions
	{
		public static void Log<T> (this T dummy, string format, params object[] args)
			where T : class, ICanLog {
			Console.WriteLine ("[{0}] {1}", dummy.GetType ().Name, string.Format (format, args));
		}

		public static void Throw (string format, params object[] args) {
			var stacktrace = new StackTrace (skipFrames: 1);
			var stackframe = stacktrace.GetFrame (0);
			var classname = stackframe.GetMethod ().DeclaringType.Name;
			throw new Exception (string.Format ("[{0}] {1}", classname, string.Format (format, args)));
		}
		
		public static void Throw<T> (this T dummy, string format, params object[] args)
			where T : class, ICanThrow {
			var message = string.Format (format, args);
			throw new Exception (string.Format ("[{0}] {1}", dummy.GetType ().Name, message));
		}

		public static void IodineInfo<T> (this T dummy, string format, params object[] args)
			where T : IodineObject {
			var message = string.Format (format, args);
			Console.WriteLine (string.Format ("[IodineVM::Info] {0}", message));
		}

		public static void IodineError<T> (this T dummy, string format, params object[] args)
			where T : IodineObject {
			var message = string.Format (format, args);
			Console.Error.WriteLine (string.Format ("[IodineVM::Error] {0}", message));
		}
	}
}

