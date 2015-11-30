using System;
using System.Threading;
using nginz.Common;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System.Reflection;

namespace nginz
{

	/// <summary>
	/// Game.
	/// </summary>
	public class Game : ICanLog, ICanThrow
	{

		/// <summary>
		/// The sync root.
		/// </summary>
		static object syncRoot;

		/// <summary>
		/// Initializes the <see cref="nginz.Game"/> class.
		/// </summary>
		static Game () {
			syncRoot = new object ();
		}

		/// <summary>
		/// The game configuration.
		/// </summary>
		readonly public GameConfiguration Configuration;

		/// <summary>
		/// The keyboard.
		/// </summary>
		readonly public KeyboardBuffer Keyboard;

		/// <summary>
		/// The content manager.
		/// </summary>
		public ContentManager Content;

		/// <summary>
		/// The mouse.
		/// </summary>
		public MouseBuffer Mouse;

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
		volatile NativeWindow window;

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
		/// Whether the game should exit.
		/// </summary>
		volatile bool exit;

		/// <summary>
		/// Whether the game is currently drawing.
		/// </summary>
		volatile bool updating;

		/// <summary>
		/// The resolution.
		/// </summary>
		public static Resolution Resolution;

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Game"/> class.
		/// </summary>
		/// <param name="conf">Conf.</param>
		public Game (GameConfiguration conf) {

			// Set the configuration
			Configuration = conf;

			// Initialize the keyboard buffer
			Keyboard = new KeyboardBuffer ();
		}

		/// <summary>
		/// Run the game.
		/// </summary>
		public void Run () {

			// Initialize the game
			InternalInitialize ();

			// Subscribe to the Resize event of the window
			// to correctly handle resizing of the window
			window.Resize += (sender, e) => Resize ();

			// Subscribe to the Closing event of the window
			// to dispose the context when closing the window.
			window.Closing += (sender, e) => {
				this.Log ("Window::Closing event fired");
				this.Log ("Requesting exit");
				Exit ();
				e.Cancel = true;
			};

			// Start gameloop in a separate thread
			var trd = new Thread (EnterGameloop);
			trd.Start ();

			// Process the message queue
			this.Log ("Entering message processing loop");
			while (!exit && window != null && window.Exists) {
				window.ProcessEvents ();
			}

			this.Log ("Waiting for updates to finish");
			while (updating) { }

			// Dispose of the context
			context.Dispose ();

			// Dispose of the window
			window.Dispose ();
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
		/// Exit the game.
		/// </summary>
		public void Exit () {

			// Call the BeforeExit function
			BeforeExit ();

			// Set exit to true
			exit = true;
		}

		/// <summary>
		/// Initialize this instance.
		/// </summary>
		protected virtual void Initialize () {
			GL.Enable (EnableCap.Blend);
			GL.BlendFunc (BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
		}

		/// <summary>
		/// Update logic like movement, physics etc.
		/// </summary>
		/// <param name="time">Time.</param>
		protected virtual void Update (GameTime time) {

			// Update the mouse buffer
			Mouse.Update ();
		}

		/// <summary>
		/// Draw stuff onto the screen.
		/// </summary>
		/// <param name="time">Time.</param>
		protected virtual void Draw (GameTime time) {
			
			// Present the rendered scene to the user
			if (!context.IsDisposed)
				context.SwapBuffers ();
		}

		/// <summary>
		/// Resize the game window.
		/// </summary>
		protected virtual void Resize () {

			// Update the resolution
			Resolution = new Resolution { Width = window.Width, Height = window.Height };

			// Update the context
			context.Update (window.WindowInfo);
		}

		/// <summary>
		/// Gets called before the game exits.
		/// Maybe save the game here and do other stuff
		/// that needs to be done before the game exits.
		/// </summary>
		protected virtual void BeforeExit () { }

		/// <summary>
		/// Initializes the game internally.
		/// </summary>
		void InternalInitialize () {
			
			// Initialize graphics mode to default
			graphicsMode = GraphicsMode.Default;

			// Start with default window flags
			var flags = GameWindowFlags.Default;

			// Set Fullscreen flag if requested
			if (Configuration.Fullscreen && !flags.HasFlag (GameWindowFlags.Fullscreen))
				flags |= GameWindowFlags.Fullscreen;

			// Set FixedWindow flag if requested
			if (Configuration.FixedWindow && !flags.HasFlag (GameWindowFlags.FixedWindow))
				flags |= GameWindowFlags.FixedWindow;

			// Create window
			this.Log ("Creating native window");
			window = new NativeWindow (
				width: Configuration.Width,
				height: Configuration.Height,
				title: Configuration.WindowTitle,
				options: flags,
				mode: GraphicsMode.Default,
				device: DisplayDevice.Default
			);

			Resolution = new Resolution { Width = window.Width, Height = window.Height };

			// Register events
			window.KeyDown += Keyboard.RegisterKeyDown;
			window.KeyUp += Keyboard.RegisterKeyUp;
			window.KeyPress += delegate { };

			// Initialize startTime and lastTime
			startTime = DateTime.UtcNow;
			lastTime = startTime;

			// Initialize the mouse buffer
			Mouse = new MouseBuffer (window);

			// Initialize the content manager
			var contentRoot = AppDomain.CurrentDomain.BaseDirectory;
			Content = new ContentManager (contentRoot);
			RegisterProviders ();
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
				minor: 0,
				flags: GraphicsContextFlags.ForwardCompatible
			);

			// Make the created context the current context
			context.MakeCurrent (window.WindowInfo);

			// Throw if context is not available
			GraphicsContext.Assert ();

			// Set vsync mode
			this.Log ("Setting VSync mode: {0}", Configuration.Vsync);
			switch (Configuration.Vsync) {
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
			var framerate = Configuration.TargetFramerate > 0 ? Configuration.TargetFramerate : 60;

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

			// Present the window to the user
			window.Visible = true;

			// Enter the actual game loop
			while (!exit) {

				// Set updating to true
				updating = true;

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
				if (Configuration.FixedFramerate) {

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

				// Set updating to false
				updating = false;
			}
		}

		/// <summary>
		/// Registers the basic built-in asset providers.
		/// </summary>
		void RegisterProviders () {

			// Register an asset provider for the vertex shader.
			Content.RegisterAssetProvider<VertexShader> (typeof (ShaderProvider<VertexShader>));

			// Register an asset provider for the vertex fragment.
			Content.RegisterAssetProvider<FragmentShader> (typeof (ShaderProvider<FragmentShader>));

			// Register an asset provider for the geometry shader.
			Content.RegisterAssetProvider<GeometryShader> (typeof (ShaderProvider<GeometryShader>));

			// Register an asset provider for the Texture2D.
			Content.RegisterAssetProvider<Texture2D> (typeof (Texture2DProvider));

			// Register an asset provider for the obj loader.
			Content.RegisterAssetProvider<ObjFile> (typeof (ObjProvider));

			// Register an asset provider for the shader program.
			Content.RegisterAssetProvider<ShaderProgram> (typeof (ShaderProgramProvider));
		}
	}
}

