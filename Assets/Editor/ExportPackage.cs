using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using BlurMode = UnityEngine.UI.UIEffect.BlurMode;
using ColorMode = UnityEngine.UI.UIEffect.ColorMode;
using ToneMode = UnityEngine.UI.UIEffect.ToneMode;
using UnityEngine.UI;
using System.IO;

namespace UnityEditor.UI
{
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
			return;
#endif

			// Export materials.
			AssetDatabase.StartAssetEditing();
			GenerateMaterialVariants(
				Shader.Find(UIEffect.shaderName)
				, (ToneMode[])Enum.GetValues(typeof(ToneMode))
				, (ColorMode[])Enum.GetValues(typeof(ColorMode))
				, (BlurMode[])Enum.GetValues(typeof(BlurMode))
			);

			GenerateMaterialVariants(
				Shader.Find(UIEffectCapturedImage.shaderName)
				, new[] { ToneMode.None, ToneMode.Grayscale, ToneMode.Sepia, ToneMode.Nega, ToneMode.Pixel, ToneMode.Hue, }
				, (ColorMode[])Enum.GetValues(typeof(ColorMode))
				, (BlurMode[])Enum.GetValues(typeof(BlurMode))
			);

			AssetDatabase.StopAssetEditing();
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		static void GenerateMaterialVariants(Shader shader, ToneMode[] tones, ColorMode[] colors, BlurMode[] blurs)
		{
			var combinations = (from tone in tones
								from color in colors
								from blur in blurs
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