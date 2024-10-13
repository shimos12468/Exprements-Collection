Shader"Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FarColor("Far Color",Color) = (0.2,0.2,0.2,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            float4 _FarColor;
            StructuredBuffer<float4> position_buffer_1;
            StructuredBuffer<float4> position_buffer_2;
            float4 color_buffer[8];
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
    float3 color : TEXCOORD3;
};

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v,const uint instance_id:SV_InstanceID)
            {
    
    
                float4 start = position_buffer_1[instance_id];
                float4 end = position_buffer_2[instance_id];
                const float t = (sin(_Time.y + start.w) + 1 / 2);
                const float3 world_start = start.xyz + v.vertex.xyz;
                const float3 world_end = end.xyz + v.vertex.xyz;
                const float3 pos = lerp(world_start, world_end, t);
                const float3 color = lerp(color_buffer[end.w % 8], _FarColor, t);
    
                v2f o;
                o.vertex = mul(UNITY_MATRIX_VP, float4(pos, 1.0f));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);    
                o.color = color;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                
                return half4(i.color,1);
}
            ENDCG
        }
    }
}
