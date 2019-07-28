using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Coffee.UIExtensions.Editors
{
	/// <summary>
	/// UIEffect editor.
	/// </summary>
	[CustomEditor(typeof(UIDissolve))]
	[CanEditMultipleObjects]
	public class UIDissolveEditor : BaseMeshEffectEditor
	{
		class ChangeMaterialScope : EditorGUI.ChangeCheckScope
		{
			Object [] targets;
			public ChangeMaterialScope (Object [] targets)
			{
				this.targets = targets;
			}

			protected override void CloseScope ()
			{
				if (changed)
				{
					var graphics = targets.OfType<UIEffectBase> ()
						.Select (x => x.targetGraphic)
						.Where (x => x);

					foreach (var g in graphics)
					{
						g.SetMaterialDirty ();
					}
				}

				base.CloseScope ();
			}
		}

		static int s_NoiseTexId;

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
			_spKeepAspectRatio = serializedObject.FindProperty("m_KeepAspectRatio");
			_spWidth = serializedObject.FindProperty("m_Width");
			_spColor = serializedObject.FindProperty("m_Color");
			_spSoftness = serializedObject.FindProperty("m_Softness");
			_spColorMode = serializedObject.FindProperty("m_ColorMode");
			_spNoiseTexture = serializedObject.FindProperty("m_NoiseTexture");
			_spKeepAspectRatio = serializedObject.FindProperty("m_KeepAspectRatio");
			_spReverse = serializedObject.FindProperty("m_Reverse");
			var player = serializedObject.FindProperty("m_Player");
			_spPlay = player.FindPropertyRelative("play");
			_spDuration = player.FindPropertyRelative("duration");
			_spInitialPlayDelay = player.FindPropertyRelative("initialPlayDelay");
			_spLoop = player.FindPropertyRelative("loop");
			_spLoopDelay = player.FindPropertyRelative("loopDelay");
			_spUpdateMode = player.FindPropertyRelative("updateMode");

			s_NoiseTexId = Shader.PropertyToID ("_NoiseTex");

			_shader = Shader.Find ("TextMeshPro/Distance Field (UIDissolve)");
			_mobileShader = Shader.Find ("TextMeshPro/Mobile/Distance Field (UIDissolve)");
			_spriteShader = Shader.Find ("TextMeshPro/Sprite (UIDissolve)");
		}

		/// <summary>
		/// Implement this function to make a custom inspector.
		/// </summary>
		public override void OnInspectorGUI()
		{

			serializedObject.Update();

			//================
			// Effect setting.
			//================
			EditorGUILayout.PropertyField(_spEffectFactor);
			EditorGUILayout.PropertyField(_spWidth);
			EditorGUILayout.PropertyField(_spSoftness);
			EditorGUILayout.PropertyField(_spColor);

			using (new ChangeMaterialScope (targets))
			{
				EditorGUILayout.PropertyField (_spColorMode);
				EditorGUILayout.PropertyField (_spNoiseTexture);
			}

			//================
			// Advanced option.
			//================
			EditorGUILayout.PropertyField(_spEffectArea);
			EditorGUILayout.PropertyField(_spKeepAspectRatio);

			//================
			// Effect player.
			//================
			EditorGUILayout.PropertyField(_spPlay);
			EditorGUILayout.PropertyField(_spDuration);
			EditorGUILayout.PropertyField(_spInitialPlayDelay);
			EditorGUILayout.PropertyField(_spLoop);
			EditorGUILayout.PropertyField(_spLoopDelay);
			EditorGUILayout.PropertyField(_spUpdateMode);
			EditorGUILayout.PropertyField(_spReverse);

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

			ShowCanvasChannelsWarning ();

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
		SerializedProperty _spReverse;
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