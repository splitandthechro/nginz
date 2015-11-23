using System;
using OpenTK;

namespace nginz {

	/// <summary>
	/// 2D/3D camera.
	/// </summary>
	public class Camera {

		/// <summary>
		/// Gets or sets the resolution.
		/// </summary>
		/// <value>The resolution.</value>
		public Resolution Resolution { get; set; }

		/// <summary>
		/// Gets or sets the field of view.
		/// </summary>
		/// <value>The field of view.</value>
		public float FieldOfView { get; set; }

		/// <summary>
		/// Gets the aspect ratio.
		/// </summary>
		/// <value>The aspect ratio.</value>
		public float AspectRatio { get { return Resolution.AspectRatio; } }

		/// <summary>
		/// Gets or sets the near plane.
		/// </summary>
		/// <value>The near plane.</value>
		public float Near { get; set; }

		/// <summary>
		/// Gets or sets the far plane.
		/// </summary>
		/// <value>The far plane.</value>
		public float Far { get; set; }

		/// <summary>
		/// Gets or sets the projection matrix.
		/// </summary>
		/// <value>The projection matrix.</value>
		public Matrix4 ProjectionMatrix { get; set; }

		/// <summary>
		/// Gets or sets the position.
		/// </summary>
		/// <value>The position.</value>
		public Vector3 Position { get; set; }

		/// <summary>
		/// Gets or sets the orientation.
		/// </summary>
		/// <value>The orientation.</value>
		public Quaternion Orientation { get; set; }

		/// <summary>
		/// Gets the view matrix.
		/// </summary>
		/// <value>The view matrix.</value>
		public Matrix4 ViewMatrix {
			get {

				// Create the translation matrix
				var translation = Matrix4.CreateTranslation (-Position);

				// Create the orientation matrix
				var orientation = Matrix4.CreateFromQuaternion (Orientation);

				// Multiply the matrices
				return translation * orientation;
			}
		}

		public ProjectionType ProjectionType;

		/// <summary>
		/// Gets the view projection matrix.
		/// </summary>
		/// <value>The view projection matrix.</value>
		public Matrix4 ViewProjectionMatrix {
			get {

				// Multiply the view matrix with the projection matrix
				return ViewMatrix * ProjectionMatrix;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Camera"/> class.
		/// </summary>
		/// <param name="fieldOfView">Field of view.</param>
		/// <param name="resolution">Resolution.</param>
		/// <param name="near">Near plane.</param>
		/// <param name="far">Far plane.</param>
		/// <param name="radians">Whether radians should be used instead of degrees.</param>
		/// <param name="type">The camera type.</param>
		public Camera (float fieldOfView, Resolution resolution, float near, float far, bool radians = false, ProjectionType type = ProjectionType.Perspective) {

			// Calculate the field of view
			FieldOfView = radians
				? fieldOfView
				: MathHelper.DegreesToRadians (fieldOfView);

			// Set the resolution
			Resolution = resolution;

			// Set the near plane
			Near = near;

			// Set the far plane
			Far = far;

			// Initialize the position
			Position = Vector3.Zero;

			// Initialize the orientation
			Orientation = Quaternion.Identity;

			// Set the camera projection type
			ProjectionType = type;

			// Calculate the projection graphics
			ProjectionMatrix =
				type == ProjectionType.Orthographic
				? Matrix4.CreateOrthographic (Resolution.Width, Resolution.Height, Near, Far)
				: Matrix4.CreatePerspectiveFieldOfView (FieldOfView, AspectRatio, Near, Far);
		}

		/// <summary>
		/// Update the camera matrix.
		/// </summary>
		/// <param name="resolution">Resolution.</param>
		public void UpdateCameraMatrix (Resolution resolution) {

			// Set the resolution
			Resolution = resolution;

			// Resize the projection matrix
			ProjectionMatrix =
				ProjectionType == ProjectionType.Orthographic
				? Matrix4.CreateOrthographic (Resolution.Width, Resolution.Height, Near, Far)
				: Matrix4.CreatePerspectiveFieldOfView (FieldOfView, AspectRatio, Near, Far);
		}

		/// <summary>
		/// Set the absolute position.
		/// </summary>
		/// <param name="position">Position.</param>
		public void SetAbsolutePosition (Vector3 position) {
			Position = position;
		}

		/// <summary>
		/// Set the relative position.
		/// </summary>
		/// <param name="relative">Relative.</param>
		public void SetRelativePosition (Vector3 relative) {
			Position += relative;
		}

		/// <summary>
		/// Set the rotation.
		/// </summary>
		/// <param name="axis">Axis.</param>
		/// <param name="amount">Amount.</param>
		/// <param name="radians">Whether radians should be used instead of degrees.</param>
		/// <param name="relative">Whether the rotation should be done relative to the current rotation.</param> 
		public void SetRotation (Vector3 axis, float amount, bool radians = false, bool relative = false) {

			// Calculate the rotation
			var rotation = radians
				? amount
				: MathHelper.DegreesToRadians (amount);

			// Calculate the start value
			var startValue = relative
				? Orientation
				: Quaternion.Identity;

			// Set the orientation
			Orientation = startValue + Quaternion.FromAxisAngle (axis, rotation);
		}

		/// <summary>
		/// Set the rotation.
		/// </summary>
		/// <param name="pitch">Pitch.</param>
		/// <param name="yaw">Yaw.</param>
		/// <param name="roll">Roll.</param>
		/// <param name="radians">Whether radians should be used instead of degrees.</param>
		/// <param name="relative">Whether the rotation should be done relative to the current rotation.</param> 
		public void SetRotation (float pitch, float yaw, float roll, bool radians = false, bool relative = false) {

			// Calculate the pitch
			pitch = radians
				? pitch
				: MathHelper.DegreesToRadians (pitch);

			// Calculate the yaw
			yaw = radians
				? yaw
				: MathHelper.DegreesToRadians (yaw);

			// Calculate the roll
			roll = radians
				? roll
				: MathHelper.DegreesToRadians (roll);

			// Calculate the start value
			var startValue = relative
				? Orientation
				: Quaternion.Identity;

			// Set the orientation
			Orientation = startValue + Quaternion.FromEulerAngles (pitch, yaw, roll);
		}
	}
}
