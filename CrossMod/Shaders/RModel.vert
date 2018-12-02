#version 330

in vec3 Position0;
in vec3 Normal0;

uniform mat4 mvp;

out vec3 N;

void main()
{
N = Normal0;
gl_Position = mvp * vec4(Position0, 1);
}