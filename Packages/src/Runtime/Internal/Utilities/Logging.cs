using System;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;
#if ENABLE_COFFEE_LOGGER
using System.Reflection;
using System.Collections.Generic;
#else
using Conditional = System.Diagnostics.ConditionalAttribute;
#endif

namespace Coffee.UIEffectInternal
{
    internal static class Logging
    {
#if !ENABLE_COFFEE_LOGGER
        private const string k_DisableSymbol = "DISABLE_COFFEE_LOGGER";

        [Conditional(k_DisableSymbol)]
#endif
        private static void Log_Internal(LogType type, object tag, object message, Object context)
        {
#if ENABLE_COFFEE_LOGGER
            AppendTag(s_Sb, tag);
            s_Sb.Append(message);
            switch (type)
            {
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    Debug.LogError(s_Sb, context);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(s_Sb, context);
                    break;
                case LogType.Log:
                    Debug.Log(s_Sb, context);
                    break;
            }

            s_Sb.Length = 0;
#endif
        }

#if !ENABLE_COFFEE_LOGGER
        [Conditional(k_DisableSymbol)]
#endif
        public static void LogIf(bool enable, object tag, object message, Object context = null)
        {
            if (!enable) return;
            Log_Internal(LogType.Log, tag, message, context ? context : tag as Object);
        }

#if !ENABLE_COFFEE_LOGGER
        [Conditional(k_DisableSymbol)]
#endif
        public static void Log(object tag, object message, Object context = null)
        {
            Log_Internal(LogType.Log, tag, message, context ? context : tag as Object);
        }

#if !ENABLE_COFFEE_LOGGER
        [Conditional(k_DisableSymbol)]
#endif
        public static void LogWarning(object tag, object message, Object context = null)
        {
            Log_Internal(LogType.Warning, tag, message, context ? context : tag as Object);
        }

        public static void LogError(object tag, object message, Object context = null)
        {
#if ENABLE_COFFEE_LOGGER
            Log_Internal(LogType.Error, tag, message, context ? context : tag as Object);
#else
            Debug.LogError($"{tag}: {message}", context);
#endif
        }

#if !ENABLE_COFFEE_LOGGER
        [Conditional(k_DisableSymbol)]
#endif
        public static void LogMulticast(Type type, string fieldName, object instance = null, string message = null)
        {
#if ENABLE_COFFEE_LOGGER
            AppendTag(s_Sb, instance ?? type);

            var handler = type
                .GetField(fieldName,
                    BindingFlags.Static | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                ?.GetValue(instance);

            var list = ((MulticastDelegate)handler)?.GetInvocationList() ?? Array.Empty<Delegate>();
            s_Sb.Append("<color=orange>");
            s_Sb.Append(type.Name);
            s_Sb.Append(".");
            s_Sb.Append(fieldName);
            s_Sb.Append(" has ");
            s_Sb.Append(list.Length);
            s_Sb.Append(" callbacks");
            if (message != null)
            {
                s_Sb.Append(" (");
                s_Sb.Append(message);
                s_Sb.Append(")");
            }

            s_Sb.Append(":</color>");

            for (var i = 0; i < list.Length; i++)
            {
                s_Sb.Append("\n - ");
                s_Sb.Append(list[i].Method.DeclaringType?.Name);
                s_Sb.Append(".");
                s_Sb.Append(list[i].Method.Name);
            }

            Debug.Log(s_Sb);
            s_Sb.Length = 0;
#endif
        }

#if !ENABLE_COFFEE_LOGGER
        [Conditional(k_DisableSymbol)]
#endif
        private static void AppendTag(StringBuilder sb, object tag)
        {
#if ENABLE_COFFEE_LOGGER
            try
            {
                sb.Append("f");
                sb.Append(Time.frameCount);
                sb.Append(":<color=#");
                AppendReadableCode(sb, tag);
                sb.Append("><b>[");

                switch (tag)
                {
                    case string name:
                        sb.Append(name);
                        break;
                    case Type type:
                        AppendType(sb, type);
                        break;
                    case Object uObject:
                        AppendType(sb, tag.GetType());
                        sb.Append(" #");
                        sb.Append(uObject.name);
                        break;
                    default:
                        AppendType(sb, tag.GetType());
                        break;
                }

                sb.Append("]</b></color> ");
            }
            catch
            {
                sb.Append("f");
                sb.Append(Time.frameCount);
                sb.Append(":<b>[");
                sb.Append(tag);
                sb.Append("]</b> ");
            }
#endif
        }

#if !ENABLE_COFFEE_LOGGER
        [Conditional(k_DisableSymbol)]
#endif
        private static void AppendType(StringBuilder sb, Type type)
        {
#if ENABLE_COFFEE_LOGGER
            if (s_TypeNameCache.TryGetValue(type, out var name))
            {
                sb.Append(name);
                return;
            }

            // New type found
            var start = sb.Length;
            if (0 < start && sb[start - 1] == '<' && (type.Name == "Material" || type.Name == "Color"))
            {
                sb.Append('@');
            }

            sb.Append(type.Name);
            if (type.IsGenericType)
            {
                sb.Length -= 2;
                sb.Append("<");
                foreach (var gType in type.GetGenericArguments())
                {
                    AppendType(sb, gType);
                    sb.Append(", ");
                }

                sb.Length -= 2;
                sb.Append(">");
            }

            s_TypeNameCache.Add(type, sb.ToString(start, sb.Length - start));
#endif
        }

#if !ENABLE_COFFEE_LOGGER
        [Conditional(k_DisableSymbol)]
#endif
        private static void AppendReadableCode(StringBuilder sb, object tag)
        {
#if ENABLE_COFFEE_LOGGER
            int hash;
            try
            {
                switch (tag)
                {
                    case string text:
                        hash = text.GetHashCode();
                        break;
                    case Type type:
                        type = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                        hash = type.FullName?.GetHashCode() ?? 0;
                        break;
                    default:
                        hash = tag.GetType().FullName?.GetHashCode() ?? 0;
                        break;
                }
            }
            catch
            {
                sb.Append("FFFFFF");
                return;
            }

            hash = hash & (s_Codes.Length - 1);
            if (s_Codes[hash] == null)
            {
                var hue = hash / (float)s_Codes.Length;
                var modifier = 1f - Mathf.Clamp01(Mathf.Abs(hue - 0.65f) / 0.2f);
                var saturation = 0.7f + modifier * -0.2f;
                var value = 0.8f + modifier * 0.3f;
                s_Codes[hash] = ColorUtility.ToHtmlStringRGB(Color.HSVToRGB(hue, saturation, value));
            }

            sb.Append(s_Codes[hash]);
#endif
        }

#if ENABLE_COFFEE_LOGGER
        private static readonly StringBuilder s_Sb = new StringBuilder();
        private static readonly string[] s_Codes = new string[64];
        private static readonly Dictionary<Type, string> s_TypeNameCache = new Dictionary<Type, string>();
#endif
    }
}
