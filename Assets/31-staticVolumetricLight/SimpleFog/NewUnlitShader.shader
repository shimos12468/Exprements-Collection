Shader "Tutorial/VolumetricFog"
{
    Properties
    {
        
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

            Pass
            {
                HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment frag
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

                half4 frag(Varyings IN) : SV_Target
                {
                    float4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, IN.texcoord);
                    float depth = SampleSceneDepth(IN.texcoord);
                    float3 worldPos = ComputeWorldSpacePosition(IN.texcoord, depth, UNITY_MATRIX_I_VP);

                    float3 entryPoint = _WorldSpaceCameraPos;
                    float3 viewDir = worldPos - _WorldSpaceCameraPos;
                    float viewLength = length(viewDir);
                    float3 rayDir = normalize(viewDir);

                    float2 pixelCoords = IN.texcoord * _BlitTexture_TexelSize.zw;
                    //float distLimit = min(viewLength, _MaxDistance);
                    //float distTravelled = InterleavedGradientNoise(pixelCoords, (int)(_Time.y / max(HALF_EPS, unity_DeltaTime.x))) * _NoiseOffset;
                    float transmittance = 1;
                    //float4 fogCol = _Color;

                    

                    return depth;
                }
                ENDHLSL
            }
        }
}