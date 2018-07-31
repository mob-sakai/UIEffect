using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
#endif

namespace Coffee.UIExtensions
{
	/// <summary>
	/// UIEffect.
	/// </summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(Graphic))]
	[DisallowMultipleComponent]
	public class UIEffect : UIEffectBase
	{
		//################################
		// Constant or Static Members.
		//################################
		public const string shaderName = "UI/Hidden/UI-Effect";

		static readonly ParameterTexture _ptex = new ParameterTexture(4, 1024, "_ParamTex");


//		/// <summary>
//		/// Tone effect mode.
//		/// </summary>
//		public enum ToneMode
//		{
//			None = 0,
//			Grayscale,
//			Sepia,
//			Nega,
//			Pixel,
//			Mono,
//			Cutoff,
//			Hue,
//		}
//
//		/// <summary>
//		/// Color effect mode.
//		/// </summary>
//		public enum ColorMode
//		{
//			None = 0,
//			Set,
//			Add,
//			Sub,
//		}
//
//		/// <summary>
//		/// Shadow effect style.
//		/// </summary>
//		public enum ShadowStyle
//		{
//			None = 0,
//			Shadow,
//			Outline,
//			Outline8,
//			Shadow3,
//		}
//
//		/// <summary>
//		/// Blur effect mode.
//		/// </summary>
//		public enum BlurMode
//		{
//			None = 0,
//			Fast,
//			Medium,
//			Detail,
//		}
//

		//################################
		// Serialize Members.
		//################################
		[SerializeField][Range(0, 1)] float m_ToneLevel = 1;
		[SerializeField][Range(0, 1)] float m_ColorFactor = 1;
		[SerializeField][Range(0, 1)] float m_Blur = 1;
		[Obsolete][HideInInspector]
		[SerializeField][Range(0, 1)] float m_ShadowBlur = 1;
		[Obsolete][HideInInspector]
		[SerializeField] ShadowStyle m_ShadowStyle;
		[SerializeField] ToneMode m_ToneMode;
		[SerializeField] ColorMode m_ColorMode;
		[SerializeField] BlurMode m_BlurMode;
		[Obsolete][HideInInspector]
		[SerializeField] Color m_ShadowColor = Color.black;
		[Obsolete][HideInInspector]
		[SerializeField] Vector2 m_EffectDistance = new Vector2(1f, -1f);
		[Obsolete][HideInInspector]
		[SerializeField] bool m_UseGraphicAlpha = true;
		[SerializeField] Color m_EffectColor = Color.white;
		[Obsolete][HideInInspector]
		[SerializeField] List<UIShadow.AdditionalShadow> m_AdditionalShadows = new List<UIShadow.AdditionalShadow>();




		//################################
		// Public Members.
		//################################
		/// <summary>
		/// Graphic affected by the UIEffect.
		/// </summary>
		[System.Obsolete ("Use targetGraphic instead (UnityUpgradable) -> targetGraphic")]
		new public Graphic graphic { get { return base.graphic; } }

		/// <summary>
		/// Tone effect level between 0(no effect) and 1(complete effect).
		/// </summary>
		public float toneLevel{ get { return m_ToneLevel; } set { m_ToneLevel = Mathf.Clamp(value, 0, 1); SetDirty(); } }

		/// <summary>
		/// Color effect factor between 0(no effect) and 1(complete effect).
		/// </summary>
		public float colorFactor { get { return m_ColorFactor; } set { m_ColorFactor = Mathf.Clamp(value, 0, 1); SetDirty(); } }

		/// <summary>
		/// How far is the blurring from the graphic.
		/// </summary>
		public float blur { get { return m_Blur; } set { m_Blur = Mathf.Clamp(value, 0, 1); SetDirty(); } }

		/// <summary>
		/// Tone effect mode.
		/// </summary>
		public ToneMode toneMode { get { return m_ToneMode; } }

		/// <summary>
		/// Color effect mode.
		/// </summary>
		public ColorMode colorMode { get { return m_ColorMode; } }

		/// <summary>
		/// Blur effect mode.
		/// </summary>
		public BlurMode blurMode { get { return m_BlurMode; } }

		/// <summary>
		/// Color for the color effect.
		/// </summary>
		public Color effectColor { get { return m_EffectColor; } set { m_EffectColor = value; SetDirty(); } }

