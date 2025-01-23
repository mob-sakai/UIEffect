using System;
using System.Linq;
using UnityEngine;
using Coffee.UIEffects;
using UnityEngine.Events;
using UnityEngine.UI;

public class ControlByScript : MonoBehaviour
{
    [SerializeField]
    private UIEffect m_Effect;

    [SerializeField]
    private ControlTemplate m_ControlTemplate;

    private void Start()
    {
        m_ControlTemplate.Instantiate()
            .InitAsDropDown(nameof(m_Effect.toneFilter), typeof(ToneFilter),
                (int)m_Effect.toneFilter, x => m_Effect.toneFilter = (ToneFilter)x);

        m_ControlTemplate.Instantiate()
            .InitAsSlider(nameof(m_Effect.toneIntensity), m_Effect.toneIntensity, x => m_Effect.toneIntensity = x);

        m_ControlTemplate.Instantiate()
            .InitAsDropDown(nameof(m_Effect.colorFilter), typeof(ColorFilter),
                (int)m_Effect.colorFilter, x => m_Effect.colorFilter = (ColorFilter)x);

        m_ControlTemplate.Instantiate()
            .InitAsSlider(nameof(m_Effect.colorIntensity), m_Effect.colorIntensity, x => m_Effect.colorIntensity = x);

        m_ControlTemplate.Instantiate()
            .InitAsColorDropDown(nameof(m_Effect.color), m_Effect.color, x => m_Effect.color = x);

        m_ControlTemplate.Instantiate()
            .InitAsSlider("Hue", m_Effect.color.r, x =>
            {
                var color = m_Effect.color;
                color.r = x;
                m_Effect.color = color;
            });

        m_ControlTemplate.Instantiate()
            .InitAsToggle(nameof(m_Effect.colorGlow), m_Effect.colorGlow, x => m_Effect.colorGlow = x);
    }


    private void AddColor(ColorFilter filter, Action<ColorFilter> filterSetter,
        float intensity, Action<float> intensitySetter,
        Color color, Action<Color> colorSetter,
        bool glow, Action<bool> glowSetter)
    {
        m_ControlTemplate.Instantiate()
            .InitAsDropDown(nameof(m_Effect.colorFilter), typeof(ColorFilter),
                (int)m_Effect.colorFilter, x => m_Effect.colorFilter = (ColorFilter)x);

        m_ControlTemplate.Instantiate()
            .InitAsSlider(nameof(m_Effect.colorIntensity), m_Effect.colorIntensity, x => m_Effect.colorIntensity = x);

        m_ControlTemplate.Instantiate()
            .InitAsColorDropDown(nameof(m_Effect.color), m_Effect.color, x => m_Effect.color = x);

        m_ControlTemplate.Instantiate()
            .InitAsSlider("Hue", m_Effect.color.r, x =>
            {
                var color = m_Effect.color;
                color.r = x;
                m_Effect.color = color;
            });

        m_ControlTemplate.Instantiate()
            .InitAsToggle(nameof(m_Effect.colorGlow), m_Effect.colorGlow, x => m_Effect.colorGlow = x);
    }
}
