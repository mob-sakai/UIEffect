using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Serialization;

namespace Coffee.UIExtensions
{
	/// <summary>
	/// Dissolve effect for uGUI.
	/// </summary>
	[AddComponentMenu("UI/UIEffect/UIDissolve", 3)]
	public class UIDissolve : UIEffectBase
	{
		//################################
		// Constant or Static Members.
		//################################
		public const string shaderName = "UI/Hidden/UI-Effect-Dissolve";
		static readonly ParameterTexture _ptex = new ParameterTexture(8, 128, "_ParamTex");


		//################################
		// Serialize Members.
		//################################
		[Tooltip("Current location[0-1] for dissolve effect. 0 is not dissolved, 1 is completely dissolved.")]
		[FormerlySerializedAs("m_Location")]
		[SerializeField] [Range(0, 1)] float m_EffectFactor = 0.5f;

		[Tooltip("Edge width.")]
		[SerializeField] [Range(0, 1)] float m_Width = 0.5f;

		[Tooltip("Edge softness.")]
		[SerializeField] [Range(0, 1)] float m_Softness = 0.5f;

		[Tooltip("Edge color.")]
		[SerializeField] [ColorUsage(false)] Color m_Color = new Color(0.0f, 0.25f, 1.0f);

		[Tooltip("Edge color effect mode.")]
		[SerializeField] ColorMode m_ColorMode = ColorMode.Add;

		[Tooltip("Noise texture for dissolving (single channel texture).")]
		[SerializeField] Texture m_NoiseTexture;

		[Header("Advanced Option")]
		[Tooltip("The area for effect.")]
		[SerializeField] protected EffectArea m_EffectArea;

		[Tooltip("Keep effect aspect ratio.")]
		[SerializeField] bool m_KeepAspectRatio;

		[Header("Effect Player")]
		[SerializeField] EffectPlayer m_Player;

		[Tooltip("Reverse the dissolve effect.")]
		[FormerlySerializedAs("m_ReverseAnimation")]
		[SerializeField] bool m_Reverse = false;

		#pragma warning disable 0414
		[Obsolete][HideInInspector]
		[SerializeField][Range(0.1f, 10)] float m_Duration = 1;
		[Obsolete][HideInInspector]
		[SerializeField] AnimatorUpdateMode m_UpdateMode = AnimatorUpdateMode.Normal;
		#pragma warning restore 0414


		//################################
		// Public Members.
		//################################

