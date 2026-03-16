Shader "Custom/RevealUnderUV"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)

        _LightPos ("Light Position", Vector) = (0,0,0,1)
        _LightDir ("Light Direction", Vector) = (0,0,1,0)
        _LightAngle ("Light Angle", Float) = 45
        _Strength ("Strength", Float) = 10
        _LightEnabled ("Light Enabled", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

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
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            float3 _LightPos;
            float3 _LightDir;
            float _LightAngle;
            float _Strength;
            float _LightEnabled;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.worldPos = TransformObjectToWorld(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                if (_LightEnabled < 0.5)
                    discard;

                // Vector desde la luz al punto
                float3 lightToPoint = i.worldPos - _LightPos;
                float distance = length(lightToPoint);
                float3 lightToPointDir = normalize(lightToPoint);

                float3 lightForward = normalize(_LightDir);

                // 1 ¿Esta delante del foco?
                float facing = dot(lightForward, lightToPointDir);
                if (facing <= 0)
                    discard;

                // 2 Angulo del cono
                float cutoff = cos(radians(_LightAngle * 0.5));
                float cone = smoothstep(cutoff, 1.0, facing);

                // 3 Atenuacion por distancia (ajustable)
                float distanceFade = saturate(1.0 / (distance * distance * 0.25));

                float reveal = cone * distanceFade * _Strength;
                reveal = saturate(reveal);

                half4 col = tex2D(_MainTex, i.uv) * _Color;
                col.rgb *= reveal;
                col.a *= reveal;

                return col;
            }
            ENDHLSL
        }
    }
}