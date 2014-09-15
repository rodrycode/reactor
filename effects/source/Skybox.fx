Texture surfaceTexture;
samplerCUBE TextureSampler = sampler_state 
{ 
    texture = <surfaceTexture> ; 
    magfilter = ANISOTROPIC; 
    minfilter = ANISOTROPIC; 
    mipfilter = ANISOTROPIC; 
    AddressU = Mirror;
    AddressV = Mirror;
};

float4x4 World : World;
float4x4 View : View;
float4x4 Projection : Projection;
float TextureScale = 1.0;
float3 EyePosition : CameraPosition;
bool Reflected = false;
struct VS_INPUT 
{
    float4 Position    : POSITION0;
    float3 Normal : NORMAL0;    
    float2 TexCoord : TEXCOORD0;
    float3 Color : COLOR0;
};

struct VS_OUTPUT 
{
    float4 Position    : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float3 ViewDirection : TEXCOORD1;
    float3 Color : COLOR0;
};

float4 CubeMapLookup(float3 CubeTexcoord)
{    
    return texCUBE(TextureSampler, CubeTexcoord);
}

VS_OUTPUT Transform(VS_INPUT Input)
{
    float4x4 WorldViewProjection = mul(mul(World, View), Projection);
    float4 ObjectPosition = mul(Input.Position, World);
    
    VS_OUTPUT Output;
    Output.Position    = mul(Input.Position, WorldViewProjection);
    //Output.Position = ObjectPosition;
    
    Output.ViewDirection = ObjectPosition - EyePosition;    
    
    Output.TexCoord = Input.TexCoord;
    Output.Color = float3(1.0,1.0,1.0);
    return Output;
}

struct PS_INPUT 
{   
	float2 TexCoord : TEXCOORD0; 
    float3 ViewDirection : TEXCOORD1;
};

float4 BasicShader(PS_INPUT Input) : COLOR0
{    
    //float3 ViewDirection = normalize(Input.ViewDirection);
    float3 ViewDirection = Input.ViewDirection;     
    ViewDirection.x *= -1;
    if(Reflected)
		ViewDirection.y *= -1;
	float4 tex = TextureScale * texCUBE(TextureSampler, ViewDirection);
	
    return tex;
}

technique normal 
{
    pass cubemap
    {

#ifdef SM4
        VertexShader = compile vs_4_0_level_9_1 Transform();
        PixelShader = compile ps_4_0_level_9_1 BasicShader();
	#else
		VertexShader = compile vs_3_0 Transform();
        PixelShader = compile ps_3_0 BasicShader();
	#endif
    }
}