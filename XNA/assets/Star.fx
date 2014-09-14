//float4x3 World[52] : MINIMESH_WORLD;
float4x4 World     : WORLD;
float4x4 ViewProj  : VIEWPROJECTION;
//float4   Colour[52]: MINIMESH_COLOR;
float3   cp        : VIEWPOS;
texture  colour    : TEXTURE0;
texture  bump      : TEXTURE1;

float3 AllowedRotDir = float3(1.0,1.0,1.0);
float3 LightPos;
float  Radius = 1000000;

sampler2D cs = sampler_state
{
	Texture   = (colour);
	MIPFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
};

sampler2D bs = sampler_state
{
	Texture   = (bump);
	MIPFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
};

struct BBVertexToPixel
{
	float4 Position: POSITION;
	float2 TexCoord: TEXCOORD0;
	//float2 Index   : TEXCOORD3;
};

struct BBPixelToFrame
{
	float4 Position: POSITION;
	float2 TexCoord : TEXCOORD0;
};

//------- Technique: CylBillboard --------
BBPixelToFrame CylBillboardVS(in BBVertexToPixel IN)
{
    BBPixelToFrame o = (BBPixelToFrame)0;
	//float4x3 world = World[IN.Index.x];
	
    /*float3 center = mul(IN.Position, World);
    float3 eyeVector = center - cp;

    float3 upVector = AllowedRotDir;
    upVector = normalize(upVector);
    float3 sideVector = cross(eyeVector,upVector);
    sideVector = normalize(sideVector);

    float3 finalPosition = center;
    //finalPosition += (IN.TexCoord.x-0.5f)*sideVector;
    //finalPosition += (1.5f-IN.TexCoord.y*1.5f)*upVector;

    float4 finalPosition4 = float4(finalPosition, 1);
	*/

    //OUT.Position = mul(finalPosition4, ViewProj);
    o.Position = IN.Position;

    o.TexCoord = IN.TexCoord;
   return o;
}

float4 BillboardPS(in BBVertexToPixel IN) : COLOR0
{
    
    //OUT.Color = tex2D(textureSampler, IN.TexCoord);

    float4 c = tex2D(cs, IN.TexCoord);
    return c;
}

technique CylBillboard
{
    pass Pass0
    {   
		CullMode=None;
		AlphablendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = One;
		AlphaTestEnable = false;
        ZWriteEnable = false;
        VertexShader = compile vs_2_0 CylBillboardVS();
        PixelShader = compile ps_2_0 BillboardPS();        
    }
}