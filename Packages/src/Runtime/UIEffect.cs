using System.Collections.Generic;
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
        protected RectTransform m_CustomRoot = null;

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
                context.toneIntensity = m_ToneIntensity = value;
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
                context.colorFilter = m_ColorFilter = value;
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
                context.colorIntensity = m_ColorIntensity = value;
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
                context.color = m_Color = value;
                SetMaterialDirty();
            }
        }

        public bool colorGlow
        {
            get => m_ColorGlow;
            set
            {
                if (m_ColorGlow == value) return;
                context.colorGlow = m_ColorGlow = value;
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
                context.samplingFilter = m_SamplingFilter = value;
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
                context.samplingIntensity = m_SamplingIntensity = value;
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
                context.samplingWidth = m_SamplingWidth = value;
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
                context.transitionFilter = m_TransitionFilter = value;
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
                context.transitionRate = m_TransitionRate = value;
                SetMaterialDirty();
            }
        }

        public bool transitionReverse
        {
            get => m_TransitionReverse;
            set
            {
                if (m_TransitionReverse == value) return;
                context.transitionReverse = m_TransitionReverse = value;
                SetMaterialDirty();
            }
        }

        public Texture transitionTexture
        {
            get => m_TransitionTex;
            set
            {
                if (m_TransitionTex == value) return;
                context.transitionTex = m_TransitionTex = value;
                SetMaterialDirty();
            }
        }

        public Vector2 transitionTextureScale
        {
            get => m_TransitionTexScale;
            set
            {
                if (m_TransitionTexScale == value) return;
                context.transitionTexScale = m_TransitionTexScale = value;
                SetMaterialDirty();
            }
        }

        public Vector2 transitionTextureOffset
        {
            get => m_TransitionTexOffset;
            set
            {
                if (m_TransitionTexOffset == value) return;
                context.transitionTexOffset = m_TransitionTexOffset = value;
                SetMaterialDirty();
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
                context.transitionSoftness = m_TransitionSoftness = value;
                SetMaterialDirty();
            }
        }

        public MinMax01 transitionRange
        {
            get => m_TransitionRange;
            set
            {
                if (m_TransitionRange.Approximately(value)) return;
                context.transitionRange = m_TransitionRange = value;
                SetMaterialDirty();
            }
        }

        public ColorFilter transitionColorFilter
        {
            get => m_TransitionColorFilter;
            set
            {
                if (m_TransitionColorFilter == value) return;
                context.transitionColorFilter = m_TransitionColorFilter = value;
                SetMaterialDirty();
            }
        }

        public Color transitionColor
        {
            get => m_TransitionColor;
            set
            {
                if (m_TransitionColor == value) return;
                context.transitionColor = m_TransitionColor = value;
                SetMaterialDirty();
            }
        }

        public bool transitionColorGlow
        {
            get => m_TransitionColorGlow;
            set
            {
                if (m_TransitionColorGlow == value) return;
                context.transitionColorGlow = m_TransitionColorGlow = value;
                SetMaterialDirty();
            }
        }

        public bool transitionPatternReverse
        {
            get => m_TransitionPatternReverse;
            set
            {
                if (m_TransitionPatternReverse == value) return;
                context.transitionPatternReverse = m_TransitionPatternReverse = value;
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
                context.transitionAutoPlaySpeed = m_TransitionAutoPlaySpeed = value;
                SetMaterialDirty();
            }
        }

        public TargetMode targetMode
        {
            get => m_TargetMode;
            set
            {
                if (m_TargetMode == value) return;
                context.targetMode = m_TargetMode = value;
                SetMaterialDirty();
            }
        }

        public Color targetColor
        {
            get => m_TargetColor;
            set
            {
                if (m_TargetColor == value) return;
                context.targetColor = m_TargetColor = value;
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
                context.targetRange = m_TargetRange = value;
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
                context.targetSoftness = m_TargetSoftness = value;
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
                context.srcBlendMode = m_SrcBlendMode;
                context.dstBlendMode = m_DstBlendMode;
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
                context.srcBlendMode = m_SrcBlendMode = value;
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
                context.dstBlendMode = m_DstBlendMode = value;
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
                SetMaterialDirty();
            }
        }

        public ColorFilter shadowColorFilter
        {
            get => m_ShadowColorFilter;
            set
            {
                if (m_ShadowColorFilter == value) return;
                context.shadowColorFilter = m_ShadowColorFilter = value;
                SetMaterialDirty();
            }
        }

        public Color shadowColor
        {
            get => m_ShadowColor;
            set
            {
                if (m_ShadowColor == value) return;
                context.shadowColor = m_ShadowColor = value;
                SetMaterialDirty();
            }
        }

        public bool shadowGlow
        {
            get => m_ShadowColorGlow;
            set
            {
                if (m_ShadowColorGlow == value) return;
                context.shadowColorGlow = m_ShadowColorGlow = value;
                SetMaterialDirty();
            }
        }

        public EdgeMode edgeMode
        {
            get => m_EdgeMode;
            set
            {
                if (m_EdgeMode == value) return;
                context.edgeMode = m_EdgeMode = value;
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
                context.edgeShinyRate = m_EdgeShinyRate = value;
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
                context.edgeWidth = m_EdgeWidth = value;
                SetMaterialDirty();
            }
        }

        public ColorFilter edgeColorFilter
        {
            get => m_EdgeColorFilter;
            set
            {
                if (m_EdgeColorFilter == value) return;
                context.edgeColorFilter = m_EdgeColorFilter = value;
                SetMaterialDirty();
            }
        }

        public Color edgeColor
        {
            get => m_EdgeColor;
            set
            {
                if (m_EdgeColor == value) return;
                context.edgeColor = m_EdgeColor = value;
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
                context.edgeShinyWidth = m_EdgeShinyWidth = value;
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
                context.edgeShinyAutoPlaySpeed = m_EdgeShinyAutoPlaySpeed = value;
                SetMaterialDirty();
            }
        }

        public PatternArea patternArea
        {
            get => m_PatternArea;
            set
            {
                if (m_PatternArea == value) return;
                context.patternArea = m_PatternArea = value;
                SetMaterialDirty();
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

        public Color gradationColor3
        {
            get => m_GradationColor3;
            set
            {
                if (m_GradationColor3 == value) return;
                context.gradationColor3 = m_GradationColor3 = value;
                SetVerticesDirty();
            }
        }

        public Color gradationColor4
        {
            get => m_GradationColor4;
            set
            {
                if (m_GradationColor4 == value) return;
                context.gradationColor4 = m_GradationColor4 = value;
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
                m_AllowToModifyMeshShape = value;
                SetVerticesDirty();
            }
        }

        public RectTransform customRoot
        {
            get => m_CustomRoot;
            set
            {
                if (m_CustomRoot == value) return;
                m_CustomRoot = value;
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
#endif

        protected override void OnBeforeCanvasRebuild()
        {
            base.OnBeforeCanvasRebuild();
            if (CheckTransformChangedWith(customRoot))
            {
                SetVerticesDirty();
            }
        }

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
        public void SetGradientKeys(GradientColorKey[] colorKeys, GradientAlphaKey[] alphaKeys,
            GradientMode mode = GradientMode.Blend)
        {
            m_GradationGradient ??= new Gradient();
            m_GradationGradient.SetKeys(colorKeys, alphaKeys);
            m_GradationGradient.mode = mode;
            context?.SetGradationDirty();
            SetVerticesDirty();
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
            c.samplingWidth = m_SamplingWidth;

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
            c.transitionRange = m_TransitionRange;
            c.transitionColorFilter = m_TransitionColorFilter;
            c.transitionColor = m_TransitionColor;
            c.transitionColorGlow = m_TransitionColorGlow;
            c.transitionPatternReverse = m_TransitionPatternReverse;
            c.transitionAutoPlaySpeed = m_TransitionAutoPlaySpeed;

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

            c.edgeMode = m_EdgeMode;
            c.edgeShinyRate = m_EdgeShinyRate;
            c.edgeWidth = m_EdgeWidth;
            c.edgeColorFilter = m_EdgeColorFilter;
            c.edgeColor = m_EdgeColor;
            c.edgeShinyWidth = m_EdgeShinyWidth;
            c.edgeShinyAutoPlaySpeed = m_EdgeShinyAutoPlaySpeed;
            c.patternArea = m_PatternArea;

            c.gradationMode = m_GradationMode;
            c.gradationColor1 = m_GradationColor1;
            c.gradationColor2 = m_GradationColor2;
            c.gradationColor3 = m_GradationColor3;
            c.gradationColor4 = m_GradationColor4;
            c.gradationGradient = m_GradationGradient;
            c.gradationOffset = m_GradationOffset;
            c.gradationScale = m_GradationScale;
            c.gradationRotation = m_GradationRotation;
        }

        public override void ApplyContextToMaterial(Material material)
        {
            base.ApplyContextToMaterial(material);
            replicas.ForEach(c =>
            {
                if (!c) return;
                c.ApplyContextToMaterial(material);
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
            LoadPreset(UIEffectProjectSettings.LoadRuntimePreset(presetName), append);
        }

        /// <summary>
        /// Load preset settings.
        /// </summary>
        public void LoadPreset(UIEffect preset)
        {
            LoadPreset(preset, false);
        }

        /// <summary>
        /// Load preset settings.
        /// </summary>
        public void LoadPreset(UIEffect preset, bool append)
        {
            if (!preset) return;

            if (!append || preset.m_ToneFilter != ToneFilter.None)
            {
                m_ToneFilter = preset.m_ToneFilter;
                m_ToneIntensity = preset.m_ToneIntensity;
                m_ToneParams = preset.m_ToneParams;
            }

            if (!append || preset.m_ColorFilter != ColorFilter.None)
            {
                m_ColorFilter = preset.m_ColorFilter;
                m_Color = preset.m_Color;
                m_ColorIntensity = preset.m_ColorIntensity;
                m_ColorGlow = preset.m_ColorGlow;
            }

            if (!append || preset.m_SamplingFilter != SamplingFilter.None)
            {
                m_SamplingFilter = preset.m_SamplingFilter;
                m_SamplingIntensity = preset.m_SamplingIntensity;
                m_SamplingWidth = preset.m_SamplingWidth;
            }

            if (!append || preset.m_TransitionFilter != TransitionFilter.None)
            {
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
                m_TransitionRange = preset.m_TransitionRange;
                m_TransitionColorFilter = preset.m_TransitionColorFilter;
                m_TransitionColor = preset.m_TransitionColor;
                m_TransitionColorGlow = preset.m_TransitionColorGlow;
                m_TransitionPatternReverse = preset.m_TransitionPatternReverse;
                m_TransitionAutoPlaySpeed = preset.m_TransitionAutoPlaySpeed;
            }

            if (!append || preset.m_TargetMode != TargetMode.None)
            {
                m_TargetMode = preset.m_TargetMode;
                m_TargetColor = preset.m_TargetColor;
                m_TargetRange = preset.m_TargetRange;
                m_TargetSoftness = preset.m_TargetSoftness;
            }


            if (!append || preset.m_BlendType != BlendType.AlphaBlend)
            {
                m_BlendType = preset.m_BlendType;
                (m_SrcBlendMode, m_DstBlendMode) =
                    (m_BlendType, preset.m_SrcBlendMode, preset.m_DstBlendMode).Convert();
            }

            if (!append || preset.m_ShadowMode != ShadowMode.None)
            {
                m_ShadowMode = preset.m_ShadowMode;
                m_ShadowDistance = preset.m_ShadowDistance;
                m_ShadowIteration = preset.m_ShadowIteration;
                m_ShadowFade = preset.m_ShadowFade;
                m_ShadowMirrorScale = preset.m_ShadowMirrorScale;
                m_ShadowBlurIntensity = preset.m_ShadowBlurIntensity;
                m_ShadowColorFilter = preset.m_ShadowColorFilter;
                m_ShadowColor = preset.m_ShadowColor;
                m_ShadowColorGlow = preset.m_ShadowColorGlow;
            }

            if (!append || preset.m_EdgeMode != EdgeMode.None)
            {
                m_EdgeMode = preset.m_EdgeMode;
                m_EdgeShinyRate = preset.m_EdgeShinyRate;
                m_EdgeWidth = preset.m_EdgeWidth;
                m_EdgeColorFilter = preset.m_EdgeColorFilter;
                m_EdgeColor = preset.m_EdgeColor;
                m_EdgeShinyWidth = preset.m_EdgeShinyWidth;
                m_EdgeShinyAutoPlaySpeed = preset.m_EdgeShinyAutoPlaySpeed;
                m_PatternArea = preset.m_PatternArea;
            }

            if (!append || preset.m_GradationMode != GradationMode.None)
            {
                m_GradationMode = preset.m_GradationMode;
                m_GradationColor1 = preset.m_GradationColor1;
                m_GradationColor2 = preset.m_GradationColor2;
                m_GradationColor3 = preset.m_GradationColor3;
                m_GradationColor4 = preset.m_GradationColor4;
                m_GradationGradient = new Gradient();
                m_GradationGradient.SetKeys(preset.m_GradationGradient.colorKeys, preset.m_GradationGradient.alphaKeys);
                m_GradationGradient.mode = preset.m_GradationGradient.mode;
                m_GradationOffset = preset.m_GradationOffset;
                m_GradationScale = preset.m_GradationScale;
                m_GradationRotation = preset.m_GradationRotation;
            }

            UpdateContext(context);
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
            m_SamplingWidth = c.samplingWidth;

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
            m_TransitionRange = c.transitionRange;
            m_TransitionColorFilter = c.transitionColorFilter;
            m_TransitionColor = c.transitionColor;
            m_TransitionColorGlow = c.transitionColorGlow;
            m_TransitionPatternReverse = c.transitionPatternReverse;
            m_TransitionAutoPlaySpeed = c.transitionAutoPlaySpeed;

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

            m_EdgeMode = c.edgeMode;
            m_EdgeShinyRate = c.edgeShinyRate;
            m_EdgeWidth = c.edgeWidth;
            m_EdgeColorFilter = c.edgeColorFilter;
            m_EdgeColor = c.edgeColor;
            m_EdgeShinyWidth = c.edgeShinyWidth;
            m_EdgeShinyAutoPlaySpeed = c.edgeShinyAutoPlaySpeed;
            m_PatternArea = c.patternArea;

            m_GradationMode = c.gradationMode;
            m_GradationColor1 = c.gradationColor1;
            m_GradationColor2 = c.gradationColor2;
            m_GradationColor3 = c.gradationColor3;
            m_GradationColor4 = c.gradationColor4;
            m_GradationGradient = new Gradient();
            m_GradationGradient.SetKeys(c.gradationGradient.colorKeys, c.gradationGradient.alphaKeys);
            m_GradationGradient.mode = c.gradationGradient.mode;
            m_GradationOffset = c.gradationOffset;
            m_GradationScale = c.gradationScale;
            m_GradationRotation = c.gradationRotation;

            UpdateContext(context);
            SetVerticesDirty();
            SetMaterialDirty();
        }
    }
}
