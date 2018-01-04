using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIEffectDialogDemo : MonoBehaviour {

	public void Open()
	{
		gameObject.SetActive(true);
		GetComponent<Animator>().SetTrigger("Open");
	}

	public void Close()
	{
		GetComponent<Animator>().SetTrigger("Close");
	}

	public void Closed()
	{
		gameObject.SetActive(false);
	}

	public void CaptureBackground()
	{
		GetComponentInChildren<UIEffectCapturedImage>().Capture();
	}
}
