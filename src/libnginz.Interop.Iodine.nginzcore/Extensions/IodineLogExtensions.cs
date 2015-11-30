using System;
using Iodine.Runtime;

namespace nginz.Interop.Iodine.nginzcore
{

	/// <summary>
	/// Log extensions.
	/// </summary>
	public static class IodineLogExtensions
	{
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

