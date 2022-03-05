Shader "Unlit/HighlightTileArea"
{
    Properties
    {
        _GirdSize ("Gird Size", Vector) = (2.0, 0.0, 2.0, 0.0)
        _MainTex  ("Texture", 2D)       = "white" {}
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
                float4 vertex : POSITION;
                float3 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 uv     : TEXCOORD0;
            };

            //GirdSize?? 10x10
            float4 _GridSize;
            float _TilesDataArray[4];
            
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv     = mul (unity_ObjectToWorld, v.vertex);
                
                return o;
            }

            //Grid Index/Pos i,j/x,z
            float2 GetTileIndex(float2 _pos)
            {
                float x_i = floor(_pos.x);
                float y_i = floor(_pos.y);

                float2 _index = float2(x_i, y_i);
                
                return _index;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 pos = i.uv.xz;

                pos = frac(pos);

				float2 _index = GetTileIndex(i.uv.xz);
				
				if(_index.x > 1)
					_index.x = 0;
				if(_index.y > 1)
					_index.y = 0;
					
				float _v = _TilesDataArray[_index.x + _index.y * 2];

				float r = _v;
				
                
                fixed4 col = fixed4(r,r,r,1.0);
                return col;
            }
            ENDCG
        }
    }
}
