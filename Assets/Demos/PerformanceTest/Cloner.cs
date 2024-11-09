using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cloner : MonoBehaviour
{
    [SerializeField]
    private Text m_Counter;

    [SerializeField]
    private GameObject m_Origin;

    [SerializeField]
    private Transform m_Parent;

    private readonly List<GameObject> _instances = new List<GameObject>();

    private void Start()
    {
        m_Origin.SetActive(false);
        m_Counter.text = _instances.Count.ToString();
    }

    public void Clone(int num)
    {
        for (var i = 0; i < num; i++)
        {
            var go = Instantiate(m_Origin, m_Parent);
            var t = go.transform as RectTransform;
            t.anchorMax = t.anchorMin = new Vector2(UnityEngine.Random.value, UnityEngine.Random.value);
            t.anchoredPosition3D = Vector3.zero;
            go.SetActive(true);
            _instances.Add(go);
        }

        m_Counter.text = _instances.Count.ToString();
    }

    public void Clear()
    {
        foreach (var go in _instances)
        {
            Destroy(go);
        }

        _instances.Clear();
        m_Counter.text = _instances.Count.ToString();
    }
}
