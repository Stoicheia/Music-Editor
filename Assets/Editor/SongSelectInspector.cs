using IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(SongSelect))]
    public class SongSelectInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            SongSelect obj = (SongSelect) target;
            base.OnInspectorGUI();
            if (GUILayout.Button("Load"))
            {
                obj.OpenSong();
            }
        }
    }
}