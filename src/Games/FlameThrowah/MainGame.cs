using System;
using nginz;
using nginz.Scripting.Python;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Graphics;

namespace FlameThrowah
{
	public class MainGame : Game
	{
		TextureLookup Textures;
		PythonVM Python;

		public MainGame (GameConfiguration conf)
			: base (conf) { }

		protected override void Initialize () {

			// Register map handler for saving and loading maps
			Content.RegisterAssetHandler<Map> (typeof(MapHandler));

			// Create texture lookup
			Textures = new TextureLookup ();
			LoadTextures ();
			SerializeTestMap ();

			// Initialize python vm
			Python = new PythonVM (this);
			var original = Python.Reloader.LoadScript;
			Python.Reloader.LoadScript = new Action<nginz.Common.Script> (script => {
				original (script);
				Python.Call ("loadcontent", this, Content);
			});
			LoadScripts ();
			Python.Call ("initialize", this);

			base.Initialize ();
		}

		void LoadTextures () {
			Textures.Add ("nginz_logo", Content.Load<Texture2D> ("ft/nginz.png"));
		}

		void LoadScripts () {
			Python.LoadLive (Content.Load<PythonScript> ("ft/game"));
		}

		void SerializeTestMap () {
			var testMap = new Map ("ft_test", Textures);
			testMap.Add ("nginz_logo", new OpenTK.Vector2 (0, 0));
			testMap.Add ("nginz_logo", new OpenTK.Vector2 (1, 1));
			Content.Save<Map> (testMap, testMap.Name);
		}

		protected override void Update (GameTime time) {
			Python.Call ("update", this, time);
			base.Update (time);
		}

		protected override void Draw (GameTime time) {
			Python.Call ("draw", this, time);
			SpriteBatch.Begin ();
			Python.Call ("draw2d", this, time, SpriteBatch);
			SpriteBatch.End ();
			base.Draw (time);
		}
	}
}

