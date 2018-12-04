#version 330

in vec3 Position0;
in vec3 Tangent0;
in vec3 Normal0;
in vec3 colorSet1;
in vec3 bake1;
in vec2 map1;
in ivec4 Bone;
in vec4 Weight;

uniform mat4 mvp;
uniform mat4 transform;

uniform bones
{
    mat4 transforms[200];
} bones_;

out vec3 N;
out vec3 tangent;
out vec2 UV0;
out vec3 vertexColor;
out vec3 bakeColor;

void main()
{
	vec4 transformedNormal = transform * vec4(Normal0, 0);
	N = transformedNormal.rgb;

	vertexColor = colorSet1;
	bakeColor = bake1;

	UV0 = map1;
	tangent = Tangent0;

	// default
	vec4 position = vec4(Position0, 1);

	//single bind
	position = transform * vec4(Position0, 1);


	// skin
	if(Weight.x != 0) position = bones_.transforms[Bone.x] * vec4(Position0, 1) * Weight.x;
	if(Weight.y != 0) position += bones_.transforms[Bone.y] * vec4(Position0, 1) * Weight.y;
	if(Weight.z != 0) position += bones_.transforms[Bone.z] * vec4(Position0, 1) * Weight.z;
	if(Weight.w != 0) position += bones_.transforms[Bone.w] * vec4(Position0, 1) * Weight.w;

	gl_Position = mvp * vec4(position.xyz, 1);
}
