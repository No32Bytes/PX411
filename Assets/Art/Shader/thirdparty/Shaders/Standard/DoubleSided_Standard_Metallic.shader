/*
 * ════════════════════════════════════════════════════════════════════════════════
 * URP Double-Sided Shaders - Standard Metallic
 * ════════════════════════════════════════════════════════════════════════════════
 * 
 * Version: 1.0.0
 * Author: Rishiraj
 * Website: https://my-portfolio-rishiraj.vercel.app
 * Repository: https://github.com/Rishiraj10/urp-doublesided-shaders
 * 
 * Description:
 * Enhanced URP Lit shader with full double-sided rendering support and advanced
 * material controls. Features metallic workflow with detail maps, parallax mapping,
 * and dithered transparent shadows.
 * 
 * Features:
 * - Full double-sided rendering with normal flipping
 * - Metallic PBR workflow
 * - Detail maps (Albedo, Normal, Metallic/Smoothness)
 * - Parallax/height mapping
 * - Transparent shadow casting with adjustable intensity
 * - Back-face material property multipliers
 * - Compatible with URP Forward/Forward+ rendering
 * 
 * Requirements:
 * - Unity 2022.3.62f3 or newer
 * - Universal Render Pipeline 14.0+
 * 
 * License: MIT
 * Copyright (c) 2024 Rishiraj
 * 
 * ════════════════════════════════════════════════════════════════════════════════
 */

