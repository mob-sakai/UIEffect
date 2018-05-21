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
		public float location { get { return m_Location; } set { m_Location = Mathf.Clamp(value, 0, 1); _SetDirty(); } }

		/// <summary>
		/// Edge width.
		/// </summary>
		public float width { get { return m_Width; } set { m_Width = Mathf.Clamp(value, 0, 1); _SetDirty(); } }

		/// <summary>
		/// Edge softness.
		/// </summary>
		public float softness { get { return m_Softness; } set { m_Softness = Mathf.Clamp(value, 0, 1); _SetDirty(); } }

		/// <summary>
		/// Edge color.
		/// </summary>
		public Color color { get { return m_Color; } set { m_Color = value; _SetDirty(); } }

		/// <summary>
		/// Color effect mode.
		/// </summary>
		public UIEffect.ColorMode colorMode { get { return m_ColorMode; } }

		/// <summary>
		/// Effect material.
		/// </summary>
		public virtual Material effectMaterial { get { return m_EffectMaterial; } }

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected override void OnEnable()
		{
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

			if (!IsActive())
				return;

			// rect.
			Rect rect = graphic.rectTransform.rect;

			// Calculate vertex position.
			UIVertex vertex = default(UIVertex);
			for (int i = 0; i < vh.currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref vertex, i);

				var x = Mathf.Clamp01 (vertex.position.x / rect.width + 0.5f);
				var y = Mathf.Clamp01 (vertex.position.y / rect.height + 0.5f);
				vertex.uv1 = new Vector2 (_PackToFloat (x, y, location, m_Width), _PackToFloat(m_Color.r, m_Color.g, m_Color.b, m_Softness));

				vh.SetUIVertex(vertex, i);
			}
		}

		//################################
		// Private Members.
		//################################
		/// <summary>
		/// Mark the UIEffect as dirty.
		/// </summary>
		void _SetDirty()
		{
			if(graphic)
				graphic.SetVerticesDirty();
		}

		/// <summary>
		/// Pack 3 low-precision [0-1] floats values to a float.
		/// Each value [0-1] has 256 steps(8 bits).
		/// </summary>
		static float _PackToFloat(float x, float y, float z)
		{
			const int PRECISION = (1 << 8) - 1;
			return (Mathf.FloorToInt(z * PRECISION) << 16)
			+ (Mathf.FloorToInt(y * PRECISION) << 8)
			+ Mathf.FloorToInt(x * PRECISION);
		}

		/// <summary>
		/// Pack 4 low-precision [0-1] floats values to a float.
		/// Each value [0-1] has 64 steps(6 bits).
		/// </summary>
		static float _PackToFloat(float x, float y, float z, float w)
		{
			const int PRECISION = (1 << 6) - 1;
			return (Mathf.FloorToInt(w * PRECISION) << 18)
			+ (Mathf.FloorToInt(z * PRECISION) << 12)
			+ (Mathf.FloorToInt(y * PRECISION) << 6)
			+ Mathf.FloorToInt(x * PRECISION);
		}
	}
}
