#version 330

in vec2 texCoord;

uniform sampler2D image;

out vec4 fragColor;

void main()
{
    fragColor.rgb = texture(image, vec2(texCoord.x, 1 - texCoord.y)).rgb;
	//fragColor.rgb = vec3(texCoord, 1);
    fragColor.a = 1;
}