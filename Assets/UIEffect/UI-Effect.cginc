#ifndef UI_EFFECT_INCLUDED
#define UI_EFFECT_INCLUDED

//################################
// Define
//################################
#define PACKER_STEP 64

#if UI_TONE_GRAYSCALE | UI_TONE_SEPIA | UI_TONE_NEGA | UI_TONE_PIXEL | UI_TONE_MONO | UI_TONE_CUTOFF
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

// Unpack float to low-precision [0-1] half3.
// The z value is a little high precision.
half3 UnpackToVec3(float value)
{   
	const int PRECISION = PACKER_STEP - 1;
	const int PACKER_STEP_HIGH = PACKER_STEP * PACKER_STEP;
	const int PRECISION_HIGH = PACKER_STEP_HIGH - 1;
	half3 color;

    color.x = (value % PACKER_STEP) / PRECISION;
    value = floor(value / PACKER_STEP);

    color.y = (value % PACKER_STEP) / PRECISION;
    value = floor(value / PACKER_STEP);

    color.z = (value % PACKER_STEP_HIGH) / (PRECISION_HIGH - 1);

    return color;
}

//################################
// Blur effect
//################################
// Calculate blur effect.
// Sample texture by blured uv, with bias.
fixed4 Blur(sampler2D tex, half2 uv, half addUv, half bias)
{
	return 
		(
		tex2D(tex, uv + half2(addUv, addUv))
		+ tex2D(tex, uv + half2(-addUv, addUv))
		+ tex2D(tex, uv + half2(addUv, -addUv))
		+ tex2D(tex, uv + half2(-addUv, -addUv))
#if UI_BLUR_DETAIL
		+ tex2D(tex, uv + half2(addUv, 0))
		+ tex2D(tex, uv + half2(-addUv, 0))
		+ tex2D(tex, uv + half2(0, addUv))
		+ tex2D(tex, uv + half2(0, -addUv))
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
fixed4 Tex2DBlurring(sampler2D tex, half2 uv, half blur)
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
fixed4 ApplyColorEffect(fixed4 color, fixed4 factor)
{
	#ifdef UI_COLOR_SET // Set
	color.rgb = lerp(color.rgb, factor.rgb, factor.a);

	#elif UI_COLOR_ADD // Add
	color.rgb += factor.rgb * factor.a;

	#elif UI_COLOR_SUB // Sub
	color.rgb -= factor.rgb * factor.a;
	#endif

	#if UI_TONE_CUTOFF
	color.a = factor.a;
	#endif

	return color;
}

#endif // UI_EFFECT_INCLUDED
