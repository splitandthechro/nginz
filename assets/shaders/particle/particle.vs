#version 450
in vec3 v_vert;
in vec4 v_col;

out vec4 f_col;

uniform mat4 VP;
uniform vec3 Right;

void main () {
	f_col = v_col;

	gl_Position = VP * vec4 (v_vert, 1.0);
}