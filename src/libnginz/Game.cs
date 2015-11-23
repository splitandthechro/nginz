﻿using System;
using System.Threading;
using nginz.Common;
using OpenTK;
using OpenTK.Graphics;

namespace nginz
{
	public class Game : ICanLog, ICanThrow
	{
		/// <summary>
		/// The game configuration.
		/// </summary>
		readonly public GameConfiguration conf;

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
		/// Whether the game should pause.
		/// </summary>
		volatile bool pause;

		/// <summary>
		/// Whether the game is paused.
		/// </summary>
		volatile bool paused;

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Game"/> class.
		/// </summary>
		/// <param name="conf">Conf.</param>
		public Game (GameConfiguration conf) {
			this.conf = conf;
		}

		/// <summary>
		/// Pause the game.
		/// </summary>
		public void Pause () {
			pause = true;
			while (!paused) { }
		}

		/// <summary>
		/// Resume the game.
		/// </summary>
		public void Resume () {
			pause = false;
		}

		/// <summary>
		/// Initializes the game internally.
		/// </summary>
		void InternalInitialize () {
			
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
			this.Log ("Creating native window");
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
		/// Initialize this instance.
		/// </summary>
		protected virtual void Initialize () {
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

			// Present the rendered scene to the user
			context.SwapBuffers ();
		}

		public virtual void Resize (Resolution resolution) {
			context.Update (window.WindowInfo);
		}

		/// <summary>
		/// Run the game.
		/// </summary>
		public void Run () {

			// Initialize the game
			InternalInitialize ();
			
			// Start gameloop in a separate thread
			var trd = new Thread (EnterGameloop);
			trd.Start ();

			// Subscribe to the Resize event of the window
			// to correctly handle resizing of the window
			window.Resize += (sender, e) => Resize (new Resolution { Width = window.Width, Height = window.Height });

			// Subscribe to the Closing event of the window
			// to dispose the context when closing the window.
			window.Closing += (sender, e) => context.Dispose ();

			// Present the window to the user
			window.Visible = true;

			// Process the message queue
			this.Log ("Entering message processing loop");
			while (window.Exists) {
				window.ProcessEvents ();
			}
		}

		/// <summary>
		/// Enter the gameloop.
		/// </summary>
		void EnterGameloop () {

			// Create graphics context
			this.Log ("Creating graphics context");
			context = new GraphicsContext (
				mode: graphicsMode,
				window: window.WindowInfo,
				major: 4,
				minor: 5,
				flags: GraphicsContextFlags.ForwardCompatible
			);

			// Make the created context the current context
			context.MakeCurrent (window.WindowInfo);

			// Throw if context is not available
			GraphicsContext.Assert ();

			// Set vsync mode
			this.Log ("Setting VSync mode: {0}", conf.Vsync);
			switch (conf.Vsync) {
			case VsyncMode.Adaptive:
				context.SwapInterval = -1;
				break;
			case VsyncMode.Off:
				context.SwapInterval = 0;
				break;
			case VsyncMode.On:
				context.SwapInterval = 1;
				break;
			}

			// Load OpenGL entry points
			this.Log ("Loading OpenGL entry points");
			context.LoadAll ();

			// Initialize the game
			this.Log ("Initializing game");
			Initialize ();

			// Set target framerate
			// Use 60hz if framerate is not set
			var framerate = conf.TargetFramerate > 0 ? conf.TargetFramerate : 60;

			// Prepare timing variables
			TimeSpan totalTime;
			TimeSpan elapsedTime;
			double updateNewTime;
			double updateFrameTime;
			var now = DateTime.UtcNow;
			var gameTime = GameTime.ZeroTime;
			var updateTime = 0d;
			var updateAccumTime = 0d;
			var updateDeltaTime = 1d / (double) framerate;
			var updateCurrentTime = now.Subtract (startTime).TotalSeconds;

			// Enter the actual game loop
			while (true) {

				// Set the paused variable to true
				// if the game should be paused and continue
				if (pause) {
					paused = true;
					continue;
				}

				// Set the paused variable to false
				paused = false;

				// Break out of the loop if the context is not available.
				if (context.IsDisposed) {
					this.Log ("Context not available");
					this.Log ("Leaving gameloop");
					break;
				}

				// Update current time
				now = DateTime.UtcNow;

				// Use fixed framerate if requested
				if (conf.FixedFramerate) {

					// Calculate timing data
					updateNewTime = now.Subtract (startTime).TotalSeconds;
					updateFrameTime = updateNewTime - updateCurrentTime;
					updateCurrentTime = updateNewTime;
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
				}

				// Use variable framerate
				else {

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
				}

				// Draw
				Draw (gameTime);
			}
		}
	}
}

