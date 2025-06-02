#if TIMELINE_ENABLE
using System;
using System.ComponentModel;
using UnityEngine;

namespace Coffee.UIEffects.Timeline
{
    [DisplayName("Clip")]
    public class UIEffectColorClip : UIEffectClip<UIEffectColorBehaviour>
    {
    }

    [Serializable]
    public class UIEffectColorBehaviour : UIEffectBehaviour, IGetValue<Color>
    {
        [ColorUsage(true, true)]
        public Color m_Value = Color.white;

        [ColorUsage(true, true)]
        public Color m_From = Color.white;

        public Color Get(float time)
        {
            return m_Tween
                ? Color.Lerp(m_From, m_Value, m_Curve.Evaluate(time))
                : m_Value;
        }
    }

    public abstract class UIEffectColorMixerBehaviour : UIEffectMixerBehaviour<Color, UIEffectColorBehaviour>
    {
        protected override Color Add(Color baseValue, Color value, float weight)
        {
            return baseValue + value * weight;
        }

        protected override Color Lerp(Color defaultValue, Color value, float weight)
        {
            return Color.Lerp(defaultValue, value, weight);
        }
    }
}

#endif
