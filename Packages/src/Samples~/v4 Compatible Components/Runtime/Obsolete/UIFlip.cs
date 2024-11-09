using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Graphic))]
    [AddComponentMenu("")]
    public class UIFlip : ObsoleteMonoBehaviour
    {
        [Tooltip("Flip horizontally.")]
        [SerializeField]
        private bool m_Horizontal = false;

        [Tooltip("Flip vertically.")]
        [SerializeField]
        private bool m_Veritical = false;

        public bool horizontal
        {
            get => m_Horizontal;
            set
            {
                if (m_Horizontal == value) return;
                m_Horizontal = value;
            }
        }

        public bool vertical
        {
            get => m_Veritical;
            set
            {
                if (m_Veritical == value) return;
                m_Veritical = value;
            }
        }
    }
}
