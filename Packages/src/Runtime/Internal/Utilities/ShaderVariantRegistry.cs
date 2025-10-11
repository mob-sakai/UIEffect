using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
#if UNITY_EDITOR
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using Object = UnityEngine.Object;
#endif

namespace Coffee.UIEffectInternal
{
    [Serializable]
    public sealed class ShaderVariantRegistry
    {
        [Serializable]
        internal class StringPair : IEquatable<StringPair>
        {
            public string key;
            public string value;

            public bool Equals(StringPair other)
            {
                return other != null && key == other.key && value == other.value;
            }

            public override bool Equals(object obj)
            {
                return obj is StringPair other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((key != null ? key.GetHashCode() : 0) * 397) ^ (value != null ? value.GetHashCode() : 0);
                }
            }
        }

        private Dictionary<int, string> _cachedOptionalShaders = new Dictionary<int, string>();

        [SerializeField]
        private List<StringPair> m_OptionalShaders = new List<StringPair>();

        [SerializeField]
        internal ShaderVariantCollection m_Asset;

#if UNITY_EDITOR
        [SerializeField]
        private bool m_ErrorOnUnregisteredVariant = false;

        [SerializeField]
        private List<StringPair> m_UnregisteredVariants = new List<StringPair>();
#endif

        public ShaderVariantCollection shaderVariantCollection => m_Asset;
        public Func<string, bool> onShaderRequested;

        public Shader FindOptionalShader(Shader shader,
            string requiredName,
            string format,
            string defaultOptionalShaderName)
        {
            if (!shader) return null;

            // Already cached.
            var id = shader.GetInstanceID();
            if (_cachedOptionalShaders.TryGetValue(id, out var optionalShaderName))
            {
                return Shader.Find(optionalShaderName);
            }

            // The shader has required name.
            var shaderName = shader.name;
            if (shaderName.Contains(requiredName))
            {
                _cachedOptionalShaders[id] = shaderName;
                return shader;
            }

            // Find optional shader.
            Shader optionalShader;
            var count = m_OptionalShaders.Count;
            for (var index = 0; index < count; index++)
            {
                var pair = m_OptionalShaders[index];
                if (pair.key != shaderName) continue;
                optionalShader = Shader.Find(pair.value);
                if (!optionalShader) continue;
                _cachedOptionalShaders[id] = pair.value;
                return optionalShader;
            }

            // Find optional shader by format.
            optionalShaderName = string.Format(format, shaderName);
            optionalShader = Shader.Find(optionalShaderName);
            if (optionalShader)
            {
                _cachedOptionalShaders[id] = optionalShaderName;
                return optionalShader;
            }

#if UNITY_EDITOR
            if (onShaderRequested?.Invoke(optionalShaderName) ?? false)
            {
                return Shader.Find(defaultOptionalShaderName);
            }
#endif

            // Find default optional shader.
            _cachedOptionalShaders[id] = defaultOptionalShaderName;
            return Shader.Find(defaultOptionalShaderName);
        }

#if UNITY_EDITOR
        private readonly HashSet<StringPair> _logVariants = new HashSet<StringPair>();
        private readonly StringBuilder _sb = new StringBuilder(1024);

        private readonly Dictionary<Hash128, (ShaderVariantCollection.ShaderVariant, StringPair)> _cachedVariants =
            new Dictionary<Hash128, (ShaderVariantCollection.ShaderVariant, StringPair)>();

        public void ClearCache()
        {
            _cachedOptionalShaders.Clear();
            _cachedVariants.Clear();
        }

        /// <summary>
        /// Register all optional shaders associated with the package.
        /// If the shader file has a comment "// [OptionalShader] {packageName}: {shaderName}", it will be registered.
        /// </summary>
        public void RegisterOptionalShaders(Object owner)
        {
            var shaderPaths = ShaderUtil.GetAllShaderInfo()
                .Select(s => AssetDatabase.GetAssetPath(Shader.Find(s.name)))
                .Where(path => !string.IsNullOrEmpty(path) && path.EndsWith(".shader"))
                .ToArray();
            foreach (var path in shaderPaths)
            {
                RegisterOptionalShaders(owner, path);
            }
        }

