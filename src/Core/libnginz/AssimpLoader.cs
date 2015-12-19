using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assimp;
using Assimp.Configs;
using nginz.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace nginz {
	public class AssimpLoader : ICanLog {
		public static Dictionary<string, Model> LoadModels (string path, ShaderProgram program) {
			AssimpContext importer = new AssimpContext ();
			NormalSmoothingAngleConfig config = new NormalSmoothingAngleConfig (66.0f);
			importer.SetConfig (config);

			LogStream logStream = new LogStream ((string msg, string userData) => {
				Console.Write ("{0}", msg);
				if (!string.IsNullOrEmpty (userData))
					Console.WriteLine ("\t{0}", userData);
		});
			logStream.Attach ();

			Scene scene = importer.ImportFile (path,
												PostProcessSteps.CalculateTangentSpace |
												PostProcessSteps.Triangulate |
												PostProcessSteps.GenerateSmoothNormals);
			Dictionary<string, Model> models = new Dictionary<string, Model> ();

			foreach (var mesh in scene.Meshes) {
				List<Vector3> pos = new List<Vector3> ();
				List<Vector2> tex = new List<Vector2> ();
				List<Vector3> nrm = new List<Vector3> ();

				foreach (var group in mesh.Faces) {
					foreach (var ind in group.Indices) {
						pos.Add (mesh.Vertices[ind].ToVector3 ());
						tex.Add (mesh.TextureCoordinateChannels[0][ind].ToVector3 ().ToVector2 ());
						nrm.Add (mesh.Normals[ind].ToVector3 ());
					}
				}

				models.Add (mesh.Name, new Model (new Geometry (BeginMode.Triangles)
																.AddBuffer ("v_pos", pos.ToGLBuffer ())
																.AddBuffer ("v_tex", tex.ToGLBuffer ())
																.Construct (program)));
			}

			return models;
		}
	}
}
