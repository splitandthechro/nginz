#version 450
in vec2 f_tex;

uniform sampler2D tex;

out vec4 frag_color;

void main () {
	frag_color = texture (tex, f_tex);
}