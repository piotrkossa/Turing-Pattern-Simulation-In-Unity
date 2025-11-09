Shader "TuringSimulation/LSDShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Frequency ("Color Frequency", Range(1, 50)) = 1.0
        _ColorSpeed ("Color Speed", Range(0, 5)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM

            #pragma vertex vertexFunction
            #pragma fragment fragmentFunction

            #include "UnityCG.cginc"

            sampler2D _MainTex; 

            float _Frequency;
            float _ColorSpeed;


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

            float4 getLSDColor(float u, float v) {
                float r = sin(u * _Frequency + _Time.y * _ColorSpeed) * 0.5 + 0.5;
                float g = sin(v * _Frequency + _Time.y * _ColorSpeed + 2.0) * 0.5 + 0.5;
                float b = sin((u-v) * _Frequency + _Time.y * _ColorSpeed + 4.0) * 0.5 + 0.5;

                return float4(r, g, b, 1.0);
            }

            fixed4 fragmentFunction (v2f i) : SV_Target
            {
                float u = tex2D(_MainTex, i.uv).r;
                float v = tex2D(_MainTex, i.uv).g;


                float4 color = getLSDColor(u, v);

                return color;
            }

            ENDCG
        }
    }
}
