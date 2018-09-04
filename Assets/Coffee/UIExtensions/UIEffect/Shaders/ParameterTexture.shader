Shader "UI/Hidden/ParameterTexture"
{
	Properties
	{
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Background"
			"IgnoreProjector" = "True"
			"RenderType" = "Opaque"
			"PreviewType" = "Plane"
		}

		Cull Off
		Lighting Off
		ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				half4 vertex : POSITION;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				half4 vertex : SV_POSITION;
				fixed4 color : COLOR;
			};

			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = v.vertex;
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return i.color;
			}
			ENDCG
		}
	}
}
