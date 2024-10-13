Shader"Unlit/CubeInstancedInteraction"
{
    Properties
    {
        _InteractionRadius("Interaction Radius",float) =30
        _InactiveColor("Inactive Color",Color) =(0.2,0.2,0.2,1)
        _ActiveColor("Active Color",Color)=(1,0.7,0.0,1)
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

float4 _InactiveColor;
float4 _ActiveColor;
float _InteractionRadius;
            
    
            struct mesh_data
            {
                float3 basePos;
                float4x4 mat;
                float amount;
            };

            struct appdata
            {
                float4 vertex : POSITION;
                float2 color : COLOR;
            };


            
            struct v2f
            {
                float4 color : COLOR;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };
             

            StructuredBuffer<mesh_data>data;

            v2f vert (const appdata v ,const uint instance_id:SV_InstanceID)
            {
                v2f o;
                
                const float4 pos = mul(data[instance_id].mat, v.vertex);
                o.vertex = UnityObjectToClipPos(pos);
                o.color = lerp(_InactiveColor, _ActiveColor, data[instance_id].amount);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
