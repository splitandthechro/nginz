using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Input;

namespace nginz {
	public class FPSCamera {
		public Camera Camera;

		private MouseBuffer mouse;
		private KeyboardBuffer keyboard;

		public Vector2 MouseRotation;
		public Vector3 Movement;

		public float Height = 0;

		public FPSCamera (float fieldOfView, Resolution resolution, MouseBuffer mouse, KeyboardBuffer keyboard) {
			Camera = new Camera (fieldOfView, resolution, 0.01f, 64f);

			this.mouse = mouse;
			this.keyboard = keyboard;

			MouseXSensitivity = 0.1f;
			MouseYSensitivity = 0.1f;

			Speed = 5f;
		}

		float deg90 = MathHelper.DegreesToRadians (90);
		float deg360 = MathHelper.DegreesToRadians (360);

		protected void ClampMouseValues () {

			if (MouseRotation.Y >= deg90) //90 degrees in radians
				MouseRotation.Y = deg90;
			if (MouseRotation.Y <= -deg90)
				MouseRotation.Y = -deg90;

			if (MouseRotation.X >= deg360) //360 degrees in radians (or something in radians)
				MouseRotation.X -= deg360;
			if (MouseRotation.X <= -deg360)
				MouseRotation.X += deg360;
		}

		public float MouseYSensitivity { get; set; }
		public float MouseXSensitivity { get; set; }
		public float Speed { get; set; }

		public Quaternion TargetOrientation { get; set; }
		public Quaternion TargetOrientationY { get; set; }

		protected void ApplyRotation () {
			TargetOrientationY = Quaternion.FromAxisAngle (Vector3.UnitY, (MathHelper.Pi + MouseRotation.X));
			TargetOrientation = Quaternion.FromAxisAngle (Vector3.UnitX, MouseRotation.Y) * TargetOrientationY;

		}

		protected void UpdateRotations (double time) {
			MouseRotation.X += (float) (mouse.DeltaX * MouseXSensitivity * time);
			MouseRotation.Y += (float) (mouse.DeltaY * MouseYSensitivity * time);

			ClampMouseValues ();
			ApplyRotation ();
			Camera.Orientation = TargetOrientation;
		}

		public void Update (GameTime dtime) {
			float time = dtime.Elapsed.Milliseconds * .0005f;

			if (keyboard.IsKeyDown(Key.W) && !keyboard.IsKeyDown (Key.S)) {
				Movement.Z = 0;
				Movement.Z -= Speed * time;
			} else if (keyboard.IsKeyDown (Key.S) && !keyboard.IsKeyDown (Key.W)) {
				Movement.Z = 0;
				Movement.Z += Speed * time;
			} else Movement.Z = 0.0f;

			if (keyboard.IsKeyDown (Key.A) && !keyboard.IsKeyDown (Key.D)) {
				Movement.X = 0.0f;
				Movement.X -= Speed * time;
			} else if (keyboard.IsKeyDown (Key.D) && !keyboard.IsKeyDown (Key.A)) {
				Movement.X = 0.0f;
				Movement.X += Speed * time;
			} else
				Movement.X = 0.0f;

			UpdateRotations (time);

			var pos = Vector3.Transform (Movement, Camera.Orientation.Inverted ());
			pos = new Vector3 (pos.X, Height, pos.Z);

			Camera.SetRelativePosition (pos);
		}
	}
}
