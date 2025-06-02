#if TIMELINE_ENABLE
using System;
using System.ComponentModel;
using UnityEngine;

namespace Coffee.UIEffects.Timeline
{
    [DisplayName("Clip")]
    public class UIEffectFloatClip : UIEffectClip<UIEffectFloatBehaviour>
    {
    }

    [Serializable]
    public class UIEffectFloatBehaviour : UIEffectBehaviour, IGetValue<float>
    {
        public float m_Value = 1;

        public float m_From = 1;

        public float Get(float time)
        {
            return m_Tween
                ? Mathf.Lerp(m_From, m_Value, m_Curve.Evaluate(time))
                : m_Value;
        }
    }

    public abstract class UIEffectFloatMixerBehaviour : UIEffectMixerBehaviour<float, UIEffectFloatBehaviour>
    {
        protected override float Add(float baseValue, float value, float weight)
        {
            return baseValue + value * weight;
        }

        protected override float Lerp(float defaultValue, float value, float weight)
        {
            return Mathf.Lerp(defaultValue, value, weight);
        }
    }
}

#endif
