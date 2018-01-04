using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
	/// <summary>
	/// UIEffect.
	/// </summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(Graphic))]
	[DisallowMultipleComponent]
	public class UIEffect : MonoBehaviour, IMeshModifier
	{
		/// <summary>
		/// Additional shadow.
		/// </summary>
		[System.Serializable]
		public class AdditionalShadow
		{
			/// <summary>
			/// How far is the blurring shadow from the graphic.
			/// </summary>
			[Range(0, 2)]
			public float shadowBlur = 0.25f;

			/// <summary>
			/// Shadow effect mode.
			/// </summary>
			public ShadowMode shadowMode = ShadowMode.Shadow;

			/// <summary>
			/// Color for the shadow effect.
			/// </summary>
			public Color shadowColor = Color.black;

			/// <summary>
			/// How far is the shadow from the graphic.
			/// </summary>
			public Vector2 effectDistance = new Vector2(1f, -1f);

			/// <summary>
			/// Should the shadow inherit the alpha from the graphic?
			/// </summary>
			public bool useGraphicAlpha = true;
		}

		//################################
		// Constant Members.
		//################################
		/// <summary>
		/// Tone effect mode.
		/// </summary>
		public enum ToneMode
		{
			None = 0,
			Grayscale,
			Sepia,
			Nega,
			Pixel,
			Mono,
			Cutoff,
		}

		/// <summary>
		/// Color effect mode.
		/// </summary>
		public enum ColorMode
		{
			None = 0,
			Set,
			Add,
			Sub,
		}

		/// <summary>
		/// Shadow effect mode.
		/// </summary>
		public enum ShadowMode
		{
			None = 0,
			Shadow,
			Outline,
			Outline8,
		}

		/// <summary>
		/// Blur effect mode.
		/// </summary>
		public enum BlurMode
		{
			None = 0,
			Fast,
			Medium,
			Detail,
		}


		//################################
		// Static Members.
		//################################
		static readonly List<UIVertex> s_Verts = new List<UIVertex>();
		static readonly Dictionary<Shader, Material[]> s_SharedMaterials = new Dictionary<Shader, Material[]>();

		//################################
		// Public or Serialize Members.
		//################################
		/// <summary>
		/// Tone effect level between 0(no effect) and 1(complete effect).
		/// </summary>
		public float toneLevel{ get { return m_ToneLevel; } set { m_ToneLevel = Mathf.Clamp(value, 0, 1); SetDirty(); } }

		[SerializeField][Range(0, 1)] float m_ToneLevel = 1;

		/// <summary>
		/// How far is the blurring from the graphic.
		/// </summary>
		public float blur { get { return m_Blur; } set { m_Blur = Mathf.Clamp(value, 0, 4); SetDirty(); } }

		[SerializeField][Range(0, 4)] float m_Blur = 0.25f;

		/// <summary>
		/// How far is the blurring shadow from the graphic.
		/// </summary>
		public float shadowBlur { get { return m_ShadowBlur; } set { m_ShadowBlur = Mathf.Clamp(value, 0, 4); SetDirty(); } }

		[SerializeField][Range(0, 4)] float m_ShadowBlur = 0.25f;

		/// <summary>
		/// Shadow effect mode.
		/// </summary>
		public ShadowMode shadowMode { get { return m_ShadowMode; } set { m_ShadowMode = value; SetDirty(); } }

		[SerializeField] ShadowMode m_ShadowMode;

		/// <summary>
		/// Tone effect mode.
		/// </summary>
		public ToneMode toneMode { get { return m_ToneMode; } set { m_ToneMode = value; SetDirty(true); } }

		[SerializeField] ToneMode m_ToneMode;

		/// <summary>
		/// Color effect mode.
		/// </summary>
		public ColorMode colorMode { get { return m_ColorMode; } set { m_ColorMode = value; SetDirty(true); } }

		[SerializeField] ColorMode m_ColorMode;

		/// <summary>
		/// Blur effect mode.
		/// </summary>
		public BlurMode blurMode { get { return m_BlurMode; } set { m_BlurMode = value; SetDirty(true); } }

		[SerializeField] BlurMode m_BlurMode;

		/// <summary>
		/// Color for the shadow effect.
		/// </summary>
		public Color shadowColor { get { return m_ShadowColor; } set { m_ShadowColor = value; SetDirty(); } }

		[SerializeField] Color m_ShadowColor = Color.black;

		/// <summary>
		/// How far is the shadow from the graphic.
		/// </summary>
		public Vector2 effectDistance { get { return m_EffectDistance; } set { m_EffectDistance = value; SetDirty(); } }

		[SerializeField] Vector2 m_EffectDistance = new Vector2(1f, -1f);

		/// <summary>
		/// Should the shadow inherit the alpha from the graphic?
		/// </summary>
		public bool useGraphicAlpha { get { return m_UseGraphicAlpha; } set { m_UseGraphicAlpha = value; SetDirty(); } }

		[SerializeField] bool m_UseGraphicAlpha = true;

		/// <summary>
		/// Color for the color effect.
		/// </summary>
		[Obsolete("UIEffect.color is obsolete, use UIEffect.effectColor instead. (UnityUpgradable) -> effectColor")]
		public Color color { get { return m_EffectColor; } set { m_EffectColor = value; SetDirty(); } }

		/// <summary>
		/// Color for the color effect.
		/// </summary>
		public Color effectColor { get { return m_EffectColor; } set { m_EffectColor = value; SetDirty(); } }

		[SerializeField][FormerlySerializedAs("m_Color")] Color m_EffectColor = Color.white;

		/// <summary>
		/// Effect shader.
		/// </summary>
		public virtual Shader shader { get { if (m_Shader == null) m_Shader = Shader.Find("UI/Hidden/UIEffect"); return m_Shader; } }

		[SerializeField] Shader m_Shader;

		/// <summary>
		/// Graphic affected by the UIEffect.
		/// </summary>
		public Graphic graphic { get { if (m_Graphic == null) m_Graphic = GetComponent<Graphic>(); return m_Graphic; } }

		Graphic m_Graphic;

		/// <summary>
		/// Additional Shadows.
		/// </summary>
		public List<AdditionalShadow> additionalShadows { get { return m_AdditionalShadows; } }

		[SerializeField] List<AdditionalShadow> m_AdditionalShadows = new List<AdditionalShadow>();


		//################################
		// MonoBehaior Callbacks.
		//################################
		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected virtual void OnEnable()
		{
			SetDirty(true);
		}

		/// <summary>
		/// This function is called when the behaviour becomes disabled () or inactive.
		/// </summary>
		protected virtual void OnDisable()
		{
			graphic.material = null;
			SetDirty();
		}

		/// <summary>
		/// Callback for when properties have been changed by animation.
		/// </summary>
		protected virtual void OnDidApplyAnimationProperties()
		{
			SetDirty(true);
		}

		/// <summary>
		/// Modifies the mesh.
		/// </summary>
		public virtual void ModifyMesh(VertexHelper vh)
		{
			if (!isActiveAndEnabled)
			{
				return;
			}

			UIVertex vt;
			vh.GetUIVertexStream(s_Verts);

			//================================
			// Effect modify original vertices.
			//================================
			{
				// Pack some effect factors to 1 float.
				Vector2 factor = new Vector2(
									 PackToFloat(toneLevel, 0, blur / graphic.mainTexture.width * 4),
									 PackToFloat(effectColor.r, effectColor.g, effectColor.b, effectColor.a)
								 );

				for (int i = 0; i < s_Verts.Count; i++)
				{
					vt = s_Verts[i];

					// Set UIEffect prameters to vertex.
					vt.uv1 = factor;
					s_Verts[i] = vt;
				}
			}

			//================================
			// Append shadow vertices.
			//================================
			{
				var inputVertCount = s_Verts.Count;
				var start = 0;
				var end = inputVertCount;

				// Additional Shadows.
				for (int i = additionalShadows.Count - 1; 0 <= i; i--)
				{
					AdditionalShadow shadow = additionalShadows[i];
					ApplyShadow(s_Verts, ref start, ref end, shadow.shadowMode, toneLevel, shadow.shadowBlur / graphic.mainTexture.width * 4, shadow.effectDistance, shadow.shadowColor, shadow.useGraphicAlpha);
				}

				// Shadow.
				ApplyShadow(s_Verts, ref start, ref end, shadowMode, toneLevel, shadowBlur / graphic.mainTexture.width * 4, effectDistance, shadowColor, useGraphicAlpha);
			}

			vh.Clear();
			vh.AddUIVertexTriangleStream(s_Verts);

			s_Verts.Clear();
		}

		/// <summary>
		/// Modifies the mesh.
		/// </summary>
		[System.Obsolete("use IMeshModifier.ModifyMesh (VertexHelper verts) instead")]
		public void ModifyMesh(Mesh mesh)
		{
		}

#if UNITY_EDITOR
		/// <summary>
		/// This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
		/// </summary>
		protected virtual void OnValidate()
		{
			SetDirty(true);
		}
#endif

		//################################
		// Internal Method.
		//################################
		/// <summary>
		/// Append shadow vertices.
		/// * It is similar to Shadow component implementation.
		/// </summary>
		static void ApplyShadow(List<UIVertex> verts, ref int start, ref int end, ShadowMode mode, float toneLevel, float blur, Vector2 effectDistance, Color color, bool useGraphicAlpha)
		{
			if (ShadowMode.None == mode)
				return;

			var factor = new Vector2(
							 PackToFloat(toneLevel, 0, blur),
							 PackToFloat(color.r, color.g, color.b, 1)
						 );

			// Append Shadow.
			ApplyShadowZeroAlloc(s_Verts, ref start, ref end, effectDistance.x, effectDistance.y, factor, color, useGraphicAlpha);

			// Append Outline.
			if (ShadowMode.Outline <= mode)
			{
				ApplyShadowZeroAlloc(s_Verts, ref start, ref end, effectDistance.x, -effectDistance.y, factor, color, useGraphicAlpha);
				ApplyShadowZeroAlloc(s_Verts, ref start, ref end, -effectDistance.x, effectDistance.y, factor, color, useGraphicAlpha);
				ApplyShadowZeroAlloc(s_Verts, ref start, ref end, -effectDistance.x, -effectDistance.y, factor, color, useGraphicAlpha);
			}

			// Append Outline8.
			if (ShadowMode.Outline8 <= mode)
			{
				ApplyShadowZeroAlloc(s_Verts, ref start, ref end, -effectDistance.x, 0, factor, color, useGraphicAlpha);
				ApplyShadowZeroAlloc(s_Verts, ref start, ref end, 0, -effectDistance.y, factor, color, useGraphicAlpha);
				ApplyShadowZeroAlloc(s_Verts, ref start, ref end, effectDistance.x, 0, factor, color, useGraphicAlpha);
				ApplyShadowZeroAlloc(s_Verts, ref start, ref end, 0, effectDistance.y, factor, color, useGraphicAlpha);
			}
		}

		/// <summary>
		/// Append shadow vertices.
		/// * It is similar to Shadow component implementation.
		/// </summary>
		static void ApplyShadowZeroAlloc(List<UIVertex> verts, ref int start, ref int end, float x, float y, Vector2 factor, Color color, bool useGraphicAlpha)
		{
			// Check list capacity.
			var neededCapacity = verts.Count + end - start;
			if (verts.Capacity < neededCapacity)
				verts.Capacity = neededCapacity;

			// Append shadow vertices to the front of list.
			// * The original vertex is pushed backward.
			UIVertex vt;
			for (int i = start; i < end; ++i)
			{
				vt = verts[i];
				verts.Add(vt);

				Vector3 v = vt.position;
				vt.position.Set(v.x + x, v.y + y, v.z);

				Color vertColor = color;
				vertColor.a = useGraphicAlpha ? color.a * vt.color.a / 255 : color.a;
				vt.color = vertColor;

				// Set UIEffect prameters to vertex.
				vt.uv1 = factor;
				verts[i] = vt;
			}

			// Update next shadow offset.
			start = end;
			end = verts.Count;
		}

		/// <summary>
		/// Mark the UIEffect as dirty.
		/// </summary>
		/// <param name="isMaterialDirty">If set to true material dirty.</param>
		void SetDirty(bool isMaterialDirty = false)
		{

			// Update material if needed.
			if (isMaterialDirty)
			{
				graphic.material = GetSharedMaterial(shader, toneMode, colorMode, blurMode);
			}
			graphic.SetVerticesDirty();
		}


		/// <summary>
		/// Get shared material for identifier.
		/// </summary>
		/// <param name="isMaterialDirty">If set to true material dirty.</param>
		public static Material GetSharedMaterial(Shader shader, ToneMode toneMode, ColorMode colorMode, BlurMode blurMode, bool nullDefault = true)
		{
			// Update material if needed.
			const int TONE_SHIFT = 0;
			const int TONE_GRAYSCALE = (int)ToneMode.Grayscale;
			const int TONE_SEPIA = (int)ToneMode.Sepia;
			const int TONE_NEGA = (int)ToneMode.Nega;
			const int TONE_PIXEL = (int)ToneMode.Pixel;
			const int TONE_MONO = (int)ToneMode.Mono;
			const int TONE_CUTOFF = (int)ToneMode.Cutoff;

			const int COLOR_SHIFT = 3;
			const int COLOR_SET = (int)ColorMode.Set;
			const int COLOR_ADD = (int)ColorMode.Add;
			const int COLOR_SUB = (int)ColorMode.Sub;

			const int BLUR_SHIFT = 5;
			const int BLUR_FAST = (int)BlurMode.Fast;
			const int BLUR_MEDIUM = (int)BlurMode.Medium;
			const int BLUR_DETAIL = (int)BlurMode.Detail;

			// Calculate shader keyword identifier from effect modes.
			int identifier = ((int)toneMode << TONE_SHIFT)
				| ((int)colorMode << COLOR_SHIFT)
				| ((int)blurMode << BLUR_SHIFT);

			// When all effect modes are disable(None), graphic uses default material.
			if (nullDefault && identifier == 0)
			{
				return null;
			}

			Material[] materials;

			if (!s_SharedMaterials.TryGetValue(shader, out materials))
			{
				materials = new Material[128];
				s_SharedMaterials.Add(shader, materials);
			}

			// Generate and cache new material by given identifier.
			if (!materials[identifier])
			{
				if (!materials[0])
				{
					materials[0] = new Material(shader);
					materials[0].name += identifier.ToString();
				}
				if (identifier == 0)
				{
					return materials[0];
				}

				Material mat = new Material(materials[0]);

				// Bits for tone effect.
				int toneBits = identifier >> TONE_SHIFT;
				mat.EnableKeyword(
					TONE_CUTOFF == (toneBits & TONE_CUTOFF) ? "UI_TONE_CUTOFF"
					: TONE_MONO == (toneBits & TONE_MONO) ? "UI_TONE_MONO"
					: TONE_PIXEL == (toneBits & TONE_PIXEL) ? "UI_TONE_PIXEL"
					: TONE_NEGA == (toneBits & TONE_NEGA) ? "UI_TONE_NEGA"
					: TONE_SEPIA == (toneBits & TONE_SEPIA) ? "UI_TONE_SEPIA"
					: TONE_GRAYSCALE == (toneBits & TONE_GRAYSCALE) ? "UI_TONE_GRAYSCALE"
					: "UI_TONE_OFF"
				);

				// Bits for color effect.
				int colorBits = identifier >> COLOR_SHIFT;
				mat.EnableKeyword(
					COLOR_SUB == (colorBits & COLOR_SUB) ? "UI_COLOR_SUB"
					: COLOR_ADD == (colorBits & COLOR_ADD) ? "UI_COLOR_ADD"
					: COLOR_SET == (colorBits & COLOR_SET) ? "UI_COLOR_SET"
					: "UI_COLOR_OFF"
				);

				// Bits for blur effect.
				int blurBits = identifier >> BLUR_SHIFT;
				mat.EnableKeyword(
					BLUR_DETAIL == (blurBits & BLUR_DETAIL) ? "UI_BLUR_DETAIL"
					: BLUR_MEDIUM == (blurBits & BLUR_MEDIUM) ? "UI_BLUR_MEDIUM"
					: BLUR_FAST == (blurBits & BLUR_FAST) ? "UI_BLUR_FAST"
					: "UI_BLUR_OFF"
				);

				mat.name += identifier.ToString();
				materials[identifier] = mat;
			}
			return materials[identifier];
		}

		/// <summary>
		/// Pack 4 low-precision [0-1] floats values to a float.
		/// Each value [0-1] has 64 steps(6 bits).
		/// </summary>
		static float PackToFloat(float x, float y, float z, float w)
		{
			const int PRECISION = (1 << 6) - 1;
			return (Mathf.FloorToInt(w * PRECISION) << 18)
			+ (Mathf.FloorToInt(z * PRECISION) << 12)
			+ (Mathf.FloorToInt(y * PRECISION) << 6)
			+ Mathf.FloorToInt(x * PRECISION);
		}

		/// <summary>
		/// Pack 3 low-precision [0-1] floats values to a float.
		/// The x and y values [0-1] have 64 steps(6 bits).
		/// The z value [0-1] is a little high precision(12 bits).
		/// </summary>
		static float PackToFloat(float x, float y, float z)
		{
			const int PRECISION = (1 << 6) - 1;
			const int PRECISION_HIGH = (1 << 12) - 1;

			return (Mathf.FloorToInt(z * PRECISION_HIGH) << 12)
			+ (Mathf.FloorToInt(y * PRECISION) << 6)
			+ Mathf.FloorToInt(x * PRECISION);
		}
	}
}
