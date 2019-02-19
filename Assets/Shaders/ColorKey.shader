﻿Shader "Hidden/ColorKey"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
			sampler2D webCamImage;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, float2(1.0 - i.uv.x,i.uv.y));
				fixed4 colA = tex2D(webCamImage, float2(1.0-i.uv.x,i.uv.y));
                // just invert the colors
                //col.rgb = colA.rgb;
				if (col.a == 0.0)
				{
					col.rgb = float3(1.0,0.0,0.0);
				}
                return col;
				//return colA;
            }
            ENDCG
        }
    }
}
