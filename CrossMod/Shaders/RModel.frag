#version 330

in vec3 N;
in vec3 tangent;
in vec2 UV0;

out vec4 fragColor;

uniform sampler2D colMap;
uniform sampler2D prmMap;
uniform sampler2D norMap;

uniform samplerCube diffusePbrCube;
uniform samplerCube specularPbrCube;

uniform mat4 mvp;

vec3 GetBumpMapNormal(vec3 N)
{
	// Calculate the resulting normal map.
	// TODO: Proper calculation of B channel.
	vec3 normalMapColor = vec3(texture(norMap, UV0).rg, 1);

	// Remap the normal map to the correct range.
	vec3 normalMapNormal = 2.0 * normalMapColor - vec3(1);

	// TBN Matrix.
	vec3 bitangent = cross(N, tangent);
	mat3 tbnMatrix = mat3(tangent, bitangent, N);

	vec3 newNormal = tbnMatrix * normalMapNormal;
	return normalize(newNormal);
}

float LambertShading(vec3 N, vec3 V)
{
	float lambert = max(dot(N, V), 0);
	return lambert;
}

vec3 GetSrgb(vec3 linear)
{
	return pow(linear, vec3(0.4545));
}

vec3 FresnelSchlickRoughness(float cosTheta, vec3 F0, float roughness)
{
    return F0 + (max(vec3(1.0 - roughness), F0) - F0) * pow(1.0 - cosTheta, 5.0);
}

float GgxShading(vec3 N, vec3 H, float roughness)
{
	float a = roughness * roughness;
    float a2 = a * a;
    float nDotH = max(dot(N, H), 0.0);
    float nDotH2 = nDotH * nDotH;

    float numerator = a2;
    float denominator = (nDotH2 * (a2 - 1.0) + 1.0);
    denominator = 3.14159 * denominator * denominator;

    return numerator / denominator;
}

void main()
{
	vec3 newNormal = GetBumpMapNormal(N);
	vec3 V = vec3(0,0,-1) * mat3(mvp);
	vec3 R = reflect(V, newNormal);

	// TODO: Accessing unitialized textures may cause crashes.
	vec4 albedoColor = texture(colMap, UV0).rgba;
	vec4 prmColor = texture(prmMap, UV0).xyzw;
	vec4 norColor = texture(norMap, UV0).xyzw;

	// Invert glossiness?
	float roughness = clamp(1 - prmColor.g, 0, 1);

	// Image based lighting.
	vec3 diffuseIbl = textureLod(diffusePbrCube, R, 0).rgb * 1.75;
	int maxLod = 10;
	vec3 specularIbl = textureLod(specularPbrCube, R, roughness * maxLod).rgb * 1.75;

	float metalness = prmColor.r;

	// Diffuse
	fragColor = albedoColor;
	// fragColor.rgb *= LambertShading(newNormal, V);
	fragColor.rgb *= diffuseIbl;
	// fragColor.rgb *= (1 - metalness); // TODO: Doesn't work for skin.

	vec3 f0 = mix(prmColor.aaa, albedoColor.rgb, metalness);
	vec3 kSpecular = FresnelSchlickRoughness(max(dot(newNormal, V), 0.0), f0, roughness);
	// fragColor.rgb += GgxShading(newNormal, V, roughness) * kSpecular;
	// TODO: Use proper GGX shading.
	fragColor.rgb += specularIbl * kSpecular;

	// Ambient Occlusion
	fragColor.rgb *= prmColor.b;

	fragColor.rgb = GetSrgb(fragColor.rgb);

}
