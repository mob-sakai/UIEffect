using System;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIEffectInternal;
using UnityEngine;
using UnityEngine.Rendering;

namespace Coffee.UIEffects
{
    [Icon("Packages/com.coffee.ui-effect/Editor/UIEffectIconIcon.png")]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class UIEffect : UIEffectBase
    {
        [SerializeField]
        protected ToneFilter m_ToneFilter = ToneFilter.None;

        [Range(0, 1)]
        [SerializeField]
        protected float m_ToneIntensity = 1;

        [SerializeField]
        protected ColorFilter m_ColorFilter = ColorFilter.None;

        [Range(0, 1)]
        [SerializeField]
        protected float m_ColorIntensity = 1;

        [SerializeField]
        protected Color m_Color = Color.white;

        [SerializeField]
        protected bool m_ColorGlow = false;

        [SerializeField]
        protected SamplingFilter m_SamplingFilter = SamplingFilter.None;

        [Range(0, 1)]
        [SerializeField]
        protected float m_SamplingIntensity = 0.5f;

        [Range(0.5f, 10f)]
        [SerializeField]
        protected float m_SamplingWidth = 1;

        [PowerRange(0.01f, 100f, 10f)]
        [SerializeField]
        protected float m_SamplingScale = 1f;

        [SerializeField]
        protected TransitionFilter m_TransitionFilter = TransitionFilter.None;

        [Range(0, 1)]
        [SerializeField]
        protected float m_TransitionRate = 0.5f;

        [SerializeField]
        protected bool m_TransitionReverse;

        [SerializeField]
        protected Texture m_TransitionTex;

        [SerializeField]
        protected Vector2 m_TransitionTexScale = new Vector2(1, 1);

        [SerializeField]
        protected Vector2 m_TransitionTexOffset = new Vector2(0, 0);

        [SerializeField]
        protected Vector2 m_TransitionTexSpeed = new Vector2(0, 0);

        [Tooltip("Effect rotation (0–360).\n" +
                 "NOTE: This property is shared between `Transition Filter` and `Detail Filter`.")]
        [SerializeField]
        [Range(0, 360)]
        private float m_TransitionRotation = 0;

        [Tooltip("The effect maintains its aspect ratio.\n" +
                 "NOTE: This property is shared between `Transition Filter`, `Gradation Mode`, and `Detail Filter`.")]
        [SerializeField]
        protected bool m_TransitionKeepAspectRatio = true;

        [Range(0, 1)]
        [SerializeField]
        protected float m_TransitionWidth = 0.2f;

        [Range(0, 1)]
        [SerializeField]
        protected float m_TransitionSoftness = 0.2f;

        [SerializeField]
        protected MinMax01 m_TransitionRange = new MinMax01(0, 1);

        [SerializeField]
        protected ColorFilter m_TransitionColorFilter = ColorFilter.MultiplyAdditive;

        [SerializeField]
        protected Color m_TransitionColor = new Color(0f, 0.5f, 1.0f, 1.0f);

        [SerializeField]
        protected bool m_TransitionColorGlow = false;

        [SerializeField]
        protected bool m_TransitionPatternReverse = false;

        [Range(-5, 5)]
        [SerializeField]
        protected float m_TransitionAutoPlaySpeed = 0f;

        [SerializeField]
        protected TargetMode m_TargetMode = TargetMode.None;

        [SerializeField]
        protected Color m_TargetColor = Color.white;

        [Range(0, 1)]
        [SerializeField]
        protected float m_TargetRange = 0.1f;

        [Range(0, 1)]
        [SerializeField]
        protected float m_TargetSoftness = 0.5f;

        [SerializeField]
        protected BlendType m_BlendType = BlendType.AlphaBlend;

        [SerializeField]
        protected BlendMode m_SrcBlendMode = BlendMode.One;

        [SerializeField]
        protected BlendMode m_DstBlendMode = BlendMode.OneMinusSrcAlpha;

        [SerializeField]
        protected ShadowMode m_ShadowMode = ShadowMode.None;

        [SerializeField]
        protected Vector2 m_ShadowDistance = new Vector2(1f, -1f);

        [Range(1, 5)]
        [SerializeField]
        protected int m_ShadowIteration = 1;

        [Range(0, 1)]
        [SerializeField]
        protected float m_ShadowFade = 0.9f;

        [Range(0, 2)]
        [SerializeField]
        protected float m_ShadowMirrorScale = 0.5f;

        [Range(0, 1)]
        [SerializeField]
        protected float m_ShadowBlurIntensity = 1;

        [SerializeField]
        protected ColorFilter m_ShadowColorFilter = ColorFilter.Replace;

        [SerializeField]
        protected Color m_ShadowColor = Color.white;

        [SerializeField]
        protected bool m_ShadowColorGlow = false;

        [SerializeField]
        protected GradationMode m_GradationMode = GradationMode.None;

        [Range(0, 1)]
        [SerializeField]
        protected float m_GradationIntensity = 1;

        [SerializeField]
        protected GradationColorFilter m_GradationColorFilter = GradationColorFilter.Multiply;

        [SerializeField]
        protected Color m_GradationColor1 = Color.white;

        [SerializeField]
        protected Color m_GradationColor2 = Color.white;

        [SerializeField]
        protected Color m_GradationColor3 = Color.white;

        [SerializeField]
        protected Color m_GradationColor4 = Color.white;

        [SerializeField]
        private Gradient m_GradationGradient = new Gradient();

        [Range(-1, 1)]
        [SerializeField]
        protected float m_GradationOffset = 0;

        [PowerRange(0.01f, 10, 10)]
        [SerializeField]
        protected float m_GradationScale = 1;

        [SerializeField]
        [Range(0, 360)]
        private float m_GradationRotation = 0;

        [SerializeField]
        protected bool m_AllowToModifyMeshShape = true;

        [SerializeField]
        protected EdgeMode m_EdgeMode = EdgeMode.None;

        [Range(0, 1)]
        [SerializeField]
        protected float m_EdgeWidth = 0.5f;

        [SerializeField]
        protected ColorFilter m_EdgeColorFilter = ColorFilter.Replace;

        [SerializeField]
        protected Color m_EdgeColor = Color.white;

        [SerializeField]
        protected bool m_EdgeColorGlow = false;

        [Range(0, 1)]
        [SerializeField]
        protected float m_EdgeShinyRate = 0.5f;

        [Range(0, 1)]
        [SerializeField]
        protected float m_EdgeShinyWidth = 0.5f;

        [Range(-5, 5)]
        [SerializeField]
        protected float m_EdgeShinyAutoPlaySpeed = 1f;

        [SerializeField]
        protected PatternArea m_PatternArea = PatternArea.Inner;

        [SerializeField]
        protected DetailFilter m_DetailFilter = DetailFilter.None;

        [Range(0, 1)]
        [SerializeField]
        protected float m_DetailIntensity = 1;

        [SerializeField]
        protected MinMax01 m_DetailThreshold = new MinMax01(0, 1);

        [SerializeField]
        protected Color m_DetailColor = Color.white;

        [SerializeField]
        protected Texture m_DetailTex;

        [SerializeField]
        protected Vector2 m_DetailTexScale = new Vector2(1, 1);

        [SerializeField]
        protected Vector2 m_DetailTexOffset = new Vector2(0, 0);

        [SerializeField]
        protected Vector2 m_DetailTexSpeed = new Vector2(0, 0);

        [SerializeField]
        protected RectTransform m_CustomRoot = null;

        [SerializeField]
        protected Flip m_Flip = 0;

        /// <summary>
        /// Tone filter for rendering.
        /// </summary>
        public ToneFilter toneFilter
        {
            get => m_ToneFilter;
            set
            {
                if (m_ToneFilter == value) return;
                context.m_ToneFilter = m_ToneFilter = value;
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// 0.0 (no effect) - 1.0 (full effect).
        /// </summary>
        public float toneIntensity
        {
            get => m_ToneIntensity;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_ToneIntensity, value)) return;
                context.m_ToneIntensity = m_ToneIntensity = value;
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// Color filter for rendering.
        /// </summary>
        public ColorFilter colorFilter
        {
            get => m_ColorFilter;
            set
            {
                if (m_ColorFilter == value) return;
                context.m_ColorFilter = m_ColorFilter = value;
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// 0.0 (no effect) - 1.0 (full effect).
        /// </summary>
        public float colorIntensity
        {
            get => m_ColorIntensity;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_ColorIntensity, value)) return;
                context.m_ColorIntensity = m_ColorIntensity = value;
                SetMaterialDirty();
            }
        }

        public Color color
        {
            get => m_Color;
            set
            {
                m_Color.a = 1;
                if (m_Color == value) return;
                context.m_Color = m_Color = value;
                SetMaterialDirty();
            }
        }

        public float colorHueShift
        {
            get => color.r;
            set
            {
                var c = color;
                c.r = Mathf.Clamp(value, -0.5f, 0.5f);
                color = c;
            }
        }

        public float colorSaturationShift
        {
            get => color.g;
            set
            {
                var c = color;
                c.g = Mathf.Clamp(value, -1, 1);
                color = c;
            }
        }

        public float colorValueShift
        {
            get => color.b;
            set
            {
                var c = color;
                c.b = Mathf.Clamp(value, -1, 1);
                color = c;
            }
        }

        public float colorContrastShift
        {
            get => color.r;
            set
            {
                var c = color;
                c.r = Mathf.Clamp(value, -1, 1);
                color = c;
            }
        }

        public float colorBrightnessShift
        {
            get => color.g;
            set
            {
                var c = color;
                c.g = Mathf.Clamp(value, -1, 1);
                color = c;
            }
        }

        public float colorAlpha
        {
            get => color.a;
            set
            {
                var c = color;
                c.a = value;
                color = c;
            }
        }

        public bool colorGlow
        {
            get => m_ColorGlow;
            set
            {
                if (m_ColorGlow == value) return;
                context.m_ColorGlow = m_ColorGlow = value;
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// Sampling filter for rendering.
        /// </summary>
        public SamplingFilter samplingFilter
        {
            get => m_SamplingFilter;
            set
            {
                if (m_SamplingFilter == value) return;
                context.m_SamplingFilter = m_SamplingFilter = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// 0.0 (no effect) - 1.0 (full effect).
        /// </summary>
        public float samplingIntensity
        {
            get => m_SamplingIntensity;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_SamplingIntensity, value)) return;
                context.m_SamplingIntensity = m_SamplingIntensity = value;
                SetMaterialDirty();
            }
        }

        public float samplingWidth
        {
            get => m_SamplingWidth;
            set
            {
                value = Mathf.Clamp(value, 0.5f, 10);
                if (Mathf.Approximately(m_SamplingWidth, value)) return;
                context.m_SamplingWidth = m_SamplingWidth = value;
                SetMaterialDirty();
            }
        }

        public float samplingScale
        {
            get => m_SamplingScale;
            set
            {
                value = Mathf.Clamp(value, 0.01f, 100);
                if (Mathf.Approximately(m_SamplingScale, value)) return;
                m_SamplingScale = value;
                SetMaterialDirty();
            }
        }

        public override float actualSamplingScale => Mathf.Clamp(m_SamplingScale, 0.01f, 100);

        public override bool canModifyShape => m_AllowToModifyMeshShape;

        /// <summary>
        /// Transition filter for rendering.
        /// </summary>
        public TransitionFilter transitionFilter
        {
            get => m_TransitionFilter;
            set
            {
                if (m_TransitionFilter == value) return;
                context.m_TransitionFilter = m_TransitionFilter = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// 0.0 (no effect) - 1.0 (full effect).
        /// </summary>
        public float transitionRate
        {
            get => m_TransitionRate;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_TransitionRate, value)) return;
                context.m_TransitionRate = m_TransitionRate = value;
                SetMaterialDirty();
            }
        }

        public bool transitionReverse
        {
            get => m_TransitionReverse;
            set
            {
                if (m_TransitionReverse == value) return;
                context.m_TransitionReverse = m_TransitionReverse = value;
                SetMaterialDirty();
            }
        }

        public Texture transitionTexture
        {
            get => m_TransitionTex;
            set
            {
                if (m_TransitionTex == value) return;
                context.m_TransitionTex = m_TransitionTex = value;
                SetMaterialDirty();
            }
        }

        public Vector2 transitionTextureScale
        {
            get => m_TransitionTexScale;
            set
            {
                if (m_TransitionTexScale == value) return;
                context.m_TransitionTexScale = m_TransitionTexScale = value;
                SetMaterialDirty();
            }
        }

        public Vector2 transitionTextureOffset
        {
            get => m_TransitionTexOffset;
            set
            {
                if (m_TransitionTexOffset == value) return;
                context.m_TransitionTexOffset = m_TransitionTexOffset = value;
                SetMaterialDirty();
            }
        }

        public Vector2 transitionTextureSpeed
        {
            get => m_TransitionTexSpeed;
            set
            {
                if (m_TransitionTexSpeed == value) return;
                context.m_TransitionTexSpeed = m_TransitionTexSpeed = value;
                SetMaterialDirty();
            }
        }

        public float transitionRotation
        {
            get => m_TransitionRotation;
            set
            {
                if (Mathf.Approximately(m_TransitionRotation, value)) return;
                context.m_TransitionRotation = m_TransitionRotation = value;
            }
        }

        public bool transitionKeepAspectRatio
        {
            get => m_TransitionKeepAspectRatio;
            set
            {
                if (m_TransitionKeepAspectRatio == value) return;
                context.m_TransitionKeepAspectRatio = m_TransitionKeepAspectRatio = value;
            }
        }

        public float transitionWidth
        {
            get => m_TransitionWidth;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_TransitionWidth, value)) return;
                context.m_TransitionWidth = m_TransitionWidth = value;
                SetMaterialDirty();
            }
        }

        public float transitionSoftness
        {
            get => m_TransitionSoftness;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_TransitionSoftness, value)) return;
                context.m_TransitionSoftness = m_TransitionSoftness = value;
                SetMaterialDirty();
            }
        }

        public MinMax01 transitionRange
        {
            get => m_TransitionRange;
            set
            {
                if (m_TransitionRange.Approximately(value)) return;
                context.m_TransitionRange = m_TransitionRange = value;
                SetMaterialDirty();
            }
        }

        public ColorFilter transitionColorFilter
        {
            get => m_TransitionColorFilter;
            set
            {
                if (m_TransitionColorFilter == value) return;
                context.m_TransitionColorFilter = m_TransitionColorFilter = value;
                SetMaterialDirty();
            }
        }

        public Color transitionColor
        {
            get => m_TransitionColor;
            set
            {
                if (m_TransitionColor == value) return;
                context.m_TransitionColor = m_TransitionColor = value;
                SetMaterialDirty();
            }
        }

        public float transitionColorHueShift
        {
            get => transitionColor.r;
            set
            {
                var c = transitionColor;
                c.r = Mathf.Clamp(value, -0.5f, 0.5f);
                transitionColor = c;
            }
        }

        public float transitionColorSaturationShift
        {
            get => transitionColor.g;
            set
            {
                var c = transitionColor;
                c.g = Mathf.Clamp(value, -1, 1);
                transitionColor = c;
            }
        }

        public float transitionColorValueShift
        {
            get => transitionColor.b;
            set
            {
                var c = transitionColor;
                c.b = Mathf.Clamp(value, -1, 1);
                transitionColor = c;
            }
        }

        public float transitionColorContrastShift
        {
            get => transitionColor.r;
            set
            {
                var c = transitionColor;
                c.r = Mathf.Clamp(value, -1, 1);
                transitionColor = c;
            }
        }

        public float transitionColorBrightnessShift
        {
            get => transitionColor.g;
            set
            {
                var c = transitionColor;
                c.g = Mathf.Clamp(value, -1, 1);
                transitionColor = c;
            }
        }

        public float transitionColorAlpha
        {
            get => transitionColor.a;
            set
            {
                var c = transitionColor;
                c.a = value;
                transitionColor = c;
            }
        }

        public bool transitionColorGlow
        {
            get => m_TransitionColorGlow;
            set
            {
                if (m_TransitionColorGlow == value) return;
                context.m_TransitionColorGlow = m_TransitionColorGlow = value;
                SetMaterialDirty();
            }
        }

        public bool transitionPatternReverse
        {
            get => m_TransitionPatternReverse;
            set
            {
                if (m_TransitionPatternReverse == value) return;
                context.m_TransitionPatternReverse = m_TransitionPatternReverse = value;
                SetMaterialDirty();
            }
        }

        public float transitionAutoPlaySpeed
        {
            get => m_TransitionAutoPlaySpeed;
            set
            {
                value = Mathf.Clamp(value, -5, 5);
                if (Mathf.Approximately(m_TransitionAutoPlaySpeed, value)) return;
                context.m_TransitionAutoPlaySpeed = m_TransitionAutoPlaySpeed = value;
                SetMaterialDirty();
            }
        }

        public TargetMode targetMode
        {
            get => m_TargetMode;
            set
            {
                if (m_TargetMode == value) return;
                context.m_TargetMode = m_TargetMode = value;
                SetMaterialDirty();
            }
        }

        public Color targetColor
        {
            get => m_TargetColor;
            set
            {
                if (m_TargetColor == value) return;
                context.m_TargetColor = m_TargetColor = value;
                SetMaterialDirty();
            }
        }

        public float targetRange
        {
            get => m_TargetRange;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_TargetRange, value)) return;
                context.m_TargetRange = m_TargetRange = value;
                SetMaterialDirty();
            }
        }

        public float targetSoftness
        {
            get => m_TargetSoftness;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_TargetSoftness, value)) return;
                context.m_TargetSoftness = m_TargetSoftness = value;
                SetMaterialDirty();
            }
        }

        public BlendType blendType
        {
            get => m_BlendType;
            set
            {
                if (m_BlendType == value) return;
                (m_SrcBlendMode, m_DstBlendMode) = (m_BlendType, m_SrcBlendMode, m_DstBlendMode).Convert();
                context.m_SrcBlendMode = m_SrcBlendMode;
                context.m_DstBlendMode = m_DstBlendMode;
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// Source blend type for rendering.
        /// This is used when material is not set and blend type is custom.
        /// </summary>
        public BlendMode srcBlendMode
        {
            get => m_SrcBlendMode;
            set
            {
                if (m_SrcBlendMode == value) return;
                context.m_SrcBlendMode = m_SrcBlendMode = value;
                m_BlendType = (m_SrcBlendMode, m_DstBlendMode).Convert();
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// Destination blend type for rendering.
        /// This is used when material is not set and blend type is custom.
        /// </summary>
        public BlendMode dstBlendMode
        {
            get => m_DstBlendMode;
            set
            {
                if (m_DstBlendMode == value) return;
                context.m_DstBlendMode = m_DstBlendMode = value;
                m_BlendType = (m_SrcBlendMode, m_DstBlendMode).Convert();
                SetMaterialDirty();
            }
        }

        public ShadowMode shadowMode
        {
            get => m_ShadowMode;
            set
            {
                if (m_ShadowMode == value) return;
                context.m_ShadowMode = m_ShadowMode = value;
                SetVerticesDirty();
            }
        }

        public Vector2 shadowDistance
        {
            get => m_ShadowDistance;
            set
            {
                if (m_ShadowDistance == value) return;
                context.m_ShadowDistance = m_ShadowDistance = value;
                SetVerticesDirty();
            }
        }

        public float shadowFade
        {
            get => m_ShadowFade;
            set
            {
                value = Mathf.Clamp01(value);
                if (Mathf.Approximately(m_ShadowFade, value)) return;
                context.m_ShadowFade = m_ShadowFade = value;
                SetVerticesDirty();
            }
        }

        public int shadowIteration
        {
            get => m_ShadowIteration;
            set
            {
                value = Mathf.Clamp(value, 1, 5);
                if (m_ShadowIteration == value) return;
                context.m_ShadowIteration = m_ShadowIteration = value;
                SetVerticesDirty();
            }
        }

        public float shadowMirrorScale
        {
            get => m_ShadowMirrorScale;
            set
            {
                value = Mathf.Clamp(value, 0f, 2f);
                if (Mathf.Approximately(m_ShadowMirrorScale, value)) return;
                context.m_ShadowMirrorScale = m_ShadowMirrorScale = value;
                SetVerticesDirty();
            }
        }

        public float shadowBlurIntensity
        {
            get => m_ShadowBlurIntensity;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_ShadowBlurIntensity, value)) return;
                context.m_ShadowBlurIntensity = m_ShadowBlurIntensity = value;
                SetMaterialDirty();
            }
        }

        public ColorFilter shadowColorFilter
        {
            get => m_ShadowColorFilter;
            set
            {
                if (m_ShadowColorFilter == value) return;
                context.m_ShadowColorFilter = m_ShadowColorFilter = value;
                SetMaterialDirty();
            }
        }

        public Color shadowColor
        {
            get => m_ShadowColor;
            set
            {
                if (m_ShadowColor == value) return;
                context.m_ShadowColor = m_ShadowColor = value;
                SetMaterialDirty();
            }
        }

        public float shadowColorHueShift
        {
            get => shadowColor.r;
            set
            {
                var c = shadowColor;
                c.r = Mathf.Clamp(value, -0.5f, 0.5f);
                shadowColor = c;
            }
        }

        public float shadowColorSaturationShift
        {
            get => shadowColor.g;
            set
            {
                var c = shadowColor;
                c.g = Mathf.Clamp(value, -1, 1);
                shadowColor = c;
            }
        }

        public float shadowColorValueShift
        {
            get => shadowColor.b;
            set
            {
                var c = shadowColor;
                c.b = Mathf.Clamp(value, -1, 1);
                shadowColor = c;
            }
        }

        public float shadowColorContrastShift
        {
            get => shadowColor.r;
            set
            {
                var c = shadowColor;
                c.r = Mathf.Clamp(value, -1, 1);
                shadowColor = c;
            }
        }

        public float shadowColorBrightnessShift
        {
            get => shadowColor.g;
            set
            {
                var c = shadowColor;
                c.g = Mathf.Clamp(value, -1, 1);
                shadowColor = c;
            }
        }

        public float shadowColorAlpha
        {
            get => shadowColor.a;
            set
            {
                var c = shadowColor;
                c.a = value;
                shadowColor = c;
            }
        }

        [Obsolete("shadowGlow is deprecated. Use shadowColorGlow instead.", false)]
        public bool shadowGlow
        {
            get => m_ShadowColorGlow;
            set => shadowColorGlow = value;
        }

        public bool shadowColorGlow
        {
            get => m_ShadowColorGlow;
            set
            {
                if (m_ShadowColorGlow == value) return;
                context.m_ShadowColorGlow = m_ShadowColorGlow = value;
                SetMaterialDirty();
            }
        }

        public EdgeMode edgeMode
        {
            get => m_EdgeMode;
            set
            {
                if (m_EdgeMode == value) return;
                context.m_EdgeMode = m_EdgeMode = value;
                SetMaterialDirty();
            }
        }

        public float edgeShinyRate
        {
            get => m_EdgeShinyRate;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_EdgeShinyRate, value)) return;
                context.m_EdgeShinyRate = m_EdgeShinyRate = value;
                SetMaterialDirty();
            }
        }

        public float edgeWidth
        {
            get => m_EdgeWidth;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_EdgeWidth, value)) return;
                context.m_EdgeWidth = m_EdgeWidth = value;
                SetMaterialDirty();
            }
        }

        public ColorFilter edgeColorFilter
        {
            get => m_EdgeColorFilter;
            set
            {
                if (m_EdgeColorFilter == value) return;
                context.m_EdgeColorFilter = m_EdgeColorFilter = value;
                SetMaterialDirty();
            }
        }

        public Color edgeColor
        {
            get => m_EdgeColor;
            set
            {
                if (m_EdgeColor == value) return;
                context.m_EdgeColor = m_EdgeColor = value;
                SetMaterialDirty();
            }
        }

        public float edgeColorHueShift
        {
            get => edgeColor.r;
            set
            {
                var c = edgeColor;
                c.r = Mathf.Clamp(value, -0.5f, 0.5f);
                edgeColor = c;
            }
        }

        public float edgeColorSaturationShift
        {
            get => edgeColor.g;
            set
            {
                var c = edgeColor;
                c.g = Mathf.Clamp(value, -1, 1);
                edgeColor = c;
            }
        }

        public float edgeColorValueShift
        {
            get => edgeColor.b;
            set
            {
                var c = edgeColor;
                c.b = Mathf.Clamp(value, -1, 1);
                edgeColor = c;
            }
        }

        public float edgeColorContrastShift
        {
            get => edgeColor.r;
            set
            {
                var c = edgeColor;
                c.r = Mathf.Clamp(value, -1, 1);
                edgeColor = c;
            }
        }

        public float edgeColorBrightnessShift
        {
            get => edgeColor.g;
            set
            {
                var c = edgeColor;
                c.g = Mathf.Clamp(value, -1, 1);
                edgeColor = c;
            }
        }

        public float edgeColorAlpha
        {
            get => edgeColor.a;
            set
            {
                var c = edgeColor;
                c.a = value;
                edgeColor = c;
            }
        }

        public bool edgeColorGlow
        {
            get => m_EdgeColorGlow;
            set
            {
                if (m_EdgeColorGlow == value) return;
                context.m_EdgeColorGlow = m_EdgeColorGlow = value;
                SetMaterialDirty();
            }
        }

        public float edgeShinyWidth
        {
            get => m_EdgeShinyWidth;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_EdgeShinyWidth, value)) return;
                context.m_EdgeShinyWidth = m_EdgeShinyWidth = value;
                SetMaterialDirty();
            }
        }

        public float edgeShinyAutoPlaySpeed
        {
            get => m_EdgeShinyAutoPlaySpeed;
            set
            {
                value = Mathf.Clamp(value, -5, 5);
                if (Mathf.Approximately(m_EdgeShinyAutoPlaySpeed, value)) return;
                context.m_EdgeShinyAutoPlaySpeed = m_EdgeShinyAutoPlaySpeed = value;
                SetMaterialDirty();
            }
        }

        public PatternArea patternArea
        {
            get => m_PatternArea;
            set
            {
                if (m_PatternArea == value) return;
                context.m_PatternArea = m_PatternArea = value;
                SetMaterialDirty();
            }
        }

        public GradationMode gradationMode
        {
            get => m_GradationMode;
            set
            {
                if (m_GradationMode == value) return;
                context.m_GradationMode = m_GradationMode = value;
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// 0.0 (no effect) - 1.0 (full effect).
        /// </summary>
        public float gradationIntensity
        {
            get => m_GradationIntensity;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_GradationIntensity, value)) return;
                context.m_GradationIntensity = m_GradationIntensity = value;
                SetMaterialDirty();
            }
        }

        public GradationColorFilter gradationColorFilter
        {
            get => m_GradationColorFilter;
            set
            {
                if (m_GradationColorFilter == value) return;
                context.m_GradationColorFilter = m_GradationColorFilter = value;
                SetMaterialDirty();
            }
        }

        public Color gradationColor1
        {
            get => m_GradationColor1;
            set
            {
                if (m_GradationColor1 == value) return;
                context.m_GradationColor1 = m_GradationColor1 = value;
                SetMaterialDirty();
            }
        }

        public Color gradationColor2
        {
            get => m_GradationColor2;
            set
            {
                if (m_GradationColor2 == value) return;
                context.m_GradationColor2 = m_GradationColor2 = value;
                SetMaterialDirty();
            }
        }

        public Color gradationColor3
        {
            get => m_GradationColor3;
            set
            {
                if (m_GradationColor3 == value) return;
                context.m_GradationColor3 = m_GradationColor3 = value;
                SetMaterialDirty();
            }
        }

        public Color gradationColor4
        {
            get => m_GradationColor4;
            set
            {
                if (m_GradationColor4 == value) return;
                context.m_GradationColor4 = m_GradationColor4 = value;
                SetMaterialDirty();
            }
        }

        public float gradationOffset
        {
            get => m_GradationOffset;
            set
            {
                if (Mathf.Approximately(m_GradationOffset, value)) return;
                context.m_GradationOffset = m_GradationOffset = value;
                SetMaterialDirty();
            }
        }

        public float gradationScale
        {
            get => m_GradationScale;
            set
            {
                if (Mathf.Approximately(m_GradationScale, value)) return;
                context.m_GradationScale = m_GradationScale = value;
                SetMaterialDirty();
            }
        }

        public float gradationRotation
        {
            get => m_GradationRotation;
            set
            {
                value = Mathf.Repeat(value, 360);
                if (Mathf.Approximately(m_GradationRotation, value)) return;
                context.m_GradationRotation = m_GradationRotation = value;
            }
        }

        public DetailFilter detailFilter
        {
            get => m_DetailFilter;
            set
            {
                if (m_DetailFilter == value) return;
                context.m_DetailFilter = m_DetailFilter = value;
                SetMaterialDirty();
            }
        }

        public float detailIntensity
        {
            get => m_DetailIntensity;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_DetailIntensity, value)) return;
                context.m_DetailIntensity = m_DetailIntensity = value;
                SetMaterialDirty();
            }
        }

        public MinMax01 detailThreshold
        {
            get => m_DetailThreshold;
            set
            {
                if (m_DetailThreshold.Approximately(value)) return;
                context.m_DetailThreshold = m_DetailThreshold = value;
                SetMaterialDirty();
            }
        }

        public Color detailColor
        {
            get => m_EdgeColor;
            set
            {
                if (m_EdgeColor == value) return;
                context.m_DetailColor = m_EdgeColor = value;
                SetMaterialDirty();
            }
        }

        public float detailColorAlpha
        {
            get => detailColor.a;
            set
            {
                var c = detailColor;
                c.a = value;
                detailColor = c;
            }
        }

        public Texture detailTexture
        {
            get => m_DetailTex;
            set
            {
                if (m_DetailTex == value) return;
                context.m_DetailTex = m_DetailTex = value;
                SetMaterialDirty();
            }
        }

        public Vector2 detailTextureScale
        {
            get => m_DetailTexScale;
            set
            {
                if (m_DetailTexScale == value) return;
                context.m_DetailTexScale = m_DetailTexScale = value;
                SetMaterialDirty();
            }
        }

        public Vector2 detailTextureOffset
        {
            get => m_DetailTexOffset;
            set
            {
                if (m_DetailTexOffset == value) return;
                context.m_DetailTexOffset = m_DetailTexOffset = value;
                SetMaterialDirty();
            }
        }

        public Vector2 detailTextureSpeed
        {
            get => m_DetailTexSpeed;
            set
            {
                if (m_DetailTexSpeed == value) return;
                context.m_DetailTexSpeed = m_DetailTexSpeed = value;
                SetMaterialDirty();
            }
        }

        public bool allowToModifyMeshShape
        {
            get => m_AllowToModifyMeshShape;
            set
            {
                if (m_AllowToModifyMeshShape == value) return;
                m_AllowToModifyMeshShape = value;
                SetVerticesDirty();
            }
        }

        public RectTransform customRoot
        {
            get => m_CustomRoot;
            set => m_CustomRoot = value;
        }

        public Flip flip
        {
            get => m_Flip;
            set
            {
                if (m_Flip == value) return;
                context.m_Flip = m_Flip = value;
                SetVerticesDirty();
            }
        }

        public override RectTransform transitionRoot => m_CustomRoot
            ? m_CustomRoot
            : transform as RectTransform;

        public List<UIEffectReplica> replicas => _replicas ??= InternalListPool<UIEffectReplica>.Rent();
        private List<UIEffectReplica> _replicas;

        protected override void OnEnable()
        {
            (m_SrcBlendMode, m_DstBlendMode) = (m_BlendType, m_SrcBlendMode, m_DstBlendMode).Convert();
            base.OnEnable();
            SetMaterialDirty();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SetMaterialDirty();
        }

        protected override void OnDestroy()
        {
            InternalListPool<UIEffectReplica>.Return(ref _replicas);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            (m_SrcBlendMode, m_DstBlendMode) = (m_BlendType, m_SrcBlendMode, m_DstBlendMode).Convert();
            context?.SetGradationDirty();
            base.OnValidate();
        }

        internal override void SetEnablePreviewIfSelected(GameObject[] selection)
        {
            var selected = 0 < selection.Length
                           && (selection.Contains(gameObject)
                               || replicas.Any(x => x && selection.Contains(x.gameObject)));

            context.SetEnablePreview(selected, effectMaterial);
            foreach (var r in replicas)
            {
                if (!r || !r.isActiveAndEnabled || r.context == null) continue;
                r.context.SetEnablePreview(selected, r.effectMaterial);
            }
        }
