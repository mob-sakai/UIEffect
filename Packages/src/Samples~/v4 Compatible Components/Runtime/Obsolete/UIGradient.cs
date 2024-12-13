using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Graphic))]
    [AddComponentMenu("")]
    public class UIGradient : ObsoleteMonoBehaviour
    {
        public enum Direction
        {
            Horizontal,
            Vertical,
            Angle,
            Diagonal
        }

        public enum GradientStyle
        {
            Rect,
            Fit,
            Split
        }

        [Tooltip("Gradient Direction.")]
        [SerializeField]
        private Direction m_Direction;

        [Tooltip("Color1: Top or Left.")]
        [SerializeField]
        private Color m_Color1 = Color.white;

        [Tooltip("Color2: Bottom or Right.")]
        [SerializeField]
        private Color m_Color2 = Color.white;

        [Tooltip("Color3: For diagonal.")]
        [SerializeField]
        private Color m_Color3 = Color.white;

        [Tooltip("Color4: For diagonal.")]
        [SerializeField]
        private Color m_Color4 = Color.white;

        [Tooltip("Gradient rotation.")]
        [SerializeField]
        [Range(-180, 180)]
        private float m_Rotation;

        [Tooltip("Gradient offset for Horizontal, Vertical or Angle.")]
        [SerializeField]
        [Range(-1, 1)]
        private float m_Offset1;

        [Tooltip("Gradient offset for Diagonal.")]
        [SerializeField]
        [Range(-1, 1)]
        private float m_Offset2;

        [Tooltip("Gradient style for Text.")]
        [SerializeField]
        private GradientStyle m_GradientStyle;

        [Tooltip("Color space to correct color.")]
        [SerializeField]
        private ColorSpace m_ColorSpace = ColorSpace.Uninitialized;

        [Tooltip("Ignore aspect ratio.")]
        [SerializeField]
        private bool m_IgnoreAspectRatio = true;

        public Direction direction
        {
            get => m_Direction;
            set
            {
                if (m_Direction == value) return;
                m_Direction = value;
            }
        }

        public Color color1
        {
            get => m_Color1;
            set
            {
                if (m_Color1 == value) return;
                m_Color1 = value;
            }
        }

        public Color color2
        {
            get => m_Color2;
            set
            {
                if (m_Color2 == value) return;
                m_Color2 = value;
            }
        }

        public Color color3
        {
            get => m_Color3;
            set
            {
                if (m_Color3 == value) return;
                m_Color3 = value;
            }
        }

        public Color color4
        {
            get => m_Color4;
            set
            {
                if (m_Color4 == value) return;
                m_Color4 = value;
            }
        }

        public float rotation
        {
            get => m_Direction == Direction.Horizontal ? -90
                : m_Direction == Direction.Vertical ? 0
                : m_Rotation;
            set
            {
                if (Mathf.Approximately(m_Rotation, value)) return;
                m_Rotation = value;
            }
        }

        public float offset
        {
            get => m_Offset1;
            set
            {
                if (Mathf.Approximately(m_Offset1, value)) return;
                m_Offset1 = value;
            }
        }

        /// <summary>
        /// Gradient offset for Diagonal.
        /// </summary>
        public Vector2 offset2
        {
            get => new Vector2(m_Offset2, m_Offset1);
            set
            {
                if (Mathf.Approximately(m_Offset1, value.y) && Mathf.Approximately(m_Offset2, value.x)) return;
                m_Offset1 = value.y;
                m_Offset2 = value.x;
            }
        }

        /// <summary>
        /// Gradient style for Text.
        /// </summary>
        public GradientStyle gradientStyle
        {
            get => m_GradientStyle;
            set
            {
                if (m_GradientStyle == value) return;
                m_GradientStyle = value;
            }
        }

        /// <summary>
        /// Color space to correct color.
        /// </summary>
        public ColorSpace colorSpace
        {
            get => m_ColorSpace;
            set
            {
                if (m_ColorSpace == value) return;
                m_ColorSpace = value;
            }
        }

        /// <summary>
        /// Ignore aspect ratio.
        /// </summary>
        public bool ignoreAspectRatio
        {
            get => m_IgnoreAspectRatio;
            set
            {
                if (m_IgnoreAspectRatio == value) return;
                m_IgnoreAspectRatio = value;
            }
        }
    }
}
