using System;
using nginz;
using nginz.Common;
using nginz.Interop.Lua;

namespace luatestgame
{
	public class MainGame : Game
	{
		LuaVM lua;

		public MainGame (GameConfiguration conf)
			: base (conf) { }

		protected override void Initialize () {

			// Set asset directory
			Content.ContentRoot = "../../assets";

			// Create the lua vm
			lua = new LuaVM (this);

			// Subscribe to events
			lua.LoadScript += script => lua.Call ("load");
			lua.UnloadScript += script => lua.Call ("unload");

			// Load the lua script
			lua.Load (Content.Load<Script> ("animation"), true);

			base.Initialize ();
		}

		protected override void Update (GameTime time) {
			lua.Call ("update");
			base.Update (time);
		}

		protected override void Draw (GameTime time) {
			lua.Call ("draw");
			base.Draw (time);
		}
	}
}

