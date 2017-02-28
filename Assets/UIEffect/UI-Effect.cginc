#ifndef UI_EFFECT_INCLUDED
#define UI_EFFECT_INCLUDED

//################################
// Define
//################################
#define PACKER_STEP 64

#if UI_TONE_GRAYSCALE | UI_TONE_SEPIA | UI_TONE_NEGA
#define UI_TONE
#endif

#if UI_COLOR_ADD | UI_COLOR_SUB | UI_COLOR_SET
#define UI_COLOR
#endif

#if UI_BLUR_FAST | UI_BLUR_DETAIL
#define UI_BLUR
#endif

//################################
// Unpacker to vector
//################################
#if defined (UI_COLOR)
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
#endif

#if defined (UI_TONE) || defined (UI_BLUR)
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
#endif

//################################
// Blur effect
//################################
#if defined (UI_BLUR)
// Calculate blur effect.
// Sample texture by blured uv, with bias.
fixed4 Blur(sampler2D tex, half2 uv, half2 addUv, half bias)
{
	half4 color = tex2D( tex, uv + addUv);
	color += tex2D( tex, uv - addUv);

	addUv.x *= -1;
	color += tex2D( tex, uv + addUv);
	color += tex2D( tex, uv - addUv);

	return color * bias;
}

// Sample texture with blurring.
// * Fast: Sample texture with 3x4 blurring.
// * Detail: Sample texture with 6x4 blurring.
fixed4 Tex2DBlurring(sampler2D tex, half2 uv, fixed blurring)
{
	half4 color = tex2D(tex, uv);
	half2 dir = half2( blurring, blurring);
	fixed4 blurring_color;

	#if UI_BLUR_FAST		// Blur(Fast)
	const half originalBias = 0.41511;
	blurring_color
		= Blur( tex, uv, dir * 3, 0.12924 )
		+ Blur( tex, uv, dir * 5, 0.01343 )
		+ Blur( tex, uv, dir * 6, 0.00353 );

	#elif UI_BLUR_DETAIL	// Blur(Detail)
	const half originalBias = 0.14387;
	blurring_color
		= Blur( tex, uv, dir * 1, 0.06781 )
		+ Blur( tex, uv, dir * 2, 0.05791 )
		+ Blur( tex, uv, dir * 3, 0.04360 )
		+ Blur( tex, uv, dir * 4, 0.02773 )
		+ Blur( tex, uv, dir * 5, 0.01343 )
		+ Blur( tex, uv, dir * 6, 0.00353 );
	#endif
	
	return color * originalBias + blurring_color;
}
#endif

//################################
// Tone effect
//################################
#if defined (UI_TONE)
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
#endif


//################################
// Color effect
//################################
#if defined (UI_COLOR)
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

	return color;
}
#endif

#endif // UI_EFFECT_INCLUDED
