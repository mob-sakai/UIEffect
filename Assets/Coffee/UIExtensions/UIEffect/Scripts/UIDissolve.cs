using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Serialization;
using System.Text;
using System.Linq;
using System.IO;

namespace Coffee.UIExtensions
{
	/// <summary>
	/// Dissolve effect for uGUI.
	/// </summary>
	[AddComponentMenu("UI/UIEffect/UIDissolve", 3)]
	public class UIDissolve : UIEffectBase, IMaterialModifier
	{
		//################################
		// Constant or Static Members.
		//################################
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
					SetEffectDirty();
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
					SetEffectDirty();
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
					SetEffectDirty();
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
					SetEffectDirty();
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
					SetEffectDirty();
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
						graphic.SetMaterialDirty();
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
		public ColorMode colorMode
		{
			get { return m_ColorMode; }
			set
			{
				if (m_ColorMode != value)
				{
					m_ColorMode = value;
					if (graphic)
					{
						graphic.SetMaterialDirty();
					}
				}
			}
		}

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

		public override Hash128 GetMaterialHash(Material material)
		{
			if(!isActiveAndEnabled || !material || !material.shader)
				return new Hash128();

			uint materialId = (uint)material.GetInstanceID();
			uint shaderId = 0 << 3;

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


			uint shaderVariantId = (uint)((int)m_ColorMode << 6);
			uint resourceId = m_NoiseTexture ? (uint)m_NoiseTexture.GetInstanceID() : 0;
			return new Hash128(
					materialId,
					shaderId + shaderVariantId,
					resourceId,
					0
				);
		}

		public override void ModifyMaterial(Material material)
		{
			Debug.LogFormat(this, $"ModifyMaterial {material}");

			string materialShaderName = material.shader.name;
			if(materialShaderName.StartsWith ("TextMeshPro/Mobile/", StringComparison.Ordinal))
			{
				material.shader = Shader.Find ("TextMeshPro/Mobile/Distance Field (UIDissolve)");
			}
			else if (materialShaderName.Equals ("TextMeshPro/Sprite", StringComparison.Ordinal))
			{
				material.shader = Shader.Find ("UI/Hidden/UI-Effect-Dissolve");
			}
			else if (materialShaderName.StartsWith ("TextMeshPro/", StringComparison.Ordinal))
			{
				material.shader = Shader.Find ("TextMeshPro/Distance Field (UIDissolve)");
			}
			else
			{
				material.shader = Shader.Find ("UI/Hidden/UI-Effect-Dissolve");
			}
			
			SetShaderVariants(material, m_ColorMode);

			if(m_NoiseTexture)
			{
				material.SetTexture("_NoiseTex", m_NoiseTexture);
			}
			ptex.RegisterMaterial (material);
		}

		/// <summary>
		/// Modifies the material.
		/// </summary>
		[ContextMenu("ModifyMaterial")]
		public override void ModifyMaterial()
		{
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

		protected override void SetEffectDirty()
		{
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
			_player.OnEnable((f) =>effectFactor = m_Reverse ? 1f - f : f);
		}

		protected override void OnDisable()
		{
			base.OnDisable ();
			_player.OnDisable();
		}

#if UNITY_EDITOR
		/// <summary>
		/// Gets the material.
		/// </summary>
		/// <returns>The material.</returns>
		protected override Material GetMaterial()
		{
			return null;
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
		EffectPlayer _player{ get { return m_Player ?? (m_Player = new EffectPlayer()); } }
	}
}
