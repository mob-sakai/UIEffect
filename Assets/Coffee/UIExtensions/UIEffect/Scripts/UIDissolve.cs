using System;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIExtensions
{
	/// <summary>
	/// Dissolve effect for uGUI.
	/// </summary>
	[ExecuteInEditMode]
	public class UIDissolve : UIEffectBase
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
		[SerializeField] protected EffectArea m_EffectArea;
		[SerializeField] bool m_KeepAspectRatio;
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
			get { return m_NoiseTexture ?? graphic.material.GetTexture("_NoiseTex"); }
			set
			{
				if (m_NoiseTexture != value)
				{
					m_NoiseTexture = value;
					if (graphic)
					{
						ModifyMaterial();
					}
				}
			}
		}

		/// <summary>
		/// The area for effect.
		/// </summary>
		public EffectArea effectArea
		{
			get { return m_EffectArea; }
			set
			{
				if (m_EffectArea != value)
				{
					m_EffectArea = value;
					SetDirty();
				}
			}
		}

		/// <summary>
		/// Keep aspect ratio.
		/// </summary>
		public bool keepAspectRatio
		{
			get { return m_KeepAspectRatio; }
			set
			{
				if (m_KeepAspectRatio != value)
				{
					m_KeepAspectRatio = value;
					SetDirty();
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
		public override void ModifyMaterial()
		{
			ulong hash = (m_NoiseTexture ? (uint)m_NoiseTexture.GetInstanceID() : 0) + ((ulong)m_ColorMode << 32);
			if (_materialCache != null && (_materialCache.hash != hash || !isActiveAndEnabled || !m_EffectMaterial))
			{
				MaterialCache.Unregister(_materialCache);
				_materialCache = null;
			}

			if (!isActiveAndEnabled || !m_EffectMaterial)
			{
				graphic.material = null;
			}
			else if (!m_NoiseTexture)
			{
				graphic.material = m_EffectMaterial;
			}
			else if (_materialCache != null && _materialCache.hash == hash)
			{
				graphic.material = _materialCache.material;
			}
			else
			{
				_materialCache = MaterialCache.Register(hash, m_NoiseTexture, () =>
					{
						var mat = new Material(m_EffectMaterial);
						mat.SetTexture("_NoiseTex", m_NoiseTexture);
						return mat;
					});
				graphic.material = _materialCache.material;
			}
		}

		/// <summary>
		/// Modifies the mesh.
		/// </summary>
		public override void ModifyMesh(VertexHelper vh)
		{
			if (!isActiveAndEnabled)
				return;

			// rect.
			var tex = noiseTexture;
			var aspectRatio = m_KeepAspectRatio && tex ? ((float)tex.width) / tex.height : -1;
			Rect rect = m_EffectArea.GetEffectArea(vh, graphic, aspectRatio);

			// Calculate vertex position.
			UIVertex vertex = default(UIVertex);
			bool effectEachCharacter = graphic is Text && m_EffectArea == EffectArea.Character;
			float x, y;
			for (int i = 0; i < vh.currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref vertex, i);

				if (effectEachCharacter)
				{
					x = splitedCharacterPosition[i%4].x;
					y = splitedCharacterPosition[i%4].y;
				}
				else
				{
					x = Mathf.Clamp01(vertex.position.x / rect.width + 0.5f);
					y = Mathf.Clamp01(vertex.position.y / rect.height + 0.5f);
				}
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
