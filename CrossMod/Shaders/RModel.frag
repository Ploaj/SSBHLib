#version 330

in vec3 vertexNormal;
in vec4 tangent;
in vec3 bitangent;
in vec2 map1;
in vec2 uvSet;
in vec2 uvSet1;
in vec2 uvSet2;
in vec4 colorSet1;
in vec4 colorSet5;
in vec2 bake1;
in vec3 position;
noperspective in vec3 edgeDistance;

uniform sampler2D colMap;

uniform int hasCol2Map;
uniform sampler2D col2Map;

uniform sampler2D prmMap;
uniform sampler2D norMap;
uniform sampler2D emiMap;
uniform sampler2D emi2Map;
uniform sampler2D bakeLitMap;
uniform sampler2D gaoMap;

uniform int hasInkNorMap;
uniform sampler2D inkNorMap;

uniform int hasDifCubeMap;
uniform samplerCube difCubeMap;

uniform int hasDiffuse;
uniform sampler2D difMap;

uniform int hasDiffuse2;
uniform sampler2D dif2Map;

uniform int hasDiffuse3;
uniform sampler2D dif3Map;

uniform sampler2D iblLut;

uniform samplerCube diffusePbrCube;
uniform samplerCube specularPbrCube;

uniform int emissionOverride;

uniform int renderDiffuse;
uniform int renderSpecular;
uniform int renderEmission;
uniform int renderRimLighting;
uniform int renderExperimental;

uniform int renderWireframe;
uniform int renderVertexColor;
uniform int renderNormalMaps;

uniform float floatTestParam;

uniform float depthBias;

uniform MaterialParams
{
    vec4 CustomVector0;
    vec4 CustomVector3;
    vec4 CustomVector6;
    vec4 CustomVector8;
    vec4 CustomVector11;
    vec4 CustomVector13;
    vec4 CustomVector14;
    vec3 CustomVector18;
    vec4 CustomVector30;
    vec4 CustomVector31;
    vec4 CustomVector32;
    vec4 CustomVector42;
    vec4 CustomVector47;
    vec4 CustomVector44;
    vec4 CustomVector45;

    vec4 vec4Param;

    int CustomBoolean1;
    int CustomBoolean2;
    int CustomBoolean3;
    int CustomBoolean4;
    int CustomBoolean9;
    int CustomBoolean11;

    float CustomFloat1;
    float CustomFloat4;
    float CustomFloat8;
    float CustomFloat10;
    float CustomFloat19;
};


// TODO: Add lighting vectors to a uniform block.
// Values taken from the training stage light.nuanmb.
float LightCustomFloat0 = 4.0;
vec4 LightCustomVector0 = vec4(1,1,1,0); 
vec4 LightCustomVector8 = vec4(1.5,1.5,1.5,1); 

uniform int hasCustomVector11;
uniform int hasCustomVector47;
uniform int hasCustomVector44;
uniform int hasCustomFloat10;

uniform vec3 chrLightDir;

uniform mat4 mvp;
uniform mat4 modelViewMatrix;
uniform vec3 cameraPos;

out vec4 fragColor;

uniform float directLightIntensity;
uniform float iblIntensity;

// Defined in Wireframe.frag.
float WireframeIntensity(vec3 distanceToEdges);

// Defined in NormalMap.frag.
vec3 GetBumpMapNormal(vec3 N, vec3 tangent, vec3 bitangent, vec4 norColor);

// Defined in Gamma.frag.
vec3 GetSrgb(vec3 linear);

vec3 FresnelSchlick(float cosTheta, vec3 F0)
{
    return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
} 

// GGX calculations adapted from https://learnopengl.com/PBR/IBL/Specular-IBL
float Ggx(float nDotH, float roughness)
{
    float a = roughness * roughness;
    float a2 = a * a;
    float nDotH2 = nDotH * nDotH;

    float numerator = a2;
    float denominator = (nDotH2 * (a2 - 1.0) + 1.0);
    denominator = 3.14159 * denominator * denominator;

    return numerator / denominator;
}

