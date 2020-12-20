#version 330

in vec3 position;
in vec3 vertexNormal;
in vec4 tangent;
in vec2 map1;
in vec2 uvSet;
in vec2 uvSet1;
in vec2 uvSet2;
in vec4 colorSet1;
in vec4 colorSet2;
in vec4 colorSet3;
in vec4 colorSet5;
in vec2 bake1;

noperspective in vec3 edgeDistance;

uniform sampler2D colMap;
uniform sampler2D col2Map;

uniform sampler2D projMap;
uniform sampler2D prmMap;
uniform sampler2D norMap;
uniform sampler2D emiMap;
uniform sampler2D emi2Map;
uniform sampler2D bakeLitMap;
uniform sampler2D gaoMap;
uniform sampler2D inkNorMap;

uniform vec3 materialId;

uniform samplerCube difCubeMap;

uniform sampler2D difMap;
uniform sampler2D dif2Map;
uniform sampler2D dif3Map;

uniform samplerCube diffusePbrCube;
uniform samplerCube specularPbrCube;

uniform vec4 renderChannels;
uniform int renderMode;
uniform int useUvPattern;

uniform int renderWireframe;
uniform int renderNorMaps;

uniform sampler2D uvPattern;

uniform MaterialParams
{
    vec4 CustomVector[64];
    ivec4 CustomBoolean[20];
    vec4 CustomFloat[20];

    vec4 vec4Param;

    int hasCustomVector11;
    int hasCustomVector47;
    int hasCustomVector44;
    int hasCustomFloat10;
    int hasCustomFloat19;
    int hasCustomBoolean1;

    int hasColMap; 
    int hasCol2Map; 
    int hasInkNorMap; 
    int hasDifCubeMap; 
    int hasDiffuse; 
    int hasDiffuse2;
    int hasDiffuse3; 
    int emissionOverride;
};

uniform mat4 mvp;
uniform mat4 modelView;
uniform vec3 cameraPos;

out vec4 fragColor;

// Defined in Wireframe.frag
float WireframeIntensity(vec3 distanceToEdges);

// Defined in NormalMap.frag.
vec3 GetBitangent(vec3 normal, vec3 tangent, float tangentSign);
vec3 GetBumpMapNormal(vec3 normal, vec3 tangent, vec3 bitangent, vec4 norColor);

// Defined in Gamma.frag.
vec3 GetSrgb(vec3 linear);

