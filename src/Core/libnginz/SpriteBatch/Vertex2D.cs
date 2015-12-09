using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;

namespace nginz
{

	/// <summary>
	/// 2D Vertex.
	/// </summary>
	[StructLayout (LayoutKind.Sequential)]
	struct Vertex2D
	{

		/// <summary>
		/// The size.
		/// </summary>
		public static readonly int Size;

		/// <summary>
		/// Initializes the <see cref="nginz.Vertex2D"/> struct.
		/// </summary>
		static Vertex2D () {
			Size = TypeHelper.SizeOf (typeof(Vertex2D));
		}

		/// <summary>
		/// The position.
		/// </summary>
		public Vector3 Position;

		/// <summary>
		/// The texture coordinate.
		/// </summary>
		public Vector2 TextureCoordinate;

		/// <summary>
		/// The color.
		/// </summary>
		public Color4 Color;

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Vertex2D"/> struct.
		/// </summary>
		/// <param name="pos">Position.</param>
		/// <param name="texcoord">Texcoord.</param>
		/// <param name="color">Color.</param>
		public Vertex2D (Vector3 pos, Vector2 texcoord, Color4 color) : this () {
			Position = pos;
			TextureCoordinate = texcoord;
			Color = color;
		}
	}
}
