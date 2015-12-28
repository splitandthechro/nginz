#version 450
in vec2 f_tex;
in vec3 f_nrm;
in vec3 f_pos;

struct Material {
	vec4 color;
	sampler2D diffuse;
	sampler2D normal;
	float specularIntensity;
	float specularPower;
};

uniform Material material;

out vec4 diffuse;
out vec4 normal;
out vec4 specular;

void main () {
	diffuse = material.color * texture (material.diffuse, f_tex);
	normal = vec4(0.5 * ((f_nrm * texture (material.normal, f_tex).xyz) + vec3 (1.0)), 1.0);
	specular = vec4 (material.specularIntensity, material.specularPower, 1.0, 1.0);
}