#if UNITY_EDITOR
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIExtensions
{
	public class MaterialResolver
	{
		static readonly StringBuilder s_StringBuilder = new StringBuilder ();

		public static Material GetOrGenerateMaterialVariant (Shader shader, params object[] append)
		{
			if (!shader) {
				return null;
			}

			string[] keywords = append.Where (x => 0 < (int)x)
				.Select (x => x.ToString ().ToUpper ())
				.ToArray ();
			Material mat = GetMaterial (shader, append);
			if (mat) {
				if(!mat.shaderKeywords.OrderBy(x=>x).SequenceEqual(keywords.OrderBy(x=>x)))
				{
					mat.shaderKeywords = keywords;
					EditorUtility.SetDirty (mat);
					if (!Application.isPlaying)
					{
						EditorApplication.delayCall += AssetDatabase.SaveAssets;
					}
				}
				return mat;
			}

			var variantName = GetVariantName (shader, append);
			Debug.Log ("Generate material : " + variantName);
			mat = new Material (shader);
			mat.shaderKeywords = keywords;

			mat.name = variantName;
			mat.hideFlags |= HideFlags.NotEditable;

#if UIEFFECT_SEPARATE
				string dir = Path.GetDirectoryName(GetDefaultMaterialPath (shader));
				string materialPath = Path.Combine(Path.Combine(dir, "Separated"), mat.name + ".mat");

				Directory.CreateDirectory (Path.GetDirectoryName (materialPath));
				AssetDatabase.CreateAsset (mat, materialPath);
				AssetDatabase.SaveAssets ();
#else
			if (append.Cast<int> ().All (x => x == 0)) {
				string materialPath = GetDefaultMaterialPath (shader);
				Directory.CreateDirectory (Path.GetDirectoryName (materialPath));
				AssetDatabase.CreateAsset (mat, materialPath);
				AssetDatabase.SaveAssets ();
			} else {
				GetOrGenerateMaterialVariant (shader);
				string materialPath = GetDefaultMaterialPath (shader);
				mat.hideFlags |= HideFlags.HideInHierarchy;
				AssetDatabase.AddObjectToAsset (mat, materialPath);
			}
#endif
			return mat;
		}

		public static Material GetMaterial (Shader shader, params object[] append)
		{
			string variantName = GetVariantName (shader, append);
			return AssetDatabase.FindAssets ("t:Material " + Path.GetFileName (shader.name))
			.Select (x => AssetDatabase.GUIDToAssetPath (x))
			.SelectMany (x => AssetDatabase.LoadAllAssetsAtPath (x))
			.OfType<Material> ()
			.FirstOrDefault (x => x.name == variantName);
		}

		public static string GetDefaultMaterialPath (Shader shader)
		{
			var name = Path.GetFileName (shader.name);
			return AssetDatabase.FindAssets ("t:Material " + name)
			.Select (x => AssetDatabase.GUIDToAssetPath (x))
			.FirstOrDefault (x => Path.GetFileNameWithoutExtension (x) == name)
			?? ("Assets/" + name + ".mat");
		}

		public static string GetVariantName (Shader shader, params object[] append)
		{
			s_StringBuilder.Length = 0;

#if UIEFFECT_SEPARATE
			s_StringBuilder.Append("[Separated] ");
#endif
			s_StringBuilder.Append (Path.GetFileName (shader.name));
			foreach (object mode in append.Where(x=>0<(int)x)) {
				s_StringBuilder.Append ("-");
				s_StringBuilder.Append (mode.ToString ());
			}
			return s_StringBuilder.ToString ();
		}
	}
}
#endif
