using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using DesamplingRate = Coffee.UIExtensions.UIEffectCapturedImage.DesamplingRate;

namespace Coffee.UIExtensions.Editors
{
	/// <summary>
	/// UIEffectCapturedImage editor.
	/// </summary>
	[CustomEditor(typeof(UIEffectCapturedImage))]
	[CanEditMultipleObjects]
	public class UIEffectCapturedImageEditor : RawImageEditor
	{
		//################################
		// Constant or Static Members.
		//################################

		public enum QualityMode : int
		{
			Fast = (DesamplingRate.x2 << 0) + (DesamplingRate.x2 << 4) + (FilterMode.Bilinear << 8) + (2 << 10),
			Medium = (DesamplingRate.x1 << 0) + (DesamplingRate.x1 << 4) + (FilterMode.Bilinear << 8) + (3 << 10),
			Detail = (DesamplingRate.None << 0) + (DesamplingRate.x1 << 4) + (FilterMode.Bilinear << 8) + (5 << 10),
			Custom = -1,
		}


		//################################
		// Public/Protected Members.
		//################################
		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			_spTexture = serializedObject.FindProperty("m_Texture");
			_spColor = serializedObject.FindProperty("m_Color");
			_spRaycastTarget = serializedObject.FindProperty("m_RaycastTarget");
			_spDesamplingRate = serializedObject.FindProperty("m_DesamplingRate");
			_spReductionRate = serializedObject.FindProperty("m_ReductionRate");
			_spFilterMode = serializedObject.FindProperty("m_FilterMode");
			_spIterations = serializedObject.FindProperty("m_BlurIterations");
			_spKeepSizeToRootCanvas = serializedObject.FindProperty("m_FitToScreen");
			_spBlurMode = serializedObject.FindProperty("m_BlurMode");
			_spCaptureOnEnable = serializedObject.FindProperty("m_CaptureOnEnable");
			_spImmediateCapturing = serializedObject.FindProperty("m_ImmediateCapturing");


			_customAdvancedOption = (qualityMode == QualityMode.Custom);
		}

		/// <summary>
		/// Implement this function to make a custom inspector.
		/// </summary>
		public override void OnInspectorGUI()
		{
			var graphic = (target as UIEffectCapturedImage);
			serializedObject.Update();

			//================
			// Basic properties.
			//================
			EditorGUILayout.PropertyField(_spTexture);
			EditorGUILayout.PropertyField(_spColor);
			EditorGUILayout.PropertyField(_spRaycastTarget);

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

			EditorGUILayout.PropertyField(_spCaptureOnEnable);// CaptureOnEnable.
			EditorGUILayout.PropertyField(_spKeepSizeToRootCanvas);// Keep Graphic Size To RootCanvas.
			EditorGUILayout.PropertyField(_spImmediateCapturing);// Capture immediately.

			EditorGUI.BeginChangeCheck();
			QualityMode quality = qualityMode;
			quality = (QualityMode)EditorGUILayout.EnumPopup("Quality Mode", quality);
			if (EditorGUI.EndChangeCheck())
			{
				_customAdvancedOption = (quality == QualityMode.Custom);
				qualityMode = quality;
			}

			// When qualityMode is `Custom`, show advanced option.
			if (_customAdvancedOption)
			{
				if (_spBlurMode.intValue != 0)
				{
					EditorGUILayout.PropertyField(_spIterations);// Iterations.
				}
				DrawDesamplingRate(_spReductionRate);// Reduction rate.

				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Result Texture Setting", EditorStyles.boldLabel);

				EditorGUILayout.PropertyField(_spFilterMode);// Filter Mode.
				DrawDesamplingRate(_spDesamplingRate);// Desampling rate.
			}

			serializedObject.ApplyModifiedProperties();

			// Debug.
			using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
			{
				GUILayout.Label("Debug");

				if (GUILayout.Button("Capture", "ButtonLeft"))
				{
					graphic.Release();
					EditorApplication.delayCall += graphic.Capture;
				}

				EditorGUI.BeginDisabledGroup(!(target as UIEffectCapturedImage).capturedTexture);
				if (GUILayout.Button("Release", "ButtonRight"))
				{
					graphic.Release();
				}
				EditorGUI.EndDisabledGroup();
			}
		}

		//################################
		// Private Members.
		//################################
		const int Bits4 = (1 << 4) - 1;
		const int Bits2 = (1 << 2) - 1;
		bool _customAdvancedOption = false;
		SerializedProperty _spTexture;
		SerializedProperty _spColor;
		SerializedProperty _spRaycastTarget;
		SerializedProperty _spDesamplingRate;
		SerializedProperty _spReductionRate;
		SerializedProperty _spFilterMode;
		SerializedProperty _spBlurMode;
		SerializedProperty _spIterations;
		SerializedProperty _spKeepSizeToRootCanvas;
		SerializedProperty _spCaptureOnEnable;
		SerializedProperty _spImmediateCapturing;

		QualityMode qualityMode
		{
			get
			{
				if (_customAdvancedOption)
					return QualityMode.Custom;

				int qualityValue = (_spDesamplingRate.intValue << 0)
				                   + (_spReductionRate.intValue << 4)
				                   + (_spFilterMode.intValue << 8)
				                   + (_spIterations.intValue << 10);

				return System.Enum.IsDefined(typeof(QualityMode), qualityValue) ? (QualityMode)qualityValue : QualityMode.Custom;
			}
			set
			{
				if (value != QualityMode.Custom)
				{
					int qualityValue = (int)value;
					_spDesamplingRate.intValue = (qualityValue >> 0) & Bits4;
					_spReductionRate.intValue = (qualityValue >> 4) & Bits4;
					_spFilterMode.intValue = (qualityValue >> 8) & Bits2;
					_spIterations.intValue = (qualityValue >> 10) & Bits4;
				}
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
	}
}