#version 330

in vec3 N;
in vec3 tangent;
in vec3 bitangent;
in vec2 map1;
in vec4 colorSet1;
in vec4 colorSet5;
in vec2 bake1;
in vec3 position;
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
// TODO: Cubemap loading doesn't work yet.
uniform sampler2D difCubemap;

uniform int hasDiffuse;
uniform sampler2D difMap;

uniform sampler2D iblLut;

uniform samplerCube diffusePbrCube;
uniform samplerCube specularPbrCube;

uniform int renderDiffuse;
uniform int renderSpecular;
uniform int renderEmission;
uniform int renderRimLighting;

uniform int renderWireframe;
uniform int renderVertexColor;
uniform int renderNormalMaps;

uniform vec4 paramA6;
uniform vec4 paramA3;
uniform vec4 paramA5;
uniform vec4 paramA0;
uniform vec4 param98;

uniform int paramE9;

uniform float paramC8;
uniform float paramCA;

uniform float transitionFactor;
uniform int transitionEffect;

uniform mat4 mvp;
uniform vec3 V;

out vec4 fragColor;

const float directLightIntensity = 1.25;

// Defined in Wireframe.frag.
float WireframeIntensity(vec3 distanceToEdges);

// Defined in NormalMap.frag.
vec3 GetBumpMapNormal(vec3 N, vec3 tangent, vec3 bitangent, vec4 norColor);

float LambertShading(vec3 N, vec3 V)
{
    float lambert = max(dot(N, V), 0);
    return lambert;
}

// Defined in Gamma.frag.
vec3 GetSrgb(vec3 linear);

vec3 FresnelSchlickRoughness(float cosTheta, vec3 F0, float roughness)
{
    return F0 + (max(vec3(1.0 - roughness), F0) - F0) * pow(1.0 - cosTheta, 5.0);
}

// GGX calculations adapted from https://learnopengl.com/PBR/IBL/Specular-IBL
float Ggx(vec3 N, vec3 H, float roughness)
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

// Code adapted from equations listed here:
// http://graphicrants.blogspot.com/2013/08/specular-brdf-reference.html
float GgxAnisotropic(vec3 N, vec3 H, vec3 tangent, vec3 bitangent, float roughX, float roughY)
{
    float normalization = 1 / (3.14159 * roughX * roughY);

    float nDotH = max(dot(N, H), 0.0);
    float nDotH2 = nDotH * nDotH;

    // Square input roughness to look correct.
    roughX *= roughX;
    roughY *= roughY;

    float roughX2 = roughX * roughX;
    float roughY2 = roughY * roughY;

    float xDotH = dot(tangent, H);
    float xTerm = (xDotH * xDotH) / roughX2;

    float yDotH = dot(bitangent, H);
    float yTerm = (yDotH * yDotH) / roughY2;

    float denominator = xTerm + yTerm + nDotH2;

    return 1.0 / (normalization * denominator * denominator);
}

vec3 DiffuseTerm(vec4 albedoColor, vec3 diffuseIbl, vec3 N, vec3 V, vec3 kDiffuse)
{
    // Baked ambient lighting.
    vec3 diffuseLight = diffuseIbl;
    diffuseLight += texture(bakeLitMap, bake1).rgb;

    // Direct lighting.
    diffuseLight += LambertShading(N, V) * directLightIntensity;

    vec3 diffuseTerm = kDiffuse * albedoColor.rgb * diffuseLight;

    // Ambient occlusion.
    diffuseTerm *= texture(gaoMap, bake1).rgb;

    diffuseTerm *= paramA5.rgb;

    // HACK: Some models have black vertex color for some reason.
    if (renderVertexColor == 1 && dot(colorSet1.rgb, vec3(1)) != 0)
        diffuseTerm *= colorSet1.rgb;
    return diffuseTerm;
}

vec3 SpecularTerm(vec3 N, vec3 V, vec3 tangent, vec3 bitangent, float roughness, vec3 specularIbl, vec3 kSpecular, float occlusion)
{
    // TODO: Image based lighting doesn't consider anisotropy.
    // Specular calculations adapted from https://learnopengl.com/PBR/IBL/Specular-IBL
    vec2 brdf  = texture(iblLut, vec2(max(dot(N, V), 0.0), roughness)).rg;
    vec3 specularTerm = vec3(0);
    specularTerm += specularIbl * ((kSpecular * brdf.x) + brdf.y);

    // This probably works differently in game.
    // https://developer.blender.org/diffusion/B/browse/master/intern/cycles/kernel/shaders/node_anisotropic_bsdf.osl
    float roughnessY = roughness / (1.0 + paramCA);
    float roughnessX = roughness * (1.0 + paramCA);

    // Direct lighting.
    // The two BRDFs look very different so don't just use anisotropic for everything.
    if (paramCA != 0)
        specularTerm += kSpecular * GgxAnisotropic(N, V, tangent, bitangent, roughnessX, roughnessY) * directLightIntensity;
    else
        specularTerm += kSpecular * Ggx(N, V, roughness) * directLightIntensity;

    // Cavity Map used for specular occlusion.
    if (paramE9 == 1)
        specularTerm.rgb *= occlusion;

    // TODO: Some sort of specular intensity?
    // specularTerm *= paramC8;

    return specularTerm;
}

vec3 RimLightingTerm(vec3 N, vec3 V, vec3 specularIbl)
{
    float rimLight = (1 - max(dot(N, V), 0));
    return paramA6.rgb * pow(rimLight, 3) * specularIbl * 0.5;
}

