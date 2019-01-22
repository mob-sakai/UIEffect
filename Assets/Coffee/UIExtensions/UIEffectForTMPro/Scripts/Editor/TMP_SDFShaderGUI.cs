#if COM_UNITY_TEXTMESHPRO
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro.EditorUtilities;
using UnityEditor;
using Coffee.UIExtensions;
using System.IO;

namespace Coffee.UIEffect.Editors
{
	public class TMP_SDFShaderGUI : TMPro.EditorUtilities.TMP_SDFShaderGUI
	{
		static MaterialPanel dissolvePanel = new MaterialPanel ("Dissolve", true);
		static MaterialPanel transitionPanel = new MaterialPanel ("Transition", true);
		static MaterialPanel spritePanel = new MaterialPanel ("Sprite", true);

		protected override void DoGUI ()
		{
			if (material.HasProperty ("_FaceColor"))
			{
				base.DoGUI ();
			}
			else if (DoPanelHeader (spritePanel))
			{
				EditorGUI.indentLevel += 1;
				DoTexture2D ("_MainTex", "Texture");
				DoColor ("_Color", "Color");
				EditorGUI.indentLevel -= 1;
			}

			var name = material.shader.name;
			if (name.EndsWith ("(UIDissolve)")) {
				DrawDissolvePanel ();
			}
			else if (name.EndsWith ("(UIShiny)")) {
				DrawShinyPanel ();
			}
			else if (name.EndsWith ("(UITransition)")) {
				DrawTransitionPanel ();
			}
		}

		void DrawDissolvePanel ()
		{
			if (DoPanelHeader (dissolvePanel))
			{
				EditorGUI.indentLevel += 1;
				DoTexture2D ("_NoiseTex", "Texture");

				ColorMode color =
					material.IsKeywordEnabled ("ADD") ? ColorMode.Add
							: material.IsKeywordEnabled ("SUBTRACT") ? ColorMode.Subtract
							: material.IsKeywordEnabled ("FILL") ? ColorMode.Fill
							: ColorMode.Multiply;

				var newColor = (ColorMode)EditorGUILayout.EnumPopup ("Color Mode", color);
				if (color != newColor)
				{
					material.DisableKeyword (color.ToString ().ToUpper ());
					if (newColor != ColorMode.Multiply)
					{
						material.EnableKeyword (newColor.ToString ().ToUpper ());
					}
				}
				EditorGUI.indentLevel -= 1;
			}
		}

		void DrawShinyPanel ()
		{
		}

		void DrawTransitionPanel ()
		{
			if (DoPanelHeader (transitionPanel))
			{
				EditorGUI.indentLevel += 1;
				DoTexture2D ("_NoiseTex", "Texture");

				UITransitionEffect.EffectMode mode =
					material.IsKeywordEnabled ("CUTOFF") ? UITransitionEffect.EffectMode.Cutoff
					: material.IsKeywordEnabled ("FADE") ? UITransitionEffect.EffectMode.Fade
					: material.IsKeywordEnabled ("DISSOLVE") ? UITransitionEffect.EffectMode.Dissolve
					: (UITransitionEffect.EffectMode)0;

				var newMode = (UITransitionEffect.EffectMode)EditorGUILayout.EnumPopup ("Effect Mode", mode);
				if (mode != newMode)
				{
					material.DisableKeyword (mode.ToString ().ToUpper ());
					material.EnableKeyword (newMode.ToString ().ToUpper ());
				}
				EditorGUI.indentLevel -= 1;
			}
		}
	}
}
#endif