Shader "Custom/Bullet"
{
    Properties
    {
        _Color ("Bullet Color", Color) = (0.3, 0.8, 1, 1)
        _Glow ("Glow Intensity", Range(0,5)) = 2
        _EmissionColor ("Emission Color", Color) = (0.5, 1, 1, 1)
        _EmissionIntensity ("Emission Intensity", Range(0,3)) = 1
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimPower ("Rim Power", Range(0.5, 8)) = 3
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha One
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD2;
                float3 worldNormal : TEXCOORD3;
            };

            fixed4 _Color;
            float _Glow;
            fixed4 _EmissionColor;
            float _EmissionIntensity;
            fixed4 _RimColor;
            float _RimPower;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 원형 그라데이션: 중심(0.5,0.5)에서 멀수록 투명
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);
                
                // UV 공간에서 중심에서 모서리까지의 최대 거리 (대각선)
                float maxDist = distance(center, float2(0, 0));

                // 중심이 밝고, 테두리가 투명한 원형
                float alpha = saturate((_Glow * (maxDist - dist)));

                // Rim lighting calculation
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 normalDir = normalize(i.worldNormal);
                float rim = 1.0 - saturate(dot(viewDir, normalDir));
                float rimEffect = pow(rim, _RimPower);

                fixed4 col = _Color;
                
                // Add emission
                col.rgb += _EmissionColor.rgb * _EmissionIntensity;
                
                // Add rim lighting
                col.rgb += _RimColor.rgb * rimEffect;
                
                col.a *= alpha;

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
