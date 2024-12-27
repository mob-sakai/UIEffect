using System;
using UnityEngine;
using UnityEngine.Profiling;

namespace Coffee.UIEffectInternal
{
    /// <summary>
    /// Provides functionality to manage materials.
    /// </summary>
    internal static class MaterialRepository
    {
        private static readonly ObjectRepository<Material> s_Repository = new ObjectRepository<Material>();

        public static int count => s_Repository.count;

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Clear()
        {
            s_Repository.Clear();
        }
#endif

        /// <summary>
        /// Retrieves a cached material based on the hash.
        /// </summary>
        public static bool Valid(Hash128 hash, Material material)
        {
            Profiler.BeginSample("(COF)[MaterialRegistry] Valid");
            var ret = s_Repository.Valid(hash, material);
            Profiler.EndSample();
            return ret;
        }

        /// <summary>
        /// Adds or retrieves a cached material based on the hash.
        /// </summary>
        public static void Get(Hash128 hash, ref Material material, Func<Material> onCreate)
        {
            Profiler.BeginSample("(COF)[MaterialRepository] Get");
            s_Repository.Get(hash, ref material, onCreate);
            Profiler.EndSample();
        }

        /// <summary>
        /// Adds or retrieves a cached material based on the hash.
        /// </summary>
        public static void Get(Hash128 hash, ref Material material, string shaderName)
        {
            Profiler.BeginSample("(COF)[MaterialRepository] Get");
            s_Repository.Get(hash, ref material, x => new Material(Shader.Find(x))
            {
                hideFlags = HideFlags.DontSave | HideFlags.NotEditable
            }, shaderName);
            Profiler.EndSample();
        }

        /// <summary>
        /// Adds or retrieves a cached material based on the hash.
        /// </summary>
        public static void Get(Hash128 hash, ref Material material, string shaderName, string[] keywords)
        {
            Profiler.BeginSample("(COF)[MaterialRepository] Get");
            s_Repository.Get(hash, ref material, x => new Material(Shader.Find(x.shaderName))
            {
                hideFlags = HideFlags.DontSave | HideFlags.NotEditable,
                shaderKeywords = x.keywords
            }, (shaderName, keywords));
            Profiler.EndSample();
        }

        /// <summary>
        /// Adds or retrieves a cached material based on the hash.
        /// </summary>
        public static void Get<T>(Hash128 hash, ref Material material, Func<T, Material> onCreate, T source)
        {
            Profiler.BeginSample("(COF)[MaterialRepository] Get");
            s_Repository.Get(hash, ref material, onCreate, source);
            Profiler.EndSample();
        }

        /// <summary>
        /// Removes a soft mask material from the cache.
        /// </summary>
        public static void Release(ref Material material)
        {
            Profiler.BeginSample("(COF)[MaterialRepository] Release");
            s_Repository.Release(ref material);
            Profiler.EndSample();
        }
    }
}
