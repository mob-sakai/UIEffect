using System.Collections.Generic;
using Coffee.UIEffectInternal;
using UnityEngine;
using UnityEngine.Rendering;

namespace Coffee.UIEffects
{
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
        protected Vector4 m_ToneParams = new Vector4(0, 0, 0, 0);

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
        [Range(0, 360)]
        private float m_TransitionRotation = 0;

        [SerializeField]
        protected bool m_TransitionKeepAspectRatio = true;

        [Range(0, 1)]
        [SerializeField]
        protected float m_TransitionWidth = 0.2f;

        [Range(0, 1)]
        [SerializeField]
        protected float m_TransitionSoftness = 0.2f;

        [SerializeField]
        protected ColorFilter m_TransitionColorFilter = ColorFilter.MultiplyAdditive;

        [SerializeField]
        protected Color m_TransitionColor = new Color(0f, 0.5f, 1.0f, 1.0f);

        [SerializeField]
        protected bool m_TransitionColorGlow = false;

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

        [SerializeField]
        protected Color m_GradationColor1 = Color.white;

        [SerializeField]
        protected Color m_GradationColor2 = Color.white;

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

        /// <summary>
        /// Tone filter for rendering.
        /// </summary>
        public ToneFilter toneFilter
        {
            get => m_ToneFilter;
            set
            {
                if (m_ToneFilter == value) return;
                context.toneFilter = m_ToneFilter = value;
                ApplyContextToMaterial();
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
                context.toneIntensity = m_ToneIntensity = value;
                ApplyContextToMaterial();
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
                context.colorFilter = m_ColorFilter = value;
                ApplyContextToMaterial();
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
                context.colorIntensity = m_ColorIntensity = value;
                ApplyContextToMaterial();
            }
        }

        public Color color
        {
            get => m_Color;
            set
            {
                m_Color.a = 1;
                if (m_Color == value) return;
                context.color = m_Color = value;
                ApplyContextToMaterial();
            }
        }

        public bool colorGlow
        {
            get => m_ColorGlow;
            set
            {
                if (m_ColorGlow == value) return;
                context.colorGlow = m_ColorGlow = value;
                ApplyContextToMaterial();
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
                context.samplingFilter = m_SamplingFilter = value;
                SetVerticesDirty();
                ApplyContextToMaterial();
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
                context.samplingIntensity = m_SamplingIntensity = value;
                ApplyContextToMaterial();
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
                ApplyContextToMaterial();
            }
        }

        public override float actualSamplingScale => samplingFilter != SamplingFilter.None
            ? Mathf.Clamp(m_SamplingScale, 0.01f, 100)
            : 1;

        /// <summary>
        /// Transition filter for rendering.
        /// </summary>
        public TransitionFilter transitionFilter
        {
            get => m_TransitionFilter;
            set
            {
                if (m_TransitionFilter == value) return;
                context.transitionFilter = m_TransitionFilter = value;
                SetVerticesDirty();
                ApplyContextToMaterial();
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
                context.transitionRate = m_TransitionRate = value;
                ApplyContextToMaterial();
            }
        }

        public bool transitionReverse
        {
            get => m_TransitionReverse;
            set
            {
                if (m_TransitionReverse == value) return;
                context.transitionReverse = m_TransitionReverse = value;
                ApplyContextToMaterial();
            }
        }

        public Texture transitionTexture
        {
            get => m_TransitionTex;
            set
            {
                if (m_TransitionTex == value) return;
                context.transitionTex = m_TransitionTex = value;
                ApplyContextToMaterial();
            }
        }

        public Vector2 transitionTextureScale
        {
            get => m_TransitionTexScale;
            set
            {
                if (m_TransitionTexScale == value) return;
                context.transitionTexScale = m_TransitionTexScale = value;
                ApplyContextToMaterial();
            }
        }

        public Vector2 transitionTextureOffset
        {
            get => m_TransitionTexOffset;
            set
            {
                if (m_TransitionTexOffset == value) return;
                context.transitionTexOffset = m_TransitionTexOffset = value;
                ApplyContextToMaterial();
            }
        }

