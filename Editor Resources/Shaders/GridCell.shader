Shader "Unlit/GridCell"
{
    Properties
    {
		_Position         ("Position", Vector)       = (1.0, 1.0, 1.0, 1.0) 
        _RectSize         ("Rectangle Size", Vector) = (1.0, 1.0, 1.0, 1.0)
		_RectSize2		  ("Second Rectangle Size", Vector) = (1.0, 1.0, 1.0, 1.0)
        _Rounding 		  ("Rounding",       Float)  = 0.2
		_LineThickness    ("Line Thickness", Float)  = 0.2
        _SecondRing       ("Second Ring",    Float)  = 0.2
        _OutsideRingColor ("Outside Ring Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _InsideRingColor  ("Inside Rign Color",  Color) = (1.0, 1.0, 1.0, 1.0)
        _MainTex          ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "SDF.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 uv     : TEXCOORD0;
            };

			float2 _Position;
			
            float2 _RectSize;
			float2 _RectSize2;

            float _Rounding;
			float _LineThickness;
            float _SecondRing;
            fixed4 _OutsideRingColor;
            fixed4 _InsideRingColor;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv     = v.vertex;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 st = i.uv.xz;

                st /= 5;
                st = frac(st);

				st = Translate(st, _Position);

                float r = AABoxSDF(st, _RectSize) - _Rounding;
				
				r = abs(r) - _LineThickness;
				
                float r2 = AABoxSDF(st, _RectSize2) - _Rounding;

				r2 = abs(r2) - _LineThickness;

				float d = Union(r, r2);

				float distDelta2 = fwidth(d);
				float antialias2 = smoothstep(distDelta2, -distDelta2, d);

				fixed4 col2 = fixed4(antialias2, antialias2, antialias2, 1.0);

				fixed4 col = col2;
				
                return col;
            }
            ENDCG
        }
    }
}
