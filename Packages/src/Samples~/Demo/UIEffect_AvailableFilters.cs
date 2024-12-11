using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    public class UIEffect_AvailableFilters : MonoBehaviour
    {
        [SerializeField]
        private Canvas m_Canvas;

        [SerializeField]
        private RectMask2D m_Mask;

        private UIEffectTweener[] _tweeners;
        private UIEffect[] _uiEffects;

        private void Start()
        {
            _tweeners = m_Canvas.GetComponentsInChildren<UIEffectTweener>(false);
            _uiEffects = m_Canvas.GetComponentsInChildren<UIEffect>(false);

            m_Mask.enabled = true;
        }

        public void SetEnableTweeners(bool flag)
        {
            foreach (var tweener in _tweeners)
            {
                tweener.enabled = flag;
            }
        }

        public void SetEnableUIEffects(bool flag)
        {
            foreach (var uiEffect in _uiEffects)
            {
                uiEffect.enabled = flag;
            }
        }
    }
}
