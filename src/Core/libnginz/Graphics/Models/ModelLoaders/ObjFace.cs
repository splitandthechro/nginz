using System;
using System.Collections.Generic;

namespace nginz
{
	public class ObjFace
	{
		readonly public List<ObjFaceVertex> Vertices;

		public ObjFace () {
			Vertices = new List<ObjFaceVertex> ();
		}
	}
}

