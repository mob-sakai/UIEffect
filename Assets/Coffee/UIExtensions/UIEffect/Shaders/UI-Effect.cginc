#ifndef UI_EFFECT_INCLUDED
#define UI_EFFECT_INCLUDED

//################################
// Define
//################################
#define PACKER_STEP 64

#if UI_TONE_GRAYSCALE | UI_TONE_SEPIA | UI_TONE_NEGA | UI_TONE_PIXEL | UI_TONE_MONO | UI_TONE_CUTOFF | UI_TONE_HUE
#define UI_TONE
#endif

#if UI_COLOR_ADD | UI_COLOR_SUB | UI_COLOR_SET
#define UI_COLOR
#endif

#if UI_BLUR_FAST | UI_BLUR_MEDIUM | UI_BLUR_DETAIL
#define UI_BLUR
#endif

//################################
// Unpacker to vector
//################################
// Unpack float to low-precision [0-1] fixed4. 
fixed4 UnpackToVec4(float value)
{   
	const int PRECISION = PACKER_STEP - 1;
	fixed4 color;

    color.r = (value % PACKER_STEP) / PRECISION;
    value = floor(value / PACKER_STEP);

    color.g = (value % PACKER_STEP) / PRECISION;
    value = floor(value / PACKER_STEP);

    color.b = (value % PACKER_STEP) / PRECISION;
    value = floor(value / PACKER_STEP);

    color.a = (value % PACKER_STEP) / PRECISION;
    return color;
}

//################################
// Blur effect
//################################
// Calculate blur effect.
// Sample texture by blured uv, with bias.
fixed4 Blur(sampler2D tex, half2 uv, half2 addUv, half bias)
{
	return 
		(
		tex2D(tex, uv + half2(addUv.x, addUv.y))
		+ tex2D(tex, uv + half2(-addUv.x, addUv.y))
		+ tex2D(tex, uv + half2(addUv.x, -addUv.y))
		+ tex2D(tex, uv + half2(-addUv.x, -addUv.y))
#if UI_BLUR_DETAIL
		+ tex2D(tex, uv + half2(addUv.x, 0))
		+ tex2D(tex, uv + half2(-addUv.x, 0))
		+ tex2D(tex, uv + half2(0, addUv.y))
		+ tex2D(tex, uv + half2(0, -addUv.y))
		)
		* bias / 2;
#else
		)
		* bias;
#endif
}

// Sample texture with blurring.
// * Fast: Sample texture with 3x4 blurring.
// * Medium: Sample texture with 6x4 blurring.
// * Detail: Sample texture with 6x8 blurring.
fixed4 Tex2DBlurring(sampler2D tex, half2 uv, half2 blur)
{
	half4 color = tex2D(tex, uv);

	#if UI_BLUR_FAST
	return color * 0.41511
		+ Blur( tex, uv, blur * 3, 0.12924 )
		+ Blur( tex, uv, blur * 5, 0.01343 )
		+ Blur( tex, uv, blur * 6, 0.00353 );

	#elif UI_BLUR_MEDIUM | UI_BLUR_DETAIL
	return color * 0.14387
		+ Blur( tex, uv, blur * 1, 0.06781 )
		+ Blur( tex, uv, blur * 2, 0.05791 )
		+ Blur( tex, uv, blur * 3, 0.04360 )
		+ Blur( tex, uv, blur * 4, 0.02773 )
		+ Blur( tex, uv, blur * 5, 0.01343 )
		+ Blur( tex, uv, blur * 6, 0.00353 );
	#else
	return color;
	#endif
}

//################################
// Tone effect
//################################
fixed3 shift_hue(fixed3 RGB, half VSU, half VSW)
{
	fixed3 RESULT;
	RESULT.x = (0.299 + 0.701*VSU + 0.168*VSW)*RGB.x
		+ (0.587 - 0.587*VSU + 0.330*VSW)*RGB.y
		+ (0.114 - 0.114*VSU - 0.497*VSW)*RGB.z;

	RESULT.y = (0.299 - 0.299*VSU - 0.328*VSW)*RGB.x
		+ (0.587 + 0.413*VSU + 0.035*VSW)*RGB.y
		+ (0.114 - 0.114*VSU + 0.292*VSW)*RGB.z;

	RESULT.z = (0.299 - 0.3*VSU + 1.25*VSW)*RGB.x
		+ (0.587 - 0.588*VSU - 1.05*VSW)*RGB.y
		+ (0.114 + 0.886*VSU - 0.203*VSW)*RGB.z;

	return (RESULT);
}

// Apply tone effect.
fixed4 ApplyToneEffect(fixed4 color, fixed factor)
{
	#ifdef UI_TONE_GRAYSCALE	// Grayscale
	color.rgb = lerp(color.rgb, Luminance(color.rgb), factor);

	#elif UI_TONE_SEPIA			// Sepia
	color.rgb = lerp(color.rgb, Luminance(color.rgb) * half3(1.07, 0.74, 0.43), factor);

	#elif UI_TONE_NEGA			// Nega
	color.rgb = lerp(color.rgb, 1 - color.rgb, factor);
	#endif

	return color;
}


//################################
// Color effect
//################################
// Apply color effect.
half4 ApplyColorEffect(half4 color, half4 factor)
{
	#ifdef UI_COLOR_SET // Set
	color.rgb = lerp(color.rgb, factor.rgb, factor.a);

	#elif UI_COLOR_ADD // Add
	color.rgb += factor.rgb * factor.a;

	#elif UI_COLOR_SUB // Sub
	color.rgb -= factor.rgb * factor.a;

	#else
	color.rgb = lerp(color.rgb, color.rgb * factor.rgb, factor.a);
	#endif

	#if UI_TONE_CUTOFF
	color.a = factor.a;
	#endif

	return color;
}

#endif // UI_EFFECT_INCLUDED
