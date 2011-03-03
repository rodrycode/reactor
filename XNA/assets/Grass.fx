//-----------------------------------------------------------------------------
// Grass.fx
//
// Microsoft Game Technology Group
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#define worldUp float3(0,1,0)

// Camera parameters.
float4x4 View : VIEW;
float4x4 Projection : PROJECTION;
float4x4 World : WORLD;
float3 ViewDirection : VIEWDIRECTION;
float3 cameraPosition;
float4 Position = 1.0;
// Lighting parameters.
float3 LightDirection;
float4 LightColor = 0.8;
float3 AmbientColor = 0.4;
float Distance = 1.0;
float GrassHeight = 1.0;
float MaxGrassHeight = 1.0;
float textureSize = 512.0;
float terrainScale = 1.0;
float elevationStrength = 1.0;
// Parameters controlling the wind effect.
float3 WindDirection = float3(1, 0, 0);
float WindWaveSize = 0.1;
float WindRandomness = 1;
float WindSpeed = 4;
float WindAmount = 0.4;
float WindTime : TIME;
bool FOGENABLE;
float FOGDIST;
float4 FOGCOLOR = float4(1, 1, 1, 0.5);

// Parameters describing the billboard itself.
float BillboardWidth;
float BillboardHeight;
float normalStrength = 8.0f;
float startFadingInDistance;
float stopFadingInDistance;

texture Texture0 : TEXTURE0;
texture Texture1 : TEXTURE1;
texture Texture2 : TEXTURE2;
texture Heightmap : HEIGHTMAP;
texture Normalmap : NORMALMAP;
sampler heightSampler = sampler_state
{
	Texture = <Heightmap>;
	
	MipFilter = Point;
	MinFilter = Point;
	MagFilter = Point;
	
	AddressU = Clamp;
	AddressV = Clamp;
};
sampler heightPSSampler = sampler_state
{
	Texture = <Heightmap>;
	
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;
	
	AddressU = Clamp;
	AddressV = Clamp;
};
sampler normalSampler = sampler_state
{
	Texture = <Normalmap>;
	
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;
	
	AddressU = Wrap;
	AddressV = Wrap;
};
float Random = 1.0;


struct VS_INPUT
{
    float3 Position : POSITION0;
    float3 Normal   : NORMAL0;
    float2 TexCoord : TEXCOORD0;
    
};


struct VS_OUTPUT
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float2 HeightCoord : TEXCOORD1;
    float4 WorldPos : TEXCOORD2;
    float4 Color    : COLOR0;
    
    float2 Fog       : TEXCOORD3;
};

float4 tex2Dlod_bilinear(sampler texSam, float4 uv)
{
	float texelSize = 1.0f / textureSize;

	float4 height00 = tex2Dlod(texSam, uv);
	
	float4 height10 = tex2Dlod(texSam, uv + float4(texelSize, 0, 0, 0));
	
	float4 height01 = tex2Dlod(texSam, uv + float4(0, texelSize, 0, 0));
	
	float4 height11 = tex2Dlod(texSam, uv + float4(texelSize, texelSize, 0, 0));
	
	float2 f = frac(uv.xy * textureSize);
	
	float4 tA = lerp(height00, height10, uv.x);
	float4 tB = lerp(height01, height11, uv.x);
	
	return lerp(tA, tB, uv.y);
}

