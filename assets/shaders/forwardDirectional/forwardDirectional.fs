#version 450
in vec2 f_tex;
in vec3 f_nrm;
in vec3 f_pos;

struct BaseLight {
	vec3 color;
	float intensity;
};

struct DirectionalLight {
	BaseLight base;
	vec3 direction;
};

struct Material {
	vec4 color;
	sampler2D diffuse;
	float specularIntensity;
	float specularPower;
};

uniform Material material;

uniform vec3 eye_pos;
uniform DirectionalLight directionalLight;

out vec4 frag_color;

vec4 calcLight (BaseLight base, vec3 direction, vec3 normal) {
	float diffuseFactor = dot (normal, -direction);

	vec4 diffuseColor = vec4 (0);
	vec4 specularColor = vec4 (0);

	if (diffuseFactor > 0) {
		diffuseColor = vec4 (base.color, 1.0) * base.intensity * diffuseFactor;

		vec3 directionToEye = normalize (eye_pos - f_pos);
		vec3 reflectDirection = normalize (reflect (direction, normal));

		float specularFactor = dot (directionToEye, reflectDirection);
		specularFactor = pow (specularFactor, material.specularPower);

		if (specularFactor > 0)
			specularColor = vec4 (base.color, 1.0) * material.specularIntensity * specularFactor;
	}

	return diffuseColor + specularColor * diffuseFactor;
}

vec4 calcDirectionalLigh (DirectionalLight light, vec3 normal) {
	return calcLight (light.base, -light.direction, normal);
}

void main () {
	frag_color = texture (material.diffuse, f_tex) * calcDirectionalLigh (directionalLight, normalize (f_nrm));
}