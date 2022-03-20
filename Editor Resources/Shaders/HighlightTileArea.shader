Shader "Unlit/HighlightTileArea"
{
    Properties
    {
        _GirdSize ("Gird Size", Vector) = (2.0, 0.0, 2.0, 0.0)
        //RectSize should be controlled by Offset of the highlight out line
        _RectSize ("Rect Size", Vector) = (0.7, 0.7, 0.0, 0.0) //need to fixe range [0, 1]
        _MainTex  ("Texture", 2D)       = "white" {}
        
        _Rounding ("Rounding Corner", Float) = 0.5
        _Rotation ("Rotation", Float) = 5.0
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

            float2 _RectSize;

            float _Rounding;
            float _Rotation;
            
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

            float GetTileType(int _type, float2 _pos)
            {
                float _r         = 0.0f;
                float _distDelta = 0.0f;
                float _antiAlias = 0.0f;
                
                [branch] switch(_type)
                {
                    case 0:
                        _pos = Translate(_pos, float2(0.5, 0.5));
                        
                        _r = AABoxSDF(_pos, _RectSize);

                        _distDelta = fwidth(_r);
				        _antiAlias = smoothstep(_distDelta, -_distDelta, _r);
                        
                        return _antiAlias;
                    case 1:

                        //float2 _newRect = float2(_RectSize.x - _Rounding * 2, _RectSize.y - _Rounding * 2);
                        
                        _pos      = Translate(_pos, float2(0.5, 0.5 + (1 - _RectSize.y)/4 ));

                        _RectSize = float2( _RectSize.x, _RectSize.y + (1 - _RectSize.y)/2);
                        
                        _r = AABoxSDF(_pos, _RectSize) - _Rounding;

                        _distDelta = fwidth(_r);
				        _antiAlias = smoothstep(_distDelta, -_distDelta, _r);
                        
                        return _antiAlias;
                    case 2:
                        return 0.0f;
                    default:
                        return 0.0f;
                        
                }
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 pos = i.uv.xz;
                pos /= 5;
                pos = frac(pos);

				float2 _index = GetTileIndex(i.uv.xz);

                //This should return int that represent tile type 0-47//
                float _d = 0;

                int _row    = 5;
                int _column = 5;

                for(int _i = 0; _i < _row; _i++){
                    for(int _j = 0; _j < _column; _j++){

                        //float _tileType = _TilesDataArray[_i + _j * 2];

                        //if(_index.x == _i && _index.y == _j)
                        //{
                            
                        //}


                        //Coordi -> GetType -> Draw Col
                       
                    }
                }

                //float _idX = _index.x;
                //float _idY = _index.y;
                
                //float _tileType = _TilesDataArray[_idX + _idY * 2];

                //Switch _tileType//
                

                //index 0,0 -> check tile type -> generate SDF//
                //Foreach loop?//
				//if(_index.x == 4 && _index.y == 0)
				//{
				//    pos = Translate(pos, float2(0.5, 0.5));
				//    _d  = AABoxSDF (pos, float2(0.5, 0.5));
				//}

                //if(_index.x == 1)
                //{
                    //pos = Translate(pos, float2(0.5, 0.5));
				//    _d = AABoxSDF(pos, float2(0.5, 0.5));
                //}
                
                //fixed4 col = fixed4(pos.x, 0, pos.y,1.0);

                //fixed4 col = fixed4(_d, _d, _d, 1.0);

                fixed4 col = GetTileType(1, pos);
                
                return col;
            }
            ENDCG
        }
    }
}
