#if TIMELINE_ENABLE
using System.ComponentModel;
using UnityEngine.Timeline;
using Coffee.UIEffects.Timeline;

namespace Coffee.UIEffects
{
    [FloatClipUsage(0f, 1f)]
    [TrackClipType(typeof(UIEffectFloatClip))]
    [DisplayName("UIEffect Tracks/Edge Shiny Rate Track")]
    public class EdgeShinyRateTrack : UIEffectTrack<EdgeShinyRateMixer>
    {
        protected override string fieldName => "m_EdgeShinyRate";
    }

    public class EdgeShinyRateMixer : UIEffectFloatMixerBehaviour
    {
        protected override float currentValue
        {
            get => effect.edgeShinyRate;
            set => effect.edgeShinyRate = value;
        }
    }
}
#endif
