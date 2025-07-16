Shader "Custom/Bullet"
{
    Properties
    {
        _Color ("Bullet Color", Color) = (0.3, 0.8, 1, 1)
        _Glow ("Glow Intensity", Range(0,5)) = 2
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color;
            float _Glow;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 원형 그라데이션: 중심(0.5,0.5)에서 멀수록 투명
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);

                // 중심이 밝고, 테두리가 투명한 원형
                float alpha = saturate((_Glow * (0.5 - dist)));

                fixed4 col = _Color;
                col.a *= alpha;

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
