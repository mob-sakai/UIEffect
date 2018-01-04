using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace UnityEditor.UI
{
	/// <summary>
	/// UIEffectCapturedImage editor.
	/// </summary>
	[CustomEditor(typeof(UIEffectCapturedImage))]
	[CanEditMultipleObjects]
	public class UIEffectCapturedImageEditor : RawImageEditor
	{
		/// <summary>
		/// Implement this function to make a custom inspector.
		/// </summary>
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			//================
			// Basic properties.
			//================
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Texture"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Color"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_RaycastTarget"));

			//================
			// Capture effect.
			//================
			GUILayout.Space(10);
			EditorGUILayout.LabelField("Capture Effect", EditorStyles.boldLabel);
			UIEffectEditor.DrawEffectProperties(serializedObject);

			//================
			// Advanced option.
			//================
			GUILayout.Space(10);
			EditorGUILayout.LabelField("Advanced Option", EditorStyles.boldLabel);

			// Desampling rate.
			DrawDesamplingRate(serializedObject.FindProperty("m_DesamplingRate"));

			// Reduction rate.
			DrawDesamplingRate(serializedObject.FindProperty("m_ReductionRate"));

			// Filter Mode.
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_FilterMode"));

			serializedObject.ApplyModifiedProperties();

			// Debug.
			using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
			{
				GUILayout.Label("Debug");

				if (GUILayout.Button("Capture", "ButtonLeft"))
					UpdateTexture(true);

				EditorGUI.BeginDisabledGroup(!(target as UIEffectCapturedImage).capturedTexture);
				if (GUILayout.Button("Release", "ButtonRight"))
					UpdateTexture(false);
				EditorGUI.EndDisabledGroup();
			}
		}

		/// <summary>
		/// Draws the desampling rate.
		/// </summary>
		void DrawDesamplingRate(SerializedProperty sp)
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.PropertyField(sp);
				int w, h;
				(target as UIEffectCapturedImage).GetDesamplingSize((UIEffectCapturedImage.DesamplingRate)sp.intValue, out w, out h);
				GUILayout.Label(string.Format("{0}x{1}", w, h), EditorStyles.miniLabel);
			}
		}

		/// <summary>
		/// Updates the texture.
		/// </summary>
		void UpdateTexture(bool capture)
		{
			var current = target as UIEffectCapturedImage;
			bool enable = current.enabled;
			current.enabled = false;
			current.Release();
			if(capture)
				current.Capture();
			
			EditorApplication.delayCall += ()=>current.enabled = enable;
		}
	}
}