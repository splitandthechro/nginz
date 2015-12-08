using System;
using System.IO;
using System.Reflection;
using Iodine;
using Iodine.Engine;
using Iodine.Runtime;
using nginz.Common;

namespace nginz.Interop.Iodine
{
	public class IodineVM : ICanLog, ICanThrow
	{
		readonly public IodineEngine Engine;
		readonly public ScriptReloader Reloader;

		readonly Game Game;

		dynamic Scope;

		public dynamic this [string name] {
			get { return CallDynamic (name); }
		}

		public IodineVM (Game game = null) {
			Game = game;
			Engine = new IodineEngine ();
			Reloader = new ScriptReloader ("*.id");
			Reloader.LoadScript = Load;
			if (Game != null) {
				Reloader.PauseGame = Game.Pause;
				Reloader.ResumeGame = Game.Resume;
				Game.Content.RegisterAssetProvider<IodineScript> (typeof(IodineScriptProvider));
			}
			SetupGlobals ();
		}

		public void Load (Script script) {
			if (!script.HasValidPath) {
				this.Log ("Cannot load script: Valid path information needed");
				this.Log ("Try loading the script via the content manager");
				return;
			}
			try {
				Scope = Engine.DoFile (script.FilePath);
			} catch (Exception e) {
				this.Log (e.Message);
			}
		}

		public IodineVM LoadLive (Script script) {
			Load (script);
			if (script.HasValidPath) {
				this.Log ("Observing {0}", Path.GetFileName (script.FilePath));
				Reloader.WatchFile (script);
			} else if (!script.HasValidPath)
				this.Log ("Live reload not possible: No path information available");
			return this;
		}

		public dynamic CallDynamic (string name) {

			// TODO: Implement this.
			return null;
		}

		public object Call (string name, params object[] args) {

			// TODO: Implement this.
			return null;
		}

		void SetupGlobals () {
			RegisterAssembly ("OpenTK.dll");
			RegisterAssembly ("libnginz.dll");
			RegisterAssembly ("libnginz.Common.dll");
		}

		void RegisterAssembly (string path) {
			var assembly = Assembly.Load (path);
			Engine.RegisterAssembly (assembly);
		}
	}
}

