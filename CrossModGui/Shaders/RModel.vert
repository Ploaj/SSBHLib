#version 330

in vec3 Position0;
in vec4 Tangent0;
in vec4 Normal0;

in vec4 colorSet1;
in vec4 colorSet2;
in vec4 colorSet3;
in vec4 colorSet5;

in vec2 map1;
in vec2 uvSet;
in vec2 uvSet1;
in vec2 uvSet2;
in vec2 bake1;

in ivec4 boneIndices;
in vec4 boneWeights;

out vec3 geomVertexNormal;
out vec4 geomTangent;
out vec2 geomMap1;
out vec2 geomUvSet;
out vec2 geomUvSet1;
out vec2 geomUvSet2;
out vec4 geomColorSet1;
out vec4 geomColorSet2;
out vec4 geomColorSet3;
out vec4 geomColorSet4;
out vec4 geomColorSet5;
out vec4 geomColorSet6;
out vec4 geomColorSet7;
out vec2 geomBake1;
out vec3 geomPosition;

uniform mat4 mvp;
uniform mat4 modelView;
uniform mat4 transform;

uniform Bones
{
    mat4 transforms[300];
};

uniform MaterialParams
{
    vec4 CustomVector[64];
    ivec4 CustomBoolean[20];
    vec4 CustomFloat[20];

    vec4 vec4Param;

    int hasCustomVector11;
    int hasCustomVector47;
    int hasCustomVector44;
    int hasCustomFloat10;
    int hasCustomFloat19;
    int hasCustomBoolean1;

    int hasColMap; 
    int hasCol2Map; 
    int hasInkNorMap; 
    int hasDifCubeMap; 
    int hasDiffuse; 
    int hasDiffuse2;
    int hasDiffuse3; 
    int emissionOverride;

    int hasColorSet1;
    int hasColorSet2;
    int hasColorSet3;
    int hasColorSet4;
    int hasColorSet5;
    int hasColorSet6;
    int hasColorSet7;

    int isValidShaderLabel;
};


void main()
{
    // Single bind transform
    vec4 position = transform * vec4(Position0.xyz, 1.0);
    vec4 transformedNormal = transform * vec4(Normal0.xyz, 0.0);
    vec4 transformedTangent = transform * vec4(Tangent0.xyz, 0.0);


    // Vertex skinning
    if (boneWeights.x != 0.0) {
        position = vec4(0.0);
        transformedNormal = vec4(0.0);

        for (int i = 0; i < 4; i++)
        {
            position += transforms[boneIndices[i]] * vec4(Position0.xyz, 1.0) * boneWeights[i];
            transformedNormal.xyz += (inverse(transpose(transforms[boneIndices[i]])) * vec4(Normal0.xyz, 1.0) * boneWeights[i]).xyz;
            transformedTangent.xyz += (inverse(transpose(transforms[boneIndices[i]])) * vec4(Tangent0.xyz, 1.0) * boneWeights[i]).xyz;
        }
    }
    
    geomPosition = position.xyz;

    transformedNormal.xyz = normalize(transformedNormal.xyz);
    transformedTangent.xyz = normalize(transformedTangent.xyz);

    // Assign geometry inputs
    geomVertexNormal = transformedNormal.xyz;
    geomColorSet1 = colorSet1 * 2.0;

    // TODO: Pack colors together to avoid hitting hardware limits of 16 attributes for some vendors.
    geomColorSet2 = colorSet2 * colorSet2 * 7.0;
    geomColorSet3 = colorSet3 * 2.0;
    geomColorSet4 = vec4(0);
    geomColorSet5 = colorSet5 * 3.0;
    geomColorSet6 = vec4(0);
    geomColorSet7 = vec4(0);

    // Sprite sheet uvs.
    geomMap1 = map1;
    if (CustomBoolean[9].x == 1)
        geomMap1 /= CustomVector[18].xy;

    geomUvSet = uvSet;
    geomUvSet1 = uvSet1;
    geomUvSet2 = uvSet2;
    geomBake1 = bake1;

    // The w component flips mirrored tangents.
    geomTangent = vec4(transformedTangent.xyz, Tangent0.w);
    gl_Position = mvp * vec4(position.xyz, 1.0);
}
