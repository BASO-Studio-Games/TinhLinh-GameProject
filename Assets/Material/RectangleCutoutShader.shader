Shader "Custom/RectangleStencilMask"
{
    Properties
    {
        _Center ("Center", Vector) = (0.5, 0.5, 0, 0) 
        _Size ("Size", Vector) = (0.2, 0.1, 0, 0) 
    }

    SubShader
    {
        Tags { "Queue"="Overlay" }

        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
        }

        Pass
        {
            ZWrite Off
            ColorMask 0

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float2 _Center;
            float2 _Size;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (i.uv.x < _Center.x - _Size.x || i.uv.x > _Center.x + _Size.x ||
                    i.uv.y < _Center.y - _Size.y || i.uv.y > _Center.y + _Size.y)
                {
                    discard; 
                }

                return fixed4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
}
