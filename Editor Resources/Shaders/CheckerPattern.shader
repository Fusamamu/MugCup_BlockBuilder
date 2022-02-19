Shader "Unlit/CheckerPattern"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
                float3 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv     = v.vertex;
             
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 pos = i.uv.xz;

                pos /= 5;
                pos = frac(pos);

                pos *= 2;
                //Should have a helper function Get Index in Grid//
                float index = 0.0;
                index += step(1., fmod(pos.x, 2.0));
                index += step(1., fmod(pos.y, 2.0)) * 2.0;

                fixed4 col = fixed4(pos.x, 0, pos.y, 1.0);
                
                if(index == 0.0){
                    col = fixed4(1.0, 1.0, 1.0, 1.0);   
                }else if(index == 1.0){
                    col = fixed4(0.0, 0.0, 0.0, 1.0);
                }else if(index == 2.0){
                    col = fixed4(0.0, 0.0, 0.0, 1.0);
                }else if(index == 3.0){
                    col = fixed4(1.0, 1.0, 1.0, 1.0);
                }
          
                return col;
            }
            ENDCG
        }
    }
}
