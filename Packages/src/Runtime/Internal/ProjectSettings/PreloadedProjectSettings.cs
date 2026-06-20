using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif

namespace Coffee.UIEffectInternal
{
    public abstract class PreloadedProjectSettings : ScriptableObject
#if UNITY_EDITOR
    {
        [Tooltip("When enabled, this settings asset will be added to PlayerSettings.preloadedAssets in build.\n\n" +
                 "When disable, you should load this settings via Resources, AssetBundles or " +
                 "Addressables to use UIEffect.")]
        [SerializeField]
        private bool m_PreLoadSettingsInBuild = true;

        protected static bool s_BuildingPlayer;

        private class Postprocessor : AssetPostprocessor
        {
            private static void OnPostprocessAllAssets(string[] _, string[] __, string[] ___, string[] ____)
            {
                Initialize();
            }
        }

        private class ExcludeFromBuild : IPreprocessBuildWithReport, IPostprocessBuildWithReport
        {
            int IOrderedCallback.callbackOrder => 0;

            void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
            {
                Initialize();
                s_BuildingPlayer = true;

                foreach (var t in TypeCache.GetTypesDerivedFrom(typeof(PreloadedProjectSettings<>)))
                {
                    var settings = GetDefaultSettings(t);
                    if (!settings || settings.m_PreLoadSettingsInBuild) continue;

                    PlayerSettings.SetPreloadedAssets(
                        PlayerSettings.GetPreloadedAssets()
                            .Where(x => x && x.GetType() != t)
                            .ToArray());

                    Debug.Log($"[PreloadedProjectSettings] Build started: removed '{settings.name}' " +
                              $"({t.Name}) from PreloadedAssets. " +
                              $"It will be restored after build completes.");
                }
            }

            void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
            {
                s_BuildingPlayer = false;
                Initialize();
            }
        }

        private static void Initialize()
        {
            foreach (var t in TypeCache.GetTypesDerivedFrom(typeof(PreloadedProjectSettings<>)))
            {
                var defaultSettings = GetDefaultSettings(t);
                if (defaultSettings == null)
                {
                    // When create a new instance, automatically set it as default settings.
                    defaultSettings = CreateInstance(t) as PreloadedProjectSettings;
                    if (!s_BuildingPlayer) SetDefaultSettings(defaultSettings);
                }
                else if (GetPreloadedSettings(t).Length != 1)
                {
                    if (!s_BuildingPlayer) SetDefaultSettings(defaultSettings);
                }

                if (defaultSettings != null)
                {
                    defaultSettings.OnInitialize();
                }
            }
        }

        protected static string GetDefaultName(Type type, bool nicify)
        {
            var typeName = type.Name;
            return nicify
                ? ObjectNames.NicifyVariableName(typeName)
                : typeName;
        }

        private static Object[] GetPreloadedSettings(Type type)
        {
            return PlayerSettings.GetPreloadedAssets()
                .Where(x => x != null && x.GetType() == type)
                .ToArray();
        }

        protected static PreloadedProjectSettings GetDefaultSettings(Type type)
        {
            return GetPreloadedSettings(type).FirstOrDefault() as PreloadedProjectSettings
                   ?? AssetDatabase.FindAssets($"t:{nameof(PreloadedProjectSettings)}")
                       .Select(AssetDatabase.GUIDToAssetPath)
                       .Select(AssetDatabase.LoadAssetAtPath<PreloadedProjectSettings>)
                       .FirstOrDefault(x => x != null && x.GetType() == type);
        }

        protected static void SetDefaultSettings(PreloadedProjectSettings asset)
        {
            if (asset == null) return;

            var type = asset.GetType();
            if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(asset)))
            {
                if (!AssetDatabase.IsValidFolder("Assets/ProjectSettings"))
                {
                    AssetDatabase.CreateFolder("Assets", "ProjectSettings");
                }

                var assetPath = $"Assets/ProjectSettings/{GetDefaultName(type, false)}.asset";
                assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
                if (!File.Exists(assetPath))
                {
                    AssetDatabase.CreateAsset(asset, assetPath);
                    asset.OnCreateAsset();
                }
            }

            var preloadedAssets = PlayerSettings.GetPreloadedAssets();
            var projectSettings = GetPreloadedSettings(type);
            PlayerSettings.SetPreloadedAssets(preloadedAssets
                .Where(x => x != null)
                .Except(projectSettings.Except(new[] { asset }))
                .Append(asset)
                .Distinct()
                .ToArray());

            AssetDatabase.Refresh();
        }

        protected virtual void OnCreateAsset()
        {
        }

        protected virtual void OnInitialize()
        {
        }
    }
#else
    {
    }
#endif

    public abstract class PreloadedProjectSettings<T> : PreloadedProjectSettings
        where T : PreloadedProjectSettings<T>
    {
        private static T s_Instance;

#if UNITY_EDITOR
        private string _jsonText;

        public static bool hasInstance => s_Instance != null;

        public static T instance
        {
            get
            {
                if (s_Instance != null) return s_Instance;

                s_Instance = GetDefaultSettings(typeof(T)) as T;
                if (s_Instance != null) return s_Instance;

                s_Instance = CreateInstance<T>();
                if (s_Instance == null)
                {
                    s_Instance = null;
                    return s_Instance;
                }

                if (!s_BuildingPlayer) SetDefaultSettings(s_Instance);
                return s_Instance;
            }
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (!this) return;

            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    _jsonText = EditorJsonUtility.ToJson(this);
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    if (_jsonText != null)
                    {
                        EditorJsonUtility.FromJsonOverwrite(_jsonText, this);
                        _jsonText = null;
                    }

                    break;
            }
        }
#else
    public static T instance => s_Instance != null ? s_Instance : s_Instance = CreateInstance<T>();
#endif

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            var isDefaultSettings = s_Instance == null || s_Instance == this || GetDefaultSettings(typeof(T)) == this;
            if (!isDefaultSettings)
            {
                DestroyImmediate(this, true);
                return;
            }

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#else
            if (s_Instance && s_Instance != this)
            {
                Destroy(s_Instance);
            }
#endif
            s_Instance = this as T;
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled.
        /// </summary>
        protected virtual void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
            if (s_Instance != this) return;

            s_Instance = null;
        }

#if UNITY_EDITOR
        protected sealed class PreloadedProjectSettingsProvider : SettingsProvider
        {
            private Editor _editor;
            private PreloadedProjectSettings<T> _target;

            public PreloadedProjectSettingsProvider(string path) : base(path, SettingsScope.Project)
            {
            }

            public override void OnGUI(string searchContext)
            {
                if (_target == null)
                {
                    if (_editor)
                    {
                        DestroyImmediate(_editor);
                        _editor = null;
                    }

                    _target = instance;
                    _editor = Editor.CreateEditor(_target);
                }

                _editor.OnInspectorGUI();
            }
        }
#endif
    }
}
