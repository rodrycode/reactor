//------- Constants --------
float4x4 xView : VIEW;
float4x4 xProjection : PROJECTION;
float4x4 xWorld : WORLD;
float4x4 WorldViewProj : WORLDVIEWPROJECTION;
float4x4 WorldView : WORLDVIEW;
float3 LightDirection;
float4 AmbientColor;
float AmbientPower;
float4 SpecularColor;
float SpecularPower;
float4 DiffuseColor;
float3 CameraForward;
bool FOGENABLE;
float FOGDIST;
float4 FOGCOLOR = float4(1, 1, 1, 0.5);
float TerrainScale;
float TerrainWidth;

//------- Texture Samplers --------
Texture TextureMap;
sampler TextureMapSampler = sampler_state { 
	texture = <TextureMap>;
	magfilter = ANISOTROPIC;
	minfilter = ANISOTROPIC;
	AddressU  = Wrap;
    AddressV  = Wrap;
	AddressW  = Wrap;
};

Texture GrassTexture;
sampler GrassTextureSampler = sampler_state { 
	texture = <GrassTexture>;
	magfilter = ANISOTROPIC;
	minfilter = ANISOTROPIC;
	AddressU  = Wrap;
    AddressV  = Wrap;
	AddressW  = Wrap;
};

Texture SandTexture;
sampler SandTextureSampler = sampler_state { 
	texture = <SandTexture>;
	magfilter = ANISOTROPIC;
	minfilter = ANISOTROPIC;
	AddressU  = Wrap;
    AddressV  = Wrap;
	AddressW  = Wrap;
};

Texture RockTexture;
sampler RockTextureSampler = sampler_state { 
	texture = <RockTexture>;
	magfilter = ANISOTROPIC;
	minfilter = ANISOTROPIC;
	AddressU  = Wrap;
    AddressV  = Wrap;
	AddressW  = Wrap;
};

Texture GrassNormal;
sampler2D GrassNormalSampler = sampler_state
{ Texture   = (GrassNormal); magfilter = ANISOTROPIC; minfilter = ANISOTROPIC; 
                             AddressU  = Wrap;
                             AddressV  = Wrap; AddressW  = Wrap;};

Texture SandNormal;
sampler2D SandNormalSampler = sampler_state
{ Texture   = (SandNormal); magfilter  = ANISOTROPIC; minfilter = ANISOTROPIC; 
                             AddressU  = Wrap;
                             AddressV  = Wrap; AddressW  = Wrap;};

Texture RockNormal;
sampler2D RockNormalSampler = sampler_state
{ Texture   = (RockNormal); magfilter = ANISOTROPIC; minfilter = ANISOTROPIC; 
                             AddressU  = Wrap;
                             AddressV  = Wrap; AddressW  = Wrap;};

Texture HeightMap;
sampler HeightMapSampler = sampler_state { texture = <HeightMap> ; magfilter = Linear; minfilter = Linear; 
                                                                       AddressU  = Clamp;
                                                                       AddressV  = Clamp; AddressW  = Clamp;};
                                                                       
Texture NormalMap;
sampler NormalMapSampler = sampler_state { texture = <NormalMap> ; magfilter = Linear; minfilter = Linear; 
                                                                       AddressU  = Clamp;
                                                                       AddressV  = Clamp; AddressW  = Clamp;};

float normalStrength = 1.0f;
float textureSize = 512;
//------- Technique: MultiTexturedNormaled --------
 
 struct VS_INPUT
 {
     float4 Position            : POSITION;    
     float3 Normal              : NORMAL0;    
     float3 Tangent             : TANGENT0;	// Needed for accurate normal mapping
     float3 Binormal            : BINORMAL0;		// Needed for accurate normal mapping
     float4 Color               : COLOR0;
 };

