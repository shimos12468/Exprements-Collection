Shader "Unlit/VolumetricFog"
{
    Properties
    {
        _Color("Color",Color) = (1,1,1,1)
       _MaxDistance("Max Distance",float) =100
       _StepSize("Step size",Range(0.1,20)) =20
       _DensityMultiplier("Density Multiplier",Range(0,10)) =1
       _NoiseOffset("Noise Offset",float) =1

       _FogNoise("Fog Noise ",3D) = "white"{}
       _NoiseTilling("Noise tilling",float) =1
       _DensityThreshold("Density threshold",Range(0,1)) =0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            float _MaxDistance;

            float _StepSize;
            float _DensityMultiplier;
            float4 _Color;
            float _NoiseOffset;
            TEXTURE3D(_FogNoise);
            float _DensityThreshold;
            float _NoiseTilling;

            float GetDensity(float3 worldPos){
            
                float4 noise = _FogNoise.SampleLevel(sampler_TrilinearRepeat ,worldPos*0.01*_NoiseTilling,0);
                float density1 = dot(noise,noise);
                density1 = saturate(density1-_DensityThreshold)*_DensityMultiplier;
                return density1;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D(_BlitTexture,sampler_LinearClamp,i.texcoord);

               float depth = SampleSceneDepth(i.texcoord);
               float3 worldPos = ComputeWorldSpacePosition(i.texcoord,depth,UNITY_MATRIX_I_VP);
               
               float3 entryPoint = _WorldSpaceCameraPos;
               float3 viewDir = worldPos - _WorldSpaceCameraPos;
               float viewLength = length(viewDir);
               float rayDir = normalize(viewDir);
               
               
               float2 pixelCoords = i.texcoord * _BlitTexture_TexelSize.zw;

               float distLimit = min(viewLength,_MaxDistance);
               float distTravelled = InterleavedGradientNoise(pixelCoords ,(int)(_Time.y/max(HALF_EPS,unity_DeltaTime.x)))*_NoiseOffset;
               float transmittance =1;
               
               while(distTravelled<distLimit)
               {
                   float3 rayPos =entryPoint+(rayDir*distTravelled); 
                   float density = GetDensity(rayPos);

                   if(density>0)
                   {
                       transmittance*=exp(-density*_StepSize);
                    }
                   distTravelled+=_StepSize;
                            
               }

               return lerp(col,_Color ,1.0-saturate(transmittance));
            }
            ENDHLSL
        }
    }
}
