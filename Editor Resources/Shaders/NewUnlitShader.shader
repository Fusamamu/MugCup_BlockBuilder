Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _RectSize   ("Rectangle Size", Vector) = (0, 0, 0, 0)
        _Position2D ("Position", Vector) = (0, 0, 0, 0)
        _Radius     ("Radius", Float)    = 2.0
        _Color      ("Tint", Color)      = (1, 1, 1, 1)
        _MainTex    ("Texture", 2D)      = "white" {}
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
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float2 _RectSize;
            float2 _Position2D;
            fixed4 _Color;

            float _Radius;

            v2f vert (appdata v)
            {
                v2f o;
                
                o.position = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                    
                return o;
            }

            float2 Translate(float2 _samplePosition, float2 _offset)
            {
                return _samplePosition - _offset;
            }
            
            float Circle(float2 _samplePos, float _radius)
            {
                return length(_samplePos) - _radius;
            }

            float Rectangle(float2 _samplePosition, float2 _halfSize)
            {
                float2 componentWiseEdgeDistance = abs(_samplePosition) - _halfSize;

                float outsideDistance = length(max(componentWiseEdgeDistance, float2(0, 0)));
                float insideDistance  = min(max(componentWiseEdgeDistance.x, componentWiseEdgeDistance.y), 0);
                
                return outsideDistance + insideDistance;
            }
            
            float scene(float2 _position)
            {
                float2 _circlePos   = Translate(_position, _Position2D);
                
                //float sceneDistance = Circle(_circlePos, _Radius);


                float sceneDistance = Rectangle(_circlePos, _RectSize);
                
                return sceneDistance;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                float dist = scene(i.worldPos.xz);

                //fixed4 col = fixed4(dist, dist, dist, 1);
                
                float distanceChange = fwidth(dist) * 0.5;
                
                float antialiasedCutoff = smoothstep(distanceChange, -distanceChange, dist);
                
                fixed4 col = fixed4(_Color.xyz, antialiasedCutoff);
               
                return col;
            }
            ENDCG
        }
    }
    
    FallBack "Standard" 
}
