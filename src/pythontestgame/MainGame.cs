using System;
using nginz;
using nginz.Interop.IronPython;

namespace pythontestgame
{
	public class MainGame : Game
	{
		PythonVM python;

		public MainGame (GameConfiguration conf)
			: base (conf) { }
		
		protected override void Initialize () {

			// Set asset directory
			Content.ContentRoot = "../../assets";

			// Register python script asset provider
			Content.RegisterAssetProvider<PythonScript> (typeof (PythonScriptProvider));

			// Create the python vm
			python = new PythonVM (this);

			// Load the python script
			python.Load (Content.Load<PythonScript> ("animation"));
			python ["init"] ();

			base.Initialize ();
		}

		protected override void Update (GameTime time) {
			python ["update"] ();
			base.Update (time);
		}

		protected override void Draw (GameTime time) {
			python ["draw"] ();
			base.Draw (time);
		}
	}
}

