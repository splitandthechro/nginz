#version 450
in vec4 v_pos;
in vec2 v_tex;
in vec4 v_col;

out vec4 f_col;
out vec2 f_tex;

uniform mat4 VP;
uniform vec3 Right;
uniform vec3 Up;

void main () {
	f_col = v_col;
	f_tex = v_tex;

	float part_size = v_pos.w;

	vec3 ver_pos =
		Right * v_pos.x * part_size
		+ Up * v_pos.y * part_size;

	gl_Position = VP * vec4 (ver_pos, 1.0);
}