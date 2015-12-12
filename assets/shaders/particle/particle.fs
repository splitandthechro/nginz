#version 450
in vec4 f_col;
in vec2 f_tex;

uniform sampler2D tex;

out vec4 frag_color;

void main () {
	frag_color = f_col * texture(tex, f_tex);
}