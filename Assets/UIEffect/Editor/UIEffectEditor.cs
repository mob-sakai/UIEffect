using UnityEngine.UI;
using UnityEditor;

namespace UnityEditor.UI
{
	/// <summary>
	/// UIEffect editor.
	/// </summary>
	[CustomEditor(typeof(UIEffect))]
	[CanEditMultipleObjects]
	public class UIEffectEditor : Editor
	{
		/// <summary>
		/// Implement this function to make a custom inspector.
		/// </summary>
		public override void OnInspectorGUI()
		{
			//
			serializedObject.Update();

			UIEffect current = target as UIEffect;

			var spToneMode = serializedObject.FindProperty("m_ToneMode");
			var spColorMode = serializedObject.FindProperty("m_ColorMode");
			var spBlurMode = serializedObject.FindProperty("m_BlurMode");
			var spShadowMode = serializedObject.FindProperty("m_ShadowMode");

			//================
			// Only the main effect can change effect modes.
			// They change shader keywords of the material.
			//================
			if (current.mainEffect == current)
			{
				// Tone setting.
				EditorGUILayout.PropertyField(spToneMode);

				// If tone  is enable, show parameters.
				if (spToneMode.intValue != (int)UIEffect.ToneMode.None)
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ToneLevel"));
					EditorGUI.indentLevel--;
				}

				// Color setting.
				EditorGUILayout.PropertyField(spColorMode);

				// If color is enable, show parameters.
				if (spColorMode.intValue != (int)UIEffect.ColorMode.None)
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Color"));
					EditorGUI.indentLevel--;
				}
			}
			// Set effect mode from main effect.
			else
			{
				spToneMode.intValue = (int)current.mainEffect.toneMode;
				spColorMode.intValue = (int)current.mainEffect.colorMode;
				spBlurMode.intValue = (int)current.mainEffect.blurMode;
			}

			//================
			// Shadow setting.
			//================
			EditorGUILayout.PropertyField(spShadowMode);

			// If shadow is enable, show parameters.
			if (spShadowMode.intValue != (int)UIEffect.ShadowMode.None)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_EffectDistance"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ShadowColor"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_UseGraphicAlpha"));
				EditorGUI.indentLevel--;
			}

			//================
			// Blur setting.
			//================
			EditorGUILayout.PropertyField(spBlurMode);

			// If blur is enable, show parameters.
			if (spBlurMode.intValue != (int)UIEffect.BlurMode.None)
			{
				EditorGUI.indentLevel++;

				// Blur parameter.
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Blur"));

				// Shadow blur parameter.
				if (spShadowMode.intValue != (int)UIEffect.ShadowMode.None)
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ShadowBlur"));
				}
				EditorGUI.indentLevel--;
			}

			// 
			serializedObject.ApplyModifiedProperties();
		}
	}
}