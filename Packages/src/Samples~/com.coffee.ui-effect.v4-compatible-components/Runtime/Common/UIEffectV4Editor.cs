#if UNITY_EDITOR
using System.Linq;
using Coffee.UIEffectInternal;
using UnityEditor;

namespace Coffee.UIEffects
{
    internal static class UIEffectV4Editor
    {
        [MenuItem("CONTEXT/" + nameof(UIDissolve) + "/Convert To UIEffect")]
        [MenuItem("CONTEXT/" + nameof(UIShiny) + "/Convert To UIEffect")]
        [MenuItem("CONTEXT/" + nameof(UIHsvModifier) + "/Convert To UIEffect")]
        [MenuItem("CONTEXT/" + nameof(UITransitionEffect) + "/Convert To UIEffect")]
        [MenuItem("CONTEXT/" + nameof(UIEffectV4) + "/Convert To UIEffect")]
        private static void ConvertToUIEffect(MenuCommand command)
        {
            if (command.context is UIEffectBase legacy)
            {
                var go = legacy.gameObject;
                var legacyContext = legacy.context;
                command.context.ConvertTo<UIEffect>();

                var effect = go.GetComponent<UIEffect>();
                effect.toneFilter = legacyContext.m_ToneFilter;
                effect.toneIntensity = legacyContext.m_ToneIntensity;

                effect.colorFilter = legacyContext.m_ColorFilter;
                effect.color = legacyContext.m_Color;
                effect.colorIntensity = legacyContext.m_ColorIntensity;

                effect.samplingFilter = legacyContext.m_SamplingFilter;
                effect.samplingIntensity = legacyContext.m_SamplingIntensity;

                effect.transitionFilter = legacyContext.m_TransitionFilter;
                effect.transitionRate = legacyContext.m_TransitionRate;
                effect.transitionReverse = legacyContext.m_TransitionReverse;
                effect.transitionTexture = legacyContext.m_TransitionTex;
                effect.transitionRotation = legacyContext.m_TransitionRotation;
                effect.transitionKeepAspectRatio = legacyContext.m_TransitionKeepAspectRatio;
                effect.transitionWidth = legacyContext.m_TransitionWidth;
                effect.transitionSoftness = legacyContext.m_TransitionSoftness;
                effect.transitionColorFilter = legacyContext.m_TransitionColorFilter;
                effect.transitionColor = legacyContext.m_TransitionColor;

                Misc.SetDirty(effect);
            }
        }

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            var presets = new[]
                {
                    "460881a8263ce454799a8c689c28d8c9",
                    "b919bc84de9254bcf8f614d392db83bb"
                }
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<UIEffectPreset>);
            foreach (var preset in presets)
            {
                UIEffectProjectSettings.RegisterRuntimePreset(preset);
            }
        }
    }
}
#endif
