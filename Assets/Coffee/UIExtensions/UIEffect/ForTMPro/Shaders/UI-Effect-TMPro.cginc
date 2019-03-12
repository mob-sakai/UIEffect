#ifndef UI_EFFECT_TMPRO_INCLUDED
#define UI_EFFECT_TMPRO_INCLUDED

// Used by Unity internally to handle Texture Tiling and Offset.
float4 _FaceTex_ST;
float4 _OutlineTex_ST;


#if MOBILE
#define UV(x) x.texcoord0
#define UV2(x) x.texcoord1

struct vertex_t {
	float4	vertex			: POSITION;
	float3	normal			: NORMAL;
	fixed4	color			: COLOR;
	float2	texcoord0		: TEXCOORD0;
	float2	texcoord1		: TEXCOORD1;
#if EX
	float2	uvMask			: TEXCOORD2;
#endif
};

struct pixel_t {
	float4	vertex			: SV_POSITION;
	fixed4	faceColor		: COLOR;
	fixed4	outlineColor	: COLOR1;
	float4	texcoord0		: TEXCOORD0;			// Texture UV, Mask UV
	half4	param			: TEXCOORD1;			// Scale(x), BiasIn(y), BiasOut(z), Bias(w)
	half4	mask			: TEXCOORD2;			// Position in clip space(xy), Softness(zw)
#if (UNDERLAY_ON | UNDERLAY_INNER)
	float4	texcoord1		: TEXCOORD3;			// Texture UV, alpha, reserved
	half2	underlayParam	: TEXCOORD4;			// Scale(x), Bias(y)
#endif
	fixed4	color			: COLOR2;
#if UI_DISSOLVE || UI_TRANSITION
	half3	eParam	: TEXCOORD5;
#elif UI_SHINY
	half2	eParam	: TEXCOORD5;
#else
	half	eParam	: TEXCOORD5;
#endif
#if EX
	half4	uvMask			: TEXCOORD6;
#endif
};


pixel_t VertShader(vertex_t input)
{
	float bold = step(input.texcoord1.y, 0);

	float4 vert = input.vertex;
	vert.x += _VertexOffsetX;
	vert.y += _VertexOffsetY;
	float4 vPosition = UnityObjectToClipPos(vert);

	float2 pixelSize = vPosition.w;
	pixelSize /= float2(_ScaleX, _ScaleY) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));
	
	float scale = rsqrt(dot(pixelSize, pixelSize));
	scale *= abs(input.texcoord1.y) * _GradientScale * 1.5;
	if(UNITY_MATRIX_P[3][3] == 0) scale = lerp(abs(scale) * (1 - _PerspectiveFilter), scale, abs(dot(UnityObjectToWorldNormal(input.normal.xyz), normalize(WorldSpaceViewDir(vert)))));

	float weight = lerp(_WeightNormal, _WeightBold, bold) / 4.0;
	weight = (weight + _FaceDilate) * _ScaleRatioA * 0.5;

	float layerScale = scale;

	scale /= 1 + (_OutlineSoftness * _ScaleRatioA * scale);
	float bias = (0.5 - weight) * scale - 0.5;
	float outline = _OutlineWidth * _ScaleRatioA * 0.5 * scale;

//	float opacity = input.color.a;
//#if (UNDERLAY_ON | UNDERLAY_INNER)
//		opacity = 1.0;
//#endif

	fixed4 faceColor = input.color * _FaceColor;
//	fixed4 faceColor = fixed4(input.color.rgb, opacity) * _FaceColor;
//	faceColor.rgb *= faceColor.a;

	fixed4 outlineColor = _OutlineColor;
//	outlineColor.a *= opacity;
//	outlineColor.rgb *= outlineColor.a;
	outlineColor = lerp(faceColor, outlineColor, sqrt(min(1.0, (outline * 2))));

#if (UNDERLAY_ON | UNDERLAY_INNER)

	layerScale /= 1 + ((_UnderlaySoftness * _ScaleRatioC) * layerScale);
	float layerBias = (.5 - weight) * layerScale - .5 - ((_UnderlayDilate * _ScaleRatioC) * .5 * layerScale);

	float x = -(_UnderlayOffsetX * _ScaleRatioC) * _GradientScale / _TextureWidth;
	float y = -(_UnderlayOffsetY * _ScaleRatioC) * _GradientScale / _TextureHeight;
	float2 layerOffset = float2(x, y);
