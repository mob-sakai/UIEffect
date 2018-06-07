using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIExtensions
{
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

			GenerateMaterialVariants(
				Shader.Find(UIDissolve.shaderName)
				, new ToneMode[]{ToneMode.None}
				, (ColorMode[])Enum.GetValues(typeof(ColorMode))
				, new BlurMode[]{BlurMode.None}
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
				var name = MaterialResolver.GetVariantName(shader, comb.tone, comb.color, comb.blur);
				EditorUtility.DisplayProgressBar("Genarate Effect Material Bundle", name, (float)i / combinations.Length);

				MaterialResolver.GetOrGenerateMaterialVariant(shader, comb.tone, comb.color, comb.blur);
			}
			EditorUtility.ClearProgressBar();
		}
	}
}