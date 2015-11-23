#version 450
in vec3 v_pos;
in vec3 v_col;

uniform mat4 MVP;

out vec3 f_col;

void main () {
	f_col = v_col;
	gl_Position = MVP * vec4 (v_pos, 1.0);
}