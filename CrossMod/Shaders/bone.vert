#version 330

in vec4 point;

uniform mat4 mvp;
uniform mat4 bone;
uniform mat4 parent;
uniform mat4 rotation;
uniform int hasParent;

const float scale = 0.4;

void main()
{
	vec4 position = bone * rotation *vec4(point.xyz*scale, 1);
	if(hasParent)
	{
		if(point.w == 0){
			position = parent * rotation *vec4(point.xyz*scale, 1);
		}else{
			position = bone * rotation *vec4((point.xyz-vec3(0, 1, 0))*scale, 1);
		}
	}
	// * rotationMatrix(vec3(0, 0, 1), 1.5708) *
    gl_Position = mvp * vec4(position.xyz, 1);
}