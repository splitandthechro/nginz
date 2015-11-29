using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;

namespace nginz {

	[StructLayout (LayoutKind.Sequential)]
	struct Vertex2D {
		public Vector3 Position;
		public Vector2 TextureCoordinate;
		public Color4 Color;
		public Vertex2D (Vector3 pos, Vector2 texcoord, Color4 color) {
			Position = pos;
			TextureCoordinate = texcoord;
			Color = color;
		}
		public static readonly int Size = TypeHelper.SizeOf (typeof (Vertex2D));
	}
}
