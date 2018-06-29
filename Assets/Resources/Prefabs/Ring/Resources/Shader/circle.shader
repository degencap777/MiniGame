
Shader "SkillEffect/Circle" {
    Properties{
        _MainTex("Main texture", 2D) = "black" {}
        _Color("Color", Color) = (1,1,1,1)
    }
        Subshader{
        Tags{ "Queue" = "Transparent" }
        Pass{
            Cull Off 
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"


            sampler2D _MainTex;
            float4 _Color;

            struct v2f {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0; 
            };


            v2f vert(appdata_full a2v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(a2v.vertex);
                o.uv = a2v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : COLOR
            {
                float2 uv1 = i.uv + float2(_Time.x/8,0);
                uv1.y *= (1+(sin(i.uv + _SinTime.w * 2)+1)/2);
                uv1.y = clamp(uv1.y,0,1);
                float2 uv2 = i.uv + float2(-_Time.x/4 + 0.5,0);
                uv2.y *= (1+(sin(i.uv + _SinTime.w * 2 + 0.5)+1)/2);
                uv2.y = clamp(uv1.y,0,1);
                float alpha = tex2D(_MainTex,uv1).a;
                alpha += tex2D(_MainTex,uv2).a / 2;
                return fixed4(_Color.rgb, alpha * abs(sin(_Time.y + i.uv.x*6.28))*2 * _Color.a);
            }
            ENDCG
        }
    }
}