		/// <summary>
		/// Effect factor between 0(start) and 1(end).
		/// </summary>
		[System.Obsolete("Use effectFactor instead (UnityUpgradable) -> effectFactor")]
		public float location
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
		/// Effect factor between 0(start) and 1(end).
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
			get { return m_NoiseTexture ?? material.GetTexture("_NoiseTex"); }
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
					SetVerticesDirty();
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
					SetVerticesDirty ();
				}
			}
		}

		/// <summary>
		/// Color effect mode.
		/// </summary>
		public ColorMode colorMode { get { return m_ColorMode; } }

		/// <summary>
		/// Play effect on enable.
		/// </summary>
		[System.Obsolete("Use Play/Stop method instead")]
		public bool play { get { return _player.play; } set { _player.play = value; } }

		/// <summary>
		/// Play effect loop.
		/// </summary>
		[System.Obsolete]
		public bool loop { get { return _player.loop; } set { _player.loop = value; } }

		/// <summary>
		/// The duration for playing effect.
		/// </summary>
		public float duration { get { return _player.duration; } set { _player.duration = Mathf.Max(value, 0.1f); } }

		/// <summary>
		/// Delay on loop effect.
		/// </summary>
		[System.Obsolete]
		public float loopDelay { get { return _player.loopDelay; } set { _player.loopDelay = Mathf.Max(value, 0); } }

		/// <summary>
		/// Update mode for playing effect.
		/// </summary>
		public AnimatorUpdateMode updateMode { get { return _player.updateMode; } set { _player.updateMode = value; } }

		/// <summary>
		/// Reverse the dissolve effect.
		/// </summary>
		public bool reverse { get { return m_Reverse; } set { m_Reverse = value; } }

		/// <summary>
		/// Gets the parameter texture.
		/// </summary>
		public override ParameterTexture ptex { get { return _ptex; } }

		protected ulong GetMaterialHash()
		{
			return (m_NoiseTexture ? (uint)m_NoiseTexture.GetInstanceID() : 0)
				+ ((ulong)1 << 32)
				+ ((ulong)(isTMPro ? 1 : 0) << 36)
				+ ((ulong)m_ColorMode << 37);
		}

		protected Hash128 GetMaterialHashX()
		{
			if(!isActiveAndEnabled)
				return new Hash128();

#if TMP_PRESENT
			if (isTMPro)
			{
				var t = textMeshPro;

				return new Hash128(
					(uint)(1 + ((int)m_ColorMode << 5)),
					(m_NoiseTexture ? (uint)m_NoiseTexture.GetInstanceID() : 0),
					1,
					(uint)t.font.materialHashCode
				);
			}
			else
#endif
			return new Hash128(
				(uint)(1 + ((int)m_ColorMode << 5)),
				(m_NoiseTexture ? (uint)m_NoiseTexture.GetInstanceID() : 0),
				0,
				0
			);
		}

		protected string shaderNameX { get { return isTMPro ? "TextMeshPro/Distance Field (UIDissolve)" : "UI/Hidden/UI-Effect-Dissolve"; } }

		/// <summary>
		/// Modifies the material.
		/// </summary>
		[ContextMenu("ModifyMaterial")]
		public override void ModifyMaterial()
		{
			Hash128 hash = GetMaterialHashX();
			// Debug.LogFormat(this, "ModifyMaterial {0}", hash);
			if(_materialEntity.id == hash)
			{
				//material = _materialEntity.material;
				return;
			}

			MaterialRepository.Unregister(_materialEntity);
			_materialEntity = MaterialRepository.Register(textMeshPro.font.material, hash, m =>{
				Debug.LogFormat(this, "Register {0}, {1}", hash, shaderNameX, targetGraphic.material);
				m.material.shader = Shader.Find(shaderNameX);
				m.material.name = "hogehoge";
				m.material.hideFlags = HideFlags.HideAndDontSave;
				// Debug.LogFormat(this, "Register {0} -> {1}", textMeshPro.font.material, m.material);
				// m.material.CopyPropertiesFromMaterial(textMeshPro.font.material);
				// m.SetVariant(m_ColorMode);
				m.SetVariant(m_ColorMode);


				if(m_NoiseTexture)
				{
					m.material.SetTexture("_NoiseTex", m_NoiseTexture);
				}
			});

			if(isTMPro)
			{
				material = hash == new Hash128()
					? textMeshPro.font.material
					: _materialEntity.material;

			}
			else
			{
				material = _materialEntity.material;

			}
			
		}

		/// <summary>
		/// Modifies the mesh.
		/// </summary>
		public override void ModifyMesh(VertexHelper vh)
		{
			if (!isActiveAndEnabled)
				return;

			bool isText = isTMPro || graphic is Text;
			float normalizedIndex = ptex.GetNormalizedIndex(this);

			// rect.
			var tex = noiseTexture;
			var aspectRatio = m_KeepAspectRatio && tex ? ((float)tex.width) / tex.height : -1;
			Rect rect = m_EffectArea.GetEffectArea(vh, rectTransform.rect, aspectRatio);

			// Calculate vertex position.
			UIVertex vertex = default(UIVertex);
			float x, y;
			int count = vh.currentVertCount;
			for (int i = 0; i < count; i++)
			{
				vh.PopulateUIVertex(ref vertex, i);
				m_EffectArea.GetPositionFactor (i, rect, vertex.position, isText, isTMPro, out x, out y);

				vertex.uv0 = new Vector2(
					Packer.ToFloat(vertex.uv0.x, vertex.uv0.y),
					Packer.ToFloat(x, y, normalizedIndex)
				);

				vh.SetUIVertex(vertex, i);
			}
		}

		protected override void SetDirty()
		{
			foreach(var m in materials)
			{
				ptex.RegisterMaterial (m);
			}
			ptex.SetData(this, 0, m_EffectFactor);	// param1.x : location
			ptex.SetData(this, 1, m_Width);		// param1.y : width
			ptex.SetData(this, 2, m_Softness);	// param1.z : softness
			ptex.SetData(this, 4, m_Color.r);	// param2.x : red
			ptex.SetData(this, 5, m_Color.g);	// param2.y : green
			ptex.SetData(this, 6, m_Color.b);	// param2.z : blue
		}

		/// <summary>
		/// Play effect.
		/// </summary>
		public void Play(bool reset = true)
		{
			_player.Play(reset);
		}

		/// <summary>
		/// Stop effect.
		/// </summary>
		public void Stop(bool reset = true)
		{
			_player.Stop(reset);
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

			_player.OnEnable((f) =>
			{
				effectFactor = m_Reverse ? 1f - f : f;
			});
		}

		protected override void OnDisable()
		{
			base.OnDisable ();
			_materialEntity = MaterialRepository.Unregister(_materialEntity);
			_player.OnDisable();
		}

#if UNITY_EDITOR
		/// <summary>
		/// Gets the material.
		/// </summary>
		/// <returns>The material.</returns>
		protected override Material GetMaterial()
		{
			return _materialEntity.material;
		}

		#pragma warning disable 0612
		protected override void UpgradeIfNeeded()
		{
			// Upgrade for v3.0.0
			if (IsShouldUpgrade(300))
			{
				_player.play = false;
				_player.duration = m_Duration;
				_player.loop = false;
				_player.loopDelay = 1;
				_player.updateMode = m_UpdateMode;
			}
		}

#pragma warning restore 0612
#endif

        //################################
        // Private Members.
        //################################
        MaterialEntity _materialEntity = MaterialEntity.none;
		EffectPlayer _player{ get { return m_Player ?? (m_Player = new EffectPlayer()); } }
	}
}
