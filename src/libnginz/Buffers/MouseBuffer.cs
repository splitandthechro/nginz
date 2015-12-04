using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using nginz.Common;
using OpenTK;
using OpenTK.Input;

namespace nginz
{

	/// <summary>
	/// Cursor buffer.
	/// </summary>
	public class MouseBuffer : ICanLog
	{

		/// <summary>
		/// The window.
		/// </summary>
		readonly NativeWindow window;

		/// <summary>
		/// The game.
		/// </summary>
		readonly Game game;

		/// <summary>
		/// The mouse state.
		/// </summary>
		MouseState State;

		/// <summary>
		/// Whether updating the mouse should be suppressed.
		/// </summary>
		bool suppressUpdate;

		/// <summary>
		/// Whether the window was unfocused.
		/// </summary>
		bool wasUnfocused;

		/// <summary>
		/// The x position.
		/// </summary>
		public float X;

		/// <summary>
		/// The y position.
		/// </summary>
		public float Y;

		/// <summary>
		/// The wheel value.
		/// </summary>
		public float Wheel;

		/// <summary>
		/// The delta-x value.
		/// </summary>
		public float DeltaX;

		/// <summary>
		/// The delta-y value.
		/// </summary>
		public float DeltaY;

		/// <summary>
		/// The delta-z (wheel) value.
		/// </summary>
		public float DeltaZ;

		/// <summary>
		/// Gets the left button.
		/// </summary>
		/// <value>The left button.</value>
		public ButtonState LeftButton {
			get { return State.LeftButton; }
		}

		/// <summary>
		/// Gets the middle button.
		/// </summary>
		/// <value>The middle button.</value>
		public ButtonState MiddleButton {
			get { return State.MiddleButton; }
		}

		/// <summary>
		/// Gets the right button.
		/// </summary>
		/// <value>The right button.</value>
		public ButtonState RightButton {
			get { return State.RightButton; }
		}

		/// <summary>
		/// Should mouse be centered each update.
		/// </summary>
		/// <value>Center state.</value>
		public bool ShouldCenterMouse { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the cursor is visible.
		/// </summary>
		/// <value><c>true</c> if the cursor is visible; otherwise, <c>false</c>.</value>
		public bool CursorVisible {
			get {
				bool visible = false;
				if (game != null)
					game.EnsureContextThread (() => visible = window.CursorVisible);
				else
					visible = window.CursorVisible;
				return visible;
			}
			set {
				if (game != null)
					game.EnsureUIThread (() => window.CursorVisible = value);
				else
					window.CursorVisible = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.MouseBuffer"/> class.
		/// </summary>
		public MouseBuffer (NativeWindow window, Game game = null, bool shouldCenterMouse = false, bool mouseVisible = true) {

			// Set the window
			this.window = window;
			this.game = game;

			// Initialize wheel and delta values
			Wheel = 0.0f;
			DeltaX = 0.0f;
			DeltaY = 0.0f;
			DeltaZ = 0.0f;

			// Initialize mouse coordinates
			X = window.Width / 2f;
			Y = window.Height / 2f;

			// Initialize mouse state
			State = Mouse.GetState ();

			// Set mouse centered state
			ShouldCenterMouse = shouldCenterMouse;

			// Set mouse visible state
			CursorVisible = mouseVisible;

			// Center the mouse
			CenterMouse ();
		}

		/// <summary>
		/// Determines whether the specified button is pressed or not.
		/// </summary>
		/// <returns>Whether the specified button is pressed or not.</returns>
		/// <param name="button">Button.</param>
		public bool IsButtonDown (MouseButton button) {
			return State.IsButtonDown (button);
		}

		/// <summary>
		/// Determines whether the specified button is released or not.
		/// </summary>
		/// <returns>Whether the specified button is released or not.</returns>
		/// <param name="button">Button.</param>
		public bool IsButtonUp (MouseButton button) {
			return State.IsButtonUp (button);
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		internal void Update () {

			// Get the latest mouse state
			var cur = Mouse.GetState ();

			// Get the absolute mouse state
			var absoluteState = Mouse.GetCursorState ();

			// Create a rectangle representing the cursor
			var mouseRectRaw = new Rectangle (absoluteState.X, absoluteState.Y, 1, 1);
			var mouseClientPoint = window.PointToClient (mouseRectRaw.Location);
			var mouseClientRect = new Rectangle (mouseClientPoint, new Size (1, 1));

			// Update if the window is focused
			if (window.Focused && (window.ClientRectangle.IntersectsWith (mouseClientRect) || !wasUnfocused)) {

				// Check if updating the mouse is suppressed
				if (suppressUpdate) {

					// Don't suppress updating the mouse anymore
					// if the window was left-clicked
					suppressUpdate &= !cur.IsButtonDown (MouseButton.Left);
					wasUnfocused &= !cur.IsButtonDown (MouseButton.Left);

					// Go to the end of the if statement
					// Yes, the goto is really needed here
					goto end;
				}

				// Update delta values
				DeltaX = cur.X - State.X;
				DeltaY = cur.Y - State.Y;
				DeltaZ = cur.WheelPrecise - State.WheelPrecise;

				// Center the mouse
				//if (ShouldCenterMouse)
				//	CenterMouse ();

				// Clamp MouseCoordinates
				X = mouseClientPoint.X;
				Y = mouseClientPoint.Y;
				//X = MathHelper.Clamp (X + DeltaX, 0, window.Width);
				//Y = MathHelper.Clamp (Y + DeltaY, 0, window.Height);
				Wheel += DeltaZ;
			} else {

				// Set delta values to 0
				DeltaX = 0;
				DeltaY = 0;
				DeltaZ = 0;

				// Suppress updating the mouse
				wasUnfocused |= !window.Focused;
				suppressUpdate |= wasUnfocused;
			}

			// Jump marker for jumping to the end of the if statement
			// if the window was left-clicked to stop suppressing the mouse update
			end:

			// Update the mouse state
			State = cur;
		}

		/// <summary>
		/// Center the mouse.
		/// </summary>
		internal void CenterMouse () {

			if (!ShouldCenterMouse)
				return;

			// Calculate target x position
			var x = window.Bounds.Left + window.Bounds.Width / 2;

			// Calculate target y position
			var y = window.Bounds.Top + window.Bounds.Height / 2;

			// Set new mouse position
			Mouse.SetPosition(x, y);
		}
	}
}

