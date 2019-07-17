using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Coffee.UIExtensions
{
	public class MaterialEntity : System.IEquatable<ulong>, System.IEquatable<MaterialEntity>
	{
		public readonly ulong id;
		public readonly Material material;
		
		public MaterialEntity(ulong id, Material material) {
			this.id = id;
			this.material = material;
		}

		public void Clear()
		{
			if(material)
			{
#if UNITY_EDITOR
				if (!Application.isPlaying)
					Object.DestroyImmediate(material, false);
				else
#endif
				Object.Destroy(material);
			}
		}

        public bool Equals(ulong other)
        {
			return this.id == other;
        }

        public bool Equals(MaterialEntity other)
        {
			return this.id == other.id;
        }
    }
	public class MaterialRepository
	{

#if UNITY_EDITOR
		[UnityEditor.InitializeOnLoadMethod]
		static void ClearCache()
		{
			foreach (var cache in materialMap.Values)
			{
				cache.Clear();
			}
			materialMap.Clear();
		}
#endif
		static MaterialEntity none = new MaterialEntity(0,null);

		static Dictionary<ulong,MaterialEntity> materialMap = new Dictionary<ulong,MaterialEntity>();
		static Dictionary<ulong,int> referenceMap = new Dictionary<ulong,int>();

		public static MaterialEntity Register(string shaderId, ulong hash, System.Action<Material> onCreateMaterial)
		{
			MaterialEntity entity;
			if(!materialMap.TryGetValue(hash, out entity))
			{
				Shader shader = Shader.Find(shaderId);
				entity = new MaterialEntity(hash, new Material(shader));

				onCreateMaterial(entity.material);
				materialMap.Add(hash, entity);
				referenceMap.Add(hash, 0);
			}

			referenceMap[hash]++;
			return entity;
		}

		public static MaterialEntity Unregister(MaterialEntity cache)
		{
			if (!none.Equals(cache) )
			{
				ulong id = cache.id;
				int count = --referenceMap[id];
				if (count <= 0)
				{
					cache.Clear();
					referenceMap.Remove(id);
					materialMap.Remove(id);
				}
			}
			return none;
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