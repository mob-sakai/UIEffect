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
	public class UIShiny : BaseMeshEffect
#if UNITY_EDITOR
		, ISerializationCallbackReceiver
#endif
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
		[SerializeField] Material m_EffectMaterial;


		//################################
		// Public Members.
		//################################
		/// <summary>
		/// Graphic affected by the UIEffect.
		/// </summary>
		new public Graphic graphic { get { return base.graphic; } }

		/// <summary>
		/// Location for shiny effect.
		/// </summary>
		public float location { get { return m_Location; } set { m_Location = Mathf.Clamp(value, 0, 1); _SetDirty(); } }

		/// <summary>
		/// Width for shiny effect.
		/// </summary>
		public float width { get { return m_Width; } set { m_Width = Mathf.Clamp(value, 0, 1); _SetDirty(); } }

		/// <summary>
		/// Softness for shiny effect.
		/// </summary>
		public float softness { get { return m_Softness; } set { m_Softness = Mathf.Clamp(value, 0.01f, 1); _SetDirty(); } }

		/// <summary>
		/// Alpha for shiny effect.
		/// </summary>
		[System.Obsolete ("Use brightness instead (UnityUpgradable) -> brightness")]
		public float alpha { get { return m_Brightness; } set { m_Brightness = Mathf.Clamp(value, 0, 1); _SetDirty(); } }

		/// <summary>
		/// Brightness for shiny effect.
		/// </summary>
		public float brightness { get { return m_Brightness; } set { m_Brightness = Mathf.Clamp(value, 0, 1); _SetDirty(); } }

		/// <summary>
		/// Highlight factor for shiny effect.
		/// </summary>
		public float highlight { get { return m_Highlight; } set { m_Highlight = Mathf.Clamp(value, 0, 1); _SetDirty(); } }

		/// <summary>
		/// Rotation for shiny effect.
		/// </summary>
		public float rotation
		{
			get
			{
				return m_Rotation;
			}
			set { if (!Mathf.Approximately(m_Rotation, value)) { m_Rotation = value; _SetDirty(); } }
		}

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
				if (Application.isPlaying || !obj)
					return;

				var mat = GetMaterial(shaderName);
				if(m_EffectMaterial == mat && graphic.material == mat)
					return;

				graphic.material = m_EffectMaterial = mat;
				EditorUtility.SetDirty(this);
				EditorUtility.SetDirty(graphic);
				EditorApplication.delayCall +=AssetDatabase.SaveAssets;
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

			// rotation.
			float rad = rotation * Mathf.Deg2Rad;
			Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
			dir.x *= rect.height / rect.width;
			dir = dir.normalized;

			// Calculate vertex position.
			UIVertex vertex = default(UIVertex);
			Vector2 nomalizedPos;
			Matrix2x3 localMatrix = new Matrix2x3(rect, dir.x, dir.y);	// Get local matrix.
			for (int i = 0; i < vh.currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref vertex, i);

				// Normalize vertex position by local matrix.
				nomalizedPos = localMatrix * vertex.position;

				vertex.uv1 = new Vector2(
					_PackToFloat(Mathf.Clamp01(nomalizedPos.y), softness, width, brightness),
					_PackToFloat(location, highlight)
				);

				vh.SetUIVertex(vertex, i);
			}
		}

		/// <summary>
		/// Play effect.
		/// </summary>
		public void Play()
		{
			Play(1);
		}

		/// <summary>
		/// Play effect.
		/// </summary>
		public void Play(float duration)
		{
			StopAllCoroutines();
			StartCoroutine(CoPlay(duration, AnimatorUpdateMode.Normal));
		}

		/// <summary>
		/// Play effect.
		/// </summary>
		public void Play(float duration, AnimatorUpdateMode updateMode)
		{
			StopAllCoroutines();
			StartCoroutine(CoPlay(duration, updateMode));
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

		IEnumerator CoPlay(float duration, AnimatorUpdateMode updateMode = AnimatorUpdateMode.Normal)
		{
			float time = 0;
			while (time < duration)
			{
				location = time / duration;
				time += updateMode == AnimatorUpdateMode.UnscaledTime
					? Time.unscaledDeltaTime
					: Time.deltaTime;
				yield return null;
			}
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

		/// <summary>
		/// Pack 2 low-precision [0-1] floats values to a float.
		/// Each value [0-1] has 4096 steps(12 bits).
		/// </summary>
		static float _PackToFloat(float x, float y)
		{
			const int PRECISION = (1 << 12) - 1;
			return (Mathf.FloorToInt(y * PRECISION) << 12)
				+ Mathf.FloorToInt(x * PRECISION);
		}


		/// <summary>
		/// Matrix2x3.
		/// </summary>
		struct Matrix2x3
		{
			public float m00, m01, m02, m10, m11, m12;

			public Matrix2x3(Rect rect, float cos, float sin)
			{
				const float center = 0.5f;
				float dx = -rect.xMin / rect.width - center;
				float dy = -rect.yMin / rect.height - center;
				m00 = cos / rect.width;
				m01 = -sin / rect.height;
				m02 = dx * cos - dy * sin + center;
				m10 = sin / rect.width;
				m11 = cos / rect.height;		
				m12 = dx * sin + dy * cos + center;
			}

			public static Vector2 operator*(Matrix2x3 m, Vector2 v)
			{
				return new Vector2(
					(m.m00 * v.x) + (m.m01 * v.y) + m.m02,
					(m.m10 * v.x) + (m.m11 * v.y) + m.m12
				);
			}
		}
	}
}
