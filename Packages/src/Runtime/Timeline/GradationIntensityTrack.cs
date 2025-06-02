#if TIMELINE_ENABLE
using System.ComponentModel;
using UnityEngine.Timeline;
using Coffee.UIEffects.Timeline;

namespace Coffee.UIEffects
{
    [FloatClipUsage(0f, 1f)]
    [TrackClipType(typeof(UIEffectFloatClip))]
    [DisplayName("UIEffect Tracks/Gradation Intensity Track")]
    public class GradationIntensityTrack : UIEffectTrack<GradationIntensityMixer>
    {
        protected override string fieldName => "m_GradationIntensity";
    }

    public class GradationIntensityMixer : UIEffectFloatMixerBehaviour
    {
        protected override float currentValue
        {
            get => effect.gradationIntensity;
            set => effect.gradationIntensity = value;
        }
    }
}
#endif
