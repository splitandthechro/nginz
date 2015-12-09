using System;

namespace nginz.Common
{
	public static class ScriptEvents
	{
		/// <summary>
		/// Script state changed event arguments.
		/// </summary>
		public delegate void ScriptStateChangedEventArgs (Script script);

		/// <summary>
		/// Occurs when a script gets unloaded.
		/// </summary>
		public static event ScriptStateChangedEventArgs LoadScript;

		/// <summary>
		/// Occurs when a script gets unloaded.
		/// </summary>
		public static event ScriptStateChangedEventArgs UnloadScript;

		static ScriptEvents () {
			LoadScript += delegate { };
			UnloadScript += delegate { };
		}

		public static void Load (Script script) {
			LoadScript (script);
		}

		public static void Unload (Script script) {
			UnloadScript (script);
		}
	}
}

