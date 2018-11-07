using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIExtensions
{
	public class UIEffect_Demo_Dialog : MonoBehaviour
	{
		[SerializeField] Animator m_Animator = null;

		public void Open()
		{
			gameObject.SetActive(true);
		}

		public void Close()
		{
			m_Animator.SetTrigger("Close");
		}

		public void Closed()
		{
			gameObject.SetActive(false);
		}
	}
}