vec3 EmissionTerm(vec4 emissionColor)
{
    vec3 emissionTerm = vec3(0);
    emissionTerm.rgb += emissionColor.rgb;
    return emissionTerm;
}

vec3 Blend(vec4 a, vec4 b)
{
    return mix(a.rgb, b.rgb, b.a);
}

vec4 GetEmissionColor()
{
    vec4 emissionColor = texture(emiMap, map1).rgba;
    vec4 emission2Color = texture(emi2Map, map1).rgba;
    emissionColor.rgb = Blend(emissionColor, emission2Color);
    return emissionColor;
}

vec4 GetAlbedoColor()
{
    // Blend two diffuse layers based on alpha.
    // The second layer is set using the first layer if not present.
    vec4 albedoColor = texture(colMap, map1).rgba;
    vec4 albedoColor2 = texture(col2Map, map1).rgba;
    vec4 diffuseColor = texture(difMap, map1).rgba;

    // Vertex color alpha is used for some stages.
    float blend = albedoColor2.a * colorSet5.a;

    albedoColor.rgb = mix(albedoColor.rgb, albedoColor2.rgb, blend);

    // We can do this because the default value is black.
    // Materials won't have col and diffuse cubemaps.
    albedoColor.rgb += texture(difCubemap, map1).rgb;

    if (hasDiffuse == 1)
        albedoColor.rgb = Blend(albedoColor, diffuseColor);

    return albedoColor;
}

void main()
{
    fragColor = vec4(0, 0, 0, 1);

    vec4 norColor = texture(norMap, map1).xyzw;
    vec3 newNormal = N;
    if (renderNormalMaps == 1)
        newNormal = GetBumpMapNormal(N, tangent, bitangent, norColor);

    vec3 R = reflect(V, newNormal);

    // Get texture color.
    vec4 albedoColor = GetAlbedoColor();
    vec4 emissionColor = GetEmissionColor();
    vec4 prmColor = texture(prmMap, map1).xyzw;

    // Material masking.
    float transitionBlend = 0;
    if (norColor.b <= (1 - transitionFactor))
        transitionBlend = 1;

    switch (transitionEffect)
    {
        case 0:
            // Ditto
            albedoColor.rgb = mix(vec3(0.302, 0.242, 0.374), albedoColor.rgb, transitionBlend);
            prmColor = mix(vec4(0, 0.65, 1, 1), prmColor, transitionBlend);
            break;
        case 1:
            // Ink
            albedoColor.rgb = mix(vec3(0.75, 0.10, 0), albedoColor.rgb, transitionBlend);
            prmColor = mix(vec4(0, 0.075, 1, 1), prmColor, transitionBlend);
            break;
        case 2:
            // Gold
            albedoColor.rgb = mix(vec3(0.6, 0.5, 0.1), albedoColor.rgb, transitionBlend);
            prmColor = mix(vec4(1, 0.25, 1, 0.3), prmColor, transitionBlend);
            break;
        case 3:
            // Metal
            albedoColor.rgb = mix(vec3(0.35), albedoColor.rgb, transitionBlend);
            prmColor = mix(vec4(1, 0.25, 1, 0.3), prmColor, transitionBlend);
            break;
    }

    float roughness = prmColor.g;
    float metalness = prmColor.r;

    // Image based lighting.
    float iblIntensity = 2.0;
    vec3 diffuseIbl = textureLod(diffusePbrCube, N, 0).rrr * iblIntensity;
    int maxLod = 10;
    vec3 specularIbl = textureLod(specularPbrCube, R, roughness * maxLod).rrr * iblIntensity;

    fragColor = vec4(0, 0, 0, 1);

    // TODO: What is the in game value?
    float maxF0Dialectric = 0.08;
    vec3 f0 = mix(prmColor.aaa * maxF0Dialectric, albedoColor.rgb, metalness);
    vec3 kSpecular = FresnelSchlickRoughness(max(dot(newNormal, V), 0.0), f0, roughness);

    // Only use one component to prevent discoloration of diffuse.
    vec3 kDiffuse = (1 - kSpecular.rrr);

    // Metals have no diffuse component.
    // This includes most skin materials.
    kDiffuse *= (1 - metalness);

    // Render passes.
    vec3 diffuseTerm = DiffuseTerm(albedoColor, diffuseIbl, newNormal, V, kDiffuse);
    fragColor.rgb += diffuseTerm * renderDiffuse;

    vec3 rimTerm = RimLightingTerm(newNormal, V, specularIbl);
    fragColor.rgb += rimTerm * renderRimLighting;

    vec3 specularTerm = SpecularTerm(newNormal, V, tangent, bitangent, roughness, specularIbl, kSpecular, norColor.a);
    fragColor.rgb += specularTerm * renderSpecular;

    // Ambient Occlusion
    fragColor.rgb *= prmColor.b;

    // Emission
    vec3 emissionTerm = EmissionTerm(emissionColor);
    fragColor.rgb += emissionTerm * renderEmission;

    if (renderWireframe == 1)
    {
        vec3 edgeColor = vec3(1);
        float intensity = WireframeIntensity(edgeDistance);
        fragColor.rgb = mix(fragColor.rgb, edgeColor, intensity);
    }

    // Gamma correction.
    fragColor.rgb = GetSrgb(fragColor.rgb);

    // Alpha calculations
    fragColor.a = albedoColor.a;
    fragColor.a *= emissionColor.a;

    // HACK: Some models have black vertex color for some reason.
    if (renderVertexColor == 1 && colorSet1.a != 0)
        fragColor.a *= colorSet1.a;

    // 0 = alpha. 1 = alpha.
    // Values can be between 0 and 1, however.
    fragColor.a += param98.x;
}
