using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Linq;
using System;

namespace Coffee.UIExtensions.Editors
{
	/// <summary>
	/// UIEffect editor.
	/// </summary>
	[CustomEditor(typeof(UIEffect))]
	[CanEditMultipleObjects]
	public class UIEffectEditor : BaseMeshEffectEditor
	{
		static readonly GUIContent contentEffectColor = new GUIContent ("Effect Color");


		//################################
		// Public/Protected Members.
		//################################

		/// <summary>
		/// Draw effect properties.
		/// </summary>
		public static void DrawEffectProperties(SerializedObject serializedObject, string colorProperty = "m_Color")
		{
			//================
			// Effect material.
			//================
			var spMaterial = serializedObject.FindProperty("m_EffectMaterial");
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.PropertyField(spMaterial);
			EditorGUI.EndDisabledGroup();

			//================
			// Effect setting.
			//================
			var spToneMode = serializedObject.FindProperty("m_EffectMode");
			EditorGUILayout.PropertyField(spToneMode);

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
			EditorGUILayout.PropertyField(spColorMode);

			// When color is enable, show parameters.
			//if (spColorMode.intValue != (int)ColorMode.Multiply)
			{
				EditorGUI.indentLevel++;

				SerializedProperty spColor = serializedObject.FindProperty(colorProperty);
				if (spColor == null && serializedObject.targetObject is UIEffect) {
					spColor = new SerializedObject (serializedObject.targetObjects.Select(x=>(x as UIEffect).targetGraphic).ToArray()).FindProperty(colorProperty);
				}

				EditorGUI.BeginChangeCheck ();
				EditorGUI.showMixedValue = spColor.hasMultipleDifferentValues;
#if UNITY_2018_1_OR_NEWER
				spColor.colorValue = EditorGUILayout.ColorField (contentEffectColor, spColor.colorValue, true, false, false);
#else
				spColor.colorValue = EditorGUILayout.ColorField (contentEffectColor, spColor.colorValue, true, false, false, null);
#endif
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
			EditorGUILayout.PropertyField(spBlurMode);

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

		int GetEnum<T>(Material mat)
		{
			Type type = typeof(T);
			string[] names = System.Enum.GetNames (type);
			int[] values = System.Enum.GetValues (type) as int[];

			int mode = 0;
			for(int i=0;i<names.Length;i++)
			{
				if (mat.IsKeywordEnabled (names [i].ToUpper()))
					mode = values [i];
			}
			return mode;
		}


		/// <summary>
		/// Implement this function to make a custom inspector.
		/// </summary>
		public override void OnInspectorGUI()
		{
			foreach (var d in targets.Cast<UIEffect> ())
			{
				var mat = d.material;
				if (d.isTMPro && mat)
				{
					var so = new SerializedObject (d);
					EffectMode eMode = (EffectMode)GetEnum<EffectMode> (mat);
					ColorMode cMode = (ColorMode)GetEnum<ColorMode> (mat);
					BlurMode bMode = (BlurMode)GetEnum<BlurMode> (mat);
					bool aBlur = mat.IsKeywordEnabled("EX");
					if (d.effectMode != eMode || d.colorMode != cMode || d.blurMode != bMode || so.FindProperty ("m_AdvancedBlur").boolValue != aBlur)
					{
						so.FindProperty ("m_EffectMode").intValue = (int)eMode;
						so.FindProperty ("m_ColorMode").intValue = (int)cMode;
						so.FindProperty ("m_BlurMode").intValue = (int)bMode;
						so.FindProperty ("m_AdvancedBlur").boolValue = aBlur;
						so.ApplyModifiedProperties ();
					}
				}
			}


			serializedObject.Update();
			bool isAnyTMPro = targets.Cast<UIEffect>().Any(x => x.isTMPro);
			var c = target as UIEffect;

			//================
			// Effect material.
			//================
			var spMaterial = serializedObject.FindProperty("m_EffectMaterial");
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.PropertyField(spMaterial);
			EditorGUI.EndDisabledGroup();

			//================
			// Effect setting.
			//================
			var spToneMode = serializedObject.FindProperty("m_EffectMode");
			using (new EditorGUI.DisabledGroupScope(isAnyTMPro))
				EditorGUILayout.PropertyField(spToneMode);

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
			using (new EditorGUI.DisabledGroupScope(isAnyTMPro))
				EditorGUILayout.PropertyField(spColorMode);

			// When color is enable, show parameters.
			//if (spColorMode.intValue != (int)ColorMode.Multiply)
			{
				EditorGUI.indentLevel++;

				SerializedProperty spColor = serializedObject.FindProperty("m_Color");
				if (spColor == null && serializedObject.targetObject is UIEffect) {
					spColor = new SerializedObject (serializedObject.targetObjects.Select(x=>(x as UIEffect).targetGraphic).ToArray()).FindProperty(!isAnyTMPro ? "m_Color" : "m_fontColor");
				}

				EditorGUI.BeginChangeCheck ();
				EditorGUI.showMixedValue = spColor.hasMultipleDifferentValues;
				#if UNITY_2018_1_OR_NEWER
				spColor.colorValue = EditorGUILayout.ColorField (contentEffectColor, spColor.colorValue, true, false, false);
				#else
				spColor.colorValue = EditorGUILayout.ColorField (contentEffectColor, spColor.colorValue, true, false, false, null);
				#endif
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
			using (new EditorGUI.DisabledGroupScope(isAnyTMPro))
				EditorGUILayout.PropertyField(spBlurMode);

			// When blur is enable, show parameters.
			if (spBlurMode.intValue != (int)BlurMode.None)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_BlurFactor"));

				var spAdvancedBlur = serializedObject.FindProperty("m_AdvancedBlur");
				using (new EditorGUI.DisabledGroupScope(isAnyTMPro))
					EditorGUILayout.PropertyField(spAdvancedBlur);
				EditorGUI.indentLevel--;
			}

			serializedObject.ApplyModifiedProperties();

			c.ShowTMProWarning (_shader, _mobileShader, _spriteShader, mat => {});
			ShowCanvasChannelsWarning ();

			ShowMaterialEditors (c.materials, 1, c.materials.Length - 1);

			serializedObject.ApplyModifiedProperties();
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable ();

			_shader = Shader.Find ("TextMeshPro/Distance Field (UIEffect)");
			_mobileShader = Shader.Find ("TextMeshPro/Mobile/Distance Field (UIEffect)");
			_spriteShader = Shader.Find ("TextMeshPro/Sprite (UIEffect)");
		}

		//################################
		// Private Members.
		//################################
		Shader _shader;
		Shader _mobileShader;
		Shader _spriteShader;

	}
}