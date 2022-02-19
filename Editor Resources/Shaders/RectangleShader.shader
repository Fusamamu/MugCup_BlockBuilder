Shader "Unlit/RectangleShader"
{
    Properties
    {
        _OffsetPos ("Offset Position", Vector) = (0, 0, 0, 0)
        _Rotation  ("Rotation", Float) = 0.0
        _Rounding ("Rounding", Float) = 0.1
        
        _GridRow   ("Grid Row", Int)     = 4
        _GridColor ("Grid Color", Color) = (1.0, 1.0, 1.0, 1.0)
        
        _LeftBorder  ("LeftBorder",   Float) = 0.1
        _RightBorder ("RightBorder",  Float) = 0.1
        _UpperBorder ("UpperBorder",  Float) = 0.1
        _BottomBorder("BottomBorder", Float) = 0.1
        
        _MainTex ("Texture", 2D) = "white" {}
        
        _HalfSize ("Half Size", Vector) = (0.1, 0.1, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        
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

            sampler2D _MainTex;
            float4    _MainTex_ST;

            int _GridRow;
            
            fixed4 _GridColor; 

            float _LeftBorder;
            float _RightBorder;
            float _UpperBorder;
            float _BottomBorder;

            float4 _HalfSize;

            float _Rotation;
            float4 _OffsetPos;
            float _Rounding;

            v2f vert (appdata v)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv     = v.vertex;
                
                return o;
            }

            float AABoxSDF(float2 pos, float2 dimension, float rounding)
            {
                float2 dist = abs(pos) - dimension;
                return length(max(dist, 0)) + min(max(dist.x, dist.y), 0) - rounding;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                i.uv *= _GridRow;
                i.uv = frac(i.uv);
                
                fixed4 col;

                float x  = i.uv.x;
                float z  = i.uv.z;
                
                float leftBorder   = step(_LeftBorder,   x);
                float rightBorder  = 1 - step(_RightBorder,  x);
                float bottomBorder = step(_BottomBorder, z);
                float upperBorder  = 1 - step(_UpperBorder, z);

                float r = leftBorder * rightBorder * upperBorder * bottomBorder;

                
                //col = fixed4(r, r, r, 1);
                //col *= _GridColor;

                //float dist       = rectangle(i.uv.xz, float2(_HalfSize.x, _HalfSize.y));
                //float distChange = fwidth(dist) * 0.5;
                
                //float antialias = smoothstep(distChange, -distChange, dist);

                //col = float4(antialias, antialias, antialias, 1.0);
                //////

                //col = AABoxSDF(i.uv.xz, _HalfSize, _Rounding);

                //float2 newPos = Translate(i.uv.xz, _OffsetPos);

                //newPos = RotateMatrix(newPos, _Rotation);

                //col = AABoxSDFOutside(i.uv.xz, _HalfSize);
                
                fixed4 shapeOne = AABoxSDFRoundedOutside(i.uv.xz, _HalfSize, _Rounding);

                float2 newPos = Translate(i.uv.xz, _OffsetPos);
                fixed4 shapeTwo = AABoxSDFRoundedOutside(newPos, _HalfSize, 0);

                col = Union(shapeOne, shapeTwo);

                col = shapeTwo;

                float outside = AABoxSDF(newPos, _HalfSize);
                float inside  = AABoxSDF(newPos, _HalfSize * 0.5f);
                float result  = Subtract(outside, inside);

                
                float distChange = fwidth(result) * 0.5;
                float antialias = smoothstep(distChange, -distChange, result);

                fixed4 shapeThree = fixed4(antialias, antialias, antialias, 1.0);
                col = shapeThree;
                
                //col = fixed4(i.uv.x, 0, 0, 1.0);
                
                return col;
            }
            ENDCG
        }
    }
     
    FallBack "Standard" 
}
