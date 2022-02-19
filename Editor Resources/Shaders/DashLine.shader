Shader "Unlit/DashLine"
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

                st *= 10;
                //st = frac(st);
                float b = sin(st.x * _Time.x);
                
                float r = fmod(st.x, 2.0);
                r = step(1.0, r);
                
                
                fixed4 col = fixed4(r, r, r, 1.0);
              
                return col;
            }
            ENDCG
        }
    }
}
