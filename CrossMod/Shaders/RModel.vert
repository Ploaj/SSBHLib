#version 330

in vec3 Position0;
in vec3 Tangent0;
in vec3 Normal0;
in vec3 colorSet1;
in vec2 map1;

uniform mat4 mvp;
uniform mat4 transform;

out vec3 N;
out vec3 tangent;
out vec2 UV0;
out vec3 vertexColor;

void main()
{
	vec4 transformedNormal = transform * vec4(Normal0, 0);
	N = transformedNormal.rgb;

	vertexColor = colorSet1;

	UV0 = map1;
	tangent = Tangent0;

	vec4 position = vec4(Position0, 1);
	position = transform * vec4(Position0, 1);

	gl_Position = mvp * vec4(position.xyz, 1);
}
