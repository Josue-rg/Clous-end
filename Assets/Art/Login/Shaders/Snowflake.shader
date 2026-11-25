Shader "ClousEnd/Snowflake"
{
    Properties
    {
        [HDR] _Color ("Color", Color) = (0.85, 0.93, 1.0, 0.9)
        _Softness ("Softness", Range(0.01, 1)) = 0.45
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _Softness;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float distanceFromCenter = length(input.uv - 0.5) * 2.0;
                float alpha = 1.0 - smoothstep(1.0 - _Softness, 1.0, distanceFromCenter);
                alpha *= _Color.a * input.color.a;
                return half4(_Color.rgb * input.color.rgb, saturate(alpha));
            }
            ENDHLSL
        }
    }
}
