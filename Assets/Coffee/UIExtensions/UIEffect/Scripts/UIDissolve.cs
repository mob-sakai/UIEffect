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
	/// UIDissolve.
	/// </summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	public class UIDissolve : BaseMeshEffect
#if UNITY_EDITOR
		, ISerializationCallbackReceiver
#endif
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
		[SerializeField] UIEffect.ColorMode m_ColorMode = UIEffect.ColorMode.Add;
		[SerializeField] Material m_EffectMaterial;
		[Space]
		[SerializeField] bool m_Play = false;
		[SerializeField][Range(0.1f, 10)] float m_Duration = 1;
		[SerializeField] AnimatorUpdateMode m_UpdateMode = AnimatorUpdateMode.Normal;

		//################################
		// Public Members.
		//################################
		/// <summary>
		/// Graphic affected by the UIEffect.
		/// </summary>
		new public Graphic graphic { get { return base.graphic; } }

		/// <summary>
		/// Location for effect.
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
					_SetDirty();
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
					_SetDirty();
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
					_SetDirty();
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
					_SetDirty();
				}
			}
		}

		/// <summary>
		/// Color effect mode.
		/// </summary>
		public UIEffect.ColorMode colorMode { get { return m_ColorMode; } }

		/// <summary>
		/// Effect material.
		/// </summary>
		public virtual Material effectMaterial { get { return m_EffectMaterial; } }

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
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected override void OnEnable()
		{
			_time = 0;
			graphic.material = effectMaterial;
			base.OnEnable();
		}

		/// <summary>
		/// This function is called when the behaviour becomes disabled () or inactive.
		/// </summary>
		protected override void OnDisable()
		{
			graphic.material = null;
			base.OnDisable();
		}

#if UNITY_EDITOR
		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			var obj = this;
			EditorApplication.delayCall += () =>
			{
				if (Application.isPlaying || !obj || !obj.graphic)
					return;
				
				var mat = UIEffect.GetOrGenerateMaterialVariant(Shader.Find(shaderName), UIEffect.ToneMode.None, m_ColorMode, UIEffect.BlurMode.None);

				if (m_EffectMaterial == mat && graphic.material == mat)
					return;

				graphic.material = m_EffectMaterial = mat;
				EditorUtility.SetDirty(this);
				EditorUtility.SetDirty(graphic);
				EditorApplication.delayCall += AssetDatabase.SaveAssets;
			};
		}
#endif

		/// <summary>
		/// Modifies the mesh.
		/// </summary>
		public override void ModifyMesh(VertexHelper vh)
		{
			if (!isActiveAndEnabled)
				return;

			// rect.
			Rect rect = graphic.rectTransform.rect;

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
		// Private Members.
		//################################
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

		/// <summary>
		/// Mark the UIEffect as dirty.
		/// </summary>
		void _SetDirty()
		{
			if (graphic)
				graphic.SetVerticesDirty();
		}
	}
}