// Code adapted from equations listed here:
// http://graphicrants.blogspot.com/2013/08/specular-brdf-reference.html
float GgxAnisotropic(float nDotH, vec3 H, vec3 tangent, vec3 bitangent, float roughness, float anisotropy)
{
    float roughnessX = max(roughness * anisotropy, 0.01);
    float roughnessY = max(roughness / anisotropy, 0.01);

    // TODO: Anisotropic rotation using PRM alpha?

    // TODO: Check this section of code.
    float normalization = (3.14159 * roughnessX * roughnessY);

    float nDotH2 = nDotH * nDotH;

    float roughnessX2 = roughnessX * roughnessX;
    float roughnessY2 = roughnessY * roughnessY;

    float xDotH = dot(tangent, H);
    float xTerm = (xDotH * xDotH) / roughnessX2;

    float yDotH = dot(bitangent, H);
    float yTerm = (yDotH * yDotH) / roughnessY2;

    float denominator = xTerm + yTerm + nDotH2;

    return 1.0 / (normalization * denominator * denominator);
}

// Defined in TextureLayers.frag.
vec4 GetEmissionColor(vec2 uv1, vec2 uv2, vec4 transform1, vec4 transform2);
vec4 GetAlbedoColor(vec2 uv1, vec2 uv2, vec2 uv3, vec3 R, vec4 transform1, vec4 transform2, vec4 transform3, vec4 colorSet5);

vec3 GetDiffuseLighting(float nDotL, vec3 ambientIbl, vec3 ao)
{
    // Diffuse shading is remapped to be softer.
    // Multiplying be a constant and clamping affects the "smoothness".
    float directShading = nDotL;
    if (hasCustomVector11 == 1)
    {
        directShading *= CustomVector30.y;
        directShading = directShading * 0.5 + 0.5;
    }
    directShading = clamp(directShading, 0, 1);

    // TODO: Investigate why diffuse is so bright?
    vec3 directLight = LightCustomVector0.xyz * directShading; // * LightCustomFloat0
    
    vec4 bakedLitColor = texture(bakeLitMap, bake1);
    vec3 ambientLight = ambientIbl * ao * bakedLitColor.rgb;

    vec3 result = directLight * directLightIntensity  + ambientLight;
    return result;
}

vec3 DiffuseTerm(vec4 albedoColor, vec3 diffuseIbl, vec3 N, vec3 V, float metalness)
{
    vec3 diffuseTerm = albedoColor.rgb;

    // Color multiplier param.
    diffuseTerm *= CustomVector13.rgb;

    // TODO: Wiifit stage model color.
    if (hasCustomVector44 == 1)
        diffuseTerm = CustomVector44.rgb + CustomVector45.rgb;

    // Fake subsurface scattering.
    // Metalness acts as a mask for the sss effect.
    float sssBlend = CustomVector30.x * metalness;
    diffuseTerm = mix(diffuseTerm, CustomVector11.rgb, sssBlend);
    diffuseTerm += CustomVector11.rgb * sssBlend;

    return diffuseTerm;
}

float SpecularBrdf(float nDotH, vec3 halfAngle, float roughness)
{
    // The two BRDFs look very different so don't just use anisotropic for everything.
    if (hasCustomFloat10 == 1)
        return GgxAnisotropic(nDotH, halfAngle, tangent.xyz, bitangent, roughness, CustomFloat10);
    else
        return Ggx(nDotH, roughness);
}

vec3 SpecularTerm(float nDotH, vec3 halfAngle, float roughness, vec3 specularIbl, float metalness)
{
    vec3 directSpecular = LightCustomVector0.xyz * LightCustomFloat0 * SpecularBrdf(nDotH, halfAngle, roughness) * directLightIntensity;
    vec3 indirectSpecular = specularIbl;
    vec3 specularTerm = (directSpecular * CustomBoolean3) + (indirectSpecular * CustomBoolean4);

    return specularTerm;
}

