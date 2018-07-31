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

		[SerializeField] bool m_AdvancedBlur = false;

		public enum BlurEx
		{
			None = 0,
			Ex = 1,
		}

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

//			UIVertex vt = default(UIVertex);
//			int count = vh.currentVertCount;
			float normalizedIndex = ptex.GetNormalizedIndex(this);
//
//			for (int i = 0; i < count; i++)
//			{
//				// Set prameters to vertex.
//				vh.PopulateUIVertex(ref vt, i);
//				vt.uv0 = new Vector2(
//					Packer.ToFloat(vt.uv0.x, vt.uv0.y),
//					normalizedIndex
//				);
//				vh.SetUIVertex(vt, i);
//			}



			if (m_AdvancedBlur)
			{
				vh.GetUIVertexStream(tempVerts);
				vh.Clear();
				var count = tempVerts.Count;

				// Bundle
				int bundleSize = targetGraphic is Text ? 6 : count;
				Rect posBounds = default(Rect);
				Rect uvBounds = default(Rect);
				Vector3 size = default(Vector3);
				Vector3 tPos = default(Vector3);
				Vector3 tUV = default(Vector3);
				float expand = (float)blurMode * 6 * 2;

				for (int i = 0; i < count; i += bundleSize)
				{
					// Quadバンドル単位での最大/最小値
					GetBounds(tempVerts, i, bundleSize, ref posBounds, ref uvBounds, true);

					// Pack uv mask.
					Vector2 uvMask = new Vector2(Packer.ToFloat(uvBounds.xMin, uvBounds.yMin), Packer.ToFloat(uvBounds.xMax, uvBounds.yMax));

					// Quad
					for (int j = 0; j < bundleSize; j += 6)
					{
						Vector3 cornerPos1 = tempVerts[i + j + 1].position;
						Vector3 cornerPos2 = tempVerts[i + j + 4].position;

						// 外周Quadかどうか.
						bool hasOuterEdge = (bundleSize == 6)
							|| !posBounds.Contains(cornerPos1)
							|| !posBounds.Contains(cornerPos2);
						if (hasOuterEdge)
						{
							Vector3 cornerUv1 = tempVerts[i + j + 1].uv0;
							Vector3 cornerUv2 = tempVerts[i + j + 4].uv0;

							Vector3 centerPos = (cornerPos1 + cornerPos2) / 2;
							Vector3 centerUV = (cornerUv1 + cornerUv2) / 2;
							size = (cornerPos1 - cornerPos2);

							size.x = 1 + expand / Mathf.Abs(size.x);
							size.y = 1 + expand / Mathf.Abs(size.y);
							size.z = 1 + expand / Mathf.Abs(size.z);

							tPos = centerPos - Vector3.Scale(size, centerPos);
							tUV = centerUV - Vector3.Scale(size, centerUV);
						}

						// Vertex
						for (int k = 0; k < 6; k++)
						{
							UIVertex vt = tempVerts[i + j + k];

							Vector3 pos = vt.position;
							Vector2 uv0 = vt.uv0;

							if (hasOuterEdge && (pos.x < posBounds.xMin || posBounds.xMax < pos.x))
							{
								pos.x = pos.x * size.x + tPos.x;
								uv0.x = uv0.x * size.x + tUV.x;
							}
							if (hasOuterEdge && (pos.y < posBounds.yMin || posBounds.yMax < pos.y))
							{
								pos.y = pos.y * size.y + tPos.y;
								uv0.y = uv0.y * size.y + tUV.y;
							}

//							vt.uv0 = new Vector2(Packer.ToFloat((uv0.x + 0.5f) / 2f, (uv0.y + 0.5f) / 2f), blur);
							vt.uv0 = new Vector2(Packer.ToFloat((uv0.x + 0.5f) / 2f, (uv0.y + 0.5f) / 2f), normalizedIndex);
							vt.position = pos;
							vt.uv1 = uvMask;

							tempVerts[i + j + k] = vt;
						}
					}
				}

				vh.AddUIVertexTriangleStream(tempVerts);
				tempVerts.Clear();
			}
			else
			{
				int count = vh.currentVertCount;
				UIVertex vt = default(UIVertex);
				for (int i = 0; i < count; i++)
				{
					vh.PopulateUIVertex(ref vt, i);
					Vector2 uv0 = vt.uv0;
					vt.uv0 = new Vector2(
						Packer.ToFloat((uv0.x + 0.5f) / 2f, (uv0.y + 0.5f) / 2f),
						normalizedIndex
					);
					vh.SetUIVertex(vt, i);
				}
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
			return MaterialResolver.GetOrGenerateMaterialVariant(Shader.Find(shaderName), m_ToneMode, m_ColorMode, m_BlurMode, m_AdvancedBlur ? BlurEx.Ex : BlurEx.None);
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
					if (m_ShadowStyle != ShadowStyle.None)
					{
						var shadow = gameObject.GetComponent<UIShadow>() ?? gameObject.AddComponent<UIShadow>();
						shadow.style = m_ShadowStyle;
						shadow.effectDistance = m_EffectDistance;
						shadow.effectColor = m_ShadowColor;
						shadow.useGraphicAlpha = m_UseGraphicAlpha;
						shadow.blur = m_ShadowBlur;
					}

					foreach(var s in m_AdditionalShadows)
					{
						if (s.style == ShadowStyle.None)
						{
							continue;
						}

						var shadow = gameObject.AddComponent<UIShadow>();
						shadow.style = s.style;
						shadow.effectDistance = s.effectDistance;
						shadow.effectColor = s.effectColor;
						shadow.useGraphicAlpha = s.useGraphicAlpha;
						shadow.blur = s.blur;
					}

					m_ShadowStyle = ShadowStyle.None;
					m_AdditionalShadows = null;

					if (m_ToneMode == ToneMode.None && m_ColorMode == ColorMode.Multiply && m_BlurMode == BlurMode.None)
					{
						DestroyImmediate(this, true);
					}
				}

				int tone = (int)m_ToneMode;
				const int Mono = 5;
				const int Cutoff = 6;
				const int Hue = 7;
				if (tone == Hue)
				{
					var go = gameObject;
					var hue = m_ToneLevel;
					DestroyImmediate(this, true);
					var hsv = go.GetComponent<UIHsvModifier>() ?? go.AddComponent<UIHsvModifier>();
					hsv.hue = hue;
					hsv.range = 1;
				}

				// Cutoff/Mono
				if (tone == Cutoff || tone == Mono)
				{
					var go = gameObject;
					var factor = m_ToneLevel;
					var transitionMode = tone == Cutoff
						? UITransitionEffect.EffectMode.Cutoff
						: UITransitionEffect.EffectMode.Fade;
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
		static void GetBounds(List<UIVertex> verts, int start, int count, ref Rect posBounds, ref Rect uvBounds, bool global)
		{
			Vector2 minPos = new Vector2(float.MaxValue, float.MaxValue);
			Vector2 maxPos = new Vector2(float.MinValue, float.MinValue);
			Vector2 minUV = new Vector2(float.MaxValue, float.MaxValue);
			Vector2 maxUV = new Vector2(float.MinValue, float.MinValue);
			for (int i = start; i < start + count; i++)
			{
				UIVertex vt = verts[i];

				Vector2 uv = vt.uv0;
				Vector3 pos = vt.position;

				// Left-Bottom
				if (minPos.x >= pos.x && minPos.y >= pos.y)
				{
					minPos = pos;
				}
				// Right-Top
				else if (maxPos.x <= pos.x && maxPos.y <= pos.y)
				{
					maxPos = pos;
				}

				// Left-Bottom
				if (minUV.x >= uv.x && minUV.y >= uv.y)
				{
					minUV = uv;
				}
				// Right-Top
				else if (maxUV.x <= uv.x && maxUV.y <= uv.y)
				{
					maxUV = uv;
				}
			}

			// Shrink coordinate for detect edge
			posBounds.Set(minPos.x + 0.001f, minPos.y + 0.001f, maxPos.x - minPos.x - 0.002f, maxPos.y - minPos.y - 0.002f);
			uvBounds.Set(minUV.x, minUV.y, maxUV.x - minUV.x, maxUV.y - minUV.y);
		}
	}
}
