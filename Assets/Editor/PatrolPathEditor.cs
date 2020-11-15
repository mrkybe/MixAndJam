using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets
{
    [CustomEditor(typeof(PatrolPath))]
    class PatrolPathEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Reverse"))
            {
                var t = target as PatrolPath;
                var children = new List<Transform>();
                foreach (Transform child in t.transform)
                {
                    children.Add(child);
                }
                int count = t.transform.childCount;
                foreach (Transform child in children)
                {
                    child.SetSiblingIndex(count - 1);
                    count--;
                }
            }
        }
    }
}
