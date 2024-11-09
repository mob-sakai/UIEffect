using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditorInternal;

namespace Coffee.UIEffects.Editors
{
    [CustomEditor(typeof(UIEffectProjectSettings))]
    public class UIEffectProjectSettingsEditor : Editor
    {
        private ReorderableList _reorderableList;
        private SerializedProperty _fallbackVariantBehaviour;
        private SerializedProperty _transformSensitivity;
        private Editor _editor;
        private bool _isInitialized;

        private void InitializeIfNeeded()
        {
            if (_isInitialized) return;

            _fallbackVariantBehaviour = serializedObject.FindProperty("m_FallbackVariantBehaviour");
            _transformSensitivity = serializedObject.FindProperty("m_TransformSensitivity");
            var runtimePresets = serializedObject.FindProperty("m_RuntimePresets");
            _reorderableList = new ReorderableList(serializedObject, runtimePresets, true, true, true, true);
            _reorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Runtime Presets");
            _reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                var effect = element.objectReferenceValue as UIEffect;
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;

                EditorGUI.BeginDisabledGroup(effect && effect.hideFlags == HideFlags.NotEditable);
                EditorGUI.PropertyField(rect, element, GUIContent.none);
                EditorGUI.EndDisabledGroup();
            };
            _reorderableList.onAddDropdownCallback = (rect, list) =>
            {
                var menu = new GenericMenu();
                foreach (var preset in UIEffectProjectSettings.LoadEditorPresets())
                {
                    if (UIEffectProjectSettings.instance.m_RuntimePresets.Contains(preset)) continue;

                    var path = UIEffectProjectSettings.GetPresetPath(preset);
                    menu.AddItem(new GUIContent(path), false, x =>
                    {
                        UIEffectProjectSettings.RegisterRuntimePreset(x as UIEffect);
                    }, preset);
                }

                menu.DropDown(rect);
            };

            var collection = serializedObject.FindProperty("m_ShaderVariantCollection").objectReferenceValue;
            _editor = CreateEditor(collection);
            _isInitialized = true;
        }

        private void OnDisable()
        {
            if (_editor)
            {
                DestroyImmediate(_editor);
            }

            _editor = null;
            _isInitialized = false;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            InitializeIfNeeded();

            // Settings
            EditorGUILayout.PropertyField(_transformSensitivity);
            _reorderableList.DoLayoutList();

            // Editor
            EditorGUILayout.PropertyField(_fallbackVariantBehaviour);
            serializedObject.ApplyModifiedProperties();

            // Shader
            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("Shader Variants", EditorStyles.boldLabel);
            EditorGUILayout.Space(-12);
            DrawUnregisteredShaderVariants(UIEffectProjectSettings.instance.m_UnregisteredShaderVariants);
            DrawRegisteredShaderVariants(_editor);
            GUILayout.FlexibleSpace();
        }

        private static void DrawUnregisteredShaderVariants(List<string> variants)
        {
            if (variants.Count == 0) return;

            EditorGUILayout.Space();
            var array = variants.ToArray();
            var r = EditorGUILayout.GetControlRect(false, 20);
            var rLabel = new Rect(r.x, r.y, r.width - 80, r.height);
            EditorGUI.LabelField(rLabel, "Registered Shader Variants");

            var rButtonClear = new Rect(r.x + r.width - 80, r.y + 2, 80, r.height - 4);
            if (GUI.Button(rButtonClear, "Clear All", EditorStyles.miniButton))
            {
                UIEffectProjectSettings.ClearUnregisteredShaderVariants();
            }

            EditorGUILayout.BeginVertical("RL Background");
            for (var i = 0; i < array.Length; i++)
            {
                var values = array[i].Split(';');
                r = EditorGUILayout.GetControlRect();
                var rShader = new Rect(r.x, r.y, 150, r.height);
                EditorGUI.ObjectField(rShader, Shader.Find(values[0]), typeof(Shader), false);

                var rKeywords = new Rect(r.x + 150, r.y, r.width - 150 - 20, r.height);
                EditorGUI.TextField(rKeywords, values[1]);

                var rButton = new Rect(r.x + r.width - 20 + 2, r.y, 20, r.height);
                if (GUI.Button(rButton, EditorGUIUtility.IconContent("icons/toolbar plus.png"), "iconbutton"))
                {
                    UIEffectProjectSettings.RegisterVariant(array[i]);
                }
            }

            EditorGUILayout.EndVertical();
        }

        private static void DrawRegisteredShaderVariants(Editor editor)
        {
            var collection = editor.target as ShaderVariantCollection;
            if (collection == null) return;

            EditorGUILayout.Space();
            var r = EditorGUILayout.GetControlRect(false, 20);
            var rLabel = new Rect(r.x, r.y, r.width - 80, r.height);
            EditorGUI.LabelField(rLabel, "Registered Shader Variants");

            var rButton = new Rect(r.x + r.width - 80, r.y + 2, 80, r.height - 4);
            if (GUI.Button(rButton, "Clear All", EditorStyles.miniButton))
            {
                collection.Clear();
            }

            EditorGUILayout.BeginVertical("RL Background");
            editor.serializedObject.Update();
            var shaders = editor.serializedObject.FindProperty("m_Shaders");
            for (var i = 0; i < shaders.arraySize; i++)
            {
                s_MiDrawShaderEntry.Invoke(editor, new object[] { i });
            }

            editor.serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndVertical();
        }

        private static readonly MethodInfo s_MiDrawShaderEntry =
            Type.GetType("UnityEditor.ShaderVariantCollectionInspector, UnityEditor")
                ?.GetMethod("DrawShaderEntry", BindingFlags.NonPublic | BindingFlags.Instance);
    }
}