#endif

	// Generate UV for the Masking Texture
	float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
	float2 maskUV = (vert.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);

	// Structure for pixel shader
	#if UI_DISSOLVE || UI_TRANSITION
	half3 param = UnpackToVec3(input.texcoord0.y);
	#elif UI_SHINY
	half2 param = UnpackToVec2(input.texcoord0.y);
	#else
	half param = input.texcoord0.y;
	#endif

	#if UI_EFFECT
	input.texcoord0 = UnpackToVec2(input.texcoord0.x) * 2 - 0.5;
	#else
	input.texcoord0 = UnpackToVec2(input.texcoord0.x);
	#endif
	pixel_t output = {
		vPosition,
		faceColor,
		outlineColor,
		float4(input.texcoord0, maskUV.x, maskUV.y),
		half4(scale, bias - outline, bias + outline, bias),
		half4(vert.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_MaskSoftnessX, _MaskSoftnessY) + pixelSize.xy)),
	#if (UNDERLAY_ON | UNDERLAY_INNER)
		float4(input.texcoord0 + layerOffset, input.color.a, 0),
		half2(layerScale, layerBias),
	#endif
		input.color,
		param,
	#if EX
		half4(UnpackToVec2(input.uvMask.x), UnpackToVec2(input.uvMask.y)),
	#endif
	};

	return output;
}


// PIXEL SHADER
fixed4 PixShader(pixel_t input) : SV_Target
{
	half d = tex2D(_MainTex, input.texcoord0.xy).a * input.param.x;
	half4 c = input.faceColor * saturate(d - input.param.w);

#ifdef OUTLINE_ON
	c = lerp(input.outlineColor, input.faceColor, saturate(d - input.param.z));
	c *= saturate(d - input.param.y);
#endif

#if UNDERLAY_ON
	d = tex2D(_MainTex, input.texcoord1.xy).a * input.underlayParam.x;
	c += float4(_UnderlayColor.rgb * _UnderlayColor.a, _UnderlayColor.a) * saturate(d - input.underlayParam.y) * (1 - c.a);
#endif

#if UNDERLAY_INNER
	half sd = saturate(d - input.param.z);
	d = tex2D(_MainTex, input.texcoord1.xy).a * input.underlayParam.x;
	c += float4(_UnderlayColor.rgb * _UnderlayColor.a, _UnderlayColor.a) * (1 - saturate(d - input.underlayParam.y)) * sd * (1 - c.a);
#endif

// Alternative implementation to UnityGet2DClipping with support for softness.
#if UNITY_UI_CLIP_RECT
	half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(input.mask.xy)) * input.mask.zw);
	c *= m.x * m.y;
#endif

#if (UNDERLAY_ON | UNDERLAY_INNER)
//	c *= input.texcoord1.z;
#endif

// Dissolve
//c = ApplyTransitionEffect(c, input.eParam);
//c.rgb *= c.a;

#if UNITY_UI_ALPHACLIP
	clip(c.a - 0.001);
#endif

	return c;
}

#else
#define UV(x) x.atlas
#define UV2(x) x.texcoord2

struct vertex_t {
	float4	position		: POSITION;
	float3	normal			: NORMAL;
	fixed4	color			: COLOR;
	float2	texcoord0		: TEXCOORD0;
	float2	texcoord1		: TEXCOORD1;
#if EX
	float2	uvMask			: TEXCOORD2;
#endif
};


