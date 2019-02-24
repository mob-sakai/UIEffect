#if COM_UNITY_TEXTMESHPRO
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro.EditorUtilities;
using UnityEditor;
using Coffee.UIExtensions;
using System.IO;
using System.Linq;
using System;

namespace Coffee.UIEffect.Editors
{
	public class TMP_SDFShaderGUI : TMPro.EditorUtilities.TMP_SDFShaderGUI
	{
		static MaterialPanel effectPanel = new MaterialPanel ("Effect", true);
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

			if (name.EndsWith ("(UIEffect)")) {
				DrawEffectPanel ();
			}
			else if (name.EndsWith ("(UIDissolve)")) {
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
				DrawEnum<ColorMode>(material);

//				ColorMode color =
//					material.IsKeywordEnabled ("ADD") ? ColorMode.Add
//							: material.IsKeywordEnabled ("SUBTRACT") ? ColorMode.Subtract
//							: material.IsKeywordEnabled ("FILL") ? ColorMode.Fill
//							: ColorMode.Multiply;
//
//				var newColor = (ColorMode)EditorGUILayout.EnumPopup ("Color Mode", color);
//				if (color != newColor)
//				{
//					material.DisableKeyword (color.ToString ().ToUpper ());
//					if (newColor != ColorMode.Multiply)
//					{
//						material.EnableKeyword (newColor.ToString ().ToUpper ());
//					}
//				}
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
				DrawEnum<UITransitionEffect.EffectMode>(material);
//
//
//				UITransitionEffect.EffectMode mode =
//					material.IsKeywordEnabled ("CUTOFF") ? UITransitionEffect.EffectMode.Cutoff
//					: material.IsKeywordEnabled ("FADE") ? UITransitionEffect.EffectMode.Fade
//					: material.IsKeywordEnabled ("DISSOLVE") ? UITransitionEffect.EffectMode.Dissolve
//					: (UITransitionEffect.EffectMode)0;
//
//				var newMode = (UITransitionEffect.EffectMode)EditorGUILayout.EnumPopup ("Effect Mode", mode);
//				if (mode != newMode)
//				{
//					material.DisableKeyword (mode.ToString ().ToUpper ());
//					material.EnableKeyword (newMode.ToString ().ToUpper ());
//				}
				EditorGUI.indentLevel -= 1;
			}
		}

		void DrawEffectPanel ()
		{
			if (DoPanelHeader (effectPanel))
			{
				EditorGUI.indentLevel += 1;
				DrawEnum<EffectMode>(material);
				DrawEnum<ColorMode>(material);
				DrawEnum<BlurMode>(material);
				DrawToggleKeyword(material,"Advanced Blur","EX");
				EditorGUI.indentLevel -= 1;
			}
		}

		void DrawEnum<T>(Material mat)
		{
			Type type = typeof(T);
			string[] names = System.Enum.GetNames (type);
			int[] values = System.Enum.GetValues (type) as int[];

			int mode = 0;
			for(int i=0;i<names.Length;i++)
			{
				if (mat.IsKeywordEnabled (names [i].ToUpper()))
					mode = values [i];
			}

			var newMode = EditorGUILayout.IntPopup (ObjectNames.NicifyVariableName(type.Name), mode, names, values);
			if (mode != newMode)
			{
				Array.IndexOf<int> (values, mode);
				material.DisableKeyword (names[Array.IndexOf<int> (values, mode)].ToUpper());
				if (newMode != 0)
					material.EnableKeyword (names[Array.IndexOf<int> (values, newMode)].ToUpper());
			}
		}

		void DrawToggleKeyword(Material mat, string label, string keyword)
		{
			bool value = mat.IsKeywordEnabled(keyword);
			if (EditorGUILayout.Toggle(label, value) != value)
			{
				if (value)
					material.DisableKeyword (keyword);
				else
					material.EnableKeyword (keyword);
			}
		}
	}
}
#endif