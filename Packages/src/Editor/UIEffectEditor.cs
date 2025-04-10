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
    public class UIEffectEditor : Editor
    {
        private class UIEffectEditorSettings : ScriptableSingleton<UIEffectEditorSettings>
        {
            public bool m_ExpandOthers = true;
        }

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
        private SerializedProperty _samplingWidth;
        private SerializedProperty _samplingScale;

        private SerializedProperty _transitionFilter;
        private SerializedProperty _transitionRate;
        private SerializedProperty _transitionTex;
        private SerializedProperty _transitionTexScale;
        private SerializedProperty _transitionTexOffset;
        private SerializedProperty _transitionTexSpeed;
        private SerializedProperty _transitionReverse;
        private SerializedProperty _transitionRotation;
        private SerializedProperty _transitionKeepAspectRatio;
        private SerializedProperty _transitionWidth;
        private SerializedProperty _transitionSoftness;
        private SerializedProperty _transitionRange;
        private SerializedProperty _transitionColorFilter;
        private SerializedProperty _transitionColor;
        private SerializedProperty _transitionColorGlow;
        private SerializedProperty _transitionPatternReverse;
        private SerializedProperty _transitionAutoPlaySpeed;

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
        private SerializedProperty _gradationIntensity;
        private SerializedProperty _gradationColorFilter;
        private SerializedProperty _gradationColor1;
        private SerializedProperty _gradationColor2;
        private SerializedProperty _gradationColor3;
        private SerializedProperty _gradationColor4;
        private SerializedProperty _gradationGradient;
        private SerializedProperty _gradationOffset;
        private SerializedProperty _gradationScale;
        private SerializedProperty _gradationRotation;

        private SerializedProperty _edgeMode;
        private SerializedProperty _edgeWidth;
        private SerializedProperty _edgeColorFilter;
        private SerializedProperty _edgeColor;
        private SerializedProperty _edgeColorGlow;
        private SerializedProperty _edgeShinyRate;
        private SerializedProperty _edgeShinyWidth;
        private SerializedProperty _edgeShinyAutoPlaySpeed;
        private SerializedProperty _patternArea;

        private SerializedProperty _detailFilter;
        private SerializedProperty _detailIntensity;
        private SerializedProperty _detailThreshold;
        private SerializedProperty _detailTex;
        private SerializedProperty _detailTexScale;
        private SerializedProperty _detailTexOffset;
        private SerializedProperty _detailTexSpeed;

        private SerializedProperty _allowToModifyMeshShape;
        private SerializedProperty _customRoot;
        private SerializedProperty _flip;

        private static bool expandOthers
        {
            set => UIEffectEditorSettings.instance.m_ExpandOthers = value;
            get => UIEffectEditorSettings.instance.m_ExpandOthers;
        }

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
            _samplingWidth = serializedObject.FindProperty("m_SamplingWidth");
            _samplingScale = serializedObject.FindProperty("m_SamplingScale");

            _transitionFilter = serializedObject.FindProperty("m_TransitionFilter");
            _transitionRate = serializedObject.FindProperty("m_TransitionRate");
            _transitionTex = serializedObject.FindProperty("m_TransitionTex");
            _transitionTexScale = serializedObject.FindProperty("m_TransitionTexScale");
            _transitionTexOffset = serializedObject.FindProperty("m_TransitionTexOffset");
            _transitionTexSpeed = serializedObject.FindProperty("m_TransitionTexSpeed");
            _transitionReverse = serializedObject.FindProperty("m_TransitionReverse");
            _transitionRotation = serializedObject.FindProperty("m_TransitionRotation");
            _transitionKeepAspectRatio = serializedObject.FindProperty("m_TransitionKeepAspectRatio");
            _transitionWidth = serializedObject.FindProperty("m_TransitionWidth");
            _transitionSoftness = serializedObject.FindProperty("m_TransitionSoftness");
            _transitionRange = serializedObject.FindProperty("m_TransitionRange");
            _transitionColorFilter = serializedObject.FindProperty("m_TransitionColorFilter");
            _transitionColor = serializedObject.FindProperty("m_TransitionColor");
            _transitionColorGlow = serializedObject.FindProperty("m_TransitionColorGlow");
            _transitionPatternReverse = serializedObject.FindProperty("m_TransitionPatternReverse");
            _transitionAutoPlaySpeed = serializedObject.FindProperty("m_TransitionAutoPlaySpeed");

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

            _edgeMode = serializedObject.FindProperty("m_EdgeMode");
            _edgeWidth = serializedObject.FindProperty("m_EdgeWidth");
            _edgeColorFilter = serializedObject.FindProperty("m_EdgeColorFilter");
            _edgeColor = serializedObject.FindProperty("m_EdgeColor");
            _edgeColorGlow = serializedObject.FindProperty("m_EdgeColorGlow");
            _edgeShinyRate = serializedObject.FindProperty("m_EdgeShinyRate");
            _edgeShinyWidth = serializedObject.FindProperty("m_EdgeShinyWidth");
            _edgeShinyAutoPlaySpeed = serializedObject.FindProperty("m_EdgeShinyAutoPlaySpeed");
            _patternArea = serializedObject.FindProperty("m_PatternArea");

            _gradationMode = serializedObject.FindProperty("m_GradationMode");
            _gradationIntensity = serializedObject.FindProperty("m_GradationIntensity");
            _gradationColorFilter = serializedObject.FindProperty("m_GradationColorFilter");
            _gradationColor1 = serializedObject.FindProperty("m_GradationColor1");
            _gradationColor2 = serializedObject.FindProperty("m_GradationColor2");
            _gradationColor3 = serializedObject.FindProperty("m_GradationColor3");
            _gradationColor4 = serializedObject.FindProperty("m_GradationColor4");
            _gradationGradient = serializedObject.FindProperty("m_GradationGradient");
            _gradationOffset = serializedObject.FindProperty("m_GradationOffset");
            _gradationScale = serializedObject.FindProperty("m_GradationScale");
            _gradationRotation = serializedObject.FindProperty("m_GradationRotation");

            _detailFilter = serializedObject.FindProperty("m_DetailFilter");
            _detailIntensity = serializedObject.FindProperty("m_DetailIntensity");
            _detailThreshold = serializedObject.FindProperty("m_DetailThreshold");
            _detailTex = serializedObject.FindProperty("m_DetailTex");
            _detailTexScale = serializedObject.FindProperty("m_DetailTexScale");
            _detailTexOffset = serializedObject.FindProperty("m_DetailTexOffset");
            _detailTexSpeed = serializedObject.FindProperty("m_DetailTexSpeed");

            _allowToModifyMeshShape = serializedObject.FindProperty("m_AllowToModifyMeshShape");
            _customRoot = serializedObject.FindProperty("m_CustomRoot");
            _flip = serializedObject.FindProperty("m_Flip");
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
                DrawColor(_color, (ColorFilter)_colorFilter.intValue, prevColorFilter, false);
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
                if (_samplingFilter.intValue == (int)SamplingFilter.EdgeAlpha
                    || _samplingFilter.intValue == (int)SamplingFilter.EdgeLuminance)
                {
                    EditorGUILayout.PropertyField(_samplingWidth);
                }

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
                EditorGUILayout.PropertyField(_transitionTexSpeed, EditorGUIUtility.TrTempContent("Speed"));
                if (_transitionTexSpeed.vector2Value != Vector2.zero)
                {
                    EditorApplication.QueuePlayerLoopUpdate();
                }

                EditorGUILayout.PropertyField(_transitionRotation, EditorGUIUtility.TrTempContent("Rotation"));
                EditorGUILayout.PropertyField(_transitionKeepAspectRatio,
                    EditorGUIUtility.TrTempContent("Keep Aspect Ratio"));
                EditorGUILayout.PropertyField(_transitionReverse, EditorGUIUtility.TrTempContent("Reverse"));
                EditorGUI.indentLevel--;

                if (_transitionFilter.intValue == (int)TransitionFilter.Pattern)
                {
                    EditorGUILayout.PropertyField(_transitionWidth, EditorGUIUtility.TrTempContent("Pattern Size"));
                    EditorGUILayout.PropertyField(_transitionRange, EditorGUIUtility.TrTempContent("Pattern Range"));
                    EditorGUILayout.PropertyField(_transitionPatternReverse,
                        EditorGUIUtility.TrTempContent("Pattern Reverse"));
                    DrawColor(_transitionColorFilter, _transitionColor, null);
                }
                else if (2 < _transitionFilter.intValue)
                {
                    EditorGUILayout.PropertyField(_transitionWidth);
                    EditorGUILayout.PropertyField(_transitionSoftness);
                    DrawColor(_transitionColorFilter, _transitionColor, _transitionColorGlow);
                }

                EditorGUILayout.PropertyField(_transitionAutoPlaySpeed);
                if (0 < _transitionFilter.intValue && !Mathf.Approximately(0, _transitionAutoPlaySpeed.floatValue))
                {
                    EditorApplication.QueuePlayerLoopUpdate();
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
                    EditorGUILayout.PropertyField(_shadowFade);
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
                    DrawColor(_shadowColorFilter, _shadowColor, _shadowColorGlow, false);
                    EditorGUILayout.PropertyField(_shadowFade);
                }

                // Shadow blur intensity
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
                EditorGUILayout.PropertyField(_gradationColorFilter);
                switch ((GradationMode)_gradationMode.intValue)
                {
                    case GradationMode.HorizontalGradient:
                    case GradationMode.VerticalGradient:
                    case GradationMode.AngleGradient:
                        DrawGradientField(_gradationGradient);
                        break;
                    case GradationMode.Diagonal:
                        DrawColorPickerField("Top", _gradationColor1, _gradationColor2);
                        DrawColorPickerField("Bottom", _gradationColor3, _gradationColor4);
                        break;
                    default:
                        DrawColorPickerField(EditorGUILayout.GetControlRect(), _gradationColor1, true);
                        var r = EditorGUILayout.GetControlRect();
                        r.width -= 24;
                        DrawColorPickerField(r, _gradationColor2, true);

                        r.x += r.width + 4;
                        r.width = 20;
                        SwapColorsButton(r, _gradationColor1, _gradationColor2);
                        break;
                }

                EditorGUILayout.PropertyField(_gradationIntensity);
                EditorGUILayout.PropertyField(_gradationOffset);
                EditorGUILayout.PropertyField(_gradationScale);

                if ((GradationMode)_gradationMode.intValue == GradationMode.Angle
                    || (GradationMode)_gradationMode.intValue == GradationMode.AngleGradient)
                {
                    EditorGUILayout.PropertyField(_gradationRotation);
                }

                EditorGUILayout.PropertyField(_transitionKeepAspectRatio,
                    EditorGUIUtility.TrTempContent("Keep Aspect Ratio"));
                EditorGUI.indentLevel--;
            }

            // Edge Mode
            DrawSeparator();
            if (DrawHeaderPopup(_edgeMode))
            {
                EditorGUILayout.PropertyField(_edgeWidth);
                DrawColor(_edgeColorFilter, _edgeColor, _edgeColorGlow);

                if ((EdgeMode)_edgeMode.intValue == EdgeMode.Shiny)
                {
                    EditorGUILayout.PropertyField(_edgeShinyRate);
                    EditorGUILayout.PropertyField(_edgeShinyWidth);
                    EditorGUILayout.PropertyField(_edgeShinyAutoPlaySpeed);

                    if (!Mathf.Approximately(0, _edgeShinyAutoPlaySpeed.floatValue))
                    {
                        EditorApplication.QueuePlayerLoopUpdate();
                    }
                }

                if (_transitionFilter.intValue == (int)TransitionFilter.Pattern)
                {
                    EditorGUILayout.PropertyField(_patternArea);
                }
            }

            // Detail filter
            DrawSeparator();
            if (DrawHeaderPopup(_detailFilter))
            {
                EditorGUI.indentLevel++;
                if ((DetailFilter)_detailFilter.intValue == DetailFilter.Masking)
                {
                    EditorGUILayout.PropertyField(_detailThreshold);
                }
                else
                {
                    EditorGUILayout.PropertyField(_detailIntensity);
                }

                EditorGUILayout.PropertyField(_detailTex);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_detailTexScale, EditorGUIUtility.TrTempContent("Scale"));
                EditorGUILayout.PropertyField(_detailTexOffset, EditorGUIUtility.TrTempContent("Offset"));
                EditorGUILayout.PropertyField(_detailTexSpeed, EditorGUIUtility.TrTempContent("Speed"));
                if (_detailTexSpeed.vector2Value != Vector2.zero)
                {
                    EditorApplication.QueuePlayerLoopUpdate();
                }

                EditorGUILayout.PropertyField(_transitionRotation, EditorGUIUtility.TrTempContent("Rotation"));
                EditorGUILayout.PropertyField(_transitionKeepAspectRatio,
                    EditorGUIUtility.TrTempContent("Keep Aspect Ratio"));
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }

            DrawSeparator();
            expandOthers = EditorGUILayout.BeginFoldoutHeaderGroup(expandOthers, "Others");
            if (expandOthers)
            {
                EditorGUILayout.PropertyField(_samplingScale);
                EditorGUILayout.PropertyField(_allowToModifyMeshShape);
                EditorGUILayout.PropertyField(_customRoot);
                EditorGUILayout.PropertyField(_flip);
            }
        }

        private static void DrawColorPickerField(string label, SerializedProperty color1, SerializedProperty color2)
        {
            var labelWidth = EditorGUIUtility.labelWidth;
            var r = EditorGUILayout.GetControlRect();
            r.width -= 24;
            EditorGUI.PrefixLabel(new Rect(r.x, r.y, labelWidth, r.height), EditorGUIUtility.TrTempContent(label));

            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            var rPos = new Rect(r.x + labelWidth, r.y, (r.width - labelWidth) / 2, r.height);
            DrawColorPickerField(rPos, GUIContent.none, color1, true);

            rPos.x += rPos.width;
            DrawColorPickerField(rPos, GUIContent.none, color2, true);

            // Swap colors button
            SwapColorsButton(new Rect(r.x + r.width + 4, r.y, 20, 20), color1, color2);
            EditorGUI.indentLevel = indentLevel;
        }

        private static void DrawColorPickerField(Rect rect, SerializedProperty color, bool showAlpha)
        {
            var label = EditorGUIUtility.TrTempContent(color.displayName);
            label.tooltip = color.tooltip;
            DrawColorPickerField(rect, label, color, showAlpha);
        }

        private static void DrawColorPickerField(Rect rect, GUIContent label, SerializedProperty color, bool showAlpha)
        {
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

        private static void DrawColor(SerializedProperty color, ColorFilter filter, ColorFilter prevFilter,
            bool showAlpha = true)
        {
            if (filter == ColorFilter.None) return;

            if (filter == ColorFilter.HsvModifier)
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
            else if (filter == ColorFilter.Contrast)
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

                DrawColorPickerField(EditorGUILayout.GetControlRect(), color, showAlpha);
            }
        }

        private static void DrawColor(SerializedProperty filter, SerializedProperty color, SerializedProperty glow,
            bool showAlpha = true)
        {
            var prevFilter = (ColorFilter)filter.intValue;
            EditorGUILayout.PropertyField(filter);
            if (filter.intValue == (int)ColorFilter.None) return;

            EditorGUI.indentLevel++;
            DrawColor(color, (ColorFilter)filter.intValue, prevFilter, showAlpha);

            if (glow != null)
            {
                EditorGUILayout.PropertyField(glow);
            }

            EditorGUI.indentLevel--;
        }

        private static void SwapColorsButton(Rect rect, SerializedProperty color1, SerializedProperty color2)
        {
            if (GUI.Button(rect, EditorGUIUtility.IconContent("preaudioloopoff"), "iconbutton"))
            {
                (color1.colorValue, color2.colorValue) = (color2.colorValue, color1.colorValue);
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
            r.width = (r.width - 185) / 2;
            if (GUI.Button(r, EditorGUIUtility.TrTempContent("Load"), "MiniPopup"))
            {
                DropDownPreset(r, null, p =>
                {
                    Undo.RecordObjects(targets, "Load UIEffect Preset");
                    Array.ForEach(targets.OfType<UIEffect>().ToArray(), t =>
                    {
                        t.LoadPreset(p, false);
                    });
                });
            }

            r.x += r.width;
            if (GUI.Button(r, EditorGUIUtility.TrTempContent("Append"), "MiniPopup"))
            {
                DropDownPreset(r, null, p =>
                {
                    Undo.RecordObjects(targets, "Append UIEffect Preset");
                    Array.ForEach(targets.OfType<UIEffect>().ToArray(), t =>
                    {
                        t.LoadPreset(p, true);
                    });
                });
            }

            r.x += r.width;
            r.width = 15;
            GUI.Label(r, GUIContent.none, "BreadcrumbsSeparator");

            r.x += r.width;
            r.width = 100;
            if (GUI.Button(r, EditorGUIUtility.TrTempContent("Save As New"), "MiniButton"))
            {
                EditorApplication.delayCall += () =>
                {
                    UIEffectProjectSettings.SaveAsNewPreset(targets.OfType<UIEffect>().FirstOrDefault());
                };
            }

            r.x += r.width;
            r.width = 15;
            GUI.Label(r, GUIContent.none, "BreadcrumbsSeparator");

            r.x += r.width;
            r.width = 55;
            if (GUI.Button(r, EditorGUIUtility.TrTempContent("Clear"), "MiniButton"))
            {
                Undo.RecordObjects(targets, "Clear UIEffect");
                Array.ForEach(targets.OfType<UIEffect>().ToArray(), Unsupported.SmartReset);
            }
        }

        public static void DropDownPreset(Rect r, Predicate<(string path, bool builtin, UIEffect preset)> valid,
            Action<UIEffect> callback)
        {
            var menu = new GenericMenu();
            var separatorAdded = false;
            foreach (var preset in UIEffectProjectSettings.LoadEditorPresets())
            {
                var (path, builtin) = UIEffectProjectSettings.GetPresetPath(preset);
                if (valid != null && valid.Invoke((path, builtin, preset)) == false) continue;

                if (builtin)
                {
                    if (!separatorAdded)
                    {
                        separatorAdded = true;
                        menu.AddSeparator(string.Empty);
                    }

                    path = path.Substring(6).Replace(" - ", "/");
                }

                menu.AddItem(new GUIContent(path), false, x => callback(x as UIEffect), preset);
            }

            menu.DropDown(r);
        }
    }
}
