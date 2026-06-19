// [OptionalShader] com.coffee.softmask-for-ugui: Hidden/UniText/Mobile/SDF (UIEffect)
// [OptionalShader] com.coffee.ui-effect: Hidden/UniText/Mobile/SDF (SoftMaskable)
// Simplified SDF shader (single-pass):
// - No Shading Option (bevel / bump / env map)
// - No Glow Option
// - Renders Face, Outline, and Underlay in one pass

Shader "Hidden/UniText/Mobile/SDF (UIEffect)" {

Properties {
	_FaceColor          ("Face Color", Color) = (1,1,1,1)
	_FaceDilate			("Face Dilate", Range(-1,1)) = 0

	_OutlineColor	    ("Outline Color", Color) = (0,0,0,1)
	_OutlineDilate		("Outline Dilate", Range(-1,1)) = 0
	_OutlineSoftness	("Outline Softness", Range(0,1)) = 0

	_UnderlayColor	    ("Border Color", Color) = (0,0,0,.5)
	_UnderlayOffsetX 	("Border OffsetX", Range(-1,1)) = 0
	_UnderlayOffsetY 	("Border OffsetY", Range(-1,1)) = 0
	_UnderlayDilate		("Border Dilate", Range(-1,1)) = 0
	_UnderlaySoftness 	("Border Softness", Range(0,1)) = 0

	_WeightNormal		("Weight Normal", float) = 0
	_WeightBold			("Weight Bold", float) = 1

	_ShaderFlags		("Flags", float) = 0
	_ScaleRatioA		("Scale RatioA", float) = 1
	_ScaleRatioB		("Scale RatioB", float) = 1
	_ScaleRatioC		("Scale RatioC", float) = 1

	_MainTex			("Font Atlas", 2D) = "white" {}
	_ScaleX				("Scale X", float) = 1
	_ScaleY				("Scale Y", float) = 1
	_PerspectiveFilter	("Perspective Correction", Range(0, 1)) = 0.875
	_Sharpness			("Sharpness", Range(-1,1)) = 0

	_VertexOffsetX		("Vertex OffsetX", float) = 0
	_VertexOffsetY		("Vertex OffsetY", float) = 0

	_ClipRect			("Clip Rect", vector) = (-32767, -32767, 32767, 32767)
	_MaskSoftnessX		("Mask SoftnessX", float) = 0
	_MaskSoftnessY		("Mask SoftnessY", float) = 0

	_StencilComp		("Stencil Comparison", Float) = 8
	_Stencil			("Stencil ID", Float) = 0
	_StencilOp			("Stencil Operation", Float) = 0
	_StencilWriteMask	("Stencil Write Mask", Float) = 255
	_StencilReadMask	("Stencil Read Mask", Float) = 255

	_CullMode			("Cull Mode", Float) = 0
	_ColorMask			("Color Mask", Float) = 15
}

SubShader {
	Tags
	{
		"Queue"="Transparent"
		"IgnoreProjector"="True"
		"RenderType"="Transparent"
	}

	Stencil
	{
		Ref [_Stencil]
		Comp [_StencilComp]
		Pass [_StencilOp]
		ReadMask [_StencilReadMask]
		WriteMask [_StencilWriteMask]
	}

	Cull [_CullMode]
	ZWrite Off
	Lighting Off
	Fog { Mode Off }
	ZTest [unity_GUIZTestMode]
	// ==== UIEFFECT START ====
	Blend [_SrcBlend] [_DstBlend]
	// ==== UIEFFECT END ====
	ColorMask [_ColorMask]

	Pass {
		CGPROGRAM
		#pragma vertex VertShader
		#pragma fragment PixShader
		#pragma shader_feature __ OUTLINE_ON
		#pragma shader_feature __ UNDERLAY_ON UNDERLAY_INNER

		#pragma multi_compile __ UNITY_UI_CLIP_RECT
		#pragma multi_compile __ UNITY_UI_ALPHACLIP

        // ==== UIEFFECT START ====
        #pragma shader_feature_local_fragment _ TONE_GRAYSCALE TONE_SEPIA TONE_NEGATIVE TONE_RETRO TONE_POSTERIZE
        #pragma shader_feature_local_fragment _ COLOR_FILTER
        #pragma shader_feature_local_fragment _ SAMPLING_BLUR_FAST SAMPLING_BLUR_MEDIUM SAMPLING_BLUR_DETAIL SAMPLING_PIXELATION SAMPLING_RGB_SHIFT SAMPLING_EDGE_LUMINANCE SAMPLING_EDGE_ALPHA
        #pragma shader_feature_local_fragment _ TRANSITION_FADE TRANSITION_CUTOFF TRANSITION_DISSOLVE TRANSITION_SHINY TRANSITION_MASK TRANSITION_MELT TRANSITION_BURN TRANSITION_PATTERN TRANSITION_BLAZE
        #pragma shader_feature_local_fragment _ EDGE_PLAIN EDGE_SHINY
        #pragma shader_feature_local_fragment _ DETAIL_MASKING DETAIL_MULTIPLY DETAIL_ADDITIVE DETAIL_SUBTRACTIVE DETAIL_REPLACE DETAIL_MULTIPLY_ADDITIVE
        #pragma shader_feature_local_fragment _ TARGET_HUE TARGET_LUMINANCE
        #pragma shader_feature_local_fragment _ GRADATION_GRADIENT GRADATION_COLOR2 GRADATION_COLOR4
        #pragma shader_feature_fragment _ UIEFFECT_EDITOR
        // ==== UIEFFECT END ====

        // ==== SOFTMASKABLE START ====
        #pragma shader_feature_fragment _ SOFTMASK_EDITOR
        #pragma shader_feature_local_fragment _ SOFTMASKABLE
        #if SOFTMASKABLE
        #include "Packages/com.coffee.softmask-for-ugui/Shaders/SoftMask.cginc"
        #endif
        // ==== SOFTMASKABLE END ====

		#include "Packages/media.lightside.unitext/Shaders/UniText_SDF-Mobile-Common.cginc"

		// Unity auto-generated: (1/width, 1/height, width, height)
		float4 _MainTex_TexelSize;

		struct sdf_vertex_tV2
		{
		    UNITY_VERTEX_INPUT_INSTANCE_ID
		    float4 vertex    : POSITION;
		    float3 normal    : NORMAL;
		    fixed4 color     : COLOR;
		    float4 texcoord0 : TEXCOORD0;  // xy = UV, z = gradientScale, w = xScaleVal
		    float4 texcoord1 : TEXCOORD1;  // x = spreadRatio (Padding / PointSize)
			// // ==== UIEFFECT START ====
			float4	texcoord2		: TEXCOORD2;
			// // ==== UIEFFECT END ====
		};
		#define sdf_vertex_t sdf_vertex_tV2
		
		struct pixel_t
		{
			UNITY_VERTEX_INPUT_INSTANCE_ID
			UNITY_VERTEX_OUTPUT_STEREO
			float4 vertex       : SV_POSITION;
			fixed4 faceColor    : COLOR;
			fixed4 outlineColor : COLOR1;
			float2 uv           : TEXCOORD0;
			half4  param        : TEXCOORD1;  // scale, faceBias, outlineBias, alpha
			half4  mask         : TEXCOORD2;
			#if (UNDERLAY_ON | UNDERLAY_INNER)
			float2 underlayUV   : TEXCOORD3;
			half2  underlayParam: TEXCOORD4;  // scale, bias
			#endif
			// ==== UIEFFECT START ====
		    float4 uvMask			: TEXCOORD5;
		    float4 worldPosition	: TEXCOORD6;
			// ==== UIEFFECT END ====
		};

		pixel_t VertShader(sdf_vertex_t input)
		{
			pixel_t output;

			UNITY_INITIALIZE_OUTPUT(pixel_t, output);
			UNITY_SETUP_INSTANCE_ID(input);
			UNITY_TRANSFER_INSTANCE_ID(input, output);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

			float4 vert = ApplyVertexOffset(input.vertex);
			float4 vPosition = UnityObjectToClipPos(vert);

			float2 pixelSize = vPosition.w;
			pixelSize /= float2(_ScaleX, _ScaleY) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

			// Get scale (for AA) and normFactor (for effect normalization)
			float2 scaleAndNorm = ComputeScaleAndNorm(vPosition, input.normal, vert, input.texcoord0, input.texcoord1);
			float scale = scaleAndNorm.x;
			float normFactor = scaleAndNorm.y;

			float baseWeight = ComputeBaseWeight(input.texcoord0);

			// Face bias (effects normalized via normFactor for independence from atlas settings)
			float faceBias = ComputeBias(baseWeight, _FaceDilate, scale, normFactor);

			// Outline bias (same logic, different dilate)
			float outlineBias = ComputeBias(baseWeight, _FaceDilate + _OutlineDilate, scale, normFactor);

			fixed4 color = GammaToLinearIfNeeded(input.color);
			float opacity = color.a;
			#if (UNDERLAY_ON | UNDERLAY_INNER)
			opacity = 1.0;
			#endif

			fixed4 faceColor = fixed4(color.rgb, opacity) * _FaceColor;
			faceColor.rgb *= faceColor.a;

			fixed4 outlineColor = _OutlineColor;
			outlineColor.a *= opacity;
			outlineColor.rgb *= outlineColor.a;

			#if (UNDERLAY_ON | UNDERLAY_INNER)
			// Underlay parameters (effects normalized via normFactor)
			float softnessFactor = _UnderlaySoftness * _ScaleRatioC * normFactor;
			float layerScale = scale / (1 + softnessFactor);
			float underlayDilate = _FaceDilate * _ScaleRatioA + _UnderlayDilate * _ScaleRatioC;
			float layerBias = ComputeBias(baseWeight, underlayDilate, layerScale, normFactor);

			// Underlay UV offset (independent of atlas settings)
			// Uses PointSize-proportional factor instead of spreadRatio-dependent
			float gradientScaleVal = input.texcoord0.z;
			float offsetFactor = ComputeUnderlayOffsetFactor(gradientScaleVal, normFactor);
			float2 layerOffset = float2(
				-(_UnderlayOffsetX * _ScaleRatioC) * offsetFactor * _MainTex_TexelSize.x,
				-(_UnderlayOffsetY * _ScaleRatioC) * offsetFactor * _MainTex_TexelSize.y
			);

			output.underlayUV = input.texcoord0.xy + layerOffset;
			output.underlayParam = half2(layerScale, layerBias);
			#endif

			output.vertex = vPosition;
			output.faceColor = faceColor;
			output.outlineColor = outlineColor;
			output.uv = input.texcoord0.xy;
			output.param = half4(scale, faceBias, outlineBias, color.a);
			output.mask = ComputeMask(vert, pixelSize);

			
			// ==== UIEFFECT START ====
			output.uvMask = input.texcoord2;
			output.worldPosition = input.vertex;
			// ==== UIEFFECT END ====
			
			return output;
		}

		fixed4 uieffect_frag(pixel_t input, float2 uv)
		{
			float2 uvAdd = uv - input.uv;
			half d = tex2D(_MainTex, input.uv + uvAdd).a * input.param.x;
			half4 result = half4(0, 0, 0, 0);

			// Underlay layer (behind everything)
			#if UNDERLAY_ON
			half ud = tex2D(_MainTex, input.underlayUV + uvAdd).a * input.underlayParam.x;
			half4 underlayColor = float4(_UnderlayColor.rgb * _UnderlayColor.a, _UnderlayColor.a);
			result = SDFLayer(ud, input.underlayParam.y, underlayColor);
			#endif

			#if UNDERLAY_INNER
			half ud = tex2D(_MainTex, input.underlayUV + uvAdd).a * input.underlayParam.x;
			half4 underlayColor = float4(_UnderlayColor.rgb * _UnderlayColor.a, _UnderlayColor.a);
			half faceMask = saturate(d - input.param.y);
			result = underlayColor * (1 - saturate(ud - input.underlayParam.y)) * faceMask;
			#endif

			// Outline layer
			#ifdef OUTLINE_ON
			half4 outlineResult = SDFLayer(d, input.param.z, input.outlineColor);
			result = BlendOver(result, outlineResult);
			#endif

			// Face layer (on top)
			half4 faceResult = SDFLayer(d, input.param.y, input.faceColor);
			result = BlendOver(result, faceResult);

			// Apply vertex alpha for underlay
			#if (UNDERLAY_ON | UNDERLAY_INNER)
			result *= input.param.w;
			#endif

			return ApplyClipping(result, input.mask);
		}
		
		#define UIEFFECT_TEXTMESHPRO 1
		#define UIEFFECT_UNITEXT 1
		#define UIEFFECT_FRAG_STRUCT pixel_t
		#include "Packages/com.coffee.ui-effect/Shaders/UIEffect.cginc"
		fixed4 PixShader(pixel_t input) : SV_Target
		{
			UNITY_SETUP_INSTANCE_ID(input);
			half4 faceColor = uieffect(input.uv, input.uvMask, input.worldPosition, input);
			// faceColor *= input.faceColor.a;

		    #if UNITY_UI_CLIP_RECT
			half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(input.mask.xy)) * input.mask.zw);
			faceColor *= m.x * m.y;
		    #endif

		    // ==== SOFTMASKABLE START ====
		    #if SOFTMASKABLE
			faceColor *= SoftMask(input.position, input.worldPosition, faceColor.a);
		    #endif
		    // ==== SOFTMASKABLE END ====

		    #if UNITY_UI_ALPHACLIP
			clip(faceColor.a - 0.001);
		    #endif

			return faceColor;
		}
		ENDCG
	}
}

CustomEditor "LightSide.UniText_SDFShaderGUI"
}
