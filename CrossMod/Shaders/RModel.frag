#version 330

in vec3 N;
in vec2 UV0;

out vec4 fragColor;

uniform sampler2D colMap;
uniform sampler2D prmMap;
uniform sampler2D norMap;

uniform mat4 mvp;

float LambertShading(vec3 N, vec3 V)
{
	float lambert = max(dot(N, V), 0);
	return lambert;
}

vec3 GetSrgb(vec3 linear)
{
	return pow(linear, vec3(0.4545));
}

float GgxShading(vec3 N, vec3 H, float roughness)
{
	float a = roughness * roughness;
    float a2 = a * a;
    float nDotH = max(dot(N, H), 0.0);
    float nDotH2 = nDotH * nDotH;

    float numerator = a2;
    float denominator = (nDotH2 * (a2 - 1.0) + 1.0);
    denominator = 3.14159 * denominator * denominator;

    return numerator / denominator;
}

void main()
{
	vec3 V = vec3(0,0,-1) * mat3(mvp);

	// TODO: Accessing unitialized textures may cause crashes.
	vec4 albedoColor = texture(colMap, UV0).rgba;
	vec4 prmColor = texture(prmMap, UV0).xyzw;
	vec4 norColor = texture(norMap, UV0).xyzw;

	fragColor = albedoColor;
	fragColor.rgb *= LambertShading(N, V);

	// Invert glossiness?
	float roughness = clamp(prmColor.g - 1, 0, 1);
	fragColor.rgb += GgxShading(N, V, 0.5) * prmColor.a;

	fragColor.rgb = GetSrgb(fragColor.rgb);
}
