#if TMP_PRESENT
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
		static GUIStyle s_PanelTitle;
		Material currentMaterial;

		public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
		{
			currentMaterial = materialEditor.target as Material;
			base.OnGUI(materialEditor, properties);
		}

		protected override void DoGUI()
		{
			if (currentMaterial.HasProperty("_FaceColor"))
			{
				base.DoGUI();
			}
			else
			{
				DrawSpritePanel();
			}

			var name = currentMaterial.shader.name;
			if (name.EndsWith("(UIEffect)"))
			{
				DrawEffectPanel();
			}
			else if (name.EndsWith("(UIDissolve)"))
			{
				DrawDissolvePanel();
			}
			else if (name.EndsWith("(UIShiny)"))
			{
				DrawShinyPanel();
			}
			else if (name.EndsWith("(UITransition)"))
			{
				DrawTransitionPanel();
			}
		}

		void DrawSpritePanel()
		{
			if (BeginTmpPanel("Sprite", true))
			{
				EditorGUI.indentLevel += 1;
				DoTexture2D("_MainTex", "Texture");
				DoColor("_Color", "Color");
				EditorGUI.indentLevel -= 1;
			}
			EndTmpPanel();
		}

		void DrawDissolvePanel()
		{
			if (BeginTmpPanel("Dissolve", true))
			{
				EditorGUI.indentLevel += 1;
				DoTexture2D("_NoiseTex", "Texture");
				DrawEnum<ColorMode>(currentMaterial);
				EditorGUI.indentLevel -= 1;
			}
			EndTmpPanel();
		}

		void DrawShinyPanel()
		{
		}

		void DrawTransitionPanel()
		{
			if (BeginTmpPanel("Transition", true))
			{
				EditorGUI.indentLevel += 1;
				DoTexture2D("_NoiseTex", "Texture");
				DrawEnum<UITransitionEffect.EffectMode>(currentMaterial);
				EditorGUI.indentLevel -= 1;
			}
			EndTmpPanel();
		}

		void DrawEffectPanel()
		{
			if (BeginTmpPanel("Effect", true))
			{
				EditorGUI.indentLevel += 1;
				DrawEnum<EffectMode>(currentMaterial);
				DrawEnum<ColorMode>(currentMaterial);
				DrawEnum<BlurMode>(currentMaterial);
				DrawToggleKeyword(currentMaterial, "Advanced Blur", "EX");
				EditorGUI.indentLevel -= 1;
			}
			EndTmpPanel();
		}

		static void DrawEnum<T>(Material mat)
		{
			Type type = typeof(T);
			string[] names = System.Enum.GetNames(type);
			int[] values = System.Enum.GetValues(type) as int[];

			int mode = 0;
			for (int i = 0; i < names.Length; i++)
			{
				if (mat.IsKeywordEnabled(names[i].ToUpper()))
					mode = values[i];
			}

			var newMode = EditorGUILayout.IntPopup(ObjectNames.NicifyVariableName(type.Name), mode, names, values);
			if (mode != newMode)
			{
				Array.IndexOf<int>(values, mode);
				mat.DisableKeyword(names[Array.IndexOf<int>(values, mode)].ToUpper());
				if (newMode != 0)
					mat.EnableKeyword(names[Array.IndexOf<int>(values, newMode)].ToUpper());
			}
		}

		static void DrawToggleKeyword(Material mat, string label, string keyword)
		{
			bool value = mat.IsKeywordEnabled(keyword);
			if (EditorGUILayout.Toggle(label, value) != value)
			{
				if (value)
					mat.DisableKeyword(keyword);
				else
					mat.EnableKeyword(keyword);
			}
		}

		static bool BeginTmpPanel(string panel, bool expanded)
		{
			if (s_PanelTitle == null)
			{
				s_PanelTitle = new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Bold };
			}

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			Rect position = EditorGUI.IndentedRect(GUILayoutUtility.GetRect(20f, 18f));
			position.x += 20;
			position.width += 6f;
			expanded = GUI.Toggle(position, expanded, panel, s_PanelTitle);
			EditorGUI.indentLevel++;
			EditorGUI.BeginDisabledGroup(false);
			return expanded;
		}

		static void EndTmpPanel()
		{
			EditorGUI.EndDisabledGroup();
			EditorGUI.indentLevel--;
			EditorGUILayout.EndVertical();
		}
	}
}
#endif