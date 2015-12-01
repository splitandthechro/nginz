using System;
using nginz.Common;

namespace nginz
{
	public class ShaderProvider<Shader> : AssetProvider<Shader> where Shader: BasicShader
	{
		public ShaderProvider (ContentManager manager)
			: base (manager, "shaders") {
		}

		public override Shader Load (string assetName, params object[] args) {
			return BasicShader.FromFile<Shader> (assetName);
		}
	}
}
