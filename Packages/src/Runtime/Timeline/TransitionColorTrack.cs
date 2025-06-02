#if TIMELINE_ENABLE
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;
using Coffee.UIEffects.Timeline;

namespace Coffee.UIEffects
{
    [ColorClipUsage(true)]
    [TrackClipType(typeof(UIEffectColorClip))]
    [DisplayName("UIEffect Tracks/Transition Color Track")]
    public class TransitionColorTrack : UIEffectTrack<TransitionColorMixer>
    {
        protected override string fieldName => "m_TransitionColor";
    }

    public class TransitionColorMixer : UIEffectColorMixerBehaviour
    {
        protected override Color currentValue
        {
            get => effect.transitionColor;
            set => effect.transitionColor = value;
        }
    }
}
#endif
