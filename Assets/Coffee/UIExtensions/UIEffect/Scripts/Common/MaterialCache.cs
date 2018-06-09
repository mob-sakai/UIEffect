using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Coffee.UIExtensions
{
	public class MaterialCache
	{
		public ColorMode colorMode;
		public int referenceCount = 0;
		public Texture texture;
		public Material material;

		[UnityEditor.InitializeOnLoadMethod]
		static void ClearCache()
		{
			materialCache.Clear();
		}

		public static List<MaterialCache> materialCache = new List<MaterialCache>();

		public static MaterialCache Register(ColorMode color, Texture tex, System.Func<Material> onCreateMaterial)
		{
			var cache = materialCache.FirstOrDefault(x => x.IsMatch(color, tex));
			if (cache != null)
			{
				cache.referenceCount++;
			}
			if (cache == null)
			{
				cache = new MaterialCache()
				{
					colorMode = color,
					texture = tex,
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

		public bool IsMatch(ColorMode color, Texture tex)
		{
			return colorMode == color && texture == tex;
		}
	}
}