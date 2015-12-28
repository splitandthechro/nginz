using nginz.Lighting;
using OpenTK;

namespace nginz {
	public static class LightingExt {
		public static void SetBaseLight (this ShaderProgram shader, string name, BaseLight light) {
			shader[name + ".color"] = light.Color;
			shader[name + ".intensity"] = light.Intensity;
		}

		public static void SetAttenuation (this ShaderProgram shader, string name, Attenuation atten) {
			shader[name + ".constant"] = atten.constant;
			shader[name + ".linear"] = atten.linear;
			shader[name + ".exponent"] = atten.exponent;
		}

		public static void SetDirectionalLight (this ShaderProgram shader, string name, DirectionalLight light) {
			shader.SetBaseLight (name + ".base", light.@base);
			shader[name + ".direction"] = light.direction;
		}

		public static void SetPointLight (this ShaderProgram shader, string name, PointLight light) {
			shader.SetBaseLight (name + ".base", light.@base);
			shader.SetAttenuation (name + ".atten", light.atten);
			shader[name + ".position"] = light.position;
			shader[name + ".range"] = light.range;
		}

		public static void SetMaterial (this ShaderProgram shader, string name, Material material) {
			shader[name + ".color"] = material.Color;
			shader[name + ".diffuse"] = 0;
			shader[name + ".normal"] = 1;
			shader[name + ".specularIntensity"] = material.SpecularIntensity;
			shader[name + ".specularPower"] = material.SpecularPower;
		}
	}
}
