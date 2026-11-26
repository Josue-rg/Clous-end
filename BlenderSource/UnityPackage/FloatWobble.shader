// CLOUS-END · Animacion matematica por shader (sin texturas)
// Alternativa ligera al VAT para aliens que flotan u ondulan.
// Todo en vertex shader: cero huesos, cero memoria de animacion.

Shader "ClousEnd/FloatWobble"
{
    Properties
    {
        _BaseColor  ("Color", Color) = (1,1,1,1)
        _BaseMap    ("Albedo (opcional)", 2D) = "white" {}

        _WaveAmp    ("Amplitud onda (m)", Float) = 0.06
        _WaveFreq   ("Frecuencia", Float) = 3.0
        _WaveSpeed  ("Velocidad", Float) = 1.5
        _WaveAxis   ("Eje de onda", Vector) = (0,1,0,0)

        _FloatAmp   ("Flotar (m)", Float) = 0.12
        _FloatSpeed ("Velocidad flotar", Float) = 0.6

        _Pulse      ("Pulso de escala", Range(0,0.3)) = 0.04
        _PulseSpeed ("Velocidad pulso", Float) = 1.2

        _Ambient    ("Ambiente", Range(0,2)) = 0.45
        _LightDir   ("Direccion de luz", Vector) = (0.35, 0.75, 0.55, 0)
        _LightColor ("Color de luz", Color) = (1,0.97,0.92,1)
        _Rim        ("Rim", Range(0,3)) = 0.5
        _Emission   ("Emision", Color) = (0,0,0,0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        Pass
        {
            Name "FloatForward"
            Tags { "LightMode"="UniversalForward" }
            Cull Back
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            sampler2D _BaseMap;
            float4 _BaseColor, _LightColor, _Emission, _WaveAxis;
            float _WaveAmp, _WaveFreq, _WaveSpeed;
            float _FloatAmp, _FloatSpeed;
            float _Pulse, _PulseSpeed;
            float _Ambient, _Rim;

            // desfase por instancia para que no floten sincronizados
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _Phase)
            UNITY_INSTANCING_BUFFER_END(Props)

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;
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

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                float phase = UNITY_ACCESS_INSTANCED_PROP(Props, _Phase);
                float t = _Time.y * _WaveSpeed + phase;

                float3 p = v.vertex.xyz;

                // onda viajera: desplaza segun la altura local
                float w = sin(p.y * _WaveFreq + t) * _WaveAmp;
                p += normalize(_WaveAxis.xyz + 0.0001) * w;

                // flotar global
                p.y += sin(_Time.y * _FloatSpeed + phase) * _FloatAmp;

                // pulso de escala
                float s = 1.0 + sin(_Time.y * _PulseSpeed + phase) * _Pulse;
                p *= s;

                o.pos = UnityObjectToClipPos(float4(p, 1.0));
                o.uv = v.uv;
                o.normal = UnityObjectToWorldNormal(v.normal);
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
                col += pow(1.0 - saturate(dot(n, V)), 3.0) * _Rim * baseCol;
                col += _Emission.rgb;

                UNITY_APPLY_FOG(i.fogCoord, col);
                return float4(col, 1.0);
            }
            ENDHLSL
        }
    }
    Fallback Off
}
