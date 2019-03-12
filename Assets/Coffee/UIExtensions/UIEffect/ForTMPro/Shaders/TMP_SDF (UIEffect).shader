Shader "TextMeshPro/Distance Field (UIEffect)" {

Properties {
	_FaceTex			("Face Texture", 2D) = "white" {}
	_FaceUVSpeedX		("Face UV Speed X", Range(-5, 5)) = 0.0
	_FaceUVSpeedY		("Face UV Speed Y", Range(-5, 5)) = 0.0
	_FaceColor			("Face Color", Color) = (1,1,1,1)
	_FaceDilate			("Face Dilate", Range(-1,1)) = 0

	_OutlineColor		("Outline Color", Color) = (0,0,0,1)
	_OutlineTex			("Outline Texture", 2D) = "white" {}
	_OutlineUVSpeedX	("Outline UV Speed X", Range(-5, 5)) = 0.0
	_OutlineUVSpeedY	("Outline UV Speed Y", Range(-5, 5)) = 0.0
	_OutlineWidth		("Outline Thickness", Range(0, 1)) = 0
	_OutlineSoftness	("Outline Softness", Range(-1,1)) = 0

	_Bevel				("Bevel", Range(0,1)) = 0.5
	_BevelOffset		("Bevel Offset", Range(-0.5,0.5)) = 0
	_BevelWidth			("Bevel Width", Range(-.5,0.5)) = 0
	_BevelClamp			("Bevel Clamp", Range(0,1)) = 0
	_BevelRoundness		("Bevel Roundness", Range(0,1)) = 0

	_LightAngle			("Light Angle", Range(0.0, 6.2831853)) = 3.1416
	_SpecularColor		("Specular", Color) = (1,1,1,1)
	_SpecularPower		("Specular", Range(0,4)) = 2.0
	_Reflectivity		("Reflectivity", Range(5.0,15.0)) = 10
	_Diffuse			("Diffuse", Range(0,1)) = 0.5
	_Ambient			("Ambient", Range(1,0)) = 0.5

	_BumpMap 			("Normal map", 2D) = "bump" {}
	_BumpOutline		("Bump Outline", Range(0,1)) = 0
	_BumpFace			("Bump Face", Range(0,1)) = 0

	_ReflectFaceColor	("Reflection Color", Color) = (0,0,0,1)
	_ReflectOutlineColor("Reflection Color", Color) = (0,0,0,1)
	_Cube 				("Reflection Cubemap", Cube) = "black" { /* TexGen CubeReflect */ }
	_EnvMatrixRotation	("Texture Rotation", vector) = (0, 0, 0, 0)
		

	_UnderlayColor		("Border Color", Color) = (0,0,0, 0.5)
	_UnderlayOffsetX	("Border OffsetX", Range(-1,1)) = 0
	_UnderlayOffsetY	("Border OffsetY", Range(-1,1)) = 0
	_UnderlayDilate		("Border Dilate", Range(-1,1)) = 0
	_UnderlaySoftness	("Border Softness", Range(0,1)) = 0

	_GlowColor			("Color", Color) = (0, 1, 0, 0.5)
	_GlowOffset			("Offset", Range(-1,1)) = 0
	_GlowInner			("Inner", Range(0,1)) = 0.05
	_GlowOuter			("Outer", Range(0,1)) = 0.05
	_GlowPower			("Falloff", Range(1, 0)) = 0.75

	_WeightNormal		("Weight Normal", float) = 0
	_WeightBold			("Weight Bold", float) = 0.5

	_ShaderFlags		("Flags", float) = 0
	_ScaleRatioA		("Scale RatioA", float) = 1
	_ScaleRatioB		("Scale RatioB", float) = 1
	_ScaleRatioC		("Scale RatioC", float) = 1

	_MainTex			("Font Atlas", 2D) = "white" {}
	_TextureWidth		("Texture Width", float) = 512
	_TextureHeight		("Texture Height", float) = 512
	_GradientScale		("Gradient Scale", float) = 5.0
	_ScaleX				("Scale X", float) = 1.0
	_ScaleY				("Scale Y", float) = 1.0
	_PerspectiveFilter	("Perspective Correction", Range(0, 1)) = 0.875

	_VertexOffsetX		("Vertex OffsetX", float) = 0
	_VertexOffsetY		("Vertex OffsetY", float) = 0
	
	_MaskCoord			("Mask Coordinates", vector) = (0, 0, 32767, 32767)
	_ClipRect			("Clip Rect", vector) = (-32767, -32767, 32767, 32767)
	_MaskSoftnessX		("Mask SoftnessX", float) = 0
	_MaskSoftnessY		("Mask SoftnessY", float) = 0

	_StencilComp		("Stencil Comparison", Float) = 8
	_Stencil			("Stencil ID", Float) = 0
	_StencilOp			("Stencil Operation", Float) = 0
	_StencilWriteMask	("Stencil Write Mask", Float) = 255
	_StencilReadMask	("Stencil Read Mask", Float) = 255

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
	Blend One OneMinusSrcAlpha
	ColorMask [_ColorMask]

	Pass {
		CGPROGRAM
		#pragma target 3.0
		#pragma vertex VertShader
		#pragma fragment frag
		#pragma shader_feature __ BEVEL_ON
		#pragma shader_feature __ UNDERLAY_ON UNDERLAY_INNER
		#pragma shader_feature __ GLOW_ON

		#pragma multi_compile __ UNITY_UI_CLIP_RECT
		#pragma multi_compile __ UNITY_UI_ALPHACLIP

		#pragma shader_feature __ GRAYSCALE SEPIA NEGA PIXEL 
		#pragma shader_feature __ ADD SUBTRACT FILL
		#pragma shader_feature __ FASTBLUR MEDIUMBLUR DETAILBLUR
		#pragma shader_feature __ EX
		
		float4 _MainTex_TexelSize;
		#include "UnityCG.cginc"
		#include "UnityUI.cginc"
		#include "Assets/TextMesh Pro/Resources/Shaders/TMPro_Properties.cginc"
		#include "Assets/TextMesh Pro/Resources/Shaders/TMPro.cginc"
		
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

Fallback "TextMeshPro/Mobile/Distance Field (UIEffect)"
CustomEditor "Coffee.UIEffect.Editors.TMP_SDFShaderGUI"
}
