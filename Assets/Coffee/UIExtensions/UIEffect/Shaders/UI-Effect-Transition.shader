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
			#pragma shader_feature __ MONO CUTOFF

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			struct appdata_t
			{
				float4 vertex	: POSITION;
				float4 color	: COLOR;
				float2 texcoord	: TEXCOORD0;
				half factor		: TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex	: SV_POSITION;
				fixed4 color	: COLOR;
				float2 texcoord	: TEXCOORD0;
				float4 wpos		: TEXCOORD1;
				half factor		: TEXCOORD2;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;
			sampler2D _MainTex;
			
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.wpos = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.wpos);

				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;
				OUT.factor = IN.factor;
				
				return OUT;
			}


			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
				color.a *= UnityGet2DClipping(IN.wpos.xy, _ClipRect);

				#ifdef CUTOFF
				clip (color.a - 1 + IN.factor * 1.001);
				#elif UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				#if MONO
				color.rgb = IN.color.rgb;
				color.a = color.a * tex2D(_MainTex, IN.texcoord).a + IN.factor * 2 - 1;
				#elif CUTOFF
				color.rgb = IN.color.rgb;
				color.a = 1;
				#endif

				return color;
			}
		ENDCG
		}
	}
}
