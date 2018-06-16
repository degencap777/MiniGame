Shader "VisualManager/FogShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_FogMap ("FogMap", 2D) = "Black" {}
		_Explored ("Explored", Range(0,1)) = 0.3
	}
	SubShader {
		Tags
		{
			"Queue" = "Overlay-1"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		Lighting Off
		ZWrite Off

		AlphaToMask On

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 _Color;
			float _Explored;
			sampler2D _FogMap;

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

			v2f vert(appdata_t IN)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(IN.vertex);
				o.texcoord = IN.texcoord;
				return o;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				float2 a = tex2D(_FogMap, IN.texcoord).ba;
				return fixed4(_Color.xyz,1-a.x - a.y * _Explored);
			}
			ENDCG
		}
	}
}