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
		_NoiseTex ("Noise Texture (A)", 2D) = "white" {}
		_EdgeWidth ("Edge width", Range (0.0001, 1)) = 0.05
		_OuterEdgeColor ("Outer edge color", Color) = (1.0, 0.75, 0.0, 1.0)
		_OuterSoftness ("Outer softness", Range (0.0001, 1)) = 0.5
		_InnerEdgeColor ("Inner edge color", Color) = (1.0, 0.0, 0.0, 1.0)
		_InnerSoftness ("Inner softness", Range (0.0001, 1)) = 0.5
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
				
				half3 effectFactor : TEXCOORD2;
			};
			
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;
			sampler2D _MainTex;

			sampler2D _NoiseTex;
			float _EdgeWidth;
			float3 _OuterEdgeColor;
			float _OuterSoftness;
			float3 _InnerEdgeColor;
			float _InnerSoftness;

			fixed3 UnpackToVec3(float value)
			{
				const int PACKER_STEP = 256;
				const int PRECISION = PACKER_STEP - 1;
				fixed3 vec;

				vec.x = (value % PACKER_STEP) / PRECISION;
				value = floor(value / PACKER_STEP);

				vec.y = (value % PACKER_STEP) / PRECISION;
				value = floor(value / PACKER_STEP);

				vec.z = (value % PACKER_STEP) / PRECISION;
				return vec;
			}

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.worldPosition = IN.vertex;

				OUT.vertex = UnityObjectToClipPos(IN.vertex);

				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;

				//xy: Noize uv
				//z: Dissolve factor
				OUT.effectFactor = UnpackToVec3(IN.uv1.x);
				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

				color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

				float cutout = tex2D(_NoiseTex, IN.effectFactor.xy).a;
				fixed factor = cutout - IN.effectFactor.z;

				#ifdef UNITY_UI_ALPHACLIP
				clip (min(color.a - 0.01, factor));
				#endif

				fixed edgeLerp = step(cutout, color.a) * saturate((_EdgeWidth - factor)*16/_InnerSoftness);
				fixed3 edgeColor = lerp(_OuterEdgeColor, _InnerEdgeColor, (factor)/_EdgeWidth);

				color.rgb =lerp(color.rgb, edgeColor, edgeLerp);
				color.a *= saturate((factor)*32/_OuterSoftness);

				return color;
			}
		ENDCG
		}
	}
}