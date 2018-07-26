using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.Collections;

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
	[DisallowMultipleComponent]
	public class UIShiny : UIEffectBase
	{
		//################################
		// Constant or Static Members.
		//################################
		public const string shaderName = "UI/Hidden/UI-Effect-Shiny";


		//################################
		// Serialize Members.
		//################################
		[SerializeField] [Range(0, 1)] float m_Location = 0;
		[SerializeField] [Range(0, 1)] float m_Width = 0.25f;
		[SerializeField] [Range(-180, 180)] float m_Rotation;
		[SerializeField][Range(0.01f, 1)] float m_Softness = 1f;
		[FormerlySerializedAs("m_Alpha")]
		[SerializeField][Range(0, 1)] float m_Brightness = 1f;
		[SerializeField][Range(0, 1)] float m_Highlight = 1;
		[SerializeField] protected EffectArea m_EffectArea;
		[Header("Play Effect")]
		[SerializeField] bool m_Play = false;
		[SerializeField] bool m_Loop = false;
		[SerializeField][Range(0.1f, 10)] float m_Duration = 1;
		[SerializeField][Range(0, 10)] float m_LoopDelay = 1;
		[SerializeField] AnimatorUpdateMode m_UpdateMode = AnimatorUpdateMode.Normal;


		//################################
		// Public Members.
		//################################
//		/// <summary>
//		/// Graphic affected by the UIEffect.
//		/// </summary>
//		new public Graphic graphic { get { return base.graphic; } }

		/// <summary>
		/// Location for shiny effect.
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
		/// Width for shiny effect.
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
		/// Softness for shiny effect.
		/// </summary>
		public float softness
		{
			get { return m_Softness; }
			set
			{
				value = Mathf.Clamp(value, 0.01f, 1);
				if (!Mathf.Approximately(m_Softness, value))
				{
					m_Softness = value;
					SetDirty();
				}
			}
		}

		/// <summary>
		/// Alpha for shiny effect.
		/// </summary>
		[System.Obsolete("Use brightness instead (UnityUpgradable) -> brightness")]
		public float alpha
		{
			get { return m_Brightness; }
			set
			{
				value = Mathf.Clamp(value, 0, 1);
				if (!Mathf.Approximately(m_Brightness, value))
				{
					m_Brightness = value;
					SetDirty();
				}
			}
		}

		/// <summary>
		/// Brightness for shiny effect.
		/// </summary>
		public float brightness
		{
			get { return m_Brightness; }
			set
			{
				value = Mathf.Clamp(value, 0, 1);
				if (!Mathf.Approximately(m_Brightness, value))
				{
					m_Brightness = value;
					SetDirty();
				}
			}
		}

		/// <summary>
		/// Highlight factor for shiny effect.
		/// </summary>
		public float highlight
		{
			get { return m_Highlight; }
			set
			{
				value = Mathf.Clamp(value, 0, 1);
				if (!Mathf.Approximately(m_Highlight, value))
				{
					m_Highlight = value;
					SetDirty();
				}
			}
		}

		/// <summary>
		/// Rotation for shiny effect.
		/// </summary>
		public float rotation
		{
			get { return m_Rotation; }
			set
			{
				if (!Mathf.Approximately(m_Rotation, value))
				{
					m_Rotation = value;
					SetDirty();
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
		/// Play shinning on enable.
		/// </summary>
		public bool play { get { return m_Play; } set { m_Play = value; } }

		/// <summary>
		/// Play shinning loop.
		/// </summary>
		public bool loop { get { return m_Loop; } set { m_Loop = value; } }

		/// <summary>
		/// Shinning duration.
		/// </summary>
		public float duration { get { return m_Duration; } set { m_Duration = Mathf.Max(value, 0.1f); } }

		/// <summary>
		/// Delay on loop.
		/// </summary>
		public float loopDelay { get { return m_LoopDelay; } set { m_LoopDelay = Mathf.Max(value, 0); } }

		/// <summary>
		/// Shinning update mode.
		/// </summary>
		public AnimatorUpdateMode updateMode { get { return m_UpdateMode; } set { m_UpdateMode = value; } }

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected override void OnEnable()
		{
			_time = 0;
//			graphic.material = effectMaterial;
			base.OnEnable();
		}


//		/// <summary>
//		/// This function is called when the behaviour becomes disabled () or inactive.
//		/// </summary>
//		protected override void OnDisable()
//		{
//			graphic.material = null;
//			base.OnDisable();
//		}

#if UNITY_EDITOR
		protected override Material GetMaterial()
		{
			return MaterialResolver.GetOrGenerateMaterialVariant(Shader.Find(shaderName));
		}

//		public void OnBeforeSerialize()
//		{
//		}
//
//		public void OnAfterDeserialize()
//		{
//			var obj = this;
//			EditorApplication.delayCall += () =>
//			{
//				if (Application.isPlaying || !obj)
//					return;
//
//				var mat = GetMaterial(shaderName);
//				if (m_EffectMaterial == mat && graphic.material == mat)
//					return;
//
//				graphic.material = m_EffectMaterial = mat;
//				EditorUtility.SetDirty(this);
//				EditorUtility.SetDirty(graphic);
//				EditorApplication.delayCall += AssetDatabase.SaveAssets;
//			};
//		}
//
//		public static Material GetMaterial(string shaderName)
//		{
//			string name = Path.GetFileName(shaderName);
//			return AssetDatabase.FindAssets("t:Material " + name)
//				.Select(x => AssetDatabase.GUIDToAssetPath(x))
//				.SelectMany(x => AssetDatabase.LoadAllAssetsAtPath(x))
//				.OfType<Material>()
//				.FirstOrDefault(x => x.name == name);
//		}
#endif

		/// <summary>
		/// Modifies the mesh.
		/// </summary>
		public override void ModifyMesh(VertexHelper vh)
		{
			if (!isActiveAndEnabled)
				return;

			// rect.
			Rect rect = m_EffectArea.GetEffectArea(vh, graphic);

			// rotation.
			float rad = rotation * Mathf.Deg2Rad;
			Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
			dir.x *= rect.height / rect.width;
			dir = dir.normalized;

			// Calculate vertex position.
			bool effectEachCharacter = graphic is Text && m_EffectArea == EffectArea.Character;

			UIVertex vertex = default(UIVertex);
			Vector2 nomalizedPos;
			Matrix2x3 localMatrix = new Matrix2x3(rect, dir.x, dir.y);	// Get local matrix.
			for (int i = 0; i < vh.currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref vertex, i);

				// Normalize vertex position by local matrix.
				nomalizedPos = localMatrix * vertex.position;

				// Normalize vertex position by local matrix.
				if (effectEachCharacter)
				{
					// Each characters.
					nomalizedPos = localMatrix * splitedCharacterPosition[i % 4];
				}
				else
				{
					nomalizedPos = localMatrix * vertex.position;
				}

				vertex.uv1 = new Vector2(
					Packer.ToFloat(Mathf.Clamp01(nomalizedPos.y), softness, width, brightness),
					Packer.ToFloat(location, highlight)
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
				m_Play = m_Loop;
				_time = m_Loop ? -m_LoopDelay : 0;
			}
		}

//		/// <summary>
//		/// Mark the UIEffect as dirty.
//		/// </summary>
//		void _SetDirty()
//		{
//			if (graphic)
//				graphic.SetVerticesDirty();
//		}
	}
}
