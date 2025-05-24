using Coffee.UIEffectInternal;
using UnityEngine;
using UnityEngine.Rendering;

namespace Coffee.UIEffects
{
    [CreateAssetMenu]
    [ExecuteAlways]
    public class UIEffectPreset : ScriptableObject
    {
        public ToneFilter m_ToneFilter = ToneFilter.None;
        [Range(0, 1)] public float m_ToneIntensity = 1;

        public ColorFilter m_ColorFilter = ColorFilter.None;
        [Range(0, 1)] public float m_ColorIntensity = 1;
        public Color m_Color = Color.white;
        public bool m_ColorGlow = false;

        public SamplingFilter m_SamplingFilter = SamplingFilter.None;
        [Range(0, 1)] public float m_SamplingIntensity = 0.5f;
        [Range(0.5f, 10f)] public float m_SamplingWidth = 1;

        public TransitionFilter m_TransitionFilter = TransitionFilter.None;
        [Range(0, 1)] public float m_TransitionRate = 0.5f;
        public bool m_TransitionReverse;
        public Texture m_TransitionTex;
        public Vector2 m_TransitionTexScale = new Vector2(1, 1);
        public Vector2 m_TransitionTexOffset = new Vector2(0, 0);
        public Vector2 m_TransitionTexSpeed = new Vector2(0, 0);
        [Range(0, 360)] public float m_TransitionRotation = 0;
        public bool m_TransitionKeepAspectRatio = true;
        [Range(0, 1)] public float m_TransitionWidth = 0.2f;
        [Range(0, 1)] public float m_TransitionSoftness = 0.2f;
        public MinMax01 m_TransitionRange = new MinMax01(0, 1);
        public ColorFilter m_TransitionColorFilter = ColorFilter.MultiplyAdditive;
        public Color m_TransitionColor = new Color(0f, 0.5f, 1.0f, 1.0f);
        public bool m_TransitionColorGlow = false;
        public bool m_TransitionPatternReverse = false;
        [Range(-5, 5)] public float m_TransitionAutoPlaySpeed = 0f;
        public Gradient m_TransitionGradient = new Gradient();

        public TargetMode m_TargetMode = TargetMode.None;
        public Color m_TargetColor = Color.white;
        [Range(0, 1)] public float m_TargetRange = 0.1f;
        [Range(0, 1)] public float m_TargetSoftness = 0.5f;

        public BlendType m_BlendType = BlendType.AlphaBlend;
        public BlendMode m_SrcBlendMode = BlendMode.One;
        public BlendMode m_DstBlendMode = BlendMode.OneMinusSrcAlpha;

        public ShadowMode m_ShadowMode = ShadowMode.None;
        public Vector2 m_ShadowDistance = new Vector2(1f, -1f);
        [Range(1, 5)] public int m_ShadowIteration = 1;
        [Range(0, 1)] public float m_ShadowFade = 0.9f;
        [Range(0, 2)] public float m_ShadowMirrorScale = 0.5f;
        [Range(0, 1)] public float m_ShadowBlurIntensity = 1;
        public ColorFilter m_ShadowColorFilter = ColorFilter.Replace;
        public Color m_ShadowColor = Color.white;
        public bool m_ShadowColorGlow = false;

        public GradationMode m_GradationMode = GradationMode.None;
        [Range(0, 1)] public float m_GradationIntensity = 1;
        public GradationColorFilter m_GradationColorFilter = GradationColorFilter.Multiply;
        public Color m_GradationColor1 = Color.white;
        public Color m_GradationColor2 = Color.white;
        public Color m_GradationColor3 = Color.white;
        public Color m_GradationColor4 = Color.white;
        public Gradient m_GradationGradient = new Gradient();
        [Range(-1, 1)] public float m_GradationOffset = 0;
        [PowerRange(0.01f, 10, 10)] public float m_GradationScale = 1;
        [Range(0, 360)] public float m_GradationRotation = 0;

        public EdgeMode m_EdgeMode = EdgeMode.None;
        [Range(0, 1)] public float m_EdgeWidth = 0.5f;
        public ColorFilter m_EdgeColorFilter = ColorFilter.Replace;
        public Color m_EdgeColor = Color.white;
        public bool m_EdgeColorGlow = false;
        [Range(0, 1)] public float m_EdgeShinyRate = 0.5f;
        [Range(0, 1)] public float m_EdgeShinyWidth = 0.5f;
        [Range(-5, 5)] public float m_EdgeShinyAutoPlaySpeed = 1f;
        public PatternArea m_PatternArea = PatternArea.Inner;

        public DetailFilter m_DetailFilter = DetailFilter.None;
        [Range(0, 1)] public float m_DetailIntensity = 1;
        public Color m_DetailColor = Color.white;
        public MinMax01 m_DetailThreshold = new MinMax01(0, 1);
        public Texture m_DetailTex;
        public Vector2 m_DetailTexScale = new Vector2(1, 1);
        public Vector2 m_DetailTexOffset = new Vector2(0, 0);
        public Vector2 m_DetailTexSpeed = new Vector2(0, 0);

        public Flip m_Flip = 0;

        public void UpdateContext(UIEffectContext dst)
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
            dst.m_TransitionGradient = src.m_TransitionGradient;

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
