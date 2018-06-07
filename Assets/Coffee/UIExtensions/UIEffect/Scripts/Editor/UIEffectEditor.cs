using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Coffee.UIExtensions
{
	/// <summary>
	/// UIEffect editor.
	/// </summary>
	[CustomEditor(typeof(UIEffect))]
	[CanEditMultipleObjects]
	public class UIEffectEditor : Editor
	{
		//################################
		// Constant or Static Members.
		//################################
		/// <summary>
		/// Draw effect properties.
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
			if (spToneMode.intValue != (int)ToneMode.None)
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
			if (spColorMode.intValue != (int)ColorMode.Multiply)
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
			if (spBlurMode.intValue != (int)BlurMode.None)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Blur"));
				EditorGUI.indentLevel--;
			}

//			// Set effect material.
//			if (!serializedObject.isEditingMultipleObjects && spToneMode.intValue == 0 && spColorMode.intValue == 0 && spBlurMode.intValue == 0)
//			{
//				spMaterial.objectReferenceValue = null;
//			}
//			else if (changed || !serializedObject.isEditingMultipleObjects)
//			{
//				spMaterial.objectReferenceValue = UIEffect.GetOrGenerateMaterialVariant(Shader.Find(shaderName),
//					(UIEffect.ToneMode)spToneMode.intValue,
//					(UIEffect.ColorMode)spColorMode.intValue,
//					(UIEffect.BlurMode)spBlurMode.intValue
//				);
//			}
		}

		//################################
		// Private Members.
		//################################
		ReorderableList _roAdditionalShadows;
		SerializedProperty _spAdditionalShadows;
		SerializedProperty _spBlurMode;
		SerializedProperty _spCustomEffect;
		SerializedProperty _spEffectMaterial;
		SerializedProperty _spEffectColor;
		SerializedProperty _spCustomFactorX;
		SerializedProperty _spCustomFactorY;
		SerializedProperty _spCustomFactorZ;
		SerializedProperty _spCustomFactorW;

		void OnEnable()
		{
			_spAdditionalShadows = serializedObject.FindProperty("m_AdditionalShadows");
			_spBlurMode = serializedObject.FindProperty("m_BlurMode");
			_spEffectColor = serializedObject.FindProperty("m_EffectColor");

			_spCustomEffect = serializedObject.FindProperty("m_CustomEffect");
			_spEffectMaterial = serializedObject.FindProperty("m_EffectMaterial");
			var spFactor = serializedObject.FindProperty("m_CustomFactor");
			_spCustomFactorX = spFactor.FindPropertyRelative("x");
			_spCustomFactorY = spFactor.FindPropertyRelative("y");
			_spCustomFactorZ = spFactor.FindPropertyRelative("z");
			_spCustomFactorW = spFactor.FindPropertyRelative("w");

			_roAdditionalShadows = new ReorderableList(serializedObject, _spAdditionalShadows, true, true, true, true);
			_roAdditionalShadows.drawElementCallback = DrawElementCallback;
			_roAdditionalShadows.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Additional Shadows");
			_roAdditionalShadows.onAddCallback = OnAddCallback;
			_roAdditionalShadows.elementHeightCallback = ElementHeightCallback;

		}

		void OnAddCallback(ReorderableList ro)
		{
			_spAdditionalShadows.InsertArrayElementAtIndex(ro.count);
			var element = _spAdditionalShadows.GetArrayElementAtIndex(ro.count - 1);
			element.FindPropertyRelative("shadowMode").intValue = (int)ShadowStyle.Shadow;
			element.FindPropertyRelative("shadowColor").colorValue = Color.black;
			element.FindPropertyRelative("effectDistance").vector2Value = new Vector2(1f, -1f);
			element.FindPropertyRelative("useGraphicAlpha").boolValue = true;
			element.FindPropertyRelative("shadowBlur").floatValue = 0.25f;
		}

		float ElementHeightCallback(int index)
		{
			var element = _spAdditionalShadows.GetArrayElementAtIndex(index);
			if (element.FindPropertyRelative("shadowMode").intValue == (int)ShadowStyle.None)
				return 16;

			return (_spBlurMode.intValue == (int)BlurMode.None ? 66 : 84) + (EditorGUIUtility.wideMode ? 0 : 18);
		}

		/// <summary>
		/// 
		/// </summary>
		void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
		{
			var sp = _roAdditionalShadows.serializedProperty.GetArrayElementAtIndex(index);

			Rect r = new Rect(rect);
			r.height = EditorGUIUtility.singleLineHeight;
			var spMode = sp.FindPropertyRelative("shadowMode");
			EditorGUI.PropertyField(r, spMode);
			if (spMode.intValue == (int)ShadowStyle.None)
				return;

			r.y += r.height;
			EditorGUI.PropertyField(r, sp.FindPropertyRelative("shadowColor"));
			r.y += r.height;
			EditorGUI.PropertyField(r, sp.FindPropertyRelative("effectDistance"));
			r.y += EditorGUIUtility.wideMode ? r.height : r.height * 2;
			EditorGUI.PropertyField(r, sp.FindPropertyRelative("useGraphicAlpha"));

			if (_spBlurMode.intValue != (int)BlurMode.None)
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

			// Custom effect.
			EditorGUILayout.PropertyField(_spCustomEffect);
			if(_spCustomEffect.boolValue)
			{
				EditorGUILayout.PropertyField(_spEffectMaterial);

				EditorGUI.indentLevel++;
				EditorGUILayout.Slider(_spCustomFactorX, 0, 1, new GUIContent("Effect Factor X"));
				EditorGUILayout.Slider(_spCustomFactorY, 0, 1, new GUIContent("Effect Factor Y"));
				EditorGUILayout.Slider(_spCustomFactorZ, 0, 1, new GUIContent("Effect Factor Z"));
				EditorGUILayout.Slider(_spCustomFactorW, 0, 1, new GUIContent("Effect Factor W"));
				EditorGUILayout.PropertyField(_spEffectColor);
				EditorGUI.indentLevel--; 
			}
			else
			{
				DrawEffectProperties(UIEffect.shaderName, serializedObject);
			}

			//================
			// Shadow setting.
			//================
			var spShadowMode = serializedObject.FindProperty("m_ShadowStyle");
			EditorGUILayout.PropertyField(spShadowMode);

			// When shadow is enable, show parameters.
			if (spShadowMode.intValue != (int)ShadowStyle.None)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_EffectDistance"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ShadowColor"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_UseGraphicAlpha"));

				if (_spBlurMode.intValue != (int)BlurMode.None)
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ShadowBlur"));
				}
				EditorGUI.indentLevel--;
			}

			//================
			// Additional shadow setting.
			//================
			_roAdditionalShadows.DoLayoutList();

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