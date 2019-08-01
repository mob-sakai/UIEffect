using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Coffee.UIExtensions
{
	/// <summary>
	/// Dissolve effect for uGUI.
	/// </summary>
	[ExecuteInEditMode]
	public class UIEffectMaterialResolver : MonoBehaviour, IMaterialModifier, IMeshModifier
	{
		[ContextMenu("test")]
		void test()
		{
			GetComponent<Graphic> ().SetMaterialDirty ();
			GetComponent<Graphic> ().SetVerticesDirty ();
		}

		Hash128 _effectMaterialHash;


		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected void OnEnable ()
		{
			//GetComponent<Graphic> ().SetMaterialDirty ();
			//GetComponent<Graphic> ().SetVerticesDirty ();
		}

		/// <summary>
		/// This function is called when the behaviour becomes disabled () or inactive.
		/// </summary>
		protected void OnDisable ()
		{
			MaterialRepository.Unregister (_effectMaterialHash);
			_effectMaterialHash = new Hash128 ();

			//GetComponent<Graphic> ().SetMaterialDirty ();
			//GetComponent<Graphic> ().SetVerticesDirty ();
		}

		protected Hash128 GetMaterialHash (Material material)
		{
			return new Hash128 ();
		}

		public Material GetModifiedMaterial (Material baseMaterial)
		{
			if (!isActiveAndEnabled)
			{
				return baseMaterial;
			}

			var modifier = transform.parent.GetComponent<UIEffectBase> ();


			var oldHash = _effectMaterialHash;
			_effectMaterialHash = modifier.GetMaterialHash (baseMaterial);
			var modifiedMaterial = baseMaterial;
			if (_effectMaterialHash.isValid)
			{
				modifiedMaterial = MaterialRepository.Register (baseMaterial, _effectMaterialHash, modifier.ModifyMaterial);
			}
			MaterialRepository.Unregister (oldHash);

			return modifiedMaterial;
		}

		//public Material GetModifiedMaterial2 (Material baseMaterial)
		//{
		//		Debug.Log ("UIEffectMaterialResolver 0");
		//	if (!isActiveAndEnabled)
		//		return baseMaterial;

		//	var parent = transform.parent;
		//	if (!parent)
		//		return baseMaterial;

		//	var modifier = parent.GetComponent<UIEffectBase> ();
		//	if (modifier == null)
		//	{
		//		Debug.Log ("UIEffectMaterialResolver modifier is null");
		//		return baseMaterial;
		//	}

		//	Debug.Log ("UIEffectMaterialResolver !!! " + modifier);
		//	return modifier.GetModifiedMaterial (baseMaterial);
		//}

		public void ModifyMesh (Mesh mesh)
		{
		}

		public void ModifyMesh (VertexHelper verts)
		{
			if (!isActiveAndEnabled)
			{
				return;
			}

			var modifier = transform.parent.GetComponent<UIEffectBase> ();
			modifier.ModifyMesh (verts);
		}
	}
}