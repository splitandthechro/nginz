using System;
using OpenTK;
using OpenTK.Graphics;

namespace nginz
{

	/// <summary>
	/// Material.
	/// </summary>
	public class OldMaterial
	{

		/// <summary>
		/// The name of the material.
		/// </summary>
		readonly public string Name;

		/// <summary>
		/// The color of the ambient.
		/// </summary>
		public Vector3 AmbientColor;

		/// <summary>
		/// The color of the diffuse.
		/// </summary>
		public Vector3 DiffuseColor;

		/// <summary>
		/// The color of the specular.
		/// </summary>
		public Vector3 SpecularColor;

		/// <summary>
		/// The specular coefficient.
		/// </summary>
		public float SpecularCoefficient;

		/// <summary>
		/// The transparency value.
		/// </summary>
		public float Transparency;

		/// <summary>
		/// The illumination model.
		/// </summary>
		public int IlluminationModel;

		/// <summary>
		/// The alpha texture map.
		/// </summary>
		public string AlphaTextureMap;

		/// <summary>
		/// The ambient texture map.
		/// </summary>
		public string AmbientTextureMap;

		/// <summary>
		/// The diffuse texture map.
		/// </summary>
		public string DiffuseTextureMap;

		/// <summary>
		/// The specular texture map.
		/// </summary>
		public string SpecularTextureMap;

		/// <summary>
		/// The specular highlight texture map.
		/// </summary>
		public string SpecularHighlightTextureMap;

		/// <summary>
		/// The displacement map.
		/// </summary>
		public string DisplacementMap;

		/// <summary>
		/// The stencil decal map.
		/// </summary>
		public string StencilDecalMap;

		/// <summary>
		/// The bump map.
		/// </summary>
		public string BumpMap;

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Material"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		public OldMaterial (string name) {
			Name = name;
		}
	}
}

