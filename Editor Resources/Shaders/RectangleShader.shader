Shader "Unlit/RectangleShader"
{
    Properties
    {
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

        Pass
        {
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            float2 rotate(float2 samplePosition, float rotation)
            {
                const float PI = 3.14159;
                
                float angle = rotation * PI * 2 * -1;
                float sine, cosine;
                
                sincos(angle, sine, cosine);
                
                return float2(cosine * samplePosition.x + sine * samplePosition.y, cosine * samplePosition.y - sine * samplePosition.x);
            }

            float2 translate(float2 samplePosition, float2 offset)
            {
                //move samplepoint in the opposite direction that we want to move shapes in
                return samplePosition - offset;
            }

            float2 scale(float2 samplePosition, float scale)
            {
                return samplePosition / scale;
            }

            float circle(float2 samplePosition, float radius)
            {
                //get distance from center and grow it according to radius
                return length(samplePosition) - radius;
            }

            float rectangle(float2 samplePosition, float2 halfSize)
            {
                float2 componentWiseEdgeDistance = abs(samplePosition) - halfSize;

                float outsideDistance = length(max(componentWiseEdgeDistance, 0));
                float insideDistance  = min(max(componentWiseEdgeDistance.x, componentWiseEdgeDistance.y), 0);
                
                return outsideDistance + insideDistance;
            }

            v2f vert (appdata v)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv     = v.vertex;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //i.uv *= _GridRow;
                //i.uv = frac(i.uv);
                
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

                col = rectangle(i.uv.xz, float2(_HalfSize.x, _HalfSize.y));
                
                return col;
            }
            ENDCG
        }
    }
     
    FallBack "Standard" 
}
