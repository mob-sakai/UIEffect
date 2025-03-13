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

        [Header("Editor")]
        [Tooltip("Use HDR color pickers on color fields.")]
        [SerializeField] private bool m_UseHDRColorPicker = true;

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

        public static bool useHdrColorPicker
        {
            get => instance.m_UseHDRColorPicker;
            set => instance.m_UseHDRColorPicker = value;
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

        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
#if UNITY_2023_2_OR_NEWER
            const string tmpSupport = "TextMeshPro Support (Unity 6)";
            const string version = "v5.6.0 (Unity 6)";
#else
            const string tmpSupport = "TextMeshPro Support";
            const string version = "v5.6.0";
#endif
            ShaderSampleImporter.RegisterShaderSamples(new[]
            {
                // TextMeshPro Support/TextMeshPro Support (Unity 6)
                ("Hidden/TextMeshPro/Distance Field SSD (UIEffect)", tmpSupport, version),
                ("Hidden/TextMeshPro/Mobile/Distance Field SSD (UIEffect)", tmpSupport, version),
                ("Hidden/TextMeshPro/Distance Field Overlay (UIEffect)", tmpSupport, version),
                ("Hidden/TextMeshPro/Mobile/Distance Field Overlay (UIEffect)", tmpSupport, version),
                ("Hidden/TextMeshPro/Bitmap (UIEffect)", tmpSupport, version),
                ("Hidden/TextMeshPro/Mobile/Bitmap (UIEffect)", tmpSupport, version),
                ("Hidden/TextMeshPro/Distance Field (UIEffect)", tmpSupport, version),
                ("Hidden/TextMeshPro/Mobile/Distance Field (UIEffect)", tmpSupport, version)
            });
            ShaderSampleImporter.RegisterShaderAliases(new[]
            {
                ("Hidden/Hidden/TextMeshPro/Distance Field SSD (SoftMaskable) (UIEffect)",
                    "Hidden/TextMeshPro/Distance Field SSD (UIEffect)"),
                ("Hidden/Hidden/TextMeshPro/Mobile/Distance Field SSD (SoftMaskable) (UIEffect)",
                    "Hidden/TextMeshPro/Mobile/Distance Field SSD (UIEffect)"),
                ("Hidden/Hidden/TextMeshPro/Distance Field Overlay (SoftMaskable) (UIEffect)",
                    "Hidden/TextMeshPro/Distance Field Overlay (UIEffect)"),
                ("Hidden/Hidden/TextMeshPro/Mobile/Distance Field Overlay (SoftMaskable) (UIEffect)",
                    "Hidden/TextMeshPro/Mobile/Distance Field Overlay (UIEffect)"),
                ("Hidden/Hidden/TextMeshPro/Bitmap (SoftMaskable) (UIEffect)",
                    "Hidden/TextMeshPro/Bitmap (UIEffect)"),
                ("Hidden/Hidden/TextMeshPro/Mobile/Bitmap (SoftMaskable) (UIEffect)",
                    "Hidden/TextMeshPro/Mobile/Bitmap (UIEffect)"),
                ("Hidden/Hidden/TextMeshPro/Distance Field (SoftMaskable) (UIEffect)",
                    "Hidden/TextMeshPro/Distance Field (UIEffect)"),
                ("Hidden/Hidden/TextMeshPro/Mobile/Distance Field (SoftMaskable) (UIEffect)",
                    "Hidden/TextMeshPro/Mobile/Distance Field (UIEffect)")
            });
            ShaderSampleImporter.RegisterDeprecatedShaders(new[]
            {
                // Old shaders.
                ("367310b4ca95c4b00a2215568f1af735", "Hidden-TMP_Bitmap-Mobile-UIEffect-Unity6.shader"),
                ("490fe6c46146140cca8766a31c9dfc0c", "Hidden-TMP_Bitmap-UIEffect-Unity6.shader"),
                ("b875fa74fa43a4c48a44634d5f3e6d3d", "Hidden-TMP_SDF-Mobile-UIEffect-Unity6.shader"),
                ("c67f821adbb6348a3a21573a25b55de0", "Hidden-TMP_SDF-UIEffect-Unity6.shader")
            });
            EditorApplication.update += ShaderSampleImporter.Update;

            CgincPathSync.RegisterShaders("/(TMPro|TMPro_Properties).cginc", new[]
            {
                "Hidden/TextMeshPro/Distance Field (UIEffect)",
                "Hidden/TextMeshPro/Mobile/Distance Field (UIEffect)",
                "Hidden/TextMeshPro/Distance Field SSD (UIEffect)",
                "Hidden/TextMeshPro/Mobile/Distance Field SSD (UIEffect)",
                "Hidden/TextMeshPro/Distance Field Overlay (UIEffect)",
                "Hidden/TextMeshPro/Mobile/Distance Field Overlay (UIEffect)",
                "Hidden/TextMeshPro/Bitmap (UIEffect)",
                "Hidden/TextMeshPro/Mobile/Bitmap (UIEffect)",
                "Hidden/TextMeshPro/Distance Field (UIEffect)",
                "Hidden/TextMeshPro/Mobile/Distance Field (UIEffect)"
            });

            Selection.selectionChanged += () =>
            {
                if (Application.isPlaying || Misc.isBatchOrBuilding) return;

                var selection = Selection.gameObjects;
                foreach (var c in FindAll())
                {
                    if (!c || !c.isActiveAndEnabled) continue;
                    c.SetEnablePreviewIfSelected(selection);
                }
            };
        }

        private static IEnumerable<UIEffectBase> FindAll()
        {
            return Misc.FindObjectsOfType<UIEffectBase>()
                .Concat(Misc.GetAllComponentsInPrefabStage<UIEffectBase>());
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_ShaderVariantRegistry.onShaderRequested = ShaderSampleImporter.ImportShaderIfSelected;
            m_ShaderVariantRegistry.ClearCache();
        }

        protected override void OnCreateAsset()
        {
            m_ShaderVariantRegistry.InitializeIfNeeded(this);
            m_ShaderVariantRegistry.RegisterOptionalShaders(this);
        }

        protected override void OnInitialize()
        {
            m_ShaderVariantRegistry.InitializeIfNeeded(this);
        }

        private void Reset()
        {
            m_ShaderVariantRegistry.InitializeIfNeeded(this);
            m_ShaderVariantRegistry.RegisterOptionalShaders(this);
        }

        internal static UIEffect[] LoadEditorPresets()
        {
            var dirs = AssetDatabase.FindAssets(k_PresetDir + " t:folder")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(x => Path.GetFileName(x) == k_PresetDir)
                .OrderBy(x => x.StartsWith("Packages/com.coffee.ui-effect/"))
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

        internal static (string path, bool builtin) GetPresetPath(UIEffect preset)
        {
            var assetPath = AssetDatabase.GetAssetPath(preset);
            var builtin = assetPath.StartsWith("Packages/com.coffee.ui-effect/");
            var m = Regex.Match(assetPath, k_PresetPathPattern);
            return (m.Success ? m.Groups[1].Value : Path.GetFileNameWithoutExtension(assetPath), builtin);
        }

        private class Postprocessor : AssetPostprocessor
        {
            private static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] ___, string[] ____)
            {
                if (Misc.isBatchOrBuilding) return;

                // Register optional shaders when shaders are imported.
                if (imported.Concat(deleted).Any(path => path.EndsWith(".shader")))
                {
                    MaterialRepository.Clear();

                    // Refresh
                    if (hasInstance)
                    {
                        shaderRegistry.ClearCache();
                        shaderRegistry.RegisterOptionalShaders(instance);
                    }

                    // Refresh all UIEffect instances.
                    foreach (var c in FindAll())
                    {
                        c.ReleaseMaterial();
                    }
                }

                // Refresh all UIEffect instances.
                EditorApplication.delayCall += () =>
                {
                    foreach (var c in FindAll())
                    {
                        c.SetMaterialDirty();
                        c.SetVerticesDirty();
                    }
                };
            }
        }
#endif
    }
}
