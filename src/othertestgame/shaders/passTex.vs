#version 450
in vec3 v_pos;
in vec2 v_tex;

uniform mat4 MVP;

out vec2 f_tex;

void main () {
	f_tex = v_tex;
	gl_Position = MVP * vec4 (v_pos, 1.0);
}