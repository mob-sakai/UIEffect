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
		[SerializeField] [Range(0, 1)] float m_Location = 0;
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

				if(!m_EffectMaterial)
				{
					m_EffectMaterial = GetMaterial(shaderName);
					EditorUtility.SetDirty(this);
					EditorApplication.delayCall +=AssetDatabase.SaveAssets;
				}
				if(obj.isActiveAndEnabled && graphic.material != m_EffectMaterial)
				{
					graphic.material = m_EffectMaterial;
					EditorUtility.SetDirty(graphic);
					EditorApplication.delayCall +=AssetDatabase.SaveAssets;
				}
			};
		}

		public static Material GetMaterial(string shaderName)
		{
			string name = Path.GetFileName (shaderName);
			return AssetDatabase.FindAssets("t:Material " + name)
				.Select(x => AssetDatabase.GUIDToAssetPath(x))
				.SelectMany(x => AssetDatabase.LoadAllAssetsAtPath(x))
				.OfType<Material>()
				.FirstOrDefault(x => x.name == name);
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
				vertex.uv1 = new Vector2 (_PackToFloat (x, y, location), 0);

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
	}
}
