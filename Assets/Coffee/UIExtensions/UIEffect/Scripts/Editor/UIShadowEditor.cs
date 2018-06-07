using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Coffee.UIExtensions
{
	/// <summary>
	/// UIShadow editor.
	/// </summary>
	[CustomEditor(typeof(UIShadow))]
	[CanEditMultipleObjects]
	public class UIShadowEditor : Editor
	{
		ReorderableList roAdditionalShadows;
		SerializedProperty spAdditionalShadows;
		UIEffect uiEffect;

		void OnEnable()
		{
			uiEffect = (target as UIShadow).GetComponent<UIEffect>();
			spAdditionalShadows = serializedObject.FindProperty("m_AdditionalShadows");

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
			element.FindPropertyRelative("style").intValue = (int)ShadowStyle.Shadow;
			element.FindPropertyRelative("effectColor").colorValue = Color.black;
			element.FindPropertyRelative("effectDistance").vector2Value = new Vector2(1f, -1f);
			element.FindPropertyRelative("useGraphicAlpha").boolValue = true;
			element.FindPropertyRelative("blur").floatValue = 0.25f;
		}

		float ElementHeightCallback(int index)
		{
			var element = spAdditionalShadows.GetArrayElementAtIndex(index);
			if (element.FindPropertyRelative("style").intValue == (int)ShadowStyle.None)
				return 16;
			
			return (uiEffect && uiEffect.blurMode != BlurMode.None ? 84 : 64) + (EditorGUIUtility.wideMode ? 0 : 18);
		}

		/// <summary>
		/// 
		/// </summary>
		void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
		{
			var sp = roAdditionalShadows.serializedProperty.GetArrayElementAtIndex(index);

			Rect r = new Rect(rect);
			r.height = EditorGUIUtility.singleLineHeight;
			var spMode = sp.FindPropertyRelative("style");
			EditorGUI.PropertyField(r, spMode);
			if (spMode.intValue == (int)ShadowStyle.None)
				return;

			r.y += r.height;
			EditorGUI.PropertyField(r, sp.FindPropertyRelative("effectColor"));
			r.y += r.height;
			EditorGUI.PropertyField(r, sp.FindPropertyRelative("effectDistance"));
			r.y += EditorGUIUtility.wideMode ? r.height : r.height * 2;
			EditorGUI.PropertyField(r, sp.FindPropertyRelative("useGraphicAlpha"));

			if (uiEffect && uiEffect.blurMode != BlurMode.None)
			{
				r.y += r.height;
				EditorGUI.PropertyField(r, sp.FindPropertyRelative("blur"));
			}
		}

		/// <summary>
		/// Implement this function to make a custom inspector.
		/// </summary>
		public override void OnInspectorGUI()
		{

			serializedObject.Update();

			//================
			// Shadow setting.
			//================
			var spShadowMode = serializedObject.FindProperty("m_Style");
			EditorGUILayout.PropertyField(spShadowMode);

			// When shadow is enable, show parameters.
			if (spShadowMode.intValue != (int)ShadowStyle.None)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_EffectDistance"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_EffectColor"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_UseGraphicAlpha"));

				if (uiEffect && uiEffect.blurMode != BlurMode.None)
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Blur"));
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