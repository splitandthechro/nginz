using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using nginz.Common;
using OpenTK;

namespace nginz
{

	/// <summary>
	/// Object file loader.
	/// </summary>
	public class ObjLoader : ICanLog
	{

		/// <summary>
		/// The result.
		/// </summary>
		ObjFile result;

		/// <summary>
		/// The position.
		/// </summary>
		int pos;

		/// <summary>
		/// The line.
		/// </summary>
		int line;

		/// <summary>
		/// The line position.
		/// </summary>
		int linepos;

		/// <summary>
		/// The source.
		/// </summary>
		string src;

		/// <summary>
		/// The current face group.
		/// </summary>
		string currentGroup;

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.ObjLoader"/> class.
		/// </summary>
		/// <param name="source">Source.</param>
		public ObjLoader (string source) {
			result = new ObjFile ();
			src = source;
			pos = -1;
			line = 1;
			linepos = 0;
			currentGroup = "default";
		}

		/// <summary>
		/// Load the object file.
		/// </summary>
		public ObjFile Load () {
			
			while (CanAdvance ()) {

				// Skip whitespace
				SkipWhitespace ();

				// Break if it is not possible to advance
				// in the source character stream
				if (!CanAdvance ())
					break;

				// Skip comments
				if (Peek () == '#')
					SkipLine ();

				switch (ReadRawString ()) {
				case "v":
					ParseVertex ();
					break;
				case "vt":
					ParseTexture ();
					break;
				case "vn":
					ParseNormal ();
					break;
				case "usemtl":
					ParseMaterialName ();
					break;
				case "s":
					ParseSurfaceType ();
					break;
				case "f":
					ParseFace ();
					break;
				}
			}

			// Return the parsed obj file
			return result;
		}

		/// <summary>
		/// Construct a Vector3 from the specified float array.
		/// </summary>
		/// <param name="floats">Floats.</param>
		static Vector3 Vec3 (IList<float> floats) {
			return new Vector3 (floats [0], floats [1], floats [2]);
		}

		/// <summary>
		/// Construct a Vector2 from the specified float array.
		/// </summary>
		/// <param name="floats">Floats.</param>
		static Vector2 Vec2 (IList<float> floats) {
			return new Vector2 (floats [0], floats [1]);
		}

		/// <summary>
		/// Parse a vertex.
		/// </summary>
		void ParseVertex () {
			SkipWhitespace ();
			var raw = ReadFloats (3);
			var vertex = Vec3 (raw);
			this.Log ("Read vertex: {0}", vertex);
			result.Vertices.Add (vertex);
		}

		/// <summary>
		/// Parse a texture.
		/// </summary>
		void ParseTexture () {
			SkipWhitespace ();
			var raw = ReadFloats (2);
			var texture = Vec2 (raw);
			this.Log ("Read texture: {0}", texture);
			result.Textures.Add (texture);
		}

		/// <summary>
		/// Parse a normal.
		/// </summary>
		void ParseNormal () {
			SkipWhitespace ();
			var raw = ReadFloats (3);
			var normal = Vec3 (raw);
			this.Log ("Read normal: {0}", normal);
			result.Normals.Add (normal);
		}

		/// <summary>
		/// Parse a material name.
		/// </summary>
		void ParseMaterialName () {
			SkipWhitespace ();
			var name = ReadRawString ();
			this.Log ("Read material: {0}", name);
			result.Materials.Add (new Material (name));
		}

		/// <summary>
		/// Parse a surface rendeirng type.
		/// </summary>
		void ParseSurfaceType () {
			SkipWhitespace ();
			var raw = ReadInt ();
			var type = (ObjSurfaceType) raw;
			this.Log ("Read shading type: {0}", type);
			result.SurfaceType = type;
		}

		/// <summary>
		/// Parse a face.
		/// </summary>
		void ParseFace () {
			SkipWhitespace ();
			var face = new ObjFace ();
			var raw = ReadVertexGroupString (ReadLine ());
			for (var i = 0; i < raw.Length; i++) {
				var vertex = ExtractFaceVertex (raw [i]);
				var vi = vertex.VertexIndex;
				var ti = vertex.TextureIndex;
				var ni = vertex.NormalIndex;
				this.Log ("Read face vertex (V:{0}; T:{1}; N:{2})", vi, ti, ni);
				face.Vertices.Add (vertex);
			}
			if (result.Groups.All (e => e.Name != currentGroup)) {
				result.Groups.Add (new ObjFaceGroup (currentGroup));
				this.Log ("Added face group {0}", currentGroup);
			}
			var g = result.Groups.First (e => e.Name == currentGroup);
			g.Faces.Add (face);
		}

		/// <summary>
		/// Extract a face vertex.
		/// </summary>
		/// <returns>The face vertex.</returns>
		/// <param name="vertexStr">The vertex string.</param>
		ObjFaceVertex ExtractFaceVertex (string vertexStr) {
			var vertices = ReadVertexGroup (vertexStr);
			var face = new ObjFaceVertex {
				VertexIndex = 0,
				TextureIndex = 0,
				NormalIndex = 0
			};
			face.VertexIndex = ParseInt (vertices [0]);
			if (vertices.Length > 1)
				face.TextureIndex = ParseInt (vertices [1]);
			if (vertices.Length > 2)
				face.NormalIndex = ParseInt (vertices [2]);
			return face;
		}

