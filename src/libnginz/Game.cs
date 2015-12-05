using System;
using System.Collections.Concurrent;
using System.Threading;
using nginz.Common;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;

namespace nginz
{

	/// <summary>
	/// Game.
	/// </summary>
	public class Game : ICanLog, ICanThrow, IDisposable
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
			InteropTools.DetectArchitecture ();
		}

		/// <summary>
		/// The game configuration.
		/// </summary>
		readonly public GameConfiguration Configuration;

		/// <summary>
		/// The user interface controller.
		/// </summary>
		readonly public UIController UI;

		/// <summary>
		/// The keyboard.
		/// </summary>
		readonly public KeyboardBuffer Keyboard;

		/// <summary>
		/// Queue of actions that should be invoked in the context thread.
		/// </summary>
		readonly public ConcurrentQueue<Action> ContextActions;

		/// <summary>
		/// Queue of actions that should be invoked in the UI thread.
		/// </summary>
		readonly public ConcurrentQueue<Action> UIActions;

		/// <summary>
		/// The sprite batch.
		/// </summary>
		public SpriteBatch SpriteBatch;

		/// <summary>
		/// The content manager.
		/// </summary>
		[CLSCompliant (false)]
		public ContentManager Content;

		/// <summary>
		/// The mouse.
		/// </summary>
		public MouseBuffer Mouse;

		public bool IsRunningInScriptedEnvironment;
		public bool HasCrashed;
		public string ErrorMessage;

		// No XML doc for those since they are not
		// used anywhere except in the gameloop.
		#region Timing values
		GameTime gameTime;
		DateTime startTime;
		DateTime lastTime;
		DateTime currentTime;
		TimeSpan totalTime;
		TimeSpan elapsedTime;
		double updateTime;
		double updateNewTime;
		double updateFrameTime;
		double updateAccumTime;
		double updateDeltaTime;
		double updateCurrentTime;
		#endregion

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
		/// Whether the game has exited.
		/// </summary>
		volatile bool exited;

		/// <summary>
		/// Whether the game is currently drawing.
		/// </summary>
		volatile bool updating;

		volatile bool windowVisible;

		/// <summary>
		/// The resolution.
		/// </summary>
		public static Resolution Resolution;

		/// <summary>
		/// The content root.
		/// </summary>
		public string ContentRoot;

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Game"/> class.
		/// </summary>
		/// <param name="conf">Conf.</param>
		public Game (GameConfiguration conf) {

			// Set the configuration
			Configuration = conf;

			// Initialize the keyboard buffer
			Keyboard = new KeyboardBuffer ();

			// Initialize the actions
			ContextActions = new ConcurrentQueue<Action> ();
			UIActions = new ConcurrentQueue<Action> ();

			// Initialize scripting enviroment
			IsRunningInScriptedEnvironment = false;

			// Set UI controller
			UI = UIController.Instance;
			UI.Bind (this);
		}

		/// <summary>
		/// Resize the window.
		/// </summary>
		/// <param name="newWidth">New width.</param>
		/// <param name="newHeight">New height.</param>
		public void Resize (int newWidth, int newHeight) {
			window.Width = newWidth;
			window.Height = newHeight;
			UpdateResolution ();
		}

		/// <summary>
		/// Run the game.
		/// </summary>
		public void Run () {

			// Initialize the game
			InternalInitialize ();

			// Subscribe to the Resize event of the window
			// to correctly handle resizing of the window
			window.Resize += (sender, e) => OnResize ();

			// Subscribe to the Closing event of the window
			// to dispose the context when closing the window.
			window.Closing += (sender, e) => {
				this.Log ("Window::Closing event fired");
				this.Log ("Requesting exit");
				Exit ();
				e.Cancel = true;
			};

			// Start gameloop in a separate thread
			if (!IsRunningInScriptedEnvironment) {
				var trd = new Thread (EnterGameloop);
				trd.Start ();
			} else {
				var trd = new Thread (() => {
					try {
						EnterGameloop ();
					} catch (Exception e) {
						Exit ();
						while (!exited) { }
						HasCrashed = true;
						ErrorMessage = e.Message;
					}
				});
				trd.Start ();
			}

			// Process the message queue
			this.Log ("Entering message processing loop");
			while (!exit && window != null && window.Exists) {
				
				// Invoke waiting actions
				for (var i = 0; i < UIActions.Count; i++) {
					Action action;
					if (UIActions.TryDequeue (out action))
						action ();
				}

				// Center the mouse
				Mouse.CenterMouse ();

				// Process window events
				window.ProcessEvents ();

				if (!windowVisible)
					windowVisible = true;
			}

			// Wait till all updates finished
			this.Log ("Waiting for updates to finish");
			while (updating) { }
			exited = true;
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
		/// Ensures that the specified action is called in the context thread.
		/// </summary>
		/// <param name="act">The action.</param>
		public void EnsureContextThread (Action act) {
			ContextActions.Enqueue (act);
		}

		/// <summary>
		/// Ensures that the specified action is called in the UI thread.
		/// </summary>
		/// <param name="act">The action.</param>
		public void EnsureUIThread (Action act) {
			UIActions.Enqueue (act);
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
		protected virtual void OnResize () {

			// Update the resolution
			UpdateResolution ();

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
		/// Update the resolution.
		/// </summary>
		void UpdateResolution () {
			Resolution = new Resolution { Width = window.Width, Height = window.Height };
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

			// Update the resolution
			UpdateResolution ();

			// Register events
			window.KeyDown += Keyboard.RegisterKeyDown;
			window.KeyUp += Keyboard.RegisterKeyUp;
			window.KeyPress += delegate { };

			// Initialize startTime and lastTime
			startTime = DateTime.UtcNow;
			lastTime = startTime;

			// Initialize the mouse buffer
			Mouse = new MouseBuffer (window, this);

			// Initialize the context manager
			Content = new ContentManager (ContentRoot ?? AppDomain.CurrentDomain.BaseDirectory);
			RegisterProviders ();
		}

		/// <summary>
		/// Setup up vsync.
		/// </summary>
		void SetupVsync () {
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
		}

		void PrepareTiming (int targetFramerate) {
			currentTime = DateTime.UtcNow;
			gameTime = GameTime.ZeroTime;
			updateTime = 0d;
			updateAccumTime = 0d;
			updateDeltaTime = 1d / (double) targetFramerate;
			updateCurrentTime = currentTime.Subtract (startTime).TotalSeconds;
		}

		void InternalUpdate (DateTime now) {
			
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

					// Not sure if this does anything. Testing needed
					now = DateTime.UtcNow;

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
			SetupVsync ();

			// Load OpenGL entry points
			this.Log ("Loading OpenGL entry points");
			context.LoadAll ();

			// Initialize the sprite batch
			SpriteBatch = new SpriteBatch ();

			// Initialize the game
			this.Log ("Initializing game");
			Initialize ();

			// Set target framerate
			// Use 60hz if framerate is not set
			var framerate = Configuration.TargetFramerate > 0 ? Configuration.TargetFramerate : 60;

			// Prepare timing variables
			PrepareTiming (framerate);

			// Present the window to the user
			window.Visible = true;

			// Wait till the window is visible
			while (!windowVisible && !window.Focused && !window.Visible) { }

			// Enter the actual game loop
			while (!exit) {

				// Set updating to true
				updating = true;

				// Invoke waiting actions
				for (var i = 0; i < ContextActions.Count; i++) {
					Action action;
					if (ContextActions.TryDequeue (out action))
						action ();
				}

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
				currentTime = DateTime.UtcNow;

				// Update
				InternalUpdate (currentTime);

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

			// Register an asset provider for the vertex shaders.
			Content.RegisterAssetProvider<VertexShader> (typeof (ShaderProvider<VertexShader>));

			// Register an asset provider for fragment shaders.
			Content.RegisterAssetProvider<FragmentShader> (typeof (ShaderProvider<FragmentShader>));

			// Register an asset provider for geometry shaders.
			Content.RegisterAssetProvider<GeometryShader> (typeof (ShaderProvider<GeometryShader>));

			// Register an asset provider for 2D textures.
			Content.RegisterAssetProvider<Texture2D> (typeof (Texture2DProvider));

			// Register an asset provider for the object files.
			Content.RegisterAssetProvider<ObjFile> (typeof (ObjProvider));

			// Register an asset provider for shader programs.
			Content.RegisterAssetProvider<ShaderProgram> (typeof (ShaderProgramProvider));

			// Register an asset provider for scripts.
			Content.RegisterAssetProvider<Script> (typeof(ScriptProvider));
		}

		#region IDisposable implementation

		/// <summary>
		/// Releases all resource used by the <see cref="nginz.Game"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="nginz.Game"/>. The <see cref="Dispose"/>
		/// method leaves the <see cref="nginz.Game"/> in an unusable state. After calling <see cref="Dispose"/>, you must
		/// release all references to the <see cref="nginz.Game"/> so the garbage collector can reclaim the memory that the
		/// <see cref="nginz.Game"/> was occupying.</remarks>
		public void Dispose () {

			// Exit the game
			if (!exit)
				Exit ();

			// Wait till the game stops updating
			while (updating) { }

			// Dispose the window
			window.Dispose ();

			// Dispose the context
			context.Dispose ();
		}

		#endregion
	}
}