VS_OUTPUT VertexShader(VS_INPUT input)
{
    VS_OUTPUT output;

    // Apply a scaling factor to make some of the billboards
    // shorter and fatter while others are taller and thinner.
    
    // Flip half of the billboards from left to right. This gives visual variety
    // even though we are actually just repeating the same texture over and over.
    if (Random < 0)
        BillboardWidth = -BillboardWidth;

    // Work out what direction we are viewing the billboard from.
    float3 viewDirection = View._m02_m12_m22;

    float3 rightVector = normalize(cross(viewDirection, input.Normal));

    // Calculate the position of this billboard vertex.
    //float3 position = mul(input.Position,Position);
    float3 position = float3(input.Position.x+Position.x,input.Position.y+Position.y,input.Position.z+Position.z) * terrainScale;
    output.WorldPos = float4(position,1);
    
    float2 heightMapCoord = float2((((position.x/terrainScale)/(textureSize))), 
	                               (((position.z/terrainScale)/(textureSize))));
	float posheight = tex2Dlod(heightSampler, float4(heightMapCoord.xy, 0, 0));
	//position.z *= terrainScale;
	//position.x *= terrainScale;	
	//position.y *= terrainScale;
	//position.y +=  ((posheight*terrainScale) * elevationStrength);  
	if(position.y < GrassHeight || position.y > MaxGrassHeight)
	{
	position.y = -10000 * terrainScale;
	}                          
    // Offset to the left or right.
    position += rightVector * (input.TexCoord.x - 0.5) * BillboardWidth;
    
    // Offset upward if we are one of the top two vertices.
    position += input.Normal * (1 - input.TexCoord.y) * BillboardHeight;

    // Work out how this vertex should be affected by the wind effect.
    float waveOffset = dot(position, WindDirection) * WindWaveSize;
    
    //waveOffset += input.Random * WindRandomness;
    
    // Wind makes things wave back and forth in a sine wave pattern.
    float wind = sin(WindTime * WindSpeed + waveOffset) * WindAmount;
    
    // But it should only affect the top two vertices of the billboard!
    wind *= (1 - input.TexCoord.y);
    
    position += WindDirection * wind;

    // Apply the camera transform.
    float4 viewPosition = mul(float4(position, 1), View);
	float4 worldPos = mul(viewPosition, Projection);
    output.Position = mul(viewPosition, Projection);
    output.TexCoord = input.TexCoord;
    // Compute lighting.
    float diffuseLight = max(-dot(input.Normal, LightDirection), 0);
    
    output.Color.rgb = diffuseLight * LightColor + AmbientColor;
    output.Color.a = 1.0f;
    output.Fog = float2(1 - saturate(length(output.WorldPos) / FOGDIST),0);
    output.WorldPos.x = worldPos.x / (textureSize * terrainScale);
    output.WorldPos.y = worldPos.y / (textureSize * terrainScale);
    output.WorldPos.z = worldPos.z / (textureSize * terrainScale);
    output.HeightCoord = heightMapCoord;
    
    return output;
}


sampler TextureSampler1 = sampler_state
{
    Texture = (Texture0);

    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    
    AddressU = Wrap;
    AddressV = Wrap;
};
sampler TextureSampler2 = sampler_state
{
    Texture = (Texture1);

    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    
    AddressU = Wrap;
    AddressV = Wrap;
};
sampler TextureSampler3 = sampler_state
{
    Texture = (Texture2);

    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    
    AddressU = Wrap;
    AddressV = Wrap;
};