        public float transitionRotation
        {
            get => m_TransitionRotation;
            set
            {
                if (Mathf.Approximately(m_TransitionRotation, value)) return;
                context.transitionRotation = m_TransitionRotation = value;
                SetVerticesDirty();
            }
        }

        public bool transitionKeepAspectRatio
        {
            get => m_TransitionKeepAspectRatio;
            set
            {
                if (m_TransitionKeepAspectRatio == value) return;
                context.transitionKeepAspectRatio = m_TransitionKeepAspectRatio = value;
                SetVerticesDirty();
            }
        }

        public float transitionWidth
        {
            get => m_TransitionWidth;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_TransitionWidth, value)) return;
                context.transitionWidth = m_TransitionWidth = value;
                ApplyContextToMaterial();
            }
        }

        public float transitionSoftness
        {
            get => m_TransitionSoftness;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_TransitionSoftness, value)) return;
                context.transitionSoftness = m_TransitionSoftness = value;
                ApplyContextToMaterial();
            }
        }

        public ColorFilter transitionColorFilter
        {
            get => m_TransitionColorFilter;
            set
            {
                if (m_TransitionColorFilter == value) return;
                context.transitionColorFilter = m_TransitionColorFilter = value;
                ApplyContextToMaterial();
            }
        }

        public Color transitionColor
        {
            get => m_TransitionColor;
            set
            {
                if (m_TransitionColor == value) return;
                context.transitionColor = m_TransitionColor = value;
                ApplyContextToMaterial();
            }
        }

        public bool transitionColorGlow
        {
            get => m_TransitionColorGlow;
            set
            {
                if (m_TransitionColorGlow == value) return;
                context.transitionColorGlow = m_TransitionColorGlow = value;
                ApplyContextToMaterial();
            }
        }

        public TargetMode targetMode
        {
            get => m_TargetMode;
            set
            {
                if (m_TargetMode == value) return;
                context.targetMode = m_TargetMode = value;
                ApplyContextToMaterial();
            }
        }

        public Color targetColor
        {
            get => m_TargetColor;
            set
            {
                if (m_TargetColor == value) return;
                context.targetColor = m_TargetColor = value;
                ApplyContextToMaterial();
            }
        }

        public float targetRange
        {
            get => m_TargetRange;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_TargetRange, value)) return;
                context.targetRange = m_TargetRange = value;
                ApplyContextToMaterial();
            }
        }

        public float targetSoftness
        {
            get => m_TargetSoftness;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_TargetSoftness, value)) return;
                context.targetSoftness = m_TargetSoftness = value;
                ApplyContextToMaterial();
            }
        }

        public BlendType blendType
        {
            get => m_BlendType;
            set
            {
                if (m_BlendType == value) return;
                (m_SrcBlendMode, m_DstBlendMode) = (m_BlendType, m_SrcBlendMode, m_DstBlendMode).Convert();
                context.srcBlendMode = m_SrcBlendMode;
                context.dstBlendMode = m_DstBlendMode;
                ApplyContextToMaterial();
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
                context.srcBlendMode = m_SrcBlendMode = value;
                m_BlendType = (m_SrcBlendMode, m_DstBlendMode).Convert();
                ApplyContextToMaterial();
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
                context.dstBlendMode = m_DstBlendMode = value;
                m_BlendType = (m_SrcBlendMode, m_DstBlendMode).Convert();
                ApplyContextToMaterial();
            }
        }

        public ShadowMode shadowMode
        {
            get => m_ShadowMode;
            set
            {
                if (m_ShadowMode == value) return;
                context.shadowMode = m_ShadowMode = value;
                SetVerticesDirty();
            }
        }

        public Vector2 shadowDistance
        {
            get => m_ShadowDistance;
            set
            {
                if (m_ShadowDistance == value) return;
                context.shadowDistance = m_ShadowDistance = value;
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
                context.shadowFade = m_ShadowFade = value;
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
                context.shadowIteration = m_ShadowIteration = value;
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
                context.shadowMirrorScale = m_ShadowMirrorScale = value;
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
                context.shadowBlurIntensity = m_ShadowBlurIntensity = value;
                ApplyContextToMaterial();
            }
        }

        public ColorFilter shadowColorFilter
        {
            get => m_ShadowColorFilter;
            set
            {
                if (m_ShadowColorFilter == value) return;
                context.shadowColorFilter = m_ShadowColorFilter = value;
                ApplyContextToMaterial();
            }
        }

        public Color shadowColor
        {
            get => m_ShadowColor;
            set
            {
                if (m_ShadowColor == value) return;
                context.shadowColor = m_ShadowColor = value;
                ApplyContextToMaterial();
            }
        }

        public bool shadowGlow
        {
            get => m_ShadowColorGlow;
            set
            {
                if (m_ShadowColorGlow == value) return;
                context.shadowColorGlow = m_ShadowColorGlow = value;
                ApplyContextToMaterial();
            }
        }

        public GradationMode gradationMode
        {
            get => m_GradationMode;
            set
            {
                if (m_GradationMode == value) return;
                context.gradationMode = m_GradationMode = value;
                SetVerticesDirty();
            }
        }

        public Color gradationColor1
        {
            get => m_GradationColor1;
            set
            {
                if (m_GradationColor1 == value) return;
                context.gradationColor1 = m_GradationColor1 = value;
                SetVerticesDirty();
            }
        }

        public Color gradationColor2
        {
            get => m_GradationColor2;
            set
            {
                if (m_GradationColor2 == value) return;
                context.gradationColor2 = m_GradationColor2 = value;
                SetVerticesDirty();
            }
        }

        public float gradationOffset
        {
            get => m_GradationOffset;
            set
            {
                if (Mathf.Approximately(m_GradationOffset, value)) return;
                context.gradationOffset = m_GradationOffset = value;
                SetVerticesDirty();
            }
        }

        public float gradationScale
        {
            get => m_GradationScale;
            set
            {
                if (Mathf.Approximately(m_GradationScale, value)) return;
                context.gradationScale = m_GradationScale = value;
                SetVerticesDirty();
            }
        }

        public float gradationRotation
        {
            get => m_GradationRotation;
            set
            {
                value = Mathf.Repeat(value, 360);
                if (Mathf.Approximately(m_GradationRotation, value)) return;
                context.gradationRotation = m_GradationRotation = value;
                SetVerticesDirty();
            }
        }

        public bool allowToModifyMeshShape
        {
            get => m_AllowToModifyMeshShape;
            set
            {
                if (m_AllowToModifyMeshShape == value) return;
                context.allowToModifyMeshShape = m_AllowToModifyMeshShape = value;
                SetVerticesDirty();
            }
        }

        public List<UIEffectReplica> replicas => _replicas ??= InternalListPool<UIEffectReplica>.Rent();
        private List<UIEffectReplica> _replicas;

        protected override void OnEnable()
        {
            (m_SrcBlendMode, m_DstBlendMode) = (m_BlendType, m_SrcBlendMode, m_DstBlendMode).Convert();
            base.OnEnable();
            ApplyContextToMaterial();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ApplyContextToMaterial();
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
#endif

        public override void SetVerticesDirty()
        {
            base.SetVerticesDirty();
            replicas.ForEach(c =>
            {
                if (!c) return;
                c.SetVerticesDirty();
            });
        }

        public override void SetMaterialDirty()
        {
            base.SetMaterialDirty();
            replicas.ForEach(c =>
            {
                if (!c) return;
                c.SetMaterialDirty();
            });
        }

        /// <summary>
        /// Set gradation gradient's keys.
        /// </summary>
        public void SetGradientKeys(GradientColorKey[] colorKeys, GradientAlphaKey[] alphaKeys)
        {
            m_GradationGradient ??= new Gradient();
            m_GradationGradient.SetKeys(colorKeys, alphaKeys);
            context?.SetGradationDirty();
        }

        protected override void UpdateContext(UIEffectContext c)
        {
            c.toneFilter = m_ToneFilter;
            c.toneIntensity = m_ToneIntensity;
            c.toneParams = m_ToneParams;

            c.colorFilter = m_ColorFilter;
            c.color = m_Color;
            c.colorIntensity = m_ColorIntensity;
            c.colorGlow = m_ColorGlow;

            c.samplingFilter = m_SamplingFilter;
            c.samplingIntensity = m_SamplingIntensity;

            c.transitionFilter = m_TransitionFilter;
            c.transitionRate = m_TransitionRate;
            c.transitionReverse = m_TransitionReverse;
            c.transitionTex = m_TransitionTex;
            c.transitionTexScale = m_TransitionTexScale;
            c.transitionTexOffset = m_TransitionTexOffset;
            c.transitionRotation = m_TransitionRotation;
            c.transitionKeepAspectRatio = m_TransitionKeepAspectRatio;
            c.transitionWidth = m_TransitionWidth;
            c.transitionSoftness = m_TransitionSoftness;
            c.transitionColorFilter = m_TransitionColorFilter;
            c.transitionColor = m_TransitionColor;
            c.transitionColorGlow = m_TransitionColorGlow;

            c.targetMode = m_TargetMode;
            c.targetColor = m_TargetColor;
            c.targetRange = m_TargetRange;
            c.targetSoftness = m_TargetSoftness;

            c.srcBlendMode = m_SrcBlendMode;
            c.dstBlendMode = m_DstBlendMode;

            c.shadowMode = m_ShadowMode;
            c.shadowDistance = m_ShadowDistance;
            c.shadowIteration = m_ShadowIteration;
            c.shadowFade = m_ShadowFade;
            c.shadowMirrorScale = m_ShadowMirrorScale;
            c.shadowBlurIntensity = m_ShadowBlurIntensity;
            c.shadowColorFilter = m_ShadowColorFilter;
            c.shadowColor = m_ShadowColor;
            c.shadowColorGlow = m_ShadowColorGlow;

            c.gradationMode = m_GradationMode;
            c.gradationColor1 = m_GradationColor1;
            c.gradationColor2 = m_GradationColor2;
            c.gradationGradient = m_GradationGradient;
            c.gradationOffset = m_GradationOffset;
            c.gradationScale = m_GradationScale;
            c.gradationRotation = m_GradationRotation;

            c.allowToModifyMeshShape = m_AllowToModifyMeshShape;
        }

        public override void ApplyContextToMaterial()
        {
            base.ApplyContextToMaterial();
            replicas.ForEach(c =>
            {
                if (!c) return;
                c.ApplyContextToMaterial();
            });
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
        }

        public override bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            switch (transitionFilter)
            {
                case TransitionFilter.None:
                case TransitionFilter.Shiny:
                case TransitionFilter.Mask:
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
            LoadPreset(UIEffectProjectSettings.LoadRuntimePreset(presetName));
        }

        /// <summary>
        /// Load preset settings.
        /// </summary>
        public void LoadPreset(UIEffect preset)
        {
            if (!preset) return;

            m_ToneFilter = preset.m_ToneFilter;
            m_ToneIntensity = preset.m_ToneIntensity;
            m_ToneParams = preset.m_ToneParams;

            m_ColorFilter = preset.m_ColorFilter;
            m_Color = preset.m_Color;
            m_ColorIntensity = preset.m_ColorIntensity;
            m_ColorGlow = preset.m_ColorGlow;

            m_SamplingFilter = preset.m_SamplingFilter;
            m_SamplingIntensity = preset.m_SamplingIntensity;

            m_TransitionFilter = preset.m_TransitionFilter;
            m_TransitionRate = preset.m_TransitionRate;
            m_TransitionReverse = preset.m_TransitionReverse;
            m_TransitionTex = preset.m_TransitionTex;
            m_TransitionTexScale = preset.m_TransitionTexScale;
            m_TransitionTexOffset = preset.m_TransitionTexOffset;
            m_TransitionRotation = preset.m_TransitionRotation;
            m_TransitionKeepAspectRatio = preset.m_TransitionKeepAspectRatio;
            m_TransitionWidth = preset.m_TransitionWidth;
            m_TransitionSoftness = preset.m_TransitionSoftness;
            m_TransitionColorFilter = preset.m_TransitionColorFilter;
            m_TransitionColor = preset.m_TransitionColor;
            m_TransitionColorGlow = preset.m_TransitionColorGlow;

            m_TargetMode = preset.m_TargetMode;
            m_TargetColor = preset.m_TargetColor;
            m_TargetRange = preset.m_TargetRange;
            m_TargetSoftness = preset.m_TargetSoftness;

            m_BlendType = preset.m_BlendType;
            (m_SrcBlendMode, m_DstBlendMode) = (m_BlendType, preset.m_SrcBlendMode, preset.m_DstBlendMode).Convert();

            m_ShadowMode = preset.m_ShadowMode;
            m_ShadowDistance = preset.m_ShadowDistance;
            m_ShadowIteration = preset.m_ShadowIteration;
            m_ShadowFade = preset.m_ShadowFade;
            m_ShadowMirrorScale = preset.m_ShadowMirrorScale;
            m_ShadowBlurIntensity = preset.m_ShadowBlurIntensity;
            m_ShadowColorFilter = preset.m_ShadowColorFilter;
            m_ShadowColor = preset.m_ShadowColor;
            m_ShadowColorGlow = preset.m_ShadowColorGlow;

            UpdateContext(context);
            ApplyContextToMaterial();
            SetVerticesDirty();
            SetMaterialDirty();
        }

        internal void CopyFrom(UIEffectContext c)
        {
            m_ToneFilter = c.toneFilter;
            m_ToneIntensity = c.toneIntensity;
            m_ToneParams = c.toneParams;

            m_ColorFilter = c.colorFilter;
            m_Color = c.color;
            m_ColorIntensity = c.colorIntensity;
            m_ColorGlow = c.colorGlow;

            m_SamplingFilter = c.samplingFilter;
            m_SamplingIntensity = c.samplingIntensity;

            m_TransitionFilter = c.transitionFilter;
            m_TransitionRate = c.transitionRate;
            m_TransitionReverse = c.transitionReverse;
            m_TransitionTex = c.transitionTex;
            m_TransitionTexScale = c.transitionTexScale;
            m_TransitionTexOffset = c.transitionTexOffset;
            m_TransitionRotation = c.transitionRotation;
            m_TransitionKeepAspectRatio = c.transitionKeepAspectRatio;
            m_TransitionWidth = c.transitionWidth;
            m_TransitionSoftness = c.transitionSoftness;
            m_TransitionColorFilter = c.transitionColorFilter;
            m_TransitionColor = c.transitionColor;
            m_TransitionColorGlow = c.transitionColorGlow;

            m_TargetMode = c.targetMode;
            m_TargetColor = c.targetColor;
            m_TargetRange = c.targetRange;
            m_TargetSoftness = c.targetSoftness;

            m_SrcBlendMode = c.srcBlendMode;
            m_DstBlendMode = c.dstBlendMode;
            m_BlendType = (m_SrcBlendMode, m_DstBlendMode).Convert();

            m_ShadowMode = c.shadowMode;
            m_ShadowDistance = c.shadowDistance;
            m_ShadowIteration = c.shadowIteration;
            m_ShadowFade = c.shadowFade;
            m_ShadowMirrorScale = c.shadowMirrorScale;
            m_ShadowBlurIntensity = c.shadowBlurIntensity;
            m_ShadowColorFilter = c.shadowColorFilter;
            m_ShadowColor = c.shadowColor;
            m_ShadowColorGlow = c.shadowColorGlow;

            m_GradationMode = c.gradationMode;
            m_GradationColor1 = c.gradationColor1;
            m_GradationColor2 = c.gradationColor2;
            m_GradationGradient = c.gradationGradient;
            m_GradationOffset = c.gradationOffset;
            m_GradationScale = c.gradationScale;
            m_GradationRotation = c.gradationRotation;

            m_AllowToModifyMeshShape = c.allowToModifyMeshShape;

            UpdateContext(context);
            ApplyContextToMaterial();
            SetVerticesDirty();
            SetMaterialDirty();
        }
    }
}
