using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridPickupPlacer))]
public class GridPickupPlacerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate"))
        {
            var generator = (GridPickupPlacer)target;

            var casters = generator.Generate();

            for (var i = 0; i < casters.Length; i++)
            {
                casters[i].name += "_" + i.ToString();
            }
        }
    }
}
