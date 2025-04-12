using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using Object = UnityEngine.Object;

namespace Coffee.UIEffects.Editors
{
    /// <summary>
    /// UIEffect editor.
    /// </summary>
    [CustomEditor(typeof(UIEffect), true)]
    [CanEditMultipleObjects]
    public class UIEffectEditor : UIEffectPropertyEditor
    {
        private class UIEffectEditorSettings : ScriptableSingleton<UIEffectEditorSettings>
        {
            public bool m_ExpandOthers = true;
        }

        private SerializedProperty _samplingScale;
        private SerializedProperty _allowToModifyMeshShape;
        private SerializedProperty _customRoot;

        private static bool expandOthers
        {
            set => UIEffectEditorSettings.instance.m_ExpandOthers = value;
            get => UIEffectEditorSettings.instance.m_ExpandOthers;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _samplingScale = serializedObject.FindProperty("m_SamplingScale");
            _allowToModifyMeshShape = serializedObject.FindProperty("m_AllowToModifyMeshShape");
            _customRoot = serializedObject.FindProperty("m_CustomRoot");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Preset
            DrawPresetMenu(targets);
            DrawSeparator();

            // Properties
            DrawProperties();

            // Others
            DrawSeparator();
            expandOthers = EditorGUILayout.BeginFoldoutHeaderGroup(expandOthers, "Others");
            if (expandOthers)
            {
                EditorGUILayout.PropertyField(_samplingScale);
                EditorGUILayout.PropertyField(_allowToModifyMeshShape);
                EditorGUILayout.PropertyField(_customRoot);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
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

        public static void DropDownPreset(Rect r, Predicate<(string path, bool builtin, UIEffectPreset preset)> valid,
            Action<UIEffectPreset> callback)
        {
            var menu = new GenericMenu();
            var separatorAdded = false;
            foreach (var preset in UIEffectProjectSettings.LoadEditorPresetsV2())
            {
                var (path, builtin) = UIEffectProjectSettings.GetPresetPath(preset);
                if (valid != null && valid.Invoke((path, builtin, preset)) == false) continue;

                if (builtin && !separatorAdded)
                {
                    separatorAdded = true;
                    menu.AddSeparator(string.Empty);
                }

                menu.AddItem(new GUIContent(path.Replace("-", "/")), false, x => callback(x as UIEffectPreset), preset);
            }

            menu.DropDown(r);
        }
    }
}
