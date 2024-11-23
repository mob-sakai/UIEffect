using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using System.IO;
using Object = UnityEngine.Object;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
#endif

namespace Coffee.UIEffectInternal
{
    [Serializable]
    public class ShaderVariantRegistry
    {
        [Serializable]
        internal class StringPair : IEquatable<StringPair>
        {
            public string key;
            public string value;

            public bool Equals(StringPair other)
            {
                if (other == null) return false;
                if (ReferenceEquals(this, other)) return true;
                return key == other.key && value == other.value;
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

            // Required shader.
            if (shader.name.Contains(requiredName))
            {
                _cachedOptionalShaders[id] = shader.name;
                return shader;
            }

            // Find optional shader.
            Shader optionalShader;
            var shaderName = shader.name;
            foreach (var pair in m_OptionalShaders)
            {
                if (pair.key != shaderName) continue;
                optionalShader = Shader.Find(pair.value);
                if (optionalShader)
                {
                    _cachedOptionalShaders[id] = pair.value;
                    return optionalShader;
                }
            }

            // Find optional shader by format.
            optionalShaderName = string.Format(format, shader.name);
            optionalShader = Shader.Find(optionalShaderName);
            if (optionalShader)
            {
                _cachedOptionalShaders[id] = optionalShaderName;
                return optionalShader;
            }

            // Find default optional shader.
            _cachedOptionalShaders[id] = defaultOptionalShaderName;
            return Shader.Find(defaultOptionalShaderName);
        }

#if UNITY_EDITOR
        private HashSet<StringPair> _logVariants = new HashSet<StringPair>();

        public void InitializeIfNeeded(Object owner, string optionalName)
        {
            // Register optional shader names by shader comment.
            if (!string.IsNullOrEmpty(optionalName))
            {
                var optionalShaders = ShaderUtil.GetAllShaderInfo()
                    .Where(s => s.name.Contains(optionalName))
                    .Select(s => (s.name, path: AssetDatabase.GetAssetPath(Shader.Find(s.name))))
                    .Where(x => !string.IsNullOrEmpty(x.path))
                    .SelectMany(x =>
                    {
                        return File.ReadLines(x.path)
                            .Take(10)
                            .Where(line => line.Contains($"OptionalShader@{optionalName}"))
                            .Select(line => Regex.Match(line, @":\s*(.*)$"))
                            .Where(match => match.Success)
                            .Select(match => new StringPair() { key = match.Groups[1].Value, value = x.name });
                    })
                    .Where(pair => m_OptionalShaders.All(x => x.key != pair.key))
                    .ToArray();
                if (0 < optionalShaders.Length)
                {
                    m_OptionalShaders.AddRange(optionalShaders);
                    EditorUtility.SetDirty(owner);
                }
            }

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
        }

        internal void RegisterVariant(Material material, string path)
        {
            if (!material || !material.shader || !m_Asset) return;

            var shaderName = material.shader.name;
            var validKeywords = material.shaderKeywords
                .Where(x => !Regex.IsMatch(x, "(_EDITOR|EDITOR_)"))
                .ToArray();
            var keywords = string.Join(" ", validKeywords);
            var variant = new ShaderVariantCollection.ShaderVariant
            {
                shader = material.shader,
                keywords = validKeywords
            };

            // Already registered.
            var pair = new StringPair() { key = shaderName, value = keywords };
            if (m_Asset.Contains(variant))
            {
                m_UnregisteredVariants.Remove(pair);
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
                    keywords = string.IsNullOrEmpty(keywords) ? "no keywords" : keywords;
                    Debug.LogError($"Shader variant '{shaderName} <{keywords}>' is not registered.\n" +
                                   $"Register it in 'ProjectSettings > {path}' to use it in player.", m_Asset);
                }

                return;
            }

            m_Asset.Add(variant);
            m_UnregisteredVariants.Remove(pair);
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
        private bool _expand;

        public ShaderVariantRegistryEditor(SerializedProperty property, string optionName)
        {
            var so = property.serializedObject;
            var optionalShaders = property.FindPropertyRelative("m_OptionalShaders");
            var unregisteredVariants = property.FindPropertyRelative("m_UnregisteredVariants");
            _errorOnUnregisteredVariant = property.FindPropertyRelative("m_ErrorOnUnregisteredVariant");
            _asset = property.FindPropertyRelative("m_Asset");

            _rlOptionalShaders = new ReorderableList(so, optionalShaders, false, true, true, true);
            _rlOptionalShaders.drawHeaderCallback = rect =>
            {
                var rLabel = new Rect(rect.x, rect.y, rect.width - 80, rect.height);
                EditorGUI.LabelField(rLabel,
                    EditorGUIUtility.TrTextContent($"Optional Shaders {optionName}",
                        "Specify optional shaders explicitly."));

                var rButton = new Rect(rect.x + rect.width - 80, rect.y, 80, rect.height - 4);
                if (GUI.Button(rButton, "Clear All", EditorStyles.miniButton))
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
                    ShowShaderDropdown(key, optionName, false);
                }

                var rArrow = new Rect(r.x, r.y + h + 4, 20, h);
                EditorGUI.LabelField(rArrow, "->");

                var rValue = new Rect(r.x + 20, r.y + h + 4, r.width - 20, h);
                if (GUI.Button(rValue, value.stringValue, EditorStyles.popup))
                {
                    ShowShaderDropdown(value, optionName, true);
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

                var rButton = new Rect(rect.x + rect.width - 80, rect.y, 80, rect.height - 4);
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
            _rlOptionalShaders.DoLayoutList();
            _expand = DrawRegisteredShaderVariants(_expand, _asset, ref _editor);
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

        private static bool DrawRegisteredShaderVariants(bool expand, SerializedProperty property, ref Editor editor)
        {
            var collection = property.objectReferenceValue as ShaderVariantCollection;
            if (collection == null) return expand;

            EditorGUILayout.Space();
            var r = EditorGUILayout.GetControlRect(false, 20);
            var rBg = new Rect(r.x - 3, r.y, r.width + 6, r.height);
            EditorGUI.LabelField(rBg, GUIContent.none, "RL Header");

            var rLabel = new Rect(r.x + 5, r.y, 200, r.height);
            expand = EditorGUI.Foldout(rLabel, expand, "Registered Shader Variants");

            var rButton = new Rect(r.x + r.width - 82, r.y + 1, 80, r.height - 4);
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
                for (var i = 0; i < shaders.arraySize; i++)
                {
                    s_MiDrawShaderEntry.Invoke(editor, new object[] { i });
                }

                EditorGUILayout.EndVertical();
                editor.serializedObject.ApplyModifiedProperties();
            }

            return expand;
        }

        private static void ShowShaderDropdown(SerializedProperty property, string option, bool required)
        {
            var menu = new GenericMenu();
            var current = property.stringValue;
            var allShaderNames = ShaderUtil.GetAllShaderInfo()
                .Where(s => !required || s.name.Contains(option))
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
