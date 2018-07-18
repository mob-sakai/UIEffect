using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


namespace Coffee.UIExtensions
{
	/// <summary>
	/// HSV Modifier.
	/// </summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	public class UIHsvModifier : UIEffectBase
	{
		//################################
		// Constant or Static Members.
		//################################
		public const string shaderName = "UI/Hidden/UI-Effect-HSV";


		//################################
		// Serialize Members.
		//################################
		[Header("Target")]
		[SerializeField] [ColorUsage(false)] Color m_TargetColor = Color.red;
		[SerializeField] [Range(0, 1)] float m_Range = 0.1f;
		[Header("Adjustment")]
		[SerializeField] [Range(-0.5f, 0.5f)] float m_Hue;
		[SerializeField] [Range(-0.5f, 0.5f)] float m_Saturation;
		[SerializeField] [Range(-0.5f, 0.5f)] float m_Value;


		//################################
		// Public Members.
		//################################
		/// <summary>
		/// Target color to affect hsv shift.
		/// </summary>
		public Color targetColor
		{
			get { return m_TargetColor; }
			set
			{ 
				if (m_TargetColor != value)
				{
					m_TargetColor = value;
					SetDirty();
				}
			}
		}

		/// <summary>
		/// Color range to affect hsv shift [0 ~ 1].
		/// </summary>
		public float range
		{
			get { return m_Range; }
			set
			{
				value = Mathf.Clamp(value, 0, 1);
				if (!Mathf.Approximately(m_Range, value))
				{
					m_Range = value;
					SetDirty();
				}
			}
		}

		/// <summary>
		/// Saturation shift [-0.5 ~ 0.5].
		/// </summary>
		public float saturation
		{
			get { return m_Saturation; }
			set
			{
				value = Mathf.Clamp(value, -0.5f, 0.5f);
				if (!Mathf.Approximately(m_Saturation, value))
				{
					m_Saturation = value;
					SetDirty();
				}
			}
		}

		/// <summary>
		/// Value shift [-0.5 ~ 0.5].
		/// </summary>
		public float value
		{
			get { return m_Value; }
			set
			{
				value = Mathf.Clamp(value, -0.5f, 0.5f);
				if (!Mathf.Approximately(m_Value, value))
				{
					m_Value = value;
					SetDirty();
				}
			}
		}

		/// <summary>
		/// Hue shift [-0.5 ~ 0.5].
		/// </summary>
		public float hue
		{
			get { return m_Hue; }
			set
			{
				value = Mathf.Clamp(value, -0.5f, 0.5f);
				if (!Mathf.Approximately(m_Hue, value))
				{
					m_Hue = value;
					SetDirty();
				}
			}
		}

#if UNITY_EDITOR
		protected override Material GetMaterial()
		{
			return MaterialResolver.GetOrGenerateMaterialVariant(Shader.Find(shaderName));
		}
#endif

		/// <summary>
		/// Modifies the mesh.
		/// </summary>
		public override void ModifyMesh(VertexHelper vh)
		{
			if (!isActiveAndEnabled)
				return;

			vh.GetUIVertexStream(tempVerts);
			vh.Clear();

			float h,s,v;
			Color.RGBToHSV(m_TargetColor, out h, out s, out v);

			// Pack some effect factors to 1 float.
			Vector2 factor = new Vector2(
				Packer.ToFloat(h, s, v, m_Range),
				Packer.ToFloat(m_Hue + 0.5f, m_Saturation + 0.5f, m_Value + 0.5f)
			);

			for (int i = 0; i < tempVerts.Count; i++)
			{
				UIVertex vt = tempVerts[i];
				vt.uv1 = factor;
				tempVerts[i] = vt;
			}

			vh.AddUIVertexTriangleStream(tempVerts);
		}

		//################################
		// Private Members.
		//################################
	}
}
