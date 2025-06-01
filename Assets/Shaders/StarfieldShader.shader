Shader "Custom/StarfieldShader"
{
    Properties
    {
        _StarDensity("Star Density", Range(10, 1000)) = 200
        _StarSize("Star Size", Range(0.0001, 1)) = 0.1
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

            // Hash function to generate pseudo-random values
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123);
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
                float brightness = 0.0;
                float2 uv = i.uv;

                // Scale UVs for density
                float2 gridUV = uv * _StarDensity;

                // Get cell index
                float2 cell = floor(gridUV);

                // Relative position inside the cell
                float2 cellUV = frac(gridUV);

                // Generate pseudo-random position of the star within the cell
                float2 starPos = float2(hash(cell), hash(cell + 1.0));

                // Distance from current fragment to star position
                float dist = distance(cellUV, starPos);

                // If close to the star position, draw a star
                brightness = smoothstep(_StarSize, 0.0, dist);

                return fixed4(brightness, brightness, brightness, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