#endif

        public override void SetVerticesDirty()
        {
            base.SetVerticesDirty();
            for (var i = 0; i < replicas.Count; i++)
            {
                if (!replicas[i]) continue;
                replicas[i].SetVerticesDirty();
            }
        }

        public override void SetMaterialDirty()
        {
            base.SetMaterialDirty();
            for (var i = 0; i < replicas.Count; i++)
            {
                if (!replicas[i]) continue;
                replicas[i].SetMaterialDirty();
            }
        }

        /// <summary>
        /// Set gradation gradient's keys.
        /// </summary>
        public void SetGradientKeys(Gradient gradient)
        {
            SetGradientKeys(gradient.colorKeys, gradient.alphaKeys, gradient.mode);
        }

        /// <summary>
        /// Set gradation gradient's keys.
        /// </summary>
        public void SetGradientKeys(GradientColorKey[] colorKeys, GradientAlphaKey[] alphaKeys,
            GradientMode mode = GradientMode.Blend)
        {
            m_GradationGradient ??= new Gradient();
            m_GradationGradient.SetKeys(colorKeys, alphaKeys);
            m_GradationGradient.mode = mode;
            context?.SetGradationDirty();
            SetMaterialDirty();
        }

        public override void ApplyContextToMaterial(Material material)
        {
            base.ApplyContextToMaterial(material);

            for (var i = 0; i < replicas.Count; i++)
            {
                if (!replicas[i]) continue;
                replicas[i].ApplyContextToMaterial(material);
            }
        }

        public override void SetRate(float rate, UIEffectTweener.CullingMask mask)
        {
            if (toneFilter != ToneFilter.None && 0 < (mask & UIEffectTweener.CullingMask.Tone))
            {
                toneIntensity = rate;
            }

            if (colorFilter != ColorFilter.None && 0 < (mask & UIEffectTweener.CullingMask.Color))
            {
                colorIntensity = rate;
            }

            if (samplingFilter != SamplingFilter.None && 0 < (mask & UIEffectTweener.CullingMask.Sampling))
            {
                samplingIntensity = rate;
            }

            if (transitionFilter != TransitionFilter.None && 0 < (mask & UIEffectTweener.CullingMask.Transition))
            {
                transitionRate = rate;
            }

            if (gradationMode != GradationMode.None && 0 < (mask & UIEffectTweener.CullingMask.GradiationOffset))
            {
                gradationOffset = Mathf.Lerp(-1f, 1f, rate);
            }

            if ((gradationMode == GradationMode.Angle || gradationMode == GradationMode.AngleGradient)
                && 0 < (mask & UIEffectTweener.CullingMask.GradiationRotation))
            {
                gradationRotation = Mathf.Lerp(0f, 360f, rate);
            }

            if (edgeMode == EdgeMode.Shiny && 0 < (mask & UIEffectTweener.CullingMask.EdgeShiny))
            {
                edgeShinyRate = rate;
            }
        }

        public override bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            switch (transitionFilter)
            {
                case TransitionFilter.None:
                case TransitionFilter.Shiny:
                case TransitionFilter.Mask:
                case TransitionFilter.Pattern:
                    return true;
                default:
                    return transitionRate < 1;
            }
        }

        /// <summary>
        /// Load preset settings.
        /// </summary>
        public void LoadPreset(string presetName)
        {
            LoadPreset(presetName, false);
        }

        /// <summary>
        /// Load preset settings.
        /// </summary>
        public void LoadPreset(string presetName, bool append)
        {
            var preset = UIEffectProjectSettings.LoadPreset(presetName);
            if (preset is UIEffect presetV1)
            {
                LoadPreset(presetV1, append);
            }
            else if (preset is UIEffectPreset presetV2)
            {
                LoadPreset(presetV2, append);
            }
        }

        /// <summary>
        /// Load preset settings.
        /// </summary>
        public void LoadPreset(UIEffect src)
        {
            LoadPreset(src, false);
        }

        /// <summary>
        /// Load preset settings.
        /// </summary>
        public void LoadPreset(UIEffectPreset preset)
        {
            LoadPreset(preset, false);
        }

        /// <summary>
        /// Load preset settings.
        /// </summary>
        public void LoadPreset(UIEffect src, bool append)
        {
            if (!src) return;

            var dst = this;
            if (!append || src.m_ToneFilter != ToneFilter.None)
            {
                dst.m_ToneFilter = src.m_ToneFilter;
                dst.m_ToneIntensity = src.m_ToneIntensity;
            }

            if (!append || src.m_ColorFilter != ColorFilter.None)
            {
                dst.m_ColorFilter = src.m_ColorFilter;
                dst.m_Color = src.m_Color;
                dst.m_ColorIntensity = src.m_ColorIntensity;
                dst.m_ColorGlow = src.m_ColorGlow;
            }

            if (!append || src.m_SamplingFilter != SamplingFilter.None)
            {
                dst.m_SamplingFilter = src.m_SamplingFilter;
                dst.m_SamplingIntensity = src.m_SamplingIntensity;
                dst.m_SamplingWidth = src.m_SamplingWidth;
            }

            if (!append || src.m_TransitionFilter != TransitionFilter.None)
            {
                dst.m_TransitionFilter = src.m_TransitionFilter;
                dst.m_TransitionRate = src.m_TransitionRate;
                dst.m_TransitionReverse = src.m_TransitionReverse;
                dst.m_TransitionTex = src.m_TransitionTex;
                dst.m_TransitionTexScale = src.m_TransitionTexScale;
                dst.m_TransitionTexOffset = src.m_TransitionTexOffset;
                dst.m_TransitionTexSpeed = src.m_TransitionTexSpeed;
                dst.m_TransitionRotation = src.m_TransitionRotation;
                dst.m_TransitionKeepAspectRatio = src.m_TransitionKeepAspectRatio;
                dst.m_TransitionWidth = src.m_TransitionWidth;
                dst.m_TransitionSoftness = src.m_TransitionSoftness;
                dst.m_TransitionRange = src.m_TransitionRange;
                dst.m_TransitionColorFilter = src.m_TransitionColorFilter;
                dst.m_TransitionColor = src.m_TransitionColor;
                dst.m_TransitionColorGlow = src.m_TransitionColorGlow;
                dst.m_TransitionPatternReverse = src.m_TransitionPatternReverse;
                dst.m_TransitionAutoPlaySpeed = src.m_TransitionAutoPlaySpeed;
            }

            if (!append || src.m_TargetMode != TargetMode.None)
            {
                dst.m_TargetMode = src.m_TargetMode;
                dst.m_TargetColor = src.m_TargetColor;
                dst.m_TargetRange = src.m_TargetRange;
                dst.m_TargetSoftness = src.m_TargetSoftness;
            }

            if (!append || src.m_BlendType != BlendType.AlphaBlend)
            {
                dst.m_BlendType = src.m_BlendType;
                (dst.m_SrcBlendMode, dst.m_DstBlendMode) =
                    (dst.m_BlendType, src.m_SrcBlendMode, src.m_DstBlendMode).Convert();
            }

            if (!append || src.m_ShadowMode != ShadowMode.None)
            {
                dst.m_ShadowMode = src.m_ShadowMode;
                dst.m_ShadowDistance = src.m_ShadowDistance;
                dst.m_ShadowIteration = src.m_ShadowIteration;
                dst.m_ShadowFade = src.m_ShadowFade;
                dst.m_ShadowMirrorScale = src.m_ShadowMirrorScale;
                dst.m_ShadowBlurIntensity = src.m_ShadowBlurIntensity;
                dst.m_ShadowColorFilter = src.m_ShadowColorFilter;
                dst.m_ShadowColor = src.m_ShadowColor;
                dst.m_ShadowColorGlow = src.m_ShadowColorGlow;
            }

            if (!append || src.m_EdgeMode != EdgeMode.None)
            {
                dst.m_EdgeMode = src.m_EdgeMode;
                dst.m_EdgeShinyRate = src.m_EdgeShinyRate;
                dst.m_EdgeWidth = src.m_EdgeWidth;
                dst.m_EdgeColorFilter = src.m_EdgeColorFilter;
                dst.m_EdgeColor = src.m_EdgeColor;
                dst.m_EdgeColorGlow = src.m_EdgeColorGlow;
                dst.m_EdgeShinyWidth = src.m_EdgeShinyWidth;
                dst.m_EdgeShinyAutoPlaySpeed = src.m_EdgeShinyAutoPlaySpeed;
                dst.m_PatternArea = src.m_PatternArea;
            }

            if (!append || src.m_GradationMode != GradationMode.None)
            {
                dst.m_GradationMode = src.m_GradationMode;
                dst.m_GradationIntensity = src.m_GradationIntensity;
                dst.m_GradationColorFilter = src.m_GradationColorFilter;
                dst.m_GradationColor1 = src.m_GradationColor1;
                dst.m_GradationColor2 = src.m_GradationColor2;
                dst.m_GradationColor3 = src.m_GradationColor3;
                dst.m_GradationColor4 = src.m_GradationColor4;
                dst.SetGradientKeys(src.m_GradationGradient);
                dst.m_GradationOffset = src.m_GradationOffset;
                dst.m_GradationScale = src.m_GradationScale;
                dst.m_GradationRotation = src.m_GradationRotation;
            }

            if (!append || src.m_DetailFilter != DetailFilter.None)
            {
                dst.m_DetailFilter = src.m_DetailFilter;
                dst.m_DetailIntensity = src.m_DetailIntensity;
                dst.m_DetailThreshold = src.m_DetailThreshold;
                dst.m_DetailTex = src.m_DetailTex;
                dst.m_DetailTexScale = src.m_DetailTexScale;
                dst.m_DetailTexOffset = src.m_DetailTexOffset;
                dst.m_DetailTexSpeed = src.m_DetailTexSpeed;
            }

            if (!append || src.m_Flip != 0)
            {
                dst.m_Flip = append ? dst.m_Flip | src.m_Flip : src.m_Flip;
            }

            Misc.SetDirty(dst);
            UpdateContext(context);
            SetVerticesDirty();
            SetMaterialDirty();
        }

        /// <summary>
        /// Load preset settings.
        /// </summary>
        public void LoadPreset(UIEffectPreset src, bool append)
        {
            if (!src) return;

            var dst = this;
            if (!append || src.m_ToneFilter != ToneFilter.None)
            {
                dst.m_ToneFilter = src.m_ToneFilter;
                dst.m_ToneIntensity = src.m_ToneIntensity;
            }

            if (!append || src.m_ColorFilter != ColorFilter.None)
            {
                dst.m_ColorFilter = src.m_ColorFilter;
                dst.m_Color = src.m_Color;
                dst.m_ColorIntensity = src.m_ColorIntensity;
                dst.m_ColorGlow = src.m_ColorGlow;
            }

            if (!append || src.m_SamplingFilter != SamplingFilter.None)
            {
                dst.m_SamplingFilter = src.m_SamplingFilter;
                dst.m_SamplingIntensity = src.m_SamplingIntensity;
                dst.m_SamplingWidth = src.m_SamplingWidth;
            }

            if (!append || src.m_TransitionFilter != TransitionFilter.None)
            {
                dst.m_TransitionFilter = src.m_TransitionFilter;
                dst.m_TransitionRate = src.m_TransitionRate;
                dst.m_TransitionReverse = src.m_TransitionReverse;
                dst.m_TransitionTex = src.m_TransitionTex;
                dst.m_TransitionTexScale = src.m_TransitionTexScale;
                dst.m_TransitionTexOffset = src.m_TransitionTexOffset;
                dst.m_TransitionTexSpeed = src.m_TransitionTexSpeed;
                dst.m_TransitionRotation = src.m_TransitionRotation;
                dst.m_TransitionKeepAspectRatio = src.m_TransitionKeepAspectRatio;
                dst.m_TransitionWidth = src.m_TransitionWidth;
                dst.m_TransitionSoftness = src.m_TransitionSoftness;
                dst.m_TransitionRange = src.m_TransitionRange;
                dst.m_TransitionColorFilter = src.m_TransitionColorFilter;
                dst.m_TransitionColor = src.m_TransitionColor;
                dst.m_TransitionColorGlow = src.m_TransitionColorGlow;
                dst.m_TransitionPatternReverse = src.m_TransitionPatternReverse;
                dst.m_TransitionAutoPlaySpeed = src.m_TransitionAutoPlaySpeed;
            }

            if (!append || src.m_TargetMode != TargetMode.None)
            {
                dst.m_TargetMode = src.m_TargetMode;
                dst.m_TargetColor = src.m_TargetColor;
                dst.m_TargetRange = src.m_TargetRange;
                dst.m_TargetSoftness = src.m_TargetSoftness;
            }

            if (!append || src.m_BlendType != BlendType.AlphaBlend)
            {
                dst.m_BlendType = src.m_BlendType;
                (dst.m_SrcBlendMode, dst.m_DstBlendMode) =
                    (dst.m_BlendType, src.m_SrcBlendMode, src.m_DstBlendMode).Convert();
            }

            if (!append || src.m_ShadowMode != ShadowMode.None)
            {
                dst.m_ShadowMode = src.m_ShadowMode;
                dst.m_ShadowDistance = src.m_ShadowDistance;
                dst.m_ShadowIteration = src.m_ShadowIteration;
                dst.m_ShadowFade = src.m_ShadowFade;
                dst.m_ShadowMirrorScale = src.m_ShadowMirrorScale;
                dst.m_ShadowBlurIntensity = src.m_ShadowBlurIntensity;
                dst.m_ShadowColorFilter = src.m_ShadowColorFilter;
                dst.m_ShadowColor = src.m_ShadowColor;
                dst.m_ShadowColorGlow = src.m_ShadowColorGlow;
            }

            if (!append || src.m_EdgeMode != EdgeMode.None)
            {
                dst.m_EdgeMode = src.m_EdgeMode;
                dst.m_EdgeShinyRate = src.m_EdgeShinyRate;
                dst.m_EdgeWidth = src.m_EdgeWidth;
                dst.m_EdgeColorFilter = src.m_EdgeColorFilter;
                dst.m_EdgeColor = src.m_EdgeColor;
                dst.m_EdgeColorGlow = src.m_EdgeColorGlow;
                dst.m_EdgeShinyWidth = src.m_EdgeShinyWidth;
                dst.m_EdgeShinyAutoPlaySpeed = src.m_EdgeShinyAutoPlaySpeed;
                dst.m_PatternArea = src.m_PatternArea;
            }

            if (!append || src.m_GradationMode != GradationMode.None)
            {
                dst.m_GradationMode = src.m_GradationMode;
                dst.m_GradationIntensity = src.m_GradationIntensity;
                dst.m_GradationColorFilter = src.m_GradationColorFilter;
                dst.m_GradationColor1 = src.m_GradationColor1;
                dst.m_GradationColor2 = src.m_GradationColor2;
                dst.m_GradationColor3 = src.m_GradationColor3;
                dst.m_GradationColor4 = src.m_GradationColor4;
                dst.SetGradientKeys(src.m_GradationGradient);
                dst.m_GradationOffset = src.m_GradationOffset;
                dst.m_GradationScale = src.m_GradationScale;
                dst.m_GradationRotation = src.m_GradationRotation;
            }

            if (!append || src.m_DetailFilter != DetailFilter.None)
            {
                dst.m_DetailFilter = src.m_DetailFilter;
                dst.m_DetailIntensity = src.m_DetailIntensity;
                dst.m_DetailColor = src.m_DetailColor;
                dst.m_DetailThreshold = src.m_DetailThreshold;
                dst.m_DetailTex = src.m_DetailTex;
                dst.m_DetailTexScale = src.m_DetailTexScale;
                dst.m_DetailTexOffset = src.m_DetailTexOffset;
                dst.m_DetailTexSpeed = src.m_DetailTexSpeed;
            }

            if (!append || src.m_Flip != 0)
            {
                dst.m_Flip = append ? dst.m_Flip | src.m_Flip : src.m_Flip;
            }

            Misc.SetDirty(dst);
            UpdateContext(context);
            SetVerticesDirty();
            SetMaterialDirty();
        }

        public void SavePreset(UIEffectPreset dst, bool append)
        {
            if (!dst) return;

            var src = this;
            if (!append || src.m_ToneFilter != ToneFilter.None)
            {
                dst.m_ToneFilter = src.m_ToneFilter;
                dst.m_ToneIntensity = src.m_ToneIntensity;
            }

            if (!append || src.m_ColorFilter != ColorFilter.None)
            {
                dst.m_ColorFilter = src.m_ColorFilter;
                dst.m_Color = src.m_Color;
                dst.m_ColorIntensity = src.m_ColorIntensity;
                dst.m_ColorGlow = src.m_ColorGlow;
            }

            if (!append || src.m_SamplingFilter != SamplingFilter.None)
            {
                dst.m_SamplingFilter = src.m_SamplingFilter;
                dst.m_SamplingIntensity = src.m_SamplingIntensity;
                dst.m_SamplingWidth = src.m_SamplingWidth;
            }

            if (!append || src.m_TransitionFilter != TransitionFilter.None)
            {
                dst.m_TransitionFilter = src.m_TransitionFilter;
                dst.m_TransitionRate = src.m_TransitionRate;
                dst.m_TransitionReverse = src.m_TransitionReverse;
                dst.m_TransitionTex = src.m_TransitionTex;
                dst.m_TransitionTexScale = src.m_TransitionTexScale;
                dst.m_TransitionTexOffset = src.m_TransitionTexOffset;
                dst.m_TransitionTexSpeed = src.m_TransitionTexSpeed;
                dst.m_TransitionRotation = src.m_TransitionRotation;
                dst.m_TransitionKeepAspectRatio = src.m_TransitionKeepAspectRatio;
                dst.m_TransitionWidth = src.m_TransitionWidth;
                dst.m_TransitionSoftness = src.m_TransitionSoftness;
                dst.m_TransitionRange = src.m_TransitionRange;
                dst.m_TransitionColorFilter = src.m_TransitionColorFilter;
                dst.m_TransitionColor = src.m_TransitionColor;
                dst.m_TransitionColorGlow = src.m_TransitionColorGlow;
                dst.m_TransitionPatternReverse = src.m_TransitionPatternReverse;
                dst.m_TransitionAutoPlaySpeed = src.m_TransitionAutoPlaySpeed;
            }

            if (!append || src.m_TargetMode != TargetMode.None)
            {
                dst.m_TargetMode = src.m_TargetMode;
                dst.m_TargetColor = src.m_TargetColor;
                dst.m_TargetRange = src.m_TargetRange;
                dst.m_TargetSoftness = src.m_TargetSoftness;
            }

            if (!append || src.m_BlendType != BlendType.AlphaBlend)
            {
                dst.m_BlendType = src.m_BlendType;
                (dst.m_SrcBlendMode, dst.m_DstBlendMode) =
                    (dst.m_BlendType, src.m_SrcBlendMode, src.m_DstBlendMode).Convert();
            }

            if (!append || src.m_ShadowMode != ShadowMode.None)
            {
                dst.m_ShadowMode = src.m_ShadowMode;
                dst.m_ShadowDistance = src.m_ShadowDistance;
                dst.m_ShadowIteration = src.m_ShadowIteration;
                dst.m_ShadowFade = src.m_ShadowFade;
                dst.m_ShadowMirrorScale = src.m_ShadowMirrorScale;
                dst.m_ShadowBlurIntensity = src.m_ShadowBlurIntensity;
                dst.m_ShadowColorFilter = src.m_ShadowColorFilter;
                dst.m_ShadowColor = src.m_ShadowColor;
                dst.m_ShadowColorGlow = src.m_ShadowColorGlow;
            }

            if (!append || src.m_EdgeMode != EdgeMode.None)
            {
                dst.m_EdgeMode = src.m_EdgeMode;
                dst.m_EdgeShinyRate = src.m_EdgeShinyRate;
                dst.m_EdgeWidth = src.m_EdgeWidth;
                dst.m_EdgeColorFilter = src.m_EdgeColorFilter;
                dst.m_EdgeColor = src.m_EdgeColor;
                dst.m_EdgeColorGlow = src.m_EdgeColorGlow;
                dst.m_EdgeShinyWidth = src.m_EdgeShinyWidth;
                dst.m_EdgeShinyAutoPlaySpeed = src.m_EdgeShinyAutoPlaySpeed;
                dst.m_PatternArea = src.m_PatternArea;
            }

            if (!append || src.m_GradationMode != GradationMode.None)
            {
                dst.m_GradationMode = src.m_GradationMode;
                dst.m_GradationIntensity = src.m_GradationIntensity;
                dst.m_GradationColorFilter = src.m_GradationColorFilter;
                dst.m_GradationColor1 = src.m_GradationColor1;
                dst.m_GradationColor2 = src.m_GradationColor2;
                dst.m_GradationColor3 = src.m_GradationColor3;
                dst.m_GradationColor4 = src.m_GradationColor4;
                dst.m_GradationGradient.SetKeys(src.m_GradationGradient.colorKeys, src.m_GradationGradient.alphaKeys);
                dst.m_GradationGradient.mode = src.m_GradationGradient.mode;
                dst.m_GradationOffset = src.m_GradationOffset;
                dst.m_GradationScale = src.m_GradationScale;
                dst.m_GradationRotation = src.m_GradationRotation;
            }

            if (!append || src.m_DetailFilter != DetailFilter.None)
            {
                dst.m_DetailFilter = src.m_DetailFilter;
                dst.m_DetailIntensity = src.m_DetailIntensity;
                dst.m_DetailColor = src.m_DetailColor;
                dst.m_DetailThreshold = src.m_DetailThreshold;
                dst.m_DetailTex = src.m_DetailTex;
                dst.m_DetailTexScale = src.m_DetailTexScale;
                dst.m_DetailTexOffset = src.m_DetailTexOffset;
                dst.m_DetailTexSpeed = src.m_DetailTexSpeed;
            }

            if (!append || src.m_Flip != 0)
            {
                dst.m_Flip = append ? dst.m_Flip | src.m_Flip : src.m_Flip;
            }

            Misc.SetDirty(dst);
        }

        internal override void UpdateContext(UIEffectContext dst)
        {
            var src = this;
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
            dst.m_TransitionRotation = src.m_TransitionRotation;
            dst.m_TransitionKeepAspectRatio = src.m_TransitionKeepAspectRatio;
            dst.m_TransitionWidth = src.m_TransitionWidth;
            dst.m_TransitionSoftness = src.m_TransitionSoftness;
            dst.m_TransitionRange = src.m_TransitionRange;
            dst.m_TransitionColorFilter = src.m_TransitionColorFilter;
            dst.m_TransitionColor = src.m_TransitionColor;
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
            dst.m_EdgeShinyWidth = src.m_EdgeShinyWidth;
            dst.m_EdgeShinyAutoPlaySpeed = src.m_EdgeShinyAutoPlaySpeed;
            dst.m_PatternArea = src.m_PatternArea;

            dst.m_GradationMode = src.m_GradationMode;
            dst.m_GradationIntensity = src.m_GradationIntensity;
            dst.m_GradationColorFilter = src.m_GradationColorFilter;
            dst.m_GradationColor1 = src.m_GradationColor1;
            dst.m_GradationColor2 = src.m_GradationColor2;
            dst.m_GradationColor3 = src.m_GradationColor3;
            dst.m_GradationColor4 = src.m_GradationColor4;
            dst.m_GradationGradient = src.m_GradationGradient;
            dst.m_GradationOffset = src.m_GradationOffset;
            dst.m_GradationScale = src.m_GradationScale;
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
    }
}