struct pixel_t {
	float4	position		: SV_POSITION;
	fixed4	color			: COLOR;
	float2	atlas			: TEXCOORD0;		// Atlas
	float4	param			: TEXCOORD1;		// alphaClip, scale, bias, weight
	float4	mask			: TEXCOORD2;		// Position in object space(xy), pixel Size(zw)
	float3	viewDir			: TEXCOORD3;
	
#if (UNDERLAY_ON || UNDERLAY_INNER)
	float4	texcoord2		: TEXCOORD4;		// u,v, scale, bias
	fixed4	underlayColor	: COLOR1;
#endif
	float4 textures			: TEXCOORD5;
#if UI_DISSOLVE || UI_TRANSITION
	half3	eParam	: TEXCOORD6;
#elif UI_SHINY
	half2	eParam	: TEXCOORD6;
#else
	half	eParam	: TEXCOORD6;
#endif
#if EX
	half4	uvMask			: TEXCOORD7;
#endif
};

pixel_t VertShader(vertex_t input)
{
	float bold = step(input.texcoord1.y, 0);

	float4 vert = input.position;
	vert.x += _VertexOffsetX;
	vert.y += _VertexOffsetY;

	float4 vPosition = UnityObjectToClipPos(vert);

	float2 pixelSize = vPosition.w;
	pixelSize /= float2(_ScaleX, _ScaleY) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));
	float scale = rsqrt(dot(pixelSize, pixelSize));
	scale *= abs(input.texcoord1.y) * _GradientScale * 1.5;
	if (UNITY_MATRIX_P[3][3] == 0) scale = lerp(abs(scale) * (1 - _PerspectiveFilter), scale, abs(dot(UnityObjectToWorldNormal(input.normal.xyz), normalize(WorldSpaceViewDir(vert)))));

	float weight = lerp(_WeightNormal, _WeightBold, bold) / 4.0;
	weight = (weight + _FaceDilate) * _ScaleRatioA * 0.5;

	float bias =(.5 - weight) + (.5 / scale);

	float alphaClip = (1.0 - _OutlineWidth*_ScaleRatioA - _OutlineSoftness*_ScaleRatioA);

#if GLOW_ON
	alphaClip = min(alphaClip, 1.0 - _GlowOffset * _ScaleRatioB - _GlowOuter * _ScaleRatioB);
#endif

	alphaClip = alphaClip / 2.0 - ( .5 / scale) - weight;

#if (UNDERLAY_ON || UNDERLAY_INNER)
	float4 underlayColor = _UnderlayColor;
	underlayColor.rgb *= underlayColor.a;

	float bScale = scale;
	bScale /= 1 + ((_UnderlaySoftness*_ScaleRatioC) * bScale);
	float bBias = (0.5 - weight) * bScale - 0.5 - ((_UnderlayDilate * _ScaleRatioC) * 0.5 * bScale);

	float x = -(_UnderlayOffsetX * _ScaleRatioC) * _GradientScale / _TextureWidth;
	float y = -(_UnderlayOffsetY * _ScaleRatioC) * _GradientScale / _TextureHeight;
	float2 bOffset = float2(x, y);
#endif

	// Generate UV for the Masking Texture
	float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
	float2 maskUV = (vert.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);

	// Support for texture tiling and offset
	float2 textureUV = UnpackUV(input.texcoord1.x);
	float2 faceUV = TRANSFORM_TEX(textureUV, _FaceTex);
	float2 outlineUV = TRANSFORM_TEX(textureUV, _OutlineTex);

	#if UI_DISSOLVE || UI_TRANSITION
	half3 param = UnpackToVec3(input.texcoord0.y);
	#elif UI_SHINY
	half2 param = UnpackToVec2(input.texcoord0.y);
	#else
	half param = input.texcoord0.y;
	#endif

	#if UI_EFFECT
	input.texcoord0 = UnpackToVec2(input.texcoord0.x) * 2 - 0.5;
	#else
	input.texcoord0 = UnpackToVec2(input.texcoord0.x);
	#endif
	pixel_t output = {
		vPosition,
		input.color,
		input.texcoord0,
		float4(alphaClip, scale, bias, weight),
		half4(vert.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_MaskSoftnessX, _MaskSoftnessY) + pixelSize.xy)),
		mul((float3x3)_EnvMatrix, _WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, vert).xyz),
	#if (UNDERLAY_ON || UNDERLAY_INNER)
		float4(input.texcoord0 + bOffset, bScale, bBias),
		underlayColor,
	#endif
		float4(faceUV, outlineUV),
		param,
	#if EX
		half4(UnpackToVec2(input.uvMask.x), UnpackToVec2(input.uvMask.y)),
	#endif
	};

	return output;
}


