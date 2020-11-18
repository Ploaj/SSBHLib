#version 330

in vec3 texCoord;

uniform samplerCube image;
uniform int isSrgb;

out vec4 fragColor;

// Defined in Gamma.frag.
vec3 GetSrgb(vec3 linear);

void main()
{
    fragColor = vec4(texture(image, texCoord).rgb, 1);
    if (isSrgb == 1)
        fragColor.rgb = GetSrgb(fragColor.rgb);
}