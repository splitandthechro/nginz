#version 450
in vec2 f_tex;

uniform sampler2D u_diffuse;
uniform sampler2D u_normal;
uniform sampler2D u_specular;
uniform sampler2D u_depth;

uniform mat4 inverseCamera;

out vec4 frag_color;

float LinearizeDepth (vec2 uv) {
  float n = 0.01f; // camera z near
  float f = 64.0f; // camera z far
  float z = texture2D (u_depth, uv).x;
  return (2.0 * n) / (f + n - z * (f - n));	
}

vec3 calcPositionFromDepth (float depth) {
	float x_hs = 2.0 * f_tex.x - 1.0;
	float y_hs = 2.0 * f_tex.y - 1.0;
	float z_hs = 2.0 * depth - 1.0;

	vec4 position_hs = vec4 (x_hs, y_hs, z_hs, 1.0);
	vec4 position_ws = inverseCamera * position_hs;

	return position_ws.xyz / position_ws.w;
}

vec4 lineartoSRGB(const in vec4 c) {
    vec3 S1 = sqrt(c.rgb);
    vec3 S2 = sqrt(S1);
    vec3 S3 = sqrt(S2);
    return vec4(0.662002687 * S1 + 0.684122060 * S2 - 0.323583601 * S3 - 0.0225411470 * c.rgb, c.a);
}

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

struct BaseLight {
	vec3 color;
	float intensity;
};

struct DirectionalLight {
	BaseLight base;
	vec3 direction;
};

uniform vec3 eye_pos;
uniform DirectionalLight directionalLight;

vec4 calcLight (BaseLight base, vec3 direction, vec3 normal, float specularPower, float specularIntensity, vec3 pos) {
	float diffuseFactor = dot (normal, -direction);

	vec4 diffuseColor = vec4 (0);
	vec4 specularColor = vec4 (0);

	if (diffuseFactor > 0) {
		diffuseColor = vec4 (base.color, 1.0) * base.intensity * diffuseFactor;

		vec3 directionToEye = normalize (eye_pos - pos);
		//vec3 reflectDirection = normalize (reflect (direction, normal));
		vec3 halfDir = normalize (directionToEye - direction);

		float specularFactor = dot (halfDir, normal);
		//float specularFactor = dot (directionToEye, reflectDirection);
		specularFactor = pow (specularFactor, specularPower);

		specularFactor = cookTorranceSpecular (-direction, directionToEye, normal, .5, 1);

		if (specularFactor > 0)
			specularColor = vec4 (base.color, 1.0) * specularIntensity * specularFactor;
	}

	return diffuseColor + specularColor;
}

vec4 calcDirectionalLigh (DirectionalLight light, vec3 normal, float specularPower, float specularIntensity, vec3 pos) {
	return calcLight (light.base, -light.direction, normal, specularPower, specularIntensity, pos);
}

vec3 decode (vec4 enc)
{
    vec4 nn = enc*vec4(2,2,0,0) + vec4(-1,-1,1,-1);
    float l = dot(nn.xyz,-nn.xyw);
    nn.z = l;
    nn.xy *= sqrt(l);
    return nn.xyz * 2 + vec3(0,0,-1);
}

void main () {
	vec3 diffuseColor = texture (u_diffuse, f_tex).rgb;
	vec3 normalEncoded = texture2D (u_normal, f_tex).xyz;
	vec3 specularColor = texture2D (u_specular, f_tex).rgb;
	float depth = texture2D (u_depth, f_tex).r;

	vec3 position = calcPositionFromDepth (depth);
	vec3 normal = decode (vec4(normalEncoded, 1.0));

	//frag_color = lineartoSRGB(vec4(diffuseColor, 1.0));
	frag_color = vec4(diffuseColor, 1.0) * calcDirectionalLigh (directionalLight, normal, specularColor.x * 32, specularColor.y * 32, position);
}