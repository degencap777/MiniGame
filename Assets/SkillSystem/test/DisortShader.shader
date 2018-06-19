Shader "Custom/DisortShader" {
	Properties {
		
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Test ("Test" , float) = 1.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass{
			CGPROGRAM 
            

            #pragma vertex vert    
            #pragma fragment frag  
            #include "UnityCG.cginc"  


            sampler2D _MainTex;
            sampler2D _NoiceMap;
            float _Test;

            struct v2f{  
                float4 pos:POSITION;
                float2 uv : TEXCOORD0;
                float4 color : TEXCOORD1;
            };

            float GetH(float seed){
               return clamp(cos(seed),0,1);
            }

            v2f vert( appdata_full IN) {
                v2f o;
                float3 normal = IN.normal;
                
                float3 objectPos = IN.vertex;
                float3 objectNormalizedPos = normalize(objectPos);

                float seed[10];

                for (int i = 0; i < 10; i ++) {
                	float floati = (float)(i)/10;
                	float s=sin(i),c=cos(i);
                	float time = _Time.w;
                	float randomX = sin(time * floati + floati * 5);
					float randomY = sin(time * floati + floati * 5 + 10 * floati);
					float randomZ = sin(time * floati + floati * 5 + 20 * floati);

                	seed[i] = clamp(dot(normalize(float3(randomX,randomY,randomZ)),objectNormalizedPos),0,1);
                }

                 float h = 0;
                for (int i = 0; i < 10; i ++) {
                	h += 1/(11-pow(10,seed[i])) * abs(sin(_Time.z + i)) * (1-float(i)/10);
                }

                h *= _Test * (_SinTime.w + 1.3);

                IN.vertex += float4 (IN.normal,0) * h;


                o.pos = UnityObjectToClipPos(IN.vertex);
                o.uv = IN.texcoord;
                return o; 
            }

            fixed4 frag(v2f v) : COLOR {
            	return tex2D(_MainTex,v.uv);
            }
            ENDCG
		}
	}
	FallBack "Diffuse"
}