vec3 EmissionTerm(vec4 emissionColor)
{
    return emissionColor.rgb * CustomVector3.rgb;
}

float GetF0(float ior)
{
    return pow((1 - ior) / (1 + ior), 2);
}

float Luminance(vec3 rgb)
{
    const vec3 W = vec3(0.2125, 0.7154, 0.0721);
    return dot(rgb, W);
}

vec3 GetSpecularWeight(float prmSpec, vec3 diffusePass, float metalness, float nDotV, float roughness)
{
    vec3 tintColor = mix(vec3(1), diffusePass, CustomFloat8); 

    // Metals use albedo instead of the specular color/tint.
    vec3 specularReflectionF0 = vec3(prmSpec * 0.2) * tintColor;
    vec3 f0Final = mix(specularReflectionF0, diffusePass, metalness);

    return FresnelSchlick(nDotV, f0Final);
}

vec3 RimLightingTerm(float nDotV, vec3 fragmentNormal, float occlusion)
{
    // TODO: Difference between RGB color intensity and W value?
    vec3 rimColor = CustomVector14.rgb * LightCustomVector8.rgb;
    
    // TODO: The direction may be controlled by some value.
    // Edge lighting lit only from above.
    float exponent = 2;
    float rimHemisphere = (1 - nDotV) * max(dot(fragmentNormal, normalize(vec3(0,1,-2))),0);
    float rimIntensity = pow(rimHemisphere, exponent) * occlusion * LightCustomVector8.w * CustomVector14.w;
    return rimColor * rimIntensity;
}

float RoughnessToLod(float roughness)
{
    // Adapted from decompiled shader source.
    // Applies a curves adjustment to roughness.
    float gpr23 = max(roughness, 0.01);
    float gpr21 = (gpr23 * gpr23);
    float gpr0 = (1.0 / gpr21);
    float gpr1 = log2(gpr0 * 2 - 2);
    gpr1 = gpr1 * -0.4545 + 4;
    return gpr1;
}

