using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    Planet planet;
    Editor planetEditor;
    Editor lodEditor;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DrawSettingsEditor(planet.planetSettings, ref planet.planetSettingsFoldout, ref planetEditor);
        DrawSettingsEditor(planet.lodSettings, ref planet.lodSettingsFoldout, ref lodEditor);
    }

    void DrawSettingsEditor(Object settings, ref bool foldout, ref Editor editor)
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            if(settings != null)
            {
                foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

                if(foldout)
                {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();
                }
            }
            if(check.changed)
            {
                //Update Editor Planet
            }
        }
    }

    private void OnEnable() 
    {
        planet = (Planet)target;
    }
}
