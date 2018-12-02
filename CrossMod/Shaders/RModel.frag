#version 330

in vec3 N;

out vec4 Color;

void main()
{
Color = vec4(N, 1);
}