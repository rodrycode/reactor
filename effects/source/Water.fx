//Water effect shader that uses reflection and refraction maps projected onto the water.
//These maps are distorted based on the two scrolling normal maps.

float4x4 World;
float4x4 View;
float4x4 WorldViewProj;

float4  WaterColor;
float3	SunDirection;
float4  SunColor;
float	SunFactor; //the intensity of the sun specular term.
float   SunPower; //how shiny we want the sun specular term on the water to be.
float3  EyePos;

// Texture coordinate offset vectors for scrolling
// normal maps.
float2  WaveMapOffset0;
float2  WaveMapOffset1;
bool FOGENABLE;
float FOGDIST;
float4 FOGCOLOR = float4(1, 1, 1, 1.0);
// Two normal maps and the reflection/refraction maps
texture WaveMap0;
texture WaveMap1;
texture ReflectMap;
texture RefractMap;
float WaterLevel = 0.0f;
//scale used on the wave maps
float TexScale;

static const float	  R0 = 0.02037f;

sampler WaveMapS0 = sampler_state
{
	Texture = <WaveMap0>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
	AddressU  = WRAP;
    AddressV  = WRAP;
};

sampler WaveMapS1 = sampler_state
{
	Texture = <WaveMap1>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
	AddressU  = WRAP;
    AddressV  = WRAP;
};

sampler ReflectMapS = sampler_state
{
	Texture = <ReflectMap>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
	AddressU  = CLAMP;
    AddressV  = CLAMP;
};

sampler RefractMapS = sampler_state
{
	Texture = <RefractMap>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
	AddressU  = CLAMP;
    AddressV  = CLAMP;
};

struct OutputVS
{
    float4 posH			: POSITION0;
    float3 toEyeW		: TEXCOORD0;
    float2 tex0			: TEXCOORD1;
    float2 tex1			: TEXCOORD2;
    float4 projTexC		: TEXCOORD3;
    float4 pos			: TEXCOORD4;
    float2 Fog           : TEXCOORD5;
};

OutputVS WaterVS( float3 posL	: POSITION0, 
                  float2 texC   : TEXCOORD0)
{
    // Zero out our output.
	OutputVS outVS = (OutputVS)0;
	
	// Transform vertex position to world space.
	float3 posW  = mul(float4(posL, 1.0f), World).xyz;
	outVS.pos.xyz = posW;
	outVS.pos.w = 1.0f;
	
	// Compute the unit vector from the vertex to the eye.
	outVS.toEyeW = posW - EyePos;
	
	// Transform to homogeneous clip space.
	outVS.posH = mul(float4(posL, 1.0f), WorldViewProj);
	
	// Scroll texture coordinates.
	outVS.tex0 = (texC * TexScale) + WaveMapOffset0;
	outVS.tex1 = (texC * TexScale) + WaveMapOffset1;
	
	// Generate projective texture coordinates from camera's perspective.
	outVS.projTexC = outVS.posH;
	outVS.Fog = float2(1.0 - saturate(length(posW) / FOGDIST),0);
	// Done--return the output.
    return outVS;
}

float4 WaterPS( float3 toEyeW		: TEXCOORD0,
				float2 tex0			: TEXCOORD1,
				float2 tex1			: TEXCOORD2,
				float4 projTexC		: TEXCOORD3,
				float4 pos			: TEXCOORD4,
				float2 fog          : TEXCOORD5) : COLOR
{
	//transform the projective texcoords to NDC space
	//and scale and offset xy to correctly sample a DX texture
	projTexC.xyz /= projTexC.w;            
	projTexC.x =  0.5f*projTexC.x + 0.5f; 
	projTexC.y = -0.5f*projTexC.y + 0.5f;
	projTexC.z = .1f / projTexC.z; //refract more based on distance from the camera
	
	toEyeW    = normalize(toEyeW);
	
	// Light vector is opposite the direction of the light.
	float3 lightVecW = -SunDirection;
	
	// Sample normal map.
	float3 normalT0 = tex2D(WaveMapS0, tex0);
	float3 normalT1 = tex2D(WaveMapS1, tex1);
    
    //unroll the normals retrieved from the normalmaps
    normalT0.yz = normalT0.zy;	
	normalT1.yz = normalT1.zy;
	
	normalT0 = 2.0f*normalT0 - 1.0f;
    normalT1 = 2.0f*normalT1 - 1.0f;
    
	float3 normalT = normalize(0.5f*(normalT0 + normalT1));
	float3 n1 = float3(0,1,0); //we'll just use the y unit vector for spec reflection.
	
	//get the reflection vector from the eye
	float3 R = normalize(reflect(toEyeW,normalT));
	
	float4 finalColor;
	finalColor.a = 1;

	//compute the fresnel term to blend reflection and refraction maps
	float ang = saturate(dot(-toEyeW,n1));
	float f = R0 + (1.0f-R0) * pow(1.0f-ang,5.0);	
	
	//also blend based on distance
	f = min(1.0f, f + 0.007f * EyePos.y);	
		
	//compute the reflection from sunlight
	float sunFactor = SunFactor;
	float sunPower = SunPower;
	
	
	float3 sunlight = sunFactor * pow(saturate(dot(R, lightVecW)), sunPower) * SunColor;

	float4 refl = tex2D(ReflectMapS, projTexC.xy + projTexC.z * normalT.xz);
	float4 refr = tex2D(RefractMapS, projTexC.xy - projTexC.z * normalT.xz);
	
	//only use the refraction map if we're under water
	if(EyePos.y < WaterLevel)
		f = 0.0f;
	
	//interpolate the reflection and refraction maps based on the fresnel term and add the sunlight
	finalColor.rgb = WaterColor * lerp( refr, refl, f) + sunlight;
	//finalColor.a = lerp(finalColor.a, 0.0, 1-(fog.x));
	//if(FOGENABLE)
		//finalColor.rgb = lerp(WaterColor, FOGCOLOR, 1-(fog.x)) * (lerp(lerp( refr, refl, f),FOGCOLOR, 1-(fog.x))) + sunlight;
	
		return finalColor;
}

technique WaterTech
{
    pass Pass1
    {
    
        // Specify the vertex and pixel shader associated with this pass.
#ifdef SM4
        VertexShader = compile vs_4_0_level_9_1 WaterVS();
        PixelShader = compile ps_4_0_level_9_1 WaterPS();
	#else
		VertexShader = compile vs_3_0 WaterVS();
        PixelShader = compile ps_3_0 WaterPS();
	#endif
    }    
}
