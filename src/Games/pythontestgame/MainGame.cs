using System;
using nginz;
using nginz.Scripting.Python;

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

			// Create the python vm
			python = new PythonVM (this);

			// Load the python script
			python.LoadLive (Content.Load<PythonScript> ("animation"));
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

