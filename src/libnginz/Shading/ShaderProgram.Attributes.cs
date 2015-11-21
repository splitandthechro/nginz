using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL4;

namespace nginz {
	/// <summary>
	/// Shader program.
	/// </summary>
	public partial class ShaderProgram {
		/// <summary>
		/// The attributes.
		/// </summary>
		Dictionary<string, int> attributes;

		public int Attrib (string attribute) {
			if (!this.attributes.ContainsKey (attribute)) {
				this.attributes [attribute] = GL.GetAttribLocation (this.programId, attribute);
			}

			return this.attributes [attribute];
		}
	}
}