// This computes a normal map using an 8-tap Sodel filter for the loaded heightmap.
float4 ComputeNormalsPS(in float2 uv : TEXCOORD0) : COLOR
{
	float texelSize = 1.0f / textureSize;
	
	// Top left
	float tl = abs(tex2D(heightPSSampler, uv + texelSize * float2(-1, 1)).x);
	
	// Left
	float l = abs(tex2D(heightPSSampler, uv + texelSize * float2(-1, 0)).x);
	
	// Bottom Left
	float bl = abs(tex2D(heightPSSampler, uv + texelSize * float2(-1, -1)).x);
	
	// Top
	float t = abs(tex2D(heightPSSampler, uv + texelSize * float2(0, 1)).x);
	
	// Bottom
	float b = abs(tex2D(heightPSSampler, uv + texelSize * float2(0, -1)).x);
	
	// Top Right
	float tr = abs(tex2D(heightPSSampler, uv + texelSize * float2(1, 1)).x);
	
	// Right
	float r = abs(tex2D(heightPSSampler, uv + texelSize * float2(1, 0)).x);
	
	// Bottom Right
	float br = abs(tex2D(heightPSSampler, uv + texelSize * float2(1, -1)).x);
	
	float dx = -tl - 2.0f * l - bl + tr + 2.0f * r + br;
	float dy = -tl - 2.0f * t - tr + bl + 2.0f * b + br;
	
	float4 normal = float4(normalize(float3(dx, 1.0f / normalStrength, dy)), 1.0f);
	
	// Convert coordinates from range (-1,1) to range (0,1)
	return normal * 0.5f + 0.5f;
}
float4 PixelShader(in float2 texCoord : TEXCOORD0, in float4 worldPos : TEXCOORD1, in float4 color : COLOR0, in float2 fog : TEXCOORD3) : COLOR0
{
    
	color = tex2D(TextureSampler1, texCoord);
	
    
	float4 normal = normalize(2.0f * (tex2D(normalSampler, (worldPos/terrainScale)) - 0.5f));
	
	float4 light = normalize(float4(-LightDirection,1.0));
	
	float LdotN = dot(light, normal);
	LdotN = max(0, LdotN);
	
	float oldA = color.a;
	
	color *= (0.1f + LdotN);
	
	float3 displacement = normalize((cameraPosition)-(worldPos));
	
	color.a = oldA * saturate(1 - (length(displacement)) / (startFadingInDistance - stopFadingInDistance));
	//color.a = fog.x/terrainScale;
    return color;
    //return tex2D(TextureSampler, texCoord) * color;
}

technique ComputeNormals
{
	pass P0
	{
		pixelShader = compile ps_3_0 ComputeNormalsPS();
	}
}
technique Billboards
{
    // We use a two-pass technique to render alpha blended geometry with almost-correct
    // depth sorting. The only way to make blending truly proper for alpha objects is
    // to draw everything in sorted order, but manually sorting all our billboards
    // would be very expensive. Instead, we draw our billboards in two passes.
    //
    // The first pass has alpha blending turned off, alpha testing set to only accept
    // ~95% or more opaque pixels, and the depth buffer turned on. Because this is only
    // rendering the solid parts of each billboard, the depth buffer works as
    // normal to give correct sorting, but obviously only part of each billboard will
    // be rendered.
    //
    // Then in the second pass we enable alpha blending, set alpha test to only accept
    // pixels with fractional alpha values, and set the depth buffer to test against
    // the existing data but not to write new depth values. This means the translucent
    // areas of each billboard will be sorted correctly against the depth buffer
    // information that was previously written while drawing the opaque parts, although
    // there can still be sorting errors between the translucent areas of different
    // billboards.
    //
    // In practice, sorting errors between translucent pixels tend not to be too
    // noticable as long as the opaque pixels are sorted correctly, so this technique
    // often looks ok, and is much faster than trying to sort everything 100%
    // correctly. It is particularly effective for organic textures like grass and
    // trees.
    
    pass RenderOpaquePixels
    {
        VertexShader = compile vs_3_0 VertexShader();
        PixelShader = compile ps_3_0 PixelShader();

        AlphaBlendEnable = false;
        SrcBlend = SrcAlpha;
        DestBlend = InvSrcAlpha;
        AlphaTestEnable = true;
        AlphaFunc = Greater;
        AlphaRef = 245;
        
        ZEnable = true;
        ZWriteEnable = true;

        CullMode = NONE;
    }

    pass RenderAlphaBlendedFringes
    {
        VertexShader = compile vs_3_0 VertexShader();
        PixelShader = compile ps_3_0 PixelShader();
        
        AlphaBlendEnable = true;
        SrcBlend = SrcAlpha;
        DestBlend = InvSrcAlpha;
        
        AlphaTestEnable = true;
        AlphaFunc = LessEqual;
        AlphaRef = 245;

        ZEnable = true;
        ZWriteEnable = false;

        CullMode = NONE;
    }
}
