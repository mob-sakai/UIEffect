using System;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIExtensions
{
	/// <summary>
	/// Dissolve effect for uGUI.
	/// </summary>
	[ExecuteInEditMode]
	public class UIDissolve : UIEffectBase, IMaterialModifier
	{
		//################################
		// Constant or Static Members.
		//################################
		public const string shaderName = "UI/Hidden/UI-Effect-Dissolve";


		//################################
		// Serialize Members.
		//################################
		[SerializeField] [Range(0, 1)] float m_Location = 0.5f;
		[SerializeField] [Range(0, 1)] float m_Width = 0.5f;
		[SerializeField] [Range(0, 1)] float m_Softness = 0.5f;
		[SerializeField] [ColorUsage(false)] Color m_Color = new Color(0.0f, 0.25f, 1.0f);
		[SerializeField] ColorMode m_ColorMode = ColorMode.Add;
		[SerializeField] Texture m_NoiseTexture;
		[Header("Play Effect")]
		[SerializeField] bool m_Play = false;
		[SerializeField][Range(0.1f, 10)] float m_Duration = 1;
		[SerializeField] AnimatorUpdateMode m_UpdateMode = AnimatorUpdateMode.Normal;


		//################################
		// Public Members.
		//################################

		/// <summary>
		/// Current location[0-1] for dissolve effect. 0 is not dissolved, 1 is completely dissolved.
		/// </summary>
		public float location
		{
			get { return m_Location; }
			set
			{
				value = Mathf.Clamp(value, 0, 1);
				if (!Mathf.Approximately(m_Location, value))
				{
					m_Location = value;
					SetDirty();
				}
			}
		}

		/// <summary>
		/// Edge width.
		/// </summary>
		public float width
		{
			get { return m_Width; }
			set
			{
				value = Mathf.Clamp(value, 0, 1);
				if (!Mathf.Approximately(m_Width, value))
				{
					m_Width = value;
					SetDirty();
				}
			}
		}

		/// <summary>
		/// Edge softness.
		/// </summary>
		public float softness
		{
			get { return m_Softness; }
			set
			{
				value = Mathf.Clamp(value, 0, 1);
				if (!Mathf.Approximately(m_Softness, value))
				{
					m_Softness = value;
					SetDirty();
				}
			}
		}

		/// <summary>
		/// Edge color.
		/// </summary>
		public Color color
		{
			get { return m_Color; }
			set
			{
				if (m_Color != value)
				{
					m_Color = value;
					SetDirty();
				}
			}
		}

		/// <summary>
		/// Noise texture.
		/// </summary>
		public Texture noiseTexture
		{
			get { return m_NoiseTexture; }
			set
			{
				if (m_NoiseTexture != value)
				{
					m_NoiseTexture = value;
					if (graphic)
					{
						graphic.SetMaterialDirty();
					}
				}
			}
		}

		/// <summary>
		/// Color effect mode.
		/// </summary>
		public ColorMode colorMode { get { return m_ColorMode; } }

		/// <summary>
		/// Play dissolve on enable.
		/// </summary>
		public bool play { get { return m_Play; } set { m_Play = value; } }

		/// <summary>
		/// Dissolve duration.
		/// </summary>
		public float duration { get { return m_Duration; } set { m_Duration = Mathf.Max(value, 0.1f); } }

		/// <summary>
		/// Dissolve update mode.
		/// </summary>
		public AnimatorUpdateMode updateMode { get { return m_UpdateMode; } set { m_UpdateMode = value; } }

		/// <summary>
		/// Modifies the material.
		/// </summary>
		public Material GetModifiedMaterial(Material baseMaterial)
		{
			if (_materialCache != null && !_materialCache.IsMatch(m_ColorMode, m_NoiseTexture))
			{
				MaterialCache.Unregister(_materialCache);
				_materialCache = null;
			}

			if (!isActiveAndEnabled || !m_NoiseTexture || !m_EffectMaterial)
			{
				return baseMaterial;
			}
			else if (_materialCache != null && _materialCache.IsMatch(m_ColorMode, m_NoiseTexture))
			{
				return _materialCache.material;
			}

			_materialCache = MaterialCache.Register(m_ColorMode, m_NoiseTexture, () =>
				{
					var mat = new Material(m_EffectMaterial);
					mat.SetTexture("_NoiseTex", m_NoiseTexture);
					return mat;
				});
			return _materialCache.material;
		}

		/// <summary>
		/// Modifies the mesh.
		/// </summary>
		public override void ModifyMesh(VertexHelper vh)
		{
			if (!isActiveAndEnabled)
				return;

			// rect.
			Rect rect = targetGraphic.rectTransform.rect;

			// Calculate vertex position.
			UIVertex vertex = default(UIVertex);
			for (int i = 0; i < vh.currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref vertex, i);

				var x = Mathf.Clamp01(vertex.position.x / rect.width + 0.5f);
				var y = Mathf.Clamp01(vertex.position.y / rect.height + 0.5f);
				vertex.uv1 = new Vector2(
					Packer.ToFloat(x, y, location, m_Width),
					Packer.ToFloat(m_Color.r, m_Color.g, m_Color.b, m_Softness)
				);

				vh.SetUIVertex(vertex, i);
			}
		}

		/// <summary>
		/// Play effect.
		/// </summary>
		public void Play()
		{
			_time = 0;
			m_Play = true;
		}


		//################################
		// Protected Members.
		//################################
		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected override void OnEnable()
		{
			_time = 0;
			base.OnEnable();
		}

		protected override void OnDisable()
		{
			MaterialCache.Unregister(_materialCache);
			_materialCache = null;
			base.OnDisable();

		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();
			if (graphic)
			{
				graphic.SetMaterialDirty();
			}
		}

		/// <summary>
		/// Gets the material.
		/// </summary>
		/// <returns>The material.</returns>
		protected override Material GetMaterial()
		{
			return MaterialResolver.GetOrGenerateMaterialVariant(Shader.Find(shaderName), m_ColorMode);
		}
#endif

		//################################
		// Private Members.
		//################################
		MaterialCache _materialCache = null;
		float _time = 0;

		void Update()
		{
			if (!m_Play || !Application.isPlaying)
			{
				return;
			}

			_time += m_UpdateMode == AnimatorUpdateMode.UnscaledTime
				? Time.unscaledDeltaTime
				: Time.deltaTime;
			location = _time / m_Duration;

			if (m_Duration <= _time)
			{
				m_Play = false;
				_time = 0;
			}
		}
	}
}
