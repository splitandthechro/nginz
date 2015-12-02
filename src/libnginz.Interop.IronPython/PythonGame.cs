using System;
using nginz.Common;

namespace nginz.Interop.IronPython
{
	public class PythonGame : ICanLog
	{
		readonly PythonVM Python;
		readonly ScriptReloader Reloader;
		readonly ContentManager Content;

		PythonGame (string assetRoot) {
			var root = assetRoot == string.Empty
				? AppDomain.CurrentDomain.BaseDirectory
				: assetRoot;
			Python = new PythonVM ();
			Reloader = new ScriptReloader ("*.py");
			Reloader.LoadScript = new Action<Script> (script => Load (script.FilePath));
			Content = new ContentManager (root);
			Content.RegisterAssetProvider<PythonScript> (typeof(PythonScriptProvider));
		}

		public static PythonGame Create (string assetRoot = "") {
			return new PythonGame (assetRoot);
		}

		public PythonGame Load (string asset) {
			Python.Load (Content.Load<PythonScript> (asset));
			return this;
		}

		public PythonGame Run (string className, GameConfiguration conf) {
			if (!Python.Scope.ContainsVariable (className)) {
				this.Log ("Class not found: {0}", className);
				return this;
			}
			var game = Python.Scope.GetVariable (className);
			try {
				var instance = game (conf);
				Reloader.PauseGame = Python.Engine.Operations.GetMember<Action> (instance, "Pause");
				Reloader.ResumeGame = Python.Engine.Operations.GetMember<Action> (instance, "Resume");
				instance.Run ();
			} catch (Exception e) {
				this.Log (e.Message);
			}
			return this;
		}
	}
}

