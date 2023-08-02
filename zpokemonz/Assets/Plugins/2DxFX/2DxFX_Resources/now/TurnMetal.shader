//////////////////////////////////////////////
/// 2DxFX v3 - by VETASOFT 2018 //
//////////////////////////////////////////////


//////////////////////////////////////////////

Shader "2DxFX_Extra_Shaders/TurnMetal"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_TurnMetal_Speed_1("_TurnMetal_Speed_1", Range(-8, 8)) = 1
_SpriteFade("SpriteFade", Range(0, 1)) = 1.0

// required for UI.Mask
[HideInInspector]_StencilComp("Stencil Comparison", Float) = 8
[HideInInspector]_Stencil("Stencil ID", Float) = 0
[HideInInspector]_StencilOp("Stencil Operation", Float) = 0
[HideInInspector]_StencilWriteMask("Stencil Write Mask", Float) = 255
[HideInInspector]_StencilReadMask("Stencil Read Mask", Float) = 255
[HideInInspector]_ColorMask("Color Mask", Float) = 15

}

SubShader
{

Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off

// required for UI.Mask
Stencil
{
Ref [_Stencil]
Comp [_StencilComp]
Pass [_StencilOp]
ReadMask [_StencilReadMask]
WriteMask [_StencilWriteMask]
}

Pass
{

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

struct appdata_t{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
};

struct v2f
{
float2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};

sampler2D _MainTex;
float _SpriteFade;
float _TurnMetal_Speed_1;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float4 ColorTurnMetal(float2 uv, sampler2D txt, float speed)
{
float4 txt1=tex2D(txt,uv);
float lum = dot(txt1.rgb, float3 (0.4126, 0.8152, 0.1722));
float3 metal = float3(lum,lum,lum);
metal.r = lum * pow(0.66*lum, 4.0);
metal.g = lum * pow(0.66*lum, 4.0);
float2 tuv = uv;
uv *= 2.5;
float time = (_Time/4)*speed;
float a = time * 50;
float n = sin(a + 2.0 * uv.x) + sin(a - 2.0 * uv.x) + sin(a + 2.0 * uv.y) + sin(a + 5.0 * uv.y);
n = fmod(((5.0 + n) / 5.0), 1.0);
n += tex2D(txt, tuv).r * 0.21 + tex2D(txt, tuv).g * 0.4 + tex2D(txt, tuv).b * 0.2;
n=fmod(n,1.0);
float tx = n * 6.0;
float r = clamp(tx - 2.0, 0.0, 1.0) + clamp(2.0 - tx, 0.0, 1.0);
float4 sortie=float4(1.0, 1.0, 1.0,r);
sortie.rgb=metal.rgb+(1-sortie.a);
sortie.rgb=0.05+sortie.rgb*0.5+dot(sortie.rgb, float3 (0.2126, 0.2152, 0.1722))*0.5;
sortie.a=txt1.a;
return sortie; 
}
float4 frag (v2f i) : COLOR
{
float4 _TurnMetal_1 = ColorTurnMetal(i.texcoord,_MainTex,_TurnMetal_Speed_1);
float4 FinalResult = _TurnMetal_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
