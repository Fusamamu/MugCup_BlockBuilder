Shader "Unlit/HighlightTileArea"
{
    Properties
    {
        //TODO
        //Make OutLine Animation
        //Set Gap by Offset Property
            //RectSize should be controlled by Offset of the highlight out line
        //Set Gradient inside SDF
        
        [KeywordEnum(Fill, Center Outline, Inside Outline, Outside Outline)] _DrawMode("Draw mode", Float) = 0
        
        _GirdSize ("Gird Size", Vector) = (2.0, 0.0, 2.0, 0.0)
        _Offset   ("Offset",    Float)  = 0.05
        _RectSize ("Rect Size", Vector) = (0.7, 0.7, 0.0, 0.0) //need to fixe range [0, 1]
        _MainTex  ("Texture",   2D)     = "white" {}
        
        _Rounding ("Rounding Corner", Float) = 0.5
        
        _LineThickness ("Line Thinkness", Float) = 0.1
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

            float _Offset;
            float _Rounding;
            float _LineThickness;
            float _Rotation;

            float _DrawMode;
            
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
                float2 _originalPos = _pos;
                
                float _r1         = 0.0f;
                float _r2         = 0.0f;
                float _finalResult = 0.0f;
                
                float _distDelta = 0.0f;
                float _antiAlias = 0.0f;
                
                [branch] switch(_type)
                {
                    case 0:
                        _pos = Translate(_pos, float2(0.5, 0.5));
                        
                        _r1 = AABoxSDF(_pos, _RectSize);

                        _distDelta = fwidth(_r1);
				        _antiAlias = smoothstep(_distDelta, -_distDelta, _r1);
                        
                        return _antiAlias;
                    case 1:

                        _pos      = Translate(_pos, float2(0.5, 0.5 + (1 - _RectSize.y)/4));
                        _pos      = Rotate(_pos, _Rotation);

                        _RectSize = float2( _RectSize.x, _RectSize.y + (1 - _RectSize.y)/2);
                        
                        _r1 = AABoxSDF_Separate_Rounded_Corner(_pos, _RectSize, float4(0, 0, _Rounding, _Rounding)) + _LineThickness;

                        if(_DrawMode != 0.0f)
                            _r1 = abs(_r1) - _LineThickness;

                        _distDelta = fwidth(_r1);
				        _antiAlias = smoothstep(_distDelta, -_distDelta, _r1);
                        
                        return _antiAlias;
                    case 2:
                        return 0.0f;
                    case 3:

                        _pos      = Translate(_originalPos, float2(0.0, 1.0));
                        _RectSize = float2(2.0 - _Offset, 2.0 - _Offset);
                        
                        _r1 = AABoxSDF_Separate_Rounded_Corner(_pos, _RectSize, float4(0, 0, 0, _Rounding)) + _LineThickness;

                        if(_DrawMode != 0.0f)
                            _r1 = abs(_r1) - _LineThickness;


                        _pos      = Translate(_originalPos, float2(0.0, 1.0));
                        _RectSize = float2(_Offset, _Offset);
                        
                        _r2 = AABoxSDF_Separate_Rounded_Corner(_pos, _RectSize, float4(0, 0, 0, _Rounding)) - _LineThickness;

                        if(_DrawMode != 0.0f)
                            _r2 = abs(_r2) - _LineThickness;


                        _finalResult = Union(_r1, _r2);
                    

                        _distDelta = fwidth(_finalResult);
				        _antiAlias = smoothstep(_distDelta, -_distDelta, _finalResult);
                        
                        return _antiAlias;
                    
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

                fixed4 col = GetTileType(3, pos);
                
                return col;
            }
            ENDCG
        }
    }
}
