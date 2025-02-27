using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace Coffee.UIEffectInternal
{
    /// <summary>
    /// Extension methods for Component class.
    /// </summary>
    internal static class ComponentExtensions
    {
        /// <summary>
        /// Get components in children of a specific type in the hierarchy of a GameObject.
        /// </summary>
        public static T[] GetComponentsInChildren<T>(this Component self, int depth)
            where T : Component
        {
            var results = InternalListPool<T>.Rent();
            self.GetComponentsInChildren_Internal(results, depth);
            var array = results.ToArray();
            InternalListPool<T>.Return(ref results);
            return array;
        }

        /// <summary>
        /// Get components in children of a specific type in the hierarchy of a GameObject.
        /// </summary>
        public static void GetComponentsInChildren<T>(this Component self, List<T> results, int depth)
            where T : Component
        {
            results.Clear();
            self.GetComponentsInChildren_Internal(results, depth);
        }

        private static void GetComponentsInChildren_Internal<T>(this Component self, List<T> results, int depth)
            where T : Component
        {
            if (!self || results == null || depth < 0) return;

            var tr = self.transform;
            if (tr.TryGetComponent<T>(out var t))
            {
                results.Add(t);
            }

            if (depth - 1 < 0) return;
            var childCount = tr.childCount;
            for (var i = 0; i < childCount; i++)
            {
                tr.GetChild(i).GetComponentsInChildren_Internal(results, depth - 1);
            }
        }

        /// <summary>
        /// Get or add a component of a specific type to a GameObject.
        /// </summary>
        public static T GetOrAddComponent<T>(this Component self) where T : Component
        {
            if (!self) return null;
            return self.TryGetComponent<T>(out var component)
                ? component
                : self.gameObject.AddComponent<T>();
        }

        /// <summary>
        /// Get the root component of a specific type in the hierarchy of a GameObject.
        /// </summary>
        public static T GetRootComponent<T>(this Component self) where T : Component
        {
            T component = null;
            var transform = self.transform;
            while (transform)
            {
                if (transform.TryGetComponent<T>(out var c))
                {
                    component = c;
                }

                transform = transform.parent;
            }

            return component;
        }

        /// <summary>
        /// Get a component of a specific type in the parent hierarchy of a GameObject.
        /// </summary>
        public static T GetComponentInParent<T>(this Component self, bool includeSelf, Transform stopAfter,
            Predicate<T> valid)
            where T : Component
        {
            var tr = includeSelf ? self.transform : self.transform.parent;
            while (tr)
            {
                if (tr.TryGetComponent<T>(out var c) && valid(c)) return c;
                if (tr == stopAfter) return null;
                tr = tr.parent;
            }

            return null;
        }

        /// <summary>
        /// Add a component of a specific type to the children of a GameObject.
        /// </summary>
        public static void AddComponentOnChildren<T>(this Component self, HideFlags hideFlags, bool includeSelf)
            where T : Component
        {
            if (self == null) return;

            Profiler.BeginSample("(COF)[ComponentExt] AddComponentOnChildren > Self");
            if (includeSelf && !self.TryGetComponent<T>(out _))
            {
                var c = self.gameObject.AddComponent<T>();
                c.hideFlags = hideFlags;
            }

            Profiler.EndSample();

            Profiler.BeginSample("(COF)[ComponentExt] AddComponentOnChildren > Child");
            var childCount = self.transform.childCount;
            for (var i = 0; i < childCount; i++)
            {
                var child = self.transform.GetChild(i);
                if (child.TryGetComponent<T>(out _)) continue;

                var c = child.gameObject.AddComponent<T>();
                c.hideFlags = hideFlags;
            }

            Profiler.EndSample();
        }

        /// <summary>
        /// Add a component of a specific type to the children of a GameObject.
        /// </summary>
        public static void AddComponentOnChildren<T>(this Component self, bool includeSelf)
            where T : Component
        {
            if (self == null) return;

            Profiler.BeginSample("(COF)[ComponentExt] AddComponentOnChildren > Self");
            if (includeSelf && !self.TryGetComponent<T>(out _))
            {
                self.gameObject.AddComponent<T>();
            }

            Profiler.EndSample();

            Profiler.BeginSample("(COF)[ComponentExt] AddComponentOnChildren > Child");
            var childCount = self.transform.childCount;
            for (var i = 0; i < childCount; i++)
            {
                var child = self.transform.GetChild(i);
                if (child.TryGetComponent<T>(out _)) continue;

                child.gameObject.AddComponent<T>();
            }

            Profiler.EndSample();
        }

#if !UNITY_2021_2_OR_NEWER && !UNITY_2020_3_45 && !UNITY_2020_3_46 && !UNITY_2020_3_47 && !UNITY_2020_3_48
        public static T GetComponentInParent<T>(this Component self, bool includeInactive) where T : Component
        {
            if (!self) return null;
            if (!includeInactive) return self.GetComponentInParent<T>();

            var current = self.transform;
            while (current)
            {
                if (current.TryGetComponent<T>(out var c)) return c;
                current = current.parent;
            }

            return null;
        }
#endif

#if UNITY_EDITOR
        /// <summary>
        /// Verify whether it can be converted to the specified component.
        /// </summary>
        internal static bool CanConvertTo<T>(this Object context) where T : MonoBehaviour
        {
            return context && context.GetType() != typeof(T);
        }

        /// <summary>
        /// Convert to the specified component.
        /// </summary>
        internal static void ConvertTo<T>(this Object context) where T : MonoBehaviour
        {
            var target = context as MonoBehaviour;
            if (target == null) return;

            var so = new SerializedObject(target);
            so.Update();

            var oldEnable = target.enabled;
            target.enabled = false;

            // Find MonoScript of the specified component.
            foreach (var script in MonoImporter.GetAllRuntimeMonoScripts())
            {
                if (script.GetClass() != typeof(T))
                {
                    continue;
                }

                // Set 'm_Script' to convert.
                so.FindProperty("m_Script").objectReferenceValue = script;
                so.ApplyModifiedProperties();
                break;
            }

            if (so.targetObject is MonoBehaviour mb)
            {
                mb.enabled = oldEnable;
            }
        }
#endif
    }
}
