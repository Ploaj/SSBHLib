#version 330

in vec3 N;

out vec4 fragColor;

void main()
{
	fragColor = vec4(N * 0.5 + 0.5, 1);
}