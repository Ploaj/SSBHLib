#version 330

in vec4 point;

uniform mat4 mvp;
uniform mat4 rotation;
uniform mat4 bone;

uniform vec3 offset;

uniform float size;

void main()
{
    vec4 position = bone * rotation * vec4(point.xyz * size, 1);

    gl_Position = mvp * vec4(position.xyz + offset, 1);
}