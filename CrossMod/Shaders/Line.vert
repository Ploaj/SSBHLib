#version 330

in vec3 point;

uniform mat4 mvp;

uniform mat4 bone1;
uniform vec3 trans1;
uniform vec3 off1;

uniform mat4 bone2;
uniform vec3 trans2;
uniform vec3 off2;

void main()
{
    vec4 after_bone = vec4(0);
    vec3 off = vec3(0);
    //hacky way of handling two points separately, although it doesn't need to be more generic
    if (point.z < 0.5)
    {
        after_bone = bone1 * vec4(trans1, 1);
        off = off1;      
    }
    else
    {
        after_bone = bone2 * vec4(trans2, 1);
        off = off2;
    }
    gl_Position = mvp * vec4(after_bone.xyz + off, 1);
}