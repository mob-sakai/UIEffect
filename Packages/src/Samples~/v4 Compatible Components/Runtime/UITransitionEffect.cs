using UnityEngine;
using UnityEngine.Serialization;

namespace Coffee.UIEffects
{
    public class UITransitionEffect : UIEffectBase
    {
        /// <summary>
        /// Effect mode.
        /// </summary>
        public enum EffectMode
        {
            None = 0,
            Fade = 1,
            Cutoff = 2,
            Dissolve = 3
        }

        [Tooltip("Effect mode.")]
        [SerializeField]
        private EffectMode m_EffectMode = EffectMode.Cutoff;

        [Tooltip("Effect factor between 0(hidden) and 1(shown).")]
        [SerializeField]
        private float m_EffectFactor = 0.5f;

        [SerializeField]
        [FormerlySerializedAs("m_NoiseTexture")]
        private Texture m_TransitionTexture;

        [SerializeField]
        [HideInInspector]
        private EffectArea m_EffectArea = EffectArea.RectTransform;

        [Header("Advanced Option")]
        [Tooltip("Keep effect aspect ratio.")]
        [SerializeField]
        private bool m_KeepAspectRatio;

        [Tooltip("Dissolve edge width.")]
        [SerializeField]
        private float m_DissolveWidth = 0.5f;

        [Tooltip("Dissolve edge softness.")]
        [SerializeField]
        private float m_DissolveSoftness = 0.5f;

        [Tooltip("Dissolve edge color.")]
        [SerializeField]
        private Color m_DissolveColor = new Color(0.0f, 0.25f, 1.0f);

        [Tooltip("Disable the graphic's raycast target on hidden.")]
        [SerializeField]
        private bool m_PassRayOnHidden;

        [Header("Effect Player")]
        [SerializeField]
        private EffectPlayer m_Player;


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
        /// Transition texture.
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
                SetMaterialDirty();
            }
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
        /// Dissolve edge width.
        /// </summary>
        public float dissolveWidth
        {
            get => m_DissolveWidth;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_DissolveWidth, value)) return;

                m_DissolveWidth = value;
                UpdateContext(context);
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// Dissolve edge softness.
        /// </summary>
        public float dissolveSoftness
        {
            get => m_DissolveSoftness;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_DissolveSoftness, value)) return;

                m_DissolveSoftness = value;
                UpdateContext(context);
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// Dissolve edge color.
        /// </summary>
        public Color dissolveColor
        {
            get => m_DissolveColor;
            set
            {
                if (m_DissolveColor == value) return;

                m_DissolveColor = value;
                UpdateContext(context);
                SetMaterialDirty();
            }
        }

        public EffectArea effectArea
        {
            get => EffectArea.RectTransform;
            set => m_EffectArea = value;
        }

        /// <summary>
        /// Disable graphic's raycast target on hidden.
        /// </summary>
        public bool passRayOnHidden
        {
            get => m_PassRayOnHidden;
            set => m_PassRayOnHidden = value;
        }

        public EffectPlayer effectPlayer => m_Player ??= new EffectPlayer();

        /// <summary>
        /// Show transition.
        /// </summary>
        public void Show(bool reset = true)
        {
            effectPlayer.loop = false;
            effectPlayer.Attach(this);
            effectPlayer.PlayReverse(reset);
        }

        /// <summary>
        /// Hide transition.
        /// </summary>
        public void Hide(bool reset = true)
        {
            effectPlayer.loop = false;
            effectPlayer.Attach(this);
            effectPlayer.Play(reset);
        }

        public override void SetRate(float rate, UIEffectTweener.CullingMask mask)
        {
            effectFactor = rate;
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

        public override bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return !passRayOnHidden || effectFactor < 1;
        }

        protected override void UpdateContext(UIEffectContext c)
        {
            c.transitionFilter = m_EffectMode.Convert();
            c.transitionRate = m_EffectFactor;
            c.transitionTex = m_TransitionTexture;
            c.transitionKeepAspectRatio = m_KeepAspectRatio;
            c.transitionWidth = m_DissolveWidth / 2;
            c.transitionSoftness = m_DissolveSoftness / 3;
            c.transitionColor = m_DissolveColor;
            c.transitionColorFilter = ColorFilter.MultiplyAdditive;
        }

        private void LoadDefaultTransitionTextureIfNeeded()
        {
            if (m_TransitionTexture) return;
            var preset = UIEffectProjectSettings.LoadRuntimePreset("Legacy-UIDissolve");
            if (!preset) return;

            m_TransitionTexture = preset.transitionTexture;
        }
    }
}
