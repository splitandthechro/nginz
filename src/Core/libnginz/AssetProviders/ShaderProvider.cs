using System;
using nginz.Common;

namespace nginz
{
	[CLSCompliant (false)]
	public class ShaderProvider<TShader> : AssetHandler<TShader> where TShader: BasicShader
	{
		public ShaderProvider (ContentManager manager)
			: base (manager, "shaders") {
		}

		public override TShader Load (string assetName, params object[] args) {
			return BasicShader.FromFile<TShader> (assetName);
		}
	}
}
