#if TIMELINE_ENABLE
using System.ComponentModel;
using UnityEngine.Timeline;
using Coffee.UIEffects.Timeline;

namespace Coffee.UIEffects
{
    [FloatClipUsage(0f, 1f)]
    [TrackClipType(typeof(UIEffectFloatClip))]
    [DisplayName("UIEffect Tracks/Color Intensity Track")]
    public class ColorIntensityTrack : UIEffectTrack<ColorIntensityMixer>
    {
        protected override string fieldName => "m_ColorIntensity";
    }

    public class ColorIntensityMixer : UIEffectFloatMixerBehaviour
    {
        protected override float currentValue
        {
            get => effect.colorIntensity;
            set => effect.colorIntensity = value;
        }
    }
}
#endif
