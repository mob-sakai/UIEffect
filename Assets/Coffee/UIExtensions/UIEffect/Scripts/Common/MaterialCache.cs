using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Coffee.UIExtensions
{
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
			materialCache.Clear();
		}
#endif

		public static List<MaterialCache> materialCache = new List<MaterialCache>();

		public static MaterialCache Register(ulong hash, Texture texture, System.Func<Material> onCreateMaterial)
		{
			var cache = materialCache.FirstOrDefault(x => x.hash == hash);
			if (cache != null)
			{
				cache.referenceCount++;
			}
			if (cache == null)
			{
				cache = new MaterialCache()
				{
					hash = hash,
					texture = texture,
					material = onCreateMaterial(),
					referenceCount = 1,
				};
				materialCache.Add(cache);
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
				MaterialCache.materialCache.Remove(cache);
				cache.material = null;
				cache.texture = null;
			}
		}
	}
}