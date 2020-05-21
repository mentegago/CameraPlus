Shader "Unlit/AlphaFilter"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "defaulttexture" {}
	}
	SubShader
	{
		Blend SrcAlpha OneMinusSrcAlpha
		
		Tags { "RenderType" = "Transparent" "IgnoreProjector"="True" "Queue" = "Transparent" }
		LOD 100
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv_MainTex : TEXCOORD0;
			};
			
			float4 _MainTex_ST;
			
			sampler2D _MainTex;
			
			fixed4 _ColorKey;
			
			v2f vert(appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			
			float4 frag(v2f IN) : COLOR {
				half4 c = tex2D (_MainTex, IN.uv_MainTex);
				c.a = (1 - c.a);
				//if(c.a == 1) discard;
				return c;
			}
			ENDCG
		}
	}
}