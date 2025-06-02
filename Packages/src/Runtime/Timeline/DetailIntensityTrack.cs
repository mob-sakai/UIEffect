#if TIMELINE_ENABLE
using System.ComponentModel;
using UnityEngine.Timeline;
using Coffee.UIEffects.Timeline;

namespace Coffee.UIEffects
{
    [FloatClipUsage(0f, 1f)]
    [TrackClipType(typeof(UIEffectFloatClip))]
    [DisplayName("UIEffect Tracks/Detail Intensity Track")]
    public class DetailIntensityTrack : UIEffectTrack<DetailIntensityMixer>
    {
        protected override string fieldName => "m_DetailIntensity";
    }

    public class DetailIntensityMixer : UIEffectFloatMixerBehaviour
    {
        protected override float currentValue
        {
            get => effect.detailIntensity;
            set => effect.detailIntensity = value;
        }
    }
}
#endif
