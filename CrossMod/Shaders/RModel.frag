#version 330

in vec3 vertexNormal;
in vec3 tangent;
in vec3 bitangent;
in vec2 map1;
in vec2 uvSet;
in vec2 uvSet1;
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
    int CustomBoolean9;

    float CustomFloat8;
    float CustomFloat10;
    float CustomFloat19;
};

uniform int hasCustomVector11;
uniform int hasCustomVector47;
uniform int hasCustomVector44;

uniform float transitionFactor;
uniform int transitionEffect;

uniform vec3 chrLightDir;

uniform mat4 mvp;
uniform vec3 cameraPos;

out vec4 fragColor;

uniform float directLightIntensity;
uniform float iblIntensity;

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
float GgxAnisotropic(vec3 N, vec3 H, vec3 tangent, vec3 bitangent, float roughness, float anisotropy)
{
    // TODO: Does image based lighting consider anisotropy?
    // This probably works differently in game.
    // https://developer.blender.org/diffusion/B/browse/master/intern/cycles/kernel/shaders/node_anisotropic_bsdf.osl
    float roughnessX = roughness * (1.0 + anisotropy);
    float roughnessY = roughness / (1.0 + anisotropy);

    float normalization = 1 / (3.14159 * roughnessX * roughnessY);

    float nDotH = max(dot(N, H), 0.0);
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

vec3 DiffuseTerm(vec4 albedoColor, vec3 diffuseIbl, vec3 N, vec3 V, float metalness, vec3 sssColor)
{
    vec3 diffuseTerm = albedoColor.rgb;

    // Color multiplier param.
    diffuseTerm *= CustomVector13.rgb;

    // TODO: Wiifit stage model color.
    if (hasCustomVector44 == 1)
        diffuseTerm = CustomVector44.rgb + CustomVector45.rgb;

    // Fake subsurface scattering.
    // Metalness acts as a mask for the sss effect.
    if (renderExperimental == 1)
    {
        diffuseTerm = mix(diffuseTerm, sssColor, (CustomVector30.x * CustomVector30.x) * metalness);
        diffuseTerm += sssColor * metalness;
    }

    // TODO: The pacman apple item has negative colors for some reason.
    return max(diffuseTerm, vec3(0));
}

float EdgeTintBlend(vec3 N, vec3 V)
{   
    // TODO: Double check the shading.
    return clamp(FresnelSchlickRoughness(dot(N,V), vec3(0), 0).x * CustomVector14.w, 0, 1);
}

float SpecularBrdf(vec3 N, vec3 V, float roughness, float specPower)
{
    // The two BRDFs look very different so don't just use anisotropic for everything.
    vec3 halfAngle = normalize(chrLightDir + V);
    float specularBrdf = 0;
    if (CustomFloat10 != 0.0)
        specularBrdf = GgxAnisotropic(N, halfAngle, tangent, bitangent, roughness, CustomFloat10);
    else
        specularBrdf = Ggx(N, halfAngle, roughness);

    return specularBrdf;
}

vec3 SpecularTerm(vec3 N, vec3 V, vec3 tangent, vec3 bitangent, float roughness, vec3 specularIbl, float cavity, float specPower, float metalness, vec3 sssColor)
{
    vec3 indirectSpecular = specularIbl;
    // TODO: Direct light intensity?
    vec3 directSpecular = vec3(SpecularBrdf(N,V, roughness, specPower)) * directLightIntensity;
    vec3 specularTerm = directSpecular + indirectSpecular;

    // TODO: The pacman apple item has negative colors for some reason.
    return max(specularTerm,vec3(0));
}

vec3 EmissionTerm(vec4 emissionColor)
{
    return emissionColor.rgb * CustomVector3.rgb;
}

float GetF0(float ior)
{
    return pow((1 - ior) / (1 + ior), 2);
}

float GetTransitionBlend(float blendMap, float transitionFactor)
{
    // Add a slight offset to prevent black speckles.
    if (blendMap <= (1 - transitionFactor + 0.01))
        return 1.0;
    else
        return 0.0;
}

float Luminance(vec3 rgb)
{
    const vec3 W = vec3(0.2125, 0.7154, 0.0721);
    return dot(rgb, W);
}

vec3 GetSpecularWeight(float prmSpec, vec3 diffusePass, float metalness, float nDotV, float roughness)
{
    vec3 albedoTint = diffusePass;

    vec3 tintColor = mix(vec3(1), albedoTint, CustomFloat8); 

    float dialectricF0Scale = 0.2; 

    // Metals use albedo instead of the specular color/tint.
    vec3 specularReflectionF0 = vec3(dialectricF0Scale * prmSpec) * tintColor;
    vec3 f0Final = mix(specularReflectionF0, diffusePass, metalness);
    return FresnelSchlickRoughness(nDotV, f0Final, roughness);
}

vec3 GetDiffuseLighting(vec3 fragmentNormal, vec3 diffuseIbl, float ao)
{
    vec4 bakedLitColor = texture(bakeLitMap, bake1);
    float directLight = LambertShading(fragmentNormal, normalize(chrLightDir)) * directLightIntensity * bakedLitColor.a;
    vec3 ambientLight = diffuseIbl + bakedLitColor.rgb * 2;
    vec3 result = vec3(directLight) + ambientLight * ao;
    
    if (renderExperimental == 1 && hasCustomVector11 == 1)
    {
        float mid = 0.75; // TODO: ambient intensity and mid value?
        float smoothWidth = 1 / floatTestParam;
        result = smoothstep(mid - smoothWidth, mid + smoothWidth,result);
    }

    return result;
}

void main()
{
    fragColor = vec4(0, 0, 0, 1);

    vec4 norColor = texture(norMap, map1).xyzw;
    if (hasInkNorMap == 1)
        norColor.xyz = texture(inkNorMap, map1).rga;

    vec3 fragmentNormal = vertexNormal;
    if (renderNormalMaps == 1)
        fragmentNormal = GetBumpMapNormal(vertexNormal, tangent, bitangent, norColor);

    vec3 viewVector = normalize(position - cameraPos);
    vec3 reflectionVector = reflect(viewVector, fragmentNormal);
    reflectionVector.y *= -1;
    float nDotV = max(dot(fragmentNormal, viewVector), 0.0);

    float iorRatio = 1.0 / (1.0 + CustomFloat19);
    vec3 refractionVector = refract(viewVector, normalize(fragmentNormal), iorRatio);

    // Get texture color.
    vec4 albedoColor = GetAlbedoColor(map1, uvSet, uvSet, reflectionVector, CustomVector6, CustomVector31, CustomVector32, colorSet5);

    vec4 emissionColor = GetEmissionColor(map1, uvSet, CustomVector6, CustomVector31);
    vec4 prmColor = texture(prmMap, map1).xyzw;

    // Probably some sort of override for PRM color.
    if (hasCustomVector47 == 1)
        prmColor = CustomVector47;

    // Defined separately so it can be disabled for material transitions.
    vec3 sssColor = CustomVector11.rgb;
    float specPower = CustomVector30.x;

    // Material masking.
    float transitionBlend = GetTransitionBlend(norColor.b, transitionFactor);

    // TODO: This might be cleaner with a struct.
    switch (transitionEffect)
    {
        case 0:
            // Ditto
            albedoColor.rgb = mix(vec3(0.302, 0.242, 0.374), albedoColor.rgb, transitionBlend);
            prmColor = mix(vec4(0, 0.65, 1, 1), prmColor, transitionBlend);
            sssColor = mix(vec3(0.1962484, 0.1721312, 0.295082), CustomVector11.rgb, transitionBlend);
            specPower = mix(1.0, CustomVector30.x, transitionBlend);
            break;
        case 1:
            // Ink
            albedoColor.rgb = mix(vec3(0.758027, 0.115859, 0.04), albedoColor.rgb, transitionBlend);
            prmColor = mix(vec4(0, 0.075, 1, 1), prmColor, transitionBlend);
            sssColor = mix(vec3(0), CustomVector11.rgb, transitionBlend);
            specPower = mix(1.0, CustomVector30.x, transitionBlend);
            break;
        case 2:
            // Gold
            albedoColor.rgb = mix(vec3(0.6, 0.5, 0.1), albedoColor.rgb, transitionBlend);
            prmColor = mix(vec4(1, 0.15, 1, 0.3), prmColor, transitionBlend);
            sssColor = mix(vec3(0), CustomVector11.rgb, transitionBlend);
            specPower = mix(1.0, CustomVector30.x, transitionBlend);
            break;
        case 3:
            // Metal
            albedoColor.rgb = mix(vec3(0.5), albedoColor.rgb, transitionBlend);
            prmColor = mix(vec4(1, 0.2, 1, 0.3), prmColor, transitionBlend);
            sssColor = mix(vec3(0), CustomVector11.rgb, transitionBlend);
            specPower = mix(1.0, CustomVector30.x, transitionBlend);
            break;
    }

    float roughness = prmColor.g;
    float metalness = prmColor.r;
    // Specular isn't effected by metalness for skin materials.
    if (hasCustomVector11 == 1)
        metalness = 0.0;

    // Image based lighting.
    vec3 diffuseIbl = textureLod(diffusePbrCube, fragmentNormal, 0).rgb * 0.5; // TODO: constant?
    int maxLod = 6;
    vec3 specularIbl = textureLod(specularPbrCube, reflectionVector, roughness * maxLod).rgb * iblIntensity * 0.5;
    vec3 refractionIbl = textureLod(specularPbrCube, refractionVector, 0.075 * maxLod).rgb * iblIntensity;

    // Render passes.
    vec3 diffusePass = DiffuseTerm(albedoColor, diffuseIbl, fragmentNormal, viewVector, prmColor.r, sssColor);
    vec3 diffuseLight = GetDiffuseLighting(fragmentNormal, diffuseIbl, prmColor.b);
    vec3 specularPass = SpecularTerm(fragmentNormal, viewVector, tangent, bitangent, roughness, specularIbl, norColor.a, specPower, metalness, sssColor);

    if (renderRimLighting == 1)
    {
        // TODO: How does the cavity map work?
        float edgeBlend = EdgeTintBlend(fragmentNormal, viewVector);
        if (renderExperimental == 1) 
            edgeBlend *= norColor.a;
        vec3 edgeTintColor = mix(vec3(1), CustomVector14.rgb, edgeBlend);
        specularPass *= edgeTintColor;
    }

    vec3 kSpecular = GetSpecularWeight(prmColor.a, diffusePass.rgb, metalness, nDotV, roughness);
    vec3 kDiffuse = (vec3(1) - kSpecular) * (1 - metalness);

    if (renderDiffuse == 1)
        fragColor.rgb += diffusePass * diffuseLight * kDiffuse;

    if (renderSpecular == 1)
        fragColor.rgb += specularPass * kSpecular * prmColor.b;

    // TODO: What passes does ambient occlusion affect?
    fragColor.rgb *= texture(gaoMap, bake1).rgb;

    // Emission
    if (renderEmission == 1)
        fragColor.rgb += EmissionTerm(emissionColor);

    // HACK: Some models have black vertex color for some reason.
    if (renderVertexColor == 1 && Luminance(colorSet1.rgb) > 0.0)
        fragColor.rgb *= colorSet1.rgb;

    // TODO: Experimental refraction.
    if (CustomFloat19 > 0.0)
        fragColor.rgb += refractionIbl * renderExperimental;


    fragColor.rgb *= CustomVector8.rgb;

    // Gamma correction.
    fragColor.rgb = GetSrgb(fragColor.rgb);

    // Alpha calculations
    fragColor.a = albedoColor.a;
    fragColor.a *= emissionColor.a;

    // HACK: Some models have black vertex color for some reason.
    if (renderVertexColor == 1 && colorSet1.a != 0)
        fragColor.a *= colorSet1.a;

    // TODO: Meshes with refraction have some sort of angle fade.
    float f0Refract = GetF0(CustomFloat19 + 1.0);
    vec3 transmissionAlpha = FresnelSchlickRoughness(nDotV, vec3(f0Refract), roughness);
    if (CustomFloat19 > 0 && renderExperimental == 1)
        fragColor.a = transmissionAlpha.x;

    // TODO: ???
    fragColor.a += CustomVector0.x;
    fragColor.a = min(fragColor.a, 1.0);

    // Alpha testing.
    if ((fragColor.a) < 0.5)
        discard;

    // TODO: How does this work?
    if (hasInkNorMap == 1 && transitionBlend < 1)
        discard;

    // Premultiplied alpha.
    fragColor.rgb *= fragColor.a;

    if (renderWireframe == 1)
    {
        vec3 edgeColor = vec3(1);
        float intensity = WireframeIntensity(edgeDistance);
        fragColor.rgb = mix(fragColor.rgb, edgeColor, intensity);
    }
}
