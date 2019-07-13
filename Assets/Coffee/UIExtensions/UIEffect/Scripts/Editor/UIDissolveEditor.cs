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
			foreach (var d in targets.Cast<UIDissolve> ())
			{
				var mat = d.material;
				if (d.isTMPro && mat && mat.HasProperty(s_NoiseTexId))
				{
					ColorMode colorMode =
								mat.IsKeywordEnabled ("ADD") ? ColorMode.Add
										: mat.IsKeywordEnabled ("SUBTRACT") ? ColorMode.Subtract
										: mat.IsKeywordEnabled ("FILL") ? ColorMode.Fill
										: ColorMode.Multiply;

					Texture noiseTexture = mat.GetTexture(s_NoiseTexId);

					if (d.colorMode != colorMode || d.noiseTexture != noiseTexture)
					{
						var so = new SerializedObject (d);
						so.FindProperty ("m_ColorMode").intValue = (int)colorMode;
						so.FindProperty ("m_NoiseTexture").objectReferenceValue = noiseTexture;
						so.ApplyModifiedProperties ();
					}
				}
			}

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

			bool isAnyTMPro = targets.Cast<UIDissolve>().Any(x => x.isTMPro);
			using (new EditorGUI.DisabledGroupScope (isAnyTMPro))
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

			var c = target as UIDissolve;
			c.ShowTMProWarning (_shader, _mobileShader, _spriteShader, mat => {
				if(mat.shader == _spriteShader)
				{
					mat.shaderKeywords = c.material.shaderKeywords;
					mat.SetTexture ("_NoiseTex", c.material.GetTexture ("_NoiseTex"));
				}
			});
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