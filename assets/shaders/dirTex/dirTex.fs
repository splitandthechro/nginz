#version 450
in vec2 f_tex;
in vec3 f_nrm;

uniform sampler2D tex;
uniform vec3 dir_loc;

out vec4 frag_color;

void main () {
	vec4 tex_color = texture2D (tex, f_tex);

	float diffuse_intensity = max (0.0, dot (normalize(f_nrm), dir_loc));

	frag_color = tex_color * vec4 (vec3(0.75, 0.75, 0.8) * diffuse_intensity, 1.0);
}