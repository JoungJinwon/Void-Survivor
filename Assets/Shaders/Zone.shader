Shader "Custom/Zone"
{
    Properties
    {
        _Color ("Zone Color", Color) = (0.2, 0.8, 1, 1)
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimPower ("Rim Power", Range(0.1, 5)) = 2
        _RimIntensity ("Rim Intensity", Range(0, 3)) = 1.5
        _CenterAlpha ("Center Alpha", Range(0, 1)) = 0.1
        _EdgeAlpha ("Edge Alpha", Range(0, 1)) = 0.8
        _RotationSpeed ("Rotation Speed", Range(-5, 5)) = 1
        _WaveSpeed ("Wave Speed", Range(0, 5)) = 2
        _WaveAmplitude ("Wave Amplitude", Range(0, 1)) = 0.3
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
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
            fixed4 _RimColor;
            float _RimPower;
            float _RimIntensity;
            float _CenterAlpha;
            float _EdgeAlpha;
            float _RotationSpeed;
            float _WaveSpeed;
            float _WaveAmplitude;

            // 2D 회전 행렬
            float2 rotate2D(float2 uv, float angle)
            {
                float s = sin(angle);
                float c = cos(angle);
                return float2(
                    uv.x * c - uv.y * s,
                    uv.x * s + uv.y * c
                );
            }

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
                // UV를 중심점(0.5, 0.5) 기준의 상대 좌표로 정규화
                float2 centeredUV = i.uv - 0.5;
                
                // 시간에 따른 회전
                float rotationAngle = _Time.y * _RotationSpeed;
                float2 rotatedUV = rotate2D(centeredUV, rotationAngle);
                
                // 중심에서의 거리 계산
                float distanceFromCenter = length(centeredUV);
                
                // 테두리 발광을 위한 거리 기반 알파 (제곱을 사용하여 더 극단적인 효과)
                float edgeFactor = saturate(distanceFromCenter * 2.0);
                edgeFactor = pow(edgeFactor, 2.0); // 제곱을 적용하여 중앙 부분을 더 투명하게
                float alpha = lerp(_CenterAlpha, _EdgeAlpha, edgeFactor);
                
                // 회전하는 패턴을 위한 각도 계산
                float angle = atan2(rotatedUV.y, rotatedUV.x);
                float normalizedAngle = (angle + 3.14159) / (2.0 * 3.14159); // 0-1 범위로 정규화
                
                // 웨이브 효과 추가
                float wave = sin(normalizedAngle * 6.28318 * 4 + _Time.y * _WaveSpeed) * _WaveAmplitude;
                alpha += wave * edgeFactor; // 테두리에서만 웨이브 효과 적용
                
                // 중앙 부분을 더욱 투명하게 만들기 위한 추가 마스크
                float centerMask = 1.0 - exp(-distanceFromCenter * 8.0); // 지수 함수를 사용한 마스크
                alpha *= centerMask;
                
                // Rim lighting 계산
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 normalDir = normalize(i.worldNormal);
                float rim = 1.0 - saturate(dot(viewDir, normalDir));
                float rimEffect = pow(rim, _RimPower) * _RimIntensity;
                
                // 최종 색상 계산
                fixed4 col = _Color;
                
                // 림 라이팅 추가
                col.rgb += _RimColor.rgb * rimEffect;
                
                // 테두리 발광 강화
                col.rgb += _RimColor.rgb * edgeFactor * 0.5;
                
                // 알파 적용
                col.a = saturate(alpha);
                
                // 안개 효과 적용
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
