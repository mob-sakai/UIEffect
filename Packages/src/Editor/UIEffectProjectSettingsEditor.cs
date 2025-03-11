using UnityEditor;
using UnityEngine;
using Coffee.UIEffectInternal;
using UnityEditorInternal;

namespace Coffee.UIEffects.Editors
{
    [CustomEditor(typeof(UIEffectProjectSettings))]
    public class UIEffectProjectSettingsEditor : Editor
    {
        private ReorderableList _reorderableList;
        private SerializedProperty _useHDRColorPicker;
        private SerializedProperty _transformSensitivity;
        private bool _isInitialized;
        private ShaderVariantRegistryEditor _shaderVariantRegistryEditor;

        private void InitializeIfNeeded()
        {
            if (_isInitialized) return;

            _transformSensitivity = serializedObject.FindProperty("m_TransformSensitivity");
            _useHDRColorPicker = serializedObject.FindProperty("m_UseHDRColorPicker");
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
                UIEffectEditor.DropDownPreset(rect,
                    x => !x.builtin && !UIEffectProjectSettings.instance.m_RuntimePresets.Contains(x.preset),
                    UIEffectProjectSettings.RegisterRuntimePreset);
            };

            _isInitialized = true;
        }

        private void OnDisable()
        {
            _isInitialized = false;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            InitializeIfNeeded();

            // Settings
            // Transform sensitivity.
            EditorGUILayout.PropertyField(_transformSensitivity);

            // Runtime Presets
            _reorderableList.DoLayoutList();

            // Editor
            // Use HDR color pickers.
            EditorGUILayout.PropertyField(_useHDRColorPicker);

            // Shader
            // Shader registry
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Shader", EditorStyles.boldLabel);
            if (_shaderVariantRegistryEditor == null)
            {
                var property = serializedObject.FindProperty("m_ShaderVariantRegistry");
                _shaderVariantRegistryEditor = new ShaderVariantRegistryEditor(property, "(UIEffect)",
                    () =>
                    {
                        UIEffectProjectSettings.shaderRegistry
                            .RegisterOptionalShaders(UIEffectProjectSettings.instance);
                    });
            }

            _shaderVariantRegistryEditor.Draw();
            GUILayout.FlexibleSpace();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
