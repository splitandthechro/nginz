using System;
using System.IO;
using System.Reflection;
using Iodine.Engine;
using Iodine.Runtime;
using nginz.Common;

namespace nginz.Scripting.Iodine
{
	public class IodineVM : ICanLog, ICanThrow
	{
		readonly public IodineEngine Engine;
		readonly public ScriptReloader Reloader;

		readonly Game Game;

		dynamic Scope;
		string currentError;

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
			currentError = string.Empty;
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
			return Engine.Get (name);
		}

		public object Call (string name, params object[] args) {
			LogExtensions.IsRunningInScriptedEnvironment = true;
			try {
				var result = Engine.Call (name, args);
				var castedResult = (object) result;
				currentError = string.Empty;
				return castedResult;
			} catch (UnhandledIodineExceptionException e) {
				var message = e.OriginalException.GetAttribute ("message");
				if (currentError != message.ToString ())
					this.Log (message.ToString ());
				currentError = message.ToString ();
			} catch (Exception e) {
				if (currentError != e.Message)
					this.Log (e.Message);
				currentError = e.Message;
			}
			LogExtensions.IsRunningInScriptedEnvironment = false;
			return null;
		}

		public T Call<T> (string name, params object[] args) {
			return (T) Call (name, args);
		}

		public string GetLastError () {
			return currentError;
		}

		void SetupGlobals () {
			RegisterAssembly ("OpenTK.dll");
			RegisterAssembly ("libnginz.dll");
			RegisterAssembly ("libnginz.Common.dll");
			RegisterAssembly ("libnginz.Compatibility.Iodine.dll");
		}

		void RegisterAssembly (string path) {
			var assembly = Assembly.LoadFrom (path);
			Engine.RegisterAssembly (assembly);
		}
	}
}

