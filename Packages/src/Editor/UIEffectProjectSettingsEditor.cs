using System;
using UnityEditor;
using UnityEngine;
using Coffee.UIEffectInternal;
using UnityEditorInternal;

namespace Coffee.UIEffects.Editors
{
    [CustomEditor(typeof(UIEffectProjectSettings))]
    public class UIEffectProjectSettingsEditor : Editor
    {
        private const string k_NoHDRGradientScriptingDefine = "UIEFFECTS_GRADIENT_NO_HDR";

        private ReorderableList _reorderableList;
        private bool _noHdrGradient;
        private SerializedProperty _transformSensitivity;
        private bool _isInitialized;
        private ShaderVariantRegistryEditor _shaderVariantRegistryEditor;

        private void InitializeIfNeeded()
        {
            if (_isInitialized) return;

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

            _isInitialized = true;
        }

        private void OnDisable()
        {
            _isInitialized = false;
        }

        private void Awake()
        {
            // Called when the domain reloads,
            // So we check if the scripting define is altered manually
#if UNITY_2023_1_OR_NEWER
            PlayerSettings.GetScriptingDefineSymbols(EditorUserBuildSettings.selectedBuildTargetGroup, out var defines);
#else
            PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, out var defines);
#endif
            _noHdrGradient = Array.IndexOf(defines, k_NoHDRGradientScriptingDefine) != -1;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            InitializeIfNeeded();

            // Settings
            EditorGUILayout.PropertyField(_transformSensitivity);

            var useHdrGradient = !_noHdrGradient;
            if (EditorGUILayout.Toggle(new GUIContent("HDR Gradient", "Use HDR colors on two-color gradients"), useHdrGradient) != useHdrGradient)
            {
                _noHdrGradient = !_noHdrGradient;
                if (_noHdrGradient)
                {
#if UNITY_2023_1_OR_NEWER
                    PlayerSettings.GetScriptingDefineSymbols(EditorUserBuildSettings.selectedBuildTargetGroup, out var defines);
#else
                    PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, out var defines);
#endif

                    Array.Resize(ref defines, defines.Length + 1);
                    defines[defines.Length - 1] = k_NoHDRGradientScriptingDefine;

#if UNITY_2023_1_OR_NEWER
                    PlayerSettings.SetScriptingDefineSymbols(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
#else
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
#endif
                }
                else
                {
#if UNITY_2023_1_OR_NEWER
                    PlayerSettings.GetScriptingDefineSymbols(EditorUserBuildSettings.selectedBuildTargetGroup, out var defines);
#else
                    PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, out var defines);
#endif

                    defines[Array.IndexOf(defines, k_NoHDRGradientScriptingDefine)] = defines[defines.Length - 1];
                    Array.Resize(ref defines, defines.Length - 1);

#if UNITY_2023_1_OR_NEWER
                    PlayerSettings.SetScriptingDefineSymbols(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
#else
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
#endif
                }
            }
            _reorderableList.DoLayoutList();

            // Shader registry
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Shader", EditorStyles.boldLabel);
            if (_shaderVariantRegistryEditor == null)
            {
                var property = serializedObject.FindProperty("m_ShaderVariantRegistry");
                _shaderVariantRegistryEditor = new ShaderVariantRegistryEditor(property, "(UIEffect)");
            }

            _shaderVariantRegistryEditor.Draw();
            GUILayout.FlexibleSpace();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
