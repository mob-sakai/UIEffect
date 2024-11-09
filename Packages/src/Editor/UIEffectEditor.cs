using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using Object = UnityEngine.Object;
#if TMP_ENABLE
using TMPro;
#endif

namespace Coffee.UIEffects.Editors
{
    /// <summary>
    /// UIEffect editor.
    /// </summary>
    [CustomEditor(typeof(UIEffect), true)]
    [CanEditMultipleObjects]
    public class UIEffect2Editor : Editor
    {
        private SerializedProperty _toneFilter;
        private SerializedProperty _toneIntensity;

        private SerializedProperty _colorFilter;
        private SerializedProperty _color;
        private SerializedProperty _colorIntensity;

        private SerializedProperty _samplingFilter;
        private SerializedProperty _samplingIntensity;

        private SerializedProperty _transitionFilter;
        private SerializedProperty _transitionRate;
        private SerializedProperty _transitionTex;
        private SerializedProperty _transitionTexScale;
        private SerializedProperty _transitionTexOffset;
        private SerializedProperty _transitionReverse;
        private SerializedProperty _transitionRotation;
        private SerializedProperty _transitionKeepAspectRatio;
        private SerializedProperty _transitionWidth;
        private SerializedProperty _transitionSoftness;
        private SerializedProperty _transitionColorFilter;
        private SerializedProperty _transitionColor;

        private SerializedProperty _targetMode;
        private SerializedProperty _targetColor;
        private SerializedProperty _targetRange;
        private SerializedProperty _targetSoftness;

        private SerializedProperty _blendType;
        private SerializedProperty _srcBlendMode;
        private SerializedProperty _dstBlendMode;

        private SerializedProperty _shadowMode;
        private SerializedProperty _shadowDistance;
        private SerializedProperty _shadowDistanceX;
        private SerializedProperty _shadowDistanceY;
        private SerializedProperty _shadowIteration;
        private SerializedProperty _shadowFade;
        private SerializedProperty _shadowEffectOnOrigin;
        private SerializedProperty _shadowMirrorScale;

        private void OnEnable()
        {
            if (target == null) return;

            _toneFilter = serializedObject.FindProperty("m_ToneFilter");
            _toneIntensity = serializedObject.FindProperty("m_ToneIntensity");
            var toneParams = serializedObject.FindProperty("m_ToneParams");

            _colorFilter = serializedObject.FindProperty("m_ColorFilter");
            _color = serializedObject.FindProperty("m_Color");
            _colorIntensity = serializedObject.FindProperty("m_ColorIntensity");

            _samplingFilter = serializedObject.FindProperty("m_SamplingFilter");
            _samplingIntensity = serializedObject.FindProperty("m_SamplingIntensity");

            _transitionFilter = serializedObject.FindProperty("m_TransitionFilter");
            _transitionRate = serializedObject.FindProperty("m_TransitionRate");
            _transitionTex = serializedObject.FindProperty("m_TransitionTex");
            _transitionTexScale = serializedObject.FindProperty("m_TransitionTexScale");
            _transitionTexOffset = serializedObject.FindProperty("m_TransitionTexOffset");
            _transitionReverse = serializedObject.FindProperty("m_TransitionReverse");
            _transitionRotation = serializedObject.FindProperty("m_TransitionRotation");
            _transitionKeepAspectRatio = serializedObject.FindProperty("m_TransitionKeepAspectRatio");
            _transitionWidth = serializedObject.FindProperty("m_TransitionWidth");
            _transitionSoftness = serializedObject.FindProperty("m_TransitionSoftness");
            _transitionColorFilter = serializedObject.FindProperty("m_TransitionColorFilter");
            _transitionColor = serializedObject.FindProperty("m_TransitionColor");

            _targetMode = serializedObject.FindProperty("m_TargetMode");
            _targetColor = serializedObject.FindProperty("m_TargetColor");
            _targetRange = serializedObject.FindProperty("m_TargetRange");
            _targetSoftness = serializedObject.FindProperty("m_TargetSoftness");

            _blendType = serializedObject.FindProperty("m_BlendType");
            _srcBlendMode = serializedObject.FindProperty("m_SrcBlendMode");
            _dstBlendMode = serializedObject.FindProperty("m_DstBlendMode");

            _shadowMode = serializedObject.FindProperty("m_ShadowMode");
            _shadowDistance = serializedObject.FindProperty("m_ShadowDistance");
            _shadowDistanceX = _shadowDistance.FindPropertyRelative("x");
            _shadowDistanceY = _shadowDistance.FindPropertyRelative("y");
            _shadowIteration = serializedObject.FindProperty("m_ShadowIteration");
            _shadowFade = serializedObject.FindProperty("m_ShadowFade");
            _shadowEffectOnOrigin = serializedObject.FindProperty("m_ShadowEffectOnOrigin");
            _shadowMirrorScale = serializedObject.FindProperty("m_ShadowMirrorScale");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPresetMenu(targets);
            DrawSeparator();
            DrawProperties();
            serializedObject.ApplyModifiedProperties();
        }

