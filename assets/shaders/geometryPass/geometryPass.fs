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

vec2 encode (vec3 n) {
    vec2 enc = normalize(n.xy) * (sqrt(-n.z*0.5+0.5));
    enc = enc*0.5+0.5;
    return enc;
}

void main () {
	diffuse = material.color * texture (material.diffuse, f_tex);
	normal = vec4(encode (f_nrm * texture (material.normal, f_tex).xyz), 0, 0);
	specular = vec4 (material.specularIntensity, material.specularPower, 1.0, 1.0);
}