        /// <summary>
        /// Register optional shaders associated with the package.
        /// If the shader file has a comment "// [OptionalShader] {packageName}: {shaderName}", it will be registered.
        /// </summary>
        private void RegisterOptionalShaders(Object owner, string path)
        {
            if (!File.Exists(path) || !path.EndsWith(".shader")) return;

            var packageName = PackageInfo.FindForAssembly(typeof(ShaderVariantRegistry).Assembly)?.name;
            if (string.IsNullOrEmpty(packageName)) return;

            // Register optional shader names by shader comment.
            var pattern = $"// \\[OptionalShader\\] {packageName}: (.*)$";
            var registeredKeys = new HashSet<string>(m_OptionalShaders.Select(x => x.key));
            var keys = File.ReadLines(path)
                .Take(10)
                .Select(line => Regex.Match(line, pattern))
                .Where(match => match.Success && registeredKeys.Add(match.Groups[1].Value))
                .Select(match => match.Groups[1].Value)
                .ToArray();
            if (0 < keys.Length)
            {
                // Find shader.
                var shader = AssetDatabase.LoadAssetAtPath<Shader>(path);
                if (!shader) return;

                var shaderName = shader.name;
                foreach (var key in keys)
                {
                    m_OptionalShaders.Add(new StringPair() { key = key, value = shaderName });
                }

                EditorUtility.SetDirty(owner);
            }
        }

        public void InitializeIfNeeded(Object owner)
        {
            Profiler.BeginSample("(EDITOR/COF)[ShaderVariantRegistry] InitializeIfNeeded");

            if (!m_Asset && AssetDatabase.IsMainAsset(owner))
            {
                // Find ShaderVariantCollection in owner.
                var path = AssetDatabase.GetAssetPath(owner);
                var collection = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(path);
                if (collection)
                {
                    m_Asset = collection;
                }
                // Create new ShaderVariantCollection.
                else
                {
                    m_Asset = new ShaderVariantCollection() { name = "ShaderVariants" };
                    AssetDatabase.AddObjectToAsset(m_Asset, owner);
                }

                EditorUtility.SetDirty(owner);
                AssetDatabase.SaveAssets();
            }

            ClearCache();
            Profiler.EndSample();
        }

        internal void RegisterVariant(Material material, string path)
        {
            if (!material || !material.shader || !m_Asset) return;

            Profiler.BeginSample("(EDITOR/COF)[ShaderVariantRegistry] RegisterVariant");
            var (variant, pair) = GetVariant(material);
            if (m_Asset.Contains(variant))
            {
                // Already registered.
                m_UnregisteredVariants.Remove(pair);
                Profiler.EndSample();
                return;
            }

            // Error when unregistered variant.
            if (m_ErrorOnUnregisteredVariant)
            {
                if (!m_UnregisteredVariants.Contains(pair))
                {
                    m_UnregisteredVariants.Add(pair);
                }

                if (_logVariants.Add(pair))
                {
                    var shaderName = pair.key;
                    var keywords = string.IsNullOrEmpty(pair.value) ? "no keywords" : pair.value;
                    Debug.LogError($"Shader variant '{shaderName} <{keywords}>' is not registered.\n" +
                                   $"Register it in 'ProjectSettings > {path}' to use it in player.", m_Asset);
                }

                Profiler.EndSample();
                return;
            }

            m_Asset.Add(variant);
            m_UnregisteredVariants.Remove(pair);
            Profiler.EndSample();
        }

        private (ShaderVariantCollection.ShaderVariant, StringPair) GetVariant(Material material)
        {
            Profiler.BeginSample("(EDITOR/COF)[ShaderVariantRegistry] GetVariant");
            var shader = material.shader;
            _sb.Length = 0;
            foreach (var keyword in material.shaderKeywords)
            {
                // Ignore editor keyword.
                if (keyword.Contains("_EDITOR") || keyword.Contains("EDITOR_")) continue;

                _sb.Append(keyword);
                _sb.Append(' ');
            }

            if (0 < _sb.Length)
            {
                _sb.Length--; // Remove last space.
            }

            var hash = new Hash128((uint)shader.GetInstanceID(), (uint)_sb.GetHashCode(), 0, 0);
            if (_cachedVariants.TryGetValue(hash, out var result))
            {
                Profiler.EndSample();
                return result;
            }

            var pair = new StringPair() { key = shader.name, value = _sb.ToString() };
            var variant = new ShaderVariantCollection.ShaderVariant
            {
                shader = shader,
                keywords = pair.value.Split(' ')
            };

            _cachedVariants.Add(hash, (variant, pair));
            Profiler.EndSample();
            return (variant, pair);
        }
#endif
    }

