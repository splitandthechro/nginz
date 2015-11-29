using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nginz.Common;

namespace nginz {
	public class ShaderProgramProvider : AssetProvider<ShaderProgram> {
		public ShaderProgramProvider (string root, ContentManager manager)
			: base (manager, root, "shaders") { }

		public override ShaderProgram Load (string assetName, params object[] args) {
			var vertexShader = Manager.LoadFrom<VertexShader> (assetName + ".vs");
			var fragmentShader = Manager.LoadFrom<FragmentShader> (assetName + ".fs");
			return new ShaderProgram (vertexShader, fragmentShader).Link ();
		}
	}
}
