using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Linq;

namespace Coffee.UIExtensions.Editors
{
	/// <summary>
	/// UIEffect editor.
	/// </summary>
	[CustomEditor(typeof(UIHsvModifier))]
	[CanEditMultipleObjects]
	public class UIHsvModifierEditor : BaseMeshEffectEditor
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
			_spTargetColor = serializedObject.FindProperty("m_TargetColor");
			_spRange = serializedObject.FindProperty("m_Range");
			_spHue = serializedObject.FindProperty("m_Hue");
			_spSaturation = serializedObject.FindProperty("m_Saturation");
			_spValue = serializedObject.FindProperty("m_Value");

			_shader = Shader.Find ("TextMeshPro/Distance Field (UIHsvModifier)");
			_mobileShader = Shader.Find ("TextMeshPro/Mobile/Distance Field (UIHsvModifier)");
			_spriteShader = Shader.Find ("TextMeshPro/Sprite (UIHsvModifier)");
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
			EditorGUILayout.PropertyField(_spTargetColor);
			EditorGUILayout.PropertyField(_spRange);
			EditorGUILayout.PropertyField(_spHue);
			EditorGUILayout.PropertyField(_spSaturation);
			EditorGUILayout.PropertyField(_spValue);

			var c = target as UIHsvModifier;
			c.ShowTMProWarning (_shader, _mobileShader, _spriteShader, mat => {});
			ShowCanvasChannelsWarning ();

			ShowMaterialEditors (c.materials, 1, c.materials.Length - 1);

			serializedObject.ApplyModifiedProperties();
		}

		//################################
		// Private Members.
		//################################
		SerializedProperty _spMaterial;
		SerializedProperty _spTargetColor;
		SerializedProperty _spRange;
		SerializedProperty _spHue;
		SerializedProperty _spSaturation;
		SerializedProperty _spValue;

		Shader _shader;
		Shader _mobileShader;
		Shader _spriteShader;
	}
}