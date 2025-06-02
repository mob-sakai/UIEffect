#if TIMELINE_ENABLE
using System.ComponentModel;
using UnityEngine.Timeline;
using Coffee.UIEffects.Timeline;

namespace Coffee.UIEffects
{
    [TrackClipType(typeof(UIEffectFloatClip))]
    [DisplayName("UIEffect Tracks/Gradation Offset Track")]
    public class GradationOffsetTrack : UIEffectTrack<GradationOffsetMixer>
    {
        protected override string fieldName => "m_GradationOffset";
    }

    public class GradationOffsetMixer : UIEffectFloatMixerBehaviour
    {
        protected override float currentValue
        {
            get => effect.gradationOffset;
            set => effect.gradationOffset = value;
        }
    }
}
#endif
