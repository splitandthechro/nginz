using System;
using System.Collections.Generic;

namespace nginz
{
	public class ObjFaceGroup
	{
		readonly public string Name;
		readonly public List<ObjFace> Faces;

		public ObjFaceGroup (string groupName) {
			Name = groupName;
			Faces = new List<ObjFace> ();
		}
	}
}

