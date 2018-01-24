using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIExtensions
{
	using BlurMode = UIEffect.BlurMode;
	using ColorMode = UIEffect.ColorMode;
	using ToneMode = UIEffect.ToneMode;

	public static class ExportPackage
	{
		const string kPackageName = "UIEffect.unitypackage";
		static readonly string[] kAssetPathes =
		{
			"Assets/UIEffect",
		};

		[MenuItem("Export Package/" + kPackageName)]
		[InitializeOnLoadMethod]
		static void Export()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
				return;

			// Export package
			AssetDatabase.ExportPackage(kAssetPathes, kPackageName, ExportPackageOptions.Recurse | ExportPackageOptions.Default);
			UnityEngine.Debug.Log("Export successfully : " + kPackageName);

			// Update readme.
			System.IO.File.Copy("Assets/UIEffect/Readme.md", "Readme.md", true);
		}

		[MenuItem("Export Package/Generate Material Variants")]
		static void GenerateMaterialVariants()
		{
#if UIEFFECT_SEPARATE
			// On "UIEFFECT_SEPARATE" mode, generate effect materials on demand.
			return;
#endif

			// Export materials.
			AssetDatabase.StartAssetEditing();
			{
				// For UIEffect
				GenerateMaterialVariants(Shader.Find(UIEffect.shaderName));

				// For UIEffectCapturedImage
				GenerateMaterialVariants(Shader.Find(UIEffectCapturedImage.shaderName));
			}
			AssetDatabase.StopAssetEditing();
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		/// <summary>
		/// Generates the material variants.
		/// </summary>
		static void GenerateMaterialVariants(Shader shader)
		{
			var combinations = (from tone in (ToneMode[])Enum.GetValues(typeof(ToneMode))
								from color in (ColorMode[])Enum.GetValues(typeof(ColorMode))
								from blur in (BlurMode[])Enum.GetValues(typeof(BlurMode))
								select new { tone, color, blur }).ToArray();

			for (int i = 0; i < combinations.Length; i++)
			{
				var comb = combinations[i];

				EditorUtility.DisplayProgressBar("Genarate Effect Material", UIEffect.GetVariantName(shader, comb.tone, comb.color, comb.blur), (float)i / combinations.Length);
				UIEffect.GetOrCreateMaterialVariant(shader, comb.tone, comb.color, comb.blur);
			}
			EditorUtility.ClearProgressBar();
		}
	}
}