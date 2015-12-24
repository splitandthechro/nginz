using System;
using System.Collections.Generic;
using Assimp;
using Assimp.Configs;
using nginz.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace nginz
{

	/// <summary>
	/// Assimp loader.
	/// </summary>
	public static class AssimpLoader
	{

		/// <summary>
		/// Load geometry data from a file.
		/// </summary>
		/// <returns>The geometry data.</returns>
		/// <param name="path">Path.</param>
		public static List<Geometry> LoadGeometry (string path) {

			// Create assimp context
			var importer = new AssimpContext ();

			// Create normal angle configuration
			var config = new NormalSmoothingAngleConfig (66.0f);
			importer.SetConfig (config);

			// Attach log stream
			new LogStream ((msg, userData) => {
				LogExtensions.LogStatic ("{0}", msg);
				if (!string.IsNullOrEmpty (userData))
					LogExtensions.LogStatic ("\t{0}", userData);
			}).Attach ();

			// Set post process flags
			var flags =
				PostProcessSteps.CalculateTangentSpace
				| PostProcessSteps.Triangulate
				| PostProcessSteps.GenerateSmoothNormals
				| PostProcessSteps.TransformUVCoords;

			// Import scene
			var scene = importer.ImportFile (path, flags);

			var geometry = new List<Geometry> ();

			// Iterate over meshes
			foreach (var mesh in scene.Meshes) {
				var pos = new List<Vector3> ();
				var tex = new List<Vector2> ();
				var nrm = new List<Vector3> ();

				var hasTexture = mesh.HasTextureCoords (0);

				// Iterate over face groups
				foreach (var group in mesh.Faces) {

					// Iterate over indices
					foreach (var ind in group.Indices) {
						pos.Add (mesh.Vertices[ind].ToVector3 ());
						if (hasTexture)
							tex.Add (mesh.TextureCoordinateChannels [0][ind].ToVector3 ().ToVector2 ());
						nrm.Add (mesh.Normals[ind].ToVector3 ());
					}
				}

				var geom = new Geometry (BeginMode.Triangles)
						.AddBuffer ("v_pos", pos.ToGLBuffer ());
				if (hasTexture)
					geom.AddBuffer ("v_tex", tex.ToGLBuffer ());
					geom.AddBuffer ("v_nrm", nrm.ToGLBuffer ());

				geometry.Add (geom);
			}

			return geometry;
		}
	}
}
