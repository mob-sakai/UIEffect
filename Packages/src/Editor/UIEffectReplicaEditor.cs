using UnityEditor;

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
        private Editor _uiEffectEditor;

        private void OnEnable()
        {
            _target = serializedObject.FindProperty("m_Target");
            _useTargetTransform = serializedObject.FindProperty("m_UseTargetTransform");
            _samplingScale = serializedObject.FindProperty("m_SamplingScale");
            _allowToModifyMeshShape = serializedObject.FindProperty("m_AllowToModifyMeshShape");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_target);
            EditorGUILayout.PropertyField(_useTargetTransform);
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
