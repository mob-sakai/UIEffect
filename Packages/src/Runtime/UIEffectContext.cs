using System;
using System.Collections.Generic;
using Coffee.UIEffectInternal;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    public class UIEffectContext
    {
        private static readonly UIEffectContext s_DefaultContext = new UIEffectContext();
        private static readonly List<UIVertex> s_WorkingVertices = new List<UIVertex>(1024 * 8);
        private static readonly int s_SrcBlend = Shader.PropertyToID("_SrcBlend");
        private static readonly int s_DstBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int s_ToneIntensity = Shader.PropertyToID("_ToneIntensity");
        private static readonly int s_ColorFilter = Shader.PropertyToID("_ColorFilter");
        private static readonly int s_ColorValue = Shader.PropertyToID("_ColorValue");
        private static readonly int s_ColorIntensity = Shader.PropertyToID("_ColorIntensity");
        private static readonly int s_ColorGlow = Shader.PropertyToID("_ColorGlow");
        private static readonly int s_SamplingIntensity = Shader.PropertyToID("_SamplingIntensity");
        private static readonly int s_SamplingWidth = Shader.PropertyToID("_SamplingWidth");
        private static readonly int s_SamplingScale = Shader.PropertyToID("_SamplingScale");
        private static readonly int s_TransitionRate = Shader.PropertyToID("_TransitionRate");
        private static readonly int s_TransitionReverse = Shader.PropertyToID("_TransitionReverse");
        private static readonly int s_TransitionTex = Shader.PropertyToID("_TransitionTex");
        private static readonly int s_TransitionTex_ST = Shader.PropertyToID("_TransitionTex_ST");
        private static readonly int s_TransitionTex_Speed = Shader.PropertyToID("_TransitionTex_Speed");
        private static readonly int s_TransitionWidth = Shader.PropertyToID("_TransitionWidth");
        private static readonly int s_TransitionSoftness = Shader.PropertyToID("_TransitionSoftness");
        private static readonly int s_TransitionRange = Shader.PropertyToID("_TransitionRange");
        private static readonly int s_TransitionColorFilter = Shader.PropertyToID("_TransitionColorFilter");
        private static readonly int s_TransitionColor = Shader.PropertyToID("_TransitionColor");
        private static readonly int s_TransitionColorGlow = Shader.PropertyToID("_TransitionColorGlow");
        private static readonly int s_TransitionPatternReverse = Shader.PropertyToID("_TransitionPatternReverse");
        private static readonly int s_TransitionAutoPlaySpeed = Shader.PropertyToID("_TransitionAutoPlaySpeed");
        private static readonly int s_TargetColor = Shader.PropertyToID("_TargetColor");
        private static readonly int s_TargetRange = Shader.PropertyToID("_TargetRange");
        private static readonly int s_TargetSoftness = Shader.PropertyToID("_TargetSoftness");
        private static readonly int s_ShadowColorFilter = Shader.PropertyToID("_ShadowColorFilter");
        private static readonly int s_ShadowColor = Shader.PropertyToID("_ShadowColor");
        private static readonly int s_ShadowBlurIntensity = Shader.PropertyToID("_ShadowBlurIntensity");
        private static readonly int s_ShadowColorGlow = Shader.PropertyToID("_ShadowColorGlow");
        private static readonly int s_EdgeWidth = Shader.PropertyToID("_EdgeWidth");
        private static readonly int s_EdgeColorFilter = Shader.PropertyToID("_EdgeColorFilter");
        private static readonly int s_EdgeColor = Shader.PropertyToID("_EdgeColor");
        private static readonly int s_EdgeColorGlow = Shader.PropertyToID("_EdgeColorGlow");
        private static readonly int s_EdgeShinyAutoPlaySpeed = Shader.PropertyToID("_EdgeShinyAutoPlaySpeed");
        private static readonly int s_EdgeShinyRate = Shader.PropertyToID("_EdgeShinyRate");
        private static readonly int s_EdgeShinyWidth = Shader.PropertyToID("_EdgeShinyWidth");
        private static readonly int s_PatternArea = Shader.PropertyToID("_PatternArea");
        private static readonly int s_DetailIntensity = Shader.PropertyToID("_DetailIntensity");
        private static readonly int s_DetailThreshold = Shader.PropertyToID("_DetailThreshold");
        private static readonly int s_DetailColor = Shader.PropertyToID("_DetailColor");
        private static readonly int s_DetailTex = Shader.PropertyToID("_DetailTex");
        private static readonly int s_DetailTex_ST = Shader.PropertyToID("_DetailTex_ST");
        private static readonly int s_DetailTex_Speed = Shader.PropertyToID("_DetailTex_Speed");
        private static readonly int s_GradationIntensity = Shader.PropertyToID("_GradationIntensity");
        private static readonly int s_GradationColorFilter = Shader.PropertyToID("_GradationColorFilter");
        private static readonly int s_GradationColor1 = Shader.PropertyToID("_GradationColor1");
        private static readonly int s_GradationColor2 = Shader.PropertyToID("_GradationColor2");
        private static readonly int s_GradationColor3 = Shader.PropertyToID("_GradationColor3");
        private static readonly int s_GradationColor4 = Shader.PropertyToID("_GradationColor4");
        private static readonly int s_GradationTex = Shader.PropertyToID("_GradationTex");
        private static readonly int s_GradationTex_ST = Shader.PropertyToID("_GradationTex_ST");
        private static readonly int s_RootViewMatrix = Shader.PropertyToID("_RootViewMatrix");
        private static readonly int s_GradViewMatrix = Shader.PropertyToID("_GradViewMatrix");
        private static readonly int s_MirrorRootViewMatrix = Shader.PropertyToID("_MirrorRootViewMatrix");
        private static readonly int s_MirrorGradViewMatrix = Shader.PropertyToID("_MirrorGradViewMatrix");
        private static readonly int s_CanvasToWorldMatrix = Shader.PropertyToID("_CanvasToWorldMatrix");

        private static readonly string[] s_ToneKeywords =
        {
            "",
            "TONE_GRAYSCALE",
            "TONE_SEPIA",
            "TONE_NEGATIVE",
            "TONE_RETRO",
            "TONE_POSTERIZE"
        };

        private static readonly string[] s_ColorKeywords =
        {
            "",
            "COLOR_FILTER"
        };

        private static readonly string[] s_SamplingKeywords =
        {
            "",
            "SAMPLING_BLUR_FAST",
            "SAMPLING_BLUR_MEDIUM",
            "SAMPLING_BLUR_DETAIL",
            "SAMPLING_PIXELATION",
            "SAMPLING_RGB_SHIFT",
            "SAMPLING_EDGE_LUMINANCE",
            "SAMPLING_EDGE_ALPHA"
        };

        private static readonly string[] s_TransitionKeywords =
        {
            "",
            "TRANSITION_FADE",
            "TRANSITION_CUTOFF",
            "TRANSITION_DISSOLVE",
            "TRANSITION_SHINY",
            "TRANSITION_MASK",
            "TRANSITION_MELT",
            "TRANSITION_BURN",
            "TRANSITION_PATTERN"
        };

        private static readonly string[] s_TargetKeywords =
        {
            "",
            "TARGET_HUE",
            "TARGET_LUMINANCE"
        };

        private static readonly string[] s_EdgeKeywords =
        {
            "",
            "EDGE_PLAIN",
            "EDGE_SHINY"
        };

        private static readonly string[] s_DetailKeywords =
        {
            "",
            "DETAIL_MASKING",
            "DETAIL_MULTIPLY",
            "DETAIL_ADDITIVE",
            "DETAIL_REPLACE",
            "DETAIL_MULTIPLY_ADDITIVE",
            "DETAIL_SUBTRACTIVE"
        };

        private static readonly string[] s_GradationKeywords =
        {
            "",
            "GRADATION_GRADIENT",
            "GRADATION_RADIAL",
            "GRADATION_COLOR2",
            "GRADATION_COLOR4"
        };

        private static readonly Vector2[][] s_ShadowVectors = new[]
        {
            Array.Empty<Vector2>(), // None
            new[] { Vector2.one }, // Shadow
            new[] { Vector2.one, Vector2.right, Vector2.up }, // Shadow3
            new[] { Vector2.one, -Vector2.one, new Vector2(1, -1), new Vector2(-1, 1) }, // Outline
            new[]
            {
                Vector2.one, -Vector2.one, new Vector2(1, -1), new Vector2(-1, 1),
                Vector2.right, Vector2.up, Vector2.left, Vector2.down
            } // Outline8
        };

        public static Func<UIVertex, Rect, UIVertex> onModifyVertex;
        public ToneFilter m_ToneFilter;
        public float m_ToneIntensity;

        public ColorFilter m_ColorFilter;
        public float m_ColorIntensity;
        public Color m_Color;
        public bool m_ColorGlow;

        public SamplingFilter m_SamplingFilter;
        public float m_SamplingIntensity;
        public float m_SamplingWidth;

        public TransitionFilter m_TransitionFilter;
        public float m_TransitionRate;
        public bool m_TransitionReverse;
        public Texture m_TransitionTex;
        public Vector2 m_TransitionTexScale;
        public Vector2 m_TransitionTexOffset;
        public Vector2 m_TransitionTexSpeed;
        public float m_TransitionRotation;
        public bool m_TransitionKeepAspectRatio;
        public float m_TransitionWidth;
        public float m_TransitionSoftness;
        public MinMax01 m_TransitionRange;
        public ColorFilter m_TransitionColorFilter;
        public Color m_TransitionColor;
        public bool m_TransitionColorGlow;
        public bool m_TransitionPatternReverse;
        public float m_TransitionAutoPlaySpeed;

        public TargetMode m_TargetMode;
        public Color m_TargetColor;
        public float m_TargetRange;
        public float m_TargetSoftness;

        public BlendMode m_SrcBlendMode;
        public BlendMode m_DstBlendMode;

        public ShadowMode m_ShadowMode;
        public Vector2 m_ShadowDistance;
        public int m_ShadowIteration;
        public float m_ShadowFade;
        public float m_ShadowMirrorScale;
        public float m_ShadowBlurIntensity;
        public ColorFilter m_ShadowColorFilter;
        public Color m_ShadowColor;
        public bool m_ShadowColorGlow;

        public EdgeMode m_EdgeMode;
        public float m_EdgeShinyRate;
        public float m_EdgeWidth;
        public ColorFilter m_EdgeColorFilter;
        public Color m_EdgeColor;
        public bool m_EdgeColorGlow;
        public float m_EdgeShinyWidth;
        public float m_EdgeShinyAutoPlaySpeed;
        public PatternArea m_PatternArea;

        public GradationMode m_GradationMode;
        public float m_GradationIntensity;
        public GradationColorFilter m_GradationColorFilter;
        public Color m_GradationColor1;
        public Color m_GradationColor2;
        public Color m_GradationColor3;
        public Color m_GradationColor4;
        public Gradient m_GradationGradient;
        public float m_GradationOffset;
        public float m_GradationScale;
        public float m_GradationRotation;

        public DetailFilter m_DetailFilter;
        public float m_DetailIntensity;
        public MinMax01 m_DetailThreshold;
        public Color m_DetailColor;
        public Texture m_DetailTex;
        public Vector2 m_DetailTexScale;
        public Vector2 m_DetailTexOffset;
        public Vector2 m_DetailTexSpeed;

        public Flip m_Flip;

        public bool willModifyMaterial => m_ToneFilter != ToneFilter.None
                                          || m_ColorFilter != ColorFilter.None
                                          || m_SamplingFilter != SamplingFilter.None
                                          || m_TransitionFilter != TransitionFilter.None
                                          || m_SrcBlendMode != BlendMode.One
                                          || m_DstBlendMode != BlendMode.OneMinusSrcAlpha
                                          || m_ShadowMode != ShadowMode.None
                                          || m_EdgeMode != EdgeMode.None
                                          || m_DetailFilter != DetailFilter.None
                                          || m_GradationMode != GradationMode.None;

        public bool willModifyVertex => willModifyMaterial;

        private Texture2D _gradationRampTex;
        private bool _isGradientDirty = true;
        private static readonly Color32[] s_Colors = new Color32[256];

        private static readonly InternalObjectPool<Texture2D> s_TexturePool = new InternalObjectPool<Texture2D>(
            () =>
            {
                var texture = new Texture2D(s_Colors.Length, 1, TextureFormat.RGBA32, false, false)
                {
                    name = "GradationRamp",
                    hideFlags = HideFlags.DontSave,
                    wrapMode = TextureWrapMode.Repeat,
                    anisoLevel = 0
                };

                return texture;
            },
            texture => texture,
            _ => { });

        private Texture2D gradationRampTex
        {
            get
            {
                if (m_GradationGradient == null) return null;
                if (!_gradationRampTex) _gradationRampTex = s_TexturePool.Rent();
                if (!_isGradientDirty) return _gradationRampTex;
                _isGradientDirty = false;

                var w = s_Colors.Length;
                for (var i = 0; i < w; i++)
                {
                    s_Colors[i] = m_GradationGradient.Evaluate((float)i / (w - 1));
                }

                _gradationRampTex.filterMode = m_GradationGradient.mode == GradientMode.Blend
                    ? FilterMode.Bilinear
                    : FilterMode.Point;
                _gradationRampTex.SetPixels32(s_Colors);
                _gradationRampTex.Apply();
                return _gradationRampTex;
            }
        }

        public void Reset()
        {
            _isGradientDirty = true;
            s_TexturePool.Return(ref _gradationRampTex);
            CopyFrom(s_DefaultContext);
        }

        private void CopyFrom(UIEffectContext src)
        {
            var dst = this;
            dst.m_ToneFilter = src.m_ToneFilter;
            dst.m_ToneIntensity = src.m_ToneIntensity;

            dst.m_ColorFilter = src.m_ColorFilter;
            dst.m_Color = src.m_Color;
            dst.m_ColorIntensity = src.m_ColorIntensity;
            dst.m_ColorGlow = src.m_ColorGlow;

            dst.m_SamplingFilter = src.m_SamplingFilter;
            dst.m_SamplingIntensity = src.m_SamplingIntensity;
            dst.m_SamplingWidth = src.m_SamplingWidth;

            dst.m_TransitionFilter = src.m_TransitionFilter;
            dst.m_TransitionRate = src.m_TransitionRate;
            dst.m_TransitionReverse = src.m_TransitionReverse;
            dst.m_TransitionTex = src.m_TransitionTex;
            dst.m_TransitionTexScale = src.m_TransitionTexScale;
            dst.m_TransitionTexOffset = src.m_TransitionTexOffset;
            dst.m_TransitionTexSpeed = src.m_TransitionTexSpeed;
            dst.m_TransitionKeepAspectRatio = src.m_TransitionKeepAspectRatio;
            dst.m_TransitionRotation = src.m_TransitionRotation;
            dst.m_TransitionWidth = src.m_TransitionWidth;
            dst.m_TransitionSoftness = src.m_TransitionSoftness;
            dst.m_TransitionRange = src.m_TransitionRange;
            dst.m_TransitionColor = src.m_TransitionColor;
            dst.m_TransitionColorFilter = src.m_TransitionColorFilter;
            dst.m_TransitionColorGlow = src.m_TransitionColorGlow;
            dst.m_TransitionPatternReverse = src.m_TransitionPatternReverse;
            dst.m_TransitionAutoPlaySpeed = src.m_TransitionAutoPlaySpeed;

            dst.m_TargetMode = src.m_TargetMode;
            dst.m_TargetColor = src.m_TargetColor;
            dst.m_TargetRange = src.m_TargetRange;
            dst.m_TargetSoftness = src.m_TargetSoftness;

            dst.m_SrcBlendMode = src.m_SrcBlendMode;
            dst.m_DstBlendMode = src.m_DstBlendMode;

            dst.m_ShadowMode = src.m_ShadowMode;
            dst.m_ShadowDistance = src.m_ShadowDistance;
            dst.m_ShadowIteration = src.m_ShadowIteration;
            dst.m_ShadowFade = src.m_ShadowFade;
            dst.m_ShadowMirrorScale = src.m_ShadowMirrorScale;
            dst.m_ShadowBlurIntensity = src.m_ShadowBlurIntensity;
            dst.m_ShadowColorFilter = src.m_ShadowColorFilter;
            dst.m_ShadowColor = src.m_ShadowColor;
            dst.m_ShadowColorGlow = src.m_ShadowColorGlow;

            dst.m_EdgeMode = src.m_EdgeMode;
            dst.m_EdgeShinyRate = src.m_EdgeShinyRate;
            dst.m_EdgeWidth = src.m_EdgeWidth;
            dst.m_EdgeColorFilter = src.m_EdgeColorFilter;
            dst.m_EdgeColor = src.m_EdgeColor;
            dst.m_EdgeColorGlow = src.m_EdgeColorGlow;
            dst.m_EdgeShinyAutoPlaySpeed = src.m_EdgeShinyAutoPlaySpeed;
            dst.m_EdgeShinyWidth = src.m_EdgeShinyWidth;
            dst.m_PatternArea = src.m_PatternArea;

            dst.m_GradationMode = src.m_GradationMode;
            dst.m_GradationIntensity = src.m_GradationIntensity;
            dst.m_GradationColorFilter = src.m_GradationColorFilter;
            dst.m_GradationColor1 = src.m_GradationColor1;
            dst.m_GradationColor2 = src.m_GradationColor2;
            dst.m_GradationColor3 = src.m_GradationColor3;
            dst.m_GradationColor4 = src.m_GradationColor4;
            dst.m_GradationGradient = src.m_GradationGradient;
            dst.m_GradationScale = src.m_GradationScale;
            dst.m_GradationOffset = src.m_GradationOffset;
            dst.m_GradationRotation = src.m_GradationRotation;

            dst.m_DetailFilter = src.m_DetailFilter;
            dst.m_DetailIntensity = src.m_DetailIntensity;
            dst.m_DetailColor = src.m_DetailColor;
            dst.m_DetailThreshold = src.m_DetailThreshold;
            dst.m_DetailTex = src.m_DetailTex;
            dst.m_DetailTexScale = src.m_DetailTexScale;
            dst.m_DetailTexOffset = src.m_DetailTexOffset;
            dst.m_DetailTexSpeed = src.m_DetailTexSpeed;

            dst.m_Flip = src.m_Flip;
        }

        public void SetGradationDirty()
        {
            _isGradientDirty = true;
        }

        public void ApplyToMaterial(Material material, float actualSamplingScale = 1f)
        {
            if (!material) return;

            Profiler.BeginSample("(UIE)[UIEffect] GetModifiedMaterial");

            material.SetInt(s_SrcBlend, (int)m_SrcBlendMode);
            material.SetInt(s_DstBlend, (int)m_DstBlendMode);

            material.SetFloat(s_ToneIntensity, Mathf.Clamp01(m_ToneIntensity));

            material.SetInt(s_ColorFilter, (int)m_ColorFilter);
            material.SetColor(s_ColorValue, m_Color);
            material.SetFloat(s_ColorIntensity, Mathf.Clamp01(m_ColorIntensity));
            material.SetInt(s_ColorGlow, m_ColorGlow ? 1 : 0);

            material.SetFloat(s_SamplingIntensity, Mathf.Clamp01(m_SamplingIntensity));
            material.SetFloat(s_SamplingWidth, m_SamplingWidth);
            material.SetFloat(s_SamplingScale, actualSamplingScale);

            material.SetFloat(s_TransitionRate, Mathf.Clamp01(m_TransitionRate));
            material.SetInt(s_TransitionReverse, m_TransitionReverse ? 1 : 0);
            material.SetTexture(s_TransitionTex, m_TransitionFilter != 0 ? m_TransitionTex : null);
            material.SetVector(s_TransitionTex_ST,
                new Vector4(m_TransitionTexScale.x, m_TransitionTexScale.y,
                    m_TransitionTexOffset.x, m_TransitionTexOffset.y));
            material.SetVector(s_TransitionTex_Speed, m_TransitionTexSpeed);
            material.SetFloat(s_TransitionWidth, Mathf.Clamp01(m_TransitionWidth));
            material.SetFloat(s_TransitionSoftness, Mathf.Clamp01(m_TransitionSoftness));
            material.SetVector(s_TransitionRange, new Vector2(m_TransitionRange.min, m_TransitionRange.max));
            material.SetInt(s_TransitionColorFilter, (int)m_TransitionColorFilter);
            material.SetColor(s_TransitionColor, m_TransitionColor);
            material.SetInt(s_TransitionColorGlow, m_TransitionColorGlow ? 1 : 0);
            material.SetInt(s_TransitionPatternReverse, m_TransitionPatternReverse ? 1 : 0);
            material.SetFloat(s_TransitionAutoPlaySpeed, m_TransitionAutoPlaySpeed);

            material.SetColor(s_TargetColor, m_TargetColor);
            material.SetFloat(s_TargetRange, Mathf.Clamp01(m_TargetRange));
            material.SetFloat(s_TargetSoftness, Mathf.Clamp01(m_TargetSoftness));

            switch (m_SamplingFilter)
            {
                case SamplingFilter.BlurFast:
                case SamplingFilter.BlurMedium:
                case SamplingFilter.BlurDetail:
                    material.SetFloat(s_ShadowBlurIntensity, Mathf.Clamp01(m_ShadowBlurIntensity));
                    break;
                default:
                    material.SetFloat(s_ShadowBlurIntensity, Mathf.Clamp01(m_SamplingIntensity));
                    break;
            }

            material.SetInt(s_ShadowColorFilter, (int)m_ShadowColorFilter);
            material.SetColor(s_ShadowColor, m_ShadowColor);
            material.SetInt(s_ShadowColorGlow, m_ShadowColorGlow ? 1 : 0);

            material.SetFloat(s_EdgeWidth, Mathf.Clamp01(m_EdgeWidth));
            material.SetInt(s_EdgeColorFilter, (int)m_EdgeColorFilter);
            material.SetColor(s_EdgeColor, m_EdgeColor);
            material.SetInt(s_EdgeColorGlow, m_EdgeColorGlow ? 1 : 0);
            material.SetFloat(s_EdgeShinyRate, Mathf.Clamp01(m_EdgeShinyRate));
            material.SetFloat(s_EdgeShinyWidth, Mathf.Clamp01(m_EdgeShinyWidth));
            material.SetFloat(s_EdgeShinyAutoPlaySpeed, m_EdgeShinyAutoPlaySpeed);
            material.SetInt(s_PatternArea, m_EdgeMode != EdgeMode.None ? (int)m_PatternArea : 0);

            material.SetTexture(s_GradationTex, m_GradationMode != 0 ? gradationRampTex : null);
            material.SetVector(s_GradationTex_ST, GetGradationScaleAndOffset());

            material.SetFloat(s_GradationIntensity, m_GradationIntensity);
            material.SetInt(s_GradationColorFilter, (int)m_GradationColorFilter);
            material.SetColor(s_GradationColor1, m_GradationColor1);
            material.SetColor(s_GradationColor2, m_GradationColor2);
            material.SetColor(s_GradationColor3, m_GradationColor3);
            material.SetColor(s_GradationColor4, m_GradationColor4);

            material.SetFloat(s_DetailIntensity, Mathf.Clamp01(m_DetailIntensity));
            material.SetColor(s_DetailColor, m_DetailColor);
            material.SetVector(s_DetailThreshold, new Vector2(m_DetailThreshold.min, m_DetailThreshold.max));
            material.SetTexture(s_DetailTex, m_DetailFilter != 0 ? m_DetailTex : null);
            material.SetVector(s_DetailTex_ST,
                new Vector4(m_DetailTexScale.x, m_DetailTexScale.y,
                    m_DetailTexOffset.x, m_DetailTexOffset.y));
            material.SetVector(s_DetailTex_Speed, m_DetailTexSpeed);

            SetKeyword(material, s_ToneKeywords, (int)m_ToneFilter);
            SetKeyword(material, s_ColorKeywords, m_ColorFilter != 0 || m_ShadowMode != 0 ? 1 : 0);
            SetKeyword(material, s_SamplingKeywords, (int)m_SamplingFilter);
            SetKeyword(material, s_TransitionKeywords, (int)m_TransitionFilter);
            SetKeyword(material, s_EdgeKeywords, (int)m_EdgeMode);
            SetKeyword(material, s_DetailKeywords, (int)m_DetailFilter);
            SetKeyword(material, s_TargetKeywords, (int)m_TargetMode);

            switch (m_GradationMode)
            {
                case GradationMode.None:
                    SetKeyword(material, s_GradationKeywords, 0);
                    break;
                case GradationMode.HorizontalGradient:
                case GradationMode.VerticalGradient:
                case GradationMode.AngleGradient:
                    SetKeyword(material, s_GradationKeywords, 1);
                    break;
                case GradationMode.RadialFast:
                case GradationMode.RadialDetail:
                    SetKeyword(material, s_GradationKeywords, 2);
                    break;
                case GradationMode.Horizontal:
                case GradationMode.Vertical:
                case GradationMode.DiagonalToRightBottom:
                case GradationMode.DiagonalToLeftBottom:
                case GradationMode.Angle:
                    SetKeyword(material, s_GradationKeywords, 3);
                    break;
                case GradationMode.Diagonal:
                    SetKeyword(material, s_GradationKeywords, 4);
                    break;
            }

            Profiler.EndSample();
        }

        public void SetEnablePreview(bool enable, Material material)
        {
            if (!material) return;
            material.SetVector(s_TransitionTex_Speed, enable ? (Vector4)m_TransitionTexSpeed : Vector4.zero);
            material.SetVector(s_DetailTex_Speed, enable ? (Vector4)m_DetailTexSpeed : Vector4.zero);
            material.SetFloat(s_TransitionAutoPlaySpeed, enable ? m_TransitionAutoPlaySpeed : 0);
            material.SetFloat(s_EdgeShinyAutoPlaySpeed, enable ? m_EdgeShinyAutoPlaySpeed : 0);
        }

        public void UpdateViewMatrix(Material material, RectTransform transitionRoot, Canvas canvas)
        {
            if (!material) return;

            var size = transitionRoot.rect.size;
            var scale = new Vector3(1f / size.x, 1f / size.y, 1f);
            if (m_TransitionKeepAspectRatio && !Mathf.Approximately(scale.x, scale.y))
            {
                scale.x = scale.y = Mathf.Min(scale.x, scale.y);
            }

            if (0 != (m_Flip & Flip.Effect))
            {
                scale.x = 0 != (m_Flip & Flip.Horizontal) ? -scale.x : scale.x;
                scale.y = 0 != (m_Flip & Flip.Vertical) ? -scale.y : scale.y;
            }

            var pivot = new Vector2(0.5f, 0.5f);
            var offset = (transitionRoot.pivot - pivot) * size;
            var w2LMat = Matrix4x4.Translate(offset) * transitionRoot.worldToLocalMatrix;

            var rootRotation = Quaternion.Euler(0, 0, m_TransitionRotation);
            var rootScale = 1f / GetMultiplier(m_TransitionRotation);
            material.SetMatrix(s_RootViewMatrix, Matrix4x4.TRS(pivot, rootRotation, scale * rootScale) * w2LMat);

            var gradRotation = Quaternion.Euler(0, 0, GetGradationRotation());
            var gradScale = 1f / GetMultiplier(GetGradationRotation());
            material.SetMatrix(s_GradViewMatrix, Matrix4x4.TRS(pivot, gradRotation, scale * gradScale) * w2LMat);

            if (m_ShadowMode == ShadowMode.Mirror)
            {
                var mirrorInv = 1f / m_ShadowMirrorScale;
                var mirrorOffset = new Vector3(0, size.y / 2 * (mirrorInv + 1) - m_ShadowDistance.y * mirrorInv, 0);
                var mirrorScale = new Vector3(1, mirrorInv, 1);
                var udScale = new Vector3(scale.x, -scale.y, scale.z);
                material.SetMatrix(s_MirrorRootViewMatrix,
                    Matrix4x4.TRS(pivot, rootRotation, udScale * rootScale)
                    * Matrix4x4.TRS(mirrorOffset, Quaternion.identity, mirrorScale) * w2LMat);
                material.SetMatrix(s_MirrorGradViewMatrix,
                    Matrix4x4.TRS(pivot, gradRotation, udScale * gradScale)
                    * Matrix4x4.TRS(mirrorOffset, Quaternion.identity, mirrorScale) * w2LMat);
            }

            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay || !canvas.worldCamera)
            {
                material.SetMatrix(s_CanvasToWorldMatrix, canvas.transform.localToWorldMatrix);
            }
            else
            {
                material.SetMatrix(s_CanvasToWorldMatrix, Matrix4x4.identity);
            }
        }

        private Vector4 GetGradationScaleAndOffset()
        {
            var gScale = 1 / m_GradationScale;
            switch (m_GradationMode)
            {
                case GradationMode.HorizontalGradient:
                case GradationMode.VerticalGradient:
                case GradationMode.AngleGradient:
                    return new Vector4(gScale, 1, -0.5f * (gScale + 1) - m_GradationOffset, 0);
                case GradationMode.RadialFast:
                case GradationMode.RadialDetail:
                {
                    return new Vector4(gScale, 1, m_GradationOffset, 0);
                }
                default:
                    return new Vector4(gScale, 1, m_GradationOffset * (gScale + 1) / 2 - gScale * 0.5f + 0.5f, 0);
            }
        }

        private float GetGradationRotation()
        {
            switch (m_GradationMode)
            {
                case GradationMode.DiagonalToLeftBottom:
                    return 135;
                case GradationMode.DiagonalToRightBottom:
                    return 45;
                case GradationMode.Vertical:
                    return 90;
                case GradationMode.VerticalGradient:
                    return -90;
                case GradationMode.Angle:
                case GradationMode.AngleGradient:
                    return m_GradationRotation;
                default:
                    return 0;
            }
        }

        private static float GetMultiplier(float deg)
        {
            var rad = Mathf.Deg2Rad * deg;
            var sin = Mathf.Sin(rad);
            var cos = Mathf.Cos(rad);
            return Mathf.Max(Mathf.Abs(cos - sin), Mathf.Abs(cos + sin));
        }

        private static void SetKeyword(Material material, string[] keywords, int index)
        {
            for (var i = 0; i < keywords.Length; i++)
            {
                if (i != index)
                {
                    material.DisableKeyword(keywords[i]);
                }
                else if (!string.IsNullOrEmpty(keywords[i]))
                {
                    material.EnableKeyword(keywords[i]);
                }
            }
        }

        public void ModifyMesh(Graphic graphic, RectTransform transitionRoot, VertexHelper vh, bool canModifyShape)
        {
            // Apply flip (without effect).
            ApplyFlipWithoutEffect(vh, m_Flip);

            var count = vh.currentIndexCount;
            if (!willModifyVertex || count == 0) return;

            var effectProxy = GraphicProxy.Find(graphic);
            var isText = effectProxy.IsText(graphic);
            effectProxy.OnPreModifyMesh(graphic);

            var verts = s_WorkingVertices;
            var expandSize = effectProxy.ModifyExpandSize(graphic, GetExpandSize(canModifyShape));

            // Get the rectangle to calculate the normalized position.
            vh.GetUIVertexStream(verts);
            var bundleSize = isText ? 6 : count;

            // Modify vertices for each bundled-quad.
            for (var i = 0; i < count; i += bundleSize)
            {
                // min/max for bundled-quad
                UIVertexUtil.GetBounds(verts, i, bundleSize, out var bounds, out var uvMask);
                UIVertexUtil.Expand(verts, i, bundleSize, expandSize, bounds);
                for (var j = 0; j < bundleSize; j++)
                {
                    var vt = verts[i + j];
                    if (onModifyVertex != null)
                    {
                        vt = onModifyVertex(vt, uvMask);
                    }
                    else
                    {
                        vt.uv1 = new Vector4(uvMask.xMin, uvMask.yMin, uvMask.xMax, uvMask.yMax);
                    }

                    verts[i + j] = vt;
                }
            }

            // Apply flip (with effect).
            ApplyFlipWithEffect(verts, m_Flip);

            // Apply shadow.
            ApplyShadow(verts, transitionRoot, m_Flip);

            vh.Clear();
            vh.AddUIVertexTriangleStream(verts);
        }

        private void ApplyShadow(List<UIVertex> verts, RectTransform transitionRoot, Flip flip)
        {
            var distance = m_ShadowDistance;
            if (distance == Vector2.zero) return;

            if ((flip & Flip.Shadow) != 0)
            {
                distance.x = (flip & Flip.Horizontal) != 0 ? -distance.x : distance.x;
                distance.y = (flip & Flip.Vertical) != 0 ? -distance.y : distance.y;
            }

            switch (m_ShadowMode)
            {
                case ShadowMode.Shadow:
                case ShadowMode.Shadow3:
                case ShadowMode.Outline:
                case ShadowMode.Outline8:
                    ShadowUtil.DoShadow(verts, s_ShadowVectors[(int)m_ShadowMode], distance, m_ShadowIteration,
                        m_ShadowFade);
                    break;
                case ShadowMode.Mirror:
                    ShadowUtil.DoMirror(verts, distance, m_ShadowMirrorScale, m_ShadowFade, transitionRoot);
                    break;
            }
        }

        private static void ApplyFlipWithoutEffect(VertexHelper vh, Flip flip)
        {
            if ((flip & Flip.Effect) != 0) return;

            var flipHorizontal = 0 != (flip & Flip.Horizontal);
            var flipVertical = 0 != (flip & Flip.Vertical);
            if (!flipHorizontal && !flipVertical) return;

            UIVertexUtil.Flip(vh, flipHorizontal, flipVertical);
        }

        private static void ApplyFlipWithEffect(List<UIVertex> verts, Flip flip)
        {
            if ((flip & Flip.Effect) == 0) return;

            var flipHorizontal = 0 != (flip & Flip.Horizontal);
            var flipVertical = 0 != (flip & Flip.Vertical);
            if (!flipHorizontal && !flipVertical) return;

            UIVertexUtil.Flip(verts, flipHorizontal, flipVertical);
        }

        private Vector4 GetExpandSize(bool canModifyShape)
        {
            if (!canModifyShape) return Vector4.zero;

            var expandSize = Vector4.zero;
            switch (m_SamplingFilter)
            {
                case SamplingFilter.BlurFast:
                    expandSize += Vector4.one * 10;
                    break;
                case SamplingFilter.BlurMedium:
                    expandSize += Vector4.one * 15;
                    break;
                case SamplingFilter.BlurDetail:
                    expandSize += Vector4.one * 20;
                    break;
                case SamplingFilter.RgbShift:
                    expandSize.x += 40;
                    expandSize.z += 40;
                    break;
            }

            switch (m_TransitionFilter)
            {
                case TransitionFilter.Melt:
                    expandSize.y += 40;
                    break;
                case TransitionFilter.Burn:
                    expandSize.w += 40;
                    break;
            }

            return expandSize;
        }
    }
}
