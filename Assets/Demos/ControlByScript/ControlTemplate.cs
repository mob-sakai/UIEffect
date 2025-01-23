using System;
using System.Linq;
using UnityEngine;
using Coffee.UIEffects;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ControlTemplate : MonoBehaviour
{
    [SerializeField]
    private Text m_Label;

    [SerializeField]
    private Dropdown m_Dropdown;

    [SerializeField]
    private Slider m_Slider;

    [SerializeField]
    private Toggle m_Toggle;

    [SerializeField]
    private RawImage m_RawImage;

    public ControlTemplate Instantiate()
    {
        var go = Instantiate(gameObject, transform.parent, false);
        return go.GetComponent<ControlTemplate>();
    }

    public void InitAsDropDown(string label, Type enumType, int value, UnityAction<int> onValueChanged)
    {
        m_Label.text = label;

        m_Dropdown.ClearOptions();
        m_Dropdown.AddOptions(Enum.GetNames(enumType).ToList());
        m_Dropdown.onValueChanged.RemoveAllListeners();
        m_Dropdown.onValueChanged.AddListener(onValueChanged);
        m_Dropdown.SetValueWithoutNotify(value);

        // m_Dropdown.gameObject.SetActive(false);
        m_Slider.gameObject.SetActive(false);
        m_Toggle.gameObject.SetActive(false);
        m_RawImage.gameObject.SetActive(false);
    }


    private static readonly (string name, Color color)[] s_Colors = new (string, Color)[]
    {
        ("-", Color.clear),
        ("White", Color.white),
        ("Red", Color.red),
        ("Green", Color.green),
        ("Blue", Color.blue),
        ("Yellow", Color.yellow),
        ("Cyan", Color.cyan),
        ("Magenta", Color.magenta),
        ("Black", Color.black)
    };

    public void InitAsColorDropDown(string label, Color value, UnityAction<Color> onValueChanged)
    {
        m_Label.text = label;

        m_Dropdown.ClearOptions();
        m_Dropdown.AddOptions(s_Colors.Select(x => x.name).ToList());
        m_Dropdown.onValueChanged.RemoveAllListeners();
        m_Dropdown.onValueChanged.AddListener(x =>
        {
            m_RawImage.color = s_Colors[x].color;
            onValueChanged(s_Colors[x].color);
        });
        m_RawImage.color = value;
        m_Dropdown.SetValueWithoutNotify(0);

        // m_Dropdown.gameObject.SetActive(false);
        m_Slider.gameObject.SetActive(false);
        m_Toggle.gameObject.SetActive(false);
        m_RawImage.gameObject.SetActive(true);
    }

    public void InitAsSlider(string label, float value, UnityAction<float> onValueChanged)
    {
        m_Label.text = label;

        m_Slider.onValueChanged.RemoveAllListeners();
        m_Slider.onValueChanged.AddListener(onValueChanged);
        m_Slider.SetValueWithoutNotify(value);

        m_Dropdown.gameObject.SetActive(false);
        // m_Slider.gameObject.SetActive(false);
        m_Toggle.gameObject.SetActive(false);
        m_RawImage.gameObject.SetActive(false);
    }

    public void InitAsToggle(string label, bool value, UnityAction<bool> onValueChanged)
    {
        m_Label.text = label;

        m_Toggle.onValueChanged.RemoveAllListeners();
        m_Toggle.onValueChanged.AddListener(onValueChanged);
        m_Toggle.SetIsOnWithoutNotify(value);

        m_Dropdown.gameObject.SetActive(false);
        m_Slider.gameObject.SetActive(false);
        // m_Toggle.gameObject.SetActive(false);
        m_RawImage.gameObject.SetActive(false);
    }
}
