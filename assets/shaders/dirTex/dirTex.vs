﻿#version 450
in vec3 v_pos;
in vec2 v_tex;
in vec3 v_nrm;

uniform mat4 MVP;
uniform mat4 NRM;

out vec2 f_tex;
smooth out vec3 f_nrm;

void main () {
	f_tex = v_tex;
	f_nrm = (NRM * vec4 (v_nrm, 0.0)).xyz;
	gl_Position = MVP * vec4 (v_pos, 1.0);
}