Shader "Hidden/UI/Default (UIEffect)"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend [_SrcBlend] [_DstBlend]
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            #pragma multi_compile_local_fragment _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local_fragment _ UNITY_UI_ALPHACLIP

            // ==== UIEFFECT START ====
            #pragma shader_feature_local_fragment _ TONE_GRAYSCALE TONE_SEPIA TONE_NEGATIVE TONE_RETRO TONE_POSTERIZE
            #pragma shader_feature_local_fragment _ COLOR_MULTIPLY COLOR_ADDITIVE COLOR_SUBTRACTIVE COLOR_REPLACE COLOR_MULTIPLY_LUMINANCE COLOR_MULTIPLY_ADDITIVE COLOR_HSV_MODIFIER COLOR_CONTRAST
            #pragma shader_feature_local_fragment _ SAMPLING_BLUR_FAST SAMPLING_BLUR_MEDIUM SAMPLING_BLUR_DETAIL SAMPLING_PIXELATION SAMPLING_RGB_SHIFT SAMPLING_EDGE_LUMINANCE SAMPLING_EDGE_ALPHA
            #pragma shader_feature_local_fragment _ TRANSITION_FADE TRANSITION_CUTOFF TRANSITION_DISSOLVE TRANSITION_SHINY TRANSITION_MASK TRANSITION_MELT TRANSITION_BURN
            #pragma shader_feature_local_fragment _ TRANSITION_COLOR_MULTIPLY TRANSITION_COLOR_ADDITIVE TRANSITION_COLOR_SUBTRACTIVE TRANSITION_COLOR_REPLACE TRANSITION_COLOR_MULTIPLY_LUMINANCE TRANSITION_COLOR_MULTIPLY_ADDITIVE TRANSITION_COLOR_HSV_MODIFIER TRANSITION_COLOR_CONTRAST
            #pragma shader_feature_local_fragment _ TARGET_HUE TARGET_LUMINANCE
            // ==== UIEFFECT END ====

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float4 texcoord : TEXCOORD0;
                // ==== UIEFFECT START ====
                float4 uvMask : TEXCOORD1;
                // ==== UIEFFECT END ====
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4 mask : TEXCOORD2;
                // ==== UIEFFECT START ====
                float4 uvMask : TEXCOORD3;
                float2 uvLocal : TEXCOORD4;
                // ==== UIEFFECT END ====
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;
            int _UIVertexColorAlwaysGammaSpace;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                float4 vPosition = UnityObjectToClipPos(v.vertex);
                OUT.worldPosition = v.vertex;
                OUT.vertex = vPosition;

                float2 pixelSize = vPosition.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                OUT.mask = float4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw,
                                  0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

                if (_UIVertexColorAlwaysGammaSpace)
                {
                    if (!IsGammaSpace())
                    {
                        v.color.rgb = GammaToLinearSpace(v.color.rgb);
                    }
                }

                OUT.color = v.color * _Color;
                OUT.uvMask = v.uvMask;
                OUT.uvLocal = v.texcoord.zw;
                return OUT;
            }

            // ==== UIEFFECT START ====
            v2f _fragInput;
            fixed4 uieffect_frag(float2 uv)
            {
                v2f IN = _fragInput;
                //Round up the alpha color coming from the interpolator (to 1.0/256.0 steps)
                //The incoming alpha could have numerical instability, which makes it very sensible to
                //HDR color transparency blend, when it blends with the world's texture.
                const half alphaPrecision = half(0xff);
                const half invAlphaPrecision = half(1.0 / alphaPrecision);
                IN.color.a = round(IN.color.a * alphaPrecision) * invAlphaPrecision;

                half4 color = IN.color * (tex2D(_MainTex, uv) + _TextureSampleAdd);
                color.rgb *= color.a;
                return color;
            }

            #include "Packages/com.coffee.ui-effect/Shaders/UIEffect.cginc"
            // ==== UIEFFECT END ====

            half4 frag(v2f IN) : SV_Target
            {
                _fragInput = IN;
                half4 c = uieffect(_fragInput.texcoord, _fragInput.uvMask, _fragInput.uvLocal);

                #ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                c *= m.x * m.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (c.a - 0.001);
                #endif

                return c;
            }
            ENDCG
        }
    }
}