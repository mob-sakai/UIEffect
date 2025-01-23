using System;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIEffectInternal
{
    [Serializable]
    public struct MinMax01
    {
        [SerializeField]
        private float m_Min;

        [SerializeField]
        private float m_Max;

        public MinMax01(float min, float max)
        {
            m_Min = Mathf.Clamp01(Mathf.Min(min, max));
            m_Max = Mathf.Clamp01(Mathf.Max(min, max));
        }

        public float min
        {
            get => m_Min;
            set
            {
                m_Min = Mathf.Clamp01(value);
                m_Max = Mathf.Max(value, m_Max);
            }
        }

        public float max
        {
            get => m_Max;
            set
            {
                m_Max = Mathf.Clamp01(value);
                m_Min = Mathf.Min(value, m_Min);
            }
        }

        public float average => (m_Max + m_Min) * 0.5f;

        public bool Approximately(MinMax01 other)
        {
            return Mathf.Approximately(m_Min, other.m_Min) && Mathf.Approximately(m_Max, other.m_Max);
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(MinMax01))]
    public class MinMaxRangeDrawer : PropertyDrawer
    {
        private const float k_NumWidth = 50;
        private const float k_Space = 5;
        private SerializedProperty _max;
        private SerializedProperty _min;

        private static bool IsSingleLine(GUIContent label)
        {
            return EditorGUIUtility.wideMode || label == null || string.IsNullOrEmpty(label.text);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return IsSingleLine(label) ? 18 : 36;
        }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, prop);

            if (_min == null)
            {
                prop.Next(true);
                _min = prop.Copy();
                prop.Next(true);
                _max = prop.Copy();
            }

            if (IsSingleLine(label))
            {
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            }
            else
            {
                EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                var indent = (EditorGUI.indentLevel + 1) * 15f;
                position = new Rect(position.x + indent, position.y + 18, position.width - indent, 16);
            }

            var min = _min.floatValue;
            var max = _max.floatValue;
            if (Draw(position, ref min, ref max))
            {
                _min.floatValue = min;
                _max.floatValue = max;
            }

            EditorGUI.EndProperty();
        }

        public static bool Draw(Rect position, ref float minValue, ref float maxValue)
        {
            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUI.BeginChangeCheck();

            var rect = new Rect(position.x, position.y, k_NumWidth, position.height);
            minValue = Mathf.Clamp(EditorGUI.FloatField(rect, minValue), 0, maxValue);

            rect.x += rect.width + k_Space;
            rect.width = position.width - k_NumWidth * 2 - k_Space * 2;
            EditorGUI.MinMaxSlider(rect, ref minValue, ref maxValue, 0, 1);

            rect.x += rect.width + k_Space;
            rect.width = k_NumWidth;
            maxValue = Mathf.Clamp(EditorGUI.FloatField(rect, maxValue), minValue, 1);

            EditorGUI.indentLevel = indentLevel;
            return EditorGUI.EndChangeCheck();
        }

        public static bool DrawLayout(GUIContent label, ref float minValue, ref float maxValue)
        {
            Rect position;
            if (IsSingleLine(label))
            {
                position = EditorGUILayout.GetControlRect(true, 18f);
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            }
            else
            {
                position = EditorGUILayout.GetControlRect(true, 36f);
                EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                var indent = (EditorGUI.indentLevel + 1) * 15f;
                position = new Rect(position.x + indent, position.y + 18, position.width - indent, 16);
            }

            return Draw(position, ref minValue, ref maxValue);
        }
    }
#endif
}
