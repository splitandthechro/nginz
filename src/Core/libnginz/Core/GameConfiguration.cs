using System;

namespace nginz
{
	/// <summary>
	/// Game configuration.
	/// </summary>
	public struct GameConfiguration
	{
		/// <summary>
		/// Default game configuration.
		/// </summary>
		readonly public static GameConfiguration Default;

		/// <summary>
		/// Initializes the <see cref="nginz.GameConfiguration"/> struct.
		/// </summary>
		static GameConfiguration () {
			Default = new GameConfiguration {
				Width = 640,
				Height = 480,
				TargetFramerate = 60,
				Fullscreen = false,
				FixedWindow = true,
				FixedFramerate = true,
				WindowTitle = "nginZ Engine",
				Vsync = VsyncMode.Adaptive,
				ContentRoot = "Assets",
			};
		}

		/// <summary>
		/// Width of the window.
		/// </summary>
		public int Width;

		/// <summary>
		/// Height of the window.
		/// </summary>
		public int Height;

		/// <summary>
		/// Target framerate.
		/// Only if FixedFramerate is set to true.
		/// </summary>
		public int TargetFramerate;

		/// <summary>
		/// Whether the game should start in fullscreen mode.
		/// </summary>
		public bool Fullscreen;

		/// <summary>
		/// Whether the window should be resizable.
		/// </summary>
		public bool FixedWindow;

		/// <summary>
		/// Whether the framerate should be fixed or variable.
		/// </summary>
		public bool FixedFramerate;

		/// <summary>
		/// The Vsync mode.
		/// </summary>
		public VsyncMode Vsync;

		/// <summary>
		/// The window title.
		/// </summary>
		public string WindowTitle;

		/// <summary>
		/// The path to the base assets directory.
		/// </summary>
		public string ContentRoot;
	}
}

