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

		/// <summary>
		/// Effect mode.
		/// </summary>
		public enum EffectMode
		{
			None = 0,
			Fade = 1,
			Cutoff = 2,
		}


		//################################
		// Serialize Members.
		//################################
		[SerializeField] EffectMode m_EffectMode;
		[SerializeField][Range(0, 1)] float m_EffectFactor = 1;
		[SerializeField] Texture m_TransitionTexture;
		[SerializeField] EffectArea m_EffectArea;
		[SerializeField] bool m_KeepAspectRatio;


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
					SetDirty();
				}
			}
		}

		/// <summary>
		/// Modifies the material.
		/// </summary>
		public override void ModifyMaterial()
		{
			ulong hash = (m_TransitionTexture ? (uint)m_TransitionTexture.GetInstanceID() : 0) + (uint)m_EffectMode;
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
					effectFactor,
					Packer.ToFloat (x, y)
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
