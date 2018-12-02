#version 330

in vec3 Position0;
in vec3 Normal0;
in vec2 map1;

uniform int SingleBindIndex;
uniform mat4 mvp;
uniform mat4 Transforms[100];

out vec3 N;
out vec2 UV0;

void main()
{
	N = Normal0;
	UV0 = map1;

	vec4 Position = vec4(Position0, 1);

	if(SingleBindIndex != -1)
		Position = Transforms[SingleBindIndex] * vec4(Position0, 1);

	gl_Position = mvp * vec4(Position.xyz, 1);
}