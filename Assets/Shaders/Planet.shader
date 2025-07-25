Shader "Custom/Planet"
{
    Properties
    {
        // Surface textures
        _MainTex ("Surface Albedo", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _SpecularMap ("Specular Map", 2D) = "black" {}
        
        // Night side
        _NightTex ("Night Lights", 2D) = "black" {}
        _NightIntensity ("Night Lights Intensity", Range(0, 2)) = 1.0
        
        // Clouds
        _CloudTex ("Cloud Texture", 2D) = "black" {}
        _CloudSpeed ("Cloud Speed", Range(0, 1)) = 0.1
        _CloudOpacity ("Cloud Opacity", Range(0, 1)) = 0.8
        
        // Atmosphere
        _AtmosphereColor ("Atmosphere Color", Color) = (0.5, 0.8, 1.0, 1.0)
        _AtmosphereIntensity ("Atmosphere Intensity", Range(0, 3)) = 1.5
        _AtmosphereSize ("Atmosphere Size", Range(0, 2)) = 1.2
        
        // Surface properties
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        
        // Light direction (for day/night)
        _SunDirection ("Sun Direction", Vector) = (0, 0, 1, 0)
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }
        LOD 200
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            TEXTURE2D(_SpecularMap);
            SAMPLER(sampler_SpecularMap);
            TEXTURE2D(_NightTex);
            SAMPLER(sampler_NightTex);
            TEXTURE2D(_CloudTex);
            SAMPLER(sampler_CloudTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _AtmosphereColor;
                float _AtmosphereIntensity;
                float _AtmosphereSize;
                float _NightIntensity;
                float _CloudSpeed;
                float _CloudOpacity;
                half _Metallic;
                half _Smoothness;
                float4 _SunDirection;
            CBUFFER_END
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float3 tangentWS : TEXCOORD3;
                float3 bitangentWS : TEXCOORD4;
                float3 viewDirWS : TEXCOORD5;
            };
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.normalWS = normalInput.normalWS;
                output.tangentWS = normalInput.tangentWS;
                output.bitangentWS = normalInput.bitangentWS;
                output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                // Sample textures
                half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half4 normalMap = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv);
                half4 specular = SAMPLE_TEXTURE2D(_SpecularMap, sampler_SpecularMap, input.uv);
                half4 nightLights = SAMPLE_TEXTURE2D(_NightTex, sampler_NightTex, input.uv);
                
                // Clouds with animation
                float2 cloudUV = input.uv + float2(_Time.y * _CloudSpeed, 0);
                half4 clouds = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, cloudUV);
                
                // Normal mapping
                float3x3 tangentToWorld = float3x3(input.tangentWS, input.bitangentWS, input.normalWS);
                float3 normalTS = UnpackNormal(normalMap);
                float3 normalWS = normalize(mul(normalTS, tangentToWorld));
                
                // Lighting calculation
                float3 lightDir = normalize(_SunDirection.xyz);
                float NdotL = dot(normalWS, lightDir);
                float lightIntensity = saturate(NdotL);
                
                // Night mask
                float nightMask = saturate((-NdotL + 0.3) * 2.0);
                
                // Combine day and night
                half3 dayColor = albedo.rgb * max(lightIntensity, 0.1);
                half3 nightColor = nightLights.rgb * _NightIntensity;
                half3 finalColor = lerp(nightColor, dayColor, lightIntensity);
                
                // Apply clouds
                float cloudMask = clouds.a * _CloudOpacity;
                finalColor = lerp(finalColor, clouds.rgb, cloudMask);
                
                // Atmosphere rim lighting
                float3 viewDir = normalize(input.viewDirWS);
                float rim = 1.0 - saturate(dot(viewDir, normalWS));
                rim = pow(rim, max(_AtmosphereSize, 0.1));
                half3 atmosphereGlow = _AtmosphereColor.rgb * rim * _AtmosphereIntensity;
                
                finalColor += atmosphereGlow * lightIntensity;
                
                return half4(saturate(finalColor), 1.0);
            }
            ENDHLSL
        }
        
        Pass
        {
            Name "Atmosphere"
            Tags { "LightMode" = "UniversalForward" }
            Blend One One
            ZWrite Off
            Cull Front
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
                float4 _AtmosphereColor;
                float _AtmosphereIntensity;
                float _AtmosphereSize;
            CBUFFER_END
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
            };
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                // Expand for atmosphere
                float3 expandedPos = input.positionOS.xyz + input.normalOS * 0.1;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(expandedPos);
                
                output.positionCS = vertexInput.positionCS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                float3 viewDir = normalize(input.viewDirWS);
                float3 normalWS = normalize(input.normalWS);
                
                float rim = 1.0 - saturate(dot(viewDir, normalWS));
                rim = pow(rim, _AtmosphereSize * 0.5);
                
                return _AtmosphereColor * rim * _AtmosphereIntensity * 0.3;
            }
            ENDHLSL
        }
    }
    
    FallBack "Universal Render Pipeline/Lit"
}
