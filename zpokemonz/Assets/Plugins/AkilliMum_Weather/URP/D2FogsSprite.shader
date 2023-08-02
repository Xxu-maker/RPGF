
Shader "AkilliMum/SRP/D2WeatherEffects/URP/D2Fogs"
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
		
        [HideInInspector]_NoiseTex("Noise", 2D) = "white" {}
        [HideInInspector]_UseNoiseTex("Use Noise Tex", float) = 0
        [HideInInspector]_Size("Size", float) = 2.0
        [HideInInspector]_TopFade("Top Fade", float) = 0.0
        [HideInInspector]_RightFade("Right Fade", float) = 0.0
        [HideInInspector]_BottomFade("Bottom Fade", float) = 0.0
        [HideInInspector]_LeftFade("Left Fade", float) = 0.0
        [HideInInspector]_FadeMultiplier("Fade Multiplier", float) = 0.1
        [HideInInspector]_CameraSpeedMultiplier("Camera Speed Multiplier", float) = 1.0
        [HideInInspector]_UVChangeX("UV Change X", float) = 1.0
        [HideInInspector]_UVChangeY("UV Change Y", float) = 1.0
		[HideInInspector]_Speed("Horizontal Speed", float) = 0.2
		[HideInInspector]_VSpeed("Vertical Speed", float) = 0
        [HideInInspector]_Density("Density", float) = 1
        [HideInInspector]_DarkMode("Dark Mode", float) = 0
        [HideInInspector]_DarkMultiplier("Dark Multiplier", float) = 1

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
            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            half4 _MainTex_ST;
            half4 _NoiseTex_ST;
            half4 _NormalMap_ST;

			float4 _Color;

			float _UseNoiseTex;
			float _Size;
            float _TopFade;
            float _RightFade;
            float _BottomFade;
            float _LeftFade;
            float _FadeMultiplier;
            float _CameraSpeedMultiplier;
            float _UVChangeX;
            float _UVChangeY;
            float _Speed;
            float _VSpeed;
            float _Density;
            float _DarkMode;
            float _DarkMultiplier;


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

			float hash(float n) 
            { 
                return frac(sin(n)*753.5453123); 
            }

            float noise(in float3 x)
            {
                float3 p = floor(x);
                float3 f = frac(x);
                f = f*f*(3.0 - 2.0*f);

                float n = p.x + p.y*157.0 + 113.0*p.z;
                return lerp(
                            lerp(
                                lerp(hash(n + 0.0),   hash(n + 1.0),   f.x),
                                lerp(hash(n + 157.0), hash(n + 158.0), f.x), 
                                f.y),
                            lerp(
                                lerp(hash(n + 113.0), hash(n + 114.0), f.x),
                                lerp(hash(n + 270.0), hash(n + 271.0), f.x), 
                                f.y),
                            f.z);
            }

            float texNoise(float2 uv) {
                return SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, uv).r;
            }

            float fog(in float2 uv)
            {
                float direction = _Time.y * _Speed;
                float Vdirection = _Time.y * _VSpeed;
                float color = 0.0;
                float total = 0.0;
                float k = 0.0;

                for (float i=0; i<6; i++)
                {
                    k = pow(2.0, i);
                    if (_UseNoiseTex > 0)
                    {
                        color += texNoise(
                                    float2(
                                            (uv.x * _Size + direction * (i + 1.0)*0.2) * k,
                                            (uv.y * _Size + Vdirection * (i + 1.0)*0.2) * k
                                        )
                                 ) / k;
                    }
                    else
                    {
                        color += noise(
                                    float3(
                                            (uv.x * _Size + direction * (i+1.0)*0.2) * k, 
                                            (uv.y * _Size + Vdirection * (i + 1.0)*0.2) * k,
                                            0.0
                                        )
                                    ) / k;
                    }
                    total += 1.0/k;
                }
                color /= total;
            
                return clamp(color, 0.0, 1.0);

            }

            half4 CombinedShapeLightFragment(Varyings i) : SV_Target
            {
                half4 main = i.color; // * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);

                half4 Color = CombinedShapeLightShared(main, mask, i.lightingUV);



				float2 screenUV = i.screenPosition.xy / i.screenPosition.w;

                //float2 fogUV = float2 (IN.uv_MainTex.x + _UVChangeX*_CameraSpeedMultiplier, IN.uv_MainTex.y + _UVChangeY*_CameraSpeedMultiplier);
                float2 fogUV = float2 (screenUV.x + _UVChangeX*_CameraSpeedMultiplier, screenUV.y + _UVChangeY*_CameraSpeedMultiplier);
                float f = fog(fogUV);
                float m = min(f*_Density, 1.)*_Color.a;
                //float m = f*_Density*_Color.a;
            
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
            
                //float3 Albedo = top * right * bottom * left * m * _Color.rgb;
                float3 Albedo = m * _Color.rgb;
                float Alpha =  top * right * bottom * left * m;



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
