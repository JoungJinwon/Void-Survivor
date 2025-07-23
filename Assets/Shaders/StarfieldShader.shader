Shader "Custom/StarfieldShader"
{
    Properties
    {
        _StarDensity("Star Density", Range(10, 1000)) = 200
        _StarSize("Star Size", Range(0.0001, 1)) = 0.1
        _DepthLayers("Depth Layers", Range(1, 5)) = 3
        _BigStarChance("Big Star Chance", Range(0, 1)) = 0.1
        _BigStarSizeMultiplier("Big Star Size Multiplier", Range(1, 10)) = 3
        _StarColor1("Star Color 1", Color) = (1, 1, 1, 1)
        _StarColor2("Star Color 2", Color) = (0.8, 0.9, 1, 1)
        _StarColor3("Star Color 3", Color) = (1, 0.8, 0.6, 1)
        _TwinkleSpeed("Twinkle Speed", Range(0, 10)) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Include Unity Shader libraries
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _StarDensity;
            float _StarSize;
            float _DepthLayers;
            float _BigStarChance;
            float _BigStarSizeMultiplier;
            float4 _StarColor1;
            float4 _StarColor2;
            float4 _StarColor3;
            float _TwinkleSpeed;

            // Hash function to generate pseudo-random values
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123);
            }

            // Hash function for different seed
            float hash2(float2 p)
            {
                return frac(sin(dot(p, float2(269.5, 183.3))) * 17.15);
            }

            // Hash function for color selection
            float hash3(float2 p)
            {
                return frac(sin(dot(p, float2(419.2, 371.9))) * 623.11);
            }

            // Main vertex function
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Main fragment function
            fixed4 frag(v2f i) : SV_Target
            {
                float3 finalColor = float3(0, 0, 0);
                float2 uv = i.uv;

                // Create multiple depth layers for parallax effect
                for (int layer = 0; layer < (int)_DepthLayers; layer++)
                {
                    float layerDepth = (float)(layer + 1) / _DepthLayers;
                    float layerScale = lerp(0.3, 1.0, layerDepth); // Closer layers are smaller scale
                    float layerBrightness = lerp(0.3, 1.0, layerDepth); // Closer layers are dimmer
                    
                    // Scale UVs for density with layer depth
                    float2 gridUV = uv * _StarDensity * layerScale;

                    // Get cell index
                    float2 cell = floor(gridUV);

                    // Relative position inside the cell
                    float2 cellUV = frac(gridUV);

                    // Generate pseudo-random position of the star within the cell
                    float2 starPos = float2(hash(cell + layer * 100), hash2(cell + layer * 100));

                    // Distance from current fragment to star position
                    float dist = distance(cellUV, starPos);

                    // Check if this star should be a big star
                    float bigStarRandom = hash3(cell + layer * 100);
                    bool isBigStar = bigStarRandom < _BigStarChance;
                    
                    // Calculate star size based on layer depth and big star status
                    float currentStarSize = _StarSize * layerScale;
                    if (isBigStar)
                    {
                        currentStarSize *= _BigStarSizeMultiplier;
                    }

                    // Calculate brightness with smooth falloff
                    float brightness = smoothstep(currentStarSize, 0.0, dist) * layerBrightness;
                    
                    // Add twinkling effect
                    float twinkle = sin(_Time.y * _TwinkleSpeed + hash(cell + layer * 100) * 6.28) * 0.3 + 0.7;
                    brightness *= twinkle;

                    // Select star color based on random value
                    float colorRandom = hash2(cell + layer * 50);
                    float3 starColor;
                    if (colorRandom < 0.33)
                        starColor = _StarColor1.rgb;
                    else if (colorRandom < 0.66)
                        starColor = _StarColor2.rgb;
                    else
                        starColor = _StarColor3.rgb;

                    // Add this layer's contribution to final color
                    finalColor += starColor * brightness;
                }

                return fixed4(finalColor, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
