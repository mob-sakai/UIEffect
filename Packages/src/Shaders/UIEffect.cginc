#ifndef UI_EFFECT_INCLUDED
#define UI_EFFECT_INCLUDED

#ifndef UNITY_PI
#define UNITY_PI 3.14159265358979323846
#endif

#ifndef CANVAS_SHADERGRAPH
uniform const int _SrcBlend;
uniform const int _DstBlend;
uniform const float4 _MainTex_TexelSize;
#endif

uniform const float _ToneIntensity;
uniform const int _ColorFilter;
uniform const half4 _ColorValue;
uniform const float _ColorIntensity;
uniform const int _ColorGlow;
uniform const float _SamplingIntensity;
uniform const float _SamplingWidth;
uniform const float _SamplingScale;
uniform const sampler2D _TransitionTex;
uniform const float4 _TransitionTex_ST;
uniform const float2 _TransitionTex_Speed;
uniform const float _TransitionRate;
uniform const int _TransitionReverse;
uniform const int _TransitionColorFilter;
uniform const half4 _TransitionColor;
uniform const int _TransitionColorGlow;
uniform const float _TransitionSoftness;
uniform const float2 _TransitionRange;
uniform const float _TransitionWidth;
uniform const int _TransitionPatternReverse;
uniform const float _TransitionAutoPlaySpeed;
uniform const half4 _TargetColor;
uniform const float _TargetRange;
uniform const float _TargetSoftness;
uniform const float _ShadowBlurIntensity;
uniform const int _ShadowColorFilter;
uniform const half4 _ShadowColor;
uniform const int _ShadowColorGlow;
uniform const float _EdgeWidth;
uniform const float _EdgeShinyRate;
uniform const float _EdgeShinyAutoPlaySpeed;
uniform const float _EdgeShinyWidth;
uniform const int _EdgeColorFilter;
uniform const half4 _EdgeColor;
uniform const int _EdgeColorGlow;
uniform const int _PatternArea;
uniform const float _DetailIntensity;
uniform const float2 _DetailThreshold;
uniform const sampler2D _DetailTex;
uniform const float4 _DetailTex_ST;
uniform const float2 _DetailTex_Speed;
uniform const float _GradationIntensity;
uniform const int _GradationColorFilter;
uniform const half4 _GradationColor1;
uniform const half4 _GradationColor2;
uniform const half4 _GradationColor3;
uniform const half4 _GradationColor4;
uniform const sampler2D _GradationTex;
uniform const float4 _GradationTex_ST;
uniform const matrix _RootViewMatrix;
uniform const matrix _GradViewMatrix;
uniform const matrix _CanvasToWorldMatrix;

// For performance reasons, limit the sampling of blur in TextMeshPro.
#ifdef UIEFFECT_TEXTMESHPRO
#ifdef SAMPLING_BLUR_MEDIUM
        #undef SAMPLING_BLUR_MEDIUM
        #define SAMPLING_BLUR_FAST 1
#endif
#ifdef SAMPLING_BLUR_DETAIL
        #undef SAMPLING_BLUR_DETAIL
        #define SAMPLING_BLUR_FAST 1
#endif
#endif

#define TONE_NONE (!TONE_GRAYSCALE && !TONE_SEPIA && !TONE_NEGATIVE && !TONE_RETRO && !TONE_POSTERIZE)
#define SAMPLING_NONE (!SAMPLING_BLUR_FAST && !SAMPLING_BLUR_MEDIUM && !SAMPLING_BLUR_DETAIL && !SAMPLING_PIXELATION && !SAMPLING_RGB_SHIFT && !SAMPLING_EDGE_LUMINANCE && !SAMPLING_EDGE_ALPHA)
#define TRANSITION_NONE (!TRANSITION_FADE && !TRANSITION_CUTOFF && !TRANSITION_DISSOLVE && !TRANSITION_SHINY && !TRANSITION_MASK && !TRANSITION_MELT && !TRANSITION_BURN && !TRANSITION_PATTERN)
#define TARGET_NONE (!TARGET_HUE && !TARGET_LUMINANCE)
#define EDGE_NONE (!EDGE_PLAIN && !EDGE_SHINY)
#define DETAIL_NONE (!DETAIL_MASKING && !DETAIL_MULTIPLY && !DETAIL_ADDITIVE && !DETAIL_REPLACE && !DETAIL_MULTIPLY_ADDITIVE)
#define GRADATION_NONE (!GRADATION_GRADIENT && !GRADATION_RADIAL && !GRADATION_COLOR2 && !GRADATION_COLOR4)

