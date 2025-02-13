using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

namespace Coffee.UIEffects
{
    public class UIEffectV4 : UIEffectBase
    {
        [FormerlySerializedAs("m_ToneLevel")]
        [Tooltip("Effect factor between 0(no effect) and 1(complete effect).")]
        [SerializeField]
        [Range(0, 1)]
        private float m_EffectFactor = 1;

        [Tooltip("Color effect factor between 0(no effect) and 1(complete effect).")]
        [SerializeField]
        [Range(0, 1)]
        private float m_ColorFactor = 1;

        [FormerlySerializedAs("m_Blur")]
        [Tooltip("How far is the blurring from the graphic.")]
        [SerializeField]
        [Range(0, 1)]
        private float m_BlurFactor = 1;

        [FormerlySerializedAs("m_ToneMode")]
        [Tooltip("Effect mode")]
        [SerializeField]
        private EffectMode m_EffectMode = EffectMode.None;

        [Tooltip("Color effect mode")]
        [SerializeField]
        private ColorMode m_ColorMode = ColorMode.Multiply;

        [Tooltip("Blur effect mode")]
        [SerializeField]
        private BlurMode m_BlurMode = BlurMode.None;

        [Tooltip("Advanced blurring remove common artifacts in the blur effect for uGUI.")]
        [SerializeField]
        private bool m_AdvancedBlur = false;

        [Tooltip("Dissolve edge color.")]
        [SerializeField]
        private Color m_Color = new Color(0.0f, 0.25f, 1.0f);

        [SerializeField]
        private bool m_GetColorOnAwake = true;

        /// <summary>
        /// Effect factor between 0(no effect) and 1(complete effect).
        /// </summary>
        public float effectFactor
        {
            get => m_EffectFactor;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_EffectFactor, value)) return;

                m_EffectFactor = value;
                UpdateContext(context);
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// Color effect factor between 0(no effect) and 1(complete effect).
        /// </summary>
        public float colorFactor
        {
            get => m_ColorFactor;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_ColorFactor, value)) return;

                m_ColorFactor = value;
                UpdateContext(context);
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// How far is the blurring from the graphic.
        /// </summary>
        public float blurFactor
        {
            get => m_BlurFactor;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_BlurFactor, value)) return;

                m_BlurFactor = value;
                UpdateContext(context);
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// Effect mode.
        /// </summary>
        public EffectMode effectMode
        {
            get => m_EffectMode;
            set
            {
                if (m_EffectMode == value) return;

                m_EffectMode = value;
                UpdateContext(context);
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// Color effect mode.
        /// </summary>
        public ColorMode colorMode
        {
            get => m_ColorMode;
            set
            {
                if (m_ColorMode == value) return;

                m_ColorMode = value;
                UpdateContext(context);
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// Blur effect mode(readonly).
        /// </summary>
        public BlurMode blurMode
        {
            get => m_BlurMode;
            set
            {
                if (m_BlurMode == value) return;
                m_BlurMode = value;

                UpdateContext(context);
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// Advanced blurring remove common artifacts in the blur effect for uGUI.
        /// </summary>
        [Obsolete]
        public bool advancedBlur { get => m_AdvancedBlur; set => m_AdvancedBlur = value; }

        protected override void Awake()
        {
            if (m_GetColorOnAwake && TryGetComponent<Graphic>(out var g))
            {
                m_GetColorOnAwake = false;
                m_Color = g.color;
                g.color = Color.white;
            }

            base.Awake();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (m_GetColorOnAwake && TryGetComponent<Graphic>(out var g))
            {
                m_GetColorOnAwake = false;
                m_Color = g.color;
                g.color = Color.white;
            }

            base.OnValidate();
        }
#endif

        protected override void UpdateContext(UIEffectContext c)
        {
            (c.toneFilter, c.samplingFilter) = (m_EffectMode, m_BlurMode).Convert();
            if (context.samplingFilter == SamplingFilter.Pixelation)
            {
                c.toneIntensity = 0;
                c.samplingIntensity = m_EffectFactor;
            }
            else
            {
                c.toneIntensity = m_EffectFactor;
                c.samplingIntensity = m_BlurFactor;
            }

            c.colorFilter = m_ColorMode.Convert();
            c.colorIntensity = m_ColorFactor;
            c.color = m_Color;
        }

        public override void SetRate(float rate, UIEffectTweener.CullingMask mask)
        {
            if (effectMode != EffectMode.None && 0 < (mask & UIEffectTweener.CullingMask.Tone))
            {
                effectFactor = rate;
            }

            if (colorMode != ColorMode.Multiply && 0 < (mask & UIEffectTweener.CullingMask.Color))
            {
                colorFactor = rate;
            }

            if (blurMode != BlurMode.None && 0 < (mask & UIEffectTweener.CullingMask.Sampling))
            {
                blurFactor = rate;
            }
        }

        public override bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return true;
        }
    }
}
