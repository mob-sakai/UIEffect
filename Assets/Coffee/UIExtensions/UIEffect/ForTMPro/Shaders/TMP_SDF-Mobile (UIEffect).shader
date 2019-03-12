// Simplified SDF shader:
// - No Shading Option (bevel / bump / env map)
// - No Glow Option
// - Softness is applied on both side of the outline

Shader "TextMeshPro/Mobile/Distance Field (UIEffect)" {

Properties {
	_FaceColor			("Face Color", Color) = (1,1,1,1)
	_FaceDilate			("Face Dilate", Range(-1,1)) = 0

	_OutlineColor		("Outline Color", Color) = (0,0,0,1)
	_OutlineWidth		("Outline Thickness", Range(0,1)) = 0
	_OutlineSoftness	("Outline Softness", Range(0,1)) = 0

	_UnderlayColor		("Border Color", Color) = (0,0,0,.5)
	_UnderlayOffsetX 	("Border OffsetX", Range(-1,1)) = 0
	_UnderlayOffsetY 	("Border OffsetY", Range(-1,1)) = 0
	_UnderlayDilate		("Border Dilate", Range(-1,1)) = 0
	_UnderlaySoftness 	("Border Softness", Range(0,1)) = 0

	_WeightNormal		("Weight Normal", float) = 0
	_WeightBold			("Weight Bold", float) = .5

	_ShaderFlags		("Flags", float) = 0
	_ScaleRatioA		("Scale RatioA", float) = 1
	_ScaleRatioB		("Scale RatioB", float) = 1
	_ScaleRatioC		("Scale RatioC", float) = 1

	_MainTex			("Font Atlas", 2D) = "white" {}
	_TextureWidth		("Texture Width", float) = 512
	_TextureHeight		("Texture Height", float) = 512
	_GradientScale		("Gradient Scale", float) = 5
	_ScaleX				("Scale X", float) = 1
	_ScaleY				("Scale Y", float) = 1
	_PerspectiveFilter	("Perspective Correction", Range(0, 1)) = 0.875

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
	
	_ColorMask			("Color Mask", Float) = 15
	
	_NoiseTex("Noise Texture (A)", 2D) = "white" {}
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
	Blend One OneMinusSrcAlpha
	ColorMask [_ColorMask]

	Pass {
		CGPROGRAM
		#pragma vertex VertShader
		#pragma fragment frag
		#pragma shader_feature __ OUTLINE_ON
		#pragma shader_feature __ UNDERLAY_ON UNDERLAY_INNER

		#pragma multi_compile __ UNITY_UI_CLIP_RECT
		#pragma multi_compile __ UNITY_UI_ALPHACLIP

		#pragma shader_feature __ GRAYSCALE SEPIA NEGA PIXEL 
		#pragma shader_feature __ ADD SUBTRACT FILL
		#pragma shader_feature __ FASTBLUR MEDIUMBLUR DETAILBLUR
		#pragma shader_feature __ EX
		
		#include "UnityCG.cginc"
		#include "UnityUI.cginc"
		#include "Assets/TextMesh Pro/Resources/Shaders/TMPro_Properties.cginc"
		
		#define MOBILE 1
		#define UI_EFFECT 1
		#include "Assets/Coffee/UIExtensions/UIEffect/Shaders/UI-Effect.cginc"
		#include "UI-Effect-TMPro.cginc"

		fixed4 frag(pixel_t IN) : SV_Target
		{
			fixed4 param = tex2D(_ParamTex, float2(0.5, IN.eParam));
		    fixed effectFactor = param.x;
		    fixed colorFactor = param.y;
		    fixed blurFactor = param.z;

			#if PIXEL
			half2 pixelSize = max(2, (1-effectFactor*0.95) * float2(_TextureWidth, _TextureHeight));
			UV(IN).xy = round(UV(IN).xy * pixelSize) / pixelSize;
			#endif

			#if defined(UI_BLUR) && EX
			half4 color = Tex2DBlurring(IN, blurFactor * float2(1/_TextureWidth, 1/_TextureHeight) * 4, IN.uvMask);
			#elif defined(UI_BLUR)
			half4 color = Tex2DBlurring(IN, blurFactor * float2(1/_TextureWidth, 1/_TextureHeight) * 4);
			#else
			half4 color = PixShader(IN) * IN.color.a;
			#endif
			
			#if defined (UI_TONE)
			color = ApplyToneEffect(color, effectFactor);
			#endif

			color = ApplyColorEffect(color, fixed4(IN.color.rgb, colorFactor));
			color.rgb *= color.a;
			
			return color * IN.color.a;
		}
		ENDCG
	}
}

CustomEditor "Coffee.UIEffect.Editors.TMP_SDFShaderGUI"
}
