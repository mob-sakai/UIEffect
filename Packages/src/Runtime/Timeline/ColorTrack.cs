#if TIMELINE_ENABLE
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;
using Coffee.UIEffects.Timeline;

namespace Coffee.UIEffects
{
    [ColorClipUsage(false)]
    [TrackClipType(typeof(UIEffectColorClip))]
    [DisplayName("UIEffect Tracks/Color Track")]
    public class ColorTrack : UIEffectTrack<ColorMixer>
    {
        protected override string fieldName => "m_Color";
    }

    public class ColorMixer : UIEffectColorMixerBehaviour
    {
        protected override Color currentValue
        {
            get => effect.color;
            set => effect.color = value;
        }
    }
}
#endif