void main()
{
    fragColor = vec4(0, 0, 0, 1);

    vec4 norColor = texture(norMap, map1).xyzw;
    if (hasInkNorMap == 1)
        norColor.xyz = texture(inkNorMap, map1).rga;

    vec3 fragmentNormal = vertexNormal;
    if (renderNormalMaps == 1)
        fragmentNormal = GetBumpMapNormal(vertexNormal, tangent.xyz, bitangent, norColor);

    // Transform the view vector to world space.
    vec3 viewVector = normalize(vec3(0,0,-1) * mat3(mvp));

    // TODO: Double check the orientation.
    vec3 reflectionVector = reflect(viewVector, fragmentNormal);
    reflectionVector.y *= -1;

    // TODO: ???
    float iorRatio = 1.0 / (1.0 + CustomFloat19);
    vec3 refractionVector = refract(viewVector, normalize(fragmentNormal), iorRatio);

    // Shading vectors.
    vec3 halfAngle = normalize(chrLightDir + viewVector);
    float nDotV = max(dot(fragmentNormal, viewVector), 0);
    float nDotL = dot(fragmentNormal, chrLightDir);
    float nDotH = max(dot(fragmentNormal, halfAngle), 0.0);

    // Get texture color.
    vec4 albedoColor = GetAlbedoColor(map1, uvSet, uvSet, reflectionVector, CustomVector6, CustomVector31, CustomVector32, colorSet5);

    vec4 emissionColor = GetEmissionColor(map1, uvSet, CustomVector6, CustomVector31);
    // TODO: There's probably a cleaner way of doing this.
    if (CustomBoolean11 == 0)
        emissionColor.rgb *= (1 - texture(col2Map, uvSet).a);

    vec4 prmColor = texture(prmMap, map1).xyzw;

    // Probably some sort of override for PRM color.
    if (hasCustomVector47 == 1)
        prmColor = CustomVector47;

    fragColor.a = max(albedoColor.a * emissionColor.a, CustomVector0.x);
    // Alpha testing.
    // TODO: Not all shaders have this.
    if (fragColor.a < 0.5)
        discard;

    float roughness = prmColor.g;
    float metalness = prmColor.r;
    // Specular isn't effected by metalness for skin materials.
    if (hasCustomVector11 == 1)
        metalness = 0.0;

    float specular = prmColor.a;
    if (CustomBoolean1 == 0)
        specular = 0.16;

    vec3 ambientOcclusion = vec3(prmColor.b);
    ambientOcclusion *= pow(texture(gaoMap, bake1).rgb, vec3(CustomFloat1 + 1.0));

    // Image based lighting.
    int maxLod = 6;
    float specularLod = RoughnessToLod(roughness);
    vec3 diffuseIbl = textureLod(diffusePbrCube, fragmentNormal, 0).rgb * 0.5 * iblIntensity; // TODO: constant?
    vec3 specularIbl = textureLod(specularPbrCube, reflectionVector, specularLod).rgb * iblIntensity * 0.5;
    vec3 refractionIbl = textureLod(specularPbrCube, refractionVector, 0.075 * maxLod).rgb * iblIntensity;

    // Render passes.
    vec3 diffusePass = DiffuseTerm(albedoColor, diffuseIbl, fragmentNormal, viewVector, prmColor.r);
    vec3 diffuseLight = GetDiffuseLighting(nDotL, diffuseIbl, ambientOcclusion);
    vec3 specularPass = SpecularTerm(nDotH, halfAngle, roughness, specularIbl, metalness);

    vec3 kSpecular = GetSpecularWeight(specular, diffusePass.rgb, metalness, nDotV, roughness);
    vec3 kDiffuse = (vec3(1) - kSpecular) * (1 - metalness);

    if (renderDiffuse == 1)
        fragColor.rgb += diffusePass * diffuseLight * kDiffuse;

    if (renderSpecular == 1)
        fragColor.rgb += clamp(specularPass * kSpecular * ambientOcclusion * norColor.a, 0, 1);

    // Emission
    if (renderEmission == 1)
        fragColor.rgb += EmissionTerm(emissionColor);

    if (renderRimLighting == 1)
    {
        fragColor.rgb += RimLightingTerm(nDotV, fragmentNormal, norColor.a * prmColor.b);
    }

    // HACK: Some models have black vertex color for some reason.
    if (renderVertexColor == 1 && Luminance(colorSet1.rgb) > 0.0)
        fragColor.rgb *= colorSet1.rgb;

    // TODO: Experimental refraction.
    if (CustomFloat19 > 0.0)
        fragColor.rgb += refractionIbl * renderExperimental;

    // Final color multiplier.
    fragColor.rgb *= CustomVector8.rgb;

    // Gamma correction.
    fragColor.rgb = GetSrgb(fragColor.rgb);

    // Alpha calculations
    fragColor.a *= emissionColor.a;

    // HACK: Some models have black vertex color for some reason.
    if (renderVertexColor == 1 && colorSet1.a != 0)
        fragColor.a *= colorSet1.a;

    // TODO: Meshes with refraction have some sort of angle fade.
    float f0Refract = GetF0(CustomFloat19 + 1.0);
    vec3 transmissionAlpha = FresnelSchlick(nDotV, vec3(f0Refract));
    if (CustomFloat19 > 0 && renderExperimental == 1)
        fragColor.a = transmissionAlpha.x;

    // Premultiplied alpha.
    fragColor.rgb *= fragColor.a;

    if (renderWireframe == 1)
    {
        vec3 edgeColor = vec3(1);
        float intensity = WireframeIntensity(edgeDistance);
        fragColor.rgb = mix(fragColor.rgb, edgeColor, intensity);
    }

    //gl_FragDepth = gl_FragCoord.z + depthBias;
}
