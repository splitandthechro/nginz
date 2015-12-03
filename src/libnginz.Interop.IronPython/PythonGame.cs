using System;
using nginz.Common;

namespace nginz.Interop.IronPython
{
	public class PythonGame : ICanLog
	{
		readonly PythonVM Python;
		readonly ContentManager Content;
		string scriptPath;
		dynamic instance;
		volatile bool continueLoop;
		volatile bool exitLoop;
		volatile bool hasExited;

		PythonGame (string assetRoot) {
			var root = assetRoot == string.Empty
				? AppDomain.CurrentDomain.BaseDirectory
				: assetRoot;
			Python = new PythonVM ();
			Content = new ContentManager (root);
			Content.RegisterAssetProvider<PythonScript> (typeof(PythonScriptProvider));
			continueLoop = true;
			exitLoop = false;
			hasExited = false;
		}

		public static PythonGame Create (string assetRoot = "") {
			return new PythonGame (assetRoot);
		}

		public PythonGame Load (string asset) {
			scriptPath = Content.Load<PythonScript> (asset).FilePath;
			return this;
		}

		public PythonGame Run (string className, GameConfiguration conf) {
			RunLoop (className, conf);
			return this;
		}

		public void RunLoop (string className, GameConfiguration conf) {
			hasExited = false;
			while (continueLoop && !exitLoop) {
				RunOnce (className, conf);
			}
			hasExited = true;
		}

		void RunOnce (string className, GameConfiguration conf) {
			try {
				Python.Load (Content.LoadFrom<PythonScript> (scriptPath));
			} catch (Exception e) {
				this.Log (e.Message);
				this.Log ("Please fix that issue and press any key to restart the game.");
				Console.ReadKey (true);
				ClearScope ();
				Python.Shutdown ();
				continueLoop = true;
			}
			if (!Python.Scope.ContainsVariable (className)) {
				this.Log ("Variable not found: {0}", className);
				this.Log ("Please fix that issue and press any key to restart the game.");
				Console.ReadKey (true);
				ClearScope ();
				Python.Shutdown ();
				continueLoop = true;
			}
			var game = Python.Scope.GetVariable (className);
			try {
				instance = game (conf);
			} catch (Exception e) {
				this.Log (e.Message);
				this.Log ("Please fix that issue and press any key to restart the game.");
				Console.ReadKey (true);
				ClearScope ();
				Python.Shutdown ();
				continueLoop = true;
			}
			Python.CallInstance (instance, "Run");
			if (Python.GetLastError () != string.Empty) {
				this.Log (Python.GetLastError ());
				this.Log ("Please fix that issue and press any key to restart the game.");
				Console.ReadKey (true);
				ClearScope ();
				Python.Shutdown ();
				continueLoop = true;
			}
			continueLoop = false;
		}

		void ClearScope () {
			foreach (var item in Python.Scope.GetItems ()) {
				Python.Scope.RemoveVariable (item.Key);
			}
		}
	}
}

