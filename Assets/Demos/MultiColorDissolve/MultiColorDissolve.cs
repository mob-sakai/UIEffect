using Coffee.UIEffects;
using UnityEngine;

[ExecuteAlways]
public class MultiColorDissolve : MonoBehaviour
{
    [SerializeField] private UIEffect m_Base;

    [Header("Dissolve 1")]
    [SerializeField] private UIEffect m_Dissolve1;

    [ColorUsage(true, true)]
    [SerializeField]
    private Color m_DissolveColor1 = Color.red;

    [Range(0, 0.1f)]
    [SerializeField]
    private float m_Delay1 = 0.05f;

    [Range(0, 0.2f)]
    [SerializeField]
    private float m_Width1 = 0.1f;

    [Range(0, 1f)]
    [SerializeField]
    private float m_Softness1 = 0.5f;

    [Header("Dissolve 2")]
    [SerializeField]
    private UIEffect m_Dissolve2;

    [ColorUsage(true, true)]
    [SerializeField]
    private Color m_DissolveColor2 = Color.magenta;

    [Range(0, 0.1f)]
    [SerializeField]
    private float m_Delay2 = 0.1f;

    [Range(0, 0.2f)]
    [SerializeField]
    private float m_Width2 = 0.1f;

    [Range(0, 1f)]
    [SerializeField]
    private float m_Softness2 = 0.5f;

    [Space]
    [Range(0, 1)] [SerializeField] private float m_Rate;

    public void SetRate(float rate)
    {
        m_Rate = rate;
        if (m_Base)
        {
            m_Base.transitionRate = rate;
        }

        if (m_Dissolve1)
        {
            m_Dissolve1.transitionRate = rate - m_Delay1;
            m_Dissolve1.transitionColor = m_DissolveColor1;
            m_Dissolve1.transitionWidth = m_Width1;
            m_Dissolve1.transitionSoftness = m_Softness1;
        }

        if (m_Dissolve2)
        {
            m_Dissolve2.transitionRate = rate - m_Delay2 * 2;
            m_Dissolve2.transitionColor = m_DissolveColor2;
            m_Dissolve2.transitionWidth = m_Width2;
            m_Dissolve2.transitionSoftness = m_Softness2;
        }
    }

    private void OnValidate()
    {
        SetRate(m_Rate);
    }
}
