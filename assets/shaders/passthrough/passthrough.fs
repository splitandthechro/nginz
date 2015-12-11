#version 450
in vec3 f_col;

out vec4 frag_color;

void main () {
	frag_color = vec4(f_col, 1.0);
}