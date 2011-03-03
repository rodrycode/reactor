//////////////////////////////////////////////////////////////
// Example 5.1                                              //
//                                                          //
// The include statement is interpreted automatically by    //
// the XNA Framework Content Pipeline when effects are      //
// imported though the Content Pipeline.  While you don't   //
// have to explicitly add the include file to the           //
// project, it's often useful for organizational purposes.  //
//////////////////////////////////////////////////////////////

struct RLight 
{
    float4 color;
    float4 position;
    float falloff;
    float range;
};
struct VertexShaderOutput
{
     float4 Position : POSITION;
     float2 TexCoords : TEXCOORD0;
     float3 WorldNormal : TEXCOORD1;
     float3 WorldPosition : TEXCOORD2;
};
struct PixelShaderInput
{
     float2 TexCoords : TEXCOORD0;
     float3 WorldNormal : TEXCOORD1;
     float3 WorldPosition : TEXCOORD2;
};


shared float4x4 view : View;
shared float4x4 projection : Projection;
shared float3 cameraPosition : ViewPosition;
shared float4 ambientLightColor : GlobalAmbient;
shared float numLights : LightCount = 4;
float4x4 world : World;

float4 materialColor : DiffuseColor;
float specularPower : SpecularPower;
float specularIntensity : SpecularIntensity;
bool diffuseTexEnabled = false;
bool specularTexEnabled = false;
float textureUReps = 1.0;
float textureVReps = 1.0;

texture2D diffuseTexture : Texture0;
texture2D specularTexture : Texture1; 
sampler diffuseSampler;
sampler specularSampler;

shared RLight lights[32];

float4 CalculateSingleLight(RLight light, float3 worldPosition, float3 worldNormal, 
                            float4 diffuseColor, float4 specularColor )
{
     float3 lightVector = light.position - worldPosition;
     float lightDist = length(lightVector);
     float3 directionToLight = normalize(lightVector);
     
     //calculate the intensity of the light with exponential falloff
     float baseIntensity = pow(saturate((light.range - lightDist) / light.range),
                                 light.falloff);
     
     
     float diffuseIntensity = saturate( dot(directionToLight, worldNormal));
     float4 diffuse = diffuseIntensity * light.color * diffuseColor;

     //calculate Phong components per-pixel
     float3 reflectionVector = normalize(reflect(-directionToLight, worldNormal));
     float3 directionToCamera = normalize(cameraPosition - worldPosition);
     
     //calculate specular component
     float4 specular = saturate(light.color * specularColor * specularIntensity * 
                       pow(saturate(dot(reflectionVector, directionToCamera)), 
                           specularPower));
                           
     return  baseIntensity * (diffuse + specular);
}

VertexShaderOutput BasicVS(
     float3 position : POSITION,
     float3 normal : NORMAL,
     float3 binormal : BINORMAL,
     float3 tangent : TANGENT,
     float2 texCoord : TEXCOORD0 )
{
     VertexShaderOutput output;

     //generate the world-view-projection matrix
     float4x4 wvp = mul(mul(world, view), projection);
     
     //transform the input position to the output
     output.Position = mul(float4(position, 1.0), wvp);

     output.WorldNormal =  mul(normal, world);
     float4 worldPosition =  mul(float4(position, 1.0), world);
     output.WorldPosition = worldPosition / worldPosition.w;
     
     //copy the tex coords to the interpolator
     output.TexCoords.x = texCoord.x * textureUReps;
     output.TexCoords.y = texCoord.y * textureVReps;

     //return the output structure
     return output;
}

float4 MultipleLightPS(PixelShaderInput input) : COLOR
{
    float4 diffuseColor = materialColor;
    float4 specularColor = materialColor;
     
    if(diffuseTexEnabled)
    {
        diffuseColor *= tex2D(diffuseSampler, input.TexCoords);
    }
     
    if(specularTexEnabled)
    {
        specularColor *= tex2D(specularSampler, input.TexCoords);
    }
     
    float4 color = ambientLightColor * diffuseColor;
     
    //////////////////////////////////////////////////////////////
    // Example 5.2                                              //
    //                                                          //
    // Each light is summed into a final pixel color.  The 3.0  //
    // shader supports dynamic control instructions, which      //
    // allows for loops to  behave as they would on a general   //
    // purpose CPU.                                             //
    //////////////////////////////////////////////////////////////
    for(int i=0; i< 3; i++)
    {
        color += CalculateSingleLight(lights[i], 
                  input.WorldPosition, input.WorldNormal,
                  diffuseColor, specularColor );
    }
    color.a = 1.0;
    return color;
}


technique MultipleLights
{

    pass P0
    {
        //////////////////////////////////////////////////////////////
        // Example 5.3                                              //
        //                                                          //
        // Render, sampler, and texture states can be set in an     //
        // effect, in addition to Vertex and Pixel shaders states.  //
        // Many effect will not be sensible without a specific      //
        // set of states that correspond to the kind of shaders     //
        // being employed.                                          //
        //////////////////////////////////////////////////////////////
        
        //set sampler states
        MagFilter[0] = LINEAR;
        MinFilter[0] = LINEAR;
        MipFilter[0] = LINEAR;
        AddressU[0] = WRAP;
        AddressV[0] = WRAP;
        MagFilter[1] = LINEAR;
        MinFilter[1] = LINEAR;
        MipFilter[1] = LINEAR;
        AddressU[1] = WRAP;
        AddressV[1] = WRAP;
        
        //set texture states (notice the '<' , '>' brackets)
        //as the texture state assigns a reference
        Texture[0] = <diffuseTexture>;
        Texture[1] = <specularTexture>;
        
        // set render states
        AlphaBlendEnable = FALSE;

        //set pixel shader states
        VertexShader = compile vs_3_0 BasicVS();     
        PixelShader = compile ps_3_0 MultipleLightPS();
    }
}