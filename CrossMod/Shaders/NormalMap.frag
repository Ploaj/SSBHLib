#version 330

vec3 GetBumpMapNormal(vec3 N, vec3 tangent, vec3 bitangent, vec4 norColor)
{
    // Calculate the resulting normal map.
    // TODO: Proper calculation of B channel.
    vec3 normalMapColor = vec3(norColor.rg, 1);

    // Remap the normal map to the correct range.
    vec3 normalMapNormal = 2.0 * normalMapColor - vec3(1);

    // TBN Matrix.
    mat3 tbnMatrix = mat3(tangent, bitangent, N);

    vec3 newNormal = tbnMatrix * normalMapNormal;
    return normalize(newNormal);
}