Shader "DoubleSided/Standard/Metallic"
{
    Properties
    {
        // ─── Surface Type ───────────────────────────────────────────────────────────
        [HideInInspector] _SurfaceType("Surface Type", Float) = 0
        [HideInInspector] _BlendMode("Blend Mode", Float) = 0

        // ─── Base ───────────────────────────────────────────────────────────────────
        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        [MainColor]   _BaseColor("Color", Color) = (1,1,1,1)
        _Cutoff("Alpha Cutoff", Range(0,1)) = 0.5

        // ─── Metallic / Roughness ────────────────────────────────────────────────────
        _MetallicGlossMap("Metallic (R) Smoothness (A)", 2D) = "white" {}
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _GlossMapScale("Smoothness Scale", Range(0,1)) = 1.0

        // ─── Normal ──────────────────────────────────────────────────────────────────
        [Normal] _BumpMap("Normal Map", 2D) = "bump" {}
        _BumpScale("Normal Scale", Range(0,2)) = 1.0

        // ─── Occlusion ───────────────────────────────────────────────────────────────
        _OcclusionMap("Occlusion", 2D) = "white" {}
        _OcclusionStrength("Occlusion Strength", Range(0,1)) = 1.0

        // ─── Height / Parallax ───────────────────────────────────────────────────────
        [Toggle(_PARALLAXMAP)] _ParallaxEnabled("Parallax Mapping", Float) = 0
        _ParallaxMap("Height Map", 2D) = "grey" {}
        _Parallax("Parallax Scale", Range(0.005, 0.08)) = 0.02

        // ─── Emission ────────────────────────────────────────────────────────────────
        [Toggle(_EMISSION)] _EmissionEnabled("Emission", Float) = 0
        [HDR] _EmissionColor("Emission Color", Color) = (0,0,0,1)
        _EmissionMap("Emission Map", 2D) = "white" {}

        // ═══ DETAIL MAPS ═══════════════════════════════════════════════════════════
        [Header(Detail Maps)]
        [Toggle(_DETAIL_MULX2)] _DetailEnabled("Enable Detail Maps", Float) = 0

        // ─── Detail UV ──────────────────────────────────────────────────────────────
        [Enum(UV0,0,UV1,1)] _UVSec("Detail UV Set", Float) = 0

        // ─── Detail Albedo ──────────────────────────────────────────────────────────
        _DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
        _DetailAlbedoStrength("Detail Albedo Strength", Range(0,2)) = 1.0

        // ─── Detail Normal ──────────────────────────────────────────────────────────
        [Normal] _DetailNormalMap("Detail Normal Map", 2D) = "bump" {}
        _DetailNormalMapScale("Detail Normal Scale", Range(0,2)) = 1.0

        // ─── Detail Metallic / Smoothness ────────────────────────────────────────────
        _DetailMetallicMap("Detail Metallic (R) Smoothness (A)", 2D) = "white" {}
        _DetailMetallicStrength("Detail Metallic Blend", Range(0,1)) = 0.0
        _DetailSmoothnessStrength("Detail Smoothness Blend", Range(0,1)) = 0.5

        // ─── Detail Mask ────────────────────────────────────────────────────────────
        _DetailMask("Detail Mask (A)", 2D) = "white" {}

        // ─── Detail Blend Mode ──────────────────────────────────────────────────────
        [Enum(Multiply x2,0,Overlay,1,Add,2)] _DetailBlendMode("Detail Blend Mode", Float) = 0
        _DetailBlendStrength("Detail Blend Strength", Range(0,1)) = 1.0

        // ═══ DOUBLE-SIDED ══════════════════════════════════════════════════════════
        [Header(Double Sided)]
        [Enum(UnityEngine.Rendering.CullMode)] _CullMode("Cull Mode", Float) = 2
        _BackFaceNormalFlip("Flip Back Face Normals", Range(0,1)) = 1.0
        _BackFaceColor("Back Face Tint", Color) = (1,1,1,1)
        _BackFaceMetallicMult("Back Face Metallic Multiplier", Range(0,1)) = 1.0
        _BackFaceSmoothnessMult("Back Face Smoothness Multiplier", Range(0,1)) = 1.0

        // ═══ TRANSPARENT SHADOW ════════════════════════════════════════════════════
        [Header(Transparent Shadows)]
        _ShadowIntensity("Shadow Intensity", Range(0,1)) = 0.8
        _ShadowDitherScale("Shadow Dither Softness", Range(0,4)) = 1.0

        // ─── Hidden ─────────────────────────────────────────────────────────────────
        [HideInInspector] _ZWrite("ZWrite", Float) = 1
        [HideInInspector] _SrcBlend("SrcBlend", Float) = 1
        [HideInInspector] _DstBlend("DstBlend", Float) = 0
        [HideInInspector] _Surface("Surface", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType"            = "Opaque"
            "RenderPipeline"        = "UniversalPipeline"
            "UniversalMaterialType" = "Lit"
            "IgnoreProjector"       = "True"
        }
        LOD 300

        // ════════════════════════════════════════════════════════════════════════════
        // PASS 1 — Forward Lit (Standard Metallic + Detail Maps)
        // ════════════════════════════════════════════════════════════════════════════
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            Cull [_CullMode]

            HLSLPROGRAM
            #pragma target 3.0

            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local_fragment _OCCLUSIONMAP
            #pragma shader_feature_local_fragment _NORMALMAP
            #pragma shader_feature_local_fragment _PARALLAXMAP
            #pragma shader_feature_local_fragment _DETAIL_MULX2

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _LIGHT_LAYERS
            #pragma multi_compile _ _FORWARD_PLUS
            #pragma multi_compile _ EVALUATE_SH_MIXED EVALUATE_SH_VERTEX
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma vertex   DS_StdMetallicVert
            #pragma fragment DS_StdMetallicFrag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // ── Texture Declarations ────────────────────────────────────────────────
            TEXTURE2D(_BaseMap);            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_BumpMap);            SAMPLER(sampler_BumpMap);
            TEXTURE2D(_EmissionMap);        SAMPLER(sampler_EmissionMap);

            // ── CBUFFER ─────────────────────────────────────────────────────────────
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4  _BaseColor;
                half   _Cutoff;
                half   _Metallic;
                half   _Glossiness;
                half   _GlossMapScale;
                half   _BumpScale;
                half   _OcclusionStrength;
                half   _Parallax;
                half4  _EmissionColor;
                // Detail
                float4 _DetailAlbedoMap_ST;
                half   _DetailAlbedoStrength;
                half   _DetailNormalMapScale;
                half   _DetailMetallicStrength;
                half   _DetailSmoothnessStrength;
                half   _DetailBlendMode;
                half   _DetailBlendStrength;
                half   _UVSec;
                // Double-Sided
                half   _BackFaceNormalFlip;
                half4  _BackFaceColor;
                half   _BackFaceMetallicMult;
                half   _BackFaceSmoothnessMult;
                // Shadow
                half   _ShadowIntensity;
                half   _ShadowDitherScale;
            CBUFFER_END

            TEXTURE2D(_MetallicGlossMap);   SAMPLER(sampler_MetallicGlossMap);
            TEXTURE2D(_OcclusionMap);       SAMPLER(sampler_OcclusionMap);
            TEXTURE2D(_ParallaxMap);        SAMPLER(sampler_ParallaxMap);
            TEXTURE2D(_DetailAlbedoMap);    SAMPLER(sampler_DetailAlbedoMap);
            TEXTURE2D(_DetailNormalMap);    SAMPLER(sampler_DetailNormalMap);
            TEXTURE2D(_DetailMetallicMap);  SAMPLER(sampler_DetailMetallicMap);
            TEXTURE2D(_DetailMask);         SAMPLER(sampler_DetailMask);

            // ── Helper: Detail Blend ─────────────────────────────────────────────────
            half3 ApplyDetailAlbedo(half3 base, half3 detail, half mask, half blendMode, half strength)
            {
                half3 result;
                if (blendMode < 0.5)        // Multiply x2
                    result = base * LerpWhiteTo(detail * 2.0h, mask);
                else if (blendMode < 1.5)   // Overlay
                    result = lerp(
                        2.0h * base * detail,
                        1.0h - 2.0h * (1.0h - base) * (1.0h - detail),
                        step(0.5h, base));
                else                        // Add
                    result = saturate(base + detail * mask);
                return lerp(base, result, strength * mask);
            }

            // ── Structs ─────────────────────────────────────────────────────────────
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 texcoord     : TEXCOORD0;
                float2 texcoord1    : TEXCOORD1;
                float2 texcoord2    : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float2 uvDetail     : TEXCOORD1;
                DECLARE_LIGHTMAP_OR_SH(staticLightmapUV, vertexSH, 2);
                float3 positionWS   : TEXCOORD3;
                half3  normalWS     : TEXCOORD4;
                half4  tangentWS    : TEXCOORD5;
                half3  viewDirWS    : TEXCOORD6;
                half4  fogFactorAndVertexLight : TEXCOORD7;
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                float4 shadowCoord  : TEXCOORD8;
            #endif
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // ── Vertex ──────────────────────────────────────────────────────────────
            Varyings DS_StdMetallicVert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs   nrmInputs = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                output.positionCS   = posInputs.positionCS;
                output.positionWS   = posInputs.positionWS;
                output.uv           = TRANSFORM_TEX(input.texcoord, _BaseMap);

                // Detail UV selection
                float2 rawDetailUV  = (_UVSec < 0.5) ? input.texcoord : input.texcoord1;
                output.uvDetail     = TRANSFORM_TEX(rawDetailUV, _DetailAlbedoMap);

                output.normalWS     = nrmInputs.normalWS;
                real sign           = input.tangentOS.w * GetOddNegativeScale();
                output.tangentWS    = half4(nrmInputs.tangentWS, sign);
                output.viewDirWS    = GetWorldSpaceNormalizeViewDir(posInputs.positionWS);

                output.fogFactorAndVertexLight = half4(
                    ComputeFogFactor(posInputs.positionCS.z),
                    VertexLighting(posInputs.positionWS, nrmInputs.normalWS));

                OUTPUT_LIGHTMAP_UV(input.texcoord1, unity_LightmapST, output.staticLightmapUV);
                OUTPUT_SH(output.normalWS.xyz, output.vertexSH);

            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                output.shadowCoord = GetShadowCoord(posInputs);
            #endif
                return output;
            }

            // ── Fragment ────────────────────────────────────────────────────────────
            half4 DS_StdMetallicFrag(Varyings input, FRONT_FACE_TYPE frontFace : FRONT_FACE_SEMANTIC) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                bool isFrontFace = IS_FRONT_VFACE(frontFace, true, false);

                // ── View direction for parallax ──────────────────────────────────────
                half3 viewDirTS = half3(0,0,1);
            #ifdef _PARALLAXMAP
                half3 bitangentWS0 = input.tangentWS.w * cross(input.normalWS, input.tangentWS.xyz);
                half3x3 TBN0 = half3x3(input.tangentWS.xyz, bitangentWS0, input.normalWS);
                viewDirTS = SafeNormalize(TransformWorldToTangent(input.viewDirWS, TBN0));
                float heightSample = SAMPLE_TEXTURE2D(_ParallaxMap, sampler_ParallaxMap, input.uv).g;
                input.uv += ParallaxOffset1Step(heightSample, _Parallax, viewDirTS);
            #endif

                // ── Base Albedo / Alpha ──────────────────────────────────────────────
                half4 albedoAlpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                if (!isFrontFace)
                    albedoAlpha *= _BackFaceColor;

            #ifdef _ALPHATEST_ON
                clip(albedoAlpha.a - _Cutoff);
            #endif

                // ── Metallic / Smoothness ────────────────────────────────────────────
                half metallic   = _Metallic;
                half smoothness = _Glossiness;
            #ifdef _METALLICSPECGLOSSMAP
                half4 metGloss = SAMPLE_TEXTURE2D(_MetallicGlossMap, sampler_MetallicGlossMap, input.uv);
                metallic   = metGloss.r;
                smoothness = metGloss.a * _GlossMapScale;
            #endif

                // ── Occlusion ────────────────────────────────────────────────────────
                half occlusion = 1.0h;
            #ifdef _OCCLUSIONMAP
                occlusion = LerpWhiteTo(SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, input.uv).g, _OcclusionStrength);
            #endif

                // ── Primary Normal ───────────────────────────────────────────────────
                half3 normalTS = half3(0,0,1);
            #ifdef _NORMALMAP
                half4 nrmSample = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, input.uv);
                normalTS = UnpackNormalScale(nrmSample, _BumpScale);
            #endif

                // ── Detail Maps ──────────────────────────────────────────────────────
            #ifdef _DETAIL_MULX2
                half detailMask = SAMPLE_TEXTURE2D(_DetailMask, sampler_DetailMask, input.uv).a;
                half4 detailAlbedoSample = SAMPLE_TEXTURE2D(_DetailAlbedoMap, sampler_DetailAlbedoMap, input.uvDetail);
                half4 detailMetSmoothSample = SAMPLE_TEXTURE2D(_DetailMetallicMap, sampler_DetailMetallicMap, input.uvDetail);
                half4 detailNrmSample = SAMPLE_TEXTURE2D(_DetailNormalMap, sampler_DetailNormalMap, input.uvDetail);

                // Blend albedo
                albedoAlpha.rgb = ApplyDetailAlbedo(albedoAlpha.rgb, detailAlbedoSample.rgb, detailMask, _DetailBlendMode, _DetailAlbedoStrength * _DetailBlendStrength);

                // Blend metallic / smoothness
                metallic   = lerp(metallic,   detailMetSmoothSample.r, _DetailMetallicStrength   * _DetailBlendStrength * detailMask);
                smoothness = lerp(smoothness, detailMetSmoothSample.a, _DetailSmoothnessStrength * _DetailBlendStrength * detailMask);

                // Blend normals
                half3 detailNormalTS = UnpackNormalScale(detailNrmSample, _DetailNormalMapScale);
                normalTS = lerp(normalTS, BlendNormalRNM(normalTS, detailNormalTS), _DetailBlendStrength * detailMask);
            #endif

                // ── Back face adjustments ────────────────────────────────────────────
                if (!isFrontFace)
                {
                    normalTS.z *= lerp(1.0h, -1.0h, _BackFaceNormalFlip);
                    metallic   *= _BackFaceMetallicMult;
                    smoothness *= _BackFaceSmoothnessMult;
                }

                // ── World-space Normal ───────────────────────────────────────────────
                half3 bitangentWS = input.tangentWS.w * cross(input.normalWS, input.tangentWS.xyz);
                half3 normalWS = NormalizeNormalPerPixel(
                    TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, bitangentWS, input.normalWS)));

                // ── Shadow Coord ─────────────────────────────────────────────────────
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                float4 shadowCoord = input.shadowCoord;
            #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
                float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
            #else
                float4 shadowCoord = float4(0,0,0,0);
            #endif

                // ── Fill InputData ───────────────────────────────────────────────────
                InputData inputData = (InputData)0;
                inputData.positionWS      = input.positionWS;
                inputData.normalWS        = normalWS;
                inputData.viewDirectionWS = SafeNormalize(input.viewDirWS);
                inputData.shadowCoord     = shadowCoord;
                inputData.fogCoord        = input.fogFactorAndVertexLight.x;
                inputData.vertexLighting  = input.fogFactorAndVertexLight.yzw;
                inputData.bakedGI         = SAMPLE_GI(input.staticLightmapUV, input.vertexSH, normalWS);
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
                inputData.shadowMask      = SAMPLE_SHADOWMASK(input.staticLightmapUV);

                // ── Emission ─────────────────────────────────────────────────────────
                half3 emission = half3(0,0,0);
            #ifdef _EMISSION
                emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, input.uv).rgb * _EmissionColor.rgb;
            #endif

                // ── Surface Data ─────────────────────────────────────────────────────
                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo             = albedoAlpha.rgb;
                surfaceData.metallic           = metallic;
                surfaceData.specular           = half3(0,0,0);
                surfaceData.smoothness         = smoothness;
                surfaceData.normalTS           = half3(0,0,1);
                surfaceData.emission           = emission;
                surfaceData.occlusion          = occlusion;
                surfaceData.alpha              = albedoAlpha.a;
                surfaceData.clearCoatMask      = 0;
                surfaceData.clearCoatSmoothness = 0;

                half4 color = UniversalFragmentPBR(inputData, surfaceData);
                color.rgb   = MixFog(color.rgb, inputData.fogCoord);
                color.a     = albedoAlpha.a;
                return color;
            }
            ENDHLSL
        }

        // ════════════════════════════════════════════════════════════════════════════
        // PASS 2 — Shadow Caster (with dithered transparent shadows)
        // ════════════════════════════════════════════════════════════════════════════
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull [_CullMode]

            HLSLPROGRAM
            #pragma target 2.0
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SURFACE_TYPE_TRANSPARENT
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
            #pragma vertex   DS_StdShadowVert
            #pragma fragment DS_StdShadowFrag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float3 _LightDirection;
            float3 _LightPosition;

            TEXTURE2D(_BaseMap);            SAMPLER(sampler_BaseMap);

            float3 DS_ApplyShadowBias(float3 positionWS, float3 normalWS, float3 lightDirection)
            {
                float invNdotL = 1.0 - saturate(dot(lightDirection, normalWS));
                float scale = invNdotL * 0.001;
                positionWS = lightDirection * scale + positionWS;
                return positionWS + normalWS * 0.002;
            }

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST; half4 _BaseColor; half _Cutoff;
                half _Metallic; half _Glossiness; half _GlossMapScale;
                half _BumpScale; half _OcclusionStrength; half _Parallax;
                half4 _EmissionColor;
                float4 _DetailAlbedoMap_ST;
                half _DetailAlbedoStrength; half _DetailNormalMapScale;
                half _DetailMetallicStrength; half _DetailSmoothnessStrength;
                half _DetailBlendMode; half _DetailBlendStrength; half _UVSec;
                half _BackFaceNormalFlip; half4 _BackFaceColor;
                half _BackFaceMetallicMult; half _BackFaceSmoothnessMult;
                half _ShadowIntensity; half _ShadowDitherScale;
            CBUFFER_END

            float Bayer4x4(float2 sp)
            {
                const float4x4 m = float4x4(0,8,2,10, 12,4,14,6, 3,11,1,9, 15,7,13,5) / 16.0;
                uint2 i = uint2(sp) % 4;
                return m[i.x][i.y];
            }

            struct ShadowA { float4 positionOS:POSITION; float3 normalOS:NORMAL; float2 texcoord:TEXCOORD0; UNITY_VERTEX_INPUT_INSTANCE_ID };
            struct ShadowV { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; UNITY_VERTEX_INPUT_INSTANCE_ID UNITY_VERTEX_OUTPUT_STEREO };

            ShadowV DS_StdShadowVert(ShadowA input)
            {
                ShadowV o; UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
                float3 pWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 nWS = TransformObjectToWorldNormal(input.normalOS);
            #if _CASTING_PUNCTUAL_LIGHT_SHADOW
                float3 lightDir = normalize(_LightPosition.xyz - pWS);
            #else
                float3 lightDir = _LightDirection;
            #endif
                float4 pCS = TransformWorldToHClip(DS_ApplyShadowBias(pWS, nWS, lightDir));
            #if UNITY_REVERSED_Z
                pCS.z = min(pCS.z, pCS.w * UNITY_NEAR_CLIP_VALUE);
            #else
                pCS.z = max(pCS.z, pCS.w * UNITY_NEAR_CLIP_VALUE);
            #endif
                o.positionCS = pCS; return o;
            }

            half4 DS_StdShadowFrag(ShadowV input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                half alpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv).a * _BaseColor.a;
            #ifdef _ALPHATEST_ON
                clip(alpha - _Cutoff);
            #endif
            #ifdef _SURFACE_TYPE_TRANSPARENT
                clip(Bayer4x4(input.positionCS.xy * _ShadowDitherScale) - (1.0 - alpha * _ShadowIntensity));
            #endif
                return 0;
            }
            ENDHLSL
        }

        Pass { Name "DepthOnly" Tags{"LightMode"="DepthOnly"} ZWrite On ColorMask R Cull [_CullMode]
            HLSLPROGRAM
            #pragma target 2.0
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma multi_compile_instancing
            #pragma vertex DepthVert
            #pragma fragment DepthFrag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            TEXTURE2D(_BaseMap);            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST; half4 _BaseColor; half _Cutoff;
                half _Metallic; half _Glossiness; half _GlossMapScale;
                half _BumpScale; half _OcclusionStrength; half _Parallax;
                half4 _EmissionColor; float4 _DetailAlbedoMap_ST;
                half _DetailAlbedoStrength; half _DetailNormalMapScale;
                half _DetailMetallicStrength; half _DetailSmoothnessStrength;
                half _DetailBlendMode; half _DetailBlendStrength; half _UVSec;
                half _BackFaceNormalFlip; half4 _BackFaceColor;
                half _BackFaceMetallicMult; half _BackFaceSmoothnessMult;
                half _ShadowIntensity; half _ShadowDitherScale;
            CBUFFER_END

            struct Attributes { float4 position : POSITION; float2 texcoord : TEXCOORD0; UNITY_VERTEX_INPUT_INSTANCE_ID };
            struct Varyings { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; UNITY_VERTEX_INPUT_INSTANCE_ID UNITY_VERTEX_OUTPUT_STEREO };

            Varyings DepthVert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
                output.positionCS = TransformObjectToHClip(input.position.xyz);
                return output;
            }

            half DepthFrag(Varyings input) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);
                #ifdef _ALPHATEST_ON
                half alpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv).a * _BaseColor.a;
                clip(alpha - _Cutoff);
                #endif
                return 0;
            }
            ENDHLSL }

        Pass { Name "DepthNormals" Tags{"LightMode"="DepthNormals"} ZWrite On Cull [_CullMode]
            HLSLPROGRAM
            #pragma target 2.0
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _NORMALMAP
            #pragma multi_compile_instancing
            #pragma vertex DepthNormalsVert
            #pragma fragment DepthNormalsFrag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            TEXTURE2D(_BaseMap);            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_BumpMap);            SAMPLER(sampler_BumpMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST; half4 _BaseColor; half _Cutoff;
                half _Metallic; half _Glossiness; half _GlossMapScale;
                half _BumpScale; half _OcclusionStrength; half _Parallax;
                half4 _EmissionColor; float4 _DetailAlbedoMap_ST;
                half _DetailAlbedoStrength; half _DetailNormalMapScale;
                half _DetailMetallicStrength; half _DetailSmoothnessStrength;
                half _DetailBlendMode; half _DetailBlendStrength; half _UVSec;
                half _BackFaceNormalFlip; half4 _BackFaceColor;
                half _BackFaceMetallicMult; half _BackFaceSmoothnessMult;
                half _ShadowIntensity; half _ShadowDitherScale;
            CBUFFER_END

            struct Attributes { float4 position : POSITION; float3 normal : NORMAL; float4 tangent : TANGENT; float2 texcoord : TEXCOORD0; UNITY_VERTEX_INPUT_INSTANCE_ID };
            struct Varyings { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; half3 normalWS : TEXCOORD1; half4 tangentWS : TEXCOORD2; UNITY_VERTEX_INPUT_INSTANCE_ID UNITY_VERTEX_OUTPUT_STEREO };

            Varyings DepthNormalsVert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
                output.positionCS = TransformObjectToHClip(input.position.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normal, input.tangent);
                output.normalWS = normalInput.normalWS;
                real sign = input.tangent.w * GetOddNegativeScale();
                output.tangentWS = half4(normalInput.tangentWS, sign);
                return output;
            }

            half4 DepthNormalsFrag(Varyings input) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);
                #ifdef _ALPHATEST_ON
                half alpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv).a * _BaseColor.a;
                clip(alpha - _Cutoff);
                #endif

                #ifdef _NORMALMAP
                half3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, input.uv), _BumpScale);
                half3 bitangentWS = input.tangentWS.w * cross(input.normalWS, input.tangentWS.xyz);
                half3 normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, bitangentWS, input.normalWS));
                #else
                half3 normalWS = input.normalWS;
                #endif

                return half4(NormalizeNormalPerPixel(normalWS), 0);
            }
            ENDHLSL }

        Pass { Name "Meta" Tags{"LightMode"="Meta"} Cull Off
            HLSLPROGRAM
            #pragma vertex UniversalVertexMeta
            #pragma fragment UniversalFragmentMetaLit
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

            TEXTURE2D(_MetallicGlossMap);   SAMPLER(sampler_MetallicGlossMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4  _BaseColor;
                half   _Cutoff;
                half   _Metallic;
                half   _Glossiness;
                half4  _EmissionColor;
            CBUFFER_END

            void InitializeStandardLitSurfaceData(float2 uv, out SurfaceData outSurfaceData)
            {
                outSurfaceData = (SurfaceData)0;
                half4 albedoAlpha = SampleAlbedoAlpha(uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)) * _BaseColor;
                outSurfaceData.albedo     = albedoAlpha.rgb;
                outSurfaceData.alpha      = albedoAlpha.a;
                outSurfaceData.metallic   = _Metallic;
                outSurfaceData.smoothness = _Glossiness;
                outSurfaceData.emission   = SampleEmission(uv, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));
                outSurfaceData.specular   = half3(0.04h, 0.04h, 0.04h);
                outSurfaceData.normalTS   = half3(0.0h, 0.0h, 1.0h);
                outSurfaceData.occlusion  = 1.0h;
                outSurfaceData.clearCoatMask = 0.0h;
                outSurfaceData.clearCoatSmoothness = 0.0h;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitMetaPass.hlsl"
            ENDHLSL }
    }

    CustomEditor "DoubleSidedShaderGUI"
    FallBack "Universal Render Pipeline/Lit"
}