struct VS_OUTPUT
{
    float4 position            : POSITION;
    float2 texCoord            : TEXCOORD0;
    float2 texCoordNoWrap	   : TEXCOORD1;
    float3 Binormal			   : TEXCOORD2;
    float3 Tangent			   : TEXCOORD3;
    float3 Normal			   : TEXCOORD4;
    float2 Fog                 : TEXCOORD5;
};

 VS_OUTPUT MultiTexturedNormaledVS( VS_INPUT input)    
 {
     VS_OUTPUT Output;

     // These estimated values should work on terrain that isn't steep,
     // while steep terrain will not be fully normal mapped.
     //float3 Binormal = (0.0f, 0.0f, 0.5f);
     //float3 Tangent = (0.5f, 0.0f, 0.0f);
     
     Output.position = mul(input.Position, WorldViewProj);
     Output.Normal = mul(input.Normal, xWorld);

     //float4 worldSpacePos = mul(input.Position, xWorld);

     Output.texCoord.x = input.Position.x * 0.1f / TerrainScale;
     Output.texCoord.y = input.Position.z * 0.1f / TerrainScale;
     //Output.texCoord.x = input.Position.x / (TerrainWidth * TerrainScale);
     //Output.texCoord.y = input.Position.z / (TerrainWidth * TerrainScale);
     Output.texCoordNoWrap.x = input.Position.z / (TerrainWidth * TerrainScale);
     Output.texCoordNoWrap.y = input.Position.x / (TerrainWidth * TerrainScale);
     
     //Output.texCoordNoWrap.x = input.Position.x * .01f / TerrainScale;
     //Output.texCoordNoWrap.y = input.Position.z * .01f / TerrainScale;
     
     Output.Tangent = mul(input.Tangent,   xWorld);
	 Output.Binormal = mul(input.Binormal,  xWorld);
	 //Output.Tangent = input.Tangent;
	 //Output.Binormal = input.Binormal;
	 Output.Fog = float2(1 - saturate(length(mul(input.Position, WorldView)) / FOGDIST),0);
     return Output;    
 }
 
 float4 MultiTexturedNormaledPS(VS_OUTPUT input) : COLOR0
 {
	 float3 TerrainColorWeight = tex2D(TextureMapSampler, input.texCoordNoWrap);
	 
	 input.Normal = normalize(input.Normal);
	 //float3 normalFromMap = (2.0*tex2D(NormalMapSampler, input.texCoordNoWrap)-1.0);
     float3 normalFromMap = (2.0*tex2D(SandNormalSampler, input.texCoord)-0.5) * TerrainColorWeight.r;
     normalFromMap += (2.0*tex2D(GrassNormalSampler, input.texCoord)-0.5) * TerrainColorWeight.g;
     normalFromMap += (2.0*tex2D(RockNormalSampler, input.texCoord)-0.5)  * TerrainColorWeight.b;
     normalFromMap = normalize(mul(normalFromMap, float3x3(input.Tangent, input.Binormal, input.Normal)) * 1.5f);
	
     // Factor in normal mapping and terrain vertex normals as well in lighting of the pixel
     //float lightingFactor = saturate(mul(2.0, dot(normalFromMap + input.Normal, LightDirection)));
	 float lightingFactor = dot(normalFromMap + input.Normal, -LightDirection);
     float4 Color = tex2D(SandTextureSampler, input.texCoord)   * TerrainColorWeight.r;
     Color += tex2D(GrassTextureSampler, input.texCoord) * TerrainColorWeight.g;
     Color += tex2D(RockTextureSampler, input.texCoord)  * TerrainColorWeight.b;

     float3 Reflect = ((lightingFactor) * input.Normal) + (LightDirection);
     float3 specular = pow(abs(normalize(dot(Reflect, CameraForward))), SpecularPower);
     
	 Color.rgb *= ((AmbientColor + (DiffuseColor * lightingFactor) + (SpecularColor * specular * lightingFactor)) * AmbientPower);
	 //Color.rgb *= ((AmbientColor + (DiffuseColor * lightingFactor)) * AmbientPower);
	 Color.a = 1.0f;
	 if(FOGENABLE)
     return lerp(Color, FOGCOLOR, 1-input.Fog.x);
     else
     return Color;
 }
 
 technique MultiTexturedNormaled
 {
     pass Pass0
     {
     	#ifdef SM4
        VertexShader = compile vs_4_0_level_9_1 MultiTexturedNormaledVS();
        PixelShader = compile ps_4_0_level_9_1 MultiTexturedNormaledPS();
	#else
		VertexShader = compile vs_3_0 MultiTextureNormaledVS();
        PixelShader = compile ps_3_0 MultiTexturedNormaledPS();
	#endif
        
        
     }
 }
 float4 ComputeNormalsPS(in float2 uv : TEXCOORD0) : COLOR
{
	float texelSize = 1.0f / textureSize;
	
	// Top left
	float tl = abs(tex2D(HeightMapSampler, uv + texelSize * float2(-1, 1)).x);
	
	// Left
	float l = abs(tex2D(HeightMapSampler, uv + texelSize * float2(-1, 0)).x);
	
	// Bottom Left
	float bl = abs(tex2D(HeightMapSampler, uv + texelSize * float2(-1, -1)).x);
	
	// Top
	float t = abs(tex2D(HeightMapSampler, uv + texelSize * float2(0, 1)).x);
	
	// Bottom
	float b = abs(tex2D(HeightMapSampler, uv + texelSize * float2(0, -1)).x);
	
	// Top Right
	float tr = abs(tex2D(HeightMapSampler, uv + texelSize * float2(1, 1)).x);
	
	// Right
	float r = abs(tex2D(HeightMapSampler, uv + texelSize * float2(1, 0)).x);
	
	// Bottom Right
	float br = abs(tex2D(HeightMapSampler, uv + texelSize * float2(1, -1)).x);
	
	float dx = -tl - 2.0f * l - bl + tr + 2.0f * r + br;
	float dy = -tl - 2.0f * t - tr + bl + 2.0f * b + br;
	
	float4 normal = float4(normalize(float3(dx, 1.0f / normalStrength, dy)), 1.0f);
	
	// Convert coordinates from range (-1,1) to range (0,1)
	return normal * 0.5f + 0.5f;
}
 technique ComputeNormals
{
	pass P0
	{
	#ifdef SM4
        PixelShader = compile ps_4_0_level_9_1 ComputeNormalsPS();
	#else
		PixelShader = compile ps_3_0 ComputeNormalsPS();
	#endif
		
	}
}
 // ================================================
 //------- Technique: MultiTextured --------
 
 struct VSBASIC_INPUT
 {
     float4 Position            : POSITION0;    
     float3 Normal              : NORMAL0;    
 };

