using UnityEngine;


namespace Coffee.UIEffects
{
    /// <summary>
    /// HSV Modifier.
    /// </summary>
    // [AddComponentMenu("UI/UIEffects/UIHsvModifier", 4)]
    public class UIHsvModifier : UIEffectBase
    {
        [Header("Target")]
        [Tooltip("Target color to affect hsv shift.")]
        [SerializeField]
        [ColorUsage(false)]
        private Color m_TargetColor = Color.red;

        [Tooltip("Color range to affect hsv shift [0 ~ 1].")]
        [SerializeField]
        [Range(0, 1)]
        private float m_Range = 0.1f;

        [Header("Adjustment")]
        [Tooltip("Hue shift [-0.5 ~ 0.5].")]
        [SerializeField]
        [Range(-0.5f, 0.5f)]
        private float m_Hue;

        [Tooltip("Saturation shift [-0.5 ~ 0.5].")]
        [SerializeField]
        [Range(-0.5f, 0.5f)]
        private float m_Saturation;

        [Tooltip("Value shift [-0.5 ~ 0.5].")]
        [SerializeField]
        [Range(-0.5f, 0.5f)]
        private float m_Value;

        /// <summary>
        /// Target color to affect hsv shift.
        /// </summary>
        public Color targetColor
        {
            get => m_TargetColor;
            set
            {
                if (m_TargetColor == value) return;

                m_TargetColor = value;
                UpdateContext(context);
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// Color range to affect hsv shift [0 ~ 1].
        /// </summary>
        public float range
        {
            get => m_Range;
            set
            {
                value = Mathf.Clamp01(value);
                if (Mathf.Approximately(m_Range, value)) return;

                m_Range = value;
                UpdateContext(context);
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// Saturation shift [-0.5 ~ 0.5].
        /// </summary>
        public float saturation
        {
            get => m_Saturation;
            set
            {
                value = Mathf.Clamp(value, -0.5f, 0.5f);
                if (Mathf.Approximately(m_Saturation, value)) return;

                m_Saturation = value;
                UpdateContext(context);
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// Value shift [-0.5 ~ 0.5].
        /// </summary>
        public float value
        {
            get => m_Value;
            set
            {
                value = Mathf.Clamp(value, -0.5f, 0.5f);
                if (Mathf.Approximately(m_Value, value)) return;

                m_Value = value;
                UpdateContext(context);
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// Hue shift [-0.5 ~ 0.5].
        /// </summary>
        public float hue
        {
            get => m_Hue;
            set
            {
                value = Mathf.Clamp(value, -0.5f, 0.5f);
                if (Mathf.Approximately(m_Hue, value)) return;

                m_Hue = value;
                UpdateContext(context);
                SetMaterialDirty();
            }
        }

        public override void SetRate(float rate, UIEffectTweener.CullingMask mask)
        {
        }


        public override bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return true;
        }

        protected override void UpdateContext(UIEffectContext c)
        {
            c.colorFilter = ColorFilter.HsvModifier;
            c.colorIntensity = 1;
            c.color = new Color(m_Hue, m_Saturation, m_Value, 1);
            c.targetMode = TargetMode.Hue;
            c.targetColor = m_TargetColor;
            c.targetRange = m_Range;
        }
    }
}
