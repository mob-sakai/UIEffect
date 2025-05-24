// [OptionalShader] com.coffee.softmask-for-ugui: Hidden/TextMeshPro/Bitmap (UIEffect)
// [OptionalShader] com.coffee.ui-effect: Hidden/TextMeshPro/Bitmap (SoftMaskable)
Shader "Hidden/TextMeshPro/Bitmap (UIEffect)" {

Properties {
	_MainTex		    ("Font Atlas", 2D) = "white" {}
	_FaceTex		    ("Font Texture", 2D) = "white" {}
	_FaceColor	        ("Text Color", Color) = (1,1,1,1)

	_VertexOffsetX	    ("Vertex OffsetX", float) = 0
	_VertexOffsetY	    ("Vertex OffsetY", float) = 0
	_MaskSoftnessX	    ("Mask SoftnessX", float) = 0
	_MaskSoftnessY	    ("Mask SoftnessY", float) = 0

	_ClipRect           ("Clip Rect", vector) = (-32767, -32767, 32767, 32767)

	_StencilComp        ("Stencil Comparison", Float) = 8
	_Stencil            ("Stencil ID", Float) = 0
	_StencilOp          ("Stencil Operation", Float) = 0
	_StencilWriteMask   ("Stencil Write Mask", Float) = 255
	_StencilReadMask    ("Stencil Read Mask", Float) = 255

	_CullMode           ("Cull Mode", Float) = 0
	_ColorMask          ("Color Mask", Float) = 15
}

SubShader{

	Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

	Stencil
	{
		Ref[_Stencil]
		Comp[_StencilComp]
		Pass[_StencilOp]
		ReadMask[_StencilReadMask]
		WriteMask[_StencilWriteMask]
	}


	Lighting Off
	Cull [_CullMode]
	ZTest [unity_GUIZTestMode]
	ZWrite Off
	Fog { Mode Off }
	// ==== UIEFFECT START ====
	Blend [_SrcBlend] [_DstBlend]
	// ==== UIEFFECT END ====
	ColorMask[_ColorMask]

	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

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

		#include "UnityCG.cginc"
		#include "UnityUI.cginc"

		struct appdata_t
		{
			float4 vertex		: POSITION;
			fixed4 color		: COLOR;
			float4 texcoord0	: TEXCOORD0;
			// ==== UIEFFECT START ====
			float2	texcoord1		: TEXCOORD1;
			float4	texcoord2		: TEXCOORD2;
			// ==== UIEFFECT END ====
		};

		struct v2f
		{
			float4	vertex		: SV_POSITION;
			fixed4	color		: COLOR;
			float2	texcoord0	: TEXCOORD0;
			float2	texcoord1	: TEXCOORD1;
			float4	mask		: TEXCOORD2;
			// ==== UIEFFECT START ====
		    float4 uvMask			: TEXCOORD3;
		    float4 worldPosition	: TEXCOORD4;
			// ==== UIEFFECT END ====
		};

		uniform	sampler2D 	_MainTex;
		uniform	sampler2D 	_FaceTex;
		uniform float4		_FaceTex_ST;
		uniform	fixed4		_FaceColor;

		uniform float		_VertexOffsetX;
		uniform float		_VertexOffsetY;
		uniform float4		_ClipRect;
		uniform float		_MaskSoftnessX;
		uniform float		_MaskSoftnessY;
		uniform float		_UIMaskSoftnessX;
        uniform float		_UIMaskSoftnessY;
        uniform int _UIVertexColorAlwaysGammaSpace;

		v2f vert (appdata_t v)
		{
			float4 vert = v.vertex;
			vert.x += _VertexOffsetX;
			vert.y += _VertexOffsetY;

			vert.xy += (vert.w * 0.5) / _ScreenParams.xy;

			float4 vPosition = UnityPixelSnap(UnityObjectToClipPos(vert));

            if (_UIVertexColorAlwaysGammaSpace && !IsGammaSpace())
            {
                v.color.rgb = UIGammaToLinear(v.color.rgb);
            }
			fixed4 faceColor = v.color;
			faceColor *= _FaceColor;

			v2f OUT;
			OUT.vertex = vPosition;
			OUT.color = faceColor;
			OUT.texcoord0 = v.texcoord0;
			OUT.texcoord1 = TRANSFORM_TEX(v.texcoord1, _FaceTex);
			float2 pixelSize = vPosition.w;
			pixelSize /= abs(float2(_ScreenParams.x * UNITY_MATRIX_P[0][0], _ScreenParams.y * UNITY_MATRIX_P[1][1]));

			// Clamp _ClipRect to 16bit.
			const float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
			const half2 maskSoftness = half2(max(_UIMaskSoftnessX, _MaskSoftnessX), max(_UIMaskSoftnessY, _MaskSoftnessY));
			OUT.mask = float4(vert.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * maskSoftness + pixelSize.xy));

			// ==== UIEFFECT START ====
			OUT.uvMask = v.texcoord2;
			OUT.worldPosition = v.vertex;
			// ==== UIEFFECT END ====

			return OUT;
		}

		// ==== UIEFFECT START ====
		v2f _fragInput;
		fixed4 uieffect_frag(float2 uv)
		{
			v2f IN = _fragInput;
			float2 uvMove = uv - IN.texcoord0;
			fixed4 color = tex2D(_MainTex, IN.texcoord0 + uvMove);
			color = fixed4 (tex2D(_FaceTex, IN.texcoord1).rgb * IN.color.rgb, color.a);
			color.rgb *= color.a;
			return color;
		}
		#define UIEFFECT_TEXTMESHPRO 1
		#include "Packages/com.coffee.ui-effect/Shaders/UIEffect.cginc"
		// ==== UIEFFECT END ====
		
		fixed4 frag (v2f IN) : SV_Target
		{
			_fragInput = IN;
			half4 color = uieffect(IN.texcoord0, IN.uvMask, IN.worldPosition);
			color *= IN.color.a;

			// Alternative implementation to UnityGet2DClipping with support for softness.
			#if UNITY_UI_CLIP_RECT
				half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
				color *= m.x * m.y;
			#endif

			// ==== SOFTMASKABLE START ====
			#if SOFTMASKABLE
			color *= SoftMask(IN.vertex, IN.worldPosition, color.a);
			#endif
			// ==== SOFTMASKABLE END ====

			#if UNITY_UI_ALPHACLIP
				clip(color.a - 0.001);
			#endif

			return color;
		}
		ENDCG
	}
}

	CustomEditor "TMPro.EditorUtilities.TMP_BitmapShaderGUI"
}
