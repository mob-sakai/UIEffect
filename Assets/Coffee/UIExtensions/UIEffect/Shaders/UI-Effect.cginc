#ifndef UI_EFFECT_INCLUDED
#define UI_EFFECT_INCLUDED

#if GRAYSCALE | SEPIA | NEGA | PIXEL | MONO | CUTOFF | HUE
#define UI_TONE
#endif

#if ADD | SUBTRACT | FILL
#define UI_COLOR
#endif

#if FASTBLUR | MEDIUMBLUR | DETAILBLUR
#define UI_BLUR
#endif

// Unpack float to low-precision [0-1] fixed4.
fixed4 UnpackToVec4(float value)
{
	const int PACKER_STEP = 64;
	const int PRECISION = PACKER_STEP - 1;
	fixed4 unpacked;

	unpacked.x = (value % PACKER_STEP) / PRECISION;
	value = floor(value / PACKER_STEP);

	unpacked.y = (value % PACKER_STEP) / PRECISION;
	value = floor(value / PACKER_STEP);

	unpacked.z = (value % PACKER_STEP) / PRECISION;
	value = floor(value / PACKER_STEP);

	unpacked.w = (value % PACKER_STEP) / PRECISION;
	return unpacked;
}

// Unpack float to low-precision [0-1] fixed3.
fixed3 UnpackToVec3(float value)
{
	const int PACKER_STEP = 256;
	const int PRECISION = PACKER_STEP - 1;
	fixed3 unpacked;

	unpacked.x = (value % (PACKER_STEP)) / (PACKER_STEP - 1);
	value = floor(value / (PACKER_STEP));

	unpacked.y = (value % PACKER_STEP) / (PACKER_STEP - 1);
	value = floor(value / PACKER_STEP);

	unpacked.z = (value % PACKER_STEP) / (PACKER_STEP - 1);
	return unpacked;
}

// Unpack float to low-precision [0-1] half2.
half2 UnpackToVec2(float value)
{
	const int PACKER_STEP = 4096;
	const int PRECISION = PACKER_STEP - 1;
	half2 unpacked;

	unpacked.x = (value % (PACKER_STEP)) / (PACKER_STEP - 1);
	value = floor(value / (PACKER_STEP));

	unpacked.y = (value % PACKER_STEP) / (PACKER_STEP - 1);
	return unpacked;
}

// Sample texture with blurring.
// * Fast: Sample texture with 3x3 kernel.
// * Medium: Sample texture with 5x5 kernel.
// * Detail: Sample texture with 7x7 kernel.
fixed4 Tex2DBlurring (sampler2D tex, half2 texcood, half2 blur, half4 mask)
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
	for(int x = 0; x < KERNEL_SIZE; x++)
	{
		shift.x = blur.x * (float(x) - KERNEL_SIZE/2);
		for(int y = 0; y < KERNEL_SIZE; y++)
		{
			shift.y = blur.y * (float(y) - KERNEL_SIZE/2);
			float2 uv = texcood + shift;
			float weight = KERNEL_[x] * KERNEL_[y];
			sum += weight;
			#if EX
			fixed masked = min(mask.x <= uv.x, uv.x <= mask.z) * min(mask.y <= uv.y, uv.y <= mask.w);
			o += lerp(fixed4(0.5, 0.5, 0.5, 0), tex2D(tex, uv), masked) * weight;
			#else
			o += tex2D(tex, uv) * weight;
			#endif
		}
	}
	return o / sum;
}

// Sample texture with blurring.
// * Fast: Sample texture with 3x3 kernel.
// * Medium: Sample texture with 5x5 kernel.
// * Detail: Sample texture with 7x7 kernel.
fixed4 Tex2DBlurring (sampler2D tex, half2 texcood, half2 blur)
{
	return Tex2DBlurring(tex, texcood, blur, half4(0,0,1,1));
}


// Sample texture with blurring.
// * Fast: Sample texture with 3x1 kernel.
// * Medium: Sample texture with 5x1 kernel.
// * Detail: Sample texture with 7x1 kernel.
fixed4 Tex2DBlurring1D (sampler2D tex, half2 uv, half2 blur)
{
	#if FASTBLUR
	const int KERNEL_SIZE = 3;
	#elif MEDIUMBLUR
	const int KERNEL_SIZE = 5;
	#elif DETAILBLUR
	const int KERNEL_SIZE = 7;
	#else
	const int KERNEL_SIZE = 1;
	#endif
	float4 o = 0;
	float sum = 0;
	float weight;
	half2 texcood;
	for(int i = -KERNEL_SIZE/2; i <= KERNEL_SIZE/2; i++)
	{
		texcood = uv;
		texcood.x += blur.x * i;
		texcood.y += blur.y * i;
		weight = 1.0/(abs(i)+2);
		o += tex2D(tex, texcood)*weight;
		sum += weight;
	}
	return o / sum;
}

fixed3 shift_hue(fixed3 RGB, half VSU, half VSW)
{
	fixed3 result;
	result.x = (0.299 + 0.701*VSU + 0.168*VSW)*RGB.x
		+ (0.587 - 0.587*VSU + 0.330*VSW)*RGB.y
		+ (0.114 - 0.114*VSU - 0.497*VSW)*RGB.z;

	result.y = (0.299 - 0.299*VSU - 0.328*VSW)*RGB.x
		+ (0.587 + 0.413*VSU + 0.035*VSW)*RGB.y
		+ (0.114 - 0.114*VSU + 0.292*VSW)*RGB.z;

	result.z = (0.299 - 0.3*VSU + 1.25*VSW)*RGB.x
		+ (0.587 - 0.588*VSU - 1.05*VSW)*RGB.y
		+ (0.114 + 0.886*VSU - 0.203*VSW)*RGB.z;

	return result;
}

// Apply tone effect.
fixed4 ApplyToneEffect(fixed4 color, fixed factor)
{
	#ifdef GRAYSCALE
	color.rgb = lerp(color.rgb, Luminance(color.rgb), factor);

	#elif SEPIA
	color.rgb = lerp(color.rgb, Luminance(color.rgb) * half3(1.07, 0.74, 0.43), factor);

	#elif NEGA
	color.rgb = lerp(color.rgb, 1 - color.rgb, factor);
	#endif

	return color;
}

// Apply color effect.
half4 ApplyColorEffect(half4 color, half4 factor)
{
	#ifdef FILL
	color.rgb = lerp(color.rgb, factor.rgb, factor.a);

	#elif ADD
	color.rgb += factor.rgb * factor.a;

	#elif SUBTRACT
	color.rgb -= factor.rgb * factor.a;

	#else
	color.rgb = lerp(color.rgb, color.rgb * factor.rgb, factor.a);
	#endif

	#if CUTOFF
	color.a = factor.a;
	#endif

	return color;
}

#endif // UI_EFFECT_INCLUDED
