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
	/// UIEffect.
	/// </summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(Graphic))]
	[DisallowMultipleComponent]
	public class UICustomEffect : UIEffectBase
	{
		//################################
		// Serialize Members.
		//################################
		[SerializeField] Vector4 m_CustomFactor1 = new Vector4();
		[SerializeField] Vector4 m_CustomFactor2 = new Vector4();

		//################################
		// Public Members.
		//################################
		/// <summary>
		/// Custom effect factor 1.
		/// </summary>
		public Vector4 customFactor1 { get { return m_CustomFactor1; } set { m_CustomFactor1 = value; SetDirty(); } }

		/// <summary>
		/// Custom effect factor 2.
		/// </summary>
		public Vector4 customFactor2 { get { return m_CustomFactor2; } set { m_CustomFactor2 = value; SetDirty(); } }

		/// <summary>
		/// Modifies the mesh.
		/// </summary>
		public override void ModifyMesh(VertexHelper vh)
		{
			if (!isActiveAndEnabled)
			{
				return;
			}

			UIVertex vt;
			vh.GetUIVertexStream(tempVerts);

			//================================
			// Effect modify original vertices.
			//================================
			{
				// Pack some effect factors to 1 float.
				Vector2 factor = new Vector2(
					Packer.ToFloat(m_CustomFactor1),
					Packer.ToFloat(m_CustomFactor2)
				);

				for (int i = 0; i < tempVerts.Count; i++)
				{
					vt = tempVerts[i];

					// Set prameters to vertex.
					vt.uv1 = factor;
					tempVerts[i] = vt;
				}
			}

			vh.Clear();
			vh.AddUIVertexTriangleStream(tempVerts);

			tempVerts.Clear();
		}
	}
}
