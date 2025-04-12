using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Coffee.UIEffects
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class UIShiny : UIEffectBase
    {
        [FormerlySerializedAs("m_Location")]
        [Range(0, 1)]
        [SerializeField]
        private float m_EffectFactor = 0.5f;

        [SerializeField]
        [Range(0, 1)]
        private float m_Width = 0.125f;

        [SerializeField]
        [Range(0, 360)]
        private float m_Rotation = 135;

        [SerializeField]
        [Range(0, 1)]
        private float m_Softness = 1f;

        [FormerlySerializedAs("m_Alpha")]
        [SerializeField]
        [Range(0, 1)]
        private float m_Brightness = 0.5f;

        [FormerlySerializedAs("m_Highlight")]
        [SerializeField]
        [Range(0, 1)]
        private float m_Gloss = 1;

        [SerializeField]
        protected EffectArea m_EffectArea;

        [SerializeField]
        private EffectPlayer m_Player;

        [SerializeField]
        private Texture m_TransitionTexture;

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
        /// Width for shiny effect.
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
        /// Softness for shiny effect.
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
        /// Brightness for shiny effect.
        /// </summary>
        public float brightness
        {
            get => m_Brightness;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_Brightness, value)) return;

                m_Brightness = value;
                UpdateContext(context);
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// Gloss factor for shiny effect.
        /// </summary>
        public float gloss
        {
            get => m_Gloss;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_Gloss, value)) return;

                m_Gloss = value;
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
        /// Rotation for shiny effect.
        /// </summary>
        [Obsolete]
        public float rotation
        {
            get => m_Rotation;
            set
            {
                if (Mathf.Approximately(m_Rotation, value)) return;
                m_Rotation = value;
                UpdateContext(context);
                SetVerticesDirty();
            }
        }

        public EffectPlayer effectPlayer => m_Player ?? (m_Player = new EffectPlayer());

        /// <summary>
        /// Play effect.
        /// </summary>
        public void Play(bool reset = true)
        {
            effectPlayer.Play(reset);
        }

        /// <summary>
        /// Stop effect.
        /// </summary>
        public void Stop(bool reset = true)
        {
            effectPlayer.Stop(reset);
        }

        internal override void UpdateContext(UIEffectContext c)
        {
            c.m_TransitionFilter = TransitionFilter.Shiny;
            c.m_TransitionRate = m_EffectFactor;
            c.m_TransitionTex = m_TransitionTexture;
            c.m_TransitionWidth = m_Width * 2;
            c.m_TransitionRotation = m_Rotation;
            c.m_TransitionSoftness = m_Softness / 2;
            c.m_TransitionColorFilter = 0.5f < m_Gloss ? ColorFilter.MultiplyAdditive : ColorFilter.Additive;
            c.m_TransitionColor = new Color(1, 1, 1, m_Brightness);
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
            return true;
        }

        private void LoadDefaultTransitionTextureIfNeeded()
        {
            if (m_TransitionTexture) return;
            var preset = UIEffectProjectSettings.LoadPreset("Legacy-UIShiny");
            if (preset is UIEffect presetV1)
            {
                m_TransitionTexture = presetV1.transitionTexture;
            }
            else if (preset is UIEffectPreset presetV2)
            {
                m_TransitionTexture = presetV2.m_TransitionTex;
            }
        }
    }
}
