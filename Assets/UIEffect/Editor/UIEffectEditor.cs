using System.IO;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	/// <summary>
	/// UIEffect editor.
	/// </summary>
	[CustomEditor(typeof(UIEffect))]
	[CanEditMultipleObjects]
	public class UIEffectEditor : Editor
	{
		ReorderableList roAdditionalShadows;
		SerializedProperty spAdditionalShadows;
		SerializedProperty spBlurMode;

		/// <summary>
		/// Implement this function to make a custom inspector.
		/// </summary>
		public static void DrawEffectProperties(string shaderName, SerializedObject serializedObject)
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
			var spToneMode = serializedObject.FindProperty("m_ToneMode");
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(spToneMode);
			changed |= EditorGUI.EndChangeCheck();

			// When tone is enable, show parameters.
			if (spToneMode.intValue != (int)UIEffect.ToneMode.None)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ToneLevel"));
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
			if (spColorMode.intValue != (int)UIEffect.ColorMode.None)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_EffectColor"));
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
			if (spBlurMode.intValue != (int)UIEffect.BlurMode.None)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Blur"));
				EditorGUI.indentLevel--;
			}

			// Set effect material.
			if (!serializedObject.isEditingMultipleObjects && spToneMode.intValue == 0 && spColorMode.intValue == 0 && spBlurMode.intValue == 0)
			{
				spMaterial.objectReferenceValue = null;
			}
			else if (changed || !serializedObject.isEditingMultipleObjects)
			{
				spMaterial.objectReferenceValue = UIEffect.GetMaterial(Shader.Find(shaderName),
					(UIEffect.ToneMode)spToneMode.intValue,
					(UIEffect.ColorMode)spColorMode.intValue,
					(UIEffect.BlurMode)spBlurMode.intValue
				);
			}
		}

		public static Material GetOrCreateMaterial(Shader shader, UIEffect.ToneMode tone, UIEffect.ColorMode color, UIEffect.BlurMode blur)
		{
			Material mat = UIEffect.GetMaterial(shader, tone, color, blur);
			if (!mat)
			{
				mat = new Material(shader);

				if (0 < tone)
					mat.EnableKeyword("UI_TONE_" + tone.ToString().ToUpper());
				if (0 < color)
					mat.EnableKeyword("UI_COLOR_" + color.ToString().ToUpper());
				if (0 < blur)
					mat.EnableKeyword("UI_BLUR_" + blur.ToString().ToUpper());

				mat.name = Path.GetFileName(shader.name)
				+ (0 < tone ? "-" + tone : "")
				+ (0 < color ? "-" + color : "")
				+ (0 < blur ? "-" + blur : "");
				//mat.hideFlags = HideFlags.NotEditable;

				Directory.CreateDirectory("Assets/UIEffect/Materials");
				AssetDatabase.CreateAsset(mat, "Assets/UIEffect/Materials/" + mat.name + ".mat");
			}
			return mat;
		}

		void OnEnable()
		{
			spAdditionalShadows = serializedObject.FindProperty("m_AdditionalShadows");
			spBlurMode = serializedObject.FindProperty("m_BlurMode");

			roAdditionalShadows = new ReorderableList(serializedObject, spAdditionalShadows, true, true, true, true);
			roAdditionalShadows.drawElementCallback = DrawElementCallback;
			roAdditionalShadows.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Additional Shadows");
			roAdditionalShadows.onAddCallback = OnAddCallback;
			roAdditionalShadows.elementHeightCallback = ElementHeightCallback;
		}

		void OnAddCallback(ReorderableList ro)
		{
			spAdditionalShadows.InsertArrayElementAtIndex(ro.count);
			var element = spAdditionalShadows.GetArrayElementAtIndex(ro.count - 1);
			element.FindPropertyRelative("shadowMode").intValue = (int)UIEffect.ShadowMode.Shadow;
			element.FindPropertyRelative("shadowColor").colorValue = Color.black;
			element.FindPropertyRelative("effectDistance").vector2Value = new Vector2(1f, -1f);
			element.FindPropertyRelative("useGraphicAlpha").boolValue = true;
			element.FindPropertyRelative("shadowBlur").floatValue = 0.25f;
		}

		float ElementHeightCallback(int index)
		{
			var element = spAdditionalShadows.GetArrayElementAtIndex(index);
			if (element.FindPropertyRelative("shadowMode").intValue == (int)UIEffect.ShadowMode.None)
				return 16;

			return (spBlurMode.intValue == (int)UIEffect.BlurMode.None ? 66 : 84) + (EditorGUIUtility.wideMode ? 0 : 18);
		}

		/// <summary>
		/// 
		/// </summary>
		void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
		{
			var sp = roAdditionalShadows.serializedProperty.GetArrayElementAtIndex(index);

			Rect r = new Rect(rect);
			r.height = EditorGUIUtility.singleLineHeight;
			var spMode = sp.FindPropertyRelative("shadowMode");
			EditorGUI.PropertyField(r, spMode);
			if (spMode.intValue == (int)UIEffect.ShadowMode.None)
				return;

			r.y += r.height;
			EditorGUI.PropertyField(r, sp.FindPropertyRelative("shadowColor"));
			r.y += r.height;
			EditorGUI.PropertyField(r, sp.FindPropertyRelative("effectDistance"));
			r.y += EditorGUIUtility.wideMode ? r.height : r.height * 2;
			EditorGUI.PropertyField(r, sp.FindPropertyRelative("useGraphicAlpha"));

			if (spBlurMode.intValue != (int)UIEffect.BlurMode.None)
			{
				r.y += r.height;
				EditorGUI.PropertyField(r, sp.FindPropertyRelative("shadowBlur"));
			}
		}

		/// <summary>
		/// Implement this function to make a custom inspector.
		/// </summary>
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			DrawEffectProperties(UIEffect.shaderName, serializedObject);

			//================
			// Shadow setting.
			//================
			var spShadowMode = serializedObject.FindProperty("m_ShadowMode");
			EditorGUILayout.PropertyField(spShadowMode);

			// When shadow is enable, show parameters.
			if (spShadowMode.intValue != (int)UIEffect.ShadowMode.None)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_EffectDistance"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ShadowColor"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_UseGraphicAlpha"));

				if (spBlurMode.intValue != (int)UIEffect.BlurMode.None)
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ShadowBlur"));
				}
				EditorGUI.indentLevel--;
			}

			//================
			// Additional shadow setting.
			//================
			roAdditionalShadows.DoLayoutList();

			serializedObject.ApplyModifiedProperties();

#if UNITY_5_6_OR_NEWER
			if((target as UIEffect).graphic)
			{
				var canvas = (target as UIEffect).graphic.canvas;
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