using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Coffee.UIEffectInternal;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIEffects
{
    public class UIEffectProjectSettings : PreloadedProjectSettings<UIEffectProjectSettings>
    {
        [Tooltip(
            "The sensitivity of the transformation when `Use Target Transform` is enabled in the `UIEffectReplica` component.")]
        [Header("Setting")]
        [SerializeField]
        private TransformSensitivity m_TransformSensitivity = TransformSensitivity.Medium;

        [SerializeField]
        internal List<UIEffect> m_RuntimePresets = new List<UIEffect>();

        [HideInInspector]
        [SerializeField]
        internal ShaderVariantCollection m_ShaderVariantCollection;

        [HideInInspector]
        [SerializeField]
        private ShaderVariantRegistry m_ShaderVariantRegistry = new ShaderVariantRegistry();

        public static ShaderVariantRegistry shaderRegistry => instance.m_ShaderVariantRegistry;

        public static ShaderVariantCollection shaderVariantCollection => shaderRegistry.shaderVariantCollection;

        public static TransformSensitivity transformSensitivity
        {
            get => instance.m_TransformSensitivity;
            set => instance.m_TransformSensitivity = value;
        }

        public static void RegisterRuntimePreset(UIEffect effect)
        {
            // Already registered.
            if (!effect || LoadRuntimePreset(effect.name)) return;

            instance.m_RuntimePresets.Add(effect);
#if UNITY_EDITOR
            EditorUtility.SetDirty(instance);
#endif
        }

        public static UIEffect LoadRuntimePreset(string presetName)
        {
            for (var i = 0; i < instance.m_RuntimePresets.Count; i++)
            {
                var preset = instance.m_RuntimePresets[i];
                if (preset && preset.name == presetName)
                {
                    return preset;
                }
            }

            return null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
#if UNITY_EDITOR
            SetupSamplesForShaderVariantRegistry();
            m_ShaderVariantRegistry.ClearCache();
#endif
        }

#if UNITY_EDITOR
        private const string k_PresetDir = "UIEffectPresets";
        private const string k_PresetSaveDir = "Assets/ProjectSettings/" + k_PresetDir;
        private const string k_PresetPathPattern = "/" + k_PresetDir + "/(.*).prefab$";

        protected override void OnCreateAsset()
        {
            m_ShaderVariantRegistry.InitializeIfNeeded(this, "(UIEffect)");
        }

        protected override void OnInitialize()
        {
            m_ShaderVariantRegistry.InitializeIfNeeded(this, "(UIEffect)");
        }

        private void Reset()
        {
            m_ShaderVariantRegistry.InitializeIfNeeded(this, "(UIEffect)");
        }

        private void SetupSamplesForShaderVariantRegistry()
        {
#if UNITY_2023_2_OR_NEWER
            const string tmpSupport = "TextMeshPro Support (Unity 6)";
#else
            const string tmpSupport = "TextMeshPro Support";
#endif
            m_ShaderVariantRegistry.RegisterSamples(new[]
            {
                ("Hidden/TextMeshPro/Bitmap (UIEffect)", tmpSupport),
                ("Hidden/TextMeshPro/Mobile/Bitmap (UIEffect)", tmpSupport),
                ("Hidden/TextMeshPro/Distance Field (UIEffect)", tmpSupport),
                ("Hidden/TextMeshPro/Mobile/Distance Field (UIEffect)", tmpSupport)
            });
        }

        private void Refresh()
        {
            m_ShaderVariantRegistry.ClearCache();
            MaterialRepository.Clear();
            foreach (var c in Misc.FindObjectsOfType<UIEffectBase>()
                         .Concat(Misc.GetAllComponentsInPrefabStage<UIEffectBase>()))
            {
                c.SetMaterialDirty();
            }

            EditorApplication.QueuePlayerLoopUpdate();
        }

        internal static UIEffect[] LoadEditorPresets()
        {
            var dirs = AssetDatabase.FindAssets(k_PresetDir + " t:folder")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(x => Path.GetFileName(x) == k_PresetDir)
                .ToArray();
            return AssetDatabase.FindAssets("t:prefab", dirs)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<UIEffect>)
                .Where(x => x)
                .ToArray();
        }

        internal static void SaveAsNewPreset(UIEffect effect)
        {
            if (!Directory.Exists(k_PresetSaveDir))
            {
                Directory.CreateDirectory(k_PresetSaveDir);
            }

            var prefabPath = EditorUtility.SaveFilePanel("Save Preset", k_PresetSaveDir, effect.name, "prefab");
            if (string.IsNullOrEmpty(prefabPath)) return;

            var prefab = new GameObject();
            prefab.SetActive(false);

            var preset = prefab.AddComponent<UIEffect>();
            preset.LoadPreset(effect);
            var prefabAsset = PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);

            EditorGUIUtility.PingObject(prefabAsset);
            DestroyImmediate(prefab);
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider()
        {
            return new PreloadedProjectSettingsProvider("Project/UI/UI Effect");
        }

        internal static string GetPresetPath(UIEffect preset)
        {
            var assetPath = AssetDatabase.GetAssetPath(preset);
            var m = Regex.Match(assetPath, k_PresetPathPattern);
            return m.Success ? m.Groups[1].Value : Path.GetFileNameWithoutExtension(assetPath);
        }

        private class Postprocessor : AssetPostprocessor
        {
            private static void OnPostprocessAllAssets(string[] _, string[] __, string[] ___, string[] ____)
            {
                if (Misc.isBatchOrBuilding) return;

                instance.Refresh();
            }
        }
#endif
    }
}
