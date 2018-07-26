using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIExtensions
{
	/// <summary>
	/// Abstract effect base for UI.
	/// </summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(Graphic))]
	[DisallowMultipleComponent]
	public abstract class UIEffectBase : BaseMeshEffect
	{
		protected static readonly Vector2[] splitedCharacterPosition = { Vector2.up, Vector2.one, Vector2.right, Vector2.zero };
		protected static readonly List<UIVertex> tempVerts = new List<UIVertex>();

		[SerializeField] protected Material m_EffectMaterial;

		/// <summary>
		/// Gets target graphic for effect.
		/// </summary>
		public Graphic targetGraphic { get { return graphic; } }

		/// <summary>
		/// Gets material for effect.
		/// </summary>
		public Material effectMaterial { get { return m_EffectMaterial; } }

#if UNITY_EDITOR
		protected override void Reset()
		{
			OnValidate();
		}

		/// <summary>
		/// Raises the validate event.
		/// </summary>
		protected override void OnValidate ()
		{
			var mat = GetMaterial();
			if (m_EffectMaterial != mat)
			{
				m_EffectMaterial = mat;
				UnityEditor.EditorUtility.SetDirty(this);
			}
			ModifyMaterial();
			SetDirty();
		}

		/// <summary>
		/// Gets the material.
		/// </summary>
		/// <returns>The material.</returns>
		protected virtual Material GetMaterial()
		{
			return null;
		}
#endif

		/// <summary>
		/// Modifies the material.
		/// </summary>
		public virtual void ModifyMaterial()
		{
			targetGraphic.material = isActiveAndEnabled ? m_EffectMaterial : null;
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected override void OnEnable()
		{
			ModifyMaterial();
			base.OnEnable();
		}

		/// <summary>
		/// This function is called when the behaviour becomes disabled () or inactive.
		/// </summary>
		protected override void OnDisable()
		{
			ModifyMaterial();
			base.OnDisable();
		}

		/// <summary>
		/// Mark the UIEffect as dirty.
		/// </summary>
		protected void SetDirty()
		{
			if (targetGraphic)
			{
				targetGraphic.SetVerticesDirty();
			}
		}
	}
}
