using System;
using OpenTK;
using OpenTK.Input;

namespace nginz
{

	/// <summary>
	/// First person camera.
	/// </summary>
	public class FPSCamera {

		/// <summary>
		/// The camera.
		/// </summary>
		readonly public Camera Camera;

		/// <summary>
		/// The mouse.
		/// </summary>
		readonly MouseBuffer Mouse;

		/// <summary>
		/// The keyboard.
		/// </summary>
		readonly KeyboardBuffer Keyboard;

		/// <summary>
		/// The mouse rotation.
		/// </summary>
		public Vector2 MouseRotation;

		/// <summary>
		/// The movement.
		/// </summary>
		public Vector3 Movement;

		/// <summary>
		/// The height of the camera.
		/// </summary>
		public float Height;

		/// <summary>
		/// Gets or sets the mouse X sensitivity.
		/// </summary>
		/// <value>The mouse X sensitivity.</value>
		public float MouseXSensitivity { get; set; }

		/// <summary>
		/// Gets or sets the mouse Y sensitivity.
		/// </summary>
		/// <value>The mouse Y sensitivity.</value>
		public float MouseYSensitivity { get; set; }

		/// <summary>
		/// Gets or sets the speed.
		/// </summary>
		/// <value>The speed.</value>
		public float Speed { get; set; }

		/// <summary>
		/// Gets or sets the target orientation.
		/// </summary>
		/// <value>The target orientation.</value>
		public Quaternion TargetOrientation { get; set; }

		/// <summary>
		/// Gets or sets the target orientation y.
		/// </summary>
		/// <value>The target orientation y.</value>
		public Quaternion TargetOrientationY { get; set; }

		/// <summary>
		/// 90 degrees in radians.
		/// </summary>
		const float deg90 = 1.5708f;

		/// <summary>
		/// 360 degrees in radians.
		/// </summary>
		const float deg360 = 6.28319f;

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.FPSCamera"/> class.
		/// </summary>
		/// <param name="fieldOfView">Field of view.</param>
		/// <param name="resolution">Resolution.</param>
		/// <param name="mouse">Mouse.</param>
		/// <param name="keyboard">Keyboard.</param>
		public FPSCamera (float fieldOfView, Resolution resolution, MouseBuffer mouse, KeyboardBuffer keyboard) {

			// Create the base camera
			Camera = new Camera (fieldOfView, resolution, 0.01f, 64f);

			// Set the mouse and the keyboard
			Mouse = mouse;
			Keyboard = keyboard;

			// Initialize the mouse sensitivity
			MouseXSensitivity = .1f;
			MouseYSensitivity = .1f;

			// Initialize the actor speed
			Speed = 5f;
		}

		/// <summary>
		/// Update the camera.
		/// </summary>
		/// <param name="time">The game time.</param>
		public void Update (GameTime time) {

			// Reset the movement values
			Movement.X = 0;
			Movement.Y = 0;
			Movement.Z = 0;

			// Get the time value
			float t = (float)time.Elapsed.TotalSeconds;

			// Calculate the movement distance
			var distance = Speed * t;

			// Check if the w key is down
			if (Keyboard.IsKeyDown (Key.W))

				// Update the movement accordingly
				Movement.Z = -distance;

			// Check if the s key is down
			if (Keyboard.IsKeyDown (Key.S))

				// Update the movement accordingly
				Movement.Z = distance;

			// Check if the a key is down
			if (Keyboard.IsKeyDown (Key.A))

				// Update the movement accordingly
				Movement.X = -distance;

			// Check if the d key is down
			if (Keyboard.IsKeyDown (Key.D))

				// Update the movement accordingly
				Movement.X = distance;

			// Update the mouse rotation
			UpdateMouseRotation (t);

			// Update the camera orientation
			Camera.Orientation = TargetOrientation;

			// Calculate the updated camera position
			var trans = Vector3.Transform (Movement, Camera.Orientation.Inverted ());
			var pos = new Vector3 (trans.X, Height, trans.Z);

			// Update the camera position
			Camera.SetRelativePosition (pos);
		}

		/// <summary>
		/// Update the mouse rotation.
		/// </summary>
		/// <param name="time">Time.</param>
		protected void UpdateMouseRotation (double time) {

			// Update the mouse rotation
			MouseRotation.X += (float) (Mouse.DeltaX * MouseXSensitivity * time);
			MouseRotation.Y += (float) (Mouse.DeltaY * MouseYSensitivity * time);

			// Clamp the mouse rotation
			ClampMouseRotation ();

			// Apply the mouse rotation
			ApplyRotation ();
		}

		/// <summary>
		/// Clamp the mouse rotation.
		/// </summary>
		protected void ClampMouseRotation () {

			// Clamp the mouse y rotation
			MouseRotation.Y = MathHelper.Clamp (MouseRotation.Y, -deg90, deg90);

			// Clamp the mouse x rotation
			if (MouseRotation.X >= deg360)
				MouseRotation.X -= deg360;
			if (MouseRotation.X <= -deg360)
				MouseRotation.X += deg360;
		}

		/// <summary>
		/// Apply the mouse rotation.
		/// </summary>
		protected void ApplyRotation () {
			TargetOrientationY = Quaternion.FromAxisAngle (Vector3.UnitY, (MathHelper.Pi + MouseRotation.X));
			TargetOrientation = Quaternion.FromAxisAngle (Vector3.UnitX, MouseRotation.Y) * TargetOrientationY;

		}
	}
}
