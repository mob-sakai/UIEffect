using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using System.IO;
using System.Linq;
using System.Reflection;
#if UNITY_2021_2_OR_NEWER
using UnityEditor.SceneManagement;
#else
using UnityEditor.Experimental.SceneManagement;
#endif
#endif

namespace Coffee.UIEffectInternal
{
    internal static class Misc
    {
        public static T[] FindObjectsOfType<T>() where T : Object
        {
#if UNITY_2023_1_OR_NEWER
            return Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
            return Object.FindObjectsOfType<T>();
#endif
        }

        public static void Destroy(Object obj)
        {
            if (!obj) return;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Object.DestroyImmediate(obj);
            }
            else
#endif
            {
                Object.Destroy(obj);
            }
        }

        public static void DestroyImmediate(Object obj)
        {
            if (!obj) return;
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                Object.DestroyImmediate(obj);
            }
            else
#endif
            {
                Object.Destroy(obj);
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void SetDirty(Object obj)
        {
#if UNITY_EDITOR
            if (!obj) return;
            EditorUtility.SetDirty(obj);
#endif
        }

#if UNITY_EDITOR
        public static T[] GetAllComponentsInPrefabStage<T>() where T : Component
        {
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage == null) return Array.Empty<T>();

            return prefabStage.prefabContentsRoot.GetComponentsInChildren<T>(true);
        }

        public static bool isBatchOrBuilding => Application.isBatchMode || BuildPipeline.isBuildingPlayer;
#endif

        [Conditional("UNITY_EDITOR")]
        public static void QueuePlayerLoopUpdate()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                EditorApplication.QueuePlayerLoopUpdate();
            }
#endif
        }
    }

#if !UNITY_2021_2_OR_NEWER
    [AttributeUsage(AttributeTargets.Class)]
    [Conditional("UNITY_EDITOR")]
    internal class IconAttribute : Attribute
    {
        private readonly string _path;

        public IconAttribute(string path)
        {
            _path = path;
        }

#if UNITY_EDITOR
        private static Action<Object, Texture2D> s_SetIconForObject = typeof(EditorGUIUtility)
            .GetMethod("SetIconForObject", BindingFlags.Static | BindingFlags.NonPublic)
            .CreateDelegate(typeof(Action<Object, Texture2D>), null) as Action<Object, Texture2D>;

        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            if (Misc.isBatchOrBuilding) return;

            var types = TypeCache.GetTypesWithAttribute<IconAttribute>();
            var scripts = MonoImporter.GetAllRuntimeMonoScripts();
            foreach (var type in types)
            {
                var script = scripts.FirstOrDefault(x => x.GetClass() == type);
                if (!script) continue;

                var path = type.GetCustomAttribute<IconAttribute>()?._path;
                var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (!icon) continue;

                s_SetIconForObject(script, icon);
            }
        }
#endif
    }
#endif
}
