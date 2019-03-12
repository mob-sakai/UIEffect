Shader "UI/Hidden/UI-Effect-Shiny"
{
	Properties
	{
		[PerRendererData] _MainTex ("Main Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

		_ParamTex ("Parameter Texture", 2D) = "white" {}
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
			Name "Default"

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			
			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#define UI_SHINY 1
			#include "UI-Effect.cginc"
			#include "UI-Effect-Sprite.cginc"
			
//			struct appdata_t
//			{
//				float4 vertex   : POSITION;
//				float4 color	: COLOR;
//				float2 texcoord : TEXCOORD0;
//				UNITY_VERTEX_INPUT_INSTANCE_ID
//			};
//
//			struct v2f
//			{
//				float4 vertex   : SV_POSITION;
//				fixed4 color	: COLOR;
//				float2 texcoord  : TEXCOORD0;
//				float4 worldPosition : TEXCOORD1;
//				UNITY_VERTEX_OUTPUT_STEREO
//
//				half2 param : TEXCOORD2;
//			};
//			
//			fixed4 _Color;
//			fixed4 _TextureSampleAdd;
//			float4 _ClipRect;
//			sampler2D _MainTex;
//			float4 _MainTex_TexelSize;
//
//			v2f vert(appdata_t IN)
//			{
//				v2f OUT;
//				UNITY_SETUP_INSTANCE_ID(IN);
//				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
//				OUT.worldPosition = IN.vertex;
//
//				OUT.vertex = UnityObjectToClipPos(IN.vertex);
//
//				OUT.color = IN.color * _Color;
//
//				OUT.texcoord = UnpackToVec2(IN.texcoord.x);
//				OUT.param = UnpackToVec2(IN.texcoord.y);
//				return OUT;
//			}

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
				color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

				color = ApplyShinyEffect(color, IN.eParam);
				
				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif
				
				return color;
			}
		ENDCG
		}
	}
}
