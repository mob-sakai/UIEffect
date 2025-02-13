using UnityEngine;
using UnityEngine.Serialization;

namespace Coffee.UIEffects
{
    /// <summary>
    /// Dissolve effect for uGUI.
    /// </summary>
    // [AddComponentMenu("UI/UIEffects/UIDissolve", 3)]
    public class UIDissolve : UIEffectBase
    {
        [Tooltip("Current location[0-1] for dissolve effect. 0 is not dissolved, 1 is completely dissolved.")]
        [FormerlySerializedAs("m_Location")]
        [SerializeField]
        [Range(0, 1)]
        private float m_EffectFactor = 0.5f;

        [Tooltip("Edge width.")]
        [SerializeField]
        [Range(0, 1)]
        private float m_Width = 0.5f;

        [Tooltip("Edge softness.")]
        [SerializeField]
        [Range(0, 1)]
        private float m_Softness = 0.5f;

        [Tooltip("Edge color.")]
        [SerializeField]
        [ColorUsage(false)]
        private Color m_Color = new Color(0.0f, 0.25f, 1.0f);

        [Tooltip("Edge color effect mode.")]
        [SerializeField]
        private ColorMode m_ColorMode = ColorMode.Add;

        [SerializeField]
        [FormerlySerializedAs("m_NoiseTexture")]
        private Texture m_TransitionTexture;

        [SerializeField]
        private EffectArea m_EffectArea = EffectArea.RectTransform;

        [Tooltip("Keep effect aspect ratio.")]
        [SerializeField]
        private bool m_KeepAspectRatio;

        [Header("Effect Player")] [SerializeField]
        private EffectPlayer m_Player;

        [Tooltip("Reverse the dissolve effect.")]
        [FormerlySerializedAs("m_ReverseAnimation")]
        [SerializeField]
        private bool m_Reverse = false;

        /// <summary>
        /// Effect factor between 0(start) and 1(end).
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
        /// Edge width.
        /// </summary>
        public float width
        {
            get => m_Width;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_Width, value)) return;

                m_Width = value;
                UpdateContext(context);
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// Edge softness.
        /// </summary>
        public float softness
        {
            get => m_Softness;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_Softness, value)) return;

                m_Softness = value;
                UpdateContext(context);
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// Edge color.
        /// </summary>
        public Color color
        {
            get => m_Color;
            set
            {
                if (m_Color == value) return;

                m_Color = value;
                UpdateContext(context);
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// Noise texture.
        /// </summary>
        public Texture transitionTexture
        {
            get => m_TransitionTexture;
            set
            {
                if (m_TransitionTexture == value) return;

                m_TransitionTexture = value;
                UpdateContext(context);
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        public EffectArea effectArea
        {
            get => EffectArea.RectTransform;
            set => m_EffectArea = value;
        }

        /// <summary>
        /// Keep aspect ratio.
        /// </summary>
        public bool keepAspectRatio
        {
            get => m_KeepAspectRatio;
            set
            {
                if (m_KeepAspectRatio == value) return;

                m_KeepAspectRatio = value;
                UpdateContext(context);
                SetVerticesDirty();
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

        public EffectPlayer effectPlayer => m_Player ??= new EffectPlayer();

        /// <summary>
        /// Play effect.
        /// </summary>
        public void Play(bool reset = true)
        {
            effectPlayer.Attach(this);
            effectPlayer.Play(reset);
        }

        /// <summary>
        /// Stop effect.
        /// </summary>
        public void Stop(bool reset = true)
        {
            effectPlayer.Attach(this);
            effectPlayer.Stop(reset);
        }

        protected override void OnEnable()
        {
            LoadDefaultTransitionTextureIfNeeded();
            base.OnEnable();
            if (effectPlayer.play)
            {
                effectPlayer.Attach(this);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            effectPlayer.OnDisable();
        }

        protected override void UpdateContext(UIEffectContext c)
        {
            c.transitionFilter = TransitionFilter.Dissolve;
            c.transitionRate = m_EffectFactor;
            c.transitionTex = m_TransitionTexture;
            c.transitionKeepAspectRatio = m_KeepAspectRatio;
            c.transitionReverse = m_Reverse;
            c.transitionWidth = m_Width / 2;
            c.transitionSoftness = m_Softness / 3;
            c.transitionColor = m_Color;
            c.transitionColorFilter = m_ColorMode.Convert();
        }

        private void LoadDefaultTransitionTextureIfNeeded()
        {
            if (m_TransitionTexture) return;
            var preset = UIEffectProjectSettings.LoadRuntimePreset("Legacy-UIDissolve");
            if (!preset) return;

            m_TransitionTexture = preset.transitionTexture;
        }

        public override void SetRate(float rate, UIEffectTweener.CullingMask mask)
        {
            effectFactor = rate;
        }

        public override bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return effectFactor < 1;
        }
    }
}
