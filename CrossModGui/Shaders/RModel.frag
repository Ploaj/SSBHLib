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

uniform sampler2D prmMap;
uniform sampler2D norMap;
uniform sampler2D emiMap;
uniform sampler2D emi2Map;
uniform sampler2D bakeLitMap;
uniform sampler2D gaoMap;

uniform sampler2D inkNorMap;

uniform samplerCube difCubeMap;

uniform sampler2D difMap;
uniform sampler2D dif2Map;
uniform sampler2D dif3Map;

uniform sampler2D iblLut;

uniform samplerCube diffusePbrCube;
uniform samplerCube specularPbrCube;

uniform int renderDiffuse;
uniform int renderSpecular;
uniform int renderEmission;
uniform int renderRimLighting;
uniform int renderExperimental;

uniform int renderWireframe;
uniform int renderVertexColor;
uniform int renderNorMaps;
uniform int renderPrmMetalness;
uniform int renderPrmRoughness;
uniform int renderPrmAo;
uniform int renderPrmSpec;

uniform float floatTestParam;

uniform float depthBias;

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

    int hasColorSet1;
    int hasColorSet2;
    int hasColorSet3;
    int hasColorSet4;
    int hasColorSet5;
    int hasColorSet6;
    int hasColorSet7;

    int isValidShaderLabel;
};

// TODO: Add lighting vectors to a uniform block.
// Values taken from the training stage light.nuanmb.
float LightCustomFloat0 = 4.0;
vec4 LightCustomVector0 = vec4(1,1,1,0); 
vec4 LightCustomVector8 = vec4(1.5,1.5,1.5,1); 

uniform vec3 chrLightDir;

uniform mat4 mvp;
uniform mat4 modelViewMatrix;
uniform vec3 cameraPos;

out vec4 fragColor0;

uniform float directLightIntensity;
uniform float iblIntensity;

uniform int enableBloom;
uniform float bloomIntensity;

// Defined in Wireframe.frag.
float WireframeIntensity(vec3 distanceToEdges);

// Defined in NormalMap.frag.
vec3 GetBitangent(vec3 normal, vec3 tangent, float tangentSign);
vec3 GetBumpMapNormal(vec3 normal, vec3 tangent, vec3 bitangent, vec4 norColor);

// Defined in Gamma.frag.
vec3 GetSrgb(vec3 linear);

// Schlick fresnel approximation.
vec3 FresnelSchlick(float cosTheta, vec3 F0)
{
    return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
} 

// Ultimate shaders use a schlick geometry masking term.
// http://cwyman.org/code/dxrTutors/tutors/Tutor14/tutorial14.md.html
float SchlickMaskingTerm(float nDotL, float nDotV, float a2) 
{
    // TODO: Double check this masking term.
    float k = a2 * 0.5;
    float gV = nDotV / (nDotV * (1.0 - k) + k);
    float gL = nDotL / (nDotL * (1.0 - k) + k);
    return gV * gL;
}

// Ultimate shaders use a mostly standard GGX BRDF for specular.
// http://graphicrants.blogspot.com/2013/08/specular-brdf-reference.html
float Ggx(float nDotH, float nDotL, float nDotV, float roughness)
{
    // Clamp to 0.01 to prevent divide by 0.
    float a = max(roughness, 0.01) * max(roughness, 0.01);
    float a2 = a*a;
    const float PI = 3.14159;
    float nDotH2 = nDotH * nDotH;

    float denominator = ((nDotH2) * (a2 - 1.0) + 1.0);
    float specular = a2 / (PI * denominator * denominator);
    float shadowing = SchlickMaskingTerm(nDotL, nDotV, a2);
    // TODO: double check the denominator
    return specular * shadowing / 3.141519;
}

// A very similar BRDF as used for GGX.
float GgxAnisotropic(float nDotH, vec3 H, vec3 tangent, vec3 bitangent, float roughness, float anisotropy)
{
    // TODO: How much of this is shared with GGX?
    // Clamp to 0.01 to prevent divide by 0.
    float roughnessX = max(roughness * anisotropy, 0.01);
    float roughnessY = max(roughness / anisotropy, 0.01);

    float roughnessX4 = pow(roughnessX, 4.0);
    float roughnessY4 = pow(roughnessY, 4.0);

    float xDotH = dot(bitangent, H);
    float xTerm = (xDotH * xDotH) / roughnessX4;

    float yDotH = dot(tangent, H);
    float yTerm = (yDotH * yDotH) / roughnessY4;

    // TODO: Check this section of code.
    float nDotH2 = nDotH * nDotH;
    float denominator = xTerm + yTerm + nDotH2;

    // TODO: Is there a geometry term for anisotropic?
    float normalization = (3.14159 * roughnessX * roughnessY);
    return 1.0 / (normalization * denominator * denominator);
}

