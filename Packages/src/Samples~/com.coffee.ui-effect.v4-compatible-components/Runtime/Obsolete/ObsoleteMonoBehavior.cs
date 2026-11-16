using UnityEngine;
using UnityEditor;


namespace Coffee.UIEffects
{
#if UNITY_EDITOR
    [CustomEditor(typeof(ObsoleteMonoBehaviour), true)]
    internal class ObsoleteMonoBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This component is obsolete.", MessageType.Warning);
        }
    }
#endif

    public abstract class ObsoleteMonoBehaviour : MonoBehaviour
    {
    }
}
