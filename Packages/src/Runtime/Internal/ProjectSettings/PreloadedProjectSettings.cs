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
        private class Postprocessor : AssetPostprocessor
        {
            private static void OnPostprocessAllAssets(string[] _, string[] __, string[] ___, string[] ____)
            {
                Initialize();
            }
        }

        private class PreprocessBuildWithReport : IPreprocessBuildWithReport
        {
            int IOrderedCallback.callbackOrder => 0;

            void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
            {
                Initialize();
            }
        }

        private static void Initialize()
        {
            foreach (var t in TypeCache.GetTypesDerivedFrom(typeof(PreloadedProjectSettings<>)))
            {
                var defaultSettings = GetDefaultSettings(t);
                if (!defaultSettings)
                {
                    // When create a new instance, automatically set it as default settings.
                    defaultSettings = CreateInstance(t) as PreloadedProjectSettings;
                    SetDefaultSettings(defaultSettings);
                }
                else if (GetPreloadedSettings(t).Length != 1)
                {
                    SetDefaultSettings(defaultSettings);
                }

                if (defaultSettings)
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
                .Where(x => x && x.GetType() == type)
                .ToArray();
        }

        protected static PreloadedProjectSettings GetDefaultSettings(Type type)
        {
            return GetPreloadedSettings(type).FirstOrDefault() as PreloadedProjectSettings
                   ?? AssetDatabase.FindAssets($"t:{nameof(PreloadedProjectSettings)}")
                       .Select(AssetDatabase.GUIDToAssetPath)
                       .Select(AssetDatabase.LoadAssetAtPath<PreloadedProjectSettings>)
                       .FirstOrDefault(x => x && x.GetType() == type);
        }

        protected static void SetDefaultSettings(PreloadedProjectSettings asset)
        {
            if (!asset) return;

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
                .Where(x => x)
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

        public static bool hasInstance => s_Instance;

        public static T instance
        {
            get
            {
                if (s_Instance) return s_Instance;

                s_Instance = GetDefaultSettings(typeof(T)) as T;
                if (s_Instance) return s_Instance;

                s_Instance = CreateInstance<T>();
                if (!s_Instance)
                {
                    s_Instance = null;
                    return s_Instance;
                }

                SetDefaultSettings(s_Instance);
                return s_Instance;
            }
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
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
        public static T instance => s_Instance ? s_Instance : s_Instance = CreateInstance<T>();
#endif

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            var isDefaultSettings = !s_Instance || s_Instance == this || GetDefaultSettings(typeof(T)) == this;
            if (!isDefaultSettings)
            {
                DestroyImmediate(this, true);
                return;
            }

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif

            if (s_Instance) return;
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
                if (!_target)
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
