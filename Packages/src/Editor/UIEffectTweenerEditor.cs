using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace Coffee.UIEffects.Editors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIEffectTweener))]
    internal class UIMaterialPropertyTweenerEditor : Editor
    {
        private SerializedProperty _cullingMask;
        private SerializedProperty _direction;
        private SerializedProperty _curve;
        private SerializedProperty _delay;
        private SerializedProperty _duration;
        private SerializedProperty _interval;
        private SerializedProperty _restartOnEnable;
        private SerializedProperty _updateMode;
        private SerializedProperty _wrapMode;

        private void OnEnable()
        {
            _cullingMask = serializedObject.FindProperty("m_CullingMask");
            _direction = serializedObject.FindProperty("m_Direction");
            _curve = serializedObject.FindProperty("m_Curve");
            _restartOnEnable = serializedObject.FindProperty("m_RestartOnEnable");
            _delay = serializedObject.FindProperty("m_Delay");
            _duration = serializedObject.FindProperty("m_Duration");
            _interval = serializedObject.FindProperty("m_Interval");
            _wrapMode = serializedObject.FindProperty("m_WrapMode");
            _updateMode = serializedObject.FindProperty("m_UpdateMode");
        }

        public override void OnInspectorGUI()
        {
            Profiler.BeginSample("(MPI)[MPTweenerEditor] OnInspectorGUI");
            serializedObject.UpdateIfRequiredOrScript();
            EditorGUILayout.PropertyField(_cullingMask);
            EditorGUILayout.PropertyField(_direction);
            EditorGUILayout.PropertyField(_curve);
            EditorGUILayout.PropertyField(_delay);
            EditorGUILayout.PropertyField(_duration);
            EditorGUILayout.PropertyField(_interval);
            EditorGUILayout.PropertyField(_restartOnEnable);
            EditorGUILayout.PropertyField(_wrapMode);
            EditorGUILayout.PropertyField(_updateMode);
            serializedObject.ApplyModifiedProperties();
            DrawPlayer(target as UIEffectTweener);
            Profiler.EndSample();
        }

        private void DrawPlayer(UIEffectTweener tweener)
        {
            if (!tweener) return;

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            var icon = EditorGUIUtility.IconContent("icons/playbutton.png");
            var r = EditorGUILayout.GetControlRect(false);

            var rButton = new Rect(r.x, r.y, 20, r.height);
            if (GUI.Button(rButton, icon, "IconButton"))
            {
                tweener.SetTime(0);
            }

            EditorGUI.EndDisabledGroup();

            var totalTime = tweener.totalTime;
            var time = tweener.time;
            var label = EditorGUIUtility.TrTempContent($"{time:N2}/{totalTime:N2}");
            var wLabel = Mathf.CeilToInt(EditorStyles.label.CalcSize(label).x / 5f) * 5f;
            wLabel = 80;
            var rLabel = new Rect(r.x + r.width - wLabel, r.y, wLabel, r.height);
            GUI.Label(rLabel, label, "RightLabel");
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            var rSlider = new Rect(r.x + 20, r.y, r.width - wLabel - 20, r.height);

            //

            var
                r0 = rSlider; //new Rect(rSlider.x, rSlider.y, rSlider.width * tweener.interval / totalTime, rSlider.height);
            r0.y += 4;
            r0.height -= 8;
            r0.width = rSlider.width * tweener.delay / totalTime;
            GUI.color = Color.blue;
            GUI.Label(r0, GUIContent.none, "TE DefaultTime");

            r0.x += r0.width;
            r0.width = rSlider.width * tweener.duration / totalTime;
            GUI.color = Color.green;
            GUI.Label(r0, GUIContent.none, "TE DefaultTime");

            r0.x += r0.width;
            r0.width = rSlider.width * tweener.interval / totalTime;
            GUI.color = Color.red;
            GUI.Label(r0, GUIContent.none, "TE DefaultTime");

            if (UIEffectTweener.WrapMode.PingPongOnce <= tweener.wrapMode)
            {
                r0.x += r0.width;
                r0.width = rSlider.width * tweener.duration / totalTime;
                GUI.color = Color.green;
                GUI.Label(r0, GUIContent.none, "TE DefaultTime");
            }

            if (UIEffectTweener.WrapMode.PingPongLoop <= tweener.wrapMode)
            {
                r0.x += r0.width;
                r0.width = rSlider.width * tweener.interval / totalTime;
                GUI.color = Color.red;
                GUI.Label(r0, GUIContent.none, "TE DefaultTime");
            }


            GUI.color = Color.white;

            time = GUI.HorizontalSlider(rSlider, time, 0, totalTime);
            if (EditorGUI.EndChangeCheck())
            {
                tweener.SetTime(time);
            }

            if (Application.isPlaying && tweener.isActiveAndEnabled)
            {
                Repaint();
            }
        }

        // private static void PostAddElement(SerializedProperty prop, string propertyName)
        // {
        //     prop.FindPropertyRelative("m_From.m_Type").intValue = -1;
        //     prop.FindPropertyRelative("m_From.m_PropertyName").stringValue = propertyName;
        //     prop.FindPropertyRelative("m_To.m_Type").intValue = -1;
        //     prop.FindPropertyRelative("m_To.m_PropertyName").stringValue = propertyName;
        // }
        //
        // private void ResetCallback()
        // {
        //     var current = serializedObject.targetObject as UIEffectTweener;
        //     if (!current) return;
        //
        //     Undo.RecordObject(current, "Reset Values");
        //     current.ResetPropertiesToDefault();
        // }
    }
}
