using UnityEditor;
using UnityEngine;

namespace Coffee.UIEffects.Editors
{
    /// <summary>
    /// UIEffect editor.
    /// </summary>
    [CustomEditor(typeof(UIEffectReplica))]
    [CanEditMultipleObjects]
    public class UIEffectReplicaEditor : Editor
    {
        private SerializedProperty _target;
        private SerializedProperty _preset;
        private SerializedProperty _useTargetTransform;
        private SerializedProperty _samplingScale;
        private SerializedProperty _allowToModifyMeshShape;
        private SerializedProperty _customRoot;
        private Editor _uiEffectEditor;

        private void OnEnable()
        {
            _target = serializedObject.FindProperty("m_Target");
            _preset = serializedObject.FindProperty("m_Preset");
            _useTargetTransform = serializedObject.FindProperty("m_UseTargetTransform");
            _samplingScale = serializedObject.FindProperty("m_SamplingScale");
            _allowToModifyMeshShape = serializedObject.FindProperty("m_AllowToModifyMeshShape");
            _customRoot = serializedObject.FindProperty("m_CustomRoot");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var r = EditorGUILayout.GetControlRect();
            r.width -= 120;
            if (_preset.objectReferenceValue)
            {
                EditorGUI.PropertyField(r, _preset);
            }
            else
            {
                EditorGUI.PropertyField(r, _target);
            }

            // Preset button.
            r.x += r.width;
            r.width = 60;
            if (GUI.Button(r, EditorGUIUtility.TrTempContent("Preset"), "MiniPopup"))
            {
                UIEffectEditor.DropDownPreset(r, null, p =>
                {
                    _target.objectReferenceValue = null;
                    _preset.objectReferenceValue = p;
                    _preset.serializedObject.ApplyModifiedProperties();
                });
            }

            // Clear button.
            r.x += r.width;
            r.width = 60;
            if (GUI.Button(r, EditorGUIUtility.TrTempContent("Clear")))
            {
                _target.objectReferenceValue = null;
                _preset.objectReferenceValue = null;
            }

            // Preset mode warning.
            if (_target.objectReferenceValue is UIEffect targetEffect && !targetEffect.gameObject.scene.IsValid())
            {
                r = EditorGUILayout.GetControlRect();
                EditorGUI.HelpBox(r, "The target effect is not in the scene. (Preset mode)", MessageType.Warning);
            }

            EditorGUILayout.PropertyField(_useTargetTransform);
            EditorGUI.BeginDisabledGroup(_useTargetTransform.boolValue);
            EditorGUILayout.PropertyField(_customRoot);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(_samplingScale);
            EditorGUILayout.PropertyField(_allowToModifyMeshShape);

            var parent = _target.objectReferenceValue ?? _preset.objectReferenceValue;
            if (parent)
            {
                EditorGUI.BeginDisabledGroup(true);
                CreateCachedEditor(parent, null, ref _uiEffectEditor);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                (_uiEffectEditor as UIEffectPropertyEditor)?.DrawProperties();
                EditorGUILayout.EndVertical();
                EditorGUI.EndDisabledGroup();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
