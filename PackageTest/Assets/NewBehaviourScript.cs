#define COM_UNITY_TEXTMESHPRO
#if COM_UNITY_TEXTMESHPRO
using TMPro;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using TMPro;

public class NewBehaviourScript : MonoBehaviour
{
	void Start ()
	{
#if TMP_PRESENT
		GetComponent<TMP_Text> ().text = "TMP_PRESENT is defined.";
#else
		GetComponent<TMP_Text> ().text = "TMP_PRESENT is NOT defined.";
#endif
	}
}
