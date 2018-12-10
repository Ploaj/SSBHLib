#version 330

layout (triangles) in;
layout (triangle_strip, max_vertices = 3) out;

in vec3 geomN[];
in vec3 geomTangent[];
in vec2 geomUV0[];
in vec4 geomColorSet[];
in vec3 geomBakeColor[];

out vec3 N;
out vec3 tangent;
out vec3 bitangent;
out vec2 UV0;
out vec4 colorSet;
out vec3 bakeColor;

noperspective out vec3 edgeDistance;

// Adapted from code in David Wolff's "OpenGL 4.0 Shading Language Cookbook"
// https://gamedev.stackexchange.com/questions/136915/geometry-shader-wireframe-not-rendering-correctly-glsl-opengl-c
vec3 EdgeDistances() {
    float a = length(gl_in[1].gl_Position.xyz - gl_in[2].gl_Position.xyz);
    float b = length(gl_in[2].gl_Position.xyz - gl_in[0].gl_Position.xyz);
    float c = length(gl_in[1].gl_Position.xyz - gl_in[0].gl_Position.xyz);

    float alpha = acos((b*b + c*c - a*a) / (2.0*b*c));
    float beta = acos((a*a + c*c - b*b) / (2.0*a*c));
    float ha = abs(c * sin(beta));
    float hb = abs(c * sin(alpha));
    float hc = abs(b * sin(alpha));
    return vec3(ha, hb, hc);
}

// Ported from SFGraphics.Utils.
// This should be calculated on the CPU instead.
vec3 CalculateBitangent(vec3 posA, vec3 posB, vec2 uvA, vec2 uvB, float r)
{
    float tX = uvA.x * posB.x - uvB.x * posA.x;
    float tY = uvA.x * posB.y - uvB.x * posA.y;
    float tZ = uvA.x * posB.z - uvB.x * posA.z;

    vec3 bitangent = vec3(tX, tY, tZ) * r;
    return bitangent;
}

// Ported from SFGraphics.Utils.
// This should be calculated on the CPU instead.
vec3 CalculateBitangent(vec3 v1, vec3 v2, vec3 v3, vec2 uv1, vec2 uv2, vec2 uv3)
{
    vec3 posA = v2 - v1;
    vec3 posB = v3 - v1;

    vec2 uvA = uv2 - uv1;
    vec2 uvB = uv3 - uv1;

    float div = (uvA.x * uvB.y - uvB.x * uvA.y);

    // Fix +/- infinity from division by 0.
    float r = 1.0f;
    if (div != 0)
        r = 1.0f / div;

    return CalculateBitangent(posA, posB, uvA, uvB, r);
}

void main()
{
    vec3 distances = EdgeDistances();

    vec3 newBitangent = CalculateBitangent(gl_in[0].gl_Position.xyz,
        gl_in[1].gl_Position.xyz, gl_in[2].gl_Position.xyz, geomUV0[0], geomUV0[1], geomUV0[2]);

    // Create a triangle and assign the vertex attributes.
    for (int i = 0; i < 3; i++)
    {
        gl_Position = gl_in[i].gl_Position;
        N = geomN[i];
        tangent = geomTangent[i];

        bitangent = newBitangent;
        bitangent = normalize(bitangent - N * dot(N, bitangent));

        UV0 = geomUV0[i];
        colorSet = geomColorSet[i];
        bakeColor = geomBakeColor[i];

        // The distance from a point to each of the edges.
        if (i == 0)
            edgeDistance = vec3(distances.x, 0, 0);
        else if (i == 1)
            edgeDistance = vec3(0, distances.y, 0);
        else if (i == 2)
            edgeDistance = vec3(0, 0, distances.z);

        EmitVertex();
    }

    EndPrimitive();
}
