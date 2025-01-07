using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using System.Reflection;
using Object = UnityEngine.Object;

namespace Coffee.UIEffects.Editors
{
    /// <summary>
    /// UIEffect editor.
    /// </summary>
    [CustomEditor(typeof(UIEffect), true)]
    [CanEditMultipleObjects]
    public class UIEffect2Editor : Editor
    {
        private static readonly PropertyInfo s_PiGradient = typeof(SerializedProperty)
            .GetProperty("gradientValue", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly Func<SerializedProperty, Gradient> s_GetGradient =
            (Func<SerializedProperty, Gradient>)Delegate.CreateDelegate(typeof(Func<SerializedProperty, Gradient>),
                s_PiGradient.GetMethod);

        private static readonly Action<SerializedProperty, Gradient> s_SetGradient =
            (Action<SerializedProperty, Gradient>)Delegate.CreateDelegate(typeof(Action<SerializedProperty, Gradient>),
                s_PiGradient.SetMethod);

        private SerializedProperty _toneFilter;
        private SerializedProperty _toneIntensity;

        private SerializedProperty _colorFilter;
        private SerializedProperty _color;
        private SerializedProperty _colorIntensity;
        private SerializedProperty _colorGlow;

        private SerializedProperty _samplingFilter;
        private SerializedProperty _samplingIntensity;
        private SerializedProperty _samplingScale;

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
        private SerializedProperty _transitionColorGlow;

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
        private SerializedProperty _shadowMirrorScale;
        private SerializedProperty _shadowBlurIntensity;
        private SerializedProperty _shadowColorFilter;
        private SerializedProperty _shadowColor;
        private SerializedProperty _shadowColorGlow;

        private SerializedProperty _gradationMode;
        private SerializedProperty _gradationColor1;
        private SerializedProperty _gradationColor2;
        private SerializedProperty _gradationGradient;
        private SerializedProperty _gradationOffset;
        private SerializedProperty _gradationScale;
        private SerializedProperty _gradationRotation;

        private bool _expandOthers = true;
        private SerializedProperty _allowToModifyMeshShape;

        private void OnEnable()
        {
            if (target == null) return;

            _toneFilter = serializedObject.FindProperty("m_ToneFilter");
            _toneIntensity = serializedObject.FindProperty("m_ToneIntensity");

            _colorFilter = serializedObject.FindProperty("m_ColorFilter");
            _color = serializedObject.FindProperty("m_Color");
            _colorIntensity = serializedObject.FindProperty("m_ColorIntensity");
            _colorGlow = serializedObject.FindProperty("m_ColorGlow");

            _samplingFilter = serializedObject.FindProperty("m_SamplingFilter");
            _samplingIntensity = serializedObject.FindProperty("m_SamplingIntensity");
            _samplingScale = serializedObject.FindProperty("m_SamplingScale");

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
            _transitionColorGlow = serializedObject.FindProperty("m_TransitionColorGlow");

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
            _shadowMirrorScale = serializedObject.FindProperty("m_ShadowMirrorScale");
            _shadowBlurIntensity = serializedObject.FindProperty("m_ShadowBlurIntensity");
            _shadowColorFilter = serializedObject.FindProperty("m_ShadowColorFilter");
            _shadowColor = serializedObject.FindProperty("m_ShadowColor");
            _shadowColorGlow = serializedObject.FindProperty("m_ShadowColorGlow");

            _gradationMode = serializedObject.FindProperty("m_GradationMode");
            _gradationColor1 = serializedObject.FindProperty("m_GradationColor1");
            _gradationColor2 = serializedObject.FindProperty("m_GradationColor2");
            _gradationGradient = serializedObject.FindProperty("m_GradationGradient");
            _gradationOffset = serializedObject.FindProperty("m_GradationOffset");
            _gradationScale = serializedObject.FindProperty("m_GradationScale");
            _gradationRotation = serializedObject.FindProperty("m_GradationRotation");

            _allowToModifyMeshShape = serializedObject.FindProperty("m_AllowToModifyMeshShape");
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
                DrawColor(_colorFilter, _color, prevColorFilter, false);
                EditorGUILayout.PropertyField(_colorGlow);
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
                    EditorGUILayout.PropertyField(_transitionColorGlow);
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

                EditorGUILayout.PropertyField(_shadowColorFilter);
                DrawColorPickerField(_shadowColor, false);
                EditorGUILayout.PropertyField(_shadowColorGlow);
                EditorGUILayout.PropertyField(_shadowFade);

                switch ((SamplingFilter)_samplingFilter.intValue)
                {
                    case SamplingFilter.BlurFast:
                    case SamplingFilter.BlurMedium:
                    case SamplingFilter.BlurDetail:
                        EditorGUILayout.PropertyField(_shadowBlurIntensity);
                        break;
                }

                EditorGUI.indentLevel--;
            }

            // Gradient
            DrawSeparator();
            if (DrawHeaderPopup(_gradationMode))
            {
                EditorGUI.indentLevel++;
                switch ((GradationMode)_gradationMode.intValue)
                {
                    case GradationMode.HorizontalGradient:
                    case GradationMode.VerticalGradient:
                    case GradationMode.AngleGradient:
                        DrawGradientField(_gradationGradient);
                        break;
                    default:
                        DrawColorPickerField(_gradationColor1);
                        var r = EditorGUILayout.GetControlRect();
                        r.width -= 24;
                        r.height = EditorGUIUtility.singleLineHeight;
                        DrawColorPickerField(r, _gradationColor2);

                        r.x += r.width + 4;
                        r.width = 20;
                        // Swap colors
                        if (GUI.Button(r, EditorGUIUtility.IconContent("preaudioloopoff"), "iconbutton"))
                        {
                            (_gradationColor1.colorValue, _gradationColor2.colorValue)
                                = (_gradationColor2.colorValue, _gradationColor1.colorValue);
                        }

                        break;
                }

                EditorGUILayout.PropertyField(_gradationOffset);
                EditorGUILayout.PropertyField(_gradationScale);

                if ((GradationMode)_gradationMode.intValue == GradationMode.Angle
                    || (GradationMode)_gradationMode.intValue == GradationMode.AngleGradient)
                {
                    EditorGUILayout.PropertyField(_gradationRotation);
                }

                EditorGUI.indentLevel--;
            }

            DrawSeparator();
            _expandOthers = EditorGUILayout.BeginFoldoutHeaderGroup(_expandOthers, "Others");
            if (_expandOthers)
            {
                EditorGUILayout.PropertyField(_samplingScale);
                EditorGUILayout.PropertyField(_allowToModifyMeshShape);
            }
        }

