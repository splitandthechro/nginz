#version 450
in vec2 f_tex;

struct Material {
	vec4 color;
	sampler2D diffuse;
	float specularIntensity;
	float specularPower;
};

uniform Material material;
uniform vec3 ambient_color;

out vec4 frag_color;

void main () {
	frag_color = material.color * texture2D (material.diffuse, f_tex) * vec4 (ambient_color, 1.0);
}