using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Linq;
using System;

namespace Coffee.UIExtensions.Editors
{
	/// <summary>
	/// UIEffect editor.
	/// </summary>
	[CustomEditor(typeof(UIEffect))]
	[CanEditMultipleObjects]
	public class UIEffectEditor : BaseMeshEffectEditor
	{
		static readonly GUIContent contentEffectColor = new GUIContent ("Effect Color");

		//################################
		// Public/Protected Members.
		//################################

		/// <summary>
		/// Draw effect properties.
		/// </summary>
		public static void DrawEffectProperties(SerializedObject serializedObject, string colorProperty = "m_Color")
		{
			//================
			// Effect setting.
			//================
			var spToneMode = serializedObject.FindProperty("m_EffectMode");
			EditorGUILayout.PropertyField(spToneMode);

			// When tone is enable, show parameters.
			if (spToneMode.intValue != (int)EffectMode.None)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_EffectFactor"));
				EditorGUI.indentLevel--;
			}

			//================
			// Color setting.
			//================
			var spColorMode = serializedObject.FindProperty("m_ColorMode");
			EditorGUILayout.PropertyField(spColorMode);

			// When color is enable, show parameters.
			//if (spColorMode.intValue != (int)ColorMode.Multiply)
			{
				EditorGUI.indentLevel++;

				SerializedProperty spColor = serializedObject.FindProperty(colorProperty);
				if (spColor == null && serializedObject.targetObject is UIEffect) {
					spColor = new SerializedObject (serializedObject.targetObjects.Select(x=>(x as UIEffect).targetGraphic).ToArray()).FindProperty(colorProperty);
				}

				EditorGUI.BeginChangeCheck ();
				EditorGUI.showMixedValue = spColor.hasMultipleDifferentValues;
#if UNITY_2018_1_OR_NEWER
				spColor.colorValue = EditorGUILayout.ColorField (contentEffectColor, spColor.colorValue, true, false, false);
#else
				spColor.colorValue = EditorGUILayout.ColorField (contentEffectColor, spColor.colorValue, true, false, false, null);
#endif
				if (EditorGUI.EndChangeCheck ()) {
					spColor.serializedObject.ApplyModifiedProperties ();
				}

				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ColorFactor"));
				EditorGUI.indentLevel--;
			}

			//================
			// Blur setting.
			//================
			var spBlurMode = serializedObject.FindProperty("m_BlurMode");
			EditorGUILayout.PropertyField(spBlurMode);

			// When blur is enable, show parameters.
			if (spBlurMode.intValue != (int)BlurMode.None)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_BlurFactor"));

				var spAdvancedBlur = serializedObject.FindProperty("m_AdvancedBlur");
				if (spAdvancedBlur != null)
				{
					EditorGUILayout.PropertyField(spAdvancedBlur);
				}
				EditorGUI.indentLevel--;
			}
		}

		/// <summary>
		/// Implement this function to make a custom inspector.
		/// </summary>
		public override void OnInspectorGUI()
		{
			//================
			// Effect setting.
			//================
			var spToneMode = serializedObject.FindProperty("m_EffectMode");
			using (new MaterialDirtyScope (targets))
				EditorGUILayout.PropertyField(spToneMode);

			// When tone is enable, show parameters.
			if (spToneMode.intValue != (int)EffectMode.None)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_EffectFactor"));
				EditorGUI.indentLevel--;
			}

			//================
			// Color setting.
			//================
			var spColorMode = serializedObject.FindProperty("m_ColorMode");
			using (new MaterialDirtyScope (targets))
				EditorGUILayout.PropertyField(spColorMode);

			// When color is enable, show parameters.
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField (serializedObject.FindProperty ("m_EffectColor"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ColorFactor"));
				EditorGUI.indentLevel--;
			}

			//================
			// Blur setting.
			//================
			var spBlurMode = serializedObject.FindProperty("m_BlurMode");
			using (new MaterialDirtyScope (targets))
				EditorGUILayout.PropertyField(spBlurMode);

			// When blur is enable, show parameters.
			if (spBlurMode.intValue != (int)BlurMode.None)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_BlurFactor"));

				var spAdvancedBlur = serializedObject.FindProperty("m_AdvancedBlur");
				using (new MaterialDirtyScope (targets))
					EditorGUILayout.PropertyField(spAdvancedBlur);
				EditorGUI.indentLevel--;
			}

			ShowCanvasChannelsWarning ();

			serializedObject.ApplyModifiedProperties();
		}
	}
}