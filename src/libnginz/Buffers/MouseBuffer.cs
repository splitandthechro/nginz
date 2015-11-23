using System;
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
		/// The mouse state.
		/// </summary>
		MouseState State;

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
		/// Initializes a new instance of the <see cref="nginz.MouseBuffer"/> class.
		/// </summary>
		public MouseBuffer (NativeWindow window, MouseState initialState) {

			// Set the window
			this.window = window;

			// Initialize fields
			X = 0.0f;
			Y = 0.0f;
			Wheel = 0.0f;
			DeltaX = 0.0f;
			DeltaY = 0.0f;
			DeltaZ = 0.0f;

			// Initialize mouse state
			State = initialState;
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
		internal void Update (MouseState state) {
			State = state;
			X = MathHelper.Clamp (X + DeltaX, 0, window.Width);
			Y = MathHelper.Clamp (Y + DeltaY, 0, window.Height);
			Wheel += DeltaZ;
		}
	}
}

