using System;
using System.Threading;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace splitandthechro.nginz
{
    public class Game
    {
		/// <summary>
		/// The game configuration.
		/// </summary>
		readonly GameConfiguration conf;

		/// <summary>
		/// The start time.
		/// </summary>
		DateTime startTime;

		/// <summary>
		/// DateTime of the last update.
		/// </summary>
		DateTime lastTime;

		/// <summary>
		/// The graphics mode.
		/// </summary>
		GraphicsMode graphicsMode;

		/// <summary>
		/// Game window and also the drawing surface.
		/// </summary>
		NativeWindow window;

		/// <summary>
		/// Graphics context.
		/// </summary>
		GraphicsContext context;

		/// <summary>
		/// Initializes a new instance of the <see cref="splitandthechro.nginz.Game"/> class.
		/// </summary>
		/// <param name="conf">Conf.</param>
		public Game (GameConfiguration conf) {
			this.conf = conf;
		}

		/// <summary>
		/// Initialize this instance.
		/// </summary>
		protected virtual void Initialize () {

			// Initialize graphics mode to default
			graphicsMode = GraphicsMode.Default;

			// Start with default window flags
			var flags = GameWindowFlags.Default;

			// Set Fullscreen flag if requested
			if (conf.Fullscreen && !flags.HasFlag (GameWindowFlags.Fullscreen))
				flags |= GameWindowFlags.Fullscreen;
			
			// Set FixedWindow flag if requested
			if (conf.FixedWindow && !flags.HasFlag (GameWindowFlags.FixedWindow))
				flags |= GameWindowFlags.FixedWindow;

			// Create window
			window = new NativeWindow (
				width: conf.Width,
				height: conf.Height,
				title: conf.WindowTitle,
				options: flags,
				mode: GraphicsMode.Default,
				device: DisplayDevice.Default
			);

			// Initialize startTime and lastTime
			startTime = DateTime.UtcNow;
			lastTime = startTime;
		}

		/// <summary>
		/// Update logic like movement, physics etc.
		/// </summary>
		/// <param name="time">Time.</param>
		protected virtual void Update (GameTime time) {
		}

		/// <summary>
		/// Draw stuff onto the screen.
		/// </summary>
		/// <param name="time">Time.</param>
		protected virtual void Draw (GameTime time) {
			context.SwapBuffers ();
		}

		public void Run () {
			Initialize ();
			var trd = new Thread (EnterGameloop);
			trd.Start ();
			window.Visible = true;
			while (window.Exists) {
				window.ProcessEvents ();
			}
		}

		void EnterGameloop () {

			// Create graphics context
			context = new GraphicsContext (
				mode: graphicsMode,
				window: window.WindowInfo,
				major: 4,
				minor: 5,
				flags: GraphicsContextFlags.ForwardCompatible
			);

			// Make the created context the current context
			context.MakeCurrent (window.WindowInfo);

			// Load OpenGL entry points
			context.LoadAll ();

			// Prepare timing variables
			TimeSpan totalTime;
			TimeSpan elapsedTime;
			double updateNewTime;
			double updateFrameTime;
			var now = DateTime.UtcNow;
			var gameTime = GameTime.ZeroTime;
			var updateTime = 0d;
			var updateAccumTime = 0d;
			var updateDeltaTime = 1d / 60d; // hardcode 60hz for now
			var updateCurrentTime = now.Subtract (startTime).TotalSeconds;

			// Enter the actual game loop
			while (true) {

				// Calculate timing data
				now = DateTime.UtcNow;
				updateNewTime = now.Subtract (startTime).TotalSeconds;
				updateFrameTime = updateNewTime - updateCurrentTime;
				updateAccumTime += updateFrameTime;

				// Update according to calculated timing data
				while (updateAccumTime >= updateDeltaTime) {

					// Calculate total and elapsed time
					totalTime = now.Subtract (startTime);
					elapsedTime = now.Subtract (lastTime);
					lastTime = now;

					// Create GameTime from calculated time values
					gameTime = new GameTime (
						total: totalTime,
						elapsed: elapsedTime
					);

					// Update
					Update (gameTime);

					// Update timing data
					updateAccumTime -= updateDeltaTime;
					updateTime += updateDeltaTime;
				}

				// Draw
				Draw (gameTime);
			}
		}
    }
}

