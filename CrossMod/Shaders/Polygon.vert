#version 330

in vec4 point;

uniform mat4 mvp;
uniform mat4 bone;

void main()
{
    vec4 transformed = bone * vec4(point.xyz, 1);

    gl_Position = mvp * vec4(transformed.xyz, 1);
}