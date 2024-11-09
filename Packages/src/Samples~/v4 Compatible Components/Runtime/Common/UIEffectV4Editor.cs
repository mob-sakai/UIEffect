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
                go.GetComponent<UIEffect>().CopyFrom(legacyContext);
            }
        }

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            var presets = new[]
                {
                    "3b09ad143990c4c8795c6e93760e132a",
                    "4f70ff0f1e25b4388b9bcb38d619f7bf"
                }
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<UIEffect>);
            foreach (var preset in presets)
            {
                UIEffectProjectSettings.RegisterRuntimePreset(preset);
            }
        }
    }
}
#endif
