Shader "TuringSimulation/DrawingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LastDrawPosition ("Last Draw Position", Vector) = (0,0,0,0)
        _NewDrawPosition ("New Draw Position", Vector) = (0,0,0,0)
        _LineWidth ("Line Width", Float) = 3.0
        _Resolution ("Resolution", Float) = 512.0
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
            float4 _LastDrawPosition;
            float4 _NewDrawPosition;
            float _LineWidth;
            float _Resolution;

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
                float2 p  = i.uv * _Resolution;
                float2 a  = float2(_LastDrawPosition.x, _LastDrawPosition.y);
                float2 b  = float2(_NewDrawPosition.x, _NewDrawPosition.y);

                float2 ab = b - a;
                float2 ap = p - a;

                float t = saturate(dot(ap, ab) / dot(ab, ab));
                float2 closest = a + ab * t;

                float dist = length(p - closest);

                if (dist < _LineWidth)
                {
                    return fixed4(0.0, 1.0, 0.0, 1.0);
                }

                return tex2D(_MainTex, i.uv);
            }

            ENDHLSL
        }
    }
}
