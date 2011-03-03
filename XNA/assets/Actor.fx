//-----------------------------------------------------------------------------
// Actor.fx
// REACTOR 3D ACTOR SHADER
// By: Gabriel Reiser
// Original Shader by:
// Microsoft Game Technology Group
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------


// Maximum number of bone matrices we can render using shader 2.0 in a single pass.

#define MaxBones 80


// Input parameters.
float4x4 View;
float4x4 Projection;
float4x3 Bones[MaxBones];

float3 Light1Direction = normalize(float3(1, 1, -2));
float4 Light1Color = float4(0.9, 0.8, 0.7, 1.0);

float3 Light2Direction = normalize(float3(-1, -1, 1));
float4 Light2Color = float4(0.1, 0.3, 0.8, 1.0);

float4 AmbientColor = 0.2;

texture2D Texture : TEXTURE0;

sampler Sampler = sampler_state
{
    Texture = (Texture);

    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
};


// Vertex shader input structure.
struct VS_INPUT
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD0;
    float4 Tangent : TANGENT0;
    float4 Binormal : BINORMAL0;
    float4 BoneIndices : BLENDINDICES0;
    float4 BoneWeights : BLENDWEIGHT0;
};


// Vertex shader output structure.
struct VS_OUTPUT
{
    float4 Position : POSITION0;
    float3 Lighting : COLOR0;
    float2 TexCoord : TEXCOORD0;
};


// Vertex shader program.
VS_OUTPUT VertexShader(VS_INPUT input)
{
    VS_OUTPUT output;
    
    // Blend between the weighted bone matrices.
    float4x3 skinTransform = 0;
    
    skinTransform += Bones[input.BoneIndices.x] * input.BoneWeights.x;
    skinTransform += Bones[input.BoneIndices.y] * input.BoneWeights.y;
    skinTransform += Bones[input.BoneIndices.z] * input.BoneWeights.z;
    skinTransform += Bones[input.BoneIndices.w] * input.BoneWeights.w;
    
    // Skin the vertex position.
    // Combine skin and world transformations
   float4x4 matSmoothSkinWorld = 0;
   matSmoothSkinWorld[0] = float4(skinTransform[0], 0);
   matSmoothSkinWorld[1] = float4(skinTransform[1], 0);
   matSmoothSkinWorld[2] = float4(skinTransform[2], 0);
   matSmoothSkinWorld[3] = float4(skinTransform[3], 1);
   
    float4 position = mul(input.Position, matSmoothSkinWorld);
    
    output.Position = mul(mul(position, View), Projection);

    // Skin the vertex normal, then compute lighting.
    float3 normal = normalize(mul(input.Normal, skinTransform));
    
    float3 light1 = saturate(dot(normal, Light1Direction) * Light1Color);
    float3 light2 = saturate(dot(normal, Light2Direction) * Light2Color);

    output.Lighting = light1 + light2 + AmbientColor;

    output.TexCoord = input.TexCoord;
    
    return output;
}


// Pixel shader input structure.
struct PS_INPUT
{
    float3 Lighting : COLOR0;
    float2 TexCoord : TEXCOORD0;
};


// Pixel shader program.
float4 PixelShader(PS_INPUT input) : COLOR0
{
    float4 color = tex2D(Sampler, input.TexCoord);

    color.rgb *= input.Lighting;
    
    return color;
}


technique ActorTechnique
{
    pass ActorPass
    {
        VertexShader = compile vs_3_0 VertexShader();
        PixelShader = compile ps_3_0 PixelShader();
    }
}
