#version 330

in vec2 texCoord;

uniform sampler2D image;

out vec4 fragColor;

void main()
{
    fragColor = vec4(texture(image, vec2(texCoord.x, texCoord.y)).rgb, 1);
}