		/// <summary>
		/// Gets the parameter texture.
		/// </summary>
		public override ParameterTexture ptex { get { return _ptex; } }

		/// <summary>
		/// Modifies the mesh.
		/// </summary>
		public override void ModifyMesh(VertexHelper vh)
		{
			if (!isActiveAndEnabled || (m_ToneMode == ToneMode.None && m_ColorMode == ColorMode.Multiply && m_BlurMode == BlurMode.None))
			{
				return;
			}

			UIVertex vt = default(UIVertex);
			int count = vh.currentVertCount;
			float normalizedIndex = ptex.GetNormalizedIndex(this);

			for (int i = 0; i < count; i++)
			{
				// Set prameters to vertex.
				vh.PopulateUIVertex(ref vt, i);
				vt.uv0 = new Vector2(
					Packer.ToFloat(vt.uv0.x, vt.uv0.y),
					normalizedIndex
				);
				vh.SetUIVertex(vt, i);
			}
		}

		protected override void SetDirty()
		{
			ptex.RegisterMaterial(m_EffectMaterial);
			ptex.SetData(this, 0, m_ToneLevel);	// param.x : effect factor
			ptex.SetData(this, 1, m_ColorFactor);	// param.y : color factor
			ptex.SetData(this, 2, m_Blur);	// param.z : blur factor
		}

//#if UNITY_EDITOR
//
//		protected override void OnValidate ()
//		{
//			base.OnValidate ();
//			EditorApplication.delayCall += () => UpdateMaterial(false);
//		}
//
//		public override void OnAfterDeserialize()
//		{
//			if (!m_CustomEffect)
//			{
//				EditorApplication.delayCall += () => UpdateMaterial (true);
//			}
//		}
//
//		void UpdateMaterial(bool onlyEditMode)
//		{
//			if(!this || onlyEditMode && Application.isPlaying)
//			{
//				return;
//			}
//
//			var mat = (0 == toneMode) && (0 == colorMode) && (0 == blurMode)
//				? null
//				: GetOrGenerateMaterialVariant(Shader.Find(shaderName), toneMode, colorMode, blurMode);
//
//			if (m_EffectMaterial != mat || targetGraphic.material != mat)
//			{
//				targetGraphic.material = m_EffectMaterial = mat;
//				EditorUtility.SetDirty(this);
//				EditorUtility.SetDirty(targetGraphic);
//			}
//		}
//		
//
//
//		public static Material GetOrGenerateMaterialVariant(Shader shader, ToneMode tone, ColorMode color, BlurMode blur)
//		{
//			if (!shader)
//				return null;
//
//			Material mat = GetMaterial(shader, tone, color, blur);
//
//			if (!mat)
//			{
//				Debug.Log("Generate material : " + GetVariantName(shader, tone, color, blur));
//				mat = new Material(shader);
//
//				if (0 < tone)
//					mat.EnableKeyword("" + tone.ToString().ToUpper());
//				if (0 < color)
//					mat.EnableKeyword("UI_COLOR_" + color.ToString().ToUpper());
//				if (0 < blur)
//					mat.EnableKeyword("UI_BLUR_" + blur.ToString().ToUpper());
//
//				mat.name = GetVariantName(shader, tone, color, blur);
//				mat.hideFlags |= HideFlags.NotEditable;
//
//#if UIEFFECT_SEPARATE
//				bool isMainAsset = true;
//				string dir = Path.GetDirectoryName(GetDefaultMaterialPath (shader));
//				string materialPath = Path.Combine(Path.Combine(dir, "Separated"), mat.name + ".mat");
//#else
//				bool isMainAsset = (0 == tone) && (0 == color) && (0 == blur);
//				string materialPath = GetDefaultMaterialPath (shader);
//#endif
//				if (isMainAsset)
//				{
//					Directory.CreateDirectory(Path.GetDirectoryName(materialPath));
//					AssetDatabase.CreateAsset(mat, materialPath);
//					AssetDatabase.SaveAssets();
//				}
//				else
//				{
//					mat.hideFlags |= HideFlags.HideInHierarchy;
//					AssetDatabase.AddObjectToAsset(mat, materialPath);
//				}
//			}
//			return mat;
//		}
//
//		public static Material GetMaterial(Shader shader, ToneMode tone, ColorMode color, BlurMode blur)
//		{
//			string variantName = GetVariantName(shader, tone, color, blur);
//			return AssetDatabase.FindAssets("t:Material " + Path.GetFileName(shader.name))
//				.Select(x => AssetDatabase.GUIDToAssetPath(x))
//				.SelectMany(x => AssetDatabase.LoadAllAssetsAtPath(x))
//				.OfType<Material>()
//				.FirstOrDefault(x => x.name == variantName);
//		}
//
//		public static string GetDefaultMaterialPath(Shader shader)
//		{
//			var name = Path.GetFileName (shader.name);
//			return AssetDatabase.FindAssets("t:Material " + name)
//				.Select(x => AssetDatabase.GUIDToAssetPath(x))
//				.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == name)
//				?? ("Assets/Coffee/UIExtensions/UIEffect/Materials/" + name + ".mat");
//		}
//
//		public static string GetVariantName(Shader shader, ToneMode tone, ColorMode color, BlurMode blur)
//		{
//			return
//#if UIEFFECT_SEPARATE
//				"[Separated] " + Path.GetFileName(shader.name)
//#else
//				Path.GetFileName(shader.name)
//#endif
//				+ (0 < tone ? "-" + tone : "")
//				+ (0 < color ? "-" + color : "")
//				+ (0 < blur ? "-" + blur : "");
//		}
//#endif

#if UNITY_EDITOR
		/// <summary>
		/// Gets the material.
		/// </summary>
		/// <returns>The material.</returns>
		protected override Material GetMaterial ()
		{
			if (m_ToneMode == ToneMode.None && m_ColorMode == ColorMode.Multiply && m_BlurMode == BlurMode.None)
			{
				return null;
			}
			return MaterialResolver.GetOrGenerateMaterialVariant(Shader.Find(shaderName), m_ToneMode, m_ColorMode, m_BlurMode);
		}

