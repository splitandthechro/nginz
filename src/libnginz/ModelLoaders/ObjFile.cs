using System;
using System.Collections.Generic;
using nginz.Common;
using OpenTK;

namespace nginz
{

	/// <summary>
	/// Object file.
	/// </summary>
	public class ObjFile : IAsset
	{

		/// <summary>
		/// The vertices.
		/// </summary>
		readonly public List<Vector3> Vertices;

		/// <summary>
		/// The texture coordinates.
		/// </summary>
		readonly public List<Vector2> Textures;

		/// <summary>
		/// The normals.
		/// </summary>
		readonly public List<Vector3> Normals;

		/// <summary>
		/// The materials.
		/// </summary>
		readonly public List<Material> Materials;

		/// <summary>
		/// The face groups.
		/// </summary>
		readonly public List<ObjFaceGroup> Groups;

		/// <summary>
		/// The surface rendering method.
		/// </summary>
		public ObjSurfaceType SurfaceType;

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.ObjFile"/> class.
		/// </summary>
		public ObjFile () {
			Vertices = new List<Vector3> ();
			Textures = new List<Vector2> ();
			Normals = new List<Vector3> ();
			Materials = new List<Material> ();
			Groups = new List<ObjFaceGroup> ();
			SurfaceType = ObjSurfaceType.FlatShading;
		}
	}
}

