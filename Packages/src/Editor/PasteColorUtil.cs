using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIEffects.Editors
{
    internal static class PasteColorUtil
    {
        internal const string SupportedFormats =
            "hex values (#RGB, #RRGGBB, #RGBA, #RRGGBBAA), Unity-supported color names, transparent, rgb(), and rgba()";

        internal static void PasteFromClipboard(SerializedProperty color)
        {
            if (!TryParseColor(GUIUtility.systemCopyBuffer, out var value))
            {
                EditorUtility.DisplayDialog("Paste Color",
                    $"Supported formats are {SupportedFormats}, or CSS background/background-color declarations containing one of these values.",
                    "OK");
                return;
            }

            color.colorValue = value;
        }

        internal static bool TryParseColor(string text, out Color color)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                color = default;
                return false;
            }

            if (TryExtractCssColorValue(text, out var cssColor))
            {
                text = cssColor;
            }

            text = text.Trim();
            if (text.Equals("transparent", StringComparison.OrdinalIgnoreCase))
            {
                color = Color.clear;
                return true;
            }

            if (ColorUtility.TryParseHtmlString(text, out color))
            {
                return true;
            }

            var openIndex = text.IndexOf('(');
            if (openIndex < 0 || text[text.Length - 1] != ')')
            {
                return false;
            }

            var function = text.Substring(0, openIndex).Trim();
            var isRgba = function.Equals("rgba", StringComparison.OrdinalIgnoreCase);
            if (!isRgba && !function.Equals("rgb", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var components = text.Substring(openIndex + 1, text.Length - openIndex - 2)
                .Split(new[] { ' ', '\t', '\r', '\n', ',', '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (components.Length != 3 && components.Length != 4)
            {
                return false;
            }

            if ((isRgba && components.Length != 4)
                || !TryParseRgbComponent(components[0], out var red)
                || !TryParseRgbComponent(components[1], out var green)
                || !TryParseRgbComponent(components[2], out var blue))
            {
                return false;
            }

            var alpha = 1f;
            if (components.Length == 4 && !TryParseAlphaComponent(components[3], out alpha))
            {
                return false;
            }

            color = new Color(red, green, blue, alpha);
            return true;
        }

        private static bool TryExtractCssColorValue(string text, out string value)
        {
            foreach (var declaration in text.Split(';'))
            {
                var separatorIndex = declaration.IndexOf(':');
                if (separatorIndex < 0)
                {
                    continue;
                }

                var property = declaration.Substring(0, separatorIndex).Trim();
                var commentEndIndex = property.LastIndexOf("*/", StringComparison.Ordinal);
                if (commentEndIndex >= 0)
                {
                    property = property.Substring(commentEndIndex + 2).Trim();
                }

                if (property.Equals("background", StringComparison.OrdinalIgnoreCase)
                    || property.Equals("background-color", StringComparison.OrdinalIgnoreCase))
                {
                    value = declaration.Substring(separatorIndex + 1).Trim();
                    return true;
                }
            }

            value = null;
            return false;
        }

        private static bool TryParseRgbComponent(string text, out float value)
        {
            var isPercentage = text.EndsWith("%", StringComparison.Ordinal);
            if (!float.TryParse(isPercentage ? text.Substring(0, text.Length - 1) : text,
                    NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                || float.IsNaN(value) || float.IsInfinity(value))
            {
                return false;
            }

            value = Mathf.Clamp01(isPercentage ? value / 100f : value / 255f);
            return true;
        }

        private static bool TryParseAlphaComponent(string text, out float value)
        {
            var isPercentage = text.EndsWith("%", StringComparison.Ordinal);
            if (!float.TryParse(isPercentage ? text.Substring(0, text.Length - 1) : text,
                    NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                || float.IsNaN(value) || float.IsInfinity(value))
            {
                return false;
            }

            value = Mathf.Clamp01(isPercentage ? value / 100f : value);
            return true;
        }
    }
}
