Shader "Unlit/CirclePointer"
{
    Properties
    {
        _PointerPos ("Pointer Position", Vector) = (0, 0, 0, 0)
        _Radius     ("Radius", float)            = 1.0
        _MainTex    ("Texture", 2D)              = "white" {}
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

            #include "UnityCG.cginc"
            #include "SDF.cginc"

            struct appdata
            {
                float3 uv     : TEXCOORD0;
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float3 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float3 _PointerPos;
            float  _Radius;

            v2f vert (appdata v)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv     = v.vertex;
             
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                float2 newPos = Translate(i.uv.xz, _PointerPos);
                
                float circle = Circle(newPos, _Radius);
                
                fixed4 col = fixed4(circle, circle, circle, 1.0);
              
                return col;
            }
            ENDCG
        }
    }
}
