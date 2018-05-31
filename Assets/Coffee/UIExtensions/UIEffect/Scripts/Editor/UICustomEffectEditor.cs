using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Coffee.UIExtensions
{
	/// <summary>
	/// UIEffect editor.
	/// </summary>
	[CustomEditor(typeof(UICustomEffect))]
	[CanEditMultipleObjects]
	public class UICustomEffectEditor : Editor
	{
		//################################
		// Private Members.
		//################################
		SerializedProperty spCustomFactor1X;
		SerializedProperty spCustomFactor1Y;
		SerializedProperty spCustomFactor1Z;
		SerializedProperty spCustomFactor1W;
		SerializedProperty spCustomFactor2X;
		SerializedProperty spCustomFactor2Y;
		SerializedProperty spCustomFactor2Z;
		SerializedProperty spCustomFactor2W;
		SerializedProperty _spEffectMaterial;

		void OnEnable()
		{
			var spCustomFactor1 = serializedObject.FindProperty("m_CustomFactor1");
			spCustomFactor1X = spCustomFactor1.FindPropertyRelative("x");
			spCustomFactor1Y = spCustomFactor1.FindPropertyRelative("y");
			spCustomFactor1Z = spCustomFactor1.FindPropertyRelative("z");
			spCustomFactor1W = spCustomFactor1.FindPropertyRelative("w");

			var spCustomFactor2 = serializedObject.FindProperty("m_CustomFactor2");
			spCustomFactor2X = spCustomFactor2.FindPropertyRelative("x");
			spCustomFactor2Y = spCustomFactor2.FindPropertyRelative("y");
			spCustomFactor2Z = spCustomFactor2.FindPropertyRelative("z");
			spCustomFactor2W = spCustomFactor2.FindPropertyRelative("w");

			_spEffectMaterial = serializedObject.FindProperty("m_EffectMaterial");
		}

		/// <summary>
		/// Implement this function to make a custom inspector.
		/// </summary>
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.PropertyField(_spEffectMaterial);

			EditorGUILayout.Slider(spCustomFactor1X, 0, 1, "CustomFactor1.X");
			EditorGUILayout.Slider(spCustomFactor1Y, 0, 1, "CustomFactor1.Y");
			EditorGUILayout.Slider(spCustomFactor1Z, 0, 1, "CustomFactor1.Z");
			EditorGUILayout.Slider(spCustomFactor1W, 0, 1, "CustomFactor1.W");
			EditorGUILayout.Slider(spCustomFactor2X, 0, 1, "CustomFactor2.X");
			EditorGUILayout.Slider(spCustomFactor2Y, 0, 1, "CustomFactor2.Y");
			EditorGUILayout.Slider(spCustomFactor2Z, 0, 1, "CustomFactor2.Z");
			EditorGUILayout.Slider(spCustomFactor2W, 0, 1, "CustomFactor2.W");

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
						EditorGUILayout.HelpBox("[Unity5.6+] Enable TexCoord1 of Canvas.additionalShaderChannels to use UICustomEffect.", MessageType.Warning);
						if (GUILayout.Button("Fix"))
							canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
					}
				}
			}
#endif
		}
	}
}