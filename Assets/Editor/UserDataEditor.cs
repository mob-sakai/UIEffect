using UnityEditor;

internal static class UserDataEditor
{
    [InitializeOnLoadMethod]
    private static void InitializeOnLoadMethod()
    {
        Editor.finishedDefaultHeaderGUI += OnFinishedDefaultHeaderGUI;
    }

    private static void OnFinishedDefaultHeaderGUI(Editor editor)
    {
        var importer = editor.target as AssetImporter;
        if (!importer)
        {
            if (!AssetDatabase.Contains(editor.target)) return;

            importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(editor.target));
        }

        if (!importer) return;

        EditorGUIUtility.labelWidth = 55;
        importer.userData = EditorGUILayout.DelayedTextField("userdata", importer.userData);
        AssetDatabase.WriteImportSettingsIfDirty(importer.assetPath);
    }
}
