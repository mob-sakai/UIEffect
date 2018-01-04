Shader "UI/Hidden/UIEffectCapturedImage"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
	_Color("Tint", Color) = (1,1,1,1)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
	}

		SubShader
	{

		ZTest Always
		Cull Off
		ZWrite Off
		Fog{ Mode off }

		Pass
	{
		Name "Default"
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#if UNITY_VERSION >= 540
#pragma target 2.0
#endif

#include "UnityCG.cginc"
#include "UnityUI.cginc"

		// vvvv [For UIEffect] vvvv : Define keyword and include.
#pragma multi_compile __ UI_TONE_GRAYSCALE UI_TONE_SEPIA UI_TONE_NEGA UI_TONE_PIXEL UI_TONE_MONO UI_TONE_CUTOFF 
#pragma multi_compile __ UI_COLOR_ADD UI_COLOR_SUB UI_COLOR_SET
#pragma multi_compile __ UI_BLUR_FAST UI_BLUR_DETAIL UI_BLUR_DETAIL8
#include "UI-Effect.cginc"
		// ^^^^ [For UIEffect] ^^^^


	v2f_img vert(appdata_img v)
	{
		v2f_img o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

#if UNITY_UV_STARTS_AT_TOP
		o.uv = half2(v.texcoord.x, 1 - v.texcoord.y);
#else
		o.uv = v.texcoord;
#endif

		return o;
	}

	sampler2D _MainTex;
	fixed4 _EffectFactor;
	fixed4 _ColorFactor;

	fixed4 frag(v2f_img IN) : SV_Target
	{

#if UI_TONE_PIXEL
		float pixelRate = max(1,(1 - _EffectFactor.x) * 256);
	IN.uv = round(IN.uv * pixelRate) / pixelRate;
#endif

#if defined (UI_BLUR)
	half4 color = Tex2DBlurring(_MainTex, IN.uv, _EffectFactor.y);
#else
	half4 color = tex2D(_MainTex, IN.uv);
#endif

#ifdef UI_TONE_CUTOFF
	clip(color.a - _EffectFactor.x * 1.001);
#elif UNITY_UI_ALPHACLIP
	clip(color.a - 0.001);
#endif

#if defined (UI_TONE)
#if UI_TONE_MONO
	color.a = color.a * tex2D(_MainTex, IN.uv).a + _EffectFactor.x * 2 - 1;
#elif !UI_TONE_CUTOFF
	color = ApplyToneEffect(color, _EffectFactor.x);	// Tone effect.
#endif
#endif

#if defined (UI_COLOR)
	color = ApplyColorEffect(color, _ColorFactor);	// Color effect.
#endif
	color.a = 1;
	return color;
	}
		ENDCG
	}
	}
}