// Defined in TextureLayers.frag.
vec4 GetEmissionColor(vec2 uv1, vec2 uv2, vec4 transform1, vec4 transform2);
vec4 GetAlbedoColor(vec2 uv1, vec2 uv2, vec2 uv3, vec3 R, vec4 transform1, vec4 transform2, vec4 transform3, vec4 colorSet5);
vec3 GetAlbedoColorFinal(vec4 albedoColor, float metalness);

vec3 GetDiffuseLighting(float nDotL, vec3 ambientIbl, vec3 ao, float sssBlend)
{
    // Diffuse shading is remapped to be softer.
    // Multiplying be a constant and clamping affects the "smoothness".
    float directShading = nDotL;
    if (hasCustomVector11 == 1)
    {
        float skinShading = nDotL;
        skinShading *= CustomVector[30].y;
        skinShading = skinShading * 0.5 + 0.5;
        directShading = mix(directShading, skinShading, sssBlend);
    }
    directShading = clamp(directShading, 0.0, 1.0);

    vec4 bakedLitColor = texture(bakeLitMap, bake1).rgba;

    vec3 directLight = LightCustomVector0.xyz * directShading * LightCustomFloat0 * bakedLitColor.a;

    // Baked lighting maps are not affected by ambient occlusion.
    vec3 ambientLight = ambientIbl * ao;
    ambientLight += bakedLitColor.rgb * 8.0;

    vec3 result = directLight * directLightIntensity + ambientLight;

    // Baked stage lighting.
    if (renderVertexColor == 1 && hasColorSet2 == 1)
        result *= colorSet2.rgb;

    return result;
}

float SpecularBrdf(float nDotH, float nDotL, float nDotV, vec3 halfAngle, vec3 bitangent, float roughness)
{
    // The two BRDFs look very different so don't just use anisotropic for everything.
    if (hasCustomFloat10 == 1)
        return GgxAnisotropic(nDotH, halfAngle, tangent.xyz, bitangent, roughness, CustomFloat[10].x);
    else
        return Ggx(nDotH, nDotL, nDotV, roughness);
}

vec3 SpecularTerm(float nDotH, float nDotL, float nDotV, vec3 halfAngle, vec3 bitangent, float roughness, vec3 specularIbl, float metalness)
{
    vec3 directSpecular = LightCustomVector0.xyz * LightCustomFloat0 * SpecularBrdf(nDotH, nDotL, nDotV, halfAngle, bitangent, roughness) * directLightIntensity;
    vec3 indirectSpecular = specularIbl;
    vec3 specularTerm = (directSpecular * CustomBoolean[3].x) + (indirectSpecular * CustomBoolean[4].x);

    return specularTerm;
}

vec3 EmissionTerm(vec4 emissionColor)
{
    return emissionColor.rgb * CustomVector[3].rgb;
}

float GetF0FromIor(float ior)
{
    return pow((1.0 - ior) / (1.0 + ior), 2.0);
}

float Luminance(vec3 rgb)
{
    const vec3 W = vec3(0.2125, 0.7154, 0.0721);
    return dot(rgb, W);
}

vec3 GetSpecularWeight(float f0, vec3 diffusePass, float metalness, float nDotV, float roughness)
{
    // Metals use albedo instead of the specular color/tint.
    vec3 specularReflectionF0 = vec3(f0);
    vec3 f0Final = mix(specularReflectionF0, diffusePass, metalness);
    return FresnelSchlick(nDotV, f0Final);
}

vec3 GetRimBlend(vec3 baseColor, vec3 diffusePass, float nDotV)
{
    vec3 rimColor = CustomVector[14].rgb * LightCustomVector8.rgb;

    // TODO: Black edges for large blend values?
    // Edge tint.
    rimColor *= clamp(mix(vec3(1.0), diffusePass, CustomFloat[8].x), 0.0, 1.0);

    // TODO: There some sort of directional lighting that controls the intensity of this effect.
    // This appears to be lighting done in the vertex shader.

    // TODO: How is the overall intensity controlled?
    // Hardcoded shader constant.
    float rimIntensity = 0.2125999927520752; 

    float fresnel = pow(1 - nDotV, 5.0);
    float rimBlend = fresnel * LightCustomVector8.w * CustomVector[14].w * 0.6;

    return mix(baseColor, rimColor, clamp(rimBlend, 0.0, 1.0));
}

