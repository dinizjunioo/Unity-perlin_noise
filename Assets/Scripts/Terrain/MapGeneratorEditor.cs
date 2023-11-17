using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGeneratorTwo))]
public class MapGeneratorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        MapGeneratorTwo mapGen = (MapGeneratorTwo)target;

        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.DrawMapInEditor();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.DrawMapInEditor();
        }
    }
}
