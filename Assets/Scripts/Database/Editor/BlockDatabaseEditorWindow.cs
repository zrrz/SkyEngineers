using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BlockDatabaseEditorWindow : EditorWindow {

    [MenuItem("Window/Block Editor")]
    public static void Init()
    {
        // Get existing open window or if none, make a new one:
        BlockDatabaseEditorWindow window = (BlockDatabaseEditorWindow)EditorWindow.GetWindow(typeof(BlockDatabaseEditorWindow));
        window.Show();
    }

    BlockDatabaseScriptableObject blockDatabase;

    void OnEnable() {
        blockDatabase = AssetDatabase.LoadAssetAtPath<BlockDatabaseScriptableObject>("Assets/Scripts/Database/BlockDatabase.asset");
    }

    void OnGUI()
    {
        for (int i = 0; i < blockDatabase.blocks.Count; i++)
        {

        }


        //TODO indicate uncompiled through graphic or color
        if (GUILayout.Button("Compile Database"))
        {
            BuildAtlas();
            AssetDatabase.SaveAssets();
        }
    }

    void BuildAtlas() {
        for (int i = 0; i < blockDatabase.blocks.Count; i++)
        {
            //TODO get texture
            //Find spot in atlas
            //Set UV coords of block
//            blockDatabase.blocks[i].texturePosition[(int)BlockInstance.Direction.Down] = new BlockData.TexturePosition(x,y);
            //Save atlas to texture and assign to material

            //ceil(sqrt(x)) is useful somehow?
        }
    }
}