float RoughnessToLod(float roughness)
{
    // Adapted from decompiled shader source.
    // Applies a curves adjustment to roughness.
    // Clamp roughness to avoid divide by 0.
    float roughnessClamped = max(roughness, 0.01);
    float a = (roughnessClamped * roughnessClamped);
    return log2((1.0 / a) * 2.0 - 2.0) * -0.4545 + 4.0;
}

vec3 GetInvalidShaderLabelColor()
{
    // TODO: Account for screen resolution and use the values from in game for scaling.
    // TODO: Add proper bloom.
    vec3 screenPosition = gl_FragCoord.xyz;
    float checkSize = 0.15;
    float checkerBoard = mod(floor(screenPosition.x * checkSize) + floor(screenPosition.y * checkSize), 2.0);
    float checkerBoardFinal = max(sign(checkerBoard), 0.0);
    return vec3(mix(0.8,1.0,checkerBoardFinal), 0.0, 0.0);
}

float GetAngleFade(float nDotV, float ior, float specularf0) 
{
    // CustomFloat19 defines the IOR for a separate fresnel based fade.
    // The specular f0 value is used to set the minimum opacity.
    float f0AngleFade = GetF0FromIor(ior + 1.0);
    float facingRatio = FresnelSchlick(nDotV, vec3(f0AngleFade)).x;
    return max(facingRatio, specularf0);
}

vec3 GetBloomBrightColor(vec3 color0)
{
    // Ported bloom code.
    // TODO: Where do the uniform buffer values come from?
    float componentMax = max(max(color0.r, max(color0.g, color0.b)), 0.001);
    float scale = 1 / componentMax;
    float scale2 = max(0.925 * -0.5 + componentMax, 0.0);
    return color0.rgb * scale * scale2 * 6.0;
}

float GetF0FromSpecular(float specular) 
{
    // Specular gets remapped from [0.0,1.0] to [0.0,0.2].
    // The value is 0.16*0.2 = 0.032 if the PRM alpha is ignored.
    if (CustomBoolean[1].x == 0)
        specular = 0.16;

    return specular * 0.2;
}

