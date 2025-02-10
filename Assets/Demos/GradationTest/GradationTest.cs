using Coffee.UIEffects;
using UnityEngine;

public class GradationTest : MonoBehaviour
{
    [SerializeField] private UIEffect m_Target;

    [SerializeField] private Gradient m_Gradient;

    public void SetGradation()
    {
        m_Target.SetGradientKeys(m_Gradient.colorKeys, m_Gradient.alphaKeys, m_Gradient.mode);
    }
}
