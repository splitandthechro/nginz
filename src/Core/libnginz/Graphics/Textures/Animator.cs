using System;
using OpenTK;
using OpenTK.Graphics;
using System.Drawing;

namespace nginz
{
	/// <summary>
	/// Animator.
	/// </summary>
	public class Animator : IUpdatable, IDrawable2D
	{
		/// <summary>
		/// The speed multiplier.
		/// </summary>
		public float Speed;

		/// <summary>
		/// The position.
		/// </summary>
		public Vector2 Position;

		/// <summary>
		/// The scale.
		/// </summary>
		public Vector2 Scale;

		/// <summary>
		/// The x rotation.
		/// </summary>
		public float RotationX;

		/// <summary>
		/// The tint.
		/// </summary>
		public Color4 Tint;

		/// <summary>
		/// Gets or sets the duration in seconds.
		/// </summary>
		/// <value>The duration in seconds.</value>
		public float DurationInSeconds {
			get { return Duration; }
			set {
				Duration = value;
				UpdateTarget ();
			}
		}

		/// <summary>
		/// Gets or sets the duration in milliseconds.
		/// </summary>
		/// <value>The duration in milliseconds.</value>
		public float DurationInMilliseconds {
			get { return Duration * 1000f; }
			set {
				Duration = value / 1000f;
				UpdateTarget ();
			}
		}

		/// <summary>
		/// Gets the origin.
		/// </summary>
		/// <value>The origin.</value>
		public Vector2 Origin {
			get {
				return origin * Scale;
			}
			set {
				origin = value;
			}
		}

		Vector2 origin;

		public Rectangle Bounds {
			get {
				return new Rectangle (
					x: (int) Position.X - (Sheet.TileWidth / 2),
					y: (int) Position.Y - (Sheet.TileHeight / 2),
					width: Sheet.TileWidth,
					height: Sheet.TileHeight
				);
			}
		}

		/// <summary>
		/// The sprite sheet.
		/// </summary>
		readonly public SpriteSheet2D Sheet;

		/// <summary>
		/// The start tile.
		/// </summary>
		readonly int StartTile;

		/// <summary>
		/// The tile count.
		/// </summary>
		readonly int Count;

		/// <summary>
		/// The index.
		/// </summary>
		int Index;

		/// <summary>
		/// The duration.
		/// </summary>
		float Duration;

		/// <summary>
		/// The delta.
		/// </summary>
		float Delta;

		/// <summary>
		/// The delta target.
		/// </summary>
		float Target;

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Animator"/> class.
		/// </summary>
		/// <param name="sheet">Sheet.</param>
		/// <param name="tileCount">Tile count.</param>
		/// <param name="startTile">Start tile.</param>
		public Animator (SpriteSheet2D sheet, int tileCount, int startTile = 0) {
			Sheet = sheet;
			StartTile = startTile;
			Count = tileCount;
			Index = 0;
			Speed = 1;
			Position = Vector2.Zero;
			Scale = Vector2.One;
			Tint = Color4.White;
			Origin = Vector2.Zero;
			RotationX = 0;
			UpdateTarget ();
		}

		#region IUpdatable implementation

		/// <summary>
		/// Update.
		/// </summary>
		/// <param name="time">Time.</param>
		public void Update (GameTime time) {

			// Calculate the delta increase
			var deltaIncrease = Speed * (float) time.Elapsed.TotalSeconds;
			Delta += deltaIncrease;

			// Update the tile index
			if (Delta >= Target) {
				Delta -= Target;
				++Index;
				if (Index == Count)
					Index = 0;
			}
		}

		#endregion

		#region IDrawable2D implementation

		/// <summary>
		/// Draw.
		/// </summary>
		/// <param name="time">Time.</param>
		/// <param name="batch">Batch.</param>
		public void Draw (GameTime time, SpriteBatch batch) {
			// Draw the tile
			batch.Draw (
				texture: Sheet.Texture,
				sourceRect: Sheet [StartTile + Index],
				position: Position,
				origin: new Vector2 (Sheet [StartTile + Index].Width / 2f, Sheet [StartTile + Index].Height / 2f),
				color: Tint,
				scale: Scale,
				rotation: RotationX
			);
		}

		#endregion

		/// <summary>
		/// Update the target.
		/// </summary>
		void UpdateTarget () {
			Target = Duration / (float) Count;
		}
	}
}

