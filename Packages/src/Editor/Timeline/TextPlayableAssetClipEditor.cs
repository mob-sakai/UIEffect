#if TIMELINE_ENABLE
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace Coffee.UIEffects.Timeline
{
    public abstract class UIEffectClipEditor : ClipEditor
    {
        public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
        {
            var director = TimelineEditor.inspectedDirector;
            if (!director) return;

            var viewDuration = region.endTime - region.startTime;
            var inDuration = (float)Math.Max(clip.easeInDuration, clip.blendInDuration);
            var outDuration = (float)Math.Max(clip.easeOutDuration, clip.blendOutDuration);
            inDuration = 0 < inDuration ? Mathf.Max(inDuration - (float)region.startTime, 0) : 0f;
            outDuration = 0 < outDuration ? Mathf.Max(outDuration - (float)(clip.duration - region.endTime), 0) : 0f;

            var p = region.position;
            p.x += p.width * (float)(inDuration / viewDuration);
            p.width -= p.width * (float)((inDuration + outDuration) / viewDuration);

            DrawBackground(p, clip);
        }

        public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
        {
            clip.displayName = ObjectNames.NicifyVariableName(track.GetType().Name.Replace("Track", ""));
            if (clip.asset is UIEffectClip uiEffectClip)
            {
                uiEffectClip.timelineClip = clip;
            }

            var floatClip = clip.asset as UIEffectFloatClip;
            if (floatClip)
            {
                if (track is GradationRotationTrack || track is GradationOffsetTrack)
                {
                    floatClip.m_Data.m_From = 0f;
                    floatClip.m_Data.m_Value = 0f;
                }
            }
        }

        protected abstract void DrawBackground(Rect rect, TimelineClip clip);
    }

    [CustomTimelineEditor(typeof(UIEffectFloatClip))]
    public class UIEffectFloatClipEditor : UIEffectClipEditor
    {
        private static GUIStyle s_FromStyle;
        private static GUIStyle s_ToStyle;

        protected override void DrawBackground(Rect rect, TimelineClip clip)
        {
            var asset = clip.asset as UIEffectFloatClip;
            if (!asset) return;

            if (s_FromStyle == null)
            {
                s_FromStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperLeft, fontSize = 9 };
                s_ToStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperRight, fontSize = 9 };
            }

            var d = asset.m_Data;
            if (d.m_Tween)
            {
                DrawLabelWithShadow(rect, asset.m_Data.m_From.ToString("F2"), s_FromStyle);
                DrawLabelWithShadow(rect, asset.m_Data.m_Value.ToString("F2"), s_ToStyle);
            }
            else
            {
                DrawLabelWithShadow(rect, asset.m_Data.m_Value.ToString("F2"), s_FromStyle);
            }
        }

        private static void DrawLabelWithShadow(Rect position, string text, GUIStyle style)
        {
            GUI.color = Color.black;
            EditorGUI.LabelField(position, text, style);
            GUI.color = Color.white;
            position.x -= 1f;
            position.y -= 1f;
            EditorGUI.LabelField(position, text, style);
        }
    }

    [CustomTimelineEditor(typeof(UIEffectColorClip))]
    public class UIEffectColorClipEditor : UIEffectClipEditor
    {
        protected override void DrawBackground(Rect p, TimelineClip clip)
        {
            var asset = clip.asset as UIEffectColorClip;
            if (!asset) return;

            var d = asset.m_Data;
            if (d.m_Tween)
            {
                EditorGUIUtility.DrawColorSwatch(new Rect(p.x + 2, p.y, 8, 8), d.m_From);
                EditorGUIUtility.DrawColorSwatch(new Rect(p.x + p.width - 8, p.y, 8, 8), d.m_Value);
            }
            else
            {
                EditorGUIUtility.DrawColorSwatch(new Rect(p.x + 2, p.y, 8, 8), d.m_Value);
            }
        }
    }

    [CustomEditor(typeof(UIEffectClip<>), true)]
    public class UIEffectClipAssetEditor : Editor
    {
        private SerializedProperty _tween;
        private SerializedProperty _from;
        private SerializedProperty _curve;
        private SerializedProperty _value;
        private FloatClipUsageAttribute _floatUsage;
        private ColorClipUsageAttribute _colorUsage;
        private GUIContent _label;

        private void OnEnable()
        {
            var data = serializedObject.FindProperty("m_Data");
            _tween = data.FindPropertyRelative("m_Tween");
            _from = data.FindPropertyRelative("m_From");
            _curve = data.FindPropertyRelative("m_Curve");
            _value = data.FindPropertyRelative("m_Value");

            var clip = target as UIEffectClip;
            var type = clip.timelineClip.GetParentTrack().GetType();
            _floatUsage = type.GetCustomAttribute<FloatClipUsageAttribute>();
            _colorUsage = type.GetCustomAttribute<ColorClipUsageAttribute>();
            _label = new GUIContent(ObjectNames.NicifyVariableName(type.Name).Replace(" Track", ""));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_tween);

            if (_tween.boolValue)
            {
                EditorGUI.indentLevel++;
                DrawProperty(_from, EditorGUIUtility.TrTempContent("From"));
                EditorGUILayout.PropertyField(_curve);
                EditorGUI.indentLevel--;
            }

            DrawProperty(_value, _label);
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawProperty(SerializedProperty property, GUIContent label)
        {
            if (_floatUsage != null)
            {
                EditorGUILayout.Slider(property, _floatUsage.min, _floatUsage.max, label);
            }
            else if (property.propertyType == SerializedPropertyType.Color)
            {
                var hdr = UIEffectProjectSettings.useHdrColorPicker;
                var alpha = _colorUsage?.alpha ?? true;

                EditorGUI.BeginChangeCheck();
                var color = EditorGUILayout.ColorField(label, property.colorValue, true, alpha, hdr);
                if (EditorGUI.EndChangeCheck())
                {
                    property.colorValue = color;
                }
            }
            else
            {
                EditorGUILayout.PropertyField(property, label);
            }
        }
    }
}
#endif
