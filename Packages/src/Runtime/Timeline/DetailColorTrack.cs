#if TIMELINE_ENABLE
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;
using Coffee.UIEffects.Timeline;

namespace Coffee.UIEffects
{
    [ColorClipUsage(true)]
    [TrackClipType(typeof(UIEffectColorClip))]
    [DisplayName("UIEffect Tracks/Detail Color Track")]
    public class DetailColorTrack : UIEffectTrack<DetailColorMixer>
    {
        protected override string fieldName => "m_DetailColor";
    }

    public class DetailColorMixer : UIEffectColorMixerBehaviour
    {
        protected override Color currentValue
        {
            get => effect.detailColor;
            set => effect.detailColor = value;
        }
    }
}
#endif
