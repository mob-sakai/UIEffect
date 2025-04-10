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
        private SerializedProperty _useTargetTransform;
        private SerializedProperty _samplingScale;
        private SerializedProperty _allowToModifyMeshShape;
        private SerializedProperty _customRoot;
        private Editor _uiEffectEditor;

        private void OnEnable()
        {
            _target = serializedObject.FindProperty("m_Target");
            _useTargetTransform = serializedObject.FindProperty("m_UseTargetTransform");
            _samplingScale = serializedObject.FindProperty("m_SamplingScale");
            _allowToModifyMeshShape = serializedObject.FindProperty("m_AllowToModifyMeshShape");
            _customRoot = serializedObject.FindProperty("m_CustomRoot");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var r = EditorGUILayout.GetControlRect();
            r.width -= 60;
            EditorGUI.PropertyField(r, _target);

            // Preset button.
            r.x += r.width;
            r.width = 60;
            if (GUI.Button(r, EditorGUIUtility.TrTempContent("Preset"), "MiniPopup"))
            {
                UIEffectEditor.DropDownPreset(r, x => !x.builtin, p =>
                {
                    _target.objectReferenceValue = p;
                    _target.serializedObject.ApplyModifiedProperties();
                });
            }

            // Preset mode warning.
            if (_target.objectReferenceValue is UIEffect targetEffect && !targetEffect.gameObject.scene.IsValid())
            {
                r = EditorGUILayout.GetControlRect();
                r.width -= EditorGUIUtility.labelWidth;
                r.x += EditorGUIUtility.labelWidth;
                EditorGUI.HelpBox(r, "The target effect is not in the scene. (Preset mode)", MessageType.Warning);
            }

            EditorGUILayout.PropertyField(_useTargetTransform);
            EditorGUI.BeginDisabledGroup(_useTargetTransform.boolValue);
            EditorGUILayout.PropertyField(_customRoot);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(_samplingScale);
            EditorGUILayout.PropertyField(_allowToModifyMeshShape);

            if (_target.objectReferenceValue)
            {
                EditorGUI.BeginDisabledGroup(true);
                CreateCachedEditor(_target.objectReferenceValue, null, ref _uiEffectEditor);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                (_uiEffectEditor as UIEffectEditor)?.DrawProperties();
                EditorGUILayout.EndVertical();
                EditorGUI.EndDisabledGroup();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
