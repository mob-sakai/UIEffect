#if TIMELINE_ENABLE
using System.ComponentModel;
using UnityEngine.Timeline;
using Coffee.UIEffects.Timeline;

namespace Coffee.UIEffects
{
    [FloatClipUsage(0f, 1f)]
    [TrackClipType(typeof(UIEffectFloatClip))]
    [DisplayName("UIEffect Tracks/Tone Intensity Track")]
    public class ToneIntensityTrack : UIEffectTrack<ToneIntensityMixer>
    {
        protected override string fieldName => "m_ToneIntensity";
    }

    public class ToneIntensityMixer : UIEffectFloatMixerBehaviour
    {
        protected override float currentValue
        {
            get => effect.toneIntensity;
            set => effect.toneIntensity = value;
        }
    }
}
#endif
