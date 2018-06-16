Shader "VIsualManager/FogFilter" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_FogMap ("FogMap", 2D) = "Black" {}
		_Explored ("Explored", Range(0,1)) = 0.3
	}
	SubShader {
		Tags { 
			"Queue"="Overlay-1"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		LOD 200
		Cull off
		ZWrite Off

		GrabPass{}

		pass {
			ZWrite off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#include "UnityCG.cginc"
			#pragma fragment frag
			#pragma vertex vert

			fixed4 _Color;
			float _Explored;
			sampler2D _FogMap;

			sampler2D  _GrabTexture;
			float4 _GrabTexture_ST;
			sampler2D _CameraDepthTexture;

			matrix _MainCamera_PVM;
			matrix _OcclusionCamera_MVP;

			struct v2f {
				float4 pos : POSITION;
				float2 uv : TEXCOORD0; 
			};

			v2f vert(appdata_full a2v) {
				v2f o;
				o.pos = a2v.vertex * 2;
				o.pos.y *= -1;
				o.pos.z = 1;
				o.pos.w = 1;
				o.uv = TRANSFORM_TEX(a2v.texcoord,_GrabTexture);
				return o;
			}

			float4 getWorldPos(float2 uv, float d) {
				float camPosz = _ProjectionParams.y + (_ProjectionParams.z - _ProjectionParams.y) * d;
				float height = 2 * camPosz / unity_CameraProjection._m11;
				float width = _ScreenParams.x / _ScreenParams.y * height;
				float camPosX = width * uv.x - width/2;
				float camPosY = height * uv.y - height / 2;
				float4 camPos = float4(camPosX,camPosY,camPosz,1.0);
				return mul(unity_CameraToWorld, camPos);
			}


			float4 frag(v2f i) : COLOR {
				float depth = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv)));
				if (depth > 0.999999) return fixed4(0,0,0,0);
				float3 pos = getWorldPos(i.uv,depth);

				float4 uvzw = mul(_OcclusionCamera_MVP,float4(pos,1));
				float2 uv = uvzw.xy;
				uv += (float2(1,1));
				uv /= 2;

				float2 a = tex2D(_FogMap, uv).ba * 2;
				a += tex2D(_FogMap, uv + float2(0,0.0005)).ba * 0.7;
				a += tex2D(_FogMap, uv + float2(0.0005,0)).ba * 0.7;
				a += tex2D(_FogMap, uv + float2(0,-0.0005)).ba * 0.7;
				a += tex2D(_FogMap, uv + float2(-0.0005,0)).ba * 0.7;
				a/=5;
				return fixed4(_Color.xyz,1-a.x - a.y * _Explored);
			}



			ENDCG
		}
	}
	FallBack "Diffuse"
}
