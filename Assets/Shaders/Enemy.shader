Shader "Custom/Entity"
{
    Properties
    {
        // 기본 색상 및 텍스처
        _Color ("Main Color", Color) = (1, 1, 1, 1)
        _MainTex ("Main Texture", 2D) = "white" {}
        
        // 발광 효과
        _EmissionColor ("Emission Color", Color) = (0, 0, 0, 1)
        _EmissionIntensity ("Emission Intensity", Range(0, 5)) = 1.0
        
        // 펄싱 효과
        _PulseSpeed ("Pulse Speed", Float) = 2.0
        _PulseMin ("Pulse Min", Range(0, 1)) = 0.1
        _PulseMax ("Pulse Max", Range(0, 1)) = 1.0
        
        // 디졸브 효과
        _DissolveTex ("Dissolve Texture", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0.0
        _DissolveEdgeWidth ("Dissolve Edge Width", Range(0, 0.1)) = 0.02
        _DissolveEdgeColor ("Dissolve Edge Color", Color) = (1, 1, 1, 1)
        
        // 왜곡 효과
        _DistortionTex ("Distortion Texture", 2D) = "bump" {}
        _DistortionStrength ("Distortion Strength", Range(0, 0.1)) = 0.02
        _DistortionSpeed ("Distortion Speed", Float) = 1.0
        
        // 림 라이팅
        _RimColor ("Rim Color", Color) = (0, 1, 1, 1)
        _RimPower ("Rim Power", Range(0.1, 10)) = 2.0
        
        // 데미지 효과
        _DamageColor ("Damage Color", Color) = (1, 0, 0, 1)
        _DamageAmount ("Damage Amount", Range(0, 1)) = 0.0
        
        // 프레넬 효과
        _FresnelPower ("Fresnel Power", Range(0, 5)) = 1.0
        _FresnelScale ("Fresnel Scale", Range(0, 5)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

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
                float2 dissolveUV : TEXCOORD1;
                float2 distortionUV : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
                float3 worldNormal : TEXCOORD4;
                float3 viewDir : TEXCOORD5;
                UNITY_FOG_COORDS(6)
                float4 vertex : SV_POSITION;
            };

            // 프로퍼티 변수들
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            
            fixed4 _EmissionColor;
            float _EmissionIntensity;
            
            float _PulseSpeed;
            float _PulseMin;
            float _PulseMax;
            
            sampler2D _DissolveTex;
            float4 _DissolveTex_ST;
            float _DissolveAmount;
            float _DissolveEdgeWidth;
            fixed4 _DissolveEdgeColor;
            
            sampler2D _DistortionTex;
            float4 _DistortionTex_ST;
            float _DistortionStrength;
            float _DistortionSpeed;
            
            fixed4 _RimColor;
            float _RimPower;
            
            fixed4 _DamageColor;
            float _DamageAmount;
            
            float _FresnelPower;
            float _FresnelScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.dissolveUV = TRANSFORM_TEX(v.uv, _DissolveTex);
                o.distortionUV = TRANSFORM_TEX(v.uv, _DistortionTex);
                
                // 월드 좌표계 변환
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(_WorldSpaceCameraPos - o.worldPos);
                
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 시간 기반 애니메이션
                float time = _Time.y;
                
                // 디스토션 효과
                float2 distortionOffset = tex2D(_DistortionTex, i.distortionUV + time * _DistortionSpeed).rg;
                distortionOffset = (distortionOffset - 0.5) * _DistortionStrength;
                float2 distortedUV = i.uv + distortionOffset;
                
                // 메인 텍스처 샘플링
                fixed4 mainColor = tex2D(_MainTex, distortedUV);
                mainColor *= _Color;
                
                // 펄싱 효과
                float pulse = lerp(_PulseMin, _PulseMax, sin(time * _PulseSpeed) * 0.5 + 0.5);
                mainColor.rgb *= pulse;
                
                // 림 라이팅 효과
                float rim = 1.0 - saturate(dot(i.viewDir, i.worldNormal));
                rim = pow(rim, _RimPower);
                fixed3 rimEffect = _RimColor.rgb * rim;
                
                // 프레넬 효과
                float fresnel = pow(1.0 - saturate(dot(i.viewDir, i.worldNormal)), _FresnelPower);
                fresnel *= _FresnelScale;
                
                // 발광 효과
                fixed3 emission = _EmissionColor.rgb * _EmissionIntensity * pulse;
                
                // 데미지 효과
                fixed3 damageEffect = lerp(fixed3(1,1,1), _DamageColor.rgb, _DamageAmount);
                mainColor.rgb *= damageEffect;
                
                // 디졸브 효과
                float dissolveNoise = tex2D(_DissolveTex, i.dissolveUV).r;
                float dissolveEdge = step(_DissolveAmount, dissolveNoise) * 
                                   step(dissolveNoise, _DissolveAmount + _DissolveEdgeWidth);
                
                // 최종 색상 조합
                fixed3 finalColor = mainColor.rgb + emission + rimEffect + 
                                   (dissolveEdge * _DissolveEdgeColor.rgb * 3.0);
                
                // 프레넬을 알파에 적용
                float alpha = mainColor.a * (1.0 + fresnel * 0.5);
                
                // 디졸브 알파 처리
                alpha *= step(_DissolveAmount, dissolveNoise);
                
                fixed4 col = fixed4(finalColor, alpha);
                
                // 포그 적용
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