#if TONE_NONE && !COLOR_FILTER && SAMPLING_NONE && TRANSITION_NONE && TARGET_NONE && EDGE_NONE && DETAIL_NONE && GRADATION_NONE
#define NO_UIEFFECT
#endif

#define UIEFFECT_UV_MASK(uv, uvMask) step(uvMask.x, uv.x) * step(uv.x, uvMask.z) * step(uvMask.y, uv.y) * step(uv.y, uvMask.w)
#define UIEFFECT_SAMPLE(uv) uieffect_frag(uv)
#define UIEFFECT_SAMPLE_CLAMP(uv, uvMask) uieffect_frag(uv) * UIEFFECT_UV_MASK(uv, uvMask)

#ifdef CANVAS_SHADERGRAPH
#define TEX_SAMPLE_CLAMP(uv, uvMask) SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * UIEFFECT_UV_MASK(uv, uvMask)
#else
#define TEX_SAMPLE_CLAMP(uv, uvMask) tex2D(_MainTex, uv) * UIEFFECT_UV_MASK(uv, uvMask)
#endif

float4 object_to_world(float4 pos)
{
    #if UIEFFECT_EDITOR
    return pos;
    return mul(unity_ObjectToWorld, pos);
    #else
    return mul(_CanvasToWorldMatrix, pos);
    return mul(_CanvasToWorldMatrix, mul(unity_ObjectToWorld, pos));
    #endif
}

float2 texel_size()
{
    return _MainTex_TexelSize.xy * _SamplingScale;
}

float transition_rate()
{
    return frac(_TransitionAutoPlaySpeed * _Time.y + _TransitionRate * 0.9999);
}

