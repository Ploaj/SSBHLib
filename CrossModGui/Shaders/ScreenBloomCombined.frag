#version 330

in vec2 texCoord;

uniform sampler2D colorTex;
uniform sampler2D colorBrightTex;

uniform int enableBloom;
uniform float bloomIntensity;

out vec4 fragColor;

// TODO: Use functions from Gamma.frag.
float GetSrgb(float linear)
{
    if (linear <= 0.00031308)
        return 12.92 * linear;
    else
        return 1.055 * pow(linear, (1.0 / 2.4)) - 0.055;
}

vec3 GetSrgb(vec3 linear)
{
    return vec3(GetSrgb(linear.x), GetSrgb(linear.y), GetSrgb(linear.z));
}

void main()
{
    fragColor = vec4(0, 0, 0, 1);
    vec4 color = texture(colorTex, vec2(texCoord.x, texCoord.y)).rgba;
    vec4 brightColor = texture(colorBrightTex, vec2(texCoord.x, texCoord.y)).rgba;
    // TODO: In game appears to just render the texture to the screen with additive blending.
    fragColor.rgb = color.rgb;
    if (enableBloom == 1)
        fragColor.rgb += brightColor.rgb * bloomIntensity;
    fragColor.rgb = GetSrgb(fragColor.rgb);
}
