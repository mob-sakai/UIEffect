using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIExtensions
{
	public class UIEffect_Demo : MonoBehaviour
	{
		[SerializeField] RectMask2D mask = null;

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

		public void Open(Animator anim)
		{
			anim.GetComponentInChildren<UIEffectCapturedImage>().Capture();
			anim.gameObject.SetActive(true);
			anim.SetTrigger("Open");
		}

		public void Close(Animator anim)
		{
			anim.SetTrigger("Close");
		}

		public void Capture(Animator anim)
		{
			anim.GetComponentInChildren<UIEffectCapturedImage>().Capture();
			anim.SetTrigger("Capture");
		}

		public void SetCanvasOverlay(bool isOverlay)
		{
			GetComponent<Canvas> ().renderMode = isOverlay ? RenderMode.ScreenSpaceOverlay : RenderMode.ScreenSpaceCamera;
		}
	}
}