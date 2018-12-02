#version 330

in vec3 Position0;
in vec3 Normal0;
in vec2 map1;

uniform mat4 mvp;

out vec3 N;
out vec2 UV0;

void main()
{
N = Normal0;
UV0 = map1;
gl_Position = mvp * vec4(Position0, 1);
}