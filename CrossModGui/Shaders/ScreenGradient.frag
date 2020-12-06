#version 330

in vec2 texCoord;

uniform vec3 colorBottom;
uniform vec3 colorTop;

out vec4 fragColor;

void main()
{
    fragColor = vec4(0.0, 0.0, 0.0, 1.0);
    fragColor.rgb = mix(colorBottom, colorTop, texCoord.y);
}
