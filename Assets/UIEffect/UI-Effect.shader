Shader "UI/Hidden/UI-Effect"
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

			#pragma shader_feature __ UI_TONE_GRAYSCALE UI_TONE_SEPIA UI_TONE_NEGA UI_TONE_PIXEL UI_TONE_MONO UI_TONE_CUTOFF UI_TONE_HUE 
			#pragma shader_feature __ UI_COLOR_ADD UI_COLOR_SUB UI_COLOR_SET
			#pragma shader_feature __ UI_BLUR_FAST UI_BLUR_MEDIUM UI_BLUR_DETAIL

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			#include "UI-Effect.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID

				float2 uv1 : TEXCOORD1;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
				
				#if defined (UI_COLOR)
				fixed4 colorFactor : COLOR1;
				#endif

				#if defined (UI_TONE) || defined (UI_BLUR)
				half4 effectFactor : TEXCOORD2;
				#endif
				#if UI_TONE_HUE || UI_TONE_PIXEL
				half4 extraFactor : TEXCOORD3;
				#endif
			};
			
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;

				#if defined (UI_TONE) || defined (UI_BLUR)
				OUT.effectFactor = UnpackToVec4(IN.uv1.x);
				#endif

				#if UI_TONE_HUE
				OUT.extraFactor.x = cos(OUT.effectFactor.x*3.14159265359 * 2);
				OUT.extraFactor.y = sin(OUT.effectFactor.x*3.14159265359 * 2);
				#elif UI_TONE_PIXEL
				OUT.extraFactor.xy = max(2, (1-OUT.effectFactor.x*0.98) * _MainTex_TexelSize.zw);
				#endif
				
				#if defined (UI_COLOR)
				OUT.colorFactor = UnpackToVec4(IN.uv1.y);
				#endif
				
				return OUT;
			}


			fixed4 frag(v2f IN) : SV_Target
			{
				#if UI_TONE_PIXEL
				IN.texcoord = round(IN.texcoord * IN.extraFactor.xy) / IN.extraFactor.xy;
				#endif

				#if defined (UI_BLUR)
				half4 color = (Tex2DBlurring(_MainTex, IN.texcoord, IN.effectFactor.z * _MainTex_TexelSize.xy * 2) + _TextureSampleAdd) * IN.color;
				#else
				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
				#endif
				color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

				#ifdef UI_TONE_CUTOFF
				clip (color.a - 1 + IN.effectFactor.x * 1.001);
				#elif UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				#if UI_TONE_MONO
				color.rgb = IN.color.rgb;
				color.a = color.a * tex2D(_MainTex, IN.texcoord).a + IN.effectFactor.x * 2 - 1;
				#elif UI_TONE_HUE
				color.rgb = shift_hue(color.rgb, IN.extraFactor.x, IN.extraFactor.y);
				#elif defined (UI_TONE) & !UI_TONE_CUTOFF
				color = ApplyToneEffect(color, IN.effectFactor.x);
				#endif

				#if defined (UI_COLOR)
				color = ApplyColorEffect(color, IN.colorFactor) * IN.color;
				#endif

				return color;
			}
		ENDCG
		}
	}
}
