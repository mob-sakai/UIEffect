using Coffee.UIEffects;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UIEffectTweener))]
public class UIEffect_TweenerControler : MonoBehaviour
{
    [SerializeField]
    private Text m_Info;

    [SerializeField]
    private Slider m_SeekBar;

    [SerializeField]
    private GameObject m_SeekBarBgInterval1;

    [SerializeField]
    private GameObject m_SeekBarBgDuration2;

    [SerializeField]
    private GameObject m_SeekBarBgInterval2;

    private UIEffectTweener _tweener;
    private UIEffectTweener tweener => _tweener != null ? _tweener : GetComponent<UIEffectTweener>();

    private void Update()
    {
        if (m_Info)
        {
            var tw = tweener;
            m_Info.text =
                $"{tw.time:F2}/{tw.totalTime:F2} ({tw.rate:F2}), {tw.direction}, isTweening={tw.isTweening}, isDelaying={tw.isDelaying}, isPaused={tw.isPaused}";
        }

        if (m_SeekBar)
        {
            m_SeekBar.value = tweener.time / tweener.totalTime;
        }

        if (m_SeekBarBgInterval1)
        {
            m_SeekBarBgInterval1.SetActive(UIEffectTweener.WrapMode.Loop <= tweener.wrapMode);
        }

        if (m_SeekBarBgDuration2)
        {
            m_SeekBarBgDuration2.SetActive(UIEffectTweener.WrapMode.PingPongOnce <= tweener.wrapMode);
        }

        if (m_SeekBarBgInterval2)
        {
            m_SeekBarBgInterval2.SetActive(UIEffectTweener.WrapMode.PingPongLoop <= tweener.wrapMode);
        }
    }

    public void SetWrapMode(int value)
    {
        tweener.wrapMode = (UIEffectTweener.WrapMode)value;
    }

    public void SetPlayForwardOnEnable(bool value)
    {
        tweener.playOnEnable = value
            ? UIEffectTweener.PlayOnEnable.Forward
            : UIEffectTweener.PlayOnEnable.None;
    }
}