fixed4 PixShader(pixel_t input)// : SV_Target
{
	float c = tex2D(_MainTex, input.atlas).a;

//		#ifndef UNDERLAY_ON
//			clip(c - input.param.x);
//		#endif

	float	scale	= input.param.y;
	float	bias	= input.param.z;
	float	weight	= input.param.w;
	float	sd = (bias - c) * scale;

	float outline = (_OutlineWidth * _ScaleRatioA) * scale;
	float softness = (_OutlineSoftness * _ScaleRatioA) * scale;

	half4 faceColor = _FaceColor;
	half4 outlineColor = _OutlineColor;

//			faceColor.rgb *= input.color.rgb;
	
	faceColor *= tex2D(_FaceTex, input.textures.xy + float2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.y);
	outlineColor *= tex2D(_OutlineTex, input.textures.zw + float2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.y);

	faceColor = GetColor(sd, faceColor, outlineColor, outline, softness);

#if BEVEL_ON
	float3 dxy = float3(0.5 / _TextureWidth, 0.5 / _TextureHeight, 0);
	float3 n = GetSurfaceNormal(input.atlas, weight, dxy);

	float3 bump = UnpackNormal(tex2D(_BumpMap, input.textures.xy + float2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.y)).xyz;
	bump *= lerp(_BumpFace, _BumpOutline, saturate(sd + outline * 0.5));
	n = normalize(n- bump);

	float3 light = normalize(float3(sin(_LightAngle), cos(_LightAngle), -1.0));

	float3 col = GetSpecular(n, light);
	faceColor.rgb += col*faceColor.a;
	faceColor.rgb *= 1-(dot(n, light)*_Diffuse);
	faceColor.rgb *= lerp(_Ambient, 1, n.z*n.z);

	fixed4 reflcol = texCUBE(_Cube, reflect(input.viewDir, -n));
	faceColor.rgb += reflcol.rgb * lerp(_ReflectFaceColor.rgb, _ReflectOutlineColor.rgb, saturate(sd + outline * 0.5)) * faceColor.a;
#endif

#if UNDERLAY_ON
	float d = tex2D(_MainTex, input.texcoord2.xy).a * input.texcoord2.z;
	faceColor += input.underlayColor * saturate(d - input.texcoord2.w) * (1 - faceColor.a);
#endif

#if UNDERLAY_INNER
	float d = tex2D(_MainTex, input.texcoord2.xy).a * input.texcoord2.z;
	faceColor += input.underlayColor * (1 - saturate(d - input.texcoord2.w)) * saturate(1 - sd) * (1 - faceColor.a);
#endif

#if GLOW_ON
	float4 glowColor = GetGlowColor(sd, scale);
	faceColor.rgb += glowColor.rgb * glowColor.a;
#endif

// Alternative implementation to UnityGet2DClipping with support for softness.
#if UNITY_UI_CLIP_RECT
	half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(input.mask.xy)) * input.mask.zw);
	faceColor *= m.x * m.y;
#endif

//#if UNITY_UI_ALPHACLIP
//	clip(faceColor.a - 0.001);
//#endif

	return faceColor;
}

#endif

#if defined(UI_BLUR)
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
    #if UNDERLAY_ON
	half2 texcood2 = UV2(IN);
    #endif
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
            
            #if UNDERLAY_ON
            UV2(IN).xy = texcood2 + shift;
            #endif

			#if EX
			fixed masked = min(mask.x <= uv.x, uv.x <= mask.z) * min(mask.y <= uv.y, uv.y <= mask.w);
			o += lerp(fixed4(0, 0, 0, 0), PixShader(IN) * weight, masked);
			#else
			o += PixShader(IN) * weight;
			#endif
		}
	}
	IN.color.a = alpha;
	return o / sum;// * alpha;
}

fixed4 Tex2DBlurring (pixel_t IN, half2 blur)
{
	return Tex2DBlurring(IN, blur, half4(0,0,1,1));
}
#endif



#endif // UI_EFFECT_TMPRO_INCLUDED
