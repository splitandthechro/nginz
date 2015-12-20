using System;
using System.Collections.Generic;
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

		/// <summary>
		/// Gets the specified attribute.
		/// </summary>
		/// <param name="attribute">Attribute name.</param>
		public int Attrib (string attribute) {

			// Check if the attribute cache contains the attribute
			// If not, add it to the cache
			if (!attributes.ContainsKey (attribute)) {
				attributes.Add (attribute, GL.GetAttribLocation (programId, attribute));
			}

			// Return the attribute
			return attributes [attribute];
		}
	}
}
