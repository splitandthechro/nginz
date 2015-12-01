using System;
using MoonSharp.Interpreter;
using nginz.Common;

namespace nginz.Interop.Lua
{
	[MoonSharpUserData]
	public class Content
	{
		readonly ContentManager ContentManager;

		public Content (ContentManager contentManager) {
			ContentManager = contentManager;
		}

		public Texture2D loadTexture (string asset) {
			return new Texture2D (ContentManager.Load<global::nginz.Texture2D> (asset));
		}
	}
}

