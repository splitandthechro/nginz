#version 450
in vec3 v_pos;
in vec3 v_col;
in vec2 v_tex;

uniform mat4 MVP;

out vec3 f_col;
out vec2 f_tex;

void main () {
	f_col = v_col;
	f_tex = v_tex;
	gl_Position = MVP * vec4 (v_pos, 1.0);
}