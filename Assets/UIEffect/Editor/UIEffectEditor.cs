using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	/// <summary>
	/// UIEffect editor.
	/// </summary>
	[CustomEditor(typeof(UIEffect))]
	[CanEditMultipleObjects]
	public class UIEffectEditor : Editor
	{
		ReorderableList roAdditionalShadows;
		SerializedProperty spAdditionalShadows;
		SerializedProperty spBlurMode;

		/// <summary>
		/// Implement this function to make a custom inspector.
		/// </summary>
		public static void DrawEffectProperties(SerializedObject serializedObject)
		{
			//================
			// Tone setting.
			//================
			var spToneMode = serializedObject.FindProperty("m_ToneMode");
			EditorGUILayout.PropertyField(spToneMode);

			// When tone is enable, show parameters.
			if (spToneMode.intValue != (int)UIEffect.ToneMode.None)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ToneLevel"));
				EditorGUI.indentLevel--;
			}

			//================
			// Color setting.
			//================
			var spColorMode = serializedObject.FindProperty("m_ColorMode");
			EditorGUILayout.PropertyField(spColorMode);

			// When color is enable, show parameters.
			if (spColorMode.intValue != (int)UIEffect.ColorMode.None)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_EffectColor"));
				EditorGUI.indentLevel--;
			}

			//================
			// Blur setting.
			//================
			var spBlurMode = serializedObject.FindProperty("m_BlurMode");
			EditorGUILayout.PropertyField(spBlurMode);

			// When blur is enable, show parameters.
			if (spBlurMode.intValue != (int)UIEffect.BlurMode.None)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Blur"));
				EditorGUI.indentLevel--;
			}
		}

		void OnEnable()
		{
			spAdditionalShadows = serializedObject.FindProperty("m_AdditionalShadows");
			spBlurMode = serializedObject.FindProperty("m_BlurMode");

			roAdditionalShadows = new ReorderableList(serializedObject, spAdditionalShadows, true, true, true, true);
			roAdditionalShadows.drawElementCallback = DrawElementCallback;
			roAdditionalShadows.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Additional Shadows");
			roAdditionalShadows.onAddCallback = OnAddCallback;
			roAdditionalShadows.elementHeightCallback = ElementHeightCallback;
		}

		void OnAddCallback(ReorderableList ro)
		{
			spAdditionalShadows.InsertArrayElementAtIndex(ro.count);
			var element = spAdditionalShadows.GetArrayElementAtIndex(ro.count - 1);
			element.FindPropertyRelative("shadowMode").intValue = (int)UIEffect.ShadowMode.Shadow;
			element.FindPropertyRelative("shadowColor").colorValue = Color.black;
			element.FindPropertyRelative("effectDistance").vector2Value = new Vector2(1f, -1f);
			element.FindPropertyRelative("useGraphicAlpha").boolValue = true;
			element.FindPropertyRelative("shadowBlur").floatValue = 0.25f;
		}

		float ElementHeightCallback(int index)
		{
			var element = spAdditionalShadows.GetArrayElementAtIndex(index);
			if (element.FindPropertyRelative("shadowMode").intValue == (int)UIEffect.ShadowMode.None)
				return 16;

			return (spBlurMode.intValue == (int)UIEffect.BlurMode.None ? 66 : 84) + (EditorGUIUtility.wideMode ? 0 : 18);
		}

		/// <summary>
		/// 
		/// </summary>
		void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
		{
			var sp = roAdditionalShadows.serializedProperty.GetArrayElementAtIndex(index);

			Rect r = new Rect(rect);
			r.height = EditorGUIUtility.singleLineHeight;
			var spMode = sp.FindPropertyRelative("shadowMode");
			EditorGUI.PropertyField(r, spMode);
			if (spMode.intValue == (int)UIEffect.ShadowMode.None)
				return;

			r.y += r.height;
			EditorGUI.PropertyField(r, sp.FindPropertyRelative("shadowColor"));
			r.y += r.height;
			EditorGUI.PropertyField(r, sp.FindPropertyRelative("effectDistance"));
			r.y += EditorGUIUtility.wideMode ? r.height : r.height * 2;
			EditorGUI.PropertyField(r, sp.FindPropertyRelative("useGraphicAlpha"));

			if (spBlurMode.intValue != (int)UIEffect.BlurMode.None)
			{
				r.y += r.height;
				EditorGUI.PropertyField(r, sp.FindPropertyRelative("shadowBlur"));
			}
		}

		/// <summary>
		/// Implement this function to make a custom inspector.
		/// </summary>
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			DrawEffectProperties(serializedObject);

			//================
			// Shadow setting.
			//================
			var spShadowMode = serializedObject.FindProperty("m_ShadowMode");
			EditorGUILayout.PropertyField(spShadowMode);

			// When shadow is enable, show parameters.
			if (spShadowMode.intValue != (int)UIEffect.ShadowMode.None)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_EffectDistance"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ShadowColor"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_UseGraphicAlpha"));

				if (spBlurMode.intValue != (int)UIEffect.BlurMode.None)
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ShadowBlur"));
				}
				EditorGUI.indentLevel--;
			}

			//================
			// Additional shadow setting.
			//================
			roAdditionalShadows.DoLayoutList();

			serializedObject.ApplyModifiedProperties();
		}
	}
}