Shader "UI/Hidden/UI-EffectCapture"
{
	Properties
	{
		[PerRendererData] _MainTex("Main Texture", 2D) = "white" {}
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
			#pragma target 2.0

			#pragma shader_feature __ GRAYSCALE SEPIA NEGA PIXEL MONO CUTOFF HUE 
			#pragma shader_feature __ ADD SUBTRACT FILL
			#pragma shader_feature __ FASTBLUR MEDIUMBLUR DETAILBLUR
			
			#include "UnityCG.cginc"
			#include "UI-Effect.cginc"

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float2 texcoord  : TEXCOORD0;
				
				#if defined (UI_COLOR)
				fixed4 colorFactor : COLOR1;
				#endif

				half4 effectFactor : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			half4 _EffectFactor;
			fixed4 _ColorFactor;

			v2f vert(appdata_img v)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(v.vertex);

				OUT.texcoord = v.texcoord;
				OUT.effectFactor = _EffectFactor;

				#if HUE
				OUT.effectFactor.y = sin(OUT.effectFactor.x*3.14159265359*2);
				OUT.effectFactor.x = cos(OUT.effectFactor.x*3.14159265359*2);
				#elif PIXEL
				OUT.effectFactor.xy = max(2, (1-OUT.effectFactor.x) * _MainTex_TexelSize.zw);
				#endif
				
				#if defined (UI_COLOR)
				OUT.colorFactor = _ColorFactor;
				#endif
				
				#if UNITY_UV_STARTS_AT_TOP
				OUT.texcoord.y = lerp(OUT.texcoord.y, 1 - OUT.texcoord.y, OUT.effectFactor.w);
				#endif
				
				return OUT;
			}


			fixed4 frag(v2f IN) : SV_Target
			{
				#if PIXEL
				IN.texcoord = round(IN.texcoord * IN.effectFactor.xy) / IN.effectFactor.xy;
				#endif
				
				#if defined (UI_BLUR)
				half4 color = Tex2DBlurring(_MainTex, IN.texcoord, IN.effectFactor.z * _MainTex_TexelSize.xy * 2);
				#else
				half4 color = tex2D(_MainTex, IN.texcoord);
				#endif

				#if HUE
				color.rgb = shift_hue(color.rgb, IN.effectFactor.x, IN.effectFactor.y);
				#elif defined (UI_TONE)
				color = ApplyToneEffect(color, IN.effectFactor.x);
				#endif
				
				#if defined (UI_COLOR)
				color = ApplyColorEffect(color, IN.colorFactor);
				#endif

				color.a = 1;
				return color;
			}
		ENDCG
		}
	}
}
