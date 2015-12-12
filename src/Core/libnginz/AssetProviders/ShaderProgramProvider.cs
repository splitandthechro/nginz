using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using nginz.Common;

namespace nginz
{
	
	[CLSCompliant (false)]
	public class ShaderProgramProvider : AssetHandler<ShaderProgram>
	{
		public ShaderProgramProvider (ContentManager manager)
			: base (manager, "shaders") { }

		public override ShaderProgram Load (string path, params object[] args) {
			var elem = path.Split ('\\');
			var asset = elem[elem.Length - 1];
			var directory = new DirectoryInfo (path);
			var files = directory.GetFiles (asset + ".*");

			var shaders = new List<Shader> ();

			foreach (var f in files)
				switch (f.Extension) {
					case ".gs":
						shaders.Add (Manager.LoadFrom<GeometryShader> (f.FullName));
						break;
					case ".vs":
						shaders.Add (Manager.LoadFrom<VertexShader> (f.FullName));
						break;
					case ".fs":
						shaders.Add (Manager.LoadFrom<FragmentShader> (f.FullName));
						break;
				}
			
			return new ShaderProgram (shaders.ToArray ()).Link ();
		}
	}
}
