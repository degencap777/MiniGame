// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "VisualManager/miniMapShader"
{
	Properties
	{
		_Color("Tint", Color) = (1, 1, 1, 1)
		_Mask("Mask Texture", 2D) = "white" {}
		_MainTex("Mini Map", 2D) = "white" {}
		_FogMap("Fog Map", 2D) = "black" {}

	}

	SubShader
	{
		Tags
		{
			"Queue" = "Overlay+1"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off

		AlphaToMask On

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			sampler2D _Mask;
			sampler2D _FogMap;
			float4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
		
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif
				return OUT;
			}


			fixed4 frag(v2f IN) : SV_Target
			{
				float4 layer20 = tex2D(_MainTex, IN.texcoord);
				float2 a = tex2D(_FogMap,IN.texcoord).ba;
				float ms = clamp(a.x + a.y * 0.6,0,1);
				float4 c = fixed4(layer20.rgb * ms + _Color.rgb * (1-ms),layer20.a); 
				float4 cmask = tex2D(_Mask, IN.texcoord).a;
				return fixed4(c.rgb + cmask.rgb * (1- c.a),cmask.a);
			}
			ENDCG
		}
	}
}