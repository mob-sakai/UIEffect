#if TIMELINE_ENABLE
using System.ComponentModel;
using UnityEngine.Timeline;
using Coffee.UIEffects.Timeline;

namespace Coffee.UIEffects
{
    [FloatClipUsage(0f, 1f)]
    [TrackClipType(typeof(UIEffectFloatClip))]
    [DisplayName("UIEffect Tracks/Sampling Intensity Track")]
    public class SamplingIntensityTrack : UIEffectTrack<SamplingIntensityMixer>
    {
        protected override string fieldName => "m_SamplingIntensity";
    }

    public class SamplingIntensityMixer : UIEffectFloatMixerBehaviour
    {
        protected override float currentValue
        {
            get => effect.samplingIntensity;
            set => effect.samplingIntensity = value;
        }
    }
}
#endif
