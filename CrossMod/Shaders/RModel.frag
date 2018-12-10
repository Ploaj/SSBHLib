#version 330

in vec3 N;
in vec3 tangent;
in vec3 bitangent;
in vec2 UV0;
in vec4 colorSet;
in vec3 bakeColor;
noperspective in vec3 edgeDistance;

uniform sampler2D colMap;
uniform sampler2D col2Map;
uniform sampler2D prmMap;
uniform sampler2D norMap;
uniform sampler2D emiMap;
uniform sampler2D bakeLitMap;
uniform sampler2D gaoMap;

uniform sampler2D iblLut;

uniform samplerCube diffusePbrCube;
uniform samplerCube specularPbrCube;

uniform int renderDiffuse;
uniform int renderSpecular;
uniform int renderWireframe;
uniform int useDittoForm;

uniform vec4 paramA6;
uniform vec4 param98;

uniform mat4 mvp;

out vec4 fragColor;

// Defined in Wireframe.frag.
float WireframeIntensity(vec3 distanceToEdges);

vec3 GetBumpMapNormal(vec3 N, vec4 norColor)
{
	// Calculate the resulting normal map.
	// TODO: Proper calculation of B channel.
	vec3 normalMapColor = vec3(norColor.rg, 1);

	// Remap the normal map to the correct range.
	vec3 normalMapNormal = 2.0 * normalMapColor - vec3(1);

	// TBN Matrix.
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
    fragColor = vec4(0, 0, 0, 1);

	vec4 norColor = texture(norMap, UV0).xyzw;
	vec3 newNormal = GetBumpMapNormal(N, norColor);

	vec3 V = vec3(0,0,-1) * mat3(mvp);
	vec3 R = reflect(V, newNormal);

	// BLend two diffuse layers based on alpha.
    // The second layer is set using the first layer if not present.
	vec4 albedoColor = texture(colMap, UV0).rgba;
    vec4 albedoColor2 = texture(col2Map, UV0).rgba;
    albedoColor.rgb = mix(albedoColor.rgb, albedoColor2.rgb, albedoColor2.a);
    // Ditto color in linear gamma.
    if (useDittoForm == 1)
        albedoColor.rgb = vec3(0.302, 0.242, 0.374);

	vec4 prmColor = texture(prmMap, UV0).xyzw;
    if (useDittoForm == 1)
        prmColor = vec4(1, 0.255, 1, 1);

	float directLightIntensity = 1.25;

	// Invert glossiness?
	float roughness = clamp(1 - prmColor.g, 0, 1);

	// Image based lighting.
	vec3 diffuseIbl = textureLod(diffusePbrCube, N, 0).rrr * 2.5;
	int maxLod = 10;
	vec3 specularIbl = textureLod(specularPbrCube, R, roughness * maxLod).rrr * 2.5;

	float metalness = prmColor.r;

	fragColor = vec4(0, 0, 0, 1);

	float maxF0Dialectric = 0.08;
	vec3 f0 = mix(prmColor.aaa * maxF0Dialectric, albedoColor.rgb, metalness);
	vec3 kSpecular = FresnelSchlickRoughness(max(dot(newNormal, V), 0.0), f0, roughness);

	// Diffuse
    // TODO: Causes discoloration.
	vec3 kDiffuse = (1 - kSpecular.rrr);
    //kDiffuse *= (1 - metalness); // TODO: Doesn't look correct.
	vec3 diffuseLight = diffuseIbl;

	// Direct lighting.
	diffuseLight += LambertShading(newNormal, V) * directLightIntensity;

	vec3 diffuseTerm = kDiffuse * albedoColor.rgb * diffuseLight;

	// Bake lighting maps.
	diffuseTerm *= texture(bakeLitMap, bakeColor.xy).rgb;
    diffuseTerm *= texture(gaoMap, bakeColor.xy).rgb;

	fragColor.rgb += diffuseTerm * renderDiffuse;

    float rimLight = (1 - max(dot(newNormal, V), 0));
    fragColor.rgb += paramA6.rgb * pow(rimLight, 3) * specularIbl * 0.5;

	fragColor.a = albedoColor.a;

    // TODO: 0 = alpha. 1 = alpha.
    // Values can be between 0 and 1, however.
    fragColor.a += param98.x;

	// Specular calculations adapted from https://learnopengl.com/PBR/IBL/Specular-IBL
	vec2 brdf  = texture(iblLut, vec2(max(dot(N, V), 0.0), roughness)).rg;
	vec3 specularTerm = specularIbl * (kSpecular * brdf.x + brdf.y);

	// Direct lighting.
	specularTerm += GgxShading(newNormal, V, roughness + 0.25) * directLightIntensity;

    // Cavity Map used for specular occlusion.
	specularTerm.rgb *= norColor.aaa;

	fragColor.rgb += specularTerm * kSpecular * renderSpecular;

	// Ambient Occlusion
	fragColor.rgb *= prmColor.b;

	// Emission
	fragColor.rgb += texture(emiMap, UV0).rgb * 3.5;

	// Gamma correction.
	fragColor.rgb = GetSrgb(fragColor.rgb);

	if (renderWireframe == 1)
	{
		vec3 edgeColor = vec3(1);
		float intensity = WireframeIntensity(edgeDistance);
		fragColor.rgb = mix(fragColor.rgb, edgeColor, intensity);
	}
}
