#ifndef UI_EFFECT_TMPRO_INCLUDED
#define UI_EFFECT_TMPRO_INCLUDED

#if MOBILE
#define UV(x) x.texcoord0
#else
#define UV(x) x.atlas
#endif

fixed4 Tex2DBlurring (pixel_t IN, half2 blur, half4 mask)
{
	#if FASTBLUR && EX
	const int KERNEL_SIZE = 5;
	const float KERNEL_[5] = { 0.2486, 0.7046, 1.0, 0.7046, 0.2486};
	#elif MEDIUMBLUR && EX
	const int KERNEL_SIZE = 9;
	const float KERNEL_[9] = { 0.0438, 0.1719, 0.4566, 0.8204, 1.0, 0.8204, 0.4566, 0.1719, 0.0438};
	#elif DETAILBLUR && EX
	const int KERNEL_SIZE = 13;
	const float KERNEL_[13] = { 0.0438, 0.1138, 0.2486, 0.4566, 0.7046, 0.9141, 1.0, 0.9141, 0.7046, 0.4566, 0.2486, 0.1138, 0.0438};
	#elif FASTBLUR
	const int KERNEL_SIZE = 3;
	const float KERNEL_[3] = { 0.4566, 1.0, 0.4566};
	#elif MEDIUMBLUR
	const int KERNEL_SIZE = 5;
	const float KERNEL_[5] = { 0.2486, 0.7046, 1.0, 0.7046, 0.2486};
	#elif DETAILBLUR
	const int KERNEL_SIZE = 7;
	const float KERNEL_[7] = { 0.1719, 0.4566, 0.8204, 1.0, 0.8204, 0.4566, 0.1719};
	#else
	const int KERNEL_SIZE = 1;
	const float KERNEL_[1] = { 1.0 };
	#endif
	float4 o = 0;
	float sum = 0;
	float2 shift = 0;
	half alpha = IN.color.a;
	half2 texcood = UV(IN);
	for(int x = 0; x < KERNEL_SIZE; x++)
	{
		shift.x = blur.x * (float(x) - KERNEL_SIZE/2);
		for(int y = 0; y < KERNEL_SIZE; y++)
		{
			shift.y = blur.y * (float(y) - KERNEL_SIZE/2);
			float2 uv = texcood + shift;
			float weight = KERNEL_[x] * KERNEL_[y];
			sum += weight;
			UV(IN).xy = uv;
			IN.color.a = weight;

			#if EX
			fixed masked = min(mask.x <= uv.x, uv.x <= mask.z) * min(mask.y <= uv.y, uv.y <= mask.w);
			o += lerp(fixed4(0, 0, 0, 0), PixShader(IN)* alpha, masked);
			#else
			o += PixShader(IN) * alpha;
			#endif
		}
	}
	return o / sum;
}

fixed4 Tex2DBlurring (pixel_t IN, half2 blur)
{
	return Tex2DBlurring(IN, blur, half4(0,0,1,1));
}

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

	return color;
}

#endif // UI_EFFECT_TMPRO_INCLUDED
