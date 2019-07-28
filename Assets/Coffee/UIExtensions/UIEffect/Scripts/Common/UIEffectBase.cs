using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIExtensions
{
	/// <summary>
	/// Abstract effect base for UI.
	/// </summary>
	[DisallowMultipleComponent]
	public abstract class UIEffectBase : BaseMeshEffect, IParameterTexture, IMaterialModifier
#if UNITY_EDITOR
	, ISerializationCallbackReceiver
#endif
	{
		protected static readonly Vector2[] splitedCharacterPosition = { Vector2.up, Vector2.one, Vector2.right, Vector2.zero };
		protected static readonly List<UIVertex> tempVerts = new List<UIVertex>();
		protected static readonly StringBuilder stringBuilder = new StringBuilder();

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

		public virtual Hash128 GetMaterialHash(Material material)
		{
			return new Hash128();
		}

        public virtual Material GetModifiedMaterial(Material baseMaterial)
        {
			if(!isActiveAndEnabled)
			{
				return baseMaterial;
			}

			var oldHash = _effectMaterialHash;
			_effectMaterialHash = GetMaterialHash(baseMaterial);
			var modifiedMaterial = baseMaterial;
			if(_effectMaterialHash.isValid)
			{
				modifiedMaterial = MaterialRepository.Register(baseMaterial, _effectMaterialHash, ModifyMaterial);
			}
			MaterialRepository.Unregister(oldHash);

			return modifiedMaterial;
        }

		protected bool isTMProMobile (Material material)
		{
			return material && material.shader && material.shader.name.StartsWith ("TextMeshPro/Mobile/", StringComparison.Ordinal);
		}

		public virtual void ModifyMaterial(Material material)
		{
			Debug.Log("ModifyMaterial PTEX!!! " + ptex);
			if(isActiveAndEnabled && ptex != null)
				ptex.RegisterMaterial (material);
		}

		protected void SetShaderVariants(Material material, params object[] variants)
		{
			// Set shader keywords as variants
			var keywords = variants.Where(x => 0 < (int)x)
				.Select(x => x.ToString().ToUpper())
				.Concat(material.shaderKeywords)
				.ToArray();
			material.shaderKeywords = keywords;

			// Add variant name
			stringBuilder.Length = 0;
			stringBuilder.Append(Path.GetFileName(material.shader.name) );
			foreach(var keyword in keywords)
			{
				stringBuilder.Append("-");
				stringBuilder.Append(keyword);
			}
			material.name += stringBuilder.ToString();
		}

		Hash128 _effectMaterialHash;

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
			base.OnValidate ();

			var mat = GetMaterial();
			if (m_EffectMaterial != mat)
			{
				m_EffectMaterial = mat;
				UnityEditor.EditorUtility.SetDirty(this);
			}

			ModifyMaterial();
			SetVerticesDirty ();
			SetEffectDirty ();
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
			base.OnEnable ();

			if (ptex != null)
			{
				ptex.Register(this);
			}
			ModifyMaterial();
			
			graphic.SetMaterialDirty();
			graphic.SetVerticesDirty();
			SetEffectDirty();
		}

		/// <summary>
		/// This function is called when the behaviour becomes disabled () or inactive.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable ();

			graphic.SetMaterialDirty();
			graphic.SetVerticesDirty();
			if (ptex != null)
			{
				ptex.Unregister(this);
			}

			MaterialRepository.Unregister(_effectMaterialHash);
			_effectMaterialHash = new Hash128();
		}

		/// <summary>
		/// Mark the UIEffect as dirty.
		/// </summary>
		protected virtual void SetEffectDirty()
		{
			SetVerticesDirty();
		}

		protected override void OnDidApplyAnimationProperties()
		{
			SetEffectDirty();
		}
	}
}
