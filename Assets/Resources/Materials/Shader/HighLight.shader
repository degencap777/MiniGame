// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Effect/HighLight"
{
	Properties
	{
		_HighLight("HighLight", Color) = (0,0,0,0)
	}
	SubShader
	{
		Tags {"RenderType"="Transparent" }
		LOD 100
		Pass
		{
			ZWrite off
			ZTest off
			Cull Back 
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			float4 _HighLight;

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				return _HighLight;
			}
			ENDCG
		}
	}
}
