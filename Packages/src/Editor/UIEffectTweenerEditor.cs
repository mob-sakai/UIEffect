using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace Coffee.UIEffects.Editors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIEffectTweener))]
    internal class UIEffectTweenerEditor : Editor
    {
        private SerializedProperty _cullingMask;
        private SerializedProperty _direction;
        private SerializedProperty _curve;
        private SerializedProperty _separateReverseCurve;
        private SerializedProperty _reverseCurve;
        private SerializedProperty _delay;
        private SerializedProperty _duration;
        private SerializedProperty _interval;
        private SerializedProperty _playOnEnable;
        private SerializedProperty _resetTimeOnEnable;
        private SerializedProperty _updateMode;
        private SerializedProperty _wrapMode;
        private SerializedProperty _onComplete;
        private SerializedProperty _onChangedRate;
        private bool _isPlaying = false;
        private double _lastTime;

        private void OnEnable()
        {
            _cullingMask = serializedObject.FindProperty("m_CullingMask");
            _direction = serializedObject.FindProperty("m_Direction");
            _curve = serializedObject.FindProperty("m_Curve");
            _separateReverseCurve = serializedObject.FindProperty("m_SeparateReverseCurve");
            _reverseCurve = serializedObject.FindProperty("m_ReverseCurve");
            _playOnEnable = serializedObject.FindProperty("m_PlayOnEnable");
            _resetTimeOnEnable = serializedObject.FindProperty("m_ResetTimeOnEnable");
            _delay = serializedObject.FindProperty("m_Delay");
            _duration = serializedObject.FindProperty("m_Duration");
            _interval = serializedObject.FindProperty("m_Interval");
            _wrapMode = serializedObject.FindProperty("m_WrapMode");
            _updateMode = serializedObject.FindProperty("m_UpdateMode");
            _onComplete = serializedObject.FindProperty("m_OnComplete");
            _onChangedRate = serializedObject.FindProperty("m_OnChangedRate");

            EditorApplication.update += UpdateTweeners;
        }

        private void OnDisable()
        {
            EditorApplication.update -= UpdateTweeners;
        }

        public override void OnInspectorGUI()
        {
            Profiler.BeginSample("(UIE)[UIEffectTweener] OnInspectorGUI");
            serializedObject.UpdateIfRequiredOrScript();
            EditorGUILayout.PropertyField(_cullingMask);

            if (0 != (_cullingMask.intValue & (int)UIEffectTweener.CullingMask.Event))
            {
                EditorGUILayout.PropertyField(_onChangedRate);
            }

            EditorGUILayout.PropertyField(_direction);
            EditorGUILayout.PropertyField(_curve);

            var pos = EditorGUILayout.GetControlRect();
            var rect = new Rect(pos.x, pos.y, EditorGUIUtility.labelWidth + 20, pos.height);
            EditorGUI.PropertyField(rect, _separateReverseCurve);
            if (_separateReverseCurve.boolValue)
            {
                rect.x += rect.width;
                rect.width = pos.width - rect.width;
                EditorGUI.PropertyField(rect, _reverseCurve, GUIContent.none);
            }

            EditorGUILayout.PropertyField(_delay);
            EditorGUILayout.PropertyField(_duration);
            EditorGUILayout.PropertyField(_interval);
            EditorGUILayout.PropertyField(_playOnEnable);
            EditorGUILayout.PropertyField(_resetTimeOnEnable);
            EditorGUILayout.PropertyField(_wrapMode);
            EditorGUILayout.PropertyField(_updateMode);
            EditorGUILayout.PropertyField(_onComplete);
            serializedObject.ApplyModifiedProperties();
            DrawPlayer(target as UIEffectTweener);
            Profiler.EndSample();
        }

        private void DrawPlayer(UIEffectTweener tweener)
        {
            if (!tweener) return;

            EditorGUILayout.Space(4);
            GUILayout.Label(GUIContent.none, "sv_iconselector_sep", GUILayout.ExpandWidth(true));

            EditorGUILayout.BeginHorizontal();
            var r = EditorGUILayout.GetControlRect(false);
            var rResetTimeButton = new Rect(r.x, r.y, 20, r.height);
            if (GUI.Button(rResetTimeButton, EditorGUIUtility.IconContent("animation.firstkey"), "IconButton"))
            {
                ResetTime();
            }

            var rPlayButton = new Rect(r.x + 20, r.y, 20, r.height);
            if (GUI.Button(rPlayButton, EditorGUIUtility.IconContent("playbutton"), "IconButton"))
            {
                SetPlaying(true);
            }

            var rPauseButton = new Rect(r.x + 40, r.y, 20, r.height);
            if (GUI.Button(rPauseButton, EditorGUIUtility.IconContent("pausebutton"), "IconButton"))
            {
                SetPlaying(false);
            }

            var totalTime = tweener.totalTime;
            var time = tweener.time;
            var label = EditorGUIUtility.TrTempContent($"{time:N2}/{totalTime:N2}");
            var rLabel = new Rect(r.x + r.width - 80, r.y, 80, r.height);
            GUI.Label(rLabel, label, "RightLabel");
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            var rSlider = new Rect(r.x + 60, r.y, r.width - 140, r.height);
            var r0 = new Rect(rSlider.x, rSlider.y + 4, rSlider.width, rSlider.height - 8);
            r0.x += DrawBackground(r0, rSlider.width * tweener.delay / totalTime, Color.blue);
            r0.x += DrawBackground(r0, rSlider.width * tweener.duration / totalTime, Color.green);

            if (UIEffectTweener.WrapMode.Loop <= tweener.wrapMode)
            {
                r0.x += DrawBackground(r0, rSlider.width * tweener.interval / totalTime, Color.red);
            }

            if (UIEffectTweener.WrapMode.PingPongOnce <= tweener.wrapMode)
            {
                r0.x += DrawBackground(r0, rSlider.width * tweener.duration / totalTime, Color.green);
            }

            if (UIEffectTweener.WrapMode.PingPongLoop <= tweener.wrapMode)
            {
                r0.x += DrawBackground(r0, rSlider.width * tweener.interval / totalTime, Color.red);
            }

            GUI.color = Color.white;
            time = GUI.HorizontalSlider(rSlider, time, 0, totalTime);
            if (EditorGUI.EndChangeCheck())
            {
                SetTime(time);
            }

            if (Application.isPlaying && tweener.isActiveAndEnabled)
            {
                Repaint();
            }
        }

        private static float DrawBackground(Rect r, float width, Color color)
        {
            r.width = width;
            GUI.color = color;
            GUI.Label(r, GUIContent.none, "TE DefaultTime");
            return width;
        }

        private void SetTime(float time)
        {
            foreach (var tweener in targets.OfType<UIEffectTweener>())
            {
                tweener.SetTime(time);
            }
        }

        private void ResetTime()
        {
            foreach (var tweener in targets.OfType<UIEffectTweener>())
            {
                tweener.ResetTime(tweener.direction);
            }
        }

        private void SetPlaying(bool enable)
        {
            if (!EditorApplication.isPlaying)
            {
                _isPlaying = enable;
                _lastTime = EditorApplication.timeSinceStartup;
                return;
            }

            foreach (var tweener in targets.OfType<UIEffectTweener>())
            {
                if (enable)
                {
                    tweener.Play(false);
                }
                else
                {
                    tweener.SetPause(true);
                }
            }
        }

        private void UpdateTweeners()
        {
            if (!_isPlaying || EditorApplication.isPlayingOrWillChangePlaymode) return;

            var delta = (float)(EditorApplication.timeSinceStartup - _lastTime);
            _lastTime = EditorApplication.timeSinceStartup;
            foreach (var tweener in targets.OfType<UIEffectTweener>())
            {
                tweener.UpdateTime(tweener.direction == UIEffectTweener.Direction.Forward ? delta : -delta);
            }

            EditorApplication.QueuePlayerLoopUpdate();
        }
    }
}