        private static void DrawColorPickerField(SerializedProperty color, bool showAlpha = true)
        {
            var r = EditorGUILayout.GetControlRect();
            r.height = EditorGUIUtility.singleLineHeight;
            DrawColorPickerField(r, color, showAlpha);
        }

        private static void DrawColorPickerField(Rect rect, SerializedProperty color, bool showAlpha = true)
        {
            var label = EditorGUIUtility.TrTempContent(color.displayName);
            label.tooltip = color.tooltip;
            var hdr = UIEffectProjectSettings.useHdrColorPicker;
            EditorGUI.showMixedValue = color.hasMultipleDifferentValues;

            EditorGUI.BeginChangeCheck();
            var colorValue = EditorGUI.ColorField(rect, label, color.colorValue, true, showAlpha, hdr);
            if (EditorGUI.EndChangeCheck())
            {
                color.colorValue = colorValue;
            }
        }

        private static void DrawGradientField(SerializedProperty gradient)
        {
            var r = EditorGUILayout.GetControlRect();
            r.height = EditorGUIUtility.singleLineHeight;

            var label = EditorGUIUtility.TrTempContent(gradient.displayName);
            label.tooltip = gradient.tooltip;
            var hdr = UIEffectProjectSettings.useHdrColorPicker;
            EditorGUI.showMixedValue = gradient.hasMultipleDifferentValues;

            EditorGUI.BeginChangeCheck();
            var gradientValue = EditorGUI.GradientField(r, label, s_GetGradient(gradient), hdr);
            if (EditorGUI.EndChangeCheck())
            {
                s_SetGradient(gradient, gradientValue);
            }
        }

        private static void DrawColor(SerializedProperty filter, SerializedProperty color, ColorFilter prevFilter,
            bool showAlpha = true)
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

                DrawColorPickerField(color, showAlpha);
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
                EditorApplication.delayCall += () =>
                {
                    UIEffectProjectSettings.SaveAsNewPreset(targets.OfType<UIEffect>().FirstOrDefault());
                };
            }
        }
    }
}
