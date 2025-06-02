#if TIMELINE_ENABLE
using System.ComponentModel;
using UnityEngine.Timeline;
using Coffee.UIEffects.Timeline;

namespace Coffee.UIEffects
{
    [FloatClipUsage(0f, 1f)]
    [TrackClipType(typeof(UIEffectFloatClip))]
    [DisplayName("UIEffect Tracks/Transition Rate Track")]
    public class TransitionRateTrack : UIEffectTrack<TransitionRateMixer>
    {
        protected override string fieldName => "m_TransitionRate";
    }

    public class TransitionRateMixer : UIEffectFloatMixerBehaviour
    {
        protected override float currentValue
        {
            get => effect.transitionRate;
            set => effect.transitionRate = value;
        }
    }
}
#endif