void main()
{
    // TODO: Organize this code.
    fragColor0 = vec4(0.0, 0.0, 0.0, 1.0);
    if (isValidShaderLabel != 1) {
        fragColor0.rgb = GetInvalidShaderLabelColor();
        return;
    }

    vec4 norColor = texture(norMap, map1).xyzw;
    if (hasInkNorMap == 1)
        norColor.xyz = texture(inkNorMap, map1).rga;

    vec3 bitangent = GetBitangent(vertexNormal, tangent.xyz, tangent.w);

    vec3 fragmentNormal = normalize(vertexNormal);
    if (renderNorMaps == 1)
        fragmentNormal = GetBumpMapNormal(vertexNormal, tangent.xyz, bitangent, norColor);

    vec3 viewVector = normalize(cameraPos - position);

    // A hack to ensure backfaces render the same as front faces.
    // TODO: Does the game actually do this?
    if (dot(viewVector, fragmentNormal) < 0.0)
        fragmentNormal *= -1.0;

    // Shading vectors.
    vec3 reflectionVector = reflect(viewVector, fragmentNormal);
    reflectionVector.y *= -1;
    vec3 halfAngle = normalize(chrLightDir.xyz + viewVector);
    float nDotV = max(dot(fragmentNormal, viewVector), 0.0);
    float nDotH = max(dot(fragmentNormal, halfAngle), 0.0);
    // Don't clamp to allow remapping the range of values later.
    float nDotL = dot(fragmentNormal, chrLightDir.xyz);

    // Get texture color.
    vec4 albedoColor = GetAlbedoColor(map1, uvSet, uvSet, reflectionVector, CustomVector[6], CustomVector[31], CustomVector[32], colorSet5);
    vec4 emissionColor = GetEmissionColor(map1, uvSet, CustomVector[6], CustomVector[31]);
    // TODO: Mega man's eyes?.
    // if (CustomBoolean11 == 0)
    //     emissionColor.rgb *= (1 - texture(col2Map, uvSet).a);

    vec4 prmColor = texture(prmMap, map1).xyzw;

    // Override the PRM color with default texture colors if disabled.
    if (renderPrmMetalness != 1)
        prmColor.r = 0.0;
    if (renderPrmRoughness != 1)
        prmColor.g = 1.0;
    if (renderPrmAo != 1)
        prmColor.b = 1.0;
    if (renderPrmSpec != 1)
        prmColor.a = 0.5;

    // Probably some sort of override for PRM color.
    if (hasCustomVector47 == 1)
        prmColor = CustomVector[47];

    fragColor0.a = max(albedoColor.a * emissionColor.a, CustomVector[0].x);
    // Alpha testing.
    // TODO: Not all shaders have this.
    if (fragColor0.a < 0.5 && hasCustomFloat19 != 1)
        discard;

    float roughness = prmColor.g;
    float metalness = prmColor.r;
    // Specular isn't effected by metalness for skin materials.
    if (hasCustomVector11 == 1)
        metalness = 0.0;

    // TODO: Is specular overridden by default?
    float specularF0 = GetF0FromSpecular(prmColor.a);

    float specularOcclusion = norColor.a;
    // These materials don't have a nor map.
    if (hasCustomBoolean1 == 0)
        specularOcclusion *= prmColor.a;

    vec3 ambientOcclusion = vec3(prmColor.b);
    ambientOcclusion *= pow(texture(gaoMap, bake1).rgb, vec3(CustomFloat[1] + 1.0));

    // Image based lighting.
    // The texture is currently using exported values, 
    // so multiply by 0.5 to fix the intensity.
    // TODO: Use an existing cube map.
    int maxLod = 6;
    float specularLod = RoughnessToLod(roughness);
    vec3 specularIbl = textureLod(specularPbrCube, reflectionVector, specularLod).rgb * iblIntensity * 0.5;
    
    // TODO: This should be an irradiance map.
    // TODO: Models with no irradiance map use a vertex attribute?
    vec3 diffuseIbl = textureLod(diffusePbrCube, fragmentNormal, 0).rgb * iblIntensity; 

    // Render passes.
    float sssBlend = prmColor.r * CustomVector[30].x;
    vec3 albedoColorFinal = GetAlbedoColorFinal(albedoColor, prmColor.r);

    vec3 diffuseLight = GetDiffuseLighting(nDotL, diffuseIbl, ambientOcclusion, sssBlend);

    vec3 specularPass = SpecularTerm(nDotH, max(nDotL, 0.0), nDotV, halfAngle, bitangent, roughness, specularIbl, metalness);

    vec3 kSpecular = GetSpecularWeight(specularF0, albedoColorFinal.rgb, metalness, nDotV, roughness);
    vec3 kDiffuse = max((vec3(1.0) - kSpecular) * (1.0 - metalness), 0.0);

    // Color Passes.
    if (renderDiffuse == 1)
        fragColor0.rgb += albedoColorFinal * diffuseLight * kDiffuse / 3.14159;

    if (renderSpecular == 1)
        fragColor0.rgb += specularPass * kSpecular * ambientOcclusion * specularOcclusion;

    if (renderEmission == 1)
        fragColor0.rgb += EmissionTerm(emissionColor);

    if (renderRimLighting == 1)
        fragColor0.rgb = GetRimBlend(fragColor0.rgb, albedoColorFinal, nDotV);

    // Final color multiplier.
    fragColor0.rgb *= CustomVector[8].rgb;

    if (renderVertexColor == 1)
    {
        if (hasColorSet1 == 1)
            fragColor0 *= colorSet1; 

        if (hasColorSet3 == 1)
            fragColor0 *= colorSet3;
    }

    if (hasCustomFloat19 == 1 && renderExperimental == 1)
        fragColor0.a = GetAngleFade(nDotV, CustomFloat[19].x, specularF0);

    // Premultiplied alpha. 
    // fragColor0.a = clamp(fragColor0.a, 0.0, 1.0); // TODO: krool???
    fragColor0.rgb *= fragColor0.a;

    // TODO: Move this to post-processing.
    // This is a temporary workaround for FBOs not working on Intel.
    if (enableBloom == 1)
        fragColor0.rgb += GetBloomBrightColor(fragColor0.rgb) * bloomIntensity;

    // Gamma correction.
    fragColor0.rgb = GetSrgb(fragColor0.rgb);

    if (renderWireframe == 1)
    {
        vec3 edgeColor = vec3(1);
        float intensity = WireframeIntensity(edgeDistance);
        fragColor0.rgb = mix(fragColor0.rgb, edgeColor, intensity);
    }
}
