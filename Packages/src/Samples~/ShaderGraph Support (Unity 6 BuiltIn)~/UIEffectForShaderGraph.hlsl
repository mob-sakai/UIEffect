// ==== UIEFFECT START ====
SurfaceDescriptionInputs _fragInput;
half4 uieffect_frag(float2 uv)
{
    float2 prevUv = _fragInput.uv0.xy;
    _fragInput.uv0.xy = uv;
    SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(_fragInput);
    _fragInput.uv0.xy = prevUv;

    half alpha = surfaceDescription.Alpha;
    return half4(surfaceDescription.BaseColor * alpha + surfaceDescription.Emission, alpha);
}

#include "Packages/com.coffee.ui-effect/Shaders/UIEffect.cginc"
// ==== UIEFFECT END ====


PackedVaryings vert_override(Attributes input)
{
    Varyings output = BuildVaryings(input);
    output.texCoord2 = input.uv1;
    PackedVaryings packedOutput = PackVaryings(output);
    return packedOutput;
}

half4 frag_override(PackedVaryings packedInput) : SV_TARGET
{
    Varyings unpacked = UnpackVaryings(packedInput);
    UNITY_SETUP_INSTANCE_ID(unpacked);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(unpacked);
    SurfaceDescriptionInputs surfaceDescriptionInputs = BuildSurfaceDescriptionInputs(unpacked);
    SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);

    _fragInput = surfaceDescriptionInputs;
    const half4 origin = uieffect_frag(_fragInput.uv0.xy);
    half4 color = uieffect(origin, _fragInput.uv0.xy, unpacked.texCoord2, float4(unpacked.positionWS, 1));
    
    //Round up the alpha color coming from the interpolator (to 1.0/256.0 steps)
    //The incoming alpha could have numerical instability, which makes it very sensible to
    //HDR color transparency blend, when it blends with the world's texture.
    const half alphaPrecision = half(0xff);
    const half invAlphaPrecision = half(1.0/alphaPrecision);
    unpacked.color.a = round(unpacked.color.a * alphaPrecision)*invAlphaPrecision;
    
    #if !defined(HAVE_VFX_MODIFICATION) && !defined(_DISABLE_COLOR_TINT)
    color.rgb *= unpacked.color.rgb;
    color *= unpacked.color.a;
    #endif
    
    #ifdef UNITY_UI_CLIP_RECT
    //mask = Uv2
    half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(unpacked.texCoord1.xy)) * unpacked.texCoord1.zw);
    color *= m.x * m.y;
    #endif
    
    // ==== SOFTMASKABLE START ====
    #if SOFTMASKABLE
    color *= SoftMask(unpacked.positionCS, unpacked.positionWS, color.a);
    #endif
    // ==== SOFTMASKABLE END ====

    #if defined(_ALPHATEST_ON) || defined(_BUILTIN_ALPHATEST_ON)
    clip(color.a - surfaceDescription.AlphaClipThreshold);
    #endif
    
    #if UNITY_UI_ALPHACLIP
    clip(color.a - 0.001);
    #endif
    
    return  color;
}