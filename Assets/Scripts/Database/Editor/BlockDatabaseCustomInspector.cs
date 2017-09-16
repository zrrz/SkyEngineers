using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BlockDatabaseScriptableObject))]
public class BlockDatabaseCustomInspector : Editor {

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open Block Database"))
        {
            BlockDatabaseEditorWindow.Init();
        }
//        LevelScript myTarget = (LevelScript)target;
//
//        myTarget.experience = EditorGUILayout.IntField("Experience", myTarget.experience);
//        EditorGUILayout.LabelField("Level", myTarget.Level.ToString());
    }
}