float LambertShading(vec3 N, vec3 V)
{
	float lambert = max(dot(N, V), 0.0);
	return lambert;
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

// Defined in TextureLayers.frag.
vec4 GetEmissionColor(vec2 uv1, vec2 uv2, vec4 transform1, vec4 transform2);
vec4 GetAlbedoColor(vec2 uv1, vec2 uv2, vec2 uv3, vec3 R, vec4 transform1, vec4 transform2, vec4 transform3, vec4 colorSet5);
vec3 GetAlbedoColorFinal(vec4 albedoColor, float metalness);

void main()
{
	vec4 norColor = texture(norMap, map1).xyzw;
    if (hasInkNorMap == 1)
        norColor.rgb = texture(inkNorMap, map1).rga;

    vec3 fragmentNormal = normalize(vertexNormal);
    vec3 bitangent = GetBitangent(vertexNormal.xyz, tangent.xyz, tangent.w);
    if (renderNorMaps == 1)
        fragmentNormal = GetBumpMapNormal(vertexNormal, tangent.xyz, bitangent, norColor);

    vec3 viewVector = normalize(cameraPos - position);

    vec3 reflectionVector = reflect(viewVector, fragmentNormal);
    reflectionVector.y *= -1;

    vec3 specularIbl = textureLod(specularPbrCube, reflectionVector, 0.0).rgb;

    // Get texture colors.
	vec4 albedoColor = GetAlbedoColor(map1, uvSet, uvSet, reflectionVector, CustomVector[6], CustomVector[31], CustomVector[32], colorSet5);
	vec4 prmColor = texture(prmMap, map1).xyzw;
	vec4 emiColor = GetEmissionColor(map1, uvSet, CustomVector[6], CustomVector[31]);
	vec4 bakeLitColor = texture(bakeLitMap, bake1).rgba;
    vec4 gaoColor = texture(gaoMap, bake1).rgba;
    vec4 projColor = texture(projMap, map1).rgba;

	fragColor = vec4(1);
	switch (renderMode)
	{
        case 1:
            fragColor.rgb = vec3(0.218) * max(dot(fragmentNormal, viewVector), 0.0);
            fragColor.rgb = GetSrgb(fragColor.rgb);
            break;
		case 2:
			fragColor = albedoColor;
			fragColor.rgb = GetSrgb(fragColor.rgb);
			break;
		case 3:
			fragColor = prmColor;
			break;
		case 4:
			fragColor = norColor;
			break;
		case 5:
			fragColor = emiColor;
			fragColor.rgb = GetSrgb(fragColor.rgb);
			break;
		case 6:
			fragColor = bakeLitColor;
			fragColor.rgb = GetSrgb(fragColor.rgb);
			break;
        case 7:
            fragColor = gaoColor;
            fragColor.rgb = GetSrgb(fragColor.rgb);
            break;
        case 8:
            fragColor = projColor;
            fragColor.rgb = GetSrgb(fragColor.rgb);
            break;
		case 9:
			fragColor = colorSet1;
			break;
        case 10:
			fragColor = colorSet2;
			break;
        case 11:
			fragColor = colorSet3;
			break;
		case 12:
            vec4 viewNormal = transpose(inverse(modelView)) * vec4(fragmentNormal, 0.0);
			fragColor = vec4(normalize(viewNormal.xyz) * 0.5 + 0.5, 1.0);
			break;
        case 13:
			fragColor = vec4(vertexNormal.xyz * 0.5 + 0.5, 1.0);
			break;
		case 14:
			fragColor = vec4(tangent.xyz * 0.5 + 0.5, tangent.w);
			break;
        case 15:
            fragColor = vec4(bitangent * 0.5 + 0.5, 1.0);
            break;
		case 16:
            if (useUvPattern == 1)
                fragColor = texture(uvPattern, map1).rgba;
            else
			    fragColor = vec4(map1, 1.0, 1.0);
			break;
        case 17:
            if (useUvPattern == 1)
                fragColor = texture(uvPattern, bake1).rgba;
            else
			    fragColor = vec4(bake1, 1.0, 1.0);
            break;
        case 18:
            if (useUvPattern == 1)
                fragColor = texture(uvPattern, uvSet).rgba;
            else
			    fragColor = vec4(uvSet, 1.0, 1.0);
            break;
        case 19:
            if (useUvPattern == 1)
                fragColor = texture(uvPattern, uvSet1).rgba;
            else
			    fragColor = vec4(uvSet1, 1.0, 1.0);
            break;
        case 20:
            if (useUvPattern == 1)
                fragColor = texture(uvPattern, uvSet2).rgba;
            else
			    fragColor = vec4(uvSet2, 1.0, 1.0);
            break;
		case 21:
			fragColor = vec4Param;
			break;
        case 22:
            fragColor = vec4(materialId, 1);
            break;
        case 23:
            vec3 albedoColorFinal = GetAlbedoColorFinal(albedoColor, prmColor.r);
            fragColor = vec4(GetSrgb(albedoColorFinal), 1.0);
            break;
        case 24:
            float anisotropyLines = (dot(viewVector, normalize(vertexNormal)) * 0.5 + 0.5) * abs(sin(map1.y * 150.0));
            fragColor = vec4(vec3(anisotropyLines), 1.0);
            break;
        case 25:
            fragColor = vec4(specularIbl, 1.0);
            break;
		default:
			fragColor = vec4(0.0, 0.0, 0.0, 1.0);
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
        fragColor = vec4(fragColor.aaa, 1.0);

	// Don't use alpha blending with debug shading.
	fragColor.a = 1.0;

	if (renderWireframe == 1)
	{
		vec3 edgeColor = vec3(1);
		float intensity = WireframeIntensity(edgeDistance);
		fragColor.rgb = mix(fragColor.rgb, edgeColor, intensity);
	}
}
