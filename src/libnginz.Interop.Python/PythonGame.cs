using System;
using nginz.Common;

namespace nginz.Interop.Python
{
	public class PythonGame : ICanLog
	{
		readonly PythonVM Python;
		readonly ContentManager Content;
		string scriptPath;
		dynamic instance;
		volatile bool exitLoop;

		PythonGame (string assetRoot) {
			var root = assetRoot == string.Empty
				? AppDomain.CurrentDomain.BaseDirectory
				: assetRoot;
			Python = new PythonVM ();
			Content = new ContentManager (root);
			Content.RegisterAssetProvider<PythonScript> (typeof(PythonScriptProvider));
			exitLoop = false;
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
			while (!exitLoop && RunOnce (className, conf)) { }
		}

		bool RunOnce (string className, GameConfiguration conf) {
			try {
				Python.Load (Content.LoadFrom<PythonScript> (scriptPath));
			} catch (Exception e) {
				this.Log (e.Message);
				this.Log ("Please fix that issue and press any key to restart the game.");
				Console.ReadKey (true);
				ClearScope ();
				Python.Shutdown ();
				return true;
			}
			if (!Python.Scope.ContainsVariable (className)) {
				this.Log ("Variable not found: {0}", className);
				this.Log ("Please fix that issue and press any key to restart the game.");
				Console.ReadKey (true);
				ClearScope ();
				Python.Shutdown ();
				return true;
			}
			var game = Python.Scope.GetVariable (className);
			try {
				instance = game (conf);
				instance.IsRunningInScriptedEnvironment = true;
			} catch (Exception e) {
				this.Log (e.Message);
				this.Log ("Please fix that issue and press any key to restart the game.");
				Console.ReadKey (true);
				ClearScope ();
				Python.Shutdown ();
				return true;
			}
			Python.CallInstance (instance, "Run");
			if (Python.GetLastError () != string.Empty) {
				this.Log (Python.GetLastError ());
				this.Log ("Please fix that issue and press any key to restart the game.");
				Console.ReadKey (true);
				ClearScope ();
				Python.Shutdown ();
				return true;
			}
			if (instance.HasCrashed) {
				this.Log ("============================================================");
				this.Log ("The game has crashed. Sadface.");
				this.Log ("Reason: {0}", (string) instance.ErrorMessage);
				this.Log ("Please fix your code and press any key to make magic happen.");
				this.Log ("============================================================");
				Console.ReadKey (true);
				return true;
			}
			return false;
		}

		void ClearScope () {
			foreach (var item in Python.Scope.GetItems ()) {
				Python.Scope.RemoveVariable (item.Key);
			}
		}
	}
}

