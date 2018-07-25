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
			Mono = 1,
			Cutoff = 2,
		}


		//################################
		// Serialize Members.
		//################################
		[SerializeField] EffectMode m_EffectMode;
		[SerializeField][Range(0, 1)] float m_EffectFactor = 1;


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
		/// Effect mode.
		/// </summary>
		public EffectMode effectMode { get { return m_EffectMode; } }

		/// <summary>
		/// Modifies the mesh.
		/// </summary>
		public override void ModifyMesh(VertexHelper vh)
		{
			if (!isActiveAndEnabled || m_EffectMode == EffectMode.None)
			{
				return;
			}

			UIVertex vt;
			vh.GetUIVertexStream(tempVerts);

			// Pack effect prameters.
			Vector2 factor = new Vector2(effectFactor, 0);

			// Set prameters to vertex.
			for (int i = 0; i < tempVerts.Count; i++)
			{
				vt = tempVerts[i];
				vt.uv1 = factor;
				tempVerts[i] = vt;
			}

			vh.Clear();
			vh.AddUIVertexTriangleStream(tempVerts);

			tempVerts.Clear();
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
	}
}
