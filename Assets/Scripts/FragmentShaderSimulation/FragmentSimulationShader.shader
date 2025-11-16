Shader "TuringSimulation/VisualisationShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Resolution ("Resolution", Float) = 512.0
        _DiffusionU ("Diffusion U", Float) = 0.16
        _DiffusionV ("Diffusion V", Float) = 0.08
        _FeedRate ("Feed Rate", Float) = 0.035
        _KillRate ("Kill Rate", Float) = 0.065
        _DeltaTime ("Delta Time", Float) = 1.0
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

            float _DiffusionU;
            float _DiffusionV;
            float _FeedRate;
            float _KillRate;
            float _DeltaTime;

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
                float v = tex2D(_MainTex, i.uv).g;

                float2 offsetX = float2(1.0/_Resolution, 0);
                float2 offsetY = float2(0, 1.0/_Resolution0);

                float laplacianU = 
                    tex2D(_MainTex, i.uv + offsetY).r               * 0.2
                    + tex2D(_MainTex, i.uv - offsetY).r             * 0.2
                    + tex2D(_MainTex, i.uv + offsetX).r             * 0.2
                    + tex2D(_MainTex, i.uv - offsetX).r             * 0.2
                    + tex2D(_MainTex, i.uv + offsetX + offsetY).r   * 0.05
                    + tex2D(_MainTex, i.uv + offsetX - offsetY).r   * 0.05
                    + tex2D(_MainTex, i.uv - offsetX + offsetY).r   * 0.05
                    + tex2D(_MainTex, i.uv - offsetX - offsetY).r   * 0.05
                    - u;

                float laplacianU = 
                    tex2D(_MainTex, i.uv + offsetY).g               * 0.2
                    + tex2D(_MainTex, i.uv - offsetY).g             * 0.2
                    + tex2D(_MainTex, i.uv + offsetX).g             * 0.2
                    + tex2D(_MainTex, i.uv - offsetX).g             * 0.2
                    + tex2D(_MainTex, i.uv + offsetX + offsetY).g   * 0.05
                    + tex2D(_MainTex, i.uv + offsetX - offsetY).g   * 0.05
                    + tex2D(_MainTex, i.uv - offsetX + offsetY).g   * 0.05
                    + tex2D(_MainTex, i.uv - offsetX - offsetY).g   * 0.05
                    - v;

                float reaction = u * (v * v);

                float deltaU = (_DiffusionU * laplacianU) 
                    - reaction 
                    + (_FeedRate * (1.0 - u));

                float deltaV = (_DiffusionV * laplacianV) 
                    + reaction 
                    - ((_KillRate + _FeedRate) 
                    * v);

                float newU = clamp(u + deltaU * _DeltaTime, 0.0, 1.0);
                float newV = clamp(v + deltaV * _DeltaTime, 0.0, 1.0);


                return float4(newU, newV, 0.0, 1.0);;
            }

            ENDHLSL
        }
    }
}
