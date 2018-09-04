using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIExtensions
{
	/// <summary>
	/// Transition effect.
	/// </summary>
	public class UITransitionEffect : UIEffectBase
	{
		//################################
		// Constant or Static Members.
		//################################
		public const string shaderName = "UI/Hidden/UI-Effect-Transition";
		static readonly ParameterTexture _ptex = new ParameterTexture(8, 128, "_ParamTex");

		/// <summary>
		/// Effect mode.
		/// </summary>
		public enum EffectMode
		{
			None = 0,
			Fade = 1,
			Cutoff = 2,
			Dissolve = 3,
		}


		//################################
		// Serialize Members.
		//################################
		[SerializeField] EffectMode m_EffectMode = EffectMode.Cutoff;
		[SerializeField][Range(0, 1)] float m_EffectFactor = 1;
		[SerializeField] Texture m_TransitionTexture;
		[SerializeField] EffectArea m_EffectArea;
		[SerializeField] bool m_KeepAspectRatio;
		[SerializeField] [Range(0, 1)] float m_DissolveWidth = 0.5f;
		[SerializeField] [Range(0, 1)] float m_DissolveSoftness = 0.5f;
		[SerializeField] [ColorUsage(false)] Color m_DissolveColor = new Color(0.0f, 0.25f, 1.0f);


		//################################
		// Public Members.
		//################################
		/// <summary>
		/// Effect factor between 0(no effect) and 1(complete effect).
		/// </summary>
		public float effectFactor
		{
			get { return m_EffectFactor; }
			set
			{
				value = Mathf.Clamp(value, 0, 1);
				if (!Mathf.Approximately(m_EffectFactor, value))
				{
					m_EffectFactor = value;
					SetDirty();
				}
			}
		}

		/// <summary>
		/// Transition texture.
		/// </summary>
		public Texture transitionTexture
		{
			get { return m_TransitionTexture; }
			set
			{
				if (m_TransitionTexture != value)
				{
					m_TransitionTexture = value;
					if (graphic)
					{
						ModifyMaterial();
					}
				}
			}
		}

		/// <summary>
		/// Effect mode.
		/// </summary>
		public EffectMode effectMode { get { return m_EffectMode; } }

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
					targetGraphic.SetVerticesDirty();
				}
			}
		}

		/// Gets the parameter texture.
		/// </summary>
		public override ParameterTexture ptex { get { return _ptex; } }

		/// <summary>
		/// Dissolve edge width.
		/// </summary>
		public float dissolveWidth
		{
			get { return m_DissolveWidth; }
			set
			{
				value = Mathf.Clamp(value, 0, 1);
				if (!Mathf.Approximately(m_DissolveWidth, value))
				{
					m_DissolveWidth = value;
					SetDirty();
				}
			}
		}

		/// <summary>
		/// Dissolve edge softness.
		/// </summary>
		public float dissolveSoftness
		{
			get { return m_DissolveSoftness; }
			set
			{
				value = Mathf.Clamp(value, 0, 1);
				if (!Mathf.Approximately(m_DissolveSoftness, value))
				{
					m_DissolveSoftness = value;
					SetDirty();
				}
			}
		}

		/// <summary>
		/// Dissolve edge color.
		/// </summary>
		public Color dissolveColor
		{
			get { return m_DissolveColor; }
			set
			{
				if (m_DissolveColor != value)
				{
					m_DissolveColor = value;
					SetDirty();
				}
			}
		}

		/// <summary>
		/// Modifies the material.
		/// </summary>
		public override void ModifyMaterial()
		{
			ulong hash = (m_TransitionTexture ? (uint)m_TransitionTexture.GetInstanceID() : 0) + ((ulong)2 << 32) + ((ulong)m_EffectMode << 36);
			if (_materialCache != null && (_materialCache.hash != hash || !isActiveAndEnabled || !m_EffectMaterial))
			{
				MaterialCache.Unregister(_materialCache);
				_materialCache = null;
			}

			if (!isActiveAndEnabled || !m_EffectMaterial)
			{
				graphic.material = null;
			}
			else if (!m_TransitionTexture)
			{
				graphic.material = m_EffectMaterial;
			}
			else if (_materialCache != null && _materialCache.hash == hash)
			{
				graphic.material = _materialCache.material;
			}
			else
			{
				_materialCache = MaterialCache.Register(hash, m_TransitionTexture, () =>
					{
						var mat = new Material(m_EffectMaterial);
						mat.name += "_" + m_TransitionTexture.name;
						mat.SetTexture("_TransitionTexture", m_TransitionTexture);
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
			if (!isActiveAndEnabled || m_EffectMode == EffectMode.None)
			{
				return;
			}

			// rect.
			var tex = transitionTexture;
			var aspectRatio = m_KeepAspectRatio && tex ? ((float)tex.width) / tex.height : -1;
			Rect rect = m_EffectArea.GetEffectArea(vh, graphic, aspectRatio);

			// Set prameters to vertex.
			float normalizedIndex = ptex.GetNormalizedIndex(this);
			UIVertex vertex = default(UIVertex);
			bool effectEachCharacter = graphic is Text && m_EffectArea == EffectArea.Character;
			float x, y;
			int count = vh.currentVertCount;
			for (int i = 0; i < count; i++)
			{
				vh.PopulateUIVertex(ref vertex, i);

				if (effectEachCharacter)
				{
					x = splitedCharacterPosition[i % 4].x;
					y = splitedCharacterPosition[i % 4].y;
				}
				else
				{
					x = Mathf.Clamp01(vertex.position.x / rect.width + 0.5f);
					y = Mathf.Clamp01(vertex.position.y / rect.height + 0.5f);
				}

				vertex.uv0 = new Vector2(
					Packer.ToFloat(vertex.uv0.x, vertex.uv0.y),
					Packer.ToFloat(x, y, normalizedIndex)
				);
				vh.SetUIVertex(vertex, i);
			}
		}

		//################################
		// Protected Members.
		//################################
		protected override void OnDisable()
		{
			MaterialCache.Unregister(_materialCache);
			_materialCache = null;
			base.OnDisable();
		}

		protected override void SetDirty()
		{
			ptex.RegisterMaterial(targetGraphic.material);
			ptex.SetData(this, 0, m_EffectFactor);	// param1.x : effect factor
			if (m_EffectMode == EffectMode.Dissolve)
			{
				ptex.SetData(this, 1, m_DissolveWidth);		// param1.y : width
				ptex.SetData(this, 2, m_DissolveSoftness);	// param1.z : softness
				ptex.SetData(this, 4, m_DissolveColor.r);	// param2.x : red
				ptex.SetData(this, 5, m_DissolveColor.g);	// param2.y : green
				ptex.SetData(this, 6, m_DissolveColor.b);	// param2.z : blue
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// Gets the material.
		/// </summary>
		/// <returns>The material.</returns>
		protected override Material GetMaterial()
		{
			return m_EffectMode != EffectMode.None
				? MaterialResolver.GetOrGenerateMaterialVariant(Shader.Find(shaderName), m_EffectMode)
				: null;
		}
#endif

		//################################
		// Private Members.
		//################################
		MaterialCache _materialCache = null;
	}
}