float3 rgb_to_hsv(float3 c)
{
    const float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    const float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
    const float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
    const float d = q.x - min(q.w, q.y);
    const float e = 1.0e-4;
    return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

float3 hsv_to_rgb(float3 c)
{
    const float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    const float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
}

float inv_lerp(const float from, const float to, const float value)
{
    return saturate(max(0, value - from) / max(0.001, to - from));
}

float transition_alpha(float2 uvLocal)
{
    float2 uv;
    #if TRANSITION_NONE
    return 1;
    #elif TRANSITION_PATTERN
    {
        const half scale = lerp(100, 1, _TransitionWidth);
        const half2 time = half2(-transition_rate() * 2, 0);
        uv = uvLocal * _TransitionTex_ST.xy * scale + _TransitionTex_ST.zw + time;
    }
    #else
    {
        uv = uvLocal * _TransitionTex_ST.xy + _TransitionTex_ST.zw;
    }
    #endif

    const float alpha = tex2D(_TransitionTex, uv + _Time.y * _TransitionTex_Speed).a;
    return _TransitionReverse ? 1 - alpha : alpha;
}


half get_target_rate(const half3 color)
{
    #if TARGET_NONE
    return 1;
    #endif

    if (1 <= _TargetRange) return 1;
    if (_TargetRange <= 0) return 0;

    half diff = 0;
    #if TARGET_HUE // Target.Hue
    {
        const half value = rgb_to_hsv(color).x;
        const half target = rgb_to_hsv(_TargetColor.rgb).x;
        diff = abs(target - value);
        diff = min(diff, 1 - diff);
    }
    #elif TARGET_LUMINANCE // Target.Luminance
    {
        const half value = Luminance(color);
        const half target = Luminance(_TargetColor);
        diff = abs(target - value);
    }
    #endif

    return 1 - inv_lerp(_TargetRange * (1 - _TargetSoftness), _TargetRange, diff);
}

half4 apply_tone_filter(half4 color)
{
    #if TONE_GRAYSCALE // Tone.Grayscale
    color.rgb = lerp(color.rgb, Luminance(color.rgb), _ToneIntensity);
    #elif TONE_SEPIA // Tone.Sepia
    color.rgb = lerp(color.rgb, Luminance(color.rgb) * half3(1.07, 0.74, 0.43), _ToneIntensity);
    #elif TONE_NEGATIVE // Tone.Negative
    color.rgb = lerp(color.rgb, (1 - color.rgb) * color.a, _ToneIntensity);
    #elif TONE_RETRO // Tone.Retro
    const half l = Luminance(color.rgb);
    const half r0 = step(l, 0.25);
    const half r1 = step(l, 0.5);
    const half r2 = step(l, 0.75);
    const half3 retro = half3(0.06, 0.22, 0.06) * r0 // 0.0-0.25: (15, 56, 15)
        + half3(0.19, 0.38, 0.19) * (1 - r0) * r1 // 0.25-0.5: (48, 98, 48)
        + half3(0.54, 0.67, 0.06) * (1 - r1) * r2 // 0.5-0.75: (139, 172, 15)
        + half3(0.60, 0.74, 0.06) * (1 - r2); // 0.75-1.0: (155, 188, 15)
    color.rgb = lerp(color.rgb, retro * color.a, _ToneIntensity);
    #elif TONE_POSTERIZE // Tone.Posterize
    const half3 hsv = rgb_to_hsv(color.rgb);
    const float div = round(lerp(48, 4, _ToneIntensity) / 2) * 2;
    color.rgb = hsv_to_rgb((floor(hsv * div) + 0.5) / div) * color.a;
    #endif

    return color;
}

half4 apply_color_filter(const half4 inColor, const half4 factor, const float intensity)
{
    const float glow = _ColorGlow;
    const int mode = _ColorFilter;
    half4 color = inColor;
    if (mode == 1) // Color.Multiply
    {
        color.rgb = color.rgb * factor.rgb;
    }
    else if (mode == 2) // Color.Additive
    {
        color.rgb = color.rgb + factor.rgb * color.a;
    }
    else if (mode == 3) // Color.Subtractive
    {
        color.rgb = color.rgb - factor.rgb * color.a;
    }
    else if (mode == 4) // Color.Replace
    {
        color.rgb = factor.rgb * color.a;
    }
    else if (mode == 5) // Color.MultiplyLuminance
    {
        color.rgb = (1 + Luminance(color.rgb)) * factor.rgb / 2 * color.a;
    }
    else if (mode == 6) // Color.MultiplyAdditive
    {
        color.rgb = color.rgb * (1 + factor.rgb);
    }
    else if (mode == 7) // Color.HsvModifier
    {
        const float3 hsv = rgb_to_hsv(color.rgb);
        color.rgb = hsv_to_rgb(hsv + factor.rgb) * color.a * factor.a;
        color.a = inColor.a * factor.a;
    }
    else if (mode == 8) // Color.Contrast
    {
        color.rgb = ((color.rgb - 0.5) * (factor.r + 1) + 0.5 + factor.g * 1.5) * color.a * factor.a;
        color.a = color.a * factor.a;
    }

    if (0 < mode)
    {
        color = lerp(inColor, color, intensity);
        color.a *= 1 - glow * intensity;
    }
    
    return color;
}

half4 apply_transition_color_filter(const half4 inColor, const half4 factor, const float intensity)
{
    const float glow = _TransitionColorGlow;
    const int mode = _TransitionColorFilter;
    half4 color = inColor;
    if (mode == 1) // Color.Multiply
    {
        color.rgb = color.rgb * factor.rgb;
    }
    else if (mode == 2) // Color.Additive
    {
        color.rgb = color.rgb + factor.rgb * color.a;
    }
    else if (mode == 3) // Color.Subtractive
    {
        color.rgb = color.rgb - factor.rgb * color.a;
    }
    else if (mode == 4) // Color.Replace
    {
        color.rgb = factor.rgb * color.a;
    }
    else if (mode == 5) // Color.MultiplyLuminance
    {
        color.rgb = (1 + Luminance(color.rgb)) * factor.rgb / 2 * color.a;
    }
    else if (mode == 6) // Color.MultiplyAdditive
    {
        color.rgb = color.rgb * (1 + factor.rgb);
    }
    else if (mode == 7) // Color.HsvModifier
    {
        const float3 hsv = rgb_to_hsv(color.rgb);
        color.rgb = hsv_to_rgb(hsv + factor.rgb) * color.a * factor.a;
        color.a = inColor.a * factor.a;
    }
    else if (mode == 8) // Color.Contrast
    {
        color.rgb = ((color.rgb - 0.5) * (factor.r + 1) + 0.5 + factor.g * 1.5) * color.a * factor.a;
        color.a = color.a * factor.a;
    }

    if (0 < mode)
    {
        color = lerp(inColor, color, intensity);
        color.a *= 1 - glow * intensity;
    }

    return color;
}

half4 apply_shadow_color_filter(const half4 inColor, const half4 factor, const float intensity)
{
    const float glow = _ShadowColorGlow;
    const int mode = _ShadowColorFilter;
    half4 color = inColor;
    if (mode == 1) // Color.Multiply
    {
        color.rgb = color.rgb * factor.rgb;
    }
    else if (mode == 2) // Color.Additive
    {
        color.rgb = color.rgb + factor.rgb * color.a;
    }
    else if (mode == 3) // Color.Subtractive
    {
        color.rgb = color.rgb - factor.rgb * color.a;
    }
    else if (mode == 4) // Color.Replace
    {
        color.rgb = factor.rgb * color.a;
    }
    else if (mode == 5) // Color.MultiplyLuminance
    {
        color.rgb = (1 + Luminance(color.rgb)) * factor.rgb / 2 * color.a;
    }
    else if (mode == 6) // Color.MultiplyAdditive
    {
        color.rgb = color.rgb * (1 + factor.rgb);
    }
    else if (mode == 7) // Color.HsvModifier
    {
        const float3 hsv = rgb_to_hsv(color.rgb);
        color.rgb = hsv_to_rgb(hsv + factor.rgb) * color.a * factor.a;
        color.a = inColor.a * factor.a;
    }
    else if (mode == 8) // Color.Contrast
    {
        color.rgb = ((color.rgb - 0.5) * (factor.r + 1) + 0.5 + factor.g * 1.5) * color.a * factor.a;
        color.a = color.a * factor.a;
    }

    if (0 < mode)
    {
        color = lerp(inColor, color, intensity);
        color.a *= 1 - glow * intensity;
    }
    
    return color;
}

half4 apply_edge_color_filter(const half4 inColor, const half4 factor, const float intensity)
{
    const float glow = _EdgeColorGlow;
    const int mode = _EdgeColorFilter;
    half4 color = inColor;
    if (mode == 1) // Color.Multiply
    {
        color.rgb = color.rgb * factor.rgb;
    }
    else if (mode == 2) // Color.Additive
    {
        color.rgb = color.rgb + factor.rgb * color.a;
    }
    else if (mode == 3) // Color.Subtractive
    {
        color.rgb = color.rgb - factor.rgb * color.a;
    }
    else if (mode == 4) // Color.Replace
    {
        color.rgb = factor.rgb * color.a;
    }
    else if (mode == 5) // Color.MultiplyLuminance
    {
        color.rgb = (1 + Luminance(color.rgb)) * factor.rgb / 2 * color.a;
    }
    else if (mode == 6) // Color.MultiplyAdditive
    {
        color.rgb = color.rgb * (1 + factor.rgb);
    }
    else if (mode == 7) // Color.HsvModifier
    {
        const float3 hsv = rgb_to_hsv(color.rgb);
        color.rgb = hsv_to_rgb(hsv + factor.rgb) * color.a * factor.a;
        color.a = inColor.a * factor.a;
    }
    else if (mode == 8) // Color.Contrast
    {
        color.rgb = ((color.rgb - 0.5) * (factor.r + 1) + 0.5 + factor.g * 1.5) * color.a * factor.a;
        color.a = color.a * factor.a;
    }

    if (0 < mode)
    {
        color = lerp(inColor, color, intensity);
        color.a *= 1 - glow * intensity;
    }
    
    return color;
}

half4 apply_sampling_filter(float2 uv, const float4 uvMask, const float2 uvLocal, const float isShadow)
{
    #if SAMPLING_BLUR_FAST || SAMPLING_BLUR_MEDIUM || SAMPLING_BLUR_DETAIL
    {
        float intensity = 0 < isShadow ? _ShadowBlurIntensity : _SamplingIntensity;
        if (0 < intensity)
        {
    #if SAMPLING_BLUR_FAST
            const int KERNEL_SIZE = 5;
            const float KERNEL_[5] = {0.2486, 0.7046, 1.0, 0.7046, 0.2486};
    #elif SAMPLING_BLUR_MEDIUM
            const int KERNEL_SIZE = 9;
            const float KERNEL_[9] = { 0.0438, 0.1719, 0.4566, 0.8204, 1.0, 0.8204, 0.4566, 0.1719, 0.0438};
    #elif SAMPLING_BLUR_DETAIL
            const int KERNEL_SIZE = 13;
            const float KERNEL_[13] = { 0.0438, 0.1138, 0.2486, 0.4566, 0.7046, 0.9141, 1.0, 0.9141, 0.7046, 0.4566, 0.2486, 0.1138, 0.0438};
    #endif

            float4 o = 0;
            float sum = 0;
            float2 shift = 0;
            const half2 blur = texel_size() * intensity * 2;
            for (int x = 0; x < KERNEL_SIZE; x++)
            {
                shift.x = blur.x * (float(x) - KERNEL_SIZE / 2);
                for (int y = 0; y < KERNEL_SIZE; y++)
                {
                    shift.y = blur.y * (float(y) - KERNEL_SIZE / 2);
                    const float2 bluredUv = uv + shift;
                    const float weight = KERNEL_[x] * KERNEL_[y]
                        * step(uvMask.x, bluredUv.x) * step(bluredUv.x, uvMask.z)
                        * step(uvMask.y, bluredUv.y) * step(bluredUv.y, uvMask.w);
                    o += UIEFFECT_SAMPLE(bluredUv) * weight;
                    sum += weight;
                }
            }

            return 0 < sum ? o / sum : 0;
        }
    }
    #endif

    return UIEFFECT_SAMPLE_CLAMP(uv, uvMask);
}

float2 move_transition_filter(const float4 uvMask, const float alpha)
{
    // Transition.Melt/Burn
    #if !TRANSITION_MELT && !TRANSITION_BURN
    {
        return 0;
    }
    #endif

    const float factor = alpha - transition_rate() * (1 + _TransitionWidth * 1.5) + _TransitionWidth;
    const float band = max(0, _TransitionWidth - factor);

    #if TRANSITION_MELT
    {
        return float2(0, +band * band * (uvMask.w - uvMask.y) / max(0.01, _TransitionWidth));
    }
    #elif TRANSITION_BURN
    {
        return float2(0, -band * band * (uvMask.w - uvMask.y) / max(0.01, _TransitionWidth));
    }
    #endif

    return 0;
}

half4 apply_transition_filter(half4 color, const float alpha, const float2 uvLocal, float edgeFactor)
{
    #if TRANSITION_FADE  // Transition.Fade
    {
        color *= saturate(alpha + 1 - transition_rate() * 2);
    }
    #elif TRANSITION_CUTOFF  // Transition.Cutoff
    {
        color *= step(0.001, alpha - transition_rate());
    }
    #elif TRANSITION_PATTERN  // Transition.Pattern
    {
        const half4 patternColor = apply_transition_color_filter(half4(color.rgb, 1), half4(_TransitionColor.rgb * color.a, 1), _TransitionColor.a);
        float isPattern = min(inv_lerp(_TransitionRange.x, _TransitionRange.y, uvLocal.x), 0.995) < (_TransitionPatternReverse ? alpha : 1 - alpha);
        isPattern = _TransitionPatternReverse ? isPattern : 1 - isPattern;
        const float patternFactor = _PatternArea == 0 ? 1 : _PatternArea == 1 ? 1 - edgeFactor : edgeFactor;
        color.rgb = lerp(color.rgb, patternColor.rgb, patternFactor * isPattern);
    }
    // Transition.Dissolve/Shiny/ShinyOnly/Melt
    #elif TRANSITION_DISSOLVE || TRANSITION_SHINY || TRANSITION_MASK || TRANSITION_MELT || TRANSITION_BURN
    {
        const float factor = alpha - transition_rate() * (1 + _TransitionWidth) + _TransitionWidth;
        const float softness = max(0.0001, _TransitionWidth * _TransitionSoftness);
        const half bandLerp = saturate((_TransitionWidth - factor) * 1 / softness);
        const half softLerp = saturate(factor * 2 / softness);
        half4 bandColor = apply_transition_color_filter(half4(color.rgb, 1), half4(_TransitionColor.rgb, 1),
                                                        _TransitionColor.a);
        bandColor *= color.a;

        #if TRANSITION_MELT
        {
            color = lerp(color, bandColor, bandLerp);
            return color;
        }
        #elif TRANSITION_BURN
        {
            color = lerp(color, bandColor, bandLerp * 1.25);
            color.a *= 1 - inv_lerp(0.85, 1.0, bandLerp * 1.25);
            color.rgb *= (1 - inv_lerp(0.85, 1.0, bandLerp * 1.3)) * color.a;
            return color;
        }
        #elif TRANSITION_SHINY && UIEFFECT_TEXTMESHPRO
        {
            color.rgb *= color.a;
        }
        #endif

        color = lerp(color, bandColor, bandLerp * softLerp);

        #if TRANSITION_DISSOLVE
        {
            color *= softLerp;
        }
        #elif TRANSITION_MASK
        {
            color *= bandLerp * softLerp;
        }
        #endif
    }
    #endif

    return color;
}

half4 apply_gradation_filter(const half4 inColor, const float2 uvGrad)
{
    const float2 uv = uvGrad * _GradationTex_ST.x + _GradationTex_ST.z;
    half4 factor = inColor;

    #if GRADATION_GRADIENT // Gradation.Gradient
    {
        factor = tex2D(_GradationTex, uv);
    }
    #elif GRADATION_RADIAL // Gradation.Radial
    {
        float t = saturate(length((uvGrad - float2(0.5, 0.5)) * 2 * _GradationTex_ST.x) + _GradationTex_ST.z);
        factor = lerp(_GradationColor1, _GradationColor2, t);
    }
    #elif GRADATION_COLOR2 // Gradation.Color2
    {
        factor = lerp(_GradationColor1, _GradationColor2, saturate(uv.x));
    }
    #elif GRADATION_COLOR4 // Gradation.Color4
    {
        factor = lerp(
            lerp(_GradationColor3, _GradationColor4, saturate(uv.x)),
            lerp(_GradationColor1, _GradationColor2, saturate(uv.x)),
            saturate(uv.y));
    }
    #else
    {
        return inColor;
    }
    #endif

    const int mode = _GradationColorFilter;
    const float intensity = _GradationIntensity;
    half4 color = inColor;
    if (mode == 1) // Color.Multiply
    {
        color.rgb = color.rgb * factor.rgb;
    }
    else if (mode == 2) // Color.Additive
    {
        color.rgb = color.rgb + factor.rgb * color.a;
    }
    else if (mode == 3) // Color.Subtractive
    {
        color.rgb = color.rgb - factor.rgb * color.a;
    }
    else if (mode == 4) // Color.Replace
    {
        color.rgb = factor.rgb * color.a;
    }
    else if (mode == 5) // Color.MultiplyLuminance
    {
        color.rgb = (1 + Luminance(color.rgb)) * factor.rgb / 2 * color.a;
    }
    else if (mode == 6) // Color.MultiplyAdditive
    {
        color.rgb = color.rgb * (1 + factor.rgb);
    }

    if (0 < mode)
    {
        color = lerp(inColor, color, intensity);
    }
    
    return color;
}

half4 apply_detail_filter(half4 color, float2 uvLocal)
{
    const half4 inColor = color;
    const float2 uv = uvLocal * _DetailTex_ST.xy + _DetailTex_ST.zw + _Time.y * _DetailTex_Speed;
    const half4 detail = tex2D(_DetailTex, uv);
    #if DETAIL_MASKING // Detail.Masking
    {
        color *= inv_lerp(_DetailThreshold.x, _DetailThreshold.y, detail.a);
    }
    #elif DETAIL_MULTIPLY // Detail.Multiply
    {
        color.rgb *= detail.rgb;
        color = lerp(inColor, color * detail.a, _DetailIntensity);

    }
    #elif DETAIL_ADDITIVE // Detail.Additive
    {
        color.rgb += detail.rgb * detail.a * color.a;
        color = lerp(inColor, color, _DetailIntensity);

    }
    #elif DETAIL_REPLACE // Detail.Replace
    {
        color.rgb = detail.rgb * color.a;
        color = lerp(inColor, color, _DetailIntensity * detail.a);

    }
    #elif DETAIL_MULTIPLY_ADDITIVE // Detail.MultiplyAdditive
    {
        color.rgb *= (1 + detail.rgb * detail.a);
        color = lerp(inColor, color, _DetailIntensity);

    }
    #else
    {
        return color;
    }
    #endif

    return color;
}

half to_value(float4 c)
{
    #if SAMPLING_EDGE_LUMINANCE
    return Luminance(c) * c.a;
    #elif SAMPLING_EDGE_ALPHA
    return c.a;
    #endif
    return 0;
}

float edge(float2 uv, const float4 uvMask, float width)
{
    #if EDGE_PLAIN || EDGE_SHINY
    const float2 d = texel_size() * lerp(1, 20, width);
    float e = 1.0;

    // Minimum alpha around the current pixel (12 neighbors)
    e = min(e, (TEX_SAMPLE_CLAMP((uv + d * float2(1.0, 0.0)), uvMask)).a);
    e = min(e, (TEX_SAMPLE_CLAMP((uv + d * float2(0.866025, 0.5)), uvMask)).a);
    e = min(e, (TEX_SAMPLE_CLAMP((uv + d * float2(0.5, 0.866025)), uvMask)).a);
    e = min(e, (TEX_SAMPLE_CLAMP((uv + d * float2(0.0, 1.0)), uvMask)).a);
    e = min(e, (TEX_SAMPLE_CLAMP((uv + d * float2(-0.5, 0.866025)), uvMask)).a);
    e = min(e, (TEX_SAMPLE_CLAMP((uv + d * float2(-0.866025, 0.5)), uvMask)).a);
    e = min(e, (TEX_SAMPLE_CLAMP((uv + d * float2(-1.0, 0.0)), uvMask)).a);
    e = min(e, (TEX_SAMPLE_CLAMP((uv + d * float2(-0.866025, -0.5)), uvMask)).a);
    e = min(e, (TEX_SAMPLE_CLAMP((uv + d * float2(-0.5, -0.866025)), uvMask)).a);
    e = min(e, (TEX_SAMPLE_CLAMP((uv + d * float2(0.0, -1.0)), uvMask)).a);
    e = min(e, (TEX_SAMPLE_CLAMP((uv + d * float2(0.5, -0.866025)), uvMask)).a);
    e = min(e, (TEX_SAMPLE_CLAMP((uv + d * float2(0.866025, -0.5)), uvMask)).a);
    
    return 1 - inv_lerp(0.15, 0.3, e);
    #else
    return 0;
    #endif
}

float is_edge_shiny(const float2 uvLocal)
{
    #if EDGE_SHINY
    const float deg = atan2(uvLocal.y - 0.5, uvLocal.x - 0.5) / UNITY_PI;
    return frac(_EdgeShinyRate + _Time.y * _EdgeShinyAutoPlaySpeed + deg) < _EdgeShinyWidth;
    #else
    return 1;
    #endif
}

half4 uieffect_internal(float2 uv, float4 uvMask, const float2 uvLocal, const float2 uvGrad, const float isShadow)
{
    const half alpha = transition_alpha(uvLocal);
    const float edgeFactor = edge(uv, uvMask, _EdgeWidth);
    uv += move_transition_filter(uvMask, alpha);
    half4 color = apply_sampling_filter(uv, uvMask, uvLocal, isShadow);
    color = apply_gradation_filter(color, uvGrad);
    color = apply_tone_filter(color);
    color = apply_transition_filter(color, alpha, uvLocal, edgeFactor);
    color = apply_detail_filter(color, uvLocal);

    if (0 < isShadow)
    {
        return apply_shadow_color_filter(color, _ShadowColor, 1);
    }
    else
    {
        #if EDGE_PLAIN || EDGE_SHINY
        {
            color = apply_color_filter(color, _ColorValue, _ColorIntensity);
            const half4 edgeColor = apply_edge_color_filter(color, _EdgeColor, 1);
            const float isEdgeShiny = is_edge_shiny(uvLocal);
            return lerp(color, edgeColor, edgeFactor * isEdgeShiny);
        }
        #else
        {
            return apply_color_filter(color, _ColorValue, _ColorIntensity);
        }
        #endif
    }
}

half4 uieffect(half4 origin, float2 uv, float4 uvMask, float4 wpos)
{
    #ifdef NO_UIEFFECT
    return origin;
    #endif

    #ifndef CANVAS_SHADERGRAPH
    wpos = mul(unity_ObjectToWorld, wpos);
    #endif

    #if !UIEFFECT_EDITOR
    wpos = mul(_CanvasToWorldMatrix, wpos);
    #endif
    const half rate = get_target_rate(origin.rgb);
    const float2 uvLocal = saturate(mul(_RootViewMatrix, wpos)).xy;
    const float2 uvGrad = mul(_GradViewMatrix, wpos).xy;
    const float isShadow = uvMask.x < 0 ? 1 : 0;
    uvMask.x += isShadow * 2;

    // Sampling.Pixelation
    #if SAMPLING_PIXELATION
    {
        const half2 pixelSize = max(2, (1 - lerp(0.5, 0.95, _SamplingIntensity)) / texel_size());
        uv = round(uv * pixelSize) / pixelSize;
    }
    // Sampling.RgbShift
    #elif SAMPLING_RGB_SHIFT
    {
        const half2 offset = half2(_SamplingIntensity * texel_size().x * 20, 0);
        const half2 r = uieffect_internal(uv + offset, uvMask, uvLocal, uvGrad, isShadow).ra;
        const half2 g = uieffect_internal(uv, uvMask, uvLocal, uvGrad, isShadow).ga;
        const half2 b = uieffect_internal(uv - offset, uvMask, uvLocal, uvGrad, isShadow).ba;
        return half4(r.x * r.y, g.x * g.y, b.x * b.y, (r.y + g.y + b.y) / 3);
    }
    // Sampling.EdgeLuminance/EdgeAlpha
    #elif SAMPLING_EDGE_LUMINANCE || SAMPLING_EDGE_ALPHA
    {
        // Pixel size
        const float2 d = texel_size() * _SamplingWidth;

        // Pixel values around the current pixel (3x3, 8 neighbors)
        const half v00 = to_value(UIEFFECT_SAMPLE_CLAMP((uv + half2(-d.x, -d.y)), uvMask));
        const half v01 = to_value(UIEFFECT_SAMPLE_CLAMP((uv + half2(-d.x, 0.0)), uvMask));
        const half v02 = to_value(UIEFFECT_SAMPLE_CLAMP((uv + half2(-d.x, +d.y)), uvMask));
        const half v10 = to_value(UIEFFECT_SAMPLE_CLAMP((uv + half2(0.0, -d.y)), uvMask));
        const half v12 = to_value(UIEFFECT_SAMPLE_CLAMP((uv + half2(0.0, +d.y)), uvMask));
        const half v20 = to_value(UIEFFECT_SAMPLE_CLAMP((uv + half2(+d.x, -d.y)), uvMask));
        const half v21 = to_value(UIEFFECT_SAMPLE_CLAMP((uv + half2(+d.x, 0.0)), uvMask));
        const half v22 = to_value(UIEFFECT_SAMPLE_CLAMP((uv + half2(+d.x, +d.y)), uvMask));

        // Apply Sobel operator
        half sobel_h = v00 * -1.0 + v01 * -2.0 + v02 * -1.0 + v20 * 1.0 + v21 * 2.0 + v22 * 1.0;
        half sobel_v = v00 * -1.0 + v10 * -2.0 + v20 * -1.0 + v02 * 1.0 + v12 * 2.0 + v22 * 1.0;

        const half sobel = sqrt(sobel_h * sobel_h + sobel_v * sobel_v) * _SamplingIntensity;
        return lerp(0, uieffect_internal(uv, uvMask, uvLocal, uvGrad, isShadow), inv_lerp(0.5, 1, sobel));
    }
    #endif

    return lerp(origin, uieffect_internal(uv, uvMask, uvLocal, uvGrad, isShadow), rate);
}


half4 uieffect(float2 uv, float4 uvMask, float4 wpos)
{
    const half4 origin = UIEFFECT_SAMPLE_CLAMP(uv, uvMask);
    return uieffect(origin, uv, uvMask, wpos);
}

#endif // UI_EFFECT_INCLUDED