		#pragma warning disable 0612
		protected override void UpgradeIfNeeded()
		{
			// Upgrade for v3.0.0
			if (IsShouldUpgrade(300))
			{
				if (m_ColorMode != ColorMode.Multiply)
				{
					Color col = targetGraphic.color;
					col.r = m_EffectColor.r;
					col.g = m_EffectColor.g;
					col.b = m_EffectColor.b;
					targetGraphic.color = col;
					m_ColorFactor = m_EffectColor.a;
				}

				if (m_ShadowStyle != ShadowStyle.None || m_AdditionalShadows.Any(x=>x.style != ShadowStyle.None))
				{
					var shadow = gameObject.GetComponent<UIShadow>() ?? gameObject.AddComponent<UIShadow>();
					shadow.style = m_ShadowStyle;
					shadow.effectDistance = m_EffectDistance;
					shadow.effectColor = m_ShadowColor;
					shadow.useGraphicAlpha = m_UseGraphicAlpha;
					shadow.blur = m_ShadowBlur;
					shadow.additionalShadows.AddRange(m_AdditionalShadows);

					m_ShadowStyle = ShadowStyle.None;
					m_AdditionalShadows = null;

					if (m_ToneMode == ToneMode.None && m_ColorMode == ColorMode.Multiply && m_BlurMode == BlurMode.None)
					{
						DestroyImmediate(this, true);
					}
				}

				if (m_ToneMode == ToneMode.Hue)
				{
					var go = gameObject;
					var hue = m_ToneLevel;
					DestroyImmediate(this, true);
					var hsv = go.GetComponent<UIHsvModifier>() ?? go.AddComponent<UIHsvModifier>();
					hsv.hue = hue;
					hsv.range = 1;
				}

				if (m_ToneMode == ToneMode.Cutoff || m_ToneMode == ToneMode.Mono)
				{
					var go = gameObject;
					var factor = m_ToneLevel;
					var transitionMode = m_ToneMode == ToneMode.Cutoff
						? UITransitionEffect.EffectMode.Cutoff
						: UITransitionEffect.EffectMode.Mono;
					DestroyImmediate(this, true);
					var trans = go.GetComponent<UITransitionEffect>() ?? go.AddComponent<UITransitionEffect>();
					trans.effectFactor = factor;

					var sp = new SerializedObject(trans).FindProperty("m_EffectMode");
					sp.intValue = (int)transitionMode;
					sp.serializedObject.ApplyModifiedProperties();
				}
			}
		}
		#pragma warning restore 0612
#endif

		//################################
		// Private Members.
		//################################
	}
}
