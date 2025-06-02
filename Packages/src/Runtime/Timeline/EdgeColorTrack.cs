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
    public class EdgeColorTrack : UIEffectTrack<EdgeColorMixer>
    {
        protected override string fieldName => "m_EdgeColor";
    }

    public class EdgeColorMixer : UIEffectColorMixerBehaviour
    {
        protected override Color currentValue
        {
            get => effect.edgeColor;
            set => effect.edgeColor = value;
        }
    }
}
#endif
