using nginz.Lighting;
using OpenTK;

namespace nginz {
	public static class LightingExt {
		public static void SetBaseLight (this ShaderProgram shader, string name, BaseLight light) {
			shader[name + ".color"] = light.Color;
			shader[name + ".intensity"] = light.Intensity;
		}

		public static void SetDirectionalLight (this ShaderProgram shader, string name, DirectionalLight light) {
			shader.SetBaseLight (name + ".base", light.@base);
			shader[name + ".direction"] = light.direction;
		}

		public static void SetMaterial (this ShaderProgram shader, string name, Material material) {
			shader[name + ".color"] = material.Color;
			shader[name + ".diffuse"] = 0;
			shader[name + ".specularIntensity"] = material.SpecularIntensity;
			shader[name + ".specularPower"] = material.SpecularPower;
		}
	}
}
