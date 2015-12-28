#version 450
in vec2 f_tex;

uniform sampler2D u_diffuse;
uniform vec3 ambient_color;

out vec4 frag_color;

vec4 lineartoSRGB(const in vec4 c) {
    vec3 S1 = sqrt(c.rgb);
    vec3 S2 = sqrt(S1);
    vec3 S3 = sqrt(S2);
    return vec4(0.662002687 * S1 + 0.684122060 * S2 - 0.323583601 * S3 - 0.0225411470 * c.rgb, c.a);
}

void main () {
	vec3 diffuseColor = texture (u_diffuse, f_tex).rgb;
	frag_color = lineartoSRGB(vec4(diffuseColor * ambient_color, 1.0));
}