#version 330

in vec2 texCoord;

uniform sampler2D colorTex;
uniform sampler2D colorBrightTex;

out vec4 fragColor;

void main()
{
    fragColor = vec4(0, 0, 0, 1);
    vec4 color = texture(colorTex, vec2(texCoord.x, texCoord.y)).rgba;
    vec4 brightColor = texture(colorBrightTex, vec2(texCoord.x, texCoord.y)).rgba;
    fragColor.rgb = color.rgb + brightColor.rgb;
}
