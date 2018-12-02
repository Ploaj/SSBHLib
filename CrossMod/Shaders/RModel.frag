#version 330

in vec3 N;
in vec2 UV0;

out vec4 fragColor;

uniform sampler2D _col;

void main()
{
	fragColor = texture2D(_col, UV0);//vec4(N * 0.5 + 0.5, 1);
}