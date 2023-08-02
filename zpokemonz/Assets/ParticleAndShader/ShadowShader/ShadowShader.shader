Shader "Custom/ShadowShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	    //变换矩阵，xy存阴影的旋转值， zw存阴影的偏移值
		_Vector("_Vector",Vector)=(-0.35,0,0,0)
		//控制阴影的透明度
		_ShadowAlpha("_ShadowAlpha",Float)=0.3

		//开启镜像
		//[Toggle] _IsMirr("_IsMirr",int)=0

    }
    SubShader
    {
		Tags{
			"RenderType"="Transparent"  //设置透明队列
			"Queue"="Transparent"
	    }
        Cull Off ZWrite Off ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha   //设置混合

		  Pass       //这个Pass渲染角色的阴影
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

			sampler2D _MainTex;
			float4 _Vector;
			float _ShadowAlpha;
			//int _IsMirr;

			v2f vert(appdata v)
			{
				v2f o;
				//v.vertex += float4(-0.2, 0.1, 0, 0);
				float4x4 transMatrix = {
					1,_Vector.x,0,_Vector.z,
					_Vector.y,1,0,_Vector.w,
					0,0,1,0,
					0,0,0,1,
				};

				v.vertex = mul(transMatrix, v.vertex);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				//if (_IsMirr) {
				//	o.uv.y = 1 - o.uv.y;
				//}
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

			if (col.a > 0.1) {
				col.rgba = float4(0, 0, 0, _ShadowAlpha);
			};
			return col;
		}
		ENDCG
	}


        Pass       //这个Pass渲染角色
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

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                //col.rgb = 1 - col.rgb;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}