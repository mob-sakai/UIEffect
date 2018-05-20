using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIExtensions
{
	public class Demo_UIEffect : MonoBehaviour
	{
		[SerializeField] RectMask2D mask;

		// Use this for initialization
		void Start()
		{
			if (mask)
			{
				mask.enabled = true;
			}
		}
		
		public void SetTimeScale(float scale)
		{
			Time.timeScale = scale;
		}
	}
}