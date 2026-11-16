using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    [RequireComponent(typeof(Graphic))]
    [AddComponentMenu("")]
    public class UIShadow : ObsoleteMonoBehaviour
    {
        private const float k_KMaxEffectDistance = 600f;

        [Tooltip("How far is the blurring shadow from the graphic.")]
        [FormerlySerializedAs("m_Blur")]
        [SerializeField]
        [Range(0, 1)]
        private float m_BlurFactor = 1;

        [Tooltip("Shadow effect style.")]
        [SerializeField]
        private ShadowStyle m_Style = ShadowStyle.Shadow;

        [SerializeField]
        private Color m_EffectColor = new Color(0f, 0f, 0f, 0.5f);

        [SerializeField]
        private Vector2 m_EffectDistance = new Vector2(1f, -1f);

        [SerializeField]
        private bool m_UseGraphicAlpha = true;

        public Color effectColor
        {
            get => m_EffectColor;
            set
            {
                if (m_EffectColor == value) return;
                m_EffectColor = value;
            }
        }

        public Vector2 effectDistance
        {
            get => m_EffectDistance;
            set
            {
                if (value.x > k_KMaxEffectDistance)
                {
                    value.x = k_KMaxEffectDistance;
                }

                if (value.x < -k_KMaxEffectDistance)
                {
                    value.x = -k_KMaxEffectDistance;
                }

                if (value.y > k_KMaxEffectDistance)
                {
                    value.y = k_KMaxEffectDistance;
                }

                if (value.y < -k_KMaxEffectDistance)
                {
                    value.y = -k_KMaxEffectDistance;
                }

                if (m_EffectDistance == value) return;
                m_EffectDistance = value;
            }
        }

        public bool useGraphicAlpha
        {
            get => m_UseGraphicAlpha;
            set
            {
                if (m_UseGraphicAlpha == value) return;
                m_UseGraphicAlpha = value;
            }
        }

        public float blurFactor
        {
            get => m_BlurFactor;
            set
            {
                value = Mathf.Clamp(value, 0, 2);
                if (Mathf.Approximately(m_BlurFactor, value)) return;
                m_BlurFactor = value;
            }
        }

        public ShadowStyle style
        {
            get => m_Style;
            set
            {
                if (m_Style == value) return;
                m_Style = value;
            }
        }
    }
}
