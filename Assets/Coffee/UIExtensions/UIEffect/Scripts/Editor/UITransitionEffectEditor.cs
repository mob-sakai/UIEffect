using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Linq;

namespace Coffee.UIExtensions.Editors
{
	/// <summary>
	/// UIEffect editor.
	/// </summary>
	[CustomEditor(typeof(UITransitionEffect))]
	[CanEditMultipleObjects]
	public class UITransitionEffectEditor : BaseMeshEffectEditor
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
			base.OnEnable();

			_spMaterial = serializedObject.FindProperty("m_EffectMaterial");
			_spEffectMode = serializedObject.FindProperty("m_EffectMode");
			_spEffectFactor = serializedObject.FindProperty("m_EffectFactor");
			_spEffectArea = serializedObject.FindProperty("m_EffectArea");
			_spKeepAspectRatio = serializedObject.FindProperty("m_KeepAspectRatio");
			_spDissolveWidth = serializedObject.FindProperty("m_DissolveWidth");
			_spDissolveSoftness = serializedObject.FindProperty("m_DissolveSoftness");
			_spDissolveColor = serializedObject.FindProperty("m_DissolveColor");
			_spTransitionTexture = serializedObject.FindProperty("m_TransitionTexture");
			var player = serializedObject.FindProperty("m_Player");
			_spPlay = player.FindPropertyRelative("play");
			_spDuration = player.FindPropertyRelative("duration");
			_spInitialPlayDelay = player.FindPropertyRelative("initialPlayDelay");
			_spLoop = player.FindPropertyRelative("loop");
			_spLoopDelay = player.FindPropertyRelative("loopDelay");
			_spUpdateMode = player.FindPropertyRelative("updateMode");
			_spPassRayOnHidden = serializedObject.FindProperty("m_PassRayOnHidden");

			s_NoiseTexId = Shader.PropertyToID("_NoiseTex");

			_shader = Shader.Find("TextMeshPro/Distance Field (UITransition)");
			_mobileShader = Shader.Find("TextMeshPro/Mobile/Distance Field (UITransition)");
			_spriteShader = Shader.Find("TextMeshPro/Sprite (UITransition)");
		}


		/// <summary>
		/// Implement this function to make a custom inspector.
		/// </summary>
		public override void OnInspectorGUI()
		{
			foreach (var d in targets.Cast<UITransitionEffect> ())
			{
				var mat = d.material;
				if (d.isTMPro && mat && mat.HasProperty (s_NoiseTexId))
				{
					Texture noiseTexture = mat.GetTexture (s_NoiseTexId);
					UITransitionEffect.EffectMode mode =
						mat.IsKeywordEnabled ("CUTOFF") ? UITransitionEffect.EffectMode.Cutoff
						: mat.IsKeywordEnabled ("FADE") ? UITransitionEffect.EffectMode.Fade
						: mat.IsKeywordEnabled ("DISSOLVE") ? UITransitionEffect.EffectMode.Dissolve
						: (UITransitionEffect.EffectMode)0;

					if (mode == (UITransitionEffect.EffectMode)0)
					{
						mode = UITransitionEffect.EffectMode.Cutoff;
						mat.EnableKeyword ("CUTOFF");
					}

					bool hasChanged = d.transitionTexture != noiseTexture || d.effectMode != mode;

					if (hasChanged)
					{
						var so = new SerializedObject (d);
						so.FindProperty ("m_TransitionTexture").objectReferenceValue = noiseTexture;
						so.FindProperty ("m_EffectMode").intValue = (int)mode;
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
			bool isAnyTMPro = targets.Cast<UITransitionEffect>().Any(x => x.isTMPro);
			using (new EditorGUI.DisabledGroupScope(isAnyTMPro))
				EditorGUILayout.PropertyField(_spEffectMode);

			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(_spEffectFactor);
			if (_spEffectMode.intValue == (int)UITransitionEffect.EffectMode.Dissolve)
			{
				EditorGUILayout.PropertyField(_spDissolveWidth);
				EditorGUILayout.PropertyField(_spDissolveSoftness);
				EditorGUILayout.PropertyField(_spDissolveColor);
			}
			EditorGUI.indentLevel--;

			//================
			// Advanced option.
			//================
			EditorGUILayout.PropertyField(_spEffectArea);
			using (new EditorGUI.DisabledGroupScope(isAnyTMPro))
				EditorGUILayout.PropertyField(_spTransitionTexture);
			EditorGUILayout.PropertyField(_spKeepAspectRatio);
			EditorGUILayout.PropertyField(_spPassRayOnHidden);

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

				if (GUILayout.Button("Show", "ButtonLeft"))
				{
					(target as UITransitionEffect).Show();
				}

				if (GUILayout.Button("Hide", "ButtonRight"))
				{
					(target as UITransitionEffect).Hide();
				}
			}

			var current = target as UITransitionEffect;
			current.ShowTMProWarning(_shader, _mobileShader, _spriteShader, mat =>
				{
					if (mat.shader == _spriteShader)
					{
						mat.shaderKeywords = current.material.shaderKeywords;
						mat.SetTexture(s_NoiseTexId, current.material.GetTexture(s_NoiseTexId));
					}
				});
			ShowCanvasChannelsWarning();

			ShowMaterialEditors(current.materials, 1, current.materials.Length - 1);

			serializedObject.ApplyModifiedProperties();
		}

		//################################
		// Private Members.
		//################################
		SerializedProperty _spMaterial;
		SerializedProperty _spEffectMode;
		SerializedProperty _spEffectFactor;
		SerializedProperty _spEffectArea;
		SerializedProperty _spKeepAspectRatio;
		SerializedProperty _spDissolveWidth;
		SerializedProperty _spDissolveSoftness;
		SerializedProperty _spDissolveColor;
		SerializedProperty _spTransitionTexture;
		SerializedProperty _spPlay;
		SerializedProperty _spLoop;
		SerializedProperty _spLoopDelay;
		SerializedProperty _spDuration;
		SerializedProperty _spInitialPlayDelay;
		SerializedProperty _spUpdateMode;
		SerializedProperty _spPassRayOnHidden;

		Shader _shader;
		Shader _mobileShader;
		Shader _spriteShader;
	}
}