		/// <summary>
		/// Read a vertex group string.
		/// </summary>
		/// <returns>The vertex group string.</returns>
		/// <param name="str">String.</param>
		static string[] ReadVertexGroupString (string str) {
			var options = StringSplitOptions.RemoveEmptyEntries;
			return str.Split (new [] { ' ' }, options); 
		}

		/// <summary>
		/// Read a vertex group.
		/// </summary>
		/// <returns>The vertex group.</returns>
		/// <param name="str">String.</param>
		static string[] ReadVertexGroup (string str) {
			var options = StringSplitOptions.RemoveEmptyEntries;
			return str.Split (new [] { '/' }, options); 
		}

		/// <summary>
		/// Read a numeric value.
		/// </summary>
		/// <returns>The numeric base.</returns>
		/// <param name="allowed">Allowed.</param>
		string ReadNumericBase (string allowed) {
			SkipWhitespace ();
			var accum = new StringBuilder ();
			while (CanAdvance () && allowed.Contains (Peek ().ToString ()))
				accum.Append (Read ());
			return accum.ToString ();
		}

		/// <summary>
		/// Read a raw string.
		/// </summary>
		/// <returns>The raw string.</returns>
		string ReadRawString () {
			SkipWhitespace ();
			var accum = new StringBuilder ();
			while (CanAdvance () && !char.IsWhiteSpace (Peek ()))
				accum.Append (Read ());
			return accum.ToString ();
		}

		/// <summary>
		/// Read an integer.
		/// </summary>
		/// <returns>The int.</returns>
		int ReadInt () {
			var str = ReadNumericBase ("0123456789+-");
			return ParseInt (str);
		}

		/// <summary>
		/// Parse an integer.
		/// </summary>
		/// <returns>The int.</returns>
		/// <param name="str">String.</param>
		int ParseInt (string str) {
			var format = NumberFormatInfo.InvariantInfo;
			var styles = NumberStyles.Integer;
			int parsed;
			if (!int.TryParse (str, styles, format, out parsed))
				LogExtensions.ThrowStatic ("Invalid obj file: Expected integer at line {0}:{1}", line, linepos);
			return parsed;
		}

		/// <summary>
		/// Read a floating-point number.
		/// </summary>
		/// <returns>The float.</returns>
		float ReadFloat () {
			var str = ReadNumericBase ("0123456789+-.");
			return ParseFloat (str);
		}

		/// <summary>
		/// Parse a floating-point number.
		/// </summary>
		/// <returns>The float.</returns>
		/// <param name="str">String.</param>
		float ParseFloat (string str) {
			var format = NumberFormatInfo.InvariantInfo;
			var styles = NumberStyles.Float;
			float parsed;
			if (!float.TryParse (str, styles, format, out parsed))
				LogExtensions.ThrowStatic ("Invalid obj file: Expected float at line {0}:{1}", line, linepos);
			return parsed;
		}

		/// <summary>
		/// Read multiple floating-point numbers.
		/// </summary>
		/// <returns>The floats.</returns>
		/// <param name="count">Count.</param>
		float[] ReadFloats (int count) {
			var floats = new float[count];
			for (var i = 0; i < count; i++)
				floats [i] = ReadFloat ();
			return floats;
		}

		/// <summary>
		/// Whether it is possible to advance
		/// in the source character stream.
		/// </summary>
		/// <returns><c>true</c> if this instance can advance the specified count; otherwise, <c>false</c>.</returns>
		/// <param name="count">Count.</param>
		bool CanAdvance (int count = 1) {
			return pos + count < src.Length;
		}

		/// <summary>
		/// Skip the specified count.
		/// </summary>
		/// <param name="count">Count.</param>
		void Skip (int count = 1) {
			for (var i = 0; i < count; i++) {
				if (!CanAdvance ())
					return;
				pos++;
				if (Peek (0) == '\n') {
					line++;
					linepos = 0;
				} else
					linepos++;
			}
		}

		/// <summary>
		/// Skip a line.
		/// </summary>
		void SkipLine () {
			while (CanAdvance () && Peek () != '\n')
				Skip ();
			Skip ();
		}

		/// <summary>
		/// Skip whitespace.
		/// </summary>
		void SkipWhitespace () {
			while (CanAdvance () && char.IsWhiteSpace (Peek ()))
				Skip ();
		}

		/// <summary>
		/// Peek a character.
		/// </summary>
		/// <param name="lookahead">Lookahead.</param>
		char Peek (int lookahead = 1) {
			var chr =
				CanAdvance (lookahead)
				? src [pos + lookahead]
				: '\0';
			return chr;
		}

		/// <summary>
		/// Read a character.
		/// </summary>
		char Read () {
			Skip ();
			return Peek (0);
		}

		/// <summary>
		/// Read a line.
		/// </summary>
		/// <returns>The line.</returns>
		string ReadLine () {
			var accum = new StringBuilder ();
			while (CanAdvance () && Peek () != '\n')
				accum.Append (Read ());
			Skip ();
			return accum.ToString ();
		}
	}
}

