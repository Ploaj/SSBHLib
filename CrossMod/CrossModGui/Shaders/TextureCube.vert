#version 330

in vec3 position;

uniform mat4 mvpMatrix;

out vec3 texCoord;

void main()
{
    texCoord.xyz = position.xyz;
    gl_Position = mvpMatrix * (vec4(position.xyz, 1.0));
}
