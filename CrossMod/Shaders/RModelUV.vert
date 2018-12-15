#version 330

in vec3 Position0;

in vec3 Tangent0;
in vec3 Bitangent0;
in vec3 Normal0;

// TODO: Does this work properly?
in vec4 colorSet1;

in vec2 bake1;
in vec2 map1;

in ivec4 boneIndices;
in vec4 boneWeights;

out vec3 geomN;
out vec3 geomTangent;
out vec3 geomBitangent;
out vec2 geomUV0;
out vec4 geomColorSet;
out vec2 geomBake1;

uniform mat4 mvp;
uniform mat4 transform;

uniform Bones
{
    mat4 transforms[200];
} bones;

void main()
{
    vec4 position = vec4(map1, 0, 1);
    vec4 transformedNormal = transform * vec4(Normal0, 0);

    // Assign geometry inputs
    geomN = transformedNormal.xyz;
    geomColorSet = colorSet1;
    geomBake1 = bake1;
    geomUV0 = map1;
    geomTangent = Tangent0;
    geomBitangent = Bitangent0;

    gl_Position = mvp * vec4(position.xyz, 1);
}
