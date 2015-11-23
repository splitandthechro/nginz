﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using OpenTK;

namespace nginz {
	public class Camera {
		public Resolution Resolution { get; set; }

		public float FieldOfView { get; set; }
		public float AspectRatio {
			get {
				return Resolution.Width / Resolution.Height;
			}
		}
		public float Near { get; set; }
		public float Far { get; set; }

		public Matrix4 ProjectionMatrix { get; set; }

		public Vector3 Position { get; set; } = Vector3.Zero;
		public Quaternion Orientation { get; set; } = Quaternion.Identity;

		public Matrix4 ViewMatrix {
			get {
				return Matrix4.CreateTranslation (-Position)
					   * Matrix4.CreateFromQuaternion (Orientation);
			}
		}

		public Matrix4 ViewProjectionMatrix {
			get {
				return ViewMatrix * ProjectionMatrix;
			}
		}

		public Camera (float fieldOfView, Resolution resolution, float near, float far, bool radians = false, bool orthographic = false) {
			this.FieldOfView = radians ? fieldOfView : MathHelper.DegreesToRadians (fieldOfView);
			this.Resolution = resolution;
			this.Near = near;
			this.Far = far;

			this.ProjectionMatrix = orthographic ? Matrix4.CreateOrthographic (Resolution.Width, Resolution.Height, Near, Far)
					: Matrix4.CreatePerspectiveFieldOfView (FieldOfView, AspectRatio, Near, Far);
		}
		
		public void SetAbsolutePosition (Vector3 position) {
			this.Position = position;
		}
		public void SetRelativePosition (Vector3 relative) {
			this.Position += relative;
		}
		
		public void SetRotation (Vector3 axix, float amount, bool radians = false) {
			this.Orientation = Quaternion.FromAxisAngle (axix, radians ? amount : MathHelper.DegreesToRadians (amount));
		}
		public void SetRelativeRotation (Vector3 axix, float amount, bool radians = false) {
			this.Orientation += Quaternion.FromAxisAngle (axix, radians ? amount : MathHelper.DegreesToRadians (amount));
		}
		
		public void SetRotation (float pitch, float yaw, float roll, bool radians = false) {
			pitch = radians ? pitch : MathHelper.DegreesToRadians (pitch);
			yaw = radians ? yaw : MathHelper.DegreesToRadians (yaw);
			roll = radians ? roll : MathHelper.DegreesToRadians (roll);
			this.Orientation = Quaternion.FromEulerAngles (pitch, yaw, roll);
		}
		public void SetRelativeRotation (float pitch, float yaw, float roll, bool radians = false) {
			pitch = radians ? pitch : MathHelper.DegreesToRadians (pitch);
			yaw = radians ? yaw : MathHelper.DegreesToRadians (yaw);
			roll = radians ? roll : MathHelper.DegreesToRadians (roll);
			this.Orientation += Quaternion.FromEulerAngles (pitch, yaw, roll);
		}
	}

	public struct Resolution {
		public int Width;
		public int Height;
	}
}