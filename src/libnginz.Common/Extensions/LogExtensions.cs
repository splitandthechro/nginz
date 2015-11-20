using System;

namespace nginz.Common
{
	public static class LogExtensions
	{
		public static void Log<T> (this T dummy, string format, params object[] args)
			where T : class, ICanLog {
			Console.WriteLine ("[{0}] {1}", dummy.GetType ().Name, string.Format (format, args));
		}

		public static void Throw<T> (this T dummy, string format, params object[] args)
			where T : class, ICanThrow {
			throw new Exception (string.Format ("[{0}] {1}", dummy.GetType ().Name, string.Format (format, args)));
		}
	}
}

