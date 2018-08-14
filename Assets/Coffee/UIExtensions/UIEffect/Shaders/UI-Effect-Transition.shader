Shader "UI/Hidden/UI-Effect-Transition"
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

		[Header(Transition)]
		_TransitionTexture("Transition Texture (A)", 2D) = "white" {}
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
			#pragma shader_feature __ FADE CUTOFF

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			#include "UI-Effect.cginc"
			
			struct appdata_t
			{
				float4 vertex	: POSITION;
				float4 color	: COLOR;
				float2 texcoord	: TEXCOORD0;
				half2 factor	: TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex	: SV_POSITION;
				fixed4 color	: COLOR;
				float2 texcoord	: TEXCOORD0;
				float4 wpos		: TEXCOORD1;
				half3 factor	: TEXCOORD2;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;
			sampler2D _MainTex;
			sampler2D _TransitionTexture;
			
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.wpos = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.wpos);

				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;
				OUT.factor = half3(IN.factor.x, UnpackToVec2(IN.factor.y));
				
				return OUT;
			}


			fixed4 frag(v2f IN) : SV_Target
			{
				half effectFactor = IN.factor.x;
				half alpha = tex2D(_TransitionTexture, IN.factor.yz).a;

				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd);
				color.a *= UnityGet2DClipping(IN.wpos.xy, _ClipRect);

				#if FADE
				color.a = color.a * alpha + (effectFactor * 2 - 1);
				color.a *= step(0.001, color.a);
				#elif CUTOFF
				color.a = step(0.001, color.a * alpha - 1 + effectFactor);
				#endif

				#if UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				return color * IN.color;
			}
		ENDCG
		}
	}
}
