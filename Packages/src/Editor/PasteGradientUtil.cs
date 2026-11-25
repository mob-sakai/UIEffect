using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIEffects.Editors
{
    internal static class PasteGradientUtil
    {
        private struct GradientStop
        {
            public Color color;
            public float? position;
        }

        internal static void PasteFromClipboard(SerializedProperty property,
            Action<SerializedProperty, Gradient> setGradient,
            SerializedProperty gradationMode,
            SerializedProperty gradationRotation,
            SerializedProperty gradationOffset,
            SerializedProperty gradationScale,
            SerializedProperty gradationWrapMode,
            SerializedProperty gradationReverse)
        {
            var clipboard = GUIUtility.systemCopyBuffer;
            if (!TryParseCssGradient(clipboard, out var value, out var mode, out var rotation))
            {
                EditorUtility.DisplayDialog("Paste Gradient",
                    "The clipboard does not contain a supported CSS linear-gradient(...) or radial-gradient(...) value.",
                    "OK");
                return;
            }

            value.mode = GradientMode.Blend;
            setGradient(property, value);
            gradationMode.intValue = (int)mode;
            gradationWrapMode.intValue = (int)(mode == GradationMode.RadialGradient
                ? TextureWrapMode.Clamp
                : TextureWrapMode.Repeat);
            if (rotation.HasValue)
            {
                gradationRotation.floatValue = rotation.Value;
            }

            gradationOffset.floatValue = 0;
            gradationScale.floatValue = 1;
            gradationReverse.boolValue = false;
        }

        private static bool TryParseCssGradient(string text, out Gradient gradient, out GradationMode mode,
            out float? rotation)
        {
            gradient = null;
            mode = GradationMode.None;
            rotation = null;
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            text = text.Trim();
            var linearIndex = text.IndexOf("linear-gradient(", StringComparison.OrdinalIgnoreCase);
            var radialIndex = text.IndexOf("radial-gradient(", StringComparison.OrdinalIgnoreCase);
            var functionIndex = linearIndex < 0
                ? radialIndex
                : radialIndex < 0
                    ? linearIndex
                    : Math.Min(linearIndex, radialIndex);
            if (functionIndex < 0)
            {
                return false;
            }

            text = text.Substring(functionIndex);
            var openIndex = text.IndexOf('(');
            if (openIndex < 0)
            {
                return false;
            }

            var functionName = text.Substring(0, openIndex).Trim();
            var isRadial = functionName.Equals("radial-gradient", StringComparison.OrdinalIgnoreCase);
            if (!isRadial && !functionName.Equals("linear-gradient", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            mode = isRadial ? GradationMode.RadialGradient : GradationMode.AngleGradient;

            var depth = 0;
            var closeIndex = -1;
            for (var i = openIndex; i < text.Length; i++)
            {
                if (text[i] == '(')
                {
                    depth++;
                }
                else if (text[i] == ')')
                {
                    depth--;
                    if (depth < 0)
                    {
                        return false;
                    }

                    if (depth == 0)
                    {
                        closeIndex = i;
                        break;
                    }
                }
            }

            if (closeIndex < 0)
            {
                return false;
            }

            var arguments = new List<string>();
            if (!TrySplitGradientArguments(text.Substring(openIndex + 1, closeIndex - openIndex - 1), arguments)
                || arguments.Count < 2)
            {
                return false;
            }

            var firstStopIndex = 0;
            if (!TryParseGradientStop(arguments[0], out _)
                && (isRadial || IsGradientDirection(arguments[0])))
            {
                firstStopIndex = 1;
                if (!isRadial && TryParseLinearGradientAngle(arguments[0], out var cssAngle))
                {
                    rotation = Mathf.Repeat(cssAngle - 90, 360);
                }
            }

            var stops = new List<GradientStop>();
            for (var i = firstStopIndex; i < arguments.Count; i++)
            {
                if (!TryParseGradientStop(arguments[i], out var stop))
                {
                    return false;
                }

                stops.Add(stop);
            }

            if (stops.Count < 2)
            {
                return false;
            }

            if (!stops[0].position.HasValue)
            {
                stops[0] = SetStopPosition(stops[0], 0);
            }

            var lastStopIndex = stops.Count - 1;
            if (!stops[lastStopIndex].position.HasValue)
            {
                stops[lastStopIndex] = SetStopPosition(stops[lastStopIndex], 1);
            }

            var previousPositionIndex = 0;
            for (var i = 1; i < stops.Count; i++)
            {
                if (!stops[i].position.HasValue)
                {
                    continue;
                }

                var previousPosition = stops[previousPositionIndex].position.Value;
                var currentPosition = stops[i].position.Value;
                var interval = i - previousPositionIndex;
                for (var j = 1; j < interval; j++)
                {
                    var position = Mathf.Lerp(previousPosition, currentPosition, (float)j / interval);
                    stops[previousPositionIndex + j] =
                        SetStopPosition(stops[previousPositionIndex + j], position);
                }

                previousPositionIndex = i;
            }

            var colorKeys = new GradientColorKey[stops.Count];
            var alphaKeys = new GradientAlphaKey[stops.Count];
            var previousTime = 0f;
            for (var i = 0; i < stops.Count; i++)
            {
                var stop = stops[i];
                var time = Mathf.Clamp01(stop.position.Value);
                time = Mathf.Max(previousTime, time);
                colorKeys[i] = new GradientColorKey(new Color(stop.color.r, stop.color.g, stop.color.b), time);
                alphaKeys[i] = new GradientAlphaKey(stop.color.a, time);
                previousTime = time;
            }

            gradient = new Gradient();
            gradient.SetKeys(colorKeys, alphaKeys);
            return true;
        }

        private static bool TrySplitGradientArguments(string text, List<string> arguments)
        {
            var depth = 0;
            var argumentStart = 0;
            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == '(')
                {
                    depth++;
                }
                else if (text[i] == ')')
                {
                    depth--;
                    if (depth < 0)
                    {
                        return false;
                    }
                }
                else if (text[i] == ',' && depth == 0)
                {
                    arguments.Add(text.Substring(argumentStart, i - argumentStart).Trim());
                    argumentStart = i + 1;
                }
            }

            if (depth != 0)
            {
                return false;
            }

            arguments.Add(text.Substring(argumentStart).Trim());
            return true;
        }

        private static bool TryParseGradientStop(string text, out GradientStop stop)
        {
            stop = default;
            text = text.Trim();

            var colorLength = text.Length;
            var openIndex = text.IndexOf('(');
            if (openIndex > 0
                && (text.Substring(0, openIndex).Trim().Equals("rgb", StringComparison.OrdinalIgnoreCase)
                    || text.Substring(0, openIndex).Trim().Equals("rgba", StringComparison.OrdinalIgnoreCase)))
            {
                var closeIndex = text.IndexOf(')', openIndex + 1);
                if (closeIndex < 0)
                {
                    return false;
                }

                colorLength = closeIndex + 1;
            }
            else
            {
                var whitespaceIndex = text.IndexOfAny(new[] { ' ', '\t', '\r', '\n' });
                if (whitespaceIndex >= 0)
                {
                    colorLength = whitespaceIndex;
                }
            }

            var colorText = text.Substring(0, colorLength).Trim();
            var positionText = text.Substring(colorLength).Trim();
            if (!PasteColorUtil.TryParseColor(colorText, out var color))
            {
                return false;
            }

            float? position = null;
            if (positionText.Length > 0)
            {
                if (!positionText.EndsWith("%", StringComparison.Ordinal)
                    || !float.TryParse(positionText.Substring(0, positionText.Length - 1),
                        NumberStyles.Float, CultureInfo.InvariantCulture, out var percentage)
                    || float.IsNaN(percentage) || float.IsInfinity(percentage))
                {
                    return false;
                }

                position = percentage / 100f;
            }

            stop.color = color;
            stop.position = position;
            return true;
        }

        private static bool IsGradientDirection(string text)
        {
            text = text.Trim();
            if (text.StartsWith("to ", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            foreach (var suffix in new[] { "deg", "grad", "rad", "turn" })
            {
                if (text.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)
                    && float.TryParse(text.Substring(0, text.Length - suffix.Length),
                        NumberStyles.Float, CultureInfo.InvariantCulture, out var direction)
                    && !float.IsNaN(direction) && !float.IsInfinity(direction))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TryParseLinearGradientAngle(string text, out float degrees)
        {
            text = text.Trim();
            switch (text.ToLowerInvariant())
            {
                case "to top":
                    degrees = 0;
                    return true;
                case "to top right":
                case "to right top":
                    degrees = 45;
                    return true;
                case "to right":
                    degrees = 90;
                    return true;
                case "to bottom right":
                case "to right bottom":
                    degrees = 135;
                    return true;
                case "to bottom":
                    degrees = 180;
                    return true;
                case "to bottom left":
                case "to left bottom":
                    degrees = 225;
                    return true;
                case "to left":
                    degrees = 270;
                    return true;
                case "to top left":
                case "to left top":
                    degrees = 315;
                    return true;
            }

            var suffixes = new[] { "deg", "grad", "rad", "turn" };
            foreach (var suffix in suffixes)
            {
                if (!text.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)
                    || !float.TryParse(text.Substring(0, text.Length - suffix.Length),
                        NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
                    || float.IsNaN(value) || float.IsInfinity(value))
                {
                    continue;
                }

                switch (suffix)
                {
                    case "grad":
                        degrees = value * 0.9f;
                        return true;
                    case "rad":
                        degrees = value * Mathf.Rad2Deg;
                        return true;
                    case "turn":
                        degrees = value * 360;
                        return true;
                    default:
                        degrees = value;
                        return true;
                }
            }

            degrees = 0;
            return false;
        }

        private static GradientStop SetStopPosition(GradientStop stop, float position)
        {
            stop.position = position;
            return stop;
        }
    }
}
