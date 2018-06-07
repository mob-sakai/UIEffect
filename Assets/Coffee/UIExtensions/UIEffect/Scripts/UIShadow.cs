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
	public class UIShadow : Shadow
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
			[Range(0, 1)] public float blur = 0.25f;

			/// <summary>
			/// Shadow effect mode.
			/// </summary>
			public ShadowStyle style = ShadowStyle.Shadow;

			/// <summary>
			/// Color for the shadow effect.
			/// </summary>
			public Color effectColor = Color.black;

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
		// Serialize Members.
		//################################
		[SerializeField][Range(0, 1)] float m_Blur = 0.25f;
		[SerializeField] ShadowStyle m_Style = ShadowStyle.Shadow;
		[SerializeField] List<AdditionalShadow> m_AdditionalShadows = new List<AdditionalShadow>();


		//################################
		// Public Members.
		//################################
		/// <summary>
		/// Graphic affected by the UIEffect.
		/// </summary>
		new public Graphic graphic { get { return base.graphic; } }
		
		/// <summary>
		/// How far is the blurring shadow from the graphic.
		/// </summary>
		public float blur { get { return m_Blur; } set { m_Blur = Mathf.Clamp(value, 0, 2); _SetDirty(); } }

		/// <summary>
		/// Shadow effect mode.
		/// </summary>
		public ShadowStyle style { get { return m_Style; } set { m_Style = value; _SetDirty(); } }
		
		/// <summary>
		/// Additional Shadows.
		/// </summary>
		public List<AdditionalShadow> additionalShadows { get { return m_AdditionalShadows; } }

		/// <summary>
		/// Modifies the mesh.
		/// </summary>
		public override void ModifyMesh(VertexHelper vh)
		{
			if (!isActiveAndEnabled || vh.currentVertCount <= 0)
			{
				return;
			}

			vh.GetUIVertexStream(s_Verts);

			//================================
			// Append shadow vertices.
			//================================
			{
				_uiEffect = GetComponent<UIEffect>();
				var inputVertCount = s_Verts.Count;
				var start = 0;
				var end = inputVertCount;
				var toneLevel = _uiEffect && _uiEffect.isActiveAndEnabled ? _uiEffect.toneLevel : 0;

				// Additional Shadows.
				for (int i = additionalShadows.Count - 1; 0 <= i; i--)
				{
					AdditionalShadow shadow = additionalShadows[i];
					UpdateFactor(toneLevel, shadow.blur, shadow.effectColor);
					_ApplyShadow(s_Verts, shadow.effectColor, ref start, ref end, shadow.effectDistance, shadow.style, shadow.useGraphicAlpha);
				}

				// Shadow.
				UpdateFactor(toneLevel, blur, effectColor);
				_ApplyShadow(s_Verts, effectColor, ref start, ref end, effectDistance, style, useGraphicAlpha);
			}

			vh.Clear();
			vh.AddUIVertexTriangleStream(s_Verts);

			s_Verts.Clear();
		}

		UIEffect _uiEffect;
		Vector2 _factor;

		//################################
		// Private Members.
		//################################
		static readonly List<UIVertex> s_Verts = new List<UIVertex>();

		void UpdateFactor(float tone, float blur, Color color)
		{
			if (_uiEffect && _uiEffect.isActiveAndEnabled)
			{
				_factor = new Vector2(Packer.ToFloat(tone, 0, blur, 0), Packer.ToFloat(color.r, color.g, color.b, 1));
			}
		}

		/// <summary>
		/// Append shadow vertices.
		/// * It is similar to Shadow component implementation.
		/// </summary>
		void _ApplyShadow(List<UIVertex> verts, Color color, ref int start, ref int end, Vector2 effectDistance, ShadowStyle style, bool useGraphicAlpha)
		{
			if (style == ShadowStyle.None || color.a <= 0)
				return;

			// Append Shadow.
			_ApplyShadowZeroAlloc(s_Verts, color, ref start, ref end, effectDistance.x, effectDistance.y, useGraphicAlpha);

			// Append Shadow3.
			if (ShadowStyle.Shadow3 == style)
			{
				_ApplyShadowZeroAlloc(s_Verts, color, ref start, ref end, effectDistance.x, 0, useGraphicAlpha);
				_ApplyShadowZeroAlloc(s_Verts, color, ref start, ref end, 0, effectDistance.y, useGraphicAlpha);
			}

			// Append Outline.
			else if (ShadowStyle.Outline == style)
			{
				_ApplyShadowZeroAlloc(s_Verts, color, ref start, ref end, effectDistance.x, -effectDistance.y, useGraphicAlpha);
				_ApplyShadowZeroAlloc(s_Verts, color, ref start, ref end, -effectDistance.x, effectDistance.y, useGraphicAlpha);
				_ApplyShadowZeroAlloc(s_Verts, color, ref start, ref end, -effectDistance.x, -effectDistance.y, useGraphicAlpha);
			}

			// Append Outline8.
			else if (ShadowStyle.Outline8 == style)
			{
				_ApplyShadowZeroAlloc(s_Verts, color, ref start, ref end, effectDistance.x, -effectDistance.y, useGraphicAlpha);
				_ApplyShadowZeroAlloc(s_Verts, color, ref start, ref end, -effectDistance.x, effectDistance.y, useGraphicAlpha);
				_ApplyShadowZeroAlloc(s_Verts, color, ref start, ref end, -effectDistance.x, -effectDistance.y, useGraphicAlpha);
				_ApplyShadowZeroAlloc(s_Verts, color, ref start, ref end, -effectDistance.x, 0, useGraphicAlpha);
				_ApplyShadowZeroAlloc(s_Verts, color, ref start, ref end, 0, -effectDistance.y, useGraphicAlpha);
				_ApplyShadowZeroAlloc(s_Verts, color, ref start, ref end, effectDistance.x, 0, useGraphicAlpha);
				_ApplyShadowZeroAlloc(s_Verts, color, ref start, ref end, 0, effectDistance.y, useGraphicAlpha);
			}
		}

		/// <summary>
		/// Append shadow vertices.
		/// * It is similar to Shadow component implementation.
		/// </summary>
		void _ApplyShadowZeroAlloc(List<UIVertex> verts, Color color, ref int start, ref int end, float x, float y, bool useGraphicAlpha)
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
				if(_uiEffect)
					vt.uv1 = _factor;
				verts[i] = vt;
			}

			// Update next shadow offset.
			start = end;
			end = verts.Count;
		}

		/// <summary>
		/// Mark the UIEffect as dirty.
		/// </summary>
		void _SetDirty()
		{
			if(graphic)
				graphic.SetVerticesDirty();
		}
	}
}
