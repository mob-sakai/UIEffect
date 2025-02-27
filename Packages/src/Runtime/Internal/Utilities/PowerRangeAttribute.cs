using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIEffectInternal
{
    [AttributeUsage(AttributeTargets.Field)]
    [Conditional("UNITY_EDITOR")]
    public sealed class PowerRangeAttribute : PropertyAttribute
    {
        public readonly float min;
        public readonly float max;
        public readonly float power;

        public PowerRangeAttribute(float min, float max, float power)
        {
            this.min = min;
            this.max = max;
            this.power = power;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(PowerRangeAttribute))]
    internal sealed class RangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect r, SerializedProperty property, GUIContent label)
        {
            var labelWidth = EditorGUIUtility.labelWidth;
            var attr = (PowerRangeAttribute)attribute;
            label = EditorGUI.BeginProperty(r, label, property);

            EditorGUI.BeginChangeCheck();
            var rSlider = new Rect(r.x + labelWidth + 1, r.y, r.width - labelWidth - 57, r.height);
            var powValue = Mathf.Log(property.floatValue, attr.power);
            var powMin = Mathf.Log(attr.min, attr.power);
            var powMax = Mathf.Log(attr.max, attr.power);
            powValue = GUI.HorizontalSlider(rSlider, powValue, powMin, powMax);
            if (EditorGUI.EndChangeCheck())
            {
                property.floatValue = Mathf.Clamp(Mathf.Pow(attr.power, powValue), attr.min, attr.max);
            }

            EditorGUI.BeginChangeCheck();
            EditorGUIUtility.labelWidth = r.width - 53;
            var newValue = EditorGUI.FloatField(r, label, property.floatValue);
            if (EditorGUI.EndChangeCheck())
            {
                property.floatValue = Mathf.Clamp(newValue, attr.min, attr.max);
            }

            EditorGUI.EndProperty();
            EditorGUIUtility.labelWidth = labelWidth;
        }
    }
#endif
}
