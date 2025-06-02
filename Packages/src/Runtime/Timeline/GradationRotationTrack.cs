#if TIMELINE_ENABLE
using System.ComponentModel;
using UnityEngine.Timeline;
using Coffee.UIEffects.Timeline;

namespace Coffee.UIEffects
{
    [TrackClipType(typeof(UIEffectFloatClip))]
    [DisplayName("UIEffect Tracks/Gradation Rotation Track")]
    public class GradationRotationTrack : UIEffectTrack<GradationRotationMixer>
    {
        protected override string fieldName => "m_GradationRotation";
    }

    public class GradationRotationMixer : UIEffectFloatMixerBehaviour
    {
        protected override float currentValue
        {
            get => effect.gradationRotation;
            set => effect.gradationRotation = value;
        }
    }
}
#endif
