using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Coffee.UIExtensions.Editors
{
	/// <summary>
	/// UIShadow editor.
	/// </summary>
	[CustomEditor(typeof(UIShadow))]
	[CanEditMultipleObjects]
	public class UIShadowEditor : Editor
	{
		UIEffect uiEffect;

		void OnEnable()
		{
			uiEffect = (target as UIShadow).GetComponent<UIEffect>();

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
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_BlurFactor"));
				}
				EditorGUI.indentLevel--;
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}