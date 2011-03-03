//------- Constants --------
float4x4 xView : VIEW;
float4x4 xProjection : PROJECTION;
float4x4 xWorld : WORLD;
float4x4 WorldViewProj : WORLDVIEWPROJECTION;
float4x4 WorldView : WORLDVIEW;
float3 LightDirection;
float4 AmbientColor;
float AmbientPower;
float3 SpecularColor;
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
sampler TextureMapSampler = sampler_state { texture = <TextureMap> ; magfilter = ANISOTROPIC; minfilter = ANISOTROPIC; 
                                                                         mipfilter = ANISOTROPIC; AddressU  = Wrap;
                                                                         AddressV  = Wrap; AddressW  = Wrap;};

Texture GrassTexture;
sampler GrassTextureSampler = sampler_state { texture = <GrassTexture> ; magfilter = ANISOTROPIC; minfilter = ANISOTROPIC; 
                                                                         mipfilter = ANISOTROPIC; AddressU  = Wrap;
                                                                         AddressV  = Wrap; AddressW  = Wrap;};

Texture SandTexture;
sampler SandTextureSampler = sampler_state { texture = <SandTexture> ; magfilter = ANISOTROPIC; minfilter = ANISOTROPIC; 
                                                                       mipfilter = ANISOTROPIC; AddressU  = Wrap;
                                                                       AddressV  = Wrap; AddressW  = Wrap;};

Texture RockTexture;
sampler RockTextureSampler = sampler_state { texture = <RockTexture> ; magfilter = ANISOTROPIC; minfilter = ANISOTROPIC; 
                                                                       mipfilter = ANISOTROPIC; AddressU  = Wrap;
                                                                       AddressV  = Wrap; AddressW  = Wrap;};

Texture GrassNormal;
sampler2D GrassNormalSampler : TEXUNIT1 = sampler_state
{ Texture   = (GrassNormal); magfilter = ANISOTROPIC; minfilter = ANISOTROPIC; 
                             mipfilter = ANISOTROPIC; AddressU  = Wrap;
                             AddressV  = Wrap; AddressW  = Wrap;};

Texture SandNormal;
sampler2D SandNormalSampler : TEXUNIT1 = sampler_state
{ Texture   = (SandNormal); magfilter  = ANISOTROPIC; minfilter = ANISOTROPIC; 
                             mipfilter = ANISOTROPIC; AddressU  = Wrap;
                             AddressV  = Wrap; AddressW  = Wrap;};

Texture RockNormal;
sampler2D RockNormalSampler : TEXUNIT1 = sampler_state
{ Texture   = (RockNormal); magfilter = ANISOTROPIC; minfilter = ANISOTROPIC; 
                             mipfilter = ANISOTROPIC; AddressU  = Wrap;
                             AddressV  = Wrap; AddressW  = Wrap;};

//------- Technique: MultiTexturedNormaled --------
 
 struct VS_INPUT
 {
     float4 Position            : POSITION;    
     float3 Normal              : NORMAL0;    
     float3 Tangent            : TANGENT0;	// Needed for accurate normal mapping
     float3 Binormal             : BINORMAL0;		// Needed for accurate normal mapping
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
    float Fog                  : FOG;
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
	 Output.Fog = 1 - saturate(length(mul(input.Position, WorldView)) / FOGDIST);
     return Output;    
 }
 
 float4 MultiTexturedNormaledPS(VS_OUTPUT input) : COLOR0
 {
	 float3 TerrainColorWeight = tex2D(TextureMapSampler, input.texCoordNoWrap);
	 
	 input.Normal = normalize(input.Normal);
 
     float3 normalFromMap = (2.0f + tex2D(SandNormalSampler, input.texCoord) - 1.0f) * TerrainColorWeight.r;
     normalFromMap += (2.0f + tex2D(GrassNormalSampler, input.texCoord) - 1.0f) * TerrainColorWeight.g;
     normalFromMap += (2.0f + tex2D(RockNormalSampler, input.texCoord) - 1.0f)  * TerrainColorWeight.b;
     normalFromMap = normalize(mul(normalFromMap, float3x3(input.Tangent, input.Binormal, input.Normal)) * 0.15f);
	
     // Factor in normal mapping and terrain vertex normals as well in lighting of the pixel
     //float lightingFactor = saturate(mul(2.0, dot(normalFromMap + input.Normal, LightDirection)));
	 float lightingFactor = mul(1.0, dot(normalFromMap + input.Normal, LightDirection));
     float4 Color = tex2D(SandTextureSampler, input.texCoord)   * TerrainColorWeight.r;
     Color += tex2D(GrassTextureSampler, input.texCoord) * TerrainColorWeight.g;
     Color += tex2D(RockTextureSampler, input.texCoord)  * TerrainColorWeight.b;

     float3 Reflect = saturate((lightingFactor * input.Normal) + (-LightDirection));
     float3 specular = pow(dot(Reflect, CameraForward), SpecularPower);
     
	 Color.rgb *= (AmbientColor + (DiffuseColor * lightingFactor) + (SpecularColor * specular * lightingFactor)) * AmbientPower;
	 Color.a = 1.0f;
 
     return Color;
 }
 
 technique MultiTexturedNormaled
 {
     pass Pass0
     {
     	FogEnable = (FOGENABLE);
		FogVertexMode = NONE;
		FogColor = (FOGCOLOR);
        VertexShader = compile vs_2_0 MultiTexturedNormaledVS();
        PixelShader = compile ps_2_0 MultiTexturedNormaledPS();
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
		FogEnable = (FOGENABLE);
		FogVertexMode = NONE;
		FogColor = (FOGCOLOR);
         VertexShader = compile vs_3_0 MultiTexturedVS();
         PixelShader = compile ps_3_0 MultiTexturedPS();
     }
 }