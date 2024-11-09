using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coffee.UIEffectInternal
{
    internal static class FrameCache
    {
        private static readonly Dictionary<Type, IFrameCache> s_Caches = new Dictionary<Type, IFrameCache>();

        static FrameCache()
        {
            s_Caches.Clear();
            UIExtraCallbacks.onLateAfterCanvasRebuild += ClearAllCache;
        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Clear()
        {
            s_Caches.Clear();
        }
#endif

        /// <summary>
        /// Tries to retrieve a value from the frame cache with a specified key.
        /// </summary>
        public static bool TryGet<T>(object key1, string key2, out T result)
        {
            return GetFrameCache<T>().TryGet((key1.GetHashCode(), key2.GetHashCode()), out result);
        }

        /// <summary>
        /// Tries to retrieve a value from the frame cache with a specified key.
        /// </summary>
        public static bool TryGet<T>(object key1, string key2, int key3, out T result)
        {
            return GetFrameCache<T>().TryGet((key1.GetHashCode(), key2.GetHashCode() + key3), out result);
        }

        /// <summary>
        /// Sets a value in the frame cache with a specified key.
        /// </summary>
        public static void Set<T>(object key1, string key2, T result)
        {
            GetFrameCache<T>().Set((key1.GetHashCode(), key2.GetHashCode()), result);
        }

        /// <summary>
        /// Sets a value in the frame cache with a specified key.
        /// </summary>
        public static void Set<T>(object key1, string key2, int key3, T result)
        {
            GetFrameCache<T>().Set((key1.GetHashCode(), key2.GetHashCode() + key3), result);
        }

        private static void ClearAllCache()
        {
            foreach (var cache in s_Caches.Values)
            {
                cache.Clear();
            }
        }

        private static FrameCacheContainer<T> GetFrameCache<T>()
        {
            var t = typeof(T);
            if (s_Caches.TryGetValue(t, out var frameCache)) return frameCache as FrameCacheContainer<T>;

            frameCache = new FrameCacheContainer<T>();
            s_Caches.Add(t, frameCache);

            return (FrameCacheContainer<T>)frameCache;
        }

        private interface IFrameCache
        {
            void Clear();
        }

        private class FrameCacheContainer<T> : IFrameCache
        {
            private readonly Dictionary<(int, int), T> _caches = new Dictionary<(int, int), T>();

            public void Clear()
            {
                _caches.Clear();
            }

            public bool TryGet((int, int) key, out T result)
            {
                return _caches.TryGetValue(key, out result);
            }

            public void Set((int, int) key, T result)
            {
                _caches[key] = result;
            }
        }
    }
}
