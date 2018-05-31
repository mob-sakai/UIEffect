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

	public static class MaterialBundle
	{
		[MenuItem("UIEffect/Generate Material Bundle")]
		static void Generate()
		{
#if UIEFFECT_SEPARATE
			// On "UIEFFECT_SEPARATE" mode, generate effect materials on demand.
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
				
				EditorUtility.DisplayProgressBar("Genarate Effect Material Bundle", UIEffect.GetVariantName(shader, comb.tone, comb.color, comb.blur), (float)i / combinations.Length);
				UIEffect.GetOrGenerateMaterialVariant(shader, comb.tone, comb.color, comb.blur);
			}
			EditorUtility.ClearProgressBar();
		}
	}
}