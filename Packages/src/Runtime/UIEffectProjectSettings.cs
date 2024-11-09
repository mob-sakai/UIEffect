using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Coffee.UIEffectInternal;
using Coffee.UIEffects;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIEffects
{
    public class UIEffectProjectSettings : PreloadedProjectSettings<UIEffectProjectSettings>
    {
        public enum FallbackVariantBehaviour
        {
            RegisterShaderVariant,
            LogError
        }

        public enum TransformSensitivity
        {
            Low,
            Medium,
            High
        }

        [Tooltip(
            "The sensitivity of the transformation when `Use Target Transform` is enabled in the `UIEffectReplica` component.")]
        [Header("Setting")]
        [SerializeField]
        private TransformSensitivity m_TransformSensitivity = TransformSensitivity.Medium;

        [SerializeField]
        internal List<UIEffect> m_RuntimePresets = new List<UIEffect>();

        [SerializeField]
        [Header("Editor")]
        internal FallbackVariantBehaviour m_FallbackVariantBehaviour = FallbackVariantBehaviour.RegisterShaderVariant;

        [SerializeField]
        internal List<string> m_UnregisteredShaderVariants = new List<string>();

        [SerializeField]
        [Header("Shader")]
        internal ShaderVariantCollection m_ShaderVariantCollection;

        public static TransformSensitivity transformSensitivity
        {
            get => instance.m_TransformSensitivity;
            set => instance.m_TransformSensitivity = value;
        }

        public static ShaderVariantCollection shaderVariantCollection => instance.m_ShaderVariantCollection;

        public static float sensitivity
        {
            get
            {
                switch (instance.m_TransformSensitivity)
                {
                    case TransformSensitivity.Low: return 1f / (1 << 2);
                    case TransformSensitivity.Medium: return 1f / (1 << 5);
                    case TransformSensitivity.High: return 1f / (1 << 12);
                    default: return 1f / (1 << (int)instance.m_TransformSensitivity);
                }
            }
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

#if UNITY_EDITOR
        private const string k_PresetDir = "UIEffectPresets";
        private const string k_PresetSaveDir = "Assets/ProjectSettings/" + k_PresetDir;
        private const string k_PresetPathPattern = "/" + k_PresetDir + "/(.*).prefab$";

        protected override void OnCreateAsset()
        {
            m_ShaderVariantCollection = new ShaderVariantCollection()
            {
                name = "UIEffectShaderVariants"
            };
            AssetDatabase.AddObjectToAsset(m_ShaderVariantCollection, this);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
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

        internal static void ClearUnregisteredShaderVariants()
        {
            instance.m_UnregisteredShaderVariants.Clear();
        }

        internal static void RegisterVariant(string variant)
        {
            var values = variant.Split(';');
            instance.m_ShaderVariantCollection.Add(new ShaderVariantCollection.ShaderVariant()
            {
                shader = Shader.Find(values[0]),
                keywords = values[1].Split('|')
            });
            instance.m_UnregisteredShaderVariants.Remove(variant);
        }

        internal static void RegisterVariant(ShaderVariantCollection.ShaderVariant variant)
        {
            instance.m_ShaderVariantCollection.Add(variant);
        }

        internal static void RegisterVariant(Material material)
        {
            if (!material || !material.shader || !instance.m_ShaderVariantCollection) return;

            var variant = new ShaderVariantCollection.ShaderVariant
            {
                shader = material.shader,
                keywords = material.shaderKeywords
            };

            // Already registered.
            if (instance.m_ShaderVariantCollection.Contains(variant)) return;

            switch (instance.m_FallbackVariantBehaviour)
            {
                case FallbackVariantBehaviour.RegisterShaderVariant:
                    RegisterVariant(variant);
                    break;
                case FallbackVariantBehaviour.LogError:
                    var shaderName = variant.shader.name;
                    var keywords = string.Join("|", variant.keywords);
                    var v = $"{shaderName};{keywords}";
                    if (!instance.m_UnregisteredShaderVariants.Contains(v))
                    {
                        instance.m_UnregisteredShaderVariants.Add(v);
                    }

                    Debug.LogError($"{shaderName} with keywords <{keywords}> is not registered.\n" +
                                   "Please register it in 'Project Settings > UI > UIEffect > Shader'.");
                    return;
            }
        }
#endif
    }
}
