using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;

namespace Coffee.UIExtensions
{
	/// <summary>
	/// Abstract effect base for UI.
	/// </summary>
	[RequireComponent(typeof(Graphic))]
	[DisallowMultipleComponent]
	public abstract class UIEffectBase : BaseMeshEffect, IParameterTexture
#if UNITY_EDITOR
	, ISerializationCallbackReceiver
#endif
	{
		protected static readonly Vector2[] splitedCharacterPosition = { Vector2.up, Vector2.one, Vector2.right, Vector2.zero };
		protected static readonly List<UIVertex> tempVerts = new List<UIVertex>();

		[HideInInspector]
		[SerializeField] int m_Version;
		[SerializeField] protected Material m_EffectMaterial;

		/// <summary>
		/// Gets or sets the parameter index.
		/// </summary>
		public int parameterIndex { get; set; }

		/// <summary>
		/// Gets the parameter texture.
		/// </summary>
		public virtual ParameterTexture ptex { get { return null; } }

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
			m_Version = 300;
			OnValidate();
		}

		/// <summary>
		/// Raises the validate event.
		/// </summary>
		protected override void OnValidate()
		{
			var mat = GetMaterial();
			if (m_EffectMaterial != mat)
			{
				m_EffectMaterial = mat;
				UnityEditor.EditorUtility.SetDirty(this);
			}

			ModifyMaterial();
			targetGraphic.SetVerticesDirty();
			SetDirty();
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			UnityEditor.EditorApplication.delayCall += UpgradeIfNeeded;
		}

		protected bool IsShouldUpgrade(int expectedVersion)
		{
			if (m_Version < expectedVersion)
			{
				Debug.LogFormat(gameObject, "<b>{0}({1})</b> has been upgraded: <i>version {2} -> {3}</i>", name, GetType().Name, m_Version, expectedVersion);
				m_Version = expectedVersion;

				//UnityEditor.EditorApplication.delayCall += () =>
				{
					UnityEditor.EditorUtility.SetDirty(this);
					if (!Application.isPlaying && gameObject && gameObject.scene.IsValid())
					{
						UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
					}
				}
				;
				return true;
			}
			return false;
		}

		protected virtual void UpgradeIfNeeded()
		{
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
			if (ptex != null)
			{
				ptex.Register(this);
			}
			ModifyMaterial();
			targetGraphic.SetVerticesDirty();
			SetDirty();
		}

		/// <summary>
		/// This function is called when the behaviour becomes disabled () or inactive.
		/// </summary>
		protected override void OnDisable()
		{
			ModifyMaterial();
			targetGraphic.SetVerticesDirty();
			if (ptex != null)
			{
				ptex.Unregister(this);
			}
		}

		/// <summary>
		/// Mark the UIEffect as dirty.
		/// </summary>
		protected virtual void SetDirty()
		{
			targetGraphic.SetVerticesDirty();
		}

		protected override void OnDidApplyAnimationProperties()
		{
			SetDirty();
		}
	}
}
