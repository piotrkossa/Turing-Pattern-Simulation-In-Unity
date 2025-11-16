Shader "TuringSimulation/SeedShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SeedSize ("Seed Size", Int) = 10
        _Resolution ("Resolution", Float) = 512
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vertexFunction
            #pragma fragment fragmentFunction

            #include "UnityCG.cginc"

            sampler2D _MainTex; 
            float _Resolution;
            int _SeedSize;

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


            v2f vertexFunction (appdata IN)
            {
                v2f OUT;

                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.uv;

                return OUT;
            }

            fixed4 fragmentFunction (v2f i) : SV_Target
            {
                int2 center = int2(_Resolution / 2, _Resolution / 2);
                int2 pixelPos = int2(i.uv * _Resolution);

                int2 diff = pixelPos - center;


                if (abs(diff.x) < _SeedSize / 2 && abs(diff.y) < _SeedSize / 2)
                {
                    return fixed4(0.0, 1.0, 0.0, 1.0);
                }

                return tex2D(_MainTex, i.uv);
            }

            ENDHLSL
        }
    }
}
