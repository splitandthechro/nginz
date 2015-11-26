using System;
using System.Collections.Generic;
using OpenTK;

namespace nginz
{
	public class ObjFile
	{
		readonly public List<Vector3> Vertices;
		readonly public List<Vector2> Textures;
		readonly public List<Vector3> Normals;
		readonly public List<Material> Materials;
		readonly public List<ObjFaceGroup> Groups;

		public ObjSurfaceType SurfaceType;

		public ObjFile () {
			Vertices = new List<Vector3> ();
			Textures = new List<Vector2> ();
			Normals = new List<Vector3> ();
			Materials = new List<Material> ();
			Groups = new List<ObjFaceGroup> ();
		}
	}
}

