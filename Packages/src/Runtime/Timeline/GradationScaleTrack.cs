#if TIMELINE_ENABLE
using System.ComponentModel;
using UnityEngine.Timeline;
using Coffee.UIEffects.Timeline;

namespace Coffee.UIEffects
{
    [FloatClipUsage(0.01f, 10f)]
    [TrackClipType(typeof(UIEffectFloatClip))]
    [DisplayName("UIEffect Tracks/Gradation Scale Track")]
    public class GradationScaleTrack : UIEffectTrack<GradationScaleMixer>
    {
        protected override string fieldName => "m_GradationScale";
    }

    public class GradationScaleMixer : UIEffectFloatMixerBehaviour
    {
        protected override float currentValue
        {
            get => effect.gradationScale;
            set => effect.gradationScale = value;
        }
    }
}
#endif
