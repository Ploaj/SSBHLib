#version 330

in vec4 point;

uniform mat4 mvp;
uniform vec3 bone;

void main()
{
    vec3 translated = point.xyz + bone;

    gl_Position = mvp * vec4(translated, 1);
}