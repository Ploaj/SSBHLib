#version 330

in vec4 point;

uniform mat4 mvp;

uniform mat4 bone1;
uniform vec3 offset1;
uniform vec3 world_offset1;

uniform mat4 bone2;
uniform vec3 offset2;
uniform vec3 world_offset2;

void main()
{
	//hack
    if (point.z == 0)
	{
		vec4 transformed = bone1 * vec4(offset1, 1);
		gl_Position = mvp * vec4(transformed.xyz + world_offset1, 1);
	}
	else
	{
		vec4 transformed = bone2 * vec4(offset2, 1);
		gl_Position = mvp * vec4(transformed.xyz + world_offset2, 1);
	}
}