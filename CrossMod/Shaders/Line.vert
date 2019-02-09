#version 330

in vec4 point;

uniform mat4 mvp;
uniform mat4 rotation;
uniform mat4 bone;

uniform vec3 offset;

uniform float size;

void main()
{
    vec4 startpoint = bone * vec4(offset, 1);

    vec4 endpoint = rotation * vec4(point.xyz * size, 1);

    gl_Position = mvp * vec4(startpoint.xyz + endpoint.xyz, 1);
}