using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using BlurMode = UnityEngine.UI.UIEffect.BlurMode;
using ColorMode = UnityEngine.UI.UIEffect.ColorMode;
using ToneMode = UnityEngine.UI.UIEffect.ToneMode;
using UnityEngine.UI;

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

			// Export materials.
			AssetDatabase.StartAssetEditing();
			CreateMaterialVariant(
				Shader.Find(UIEffect.shaderName)
				, (ToneMode[])Enum.GetValues(typeof(ToneMode))
				, (ColorMode[])Enum.GetValues(typeof(ColorMode))
				, (BlurMode[])Enum.GetValues(typeof(BlurMode))
			);

			CreateMaterialVariant(
				Shader.Find(UIEffectCapturedImage.shaderName)
				, new []{ ToneMode.None, ToneMode.Grayscale, ToneMode.Sepia, ToneMode.Nega, ToneMode.Pixel, ToneMode.Hue, }
				, (ColorMode[])Enum.GetValues(typeof(ColorMode))
				, (BlurMode[])Enum.GetValues(typeof(BlurMode))
			);
			AssetDatabase.StopAssetEditing();
			AssetDatabase.Refresh();

			// Export package
			AssetDatabase.ExportPackage(kAssetPathes, kPackageName, ExportPackageOptions.Recurse | ExportPackageOptions.Default);
			UnityEngine.Debug.Log("Export successfully : " + kPackageName);

			System.IO.File.Copy("Assets/UIEffect/Readme.md", "Readme.md", true);
		}

		static void CreateMaterialVariant(Shader shader, ToneMode[] tones, ColorMode[] colors, BlurMode[] blurs)
		{
			var combinations = (from tone in tones
			                    from color in colors
			                    from blur in blurs
			                    select new { tone, color, blur }).ToArray();

			for (int i = 0; i < combinations.Length; i++)
			{
				var comb = combinations[i];

				if (comb.tone == 0 && comb.color == 0 && comb.blur == 0)
					continue;

				UIEffectEditor.GetOrCreateMaterial(shader, comb.tone, comb.color, comb.blur);
			}
		}
	}
}