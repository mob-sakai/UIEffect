using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIExtensions.Editors
{
	/// <summary>
	/// UIEffect editor.
	/// </summary>
	[CustomEditor(typeof(UIGradient))]
	[CanEditMultipleObjects]
	public class UIGradientEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			//================
			// Direction.
			//================
			var spDirection = serializedObject.FindProperty("m_Direction");
			EditorGUILayout.PropertyField(spDirection);


			//================
			// Color.
			//================
			var spColor1 = serializedObject.FindProperty("m_Color1");
			var spColor2 = serializedObject.FindProperty("m_Color2");
			var spColor3 = serializedObject.FindProperty("m_Color3");
			var spColor4 = serializedObject.FindProperty("m_Color4");
			switch ((UIGradient.Direction)spDirection.intValue)
			{
				case UIGradient.Direction.Horizontal:
					EditorGUILayout.PropertyField(spColor1, new GUIContent("Left"));
					EditorGUILayout.PropertyField(spColor2, new GUIContent("Right"));
					break;
				case UIGradient.Direction.Vertical:
					EditorGUILayout.PropertyField(spColor1, new GUIContent("Top"));
					EditorGUILayout.PropertyField(spColor2, new GUIContent("Bottom"));
					break;
				case UIGradient.Direction.Angle:
					EditorGUILayout.PropertyField(spColor1, new GUIContent("Color 1"));
					EditorGUILayout.PropertyField(spColor2, new GUIContent("Color 2"));
					break;
				case UIGradient.Direction.Diagonal:
					Rect r = EditorGUILayout.GetControlRect(false, 34);

					r = EditorGUI.PrefixLabel(r, new GUIContent("Diagonal Color"));
					float w = r.width / 2;

					EditorGUI.PropertyField(new Rect(r.x, r.y, w, 16), spColor3, GUIContent.none);
					EditorGUI.PropertyField(new Rect(r.x + w, r.y, w, 16), spColor4, GUIContent.none);
					EditorGUI.PropertyField(new Rect(r.x, r.y + 18, w, 16), spColor1, GUIContent.none);
					EditorGUI.PropertyField(new Rect(r.x + w, r.y + 18, w, 16), spColor2, GUIContent.none);
					break;
			}


			//================
			// Angle.
			//================
			if ((int)UIGradient.Direction.Angle <= spDirection.intValue)
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Rotation"));
			}


			//================
			// Offset.
			//================
			if ((int)UIGradient.Direction.Diagonal == spDirection.intValue)
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Offset1"), new GUIContent("Vertical Offset"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Offset2"), new GUIContent("Horizontal Offset"));
			}
			else
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Offset1"), new GUIContent("Offset"));
			}


			//================
			// Advanced options.
			//================
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Advanced Options", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			{
				if ((target as UIGradient).targetGraphic is Text)
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_GradientStyle"));
				
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ColorSpace"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_IgnoreAspectRatio"));
			}
			EditorGUI.indentLevel--;

			serializedObject.ApplyModifiedProperties();

		}
	}
}