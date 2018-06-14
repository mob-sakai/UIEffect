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
		protected static readonly Rect rectForCharacter = new Rect(0, 0, 1, 1);
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

		/// <summary>
		/// Raises the validate event.
		/// </summary>
		protected override void OnValidate ()
		{
			base.OnValidate ();
#if UNITY_EDITOR
			UnityEditor.EditorApplication.delayCall += () => UpdateMaterial(false);
#endif
		}

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
			UnityEditor.EditorApplication.delayCall += () => UpdateMaterial(true);
#endif
		}

#if UNITY_EDITOR
		/// <summary>
		/// Updates the material.
		/// </summary>
		/// <param name="ignoreInPlayMode">If set to <c>true</c> ignore in play mode.</param>
		protected void UpdateMaterial(bool ignoreInPlayMode)
		{
			if(!this || ignoreInPlayMode && Application.isPlaying)
			{
				return;
			}

			var mat =  GetMaterial();
			if (m_EffectMaterial != mat || targetGraphic.material != mat)
			{
				targetGraphic.material = m_EffectMaterial = mat;
				UnityEditor.EditorUtility.SetDirty(this);
				UnityEditor.EditorUtility.SetDirty(targetGraphic);
			}
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

		/// <summary>
		/// Gets effect for area.
		/// </summary>
		protected Rect GetEffectArea(VertexHelper vh, EffectArea area)
		{
			switch(area)
			{
				case EffectArea.RectTransform: return graphic.rectTransform.rect;
				case EffectArea.Character: return rectForCharacter;
				case EffectArea.Fit:
					{
						// Fit to contents.
						Rect rect = default(Rect);
						UIVertex vertex = default(UIVertex);
						rect.xMin = rect.yMin = float.MaxValue;
						rect.xMax = rect.yMax = float.MinValue;
						for (int i = 0; i < vh.currentVertCount; i++)
						{
							vh.PopulateUIVertex(ref vertex, i);
							rect.xMin = Mathf.Min(rect.xMin, vertex.position.x);
							rect.yMin = Mathf.Min(rect.yMin, vertex.position.y);
							rect.xMax = Mathf.Max(rect.xMax, vertex.position.x);
							rect.yMax = Mathf.Max(rect.yMax, vertex.position.y);
						}
						return rect;
					}
				default: return graphic.rectTransform.rect;
			}
		}
	}
}
