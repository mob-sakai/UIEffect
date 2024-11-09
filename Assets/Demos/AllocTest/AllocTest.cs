// using Coffee.UISoftMask;

using Coffee.UIEffects;
using UnityEngine;

public class AllocTest : MonoBehaviour
{
    [SerializeField] private GameObject m_Target;
    [SerializeField] private bool m_SwitchActivation;
    [SerializeField] private bool m_SetDirty;
    [SerializeField] private bool m_DoTransform;
    [SerializeField] private bool m_Translate;
    [SerializeField] private bool m_Rotate;
    [SerializeField] private bool m_Scale;

    private UIEffect[] _targets;

    public bool switchActivation
    {
        get => m_SwitchActivation;
        set => m_SwitchActivation = value;
    }

    public bool setDirty
    {
        get => m_SetDirty;
        set => m_SetDirty = value;
    }

    public bool doTransform
    {
        get => m_DoTransform;
        set => m_DoTransform = value;
    }

    private void Start()
    {
        _targets = m_Target.GetComponentsInChildren<UIEffect>();
    }

    private void Update()
    {
        if (m_SwitchActivation)
        {
            foreach (var r in _targets)
            {
                r.enabled = true;
            }

            m_Target.SetActive(!m_Target.activeSelf);
        }
        else if (m_SetDirty)
        {
            foreach (var r in _targets)
            {
                r.enabled = !r.enabled;
            }

            if (!m_Target.activeSelf)
            {
                m_Target.SetActive(true);
            }
        }

        if (m_DoTransform)
        {
            var v = (Mathf.PingPong(Time.timeSinceLevelLoad, 4) - 2) / 2 * Time.deltaTime;
            if (m_Translate)
            {
                m_Target.transform.Translate(v * 100f * Vector3.one);
            }

            if (m_Rotate)
            {
                m_Target.transform.Rotate(v * 100f * Vector3.one);
            }

            if (m_Scale)
            {
                m_Target.transform.localScale = v * 1f * Vector3.one + Vector3.one;
            }
        }
    }
}
