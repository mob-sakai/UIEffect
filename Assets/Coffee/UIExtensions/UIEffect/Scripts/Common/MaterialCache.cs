using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using System.Text;

namespace Coffee.UIExtensions
{
	public class MaterialEntity
	{
		public static readonly MaterialEntity none = new MaterialEntity(0,null);
		static readonly StringBuilder s_StringBuilder = new StringBuilder();

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
					UnityEngine.Object.DestroyImmediate(material, false);
				else
#endif
				UnityEngine.Object.Destroy(material);
			}
		}

		public void SetVariant(params object[] variants)
		{
			// Set shader keywords as variants
			var keywords = variants.Where(x => 0 < (int)x)
				.Select(x => x.ToString().ToUpper())
				.ToArray();
			material.shaderKeywords = keywords;

			// Add variant name
			s_StringBuilder.Length = 0;
			foreach(var keyword in keywords)
			{
				s_StringBuilder.Append("-");
				s_StringBuilder.Append(keyword);
			}
			material.name += s_StringBuilder.ToString();
		}

		public void SetTexture(int propertyId, Texture texture)
		{
			if(texture)
				material.SetTexture(propertyId, texture);
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

		public static MaterialEntity Register(string shaderId, ulong hash, System.Action<MaterialEntity> onCreateMaterial)
		{
			if (none.id == hash)
				return none;

			MaterialEntity entity;
			if(!materialMap.TryGetValue(hash, out entity))
			{
				Shader shader = Shader.Find(shaderId);
				entity = new MaterialEntity(hash, new Material(shader));

				onCreateMaterial(entity);
				materialMap.Add(hash, entity);
				referenceMap.Add(hash, 0);
			}

			referenceMap[hash]++;
			return entity;
		}

		public static MaterialEntity Unregister(MaterialEntity cache)
		{
			if (none.id == cache.id)
				return none;
			
			ulong id = cache.id;
			int count = --referenceMap[id];
			if (count <= 0)
			{
				cache.Clear();
				referenceMap.Remove(id);
				materialMap.Remove(id);
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