        public void DrawProperties()
        {
            serializedObject.Update();

            // Tone filter
            if (DrawHeaderPopup(_toneFilter))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_toneIntensity);

                EditorGUI.indentLevel--;
            }

            // Color filter
            DrawSeparator();
            var prevColorFilter = (ColorFilter)_colorFilter.intValue;
            if (DrawHeaderPopup(_colorFilter))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_colorIntensity);
                DrawColor(_colorFilter, _color, prevColorFilter);
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndChangeCheck();

            // Sampling filter
            DrawSeparator();
            if (DrawHeaderPopup(_samplingFilter))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_samplingIntensity);
                EditorGUI.indentLevel--;
            }

            // Transition filter
            DrawSeparator();
            if (DrawHeaderPopup(_transitionFilter))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_transitionRate);
                EditorGUILayout.PropertyField(_transitionTex);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_transitionTexScale, EditorGUIUtility.TrTempContent("Scale"));
                EditorGUILayout.PropertyField(_transitionTexOffset, EditorGUIUtility.TrTempContent("Offset"));
                EditorGUILayout.PropertyField(_transitionRotation, EditorGUIUtility.TrTempContent("Rotation"));
                EditorGUILayout.PropertyField(_transitionKeepAspectRatio,
                    EditorGUIUtility.TrTempContent("Keep Aspect Ratio"));
                EditorGUILayout.PropertyField(_transitionReverse, EditorGUIUtility.TrTempContent("Reverse"));
                EditorGUI.indentLevel--;

                if (2 < _transitionFilter.intValue)
                {
                    EditorGUILayout.PropertyField(_transitionWidth);
                    EditorGUILayout.PropertyField(_transitionSoftness);
                    prevColorFilter = (ColorFilter)_transitionColorFilter.intValue;
                    EditorGUILayout.PropertyField(_transitionColorFilter);
                    DrawColor(_transitionColorFilter, _transitionColor, prevColorFilter);
                }

                EditorGUI.indentLevel--;
            }

            // Target
            DrawSeparator();
            if (DrawHeaderPopup(_targetMode))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_targetColor);
                EditorGUILayout.PropertyField(_targetRange);
                EditorGUILayout.PropertyField(_targetSoftness);
                EditorGUI.indentLevel--;
            }

            // Blending
            DrawSeparator();
            if (!DrawHeaderPopup(_blendType))
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(_srcBlendMode);
                EditorGUILayout.PropertyField(_dstBlendMode);
                EditorGUI.indentLevel--;
            }

            // Shadow
            DrawSeparator();
            var prevShadowMode = (ShadowMode)_shadowMode.intValue;
            if (DrawHeaderPopup(_shadowMode))
            {
                EditorGUI.indentLevel++;
                if ((ShadowMode)_shadowMode.intValue == ShadowMode.Mirror)
                {
                    if (prevShadowMode != ShadowMode.Mirror)
                    {
                        _shadowDistanceX.floatValue = 0.5f;
                        _shadowDistanceY.floatValue = 0f;
                    }

                    EditorGUILayout.Slider(_shadowDistanceX, 0, 1, "Reflection");
                    EditorGUILayout.PropertyField(_shadowDistanceY, EditorGUIUtility.TrTempContent("Offset"));
                    EditorGUILayout.PropertyField(_shadowMirrorScale);
                }
                else
                {
                    if (prevShadowMode == ShadowMode.Mirror)
                    {
                        _shadowDistanceX.floatValue = 1f;
                        _shadowDistanceY.floatValue = -1f;
                    }

                    EditorGUILayout.PropertyField(_shadowDistance);
                    EditorGUILayout.PropertyField(_shadowIteration);
                }

                EditorGUILayout.PropertyField(_shadowFade);
                EditorGUILayout.PropertyField(_shadowEffectOnOrigin);
                EditorGUI.indentLevel--;
            }
        }

        private static void DrawColor(SerializedProperty filter, SerializedProperty color, ColorFilter prevFilter)
        {
            if (filter.intValue == (int)ColorFilter.None)
            {
            }
            else if (filter.intValue == (int)ColorFilter.HsvModifier)
            {
                if (prevFilter != ColorFilter.HsvModifier)
                {
                    color.colorValue = Color.black;
                }

                EditorGUILayout.Slider(color.FindPropertyRelative("r"), -0.5f, 0.5f, "Hue");
                EditorGUILayout.Slider(color.FindPropertyRelative("g"), -1f, 1f, "Saturation");
                EditorGUILayout.Slider(color.FindPropertyRelative("b"), -1f, 1f, "Value");
                EditorGUILayout.Slider(color.FindPropertyRelative("a"), 0, 1, "Alpha");
            }
            else if (filter.intValue == (int)ColorFilter.Contrast)
            {
                if (prevFilter != ColorFilter.Contrast)
                {
                    color.colorValue = Color.black;
                }

                EditorGUILayout.Slider(color.FindPropertyRelative("r"), -1f, 1f, "Contrast");
                EditorGUILayout.Slider(color.FindPropertyRelative("g"), -1f, 1f, "Brightness");
                EditorGUILayout.Slider(color.FindPropertyRelative("a"), 0, 1, "Alpha");
            }
            else
            {
                if (prevFilter == ColorFilter.HsvModifier || prevFilter == ColorFilter.Contrast)
                {
                    color.colorValue = Color.white;
                }

                EditorGUILayout.PropertyField(color);
            }
        }

        private static bool DrawHeaderPopup(SerializedProperty sp)
        {
            var r = EditorGUILayout.GetControlRect();
            var pos = EditorGUI.PrefixLabel(r, EditorGUIUtility.TrTempContent(sp.displayName), EditorStyles.boldLabel);
            EditorGUI.PropertyField(pos, sp, GUIContent.none);
            return 0 < sp.intValue;
        }

        private static void DrawSeparator()
        {
            EditorGUILayout.Space(4);
            GUILayout.Label(GUIContent.none, "sv_iconselector_sep", GUILayout.ExpandWidth(true));
        }

        private static void DrawPresetMenu(Object[] targets)
        {
            var r = EditorGUILayout.GetControlRect();
            r.width /= 2;
            if (GUI.Button(r, EditorGUIUtility.TrTempContent("Load"), "MiniPopup"))
            {
                var menu = new GenericMenu();
                foreach (var preset in UIEffectProjectSettings.LoadEditorPresets())
                {
                    var path = UIEffectProjectSettings.GetPresetPath(preset);
                    menu.AddItem(new GUIContent(path), false, () =>
                    {
                        Array.ForEach(targets.OfType<UIEffect>().ToArray(), t =>
                        {
                            t.LoadPreset(preset);
                        });
                    });
                }

                menu.DropDown(r);
            }

            r.x += r.width;
            if (GUI.Button(r, EditorGUIUtility.TrTempContent("Save As New"), "MiniButton"))
            {
                UIEffectProjectSettings.SaveAsNewPreset(targets.OfType<UIEffect>().FirstOrDefault());
            }
        }
    }
}
