#version 330

vec3 GetBitangent(vec3 normal, vec3 tangent, float tangentSign)
{
    // Flip after normalization to avoid issues with tangentSign being 0.0.
    // Smash Ultimate requires Tangent0.W to be flipped.
    return normalize(cross(normal.xyz, tangent.xyz)) * tangentSign * -1;
}

vec3 GetBumpMapNormal(vec3 normal, vec3 tangent, vec3 bitangent, vec4 norColor)
{
    // Remap the normal map to the correct range.
    float x = 2 * norColor.x - 1.0;
    float y = 2 * norColor.y - 1.0;

    // Calculate z based on the fact that x*x + y*y + z*z = 1.
    // Clamp to prevent z being 0.0.
    float z = sqrt(max(1 - (x * x) + (y * y), 0.001));

    vec3 normalMapNormal = vec3(x, y, z);

    mat3 tbnMatrix = mat3(tangent, bitangent, normal);

    vec3 newNormal = tbnMatrix * normalMapNormal;
    return normalize(newNormal);
}
