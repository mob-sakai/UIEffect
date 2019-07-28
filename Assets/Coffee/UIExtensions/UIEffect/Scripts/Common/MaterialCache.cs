using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using System.Text;

namespace Coffee.UIExtensions
{
    public class MaterialRepository
    {
        class MaterialEntry
        {
            public Material material;
            public int referenceCount;

            public void Release()
            {
                if (material)
                {
                    UnityEngine.Object.DestroyImmediate(material, false);
                }
                material = null;
            }
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void ClearCache()
        {
            foreach (var entry in materialMap.Values)
            {
                entry.Release();
            }
            materialMap.Clear();
        }
#endif

        static Dictionary<Hash128, MaterialEntry> materialMap = new Dictionary<Hash128, MaterialEntry>();

        public static Material Register(Material material, Hash128 hash, System.Action<Material> onModifyMaterial)
        {
			if(!hash.isValid)
				return null;

            MaterialEntry entry;
            if (!materialMap.TryGetValue(hash, out entry))
            {
                entry = new MaterialEntry()
                {
                    material = new Material(material)
                    {
                        // hideFlags = HideFlags.HideAndDontSave,
                    },
                };

                onModifyMaterial(entry.material);
                materialMap.Add(hash, entry);
                Debug.LogFormat($"Register {hash} {entry.material}");
            }

            entry.referenceCount++;
            return entry.material;
        }

        public static void Unregister(Hash128 hash)
        {
            MaterialEntry entry;
            if (hash.isValid && materialMap.TryGetValue(hash, out entry))
            {
                if (--entry.referenceCount <= 0)
                {
                    entry.Release();
                    materialMap.Remove(hash);
                    Debug.LogFormat($"Unregister {hash}");
                }
            }
        }
    }







    public class MaterialCache
    {
        public ulong hash { get; private set; }

        public int referenceCount { get; private set; }

        public Texture texture { get; private set; }

        public Material material { get; private set; }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void ClearCache()
        {
            foreach (var cache in materialCaches)
            {
                cache.material = null;
            }
            materialCaches.Clear();
        }
#endif

        public static List<MaterialCache> materialCaches = new List<MaterialCache>();

        public static MaterialCache Register(ulong hash, Texture texture, System.Func<Material> onCreateMaterial)
        {
            var cache = materialCaches.FirstOrDefault(x => x.hash == hash);
            if (cache != null && cache.material)
            {
                if (cache.material)
                {
                    cache.referenceCount++;
                }
                else
                {

                    materialCaches.Remove(cache);
                    cache = null;
                }
            }
            if (cache == null)
            {
                cache = new MaterialCache()
                {
                    hash = hash,
                    material = onCreateMaterial(),
                    referenceCount = 1,
                };
                materialCaches.Add(cache);
            }
            return cache;
        }

        public static MaterialCache Register(ulong hash, System.Func<Material> onCreateMaterial)
        {
            var cache = materialCaches.FirstOrDefault(x => x.hash == hash);
            if (cache != null)
            {
                cache.referenceCount++;
            }
            if (cache == null)
            {
                cache = new MaterialCache()
                {
                    hash = hash,
                    material = onCreateMaterial(),
                    referenceCount = 1,
                };
                materialCaches.Add(cache);
            }
            return cache;
        }

        public static void Unregister(MaterialCache cache)
        {
            if (cache == null)
            {
                return;
            }

            cache.referenceCount--;
            if (cache.referenceCount <= 0)
            {
                MaterialCache.materialCaches.Remove(cache);
                cache.material = null;
            }
        }
    }
}