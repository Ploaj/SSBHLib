#version 330

in vec3 Position0;
in vec3 Tangent0;
in vec3 Normal0;
in vec4 colorSet1;
in vec4 colorSet3;
in vec4 colorSet4;
in vec3 bake1;

in vec2 map1;
in vec2 uvSet;
in vec2 uvSet1;
in vec2 uvSet2;

in ivec4 Bone;
in vec4 Weight;

uniform mat4 mvp;
uniform mat4 transform;

uniform bones
{
    mat4 transforms[200];
} bones_;

out vec3 geomN;
out vec3 geomTangent;
out vec2 geomUV0;
out vec4 geomColorSet;
out vec3 geomBakeColor;

void main()
{
	vec4 transformedNormal = transform * vec4(Normal0, 0);
	geomN = transformedNormal.rgb;

	geomColorSet = colorSet1;
	geomBakeColor = bake1;

	geomUV0 = map1;
	geomTangent = Tangent0;

	// default
	vec4 position = vec4(Position0, 1);

	//single bind
	position = transform * vec4(Position0, 1);


	// skin
	if(Weight.x != 0) {
		position = bones_.transforms[Bone.x] * vec4(Position0, 1) * Weight.x;
		geomN.xyz = (inverse(transpose(bones_.transforms[Bone.x])) * vec4(Normal0, 1) * Weight.x).xyz;
	}
	if(Weight.y != 0) {
		position += bones_.transforms[Bone.y] * vec4(Position0, 1) * Weight.y;
		geomN.xyz += (inverse(transpose(bones_.transforms[Bone.y])) * vec4(Normal0, 1) * Weight.y).xyz;
	}
	if(Weight.z != 0) {
		position += bones_.transforms[Bone.z] * vec4(Position0, 1) * Weight.z;
		geomN.xyz += (inverse(transpose(bones_.transforms[Bone.z])) * vec4(Normal0, 1) * Weight.z).xyz;
	}
	if(Weight.w != 0) {
		position += bones_.transforms[Bone.w] * vec4(Position0, 1) * Weight.w;
		geomN.xyz += (inverse(transpose(bones_.transforms[Bone.w])) * vec4(Normal0, 1) * Weight.w).xyz;
	}

	gl_Position = mvp * vec4(position.xyz, 1);
}
