Shader "UI/Hidden/UI-Effect-Dissolve"
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

		[Header(Dissolve)]
		_NoiseTex("Noise Texture (A)", 2D) = "white" {}
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
			#pragma shader_feature __ ADD SUBTRACT FILL

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			#include "UI-Effect.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color	: COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID

				float2 uv1 : TEXCOORD1;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color	: COLOR;
				float2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
				
				fixed4 effectFactor : TEXCOORD2;
				fixed4 effectFactor2 : TEXCOORD3;
			};
			
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;
			sampler2D _MainTex;
			sampler2D _NoiseTex;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.worldPosition = IN.vertex;

				OUT.vertex = UnityObjectToClipPos(IN.vertex);

				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;

				//xy: Noize uv, z: Dissolve factor, w: width
				OUT.effectFactor = UnpackToVec4(IN.uv1.x);

				//xyz: color, w: softness
				OUT.effectFactor2 = UnpackToVec4(IN.uv1.y);
				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

				color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

				float cutout = tex2D(_NoiseTex, IN.effectFactor.xy).a;
				float location = IN.effectFactor.z;
				float width = IN.effectFactor.w/4;
				float factor = cutout - location * ( 1 + width ) + width;

				#ifdef UNITY_UI_ALPHACLIP
				clip (min(color.a - 0.01, factor));
				#endif

				fixed edgeLerp = step(factor, color.a) * saturate((width - factor)*16/ IN.effectFactor2.w);
				color = ApplyColorEffect(color, fixed4(IN.effectFactor2.rgb, edgeLerp));
				color.a *= saturate((factor)*32/ IN.effectFactor2.w);

				return color;
			}
		ENDCG
		}
	}
}