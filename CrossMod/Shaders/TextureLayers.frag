#version 330

uniform sampler2D colMap;

uniform int hasCol2Map;
uniform sampler2D col2Map;

uniform sampler2D prmMap;
uniform sampler2D norMap;
uniform sampler2D emiMap;
uniform sampler2D emi2Map;
uniform sampler2D bakeLitMap;
uniform sampler2D gaoMap;
uniform sampler2D inkNorMap;

// TODO: Cubemap loading doesn't work yet.
uniform int hasDifCubemap;
uniform sampler2D difCubemap;

uniform int hasDiffuse;
uniform sampler2D difMap;

uniform int hasDiffuse2;
uniform sampler2D dif2Map;

uniform int hasDiffuse3;
uniform sampler2D dif3Map;

uniform samplerCube diffusePbrCube;
uniform samplerCube specularPbrCube;

uniform int emissionOverride;

vec3 Blend(vec4 a, vec4 b)
{
    return mix(a.rgb, b.rgb, b.a);
}

vec2 TransformUv(vec2 uv, vec4 transform)
{
    return (uv + vec2(-1.0 * transform.z, transform.w)) * transform.xy;
}

vec4 GetEmissionColor(vec2 uv1, vec2 uv2, vec4 transform1, vec4 transform2)
{
    vec2 uvLayer1 = TransformUv(uv1, transform1);
    vec4 emissionColor = texture(emiMap, uvLayer1).rgba;

    vec2 uvLayer2 = TransformUv(uv2, transform2);
    vec4 emission2Color = texture(emi2Map, uvLayer2).rgba;

    emissionColor.rgb = Blend(emissionColor, emission2Color);
    return emissionColor;
}

vec4 GetAlbedoColor(vec2 uv1, vec2 uv2, vec4 transform1, vec4 transform2, vec4 colorSet5)
{
    vec2 uvLayer1 = TransformUv(uv1, transform1);
    vec2 uvLayer2 = TransformUv(uv2, transform2);

    vec4 albedoColor = texture(colMap, uvLayer1).rgba;
    vec4 albedoColor2 = texture(col2Map, uvLayer2).rgba;

    vec4 diffuseColor = texture(difMap, uvLayer1).rgba;
    vec4 diffuse2Color = texture(dif2Map, uvLayer2).rgba;
    vec4 diffuse3Color = texture(dif3Map, uvLayer2).rgba;

    // Vertex color alpha is used for some stages.
    float blend = albedoColor2.a * colorSet5.a;

    if (hasCol2Map == 1)
        albedoColor.rgb = Blend(albedoColor, albedoColor2);

    // Materials won't have col and diffuse cubemaps.
    if (hasDifCubemap == 1)
        albedoColor.rgb = texture(difCubemap, uvLayer1).rgb;

    if (hasDiffuse == 1)
        albedoColor.rgb = Blend(albedoColor, diffuseColor);
    if (hasDiffuse2 == 1)
        albedoColor.rgb = Blend(albedoColor, diffuse2Color);
    // TODO: Is the blending always additive?
    if (hasDiffuse3 == 1)
        albedoColor.rgb += diffuse3Color.rgb;

    return albedoColor;
}
