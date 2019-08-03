using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using System;

namespace Coffee.UIExtensions
{
	/// <summary>
	/// Transition effect.
	/// </summary>
	[AddComponentMenu("UI/UIEffect/UITransitionEffect", 5)]
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
			Fade = 1,
			Cutoff = 2,
			Dissolve = 3,
		}


		//################################
		// Serialize Members.
		//################################
		[Tooltip("Effect mode.")]
		[SerializeField] EffectMode m_EffectMode = EffectMode.Cutoff;

		[Tooltip("Effect factor between 0(hidden) and 1(shown).")]
		[SerializeField][Range(0, 1)] float m_EffectFactor = 0.5f;

		[Tooltip("Transition texture (single channel texture).")]
		[SerializeField] Texture m_TransitionTexture;

		[Header("Advanced Option")]
		[Tooltip("The area for effect.")]
		[SerializeField] EffectArea m_EffectArea = EffectArea.RectTransform;

		[Tooltip("Keep effect aspect ratio.")]
		[SerializeField] bool m_KeepAspectRatio;

		[Tooltip("Dissolve edge width.")]
		[SerializeField] [Range(0, 1)] float m_DissolveWidth = 0.5f;

		[Tooltip("Dissolve edge softness.")]
		[SerializeField] [Range(0, 1)] float m_DissolveSoftness = 0.5f;

		[Tooltip("Dissolve edge color.")]
		[SerializeField] [ColorUsage(false)] Color m_DissolveColor = new Color(0.0f, 0.25f, 1.0f);

		[Tooltip("Disable graphic's raycast target on hidden.")]
		[SerializeField] bool m_PassRayOnHidden;

		[Header("Effect Player")]
		[SerializeField] EffectPlayer m_Player;

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
					SetEffectDirty ();
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

		/// <summary>
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
					SetEffectDirty ();
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
					SetEffectDirty ();
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
					SetEffectDirty ();
				}
			}
		}

		/// <summary>
		/// Duration for showing/hiding.
		/// </summary>
		public float duration { get { return _player.duration; } set { _player.duration = Mathf.Max(value, 0.1f); } }

		/// <summary>
		/// Disable graphic's raycast target on hidden.
		/// </summary>
		public bool passRayOnHidden { get { return m_PassRayOnHidden; } set { m_PassRayOnHidden = value; } }

		/// <summary>
		/// Update mode for showing/hiding.
		/// </summary>
		public AnimatorUpdateMode updateMode { get { return _player.updateMode; } set { _player.updateMode = value; } }

		/// <summary>
		/// Show transition.
		/// </summary>
		public void Show(bool reset = true)
		{
			_player.loop = false;
			_player.Play(reset, f => effectFactor = f);
		}

		/// <summary>
		/// Hide transition.
		/// </summary>
		public void Hide(bool reset = true)
		{
			_player.loop = false;
			_player.Play(reset, f => effectFactor = 1 - f);
		}


		public override Hash128 GetMaterialHash (Material material)
		{
			if (!isActiveAndEnabled || !material || !material.shader)
				return new Hash128 ();

			uint materialId = (uint)material.GetInstanceID ();
			uint shaderId = 5 << 3;

			string materialShaderName = material.shader.name;
			if (materialShaderName.StartsWith ("TextMeshPro/Mobile/", StringComparison.Ordinal))
			{
				shaderId += 2;
			}
			else if (materialShaderName.Equals ("TextMeshPro/Sprite", StringComparison.Ordinal))
			{
				shaderId += 0;
			}
			else if (materialShaderName.StartsWith ("TextMeshPro/", StringComparison.Ordinal))
			{
				shaderId += 1;
			}
			else
			{
				shaderId += 0;
			}


			uint shaderVariantId = (uint)((int)m_EffectMode << 6);
			uint resourceId = m_TransitionTexture ? (uint)m_TransitionTexture.GetInstanceID () : 0;
			return new Hash128 (
					materialId,
					shaderId + shaderVariantId,
					resourceId,
					0
				);
		}

		public override void ModifyMaterial (Material material)
		{
			Debug.LogFormat (this, $"ModifyMaterial {material}");

			string materialShaderName = material.shader.name;
			if (materialShaderName.StartsWith ("TextMeshPro/Mobile/", StringComparison.Ordinal))
			{
				material.shader = Shader.Find ("TextMeshPro/Mobile/Distance Field (UITransition)");
			}
			else if (materialShaderName.Equals ("TextMeshPro/Sprite", StringComparison.Ordinal))
			{
				material.shader = Shader.Find ("UI/Hidden/UI-Effect-Transition");
			}
			else if (materialShaderName.StartsWith ("TextMeshPro/", StringComparison.Ordinal))
			{
				material.shader = Shader.Find ("TextMeshPro/Distance Field (UITransition)");
			}
			else
			{
				material.shader = Shader.Find ("UI/Hidden/UI-Effect-Transition");
			}

			SetShaderVariants (material, m_EffectMode);

			if (m_TransitionTexture)
			{
				material.SetTexture ("_NoiseTex", m_TransitionTexture);
			}
			ptex.RegisterMaterial (material);
		}

		/// <summary>
		/// Modifies the material.
		/// </summary>
		public override void ModifyMaterial()
		{
		}

		/// <summary>
		/// Modifies the mesh.
		/// </summary>
		public override void ModifyMesh(VertexHelper vh)
		{
			if (!isActiveAndEnabled)
			{
				return;
			}

			bool isText = isTMPro || graphic is Text;
			float normalizedIndex = ptex.GetNormalizedIndex (this);

			// rect.
			var tex = transitionTexture;
			var aspectRatio = m_KeepAspectRatio && tex ? ((float)tex.width) / tex.height : -1;
			Rect rect = m_EffectArea.GetEffectArea (vh, rectTransform.rect, aspectRatio);

			// Set prameters to vertex.
			UIVertex vertex = default(UIVertex);
			float x, y;
			int count = vh.currentVertCount;
			for (int i = 0; i < count; i++)
			{
				vh.PopulateUIVertex(ref vertex, i);
				m_EffectArea.GetPositionFactor (i, rect, vertex.position, isText, isTMPro, out x, out y);

				vertex.uv0 = new Vector2 (
					Packer.ToFloat (vertex.uv0.x, vertex.uv0.y),
					Packer.ToFloat (x, y, normalizedIndex)
				);
				vh.SetUIVertex(vertex, i);
			}
		}

		//################################
		// Protected Members.
		//################################

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			_player.OnEnable(null);
			_player.loop = false;
		}

		/// <summary>
		/// This function is called when the behaviour becomes disabled () or inactive.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable ();
			MaterialCache.Unregister(_materialCache);
			_materialCache = null;
			_player.OnDisable();
		}

		protected override void SetEffectDirty ()
		{
			ptex.SetData(this, 0, m_EffectFactor);	// param1.x : effect factor
			if (m_EffectMode == EffectMode.Dissolve)
			{
				ptex.SetData(this, 1, m_DissolveWidth);		// param1.y : width
				ptex.SetData(this, 2, m_DissolveSoftness);	// param1.z : softness
				ptex.SetData(this, 4, m_DissolveColor.r);	// param2.x : red
				ptex.SetData(this, 5, m_DissolveColor.g);	// param2.y : green
				ptex.SetData(this, 6, m_DissolveColor.b);	// param2.z : blue
			}

			// Disable graphic's raycastTarget on hidden.
			if (m_PassRayOnHidden)
			{
				targetGraphic.raycastTarget = 0 < m_EffectFactor;
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// Gets the material.
		/// </summary>
		/// <returns>The material.</returns>
		protected override Material GetMaterial()
		{
			return MaterialResolver.GetOrGenerateMaterialVariant(Shader.Find(shaderName), m_EffectMode);
		}
#endif

		//################################
		// Private Members.
		//################################
		MaterialCache _materialCache = null;

		EffectPlayer _player{ get { return m_Player ?? (m_Player = new EffectPlayer()); } }
	}
}
