using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.UI
{
	/// <summary>
	/// UIEffect.
	/// </summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(Graphic))]
	public class UIEffect : MonoBehaviour, IMeshModifier
	{
		//################################
		// Constant Members.
		//################################
		/// <summary>
		/// Precision shift. Each value has 6 bits.
		/// </summary>
		const int PACKER_SHIFT_1 = 6;
		const int PACKER_SHIFT_2 = PACKER_SHIFT_1 * 2;
		const int PACKER_SHIFT_3 = PACKER_SHIFT_1 * 3;

		/// <summary>
		/// Precision bits. Each value [0-1] has 64 steps.
		/// </summary>
		const int PACKER_PRECISION = (1 << PACKER_SHIFT_1) - 1;

		/// <summary>
		/// Effect shader name.
		/// </summary>
		const string ShaderName = "UI/Hidden/UIEffect";

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
			Detail,
		}


		//################################
		// Static Members.
		//################################
		static readonly List<UIVertex> s_Verts = new List<UIVertex>();

		static readonly List<UIEffect> s_UIEffects = new List<UIEffect>();

		static readonly Material[] s_SharedMaterials = new Material[128];

		//################################
		// Public or Serialize Members.
		//################################
		/// <summary>
		/// Tone effect level between 0(no effect) and 1(complete effect).
		/// </summary>
		public float toneLevel{ get { return m_ToneLevel; } set { m_ToneLevel = Mathf.Clamp(value, 0, 1); SetDirty(); } }

		[SerializeField][Range(0, 1)]protected float m_ToneLevel = 1;

		/// <summary>
		/// How far is the blurring from the graphic.
		/// </summary>
		public float blur { get { return m_Blur; } set { m_Blur = Mathf.Clamp(value, 0, 4); SetDirty(); } }

		[SerializeField][Range(0, 4)]protected float m_Blur = 0;

		/// <summary>
		/// How far is the blurring shadow from the graphic.
		/// </summary>
		public float shadowBlur { get { return m_ShadowBlur; } set { m_ShadowBlur = Mathf.Clamp(value, 0, 4); SetDirty(); } }

		[SerializeField][Range(0, 4)]protected float m_ShadowBlur = 0;

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

		[SerializeField] Color m_ShadowColor = Color.white;

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
		public Color color { get { return m_Color; } set { m_Color = value; SetDirty(); } }

		[SerializeField] Color m_Color = Color.white;

		/// <summary>
		/// Effect shader.
		/// </summary>
		public virtual Shader shader { get { if (m_Shader == null) m_Shader = Shader.Find(ShaderName); return m_Shader; } }

		[SerializeField] Shader m_Shader;

		/// <summary>
		/// Graphic affected by the UIEffect.
		/// </summary>
		public Graphic graphic { get { if (m_Graphic == null) m_Graphic = GetComponent<Graphic>(); return m_Graphic; } }

		Graphic m_Graphic;

		/// <summary>
		/// Main effect in the gameobject.
		/// * It is top most UIEffect component in inspector.
		/// </summary>
		public UIEffect mainEffect{ get; private set; }

		//################################
		// MonoBehaior Callbacks.
		//################################
		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected virtual void OnEnable()
		{
			SetDirty(this);
		}

		/// <summary>
		/// This function is called when the behaviour becomes disabled () or inactive.
		/// </summary>
		protected virtual void OnDisable()
		{
			// When all effect modes are disable, graphic uses default material.
			GetComponents<UIEffect>(s_UIEffects);
			if (s_UIEffects.All(effect => !effect.enabled))
			{
				graphic.material = null;
				graphic.SetVerticesDirty();
			}
			s_UIEffects.Clear();
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
			// Only main effect modify original vertices.
			//================================
			if (mainEffect == this)
			{
				// Calcurate bluring uv from blur factor and texture size.
				float blurringUv = m_Blur / graphic.mainTexture.width;

				// Pack some effect factors to 1 float.
				Vector2 factor = new Vector2(
					                 PackToFloat(m_ToneLevel, 0, blurringUv * 4),
					                 PackToFloat(m_Color.r, m_Color.g, m_Color.b, m_Color.a)
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
			if (ShadowMode.Shadow <= m_ShadowMode)
			{
				var inputVertCount = s_Verts.Count;
				var start = 0;
				var end = inputVertCount;

				// Calcurate bluring uv from blur factor and texture size.
				float blurringUv = m_ShadowBlur / graphic.mainTexture.width;

				// Pack some effect factors to 1 float.
				Vector2 factor = new Vector2(
					                 PackToFloat(m_ToneLevel, 0, blurringUv * 4),
					                 PackToFloat(m_ShadowColor.r, m_ShadowColor.g, m_ShadowColor.b, 1)
				                 );

				ApplyShadowZeroAlloc(s_Verts, ref start, ref end, m_EffectDistance.x, m_EffectDistance.y, factor);


				// Append shadow for Outline.
				if (ShadowMode.Outline <= m_ShadowMode)
				{
					ApplyShadowZeroAlloc(s_Verts, ref start, ref end, m_EffectDistance.x, -m_EffectDistance.y, factor);
					ApplyShadowZeroAlloc(s_Verts, ref start, ref end, -m_EffectDistance.x, m_EffectDistance.y, factor);
					ApplyShadowZeroAlloc(s_Verts, ref start, ref end, -m_EffectDistance.x, -m_EffectDistance.y, factor);
				}

				// Append shadow for Outline8.
				if (ShadowMode.Outline8 <= m_ShadowMode)
				{
					ApplyShadowZeroAlloc(s_Verts, ref start, ref end, -m_EffectDistance.x, 0, factor);
					ApplyShadowZeroAlloc(s_Verts, ref start, ref end, 0, -m_EffectDistance.y, factor);
					ApplyShadowZeroAlloc(s_Verts, ref start, ref end, m_EffectDistance.x, 0, factor);
					ApplyShadowZeroAlloc(s_Verts, ref start, ref end, 0, m_EffectDistance.y, factor);
				}
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
			SetDirty(this);
		}
#endif

		//################################
		// Internal Method.
		//################################
		/// <summary>
		/// Append shadow vertices.
		/// * It is similar to Shadow component implementation.
		/// </summary>
		protected void ApplyShadowZeroAlloc(List<UIVertex> verts, ref int start, ref int end, float x, float y, Vector2 factor)
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

				Color vertColor = m_ShadowColor;
				vertColor.a = m_UseGraphicAlpha ? m_ShadowColor.a * vt.color.a / 255 : m_ShadowColor.a;
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
			//
			if (!mainEffect)
			{
				mainEffect = GetComponent<UIEffect>();
			}

			// Only main effect update material.
			if (mainEffect != this)
			{
				m_ToneMode = mainEffect.m_ToneMode;
				m_ColorMode = mainEffect.m_ColorMode;
				m_BlurMode = mainEffect.m_BlurMode;
				return;
			}

			// Update material if needed.
			if (isMaterialDirty && mainEffect == this)
			{
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
				const int BLUR_DETAIL = (int)BlurMode.Detail;

				// Calculate shader keyword identifier from effect modes.
				int identifier = ((int)m_ToneMode << TONE_SHIFT) | ((int)m_ColorMode << COLOR_SHIFT) | ((int)m_BlurMode << BLUR_SHIFT);

				// When all effect modes are disable(None), graphic uses default material.
				if (identifier == 0)
				{
					graphic.material = null;
					graphic.SetVerticesDirty();
					return;
				}

				// Generate and cache new material by given identifier.
				if (!s_SharedMaterials[identifier])
				{
					if (!s_SharedMaterials[0])
					{
						s_SharedMaterials[0] = new Material(shader);
					}
					Material mat = new Material(s_SharedMaterials[0]);

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
						: BLUR_FAST == (blurBits & BLUR_FAST) ? "UI_BLUR_FAST"
						: "UI_BLUR_OFF" 
					);

					mat.name += identifier.ToString();
					s_SharedMaterials[identifier] = mat;
				}

				graphic.material = s_SharedMaterials[identifier];
			}
			graphic.SetVerticesDirty();
		}

		/// <summary>
		/// Pack 4 low-precision [0-1] floats values to a float.
		/// </summary>
		static float PackToFloat(float x, float y, float z, float w)
		{   
			return (Mathf.FloorToInt(w * PACKER_PRECISION) << PACKER_SHIFT_3)
			+ (Mathf.FloorToInt(z * PACKER_PRECISION) << PACKER_SHIFT_2)
			+ (Mathf.FloorToInt(y * PACKER_PRECISION) << PACKER_SHIFT_1)
			+ Mathf.FloorToInt(x * PACKER_PRECISION);
		}

		/// <summary>
		/// Pack 3 low-precision [0-1] floats values to a float.
		/// The z value is a little high precision(12 bits).
		/// </summary>
		static float PackToFloat(float x, float y, float z)
		{   
			const int PRECISION_BITS_HIGH = (1 << PACKER_SHIFT_2) - 1;

			return (Mathf.FloorToInt(z * PRECISION_BITS_HIGH) << PACKER_SHIFT_2)
			+ (Mathf.FloorToInt(y * PACKER_PRECISION) << PACKER_SHIFT_1)
			+ Mathf.FloorToInt(x * PACKER_PRECISION);
		}
	}
}
