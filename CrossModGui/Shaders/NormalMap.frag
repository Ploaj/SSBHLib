#version 330

vec3 GetBitangent(vec3 normal, vec3 tangent, float tangentSign)
{
    // Flip after normalization to avoid issues with tangentSign being 0.0.
    // Smash Ultimate requires Tangent0.W to be flipped.
    return normalize(cross(normal.xyz, tangent.xyz)) * tangentSign * -1;
}

vec3 GetBumpMapNormal(vec3 normal, vec3 tangent, vec3 bitangent, vec4 norColor)
{
    // Calculate the resulting normal map.
    float x = 2 * norColor.x - 1;
    float y = 2 * norColor.y - 1;
    float z = sqrt(1 - ((x * x) + (y * y))) * 0.5 + 0.5;
    vec3 normalMapColor = vec3(norColor.rg, z);

    // Remap the normal map to the correct range.
    vec3 normalMapNormal = 2.0 * normalMapColor - vec3(1);

    mat3 tbnMatrix = mat3(tangent, bitangent, normal);

    vec3 newNormal = tbnMatrix * normalMapNormal;
    return normalize(newNormal);
}
