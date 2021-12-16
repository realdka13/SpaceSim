using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BodyGenerator))]
public class BodyGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BodyGenerator myScript = (BodyGenerator)target;
        if (GUILayout.Button("Create Body"))
        {
            myScript.CreatebodyGameObject();
        }
    }
}