// Originally from; http://otter2d.com/example.php?p=18

uniform sampler2D texture;
uniform float time;

// A weird way to generate a random number with a vec2 seed I guess.
float rand( vec2 co )
{
	return fract( sin( dot( co.xy, vec2( 12.9898, 78.233 ) ) ) * 43758.5453 );
}

void main()
{
	vec2 samplePos = gl_TexCoord[0].xy;
	samplePos.x -= smoothstep( 1, 0, time ) * rand( samplePos ) * 10;
	vec4 pixel = texture2D( texture, samplePos );

	gl_FragColor = pixel * gl_Color;
}