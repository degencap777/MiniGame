Shader "VisualManager/VisualCircle"
{
	Properties{	}
	SubShader
	{
		Tags { "Queue"="Overlay" }
		LOD 10

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0; 
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.vertex.xz;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = i.uv;
				uv.x = 10 - clamp(abs(uv.x),0,0.1)*100;
				uv.y = 10 - clamp(abs(uv.y),0,0.1)*100;
				return fixed4(0,uv.x * uv.y,0,1);
			}
			ENDCG
		}
	}
}
