#if UNITY_EDITOR
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIExtensions
{
	public class MaterialResolver
	{
		static readonly StringBuilder s_StringBuilder = new StringBuilder();

		static readonly Dictionary<string, Material> s_MaterialMap = new Dictionary<string, Material>();

		public static Material GetOrGenerateMaterialVariant(Shader shader, params object[] append)
		{
			if (!shader)
			{
				return null;
			}

			string[] keywords = append.Where(x => 0 < (int)x)
				.Select(x => x.ToString().ToUpper())
				.ToArray();
			Material mat = GetMaterial(shader, append);
			if (mat)
			{
				if (!mat.shaderKeywords.OrderBy(x => x).SequenceEqual(keywords.OrderBy(x => x)))
				{
					mat.shaderKeywords = keywords;
					EditorUtility.SetDirty(mat);
					if (!Application.isPlaying)
					{
						EditorApplication.delayCall += AssetDatabase.SaveAssets;
					}
				}
				return mat;
			}

			string variantName = GetVariantName(shader, append);
			if (s_MaterialMap.TryGetValue(variantName, out mat) && mat)
			{
				return mat;
			}

			Debug.Log("Generate material : " + variantName);
			mat = new Material(shader);
			mat.shaderKeywords = keywords;

			mat.name = variantName;
			mat.hideFlags |= HideFlags.NotEditable;
			s_MaterialMap[variantName] = mat;

			bool isMainAsset = append.Cast<int>().All(x => x == 0);
			EditorApplication.delayCall += () => SaveMaterial(mat, shader, isMainAsset);
			return mat;
		}

		static void SaveMaterial(Material mat, Shader shader, bool isMainAsset)
		{
			string materialPath = GetDefaultMaterialPath(shader);

#if UIEFFECT_SEPARATE
			string dir = Path.GetDirectoryName(materialPath);
			materialPath = Path.Combine(Path.Combine(dir, "Separated"), mat.name + ".mat");
			isMainAsset = true;
#endif
			if (isMainAsset)
			{
				Directory.CreateDirectory(Path.GetDirectoryName(materialPath));
				AssetDatabase.CreateAsset(mat, materialPath);
			}
			else
			{
				GetOrGenerateMaterialVariant(shader);
				mat.hideFlags |= HideFlags.HideInHierarchy;
				AssetDatabase.AddObjectToAsset(mat, materialPath);
			}
			AssetDatabase.SaveAssets();
		}

		public static Material GetMaterial(Shader shader, params object[] append)
		{
			string variantName = GetVariantName(shader, append);
			return AssetDatabase.FindAssets("t:Material " + Path.GetFileName(shader.name))
			.Select(x => AssetDatabase.GUIDToAssetPath(x))
			.SelectMany(x => AssetDatabase.LoadAllAssetsAtPath(x))
			.OfType<Material>()
			.FirstOrDefault(x => x.name == variantName);
		}

		public static string GetDefaultMaterialPath(Shader shader)
		{
			var name = Path.GetFileName(shader.name);
			return AssetDatabase.FindAssets("t:Material " + name)
			.Select(x => AssetDatabase.GUIDToAssetPath(x))
			.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == name)
			?? ("Assets/" + name + ".mat");
		}

		public static string GetVariantName(Shader shader, params object[] append)
		{
			s_StringBuilder.Length = 0;

#if UIEFFECT_SEPARATE
			s_StringBuilder.Append("[Separated] ");
#endif
			s_StringBuilder.Append(Path.GetFileName(shader.name));
			foreach (object mode in append.Where(x=>0<(int)x))
			{
				s_StringBuilder.Append("-");
				s_StringBuilder.Append(mode.ToString());
			}
			return s_StringBuilder.ToString();
		}
	}
}
#endif
