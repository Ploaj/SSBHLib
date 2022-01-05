#version 330

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

uniform samplerCube diffusePbrCube;
uniform samplerCube specularPbrCube;

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
    int isDiscard;
};
uniform int hasRequiredAttributes;

uniform float floatTestParam;

uniform int renderExperimental;

vec3 Blend(vec4 a, vec4 b)
{
    // CustomBoolean11 toggles additive vs alpha blending.
    if (CustomBoolean[11].x != 0.0)
        return a.rgb + b.rgb * b.a;
    else
        return mix(a.rgb, b.rgb, b.a);
}

vec2 TransformUv(vec2 uv, vec4 transform)
{
    vec2 translate = vec2(-1.0 * transform.z, transform.w);

    // TODO: Does this affect all layers?
    // if (CustomBoolean5 == 1 || CustomBoolean6 == 1)
    //     translate *= currentFrame / 60.0;

    vec2 scale = transform.xy;
    vec2 result = (uv + translate) * scale;

    if (renderExperimental == 1)
    {
        // dUdV Map.
        // Remap [0,1] to [-1,1].
        vec2 textureOffset = texture(norMap, uv*2).xy * 2.0 - 1.0; 
        result += (textureOffset) * CustomFloat[4].x;
    }

    return result;
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

vec4 GetAlbedoColor(vec2 uv1, vec2 uv2, vec2 uv3, vec3 R, vec4 transform1, vec4 transform2, vec4 transform3, vec4 colorSet5)
{
    // HACK: The default albedo color is white, which won't work with emission.
    if (emissionOverride == 1)
        return vec4(0.0, 0.0, 0.0, 1.0);

    vec2 uvLayer1 = TransformUv(uv1, transform1);
    vec2 uvLayer2 = TransformUv(uv2, transform2);
    vec2 uvLayer3 = TransformUv(uv3, transform3);

    vec4 albedoColor = textureLod(colMap, uvLayer1, 0).rgba;
    vec4 albedoColor2 = texture(col2Map, uvLayer2).rgba;

    vec4 diffuseColor = texture(difMap, uvLayer1).rgba;
    vec4 diffuse2Color = texture(dif2Map, uvLayer2).rgba;
    vec4 diffuse3Color = texture(dif3Map, uvLayer3).rgba;

    // colorSet5.w is used to blend between the two col map layers.
    if (hasCol2Map == 1)
        albedoColor.rgb = Blend(albedoColor, albedoColor2 * vec4(1.0, 1.0, 1.0, colorSet5.w));

    // Materials won't have col and diffuse cubemaps.
    if (hasDifCubeMap == 1)
        albedoColor.rgb = texture(difCubeMap, R).rgb;

    if (hasDiffuse == 1)
        albedoColor.rgb = Blend(albedoColor, diffuseColor);
    if (hasDiffuse2 == 1)
        albedoColor.rgb += diffuse2Color.rgb;
    // TODO: Is the blending always additive?
    if (hasDiffuse3 == 1)
        albedoColor.rgb += diffuse3Color.rgb;

    return albedoColor;
}

vec3 GetAlbedoColorFinal(vec4 albedoColor)
{    
    vec3 albedoColorFinal = albedoColor.rgb;

    // Color multiplier param.
    albedoColorFinal *= CustomVector[13].rgb;

    // TODO: Wiifit stage model color.
    if (hasCustomVector44 == 1)
        albedoColorFinal = CustomVector[44].rgb + CustomVector[45].rgb;

    return albedoColorFinal;
}
