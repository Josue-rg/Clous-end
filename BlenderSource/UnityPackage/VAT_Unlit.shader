// CLOUS-END · Shader VAT (Vertex Animation Texture)
// Mueve la malla en GPU leyendo posiciones de una textura EXR.
// Sin huesos, sin skinning: ideal para WebGL.
//
// Requisitos del import de la textura:
//   Texture Type : Default
//   sRGB         : OFF
//   Filter Mode   : Point (no filter)
//   Wrap Mode     : Clamp
//   Compression   : None
//   Max Size      : >= 2048
//
// El shader lee UV1 (TEXCOORD1) del FBX; si tu malla la importa como UV0,
// cambia  v.uv2  por  v.uv  y asegurate de no usar _BaseMap.

Shader "ClousEnd/VAT_Unlit"
{
    Properties
    {
        _BaseColor  ("Color", Color) = (1,1,1,1)
        _BaseMap    ("Albedo (opcional, UV0)", 2D) = "white" {}
        _PosTex     ("VAT Posiciones (EXR)", 2D) = "black" {}
        _Frames     ("Frames del clip", Float) = 24
        _UVFrames   ("Frames del atlas UV (0 = igual)", Float) = 0
        _Frame      ("Frame", Float) = 0
        _VATAmount  ("Cantidad VAT", Range(0,1)) = 1
        _FlipY      ("Voltear Y (0/1)", Range(0,1)) = 0

        _Ambient    ("Ambiente", Range(0,2)) = 0.45
        _LightDir   ("Direccion de luz", Vector) = (0.35, 0.75, 0.55, 0)
        _LightColor ("Color de luz", Color) = (1,0.97,0.92,1)
        _Rim        ("Rim", Range(0,3)) = 0.45
        _Emission   ("Emision", Color) = (0,0,0,0)
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }

        Pass
        {
            Name "VATForward"
            Tags { "LightMode"="UniversalForward" }
            Cull Back
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            sampler2D _BaseMap;
            sampler2D _PosTex;
            float4 _BaseColor;
            float4 _LightColor;
            float4 _Emission;
            float4 _LightDir;
            float _Frames;
            float _UVFrames;
            float _VATAmount;
            float _FlipY;
            float _Ambient;
            float _Rim;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _Frame)
            UNITY_INSTANCING_BUFFER_END(Props)

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv     : TEXCOORD0;
                float2 uv2    : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos      : SV_POSITION;
                float2 uv       : TEXCOORD0;
                float3 normal   : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                UNITY_FOG_COORDS(3)
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float3 SampleVAT(float2 baseUV, float frame, float flip)
            {
                float f = fmod(frame, _Frames);
                float dir = flip > 0.5 ? -1.0 : 1.0;
                float baseV = baseUV.y;

                // El atlas UV (UV1) se horneó con el clip mas largo del asset.
                // Cada clip tiene su propia altura de textura, asi que la fila
                // base hay que reescalarla al numero de frames de este clip.
                if (_UVFrames > 0.5)
                    baseV *= _UVFrames / max(_Frames, 1.0);

                float v = flip > 0.5 ? (1.0 - baseV) : baseV;
                float2 uv = float2(baseUV.x, v + dir * (f / _Frames));
                return tex2Dlod(_PosTex, float4(uv, 0, 0)).xyz;
            }

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                float frame  = UNITY_ACCESS_INSTANCED_PROP(Props, _Frame);
                float f0 = floor(frame);
                float f1 = f0 + 1.0;
                float t  = frame - f0;

                float3 pA = SampleVAT(v.uv2, f0, _FlipY);
                float3 pB = SampleVAT(v.uv2, f1, _FlipY);
                float3 p  = lerp(pA, pB, t);

                p = lerp(v.vertex.xyz, p, _VATAmount);

                o.pos      = UnityObjectToClipPos(float4(p, 1.0));
                o.uv       = v.uv;
                o.normal   = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, float4(p, 1.0)).xyz;
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float3 n = normalize(i.normal);
                float3 L = normalize(_LightDir.xyz);
                float ndl = saturate(dot(n, L) * 0.5 + 0.5);

                float3 baseCol = _BaseColor.rgb * tex2D(_BaseMap, i.uv).rgb;
                float3 col = baseCol * (ndl * _LightColor.rgb + _Ambient);

                float3 V = normalize(_WorldSpaceCameraPos - i.worldPos);
                float rim = pow(1.0 - saturate(dot(n, V)), 3.0) * _Rim;
                col += rim * baseCol;
                col += _Emission.rgb;

                UNITY_APPLY_FOG(i.fogCoord, col);
                return float4(col, 1.0);
            }
            ENDHLSL
        }

        Pass
        {
            Name "VATShadow"
            Tags { "LightMode"="ShadowCaster" }
            ZWrite On
            ZTest LEqual
            Cull Back

            HLSLPROGRAM
            #pragma vertex vertShadow
            #pragma fragment fragShadow
            #pragma target 3.5
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            sampler2D _PosTex;
            float _Frames;
            float _UVFrames;
            float _VATAmount;
            float _FlipY;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _Frame)
            UNITY_INSTANCING_BUFFER_END(Props)

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv2    : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float3 SampleVATShadow(float2 baseUV, float frame, float flip)
            {
                float f = fmod(frame, _Frames);
                float dir = flip > 0.5 ? -1.0 : 1.0;
                float baseV = baseUV.y;

                if (_UVFrames > 0.5)
                    baseV *= _UVFrames / max(_Frames, 1.0);

                float v = flip > 0.5 ? (1.0 - baseV) : baseV;
                float2 uv = float2(baseUV.x, v + dir * (f / _Frames));
                return tex2Dlod(_PosTex, float4(uv, 0, 0)).xyz;
            }

            float4 vertShadow(appdata v) : SV_POSITION
            {
                UNITY_SETUP_INSTANCE_ID(v);
                float frame = UNITY_ACCESS_INSTANCED_PROP(Props, _Frame);
                float3 p = SampleVATShadow(v.uv2, frame, _FlipY);
                p = lerp(v.vertex.xyz, p, _VATAmount);
                return UnityObjectToClipPos(float4(p, 1.0));
            }

            float4 fragShadow() : SV_Target { return 0; }
            ENDHLSL
        }
    }
    Fallback Off
}
