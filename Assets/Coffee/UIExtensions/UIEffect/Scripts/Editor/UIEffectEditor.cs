using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Linq;

namespace Coffee.UIExtensions
{
	/// <summary>
	/// UIEffect editor.
	/// </summary>
	[CustomEditor(typeof(UIEffect))]
	[CanEditMultipleObjects]
	public class UIEffectEditor : Editor
	{
		static readonly GUIContent contentEffectColor = new GUIContent ("Effect Color");

		//################################
		// Constant or Static Members.
		//################################
		/// <summary>
		/// Draw effect properties.
		/// </summary>
		public static void DrawEffectProperties(SerializedObject serializedObject)
		{
			bool changed = false;

			//================
			// Effect material.
			//================
			var spMaterial = serializedObject.FindProperty("m_EffectMaterial");
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.PropertyField(spMaterial);
			EditorGUI.EndDisabledGroup();

			//================
			// Tone setting.
			//================
			var spToneMode = serializedObject.FindProperty("m_EffectMode");
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(spToneMode);
			changed |= EditorGUI.EndChangeCheck();

			// When tone is enable, show parameters.
			if (spToneMode.intValue != (int)EffectMode.None)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_EffectFactor"));
				EditorGUI.indentLevel--;
			}

			//================
			// Color setting.
			//================
			var spColorMode = serializedObject.FindProperty("m_ColorMode");
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(spColorMode);
			changed |= EditorGUI.EndChangeCheck();

			// When color is enable, show parameters.
			//if (spColorMode.intValue != (int)ColorMode.Multiply)
			{
				EditorGUI.indentLevel++;

				SerializedProperty spColor = serializedObject.FindProperty("m_Color");
				if (spColor == null && serializedObject.targetObject is UIEffect) {
					spColor = new SerializedObject (serializedObject.targetObjects.Select(x=>(x as UIEffect).targetGraphic).ToArray()).FindProperty("m_Color");
				}

				EditorGUI.BeginChangeCheck ();
				EditorGUI.showMixedValue = spColor.hasMultipleDifferentValues;
				spColor.colorValue = EditorGUILayout.ColorField (contentEffectColor, spColor.colorValue, true, false, false, null);
				if (EditorGUI.EndChangeCheck ()) {
					spColor.serializedObject.ApplyModifiedProperties ();
				}

				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ColorFactor"));
				EditorGUI.indentLevel--;
			}

			//================
			// Blur setting.
			//================
			var spBlurMode = serializedObject.FindProperty("m_BlurMode");
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(spBlurMode);
			changed |= EditorGUI.EndChangeCheck();

			// When blur is enable, show parameters.
			if (spBlurMode.intValue != (int)BlurMode.None)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_BlurFactor"));

				var spAdvancedBlur = serializedObject.FindProperty("m_AdvancedBlur");
				if (spAdvancedBlur != null)
				{
					EditorGUILayout.PropertyField(spAdvancedBlur);
				}
				EditorGUI.indentLevel--;
			}
		}

		//################################
		// Private Members.
		//################################

		/// <summary>
		/// Implement this function to make a custom inspector.
		/// </summary>
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			DrawEffectProperties(serializedObject);

			serializedObject.ApplyModifiedProperties();

#if UNITY_5_6_OR_NEWER
			var graphic = (target as UIEffectBase).targetGraphic;
			if(graphic)
			{
				var canvas = graphic.canvas;
				if( canvas && 0 == (canvas.additionalShaderChannels & AdditionalCanvasShaderChannels.TexCoord1))
				{
					using (new GUILayout.HorizontalScope())
					{
						EditorGUILayout.HelpBox("[Unity5.6+] Enable TexCoord1 of Canvas.additionalShaderChannels to use UIEffect.", MessageType.Warning);
						if (GUILayout.Button("Fix"))
							canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
					}
				}
			}
#endif
		}
	}
}