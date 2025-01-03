// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Test/ClippingMask" {
    Properties{
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0

        _ClippingCentre("Clipping Centre", Vector) = (0,0,0,0)
        _Plane("Plane", Vector) = (1,0.7,0.7,0)
            //_offset ("offset",  Range(-1.5,1.5)) = 1
            [Toggle] _invert("invert", Float) = 0
    }
        SubShader{
            Tags { "RenderType" = "Opaque" "Queue" = "Transparent"}
            LOD 200
            Cull off
            CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            #pragma surface surf Standard fullforwardshadows

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0

            sampler2D _MainTex;

            struct Input {
                float2 uv_MainTex;
                float3 worldPos;
            };

            half _Glossiness;
            half _Metallic;
            fixed4 _Color;
            float _invert;
            uniform float _offset;

            uniform float3 _ClippingCentre;
            uniform float3 _Plane;

            // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
            // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
            // #pragma instancing_options assumeuniformscaling
            UNITY_INSTANCING_BUFFER_START(Props)
                // put more per-instance properties here
            UNITY_INSTANCING_BUFFER_END(Props)

            void surf(Input IN, inout SurfaceOutputStandard o) {

                if ((_offset - dot((IN.worldPos - _ClippingCentre),_Plane)) * (1 - 2 * _invert) < 0) discard;

                // Albedo comes from a texture tinted by color
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;
                // Metallic and smoothness come from slider variables
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
                o.Alpha = c.a;
            }
            ENDCG
        }
            FallBack "Diffuse"
}