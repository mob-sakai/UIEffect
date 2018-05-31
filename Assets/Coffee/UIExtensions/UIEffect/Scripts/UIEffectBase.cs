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
	public abstract class UIEffectBase : BaseMeshEffect, ISerializationCallbackReceiver
	{
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

		/// <summary>
		/// Raises the before serialize event.
		/// </summary>
		public virtual void OnBeforeSerialize()
		{
		}

		/// <summary>
		/// Raises the after deserialize event.
		/// </summary>
		public virtual void OnAfterDeserialize()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.delayCall += () => targetGraphic.material = m_EffectMaterial;
#endif
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected override void OnEnable()
		{
			targetGraphic.material = m_EffectMaterial;
			base.OnEnable();
		}

		/// <summary>
		/// This function is called when the behaviour becomes disabled () or inactive.
		/// </summary>
		protected override void OnDisable()
		{
			targetGraphic.material = null;
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
