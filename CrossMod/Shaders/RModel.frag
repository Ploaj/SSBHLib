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

void main()
{
	vec3 V = vec3(0,0,-1) * mat3(mvp);

	// TODO: Accessing unitialized textures may cause crashes.
	vec4 albedoColor = texture(colMap, UV0).rgba;
	vec4 prmColor = texture(prmMap, UV0).xyzw;
	vec4 norColor = texture(norMap, UV0).xyzw;

	fragColor = albedoColor * LambertShading(N, V);

	fragColor.rgb = GetSrgb(fragColor.rgb);
}