#if UNITY_EDITOR
    internal class ShaderVariantRegistryEditor
    {
        private static readonly MethodInfo s_MiDrawShaderEntry =
            Type.GetType("UnityEditor.ShaderVariantCollectionInspector, UnityEditor")
                ?.GetMethod("DrawShaderEntry", BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly SerializedProperty _errorOnUnregisteredVariant;
        private readonly SerializedProperty _asset;
        private readonly ReorderableList _rlOptionalShaders;
        private readonly ReorderableList _rlUnregisteredVariants;
        private Editor _editor;
        private bool _expandOptionalShaders;
        private bool _expandUnregisteredVariants;

        public ShaderVariantRegistryEditor(SerializedProperty property, string optionName, Action onFindOptions)
        {
            var so = property.serializedObject;
            var optionalShaders = property.FindPropertyRelative("m_OptionalShaders");
            var unregisteredVariants = property.FindPropertyRelative("m_UnregisteredVariants");
            _errorOnUnregisteredVariant = property.FindPropertyRelative("m_ErrorOnUnregisteredVariant");
            _asset = property.FindPropertyRelative("m_Asset");

            _rlOptionalShaders = new ReorderableList(so, optionalShaders, true, true, true, true);
            _rlOptionalShaders.drawHeaderCallback = rect =>
            {
                var rLabel = new Rect(rect.x, rect.y, rect.width - 80, rect.height);
                _expandOptionalShaders = EditorGUI.Foldout(rLabel, _expandOptionalShaders,
                    EditorGUIUtility.TrTextContent($"Optional Shaders {optionName}",
                        "Specify optional shaders explicitly."));

                var rButton1 = new Rect(rect.x + rect.width - 150, rect.y, 90, rect.height - 4);
                if (GUI.Button(rButton1, "Find Options", EditorStyles.miniButton))
                {
                    onFindOptions?.Invoke();
                }

                var rButton2 = new Rect(rect.x + rect.width - 60, rect.y, 60, rect.height - 4);
                if (GUI.Button(rButton2, "Clear All", EditorStyles.miniButton))
                {
                    optionalShaders.ClearArray();
                }
            };
            _rlOptionalShaders.elementHeight = EditorGUIUtility.singleLineHeight * 2 + 4;
            _rlOptionalShaders.drawElementCallback = (r, index, isActive, isFocused) =>
            {
                if (optionalShaders.arraySize <= index) return;

                var element = optionalShaders.GetArrayElementAtIndex(index);
                if (element == null) return;

                var key = element.FindPropertyRelative("key");
                var value = element.FindPropertyRelative("value");
                var h = EditorGUIUtility.singleLineHeight;
                var rKey = new Rect(r.x, r.y + 2, r.width, h);
                if (GUI.Button(rKey, key.stringValue, EditorStyles.popup))
                {
                    ShowShaderDropdown(key);
                }

                var rArrow = new Rect(r.x, r.y + h + 4, 20, h);
                EditorGUI.LabelField(rArrow, GUIContent.none, "ArrowNavigationRight");

                var rValue = new Rect(r.x + 20, r.y + h + 4, r.width - 20, h);
                if (GUI.Button(rValue, value.stringValue, EditorStyles.popup))
                {
                    ShowShaderDropdown(value);
                }
            };

            _rlUnregisteredVariants = new ReorderableList(so, unregisteredVariants, false, true, false, true);
            _rlUnregisteredVariants.drawHeaderCallback = rect =>
            {
                var rWarning = new Rect(rect.x, rect.y, 20, rect.height);
                var icon = EditorGUIUtility.TrIconContent("warning",
                    "These variants are not registered.\nRegister them to use in player.");
                EditorGUI.LabelField(rWarning, icon);

                var rLabel = new Rect(rect.x + 20, rect.y, 200, rect.height);
                EditorGUI.LabelField(rLabel, "Unregistered Shader Variants");

                var rButton = new Rect(rect.x + rect.width - 60, rect.y, 60, rect.height - 4);
                if (GUI.Button(rButton, "Clear All", EditorStyles.miniButton))
                {
                    unregisteredVariants.ClearArray();
                }
            };
            _rlUnregisteredVariants.elementHeight = EditorGUIUtility.singleLineHeight * 2 + 4;
            _rlUnregisteredVariants.drawElementCallback = (r, index, isActive, isFocused) =>
            {
                if (unregisteredVariants.arraySize <= index) return;

                var element = unregisteredVariants.GetArrayElementAtIndex(index);
                if (element == null) return;

                var key = element.FindPropertyRelative("key");
                var value = element.FindPropertyRelative("value");

                var h = EditorGUIUtility.singleLineHeight;
                var rKey = new Rect(r.x, r.y + 2, r.width, h);
                EditorGUI.LabelField(rKey, key.stringValue, EditorStyles.popup);

                var rValue = new Rect(r.x + 20, r.y + h + 5, r.width - 40, 14);
                var keywords = string.IsNullOrEmpty(value.stringValue) ? "<no keywords>" : value.stringValue;
                EditorGUI.TextField(rValue, GUIContent.none, keywords, "LODRenderersText");

                var rButton = new Rect(r.x + r.width - 20, r.y + h + 4, 20, h);
                if (GUI.Button(rButton, EditorGUIUtility.IconContent("icons/toolbar plus.png"), "iconbutton"))
                {
                    var collection = _asset.objectReferenceValue as ShaderVariantCollection;
                    AddVariant(collection, key.stringValue, value.stringValue);
                    unregisteredVariants.DeleteArrayElementAtIndex(index);
                }
            };
        }

        public void Draw()
        {
            DrawOptionalShaders(ref _expandOptionalShaders, _rlOptionalShaders);
            DrawRegisteredShaderVariants(ref _expandUnregisteredVariants, _asset, ref _editor);
            if (0 < _rlUnregisteredVariants.serializedProperty.arraySize)
            {
                EditorGUILayout.Space(4);
                _rlUnregisteredVariants.DoLayoutList();
                EditorGUILayout.Space(-20);
            }

            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 180;
            EditorGUILayout.PropertyField(_errorOnUnregisteredVariant);
            EditorGUIUtility.labelWidth = labelWidth;
        }

        private static void AddVariant(ShaderVariantCollection collection, string shaderName, string keywords)
        {
            if (collection == null) return;

            var shader = Shader.Find(shaderName);
            if (!shader) return;

            collection.Add(new ShaderVariantCollection.ShaderVariant
            {
                shader = shader,
                keywords = keywords.Split(' ')
            });
            EditorUtility.SetDirty(collection);
        }

        private static void DrawOptionalShaders(ref bool expand, ReorderableList list)
        {
            if (expand)
            {
                list.DoLayoutList();
            }
            else
            {
                var r = EditorGUILayout.GetControlRect(false, 20);
                var rBg = new Rect(r.x - 3, r.y, r.width + 6, r.height);
                EditorGUI.LabelField(rBg, GUIContent.none, "RL Header");

                r.x += 3;
                r.width -= 3;
                r.y += 0;
                list.drawHeaderCallback.Invoke(r);
            }
        }

        private static void DrawRegisteredShaderVariants(ref bool expand, SerializedProperty property,
            ref Editor editor)
        {
            var collection = property.objectReferenceValue as ShaderVariantCollection;
            if (collection == null) return;

            EditorGUILayout.Space();
            var r = EditorGUILayout.GetControlRect(false, 20);
            var rBg = new Rect(r.x - 3, r.y, r.width + 6, r.height);
            EditorGUI.LabelField(rBg, GUIContent.none, "RL Header");

            var rLabel = new Rect(r.x + 3, r.y, 200, r.height);
            expand = EditorGUI.Foldout(rLabel, expand, "Registered Shader Variants");

            var rButton = new Rect(r.x + r.width - 62, r.y + 1, 60, r.height - 4);
            if (GUI.Button(rButton, "Clear All", EditorStyles.miniButton))
            {
                collection.Clear();
            }

            if (expand)
            {
                EditorGUILayout.BeginVertical("RL Background");
                Editor.CreateCachedEditor(collection, null, ref editor);
                editor.serializedObject.Update();
                var shaders = editor.serializedObject.FindProperty("m_Shaders");
                var drawShaderEntry = s_MiDrawShaderEntry?.CreateDelegate(typeof(Action<int>), editor) as Action<int>;
                for (var i = 0; i < shaders.arraySize; i++)
                {
                    drawShaderEntry?.Invoke(i);
                }

                EditorGUILayout.EndVertical();
                editor.serializedObject.ApplyModifiedProperties();
            }
        }

        private static void ShowShaderDropdown(SerializedProperty property)
        {
            var menu = new GenericMenu();
            var current = property.stringValue;
            var allShaderNames = ShaderUtil.GetAllShaderInfo()
                .Select(s => s.name);

            foreach (var shaderName in allShaderNames)
            {
                menu.AddItem(new GUIContent(shaderName), shaderName == current, () =>
                {
                    property.stringValue = shaderName;
                    property.serializedObject.ApplyModifiedProperties();
                });
            }

            menu.ShowAsContext();
        }
    }
#endif
}