struct VSBASIC_OUTPUT
{
    float4 position            : POSITION0;
    float2 texCoord            : TEXCOORD0;
    float2 texCoordNoWrap	   : TEXCOORD1;
    float3 Normal			   : TEXCOORD5;
    float Fog                  : FOG;
};

 VSBASIC_OUTPUT MultiTexturedVS( VSBASIC_INPUT input)    
 {
     VSBASIC_OUTPUT Output;
     
     Output.position = mul(input.Position, WorldViewProj);
     Output.Normal = mul(input.Normal, xWorld);

     float4 worldSpacePos = mul(input.Position, xWorld);

     Output.texCoord.x = input.Position.x * .01f / TerrainScale;
     Output.texCoord.y = input.Position.z * .01f / TerrainScale;
     Output.texCoordNoWrap.x = input.Position.x / (TerrainWidth * TerrainScale);
     Output.texCoordNoWrap.y = input.Position.z / (TerrainWidth * TerrainScale);
	 Output.Fog = 1 - saturate(length(mul(input.Position, WorldView)) / FOGDIST);
     return Output;    
 }
 
 float4 MultiTexturedPS(VSBASIC_OUTPUT input) : COLOR0
 {
	 float3 TerrainColorWeight = tex2D(TextureMapSampler, input.texCoordNoWrap);
	 
	 input.Normal = normalize(input.Normal);

     // Factor in normal mapping and terrain vertex normals as well in lighting of the pixel
     float lightingFactor = saturate(dot(input.Normal, -LightDirection));

     // Multi-texture blending occurs in these three lines
     float4 Color = tex2D(SandTextureSampler, input.texCoord)   * TerrainColorWeight.r;
     Color += tex2D(GrassTextureSampler, input.texCoord) * TerrainColorWeight.g;
     Color += tex2D(RockTextureSampler, input.texCoord)  * TerrainColorWeight.b;

     float3 Reflect = (lightingFactor * input.Normal) + (-LightDirection);
     float3 specular = pow(saturate(dot(Reflect, -CameraForward)), SpecularPower);
     
	 Color.rgb *= (AmbientColor + (DiffuseColor * lightingFactor) + (SpecularColor * specular * lightingFactor)) * AmbientPower;
	 Color.a = 1.0f;
 
     return Color;
 }
 
 technique MultiTextured
 {
     pass Pass0
     {
		#ifdef SM4
        VertexShader = compile vs_4_0_level_9_1 MultiTexturedVS();
        PixelShader = compile ps_4_0_level_9_1 MultiTexturedPS();
	#else
		VertexShader = compile vs_3_0 MultiTexturedVS();
        PixelShader = compile ps_3_0 MultiTexturedPS();
	#endif

         
         
     }
 }