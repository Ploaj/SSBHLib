#version 330

in vec3 N;
in vec3 tangent;
in vec3 bitangent;
in vec2 UV0;
in vec4 colorSet;
in vec2 bake1;
noperspective in vec3 edgeDistance;

uniform sampler2D colMap;
uniform sampler2D col2Map;
uniform sampler2D prmMap;
uniform sampler2D norMap;
uniform sampler2D emiMap;
uniform sampler2D gaoMap;
uniform sampler2D bakeLitMap;

uniform sampler2D uvPattern;

uniform vec4 vec4Param;

uniform sampler2D iblLut;

uniform samplerCube diffusePbrCube;
uniform samplerCube specularPbrCube;

uniform vec4 renderChannels;
uniform int renderMode;

uniform int renderWireframe;
uniform int renderNormalMaps;

uniform mat4 mvp;

out vec4 fragColor;

// Defined in Wireframe.frag
float WireframeIntensity(vec3 distanceToEdges);

// Defined in NormalMap.frag.
vec3 GetBumpMapNormal(vec3 N, vec3 tangent, vec3 bitangent, vec4 norColor);

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
	vec4 norColor = texture(norMap, UV0).xyzw;
    vec3 newNormal = N;
    if (renderNormalMaps == 1)
        newNormal = GetBumpMapNormal(N, tangent, bitangent, norColor);

	vec3 V = vec3(0,0,-1) * mat3(mvp);
	vec3 R = reflect(V, newNormal);

    // BLend two diffuse layers based on alpha.
    // The second layer is set using the first layer if not present.
	vec4 albedoColor = texture(colMap, UV0).rgba;
    vec4 albedoColor2 = texture(col2Map, UV0).rgba;
    albedoColor.rgb = mix(albedoColor.rgb, albedoColor2.rgb, albedoColor2.a);
	vec4 prmColor = texture(prmMap, UV0).xyzw;

	vec4 emiColor = texture(emiMap, UV0).rgba;

	// TODO: Why does bake1 have UVs?
	vec4 bakeLitColor = texture(bakeLitMap, bake1).rgba;
    vec4 gaoColor = texture(gaoMap, bake1).rgba;

	// Invert glossiness?
	float roughness = clamp(1 - prmColor.g, 0, 1);

    vec4 uvPatternColor = texture(uvPattern, UV0).rgba;

	// Image based lighting.
	vec3 diffuseIbl = textureLod(diffusePbrCube, R, 0).rgb * 2.5;
	int maxLod = 10;
	vec3 specularIbl = textureLod(specularPbrCube, R, roughness * maxLod).rgb * 2.5;

	float metalness = prmColor.r;

	// Just gamma correct albedo maps.
	fragColor = vec4(1);
	switch (renderMode)
	{
		case 1:
			fragColor = albedoColor;
			fragColor.rgb = GetSrgb(fragColor.rgb);
			break;
		case 2:
			fragColor = prmColor;
			break;
		case 3:
			fragColor = norColor;
			break;
		case 4:
			fragColor = emiColor;
			fragColor.rgb = GetSrgb(fragColor.rgb);
			break;
		case 5:
			fragColor = bakeLitColor;
			fragColor.rgb = GetSrgb(fragColor.rgb);
			break;
        case 6:
            fragColor = gaoColor;
            fragColor.rgb = GetSrgb(fragColor.rgb);
            break;
		case 7:
			fragColor = colorSet;
            // The default range is  0 to 0.5 (0 to 128).
            fragColor.rgb *= 2;
			break;
		case 8:
			fragColor = vec4(newNormal * 0.5 + 0.5, 1);
			break;
		case 9:
			fragColor = vec4(tangent * 0.5 + 0.5, 1);
			break;
        case 10:
            fragColor = vec4(bitangent * 0.5 + 0.5, 1);
            break;
		case 11:
			fragColor = vec4(bake1, 1, 1);
			break;
        case 12:
            fragColor = uvPatternColor;
            break;
		case 13:
			fragColor = vec4Param;
			break;
		default:
			fragColor = vec4(0, 0, 0, 1);
			break;
	}

    fragColor.rgb *= renderChannels.rgb;
    if (renderChannels.r == 1 && renderChannels.g == 0 && renderChannels.b == 0)
        fragColor.rgb = fragColor.rrr;
    else if (renderChannels.g == 1 && renderChannels.r == 0 && renderChannels.b == 0)
        fragColor.rgb = fragColor.ggg;
    else if (renderChannels.b == 1 && renderChannels.r == 0 && renderChannels.g == 0)
        fragColor.rgb = fragColor.bbb;

    if (renderChannels.a == 1 && renderChannels.r == 0 && renderChannels.g == 0 && renderChannels.b == 0)
        fragColor = vec4(fragColor.aaa, 1);

	// Don't use alpha blending with debug shading.
	fragColor.a = 1;

	if (renderWireframe == 1)
	{
		vec3 edgeColor = vec3(1);
		float intensity = WireframeIntensity(edgeDistance);
		fragColor.rgb = mix(fragColor.rgb, edgeColor, intensity);
	}
}
