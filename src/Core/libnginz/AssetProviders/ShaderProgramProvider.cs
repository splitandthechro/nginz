using System;
using System.Collections.Generic;
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

		public override ShaderProgram Load (string assetName, params object[] args) {
			var vertexShader = Manager.LoadFrom<VertexShader> (assetName + ".vs");
			var fragmentShader = Manager.LoadFrom<FragmentShader> (assetName + ".fs");
			return new ShaderProgram (vertexShader, fragmentShader).Link ();
		}
	}
}
