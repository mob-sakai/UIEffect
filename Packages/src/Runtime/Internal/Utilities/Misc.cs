using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR && UNITY_2021_2_OR_NEWER
using UnityEditor.SceneManagement;
#elif UNITY_EDITOR
using UnityEditor.Experimental.SceneManagement;
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
    }
}
