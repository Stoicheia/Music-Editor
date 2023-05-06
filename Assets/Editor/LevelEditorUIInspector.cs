using UI;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(LevelEditorUI))]
    public class LevelEditorUIInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            LevelEditorUI obj = (LevelEditorUI) target;
            base.OnInspectorGUI();
            if (GUILayout.Button("Load"))
            {
                obj.InitFromAsset();
            }
        }
    }
}