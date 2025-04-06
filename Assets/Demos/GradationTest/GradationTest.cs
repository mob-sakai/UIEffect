using Coffee.UIEffects;
using UnityEngine;

[ExecuteAlways]
public class GradationTest : MonoBehaviour
{
    [Range(-1, 1)]
    [SerializeField]
    protected float m_GradationOffset = 0;

    [Range(0.01f, 10)]
    [SerializeField]
    protected float m_GradationScale = 1;

    [SerializeField]
    [Range(0, 360)]
    private float m_GradationRotation = 0;

    private UIEffect[] _uiEffects;

    private void Awake()
    {
        _uiEffects = GetComponentsInChildren<UIEffect>(true);
    }

    private void OnValidate()
    {
        foreach (var uiEffect in _uiEffects)
        {
            uiEffect.gradationOffset = m_GradationOffset;
            uiEffect.gradationScale = m_GradationScale;
            uiEffect.gradationRotation = m_GradationRotation;
        }
    }
}
