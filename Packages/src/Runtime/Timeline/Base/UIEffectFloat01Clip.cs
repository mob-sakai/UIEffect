// #if TIMELINE_ENABLE
// using System;
// using System.ComponentModel;
// using UnityEngine;
//
// namespace Coffee.UIEffects.Timeline
// {
//     [DisplayName("Clip")]
//     public class UIEffectFloat01Clip : UIEffectClip<UIEffectFloatBehaviour>
//     {
//     }
//
//     // [Serializable]
//     // public class UIEffectFloat01Behaviour : UIEffectFloatBehaviour
//     // {
//     //     [Range(0, 1)]
//     //     public float m_Value = 1;
//     //
//     //     [Range(0, 1)]
//     //     public float m_From = 1;
//     //
//     //     protected override float from => m_From;
//     //     protected override float value => m_Value;
//     // }
//
//     // public class UIEffectFloatBehaviour : UIEffectBehaviour, IGetValue<float>
//     // {
//     //     protected virtual float value { get; }
//     //     protected virtual float from { get; }
//     //
//     //     public float Get(float time)
//     //     {
//     //         return m_Tween
//     //             ? Mathf.Lerp(from, value, m_Curve.Evaluate(time))
//     //             : value;
//     //     }
//     // }
//     //
//     // public abstract class UIEffectFloatMixerBehaviour : UIEffectMixerBehaviour<float, UIEffectFloatBehaviour>
//     // {
//     //     protected override float Add(float baseValue, float value, float weight)
//     //     {
//     //         return baseValue + value * weight;
//     //     }
//     //
//     //     protected override float Lerp(float defaultValue, float value, float weight)
//     //     {
//     //         return Mathf.Lerp(defaultValue, value, weight);
//     //     }
//     // }
// }
//
// #endif
