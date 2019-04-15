#version 330

in vec4 point;

uniform mat4 mvp;
uniform mat4 bone;

uniform vec3 offset1;
uniform vec3 offset2;

uniform mat4 orientation;

uniform float size;

void main()
{
	vec4 position;

	if(point.w == 1)
	{
		position = bone * vec4((orientation * point).xyz * size + offset2, 1);
	}
	else
	{
		position = bone * vec4((orientation * point).xyz * size + offset1, 1);
	}

    gl_Position = mvp * vec4(position.xyz, 1);
}