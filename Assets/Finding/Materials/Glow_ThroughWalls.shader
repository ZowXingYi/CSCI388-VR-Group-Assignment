Shader "Custom/GlowThroughWalls"
{
    Properties
    {
        _Color ("Glow Color", Color) = (1, 0.8, 0.2, 1)
        _Intensity ("Intensity", Range(1, 10)) = 3
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" "RenderType" = "Transparent" }
        LOD 100

        Pass
        {
            ZTest Always     // ← This makes it visible through walls
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float4 _Color;
            float _Intensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _Color * _Intensity;
            }
            ENDCG
        }
    }
}