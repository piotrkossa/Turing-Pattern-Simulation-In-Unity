Shader "TuringSimulation/VisualisationShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
                float u = tex2D(_MainTex, i.uv).r;

                //float v = tex2D(_MainTex, i.uv).g;


                float4 color = float4(1 - u, 1- u, 1 - u, 1);

                return color;
            }

            ENDCG
        }
    }
}
