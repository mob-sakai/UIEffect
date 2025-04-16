using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private SerializedProperty _runtimePresets;
        private SerializedProperty _runtimePresetsV2;
        private SerializedProperty _useHDRColorPicker;
        private SerializedProperty _transformSensitivity;
        private bool _isInitialized;
        private ShaderVariantRegistryEditor _shaderVariantRegistryEditor;
        private UIEffect[] _legacyPresets;
        private bool _expandRuntimePresets;
        private bool _expandLegacyPrefabs;

        private void InitializeIfNeeded()
        {
            if (_isInitialized) return;

            _transformSensitivity = serializedObject.FindProperty("m_TransformSensitivity");
            _useHDRColorPicker = serializedObject.FindProperty("m_UseHDRColorPicker");
            _runtimePresets = serializedObject.FindProperty("m_RuntimePresets");
            _runtimePresetsV2 = serializedObject.FindProperty("m_RuntimePresetsV2");

            _reorderableList = new ReorderableList(serializedObject, _runtimePresetsV2, true, true, true, true);
            _reorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Runtime Presets");
            _reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                var effect = element.objectReferenceValue as UIEffectPreset;
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;

                EditorGUI.BeginDisabledGroup(effect && effect.hideFlags == HideFlags.NotEditable);
                EditorGUI.PropertyField(rect, element, GUIContent.none);
                EditorGUI.EndDisabledGroup();
            };
            _reorderableList.onAddDropdownCallback = (rect, list) =>
            {
                UIEffectEditor.DropDownPreset(rect,
                    x => !UIEffectProjectSettings.instance.m_RuntimePresetsV2.Contains(x.preset),
                    UIEffectProjectSettings.RegisterRuntimePreset);
            };

            _isInitialized = true;
        }

        private void OnEnable()
        {
            _legacyPresets = UIEffectProjectSettings.LoadEditorPresets();
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

            // Legacy Presets
            DrawLegacyPresets();

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

        private void DrawLegacyPresets()
        {
            for (var i = _runtimePresets.arraySize - 1; 0 <= i; i--)
            {
                if (!_runtimePresets.GetArrayElementAtIndex(i).objectReferenceValue)
                {
                    _runtimePresets.DeleteArrayElementAtIndex(i);
                }
            }

            if (_legacyPresets.Length == 0 && _runtimePresets.arraySize == 0) return;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            var title = EditorGUIUtility.TrTextContent("Legacy Presets (Obsolete)", "", "warning");
            EditorGUILayout.LabelField(title,
                EditorStyles.boldLabel);

            _expandRuntimePresets = EditorGUILayout.Foldout(_expandRuntimePresets,
                $"Legacy Runtime Presets ({_runtimePresets.arraySize})");
            if (_expandRuntimePresets)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.indentLevel++;

                for (var i = 0; i < _runtimePresets.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(_runtimePresets.GetArrayElementAtIndex(i), GUIContent.none);
                }

                EditorGUI.indentLevel--;
                EditorGUI.EndDisabledGroup();
            }

            _expandLegacyPrefabs = EditorGUILayout.Foldout(_expandLegacyPrefabs,
                $"Legacy Preset Prefabs ({_legacyPresets.Length})");
            if (_expandLegacyPrefabs)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.indentLevel++;

                foreach (var legacyPreset in _legacyPresets)
                {
                    EditorGUILayout.ObjectField(legacyPreset, typeof(UIEffect), false);
                }

                EditorGUI.indentLevel--;
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.TextArea(
                "The prefab based presets system is obsolete.\n" +
                "In the new preset system, UIEffectPreset assets are used.\n" +
                "Click the button below to convert your legacy preset prefabs into UIEffectPreset assets.\n" +
                "The UIEffectPreset asset will be saved in the same directory as the prefab, using the same name.\n" +
                "You can load the new preset via the UIEffect.LoadPreset method.",
                EditorStyles.miniLabel);
            if (GUILayout.Button(EditorGUIUtility.TrTempContent("Convert All Legacy Presets in the Project")))
            {
                ConvertAllLegacyPresets();
                _runtimePresets.ClearArray();
            }

            if (GUILayout.Button(EditorGUIUtility.TrTempContent("Delete All Legacy Presets in the Project")))
            {
                foreach (var legacyPreset in _legacyPresets)
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(legacyPreset));
                }

                _runtimePresets.ClearArray();
                _legacyPresets = Array.Empty<UIEffect>();
            }

            EditorGUILayout.EndVertical();
        }

        private void ConvertAllLegacyPresets()
        {
            var length = _legacyPresets.Length;
            var converted = new HashSet<string>(UIEffectProjectSettings.LoadEditorPresetsV2()
                .Select(x => Path.ChangeExtension(AssetDatabase.GetAssetPath(x), ".prefab")));
            for (var i = 0; i < length; i++)
            {
                var preset = _legacyPresets[i];
                var path = AssetDatabase.GetAssetPath(preset);
                if (converted.Contains(path)) continue;

                EditorUtility.DisplayProgressBar("Convert To UIEffectPreset", path, (float)i / length);
                var newPath = Path.ChangeExtension(path, ".asset");
                newPath = AssetDatabase.GenerateUniqueAssetPath(newPath);
                var newPreset = CreateInstance<UIEffectPreset>();
                preset.SavePreset(newPreset, false);
                newPreset.hideFlags = preset.hideFlags;
                AssetDatabase.CreateAsset(newPreset, newPath);

                var importer = AssetImporter.GetAtPath(path);
                var newImporter = AssetImporter.GetAtPath(newPath);
                newImporter.userData = importer.userData;
                newImporter.SetAssetBundleNameAndVariant(importer.assetBundleName, importer.assetBundleVariant);
                newImporter.SaveAndReimport();

                // Remove the legacy preset from the project settings and add the new preset.
                if (UIEffectProjectSettings.instance.m_RuntimePresets.Contains(preset))
                {
                    var count = _runtimePresetsV2.arraySize;
                    _runtimePresetsV2.InsertArrayElementAtIndex(count);
                    _runtimePresetsV2.GetArrayElementAtIndex(count).objectReferenceValue = newPreset;
                }
            }

            EditorUtility.ClearProgressBar();
        }
    }
}
