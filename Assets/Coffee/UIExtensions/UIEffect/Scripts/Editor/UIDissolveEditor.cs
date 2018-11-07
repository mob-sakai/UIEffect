using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Linq;

namespace Coffee.UIExtensions.Editors
{
	/// <summary>
	/// UIEffect editor.
	/// </summary>
	[CustomEditor(typeof(UIDissolve))]
	[CanEditMultipleObjects]
	public class UIDissolveEditor : Editor
	{
		//################################
		// Public/Protected Members.
		//################################
		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected void OnEnable()
		{
			_spMaterial = serializedObject.FindProperty("m_EffectMaterial");
			_spEffectFactor = serializedObject.FindProperty("m_EffectFactor");
			_spEffectArea = serializedObject.FindProperty("m_EffectArea");
			_spKeepAspectRatio = serializedObject.FindProperty("m_KeepAspectRatio");
			_spWidth = serializedObject.FindProperty("m_Width");
			_spColor = serializedObject.FindProperty("m_Color");
			_spSoftness = serializedObject.FindProperty("m_Softness");
			_spColorMode = serializedObject.FindProperty("m_ColorMode");
			_spNoiseTexture = serializedObject.FindProperty("m_NoiseTexture");
			_spKeepAspectRatio = serializedObject.FindProperty("m_KeepAspectRatio");
			var player = serializedObject.FindProperty("m_Player");
			_spDuration = player.FindPropertyRelative("duration");
			_spUpdateMode = player.FindPropertyRelative("updateMode");
		}


		/// <summary>
		/// Implement this function to make a custom inspector.
		/// </summary>
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			//================
			// Effect material.
			//================
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.PropertyField(_spMaterial);
			EditorGUI.EndDisabledGroup();

			//================
			// Effect setting.
			//================
			EditorGUILayout.PropertyField(_spEffectFactor);
			EditorGUILayout.PropertyField(_spWidth);
			EditorGUILayout.PropertyField(_spSoftness);
			EditorGUILayout.PropertyField(_spColor);
			EditorGUILayout.PropertyField(_spColorMode);
			EditorGUILayout.PropertyField(_spNoiseTexture);

			//================
			// Advanced option.
			//================
			GUILayout.Space(10);
			EditorGUILayout.LabelField("Advanced Option", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(_spEffectArea);
			EditorGUILayout.PropertyField(_spKeepAspectRatio);

			//================
			// Effect runner.
			//================
			GUILayout.Space(10);
			EditorGUILayout.LabelField("Effect Player", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(_spDuration);
			EditorGUILayout.PropertyField(_spUpdateMode);

			// Debug.
			using (new EditorGUI.DisabledGroupScope(!Application.isPlaying))
			using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
			{
				GUILayout.Label("Debug");

				if (GUILayout.Button("Play", "ButtonLeft"))
				{
					(target as UIDissolve).Play();
				}

				if (GUILayout.Button("Stop", "ButtonRight"))
				{
					(target as UIDissolve).Stop();
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		//################################
		// Private Members.
		//################################
		SerializedProperty _spMaterial;
		SerializedProperty _spEffectFactor;
		SerializedProperty _spWidth;
		SerializedProperty _spColor;
		SerializedProperty _spSoftness;
		SerializedProperty _spColorMode;
		SerializedProperty _spNoiseTexture;
		SerializedProperty _spEffectArea;
		SerializedProperty _spKeepAspectRatio;
		SerializedProperty _spDuration;
		SerializedProperty _spUpdateMode;
	}
}