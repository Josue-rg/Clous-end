Shader "ClousEnd/Skybox"
{
    Properties
    {
        [HDR] _ZenithColor ("Zenith", Color) = (0.012, 0.028, 0.075, 1)
        [HDR] _HorizonColor ("Horizon", Color) = (0.055, 0.11, 0.19, 1)
        [HDR] _GroundColor ("Ground", Color) = (0.008, 0.012, 0.025, 1)
        [HDR] _AuroraA ("Aurora A", Color) = (0.10, 0.85, 0.62, 1)
        [HDR] _AuroraB ("Aurora B", Color) = (0.45, 0.18, 0.85, 1)
        _AuroraIntensity ("Aurora Intensity", Range(0, 3)) = 0.75
        _StarIntensity ("Star Intensity", Range(0, 5)) = 1.15
        _StarDensity ("Star Density", Range(0, 1)) = 0.996
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 dir : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _ZenithColor;
                float4 _HorizonColor;
                float4 _GroundColor;
                float4 _AuroraA;
                float4 _AuroraB;
                float _AuroraIntensity;
                float _StarIntensity;
                float _StarDensity;
            CBUFFER_END

            float hash31(float3 p)
            {
                p = frac(p * 0.3183099 + 0.1);
                p *= 17.0;
                return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.dir = input.positionOS.xyz;
                return output;
            }

            float3 Aurora(float3 dir, float t)
            {
                float h = dir.y;
                float bandMask = smoothstep(0.06, 0.26, h) * smoothstep(0.78, 0.32, h);
                float2 p = dir.xz / max(0.14, h + 0.30);

                float wave = sin(p.x * 2.2 + sin(p.y * 1.7 + t * 0.06) * 1.4 + t * 0.025);
                float wave2 = sin(p.x * 4.7 - p.y * 2.3 + t * 0.04);
                float curtain = pow(saturate(wave * 0.5 + wave2 * 0.25 + 0.45), 4.0);

                float3 col = lerp(_AuroraA.rgb, _AuroraB.rgb, saturate(wave2 * 0.5 + 0.5));
                return col * curtain * bandMask * _AuroraIntensity;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 dir = normalize(input.dir);
                float h = dir.y;

                float3 sky = lerp(_HorizonColor.rgb, _ZenithColor.rgb, saturate(pow(saturate(h), 0.55)));
                float3 ground = lerp(_HorizonColor.rgb * 0.35, _GroundColor.rgb, saturate(-h * 3.0));
                float3 color = h >= 0.0 ? sky : ground;

                float horizon = exp(-abs(h) * 12.0);
                color += float3(0.10, 0.08, 0.06) * horizon * 0.6;

                float3 sp = dir * 280.0;
                float3 cell = floor(sp);
                float3 f = frac(sp) - 0.5;
                float star = smoothstep(0.10, 0.0, length(f));
                float rnd = hash31(cell);
                float twinkle = 0.75 + 0.25 * sin(_Time.y * 2.0 + rnd * 30.0);
                color += float3(0.75, 0.85, 1.0) * star * step(_StarDensity, rnd)
                    * _StarIntensity * twinkle * smoothstep(0.0, 0.15, h);

                color += Aurora(dir, _Time.y);

                return half4(color, 1.0);
            }
            ENDHLSL
        }
    }
}
