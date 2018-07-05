// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'

// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'

Shader "SkillEffect/inAlpha1" {
    Properties{
        _MainTex("Main texture", 2D) = "black" {}
        _Color("Color", Color) = (1,1,1,1)
    }
        Subshader{
        Tags{ "Queue" = "Transparent" }
        Pass{
        	offset -10,-10
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"


            sampler2D _MainTex;
            float4 _Color;
            uniform float4x4 unity_Projector;

            struct v2f {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0; 
            };


            v2f vert(appdata_full a2v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(a2v.vertex);
                o.uv = mul(unity_Projector,a2v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : COLOR
            {
                float alpha = tex2D(_MainTex,i.uv).a;
                //alpha = clamp(alpha-0.95,0,1)*10;
                if (length(i.uv*2-float2(1,1))>0.97) alpha = 0;
                //else if (length(i.uv*2-float2(1,1)) > 0.99) alpha = 1;
                return fixed4(_Color.rgb, alpha * _Color.a);
            }
            ENDCG
        }
    }
}