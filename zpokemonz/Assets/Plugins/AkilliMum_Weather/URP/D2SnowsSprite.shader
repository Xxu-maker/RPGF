
Shader "AkilliMum/SRP/D2WeatherEffects/URP/D2Snows"
{
    Properties
    {
        _MainTex("Diffuse", 2D) = "white" {}
        _MaskTex("Mask", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}

        // Legacy properties. They're here so that materials using this shader can gracefully fallback to the legacy sprite shader.
        [HideInInspector] _Color("Tint", Color) = (1,1,1,1)
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [HideInInspector] _AlphaTex("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha("Enable External Alpha", Float) = 0
		
        [HideInInspector]_TopFade("Top Fade", float) = 0.0
        [HideInInspector]_RightFade("Right Fade", float) = 0.0
        [HideInInspector]_BottomFade("Bottom Fade", float) = 0.0
        [HideInInspector]_LeftFade("Left Fade", float) = 0.0
        [HideInInspector]_FadeMultiplier("Fade Multiplier", float) = 0.1
        [HideInInspector]_CameraSpeedMultiplier("Camera Speed Multiplier", float) = 1.0
        [HideInInspector]_UVChangeX("UV Change X", float) = 1.0
        [HideInInspector]_UVChangeY("UV Change Y", float) = 1.0
        [HideInInspector]_Multiplier("Particle Multiplier", float) = 10
        //[HideInInspector]_Size("Size", float) = 0.1
        [HideInInspector]_Speed("Speed", float) = 4
        [HideInInspector]_Zoom("Zoom", float) = 1.2
        [HideInInspector]_Direction("Direction", float) = 0.2
        //[HideInInspector]_DarkMode("Dark Mode", float) = 0
        [HideInInspector]_Luminance("Luminance", float) = 1  
        //[HideInInspector]_LuminanceAdd("Luminance Add", float) = 0.001  
    }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    ENDHLSL

    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Tags { "LightMode" = "Universal2D" }
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma vertex CombinedShapeLightVertex
            #pragma fragment CombinedShapeLightFragment
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_0 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_1 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_2 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_3 __

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2  uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4  positionCS  : SV_POSITION;
                float4  color       : COLOR;
                float2	uv          : TEXCOORD0;
                float2	lightingUV  : TEXCOORD1;

				float4 screenPosition : TEXCOORD2;
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            half4 _MainTex_ST;
            half4 _NormalMap_ST;

			float4 _Color;

			float _TopFade;
            float _RightFade;
            float _BottomFade;
            float _LeftFade;
            float _FadeMultiplier;
            //float _Size;
            float _CameraSpeedMultiplier;
            float _UVChangeX;
            float _UVChangeY;
            float _Zoom;
            float _Speed;
            float _Direction;
            float _Multiplier;
            //float _DarkMode;
            float _Luminance;
            //float _LuminanceAdd;


            #if USE_SHAPE_LIGHT_TYPE_0
            SHAPE_LIGHT(0)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_1
            SHAPE_LIGHT(1)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_2
            SHAPE_LIGHT(2)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_3
            SHAPE_LIGHT(3)
            #endif

            Varyings CombinedShapeLightVertex(Attributes v)
            {
                Varyings o = (Varyings)0;

                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                float4 clipVertex = o.positionCS / o.positionCS.w;
                o.lightingUV = ComputeScreenPos(clipVertex).xy;
                o.color = v.color;


// 				o.texCoord0 = v.uv0;
// 				o.color = v.color;
// 				o.clipPos = vertexInput.positionCS;

				o.screenPosition = ComputeScreenPos(clipVertex);


                return o;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

			float mod(float a, float b)
            {
                return a - floor(a / b) * b;
            }
            float2 mod(float2 a, float2 b)
            {
                return a - floor(a / b) * b;
            }
            float3 mod(float3 a, float3 b)
            {
                return a - floor(a / b) * b;
            }
            float4 mod(float4 a, float4 b)
            {
                return a - floor(a / b) * b;
            } 

            float calcSnow(float2 uv)
            {
                const float3x3 p = float3x3(13.323122,23.5112,21.71123,21.1212,
                    28.7312,11.9312,21.8112,14.7212,61.3934);
            
                float snow = 0.;
                for (float i=0.; i < _Multiplier; i++)
                {
                    float2 q = uv * i*_Zoom;
                    float w = _Direction * mod(i*7.238917,1.0)-_Direction*0.1*sin(_Time.y+i);
                    q += float2(q.y*w, _Speed*_Time.y / (1.0+i*_Zoom*0.03));
                    float3 n = float3(floor(q),31.189+i);
                    float3 m = floor(n)*0.00001 + frac(n);
                    float3 mp = (31314.9+m) / frac(mul(p,m));
                    float3 r = frac(mp);
                    float2 s = abs(mod(q,1.0) -0.5 +0.9*r.xy -0.45);
                    s += 0.01*abs(2.0*frac(10.*q.yx)-1.); 
                    float d = 0.6*max(s.x-s.y,s.x+s.y)+max(s.x,s.y)-.01;
                    //snow += smoothstep(_Size,-_Size,d)*(r.x/(1.+.02*i*_Zoom));
                    snow += smoothstep(0.1,-0.1,d)*(r.x/(1.+.02*i*_Zoom));
                }
                return snow;
            }

            half4 CombinedShapeLightFragment(Varyings i) : SV_Target
            {
                half4 main = i.color;// * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);

                half4 Color = CombinedShapeLightShared(main, mask, i.lightingUV);




				float2 screenUV = i.screenPosition.xy / i.screenPosition.w;

                float2 fogUV = float2 (screenUV.x + _UVChangeX*_CameraSpeedMultiplier, screenUV.y + _UVChangeY*_CameraSpeedMultiplier);
                float m = calcSnow(fogUV) * _Color.a;

                //float2 fogUV = float2 (i.uv.x + _UVChangeX*_CameraSpeedMultiplier, i.uv.y + _UVChangeY*_CameraSpeedMultiplier);
                //float2 fogUV = float2 (screenUV.x + _UVChangeX*_CameraSpeedMultiplier, screenUV.y + _UVChangeY*_CameraSpeedMultiplier);
                //float m = calcSnow(fogUV);
            
                //float2 fogUV = float2 (IN.uv_MainTex.x + _UVChangeX*_CameraSpeedMultiplier, IN.uv_MainTex.y + _UVChangeY*_CameraSpeedMultiplier);
                //float2 fogUV = float2 (screenUV.x + _UVChangeX*_CameraSpeedMultiplier, screenUV.y + _UVChangeY*_CameraSpeedMultiplier);
                //float f = fog(fogUV);
                //float m = min(f*_Density, 1.);
                //float m = f*_Density;
            
                half top =    _TopFade    > 0 ?
                    (1 - i.uv.y) < _FadeMultiplier ? (1 - i.uv.y) / _FadeMultiplier : 1
                    : 1;
                half right =  _RightFade  > 0 ?
                    (1 - i.uv.x) < _FadeMultiplier ? (1 - i.uv.x) / _FadeMultiplier : 1
                    : 1;
                half bottom = _BottomFade > 0 ?
                    i.uv.y < _FadeMultiplier ? i.uv.y / _FadeMultiplier : 1
                    : 1;
                half left =   _LeftFade   > 0 ?    
                    i.uv.x < _FadeMultiplier ? i.uv.x / _FadeMultiplier : 1
                    : 1;
            
                //float3 Albedo = top * right * bottom * left * m * _Color.rgb; // * _Luminance;
                float3 Albedo = (1-m) * _Color.rgb * _Luminance;
                float Alpha =  top * right * bottom * left * m; // * _Luminance;
                Color = Color * float4(Albedo,Alpha);




				return Color;
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "NormalsRendering"}
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma vertex NormalsRenderingVertex
            #pragma fragment NormalsRenderingFragment

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color		: COLOR;
                float2 uv			: TEXCOORD0;
                float4 tangent      : TANGENT;
            };

            struct Varyings
            {
                float4  positionCS		: SV_POSITION;
                float4  color			: COLOR;
                float2	uv				: TEXCOORD0;
                float3  normalWS		: TEXCOORD1;
                float3  tangentWS		: TEXCOORD2;
                float3  bitangentWS		: TEXCOORD3;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            float4 _NormalMap_ST;  // Is this the right way to do this?

            Varyings NormalsRenderingVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;

                o.positionCS = TransformObjectToHClip(attributes.positionOS);
                o.uv = TRANSFORM_TEX(attributes.uv, _NormalMap);
                o.uv = attributes.uv;
                o.color = attributes.color;
                o.normalWS = TransformObjectToWorldDir(float3(0, 0, -1));
                o.tangentWS = TransformObjectToWorldDir(attributes.tangent.xyz);
                o.bitangentWS = cross(o.normalWS, o.tangentWS) * attributes.tangent.w;
                return o;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/NormalsRenderingShared.hlsl"

            float4 NormalsRenderingFragment(Varyings i) : SV_Target
            {
                float4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, i.uv));
                return NormalsRenderingShared(mainTex, normalTS, i.tangentWS.xyz, i.bitangentWS.xyz, i.normalWS.xyz);
            }
            ENDHLSL
        }
        Pass
        {
            Tags { "LightMode" = "UniversalForward" "Queue"="Transparent" "RenderType"="Transparent"}

            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
 			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
 			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color		: COLOR;
                float2 uv			: TEXCOORD0;
            };

            struct Varyings
            {
                float4  positionCS		: SV_POSITION;
                float4  color			: COLOR;
                float2	uv				: TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
			

            Varyings UnlitVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;

                o.positionCS = TransformObjectToHClip(attributes.positionOS);
                o.uv = TRANSFORM_TEX(attributes.uv, _MainTex);
                o.uv = attributes.uv;
                o.color = attributes.color;
                return o;
            }

			

            float4 UnlitFragment(Varyings i) : SV_Target
            {
                float4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);




                return mainTex;
            }
            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}
