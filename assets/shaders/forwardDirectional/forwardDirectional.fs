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

float beckmannDistribution(float x, float roughness) {
  float NdotH = max(x, 0.0001);
  float cos2Alpha = NdotH * NdotH;
  float tan2Alpha = (cos2Alpha - 1.0) / cos2Alpha;
  float roughness2 = roughness * roughness;
  float denom = 3.141592653589793 * roughness2 * cos2Alpha * cos2Alpha;
  return exp(tan2Alpha / roughness2) / denom;
}

float cookTorranceSpecular(
  vec3 lightDirection,
  vec3 viewDirection,
  vec3 surfaceNormal,
  float roughness,
  float fresnel) {

  float VdotN = max(dot(viewDirection, surfaceNormal), 0.0);
  float LdotN = max(dot(lightDirection, surfaceNormal), 0.0);

  //Half angle vector
  vec3 H = normalize(lightDirection + viewDirection);

  //Geometric term
  float NdotH = max(dot(surfaceNormal, H), 0.0);
  float VdotH = max(dot(viewDirection, H), 0.000001);
  float LdotH = max(dot(lightDirection, H), 0.000001);
  float G1 = (2.0 * NdotH * VdotN) / VdotH;
  float G2 = (2.0 * NdotH * LdotN) / LdotH;
  float G = min(1.0, min(G1, G2));
  
  //Distribution term
  float D = beckmannDistribution(NdotH, roughness);

  //Fresnel term
  float F = pow(1.0 - VdotN, fresnel);

  //Multiply terms and done
  return  G * F * D / max(3.14159265 * VdotN, 0.000001);
}

#define PI 3.14159265

vec4 calcLight (BaseLight base, vec3 direction, vec3 normal) {
	float diffuseFactor = dot (normal, -direction);

	vec4 diffuseColor = vec4 (0);
	vec4 specularColor = vec4 (0);

	if (diffuseFactor > 0) {
		diffuseColor = vec4 (base.color, 1.0) * base.intensity * diffuseFactor;

		vec3 directionToEye = normalize (eye_pos - f_pos);
		//vec3 reflectDirection = normalize (reflect (direction, normal));
		vec3 halfDir = normalize (directionToEye - direction);

		float specularFactor = dot (halfDir, normal);
		//float specularFactor = dot (directionToEye, reflectDirection);
		specularFactor = pow (specularFactor, material.specularPower);

		specularFactor = cookTorranceSpecular (-direction, directionToEye, normal, .5, 1);

		if (specularFactor > 0)
			specularColor = vec4 (base.color, 1.0) * material.specularIntensity * specularFactor;
	}

	return diffuseColor + specularColor;
}

vec4 calcDirectionalLigh (DirectionalLight light, vec3 normal) {
	return calcLight (light.base, -light.direction, normal);
}

const float gamma = 2.2;

float toLinear(float v) {
  return pow(v, gamma);
}

vec2 toLinear(vec2 v) {
  return pow(v, vec2(gamma));
}

vec3 toLinear(vec3 v) {
  return pow(v, vec3(gamma));
}

vec4 toLinear(vec4 v) {
  return vec4(toLinear(v.rgb), v.a);
}

float toGamma(float v) {
  return pow(v, 1.0 / gamma);
}

vec2 toGamma(vec2 v) {
  return pow(v, vec2(1.0 / gamma));
}

vec3 toGamma(vec3 v) {
  return pow(v, vec3(1.0 / gamma));
}

vec4 toGamma(vec4 v) {
  return vec4(toGamma(v.rgb), v.a);
}

void main () {
	vec4 color = toLinear(material.color * texture (material.diffuse, f_tex) * calcDirectionalLigh (directionalLight, normalize (f_nrm)));

	frag_color = toGamma (color);
}