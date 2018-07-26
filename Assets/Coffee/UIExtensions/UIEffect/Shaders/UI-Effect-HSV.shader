Shader "UI/Hidden/UI-Effect-HSV"
{
	Properties
	{
		[PerRendererData] _MainTex ("Main Texture", 2D) = "white" {}
		
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
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ UNITY_UI_ALPHACLIP

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			#include "UI-Effect.cginc"

			sampler2D _MainTex;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;

			struct appdata_t
			{
				float4 vertex	: POSITION;
				float4 color	: COLOR;
				float2 texcoord	: TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID

				float2 uv1		: TEXCOORD1;
			};

			struct v2f
			{
				float4 vertex	: SV_POSITION;
				fixed4 color	: COLOR;
				float2 texcoord	: TEXCOORD0;
				float4 wpos		: TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
				
				half4 range		: TEXCOORD2;
				half3 hsv		: TEXCOORD3;
			};


			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.wpos = IN.vertex;

				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color;

				OUT.range = UnpackToVec4(IN.uv1.x);
				OUT.hsv = UnpackToVec3(IN.uv1.y) - 0.5f;
				OUT.hsv.yz *= 2;
				
				return OUT;
			}

			half3 rgb2hsv(half3 c) {
			  half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			  half4 p = lerp(half4(c.bg, K.wz), half4(c.gb, K.xy), step(c.b, c.g));
			  half4 q = lerp(half4(p.xyw, c.r), half4(c.r, p.yzx), step(p.x, c.r));

			  half d = q.x - min(q.w, q.y);
			  half e = 1.0e-10;
			  return half3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}

			half3 hsv2rgb(half3 c) {
			  c = half3(c.x, clamp(c.yz, 0.0, 1.0));
			  half4 K = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
			  half3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
			  return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
			}

			fixed4 frag(v2f IN) : COLOR
			{
				half4 color = tex2D(_MainTex, IN.texcoord);// + _TextureSampleAdd) * IN.color;

				color.a *= UnityGet2DClipping(IN.wpos.xy, _ClipRect);

				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				half3 hsv = rgb2hsv(color.rgb);
				half3 range = abs(hsv - IN.range.xyz);
				half diff = max(max(min(1-range.x, range.x), min(1-range.y, range.y)/10), min(1-range.z, range.z)/10);

				fixed masked = step(diff, IN.range.w);
				color.rgb = hsv2rgb(hsv + IN.hsv.xyz * masked);

				return (color + _TextureSampleAdd) * IN.color;
			}

			ENDCG
		}
	}
}