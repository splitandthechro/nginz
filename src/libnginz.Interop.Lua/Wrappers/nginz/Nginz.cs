using System;
using MoonSharp.Interpreter;

namespace nginz.Interop.Lua
{
	[MoonSharpUserData]
	public class Nginz
	{
		readonly Game Game;

		public Nginz (Game game) {
			Game = game;
		}

		public int width () {
			return Game.Resolution.Width;
		}

		public int height () {
			return Game.Resolution.Height;
		}

		public float aspectRatio () {
			return Game.Resolution.AspectRatio;
		}

		public void invoke (ScriptFunctionDelegate func) {
			Game.EnsureContextThread (() => func ());
		}
	}
}

