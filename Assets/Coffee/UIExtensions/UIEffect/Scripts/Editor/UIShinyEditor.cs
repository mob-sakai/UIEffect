using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Linq;

namespace Coffee.UIExtensions.Editors
{
	/// <summary>
	/// UIEffect editor.
	/// </summary>
	[CustomEditor(typeof(UIShiny))]
	[CanEditMultipleObjects]
	public class UIShinyEditor : BaseMeshEffectEditor
	{
		//################################
		// Public/Protected Members.
		//################################
		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable ();

			_spMaterial = serializedObject.FindProperty("m_EffectMaterial");
			_spEffectFactor = serializedObject.FindProperty("m_EffectFactor");
			_spEffectArea = serializedObject.FindProperty("m_EffectArea");
			_spWidth = serializedObject.FindProperty("m_Width");
			_spRotation = serializedObject.FindProperty("m_Rotation");
			_spSoftness = serializedObject.FindProperty("m_Softness");
			_spBrightness = serializedObject.FindProperty("m_Brightness");
			_spGloss = serializedObject.FindProperty("m_Gloss");
			var player = serializedObject.FindProperty("m_Player");
			_spPlay = player.FindPropertyRelative("play");
			_spDuration = player.FindPropertyRelative("duration");
			_spInitialPlayDelay = player.FindPropertyRelative("initialPlayDelay");
			_spLoop = player.FindPropertyRelative("loop");
			_spLoopDelay = player.FindPropertyRelative("loopDelay");
			_spUpdateMode = player.FindPropertyRelative("updateMode");


			_shader = Shader.Find ("TextMeshPro/Distance Field (UIShiny)");
			_mobileShader = Shader.Find ("TextMeshPro/Mobile/Distance Field (UIShiny)");
			_spriteShader = Shader.Find ("TextMeshPro/Sprite (UIShiny)");
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
			EditorGUILayout.PropertyField(_spRotation);
			EditorGUILayout.PropertyField(_spSoftness);
			EditorGUILayout.PropertyField(_spBrightness);
			EditorGUILayout.PropertyField(_spGloss);

			//================
			// Advanced option.
			//================
			EditorGUILayout.PropertyField(_spEffectArea);

			//================
			// Effect player.
			//================
			EditorGUILayout.PropertyField(_spPlay);
			EditorGUILayout.PropertyField(_spDuration);
			EditorGUILayout.PropertyField(_spInitialPlayDelay);
			EditorGUILayout.PropertyField(_spLoop);
			EditorGUILayout.PropertyField(_spLoopDelay);
			EditorGUILayout.PropertyField(_spUpdateMode);

			// Debug.
			using (new EditorGUI.DisabledGroupScope(!Application.isPlaying))
			using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
			{
				GUILayout.Label("Debug");

				if (GUILayout.Button("Play", "ButtonLeft"))
				{
					(target as UIShiny).Play();
				}

				if (GUILayout.Button("Stop", "ButtonRight"))
				{
					(target as UIShiny).Stop();
				}
			}

			var c = target as UIShiny;
			c.ShowTMProWarning (_shader, _mobileShader, _spriteShader, mat => {});
			ShowCanvasChannelsWarning ();

			ShowMaterialEditors (c.materials, 1, c.materials.Length - 1);

			serializedObject.ApplyModifiedProperties();
		}

		//################################
		// Private Members.
		//################################
		SerializedProperty _spMaterial;
		SerializedProperty _spEffectFactor;
		SerializedProperty _spWidth;
		SerializedProperty _spRotation;
		SerializedProperty _spSoftness;
		SerializedProperty _spBrightness;
		SerializedProperty _spGloss;
		SerializedProperty _spEffectArea;
		SerializedProperty _spPlay;
		SerializedProperty _spLoop;
		SerializedProperty _spLoopDelay;
		SerializedProperty _spDuration;
		SerializedProperty _spInitialPlayDelay;
		SerializedProperty _spUpdateMode;

		Shader _shader;
		Shader _mobileShader;
		Shader _spriteShader;
	}
}