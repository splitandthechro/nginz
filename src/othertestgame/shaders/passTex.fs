#version 450
in vec3 f_col;
in vec2 f_tex;

uniform sampler2D tex;

out vec4 frag_color;

void main () {
	frag_color = vec4(f_col, 1.0) * texture(tex, f_tex);
}