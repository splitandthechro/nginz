using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nginz.SpriteBatch {
	public partial class SpriteBatch {
		const string vert_source = @"
#version 450
in vec3 v_pos;
in vec4 v_col;
in vec2 v_tex;

out vec4 f_col;
out	vec2 f_tex;

uniform mat4 MVP;

void main () {
	f_col = v_col;
	f_tex = v_tex;

	gl_Position = MVP * vec4 (v_pos, 1.0);
}
";

		const string frag_source = @"
#version 450
in vec4 f_col;
in	vec2 f_tex;

out vec4 frag_color;

uniform sampler2D tex;

void main () {
	frag_colo = texture (tex, f_tex) * f_col;
}
";
	}
}
