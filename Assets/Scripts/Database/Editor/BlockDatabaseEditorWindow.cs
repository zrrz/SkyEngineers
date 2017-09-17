using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

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

	Vector2 scrollPosition;
    void OnGUI()
    {
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        for (int i = 0; i < blockDatabase.blocks.Count; i++)
        {
			blockDatabase.blocks[i].name = GUILayout.TextField(blockDatabase.blocks[i].name);
			blockDatabase.blocks[i].textures[0] = (Texture2D) EditorGUILayout.ObjectField("Top Texture", blockDatabase.blocks[i].textures[0], typeof (Texture2D), false);
			blockDatabase.blocks[i].textures[1] = (Texture2D) EditorGUILayout.ObjectField("Bottom Texture", blockDatabase.blocks[i].textures[1], typeof (Texture2D), false);
			blockDatabase.blocks[i].textures[2] = (Texture2D) EditorGUILayout.ObjectField("West Texture", blockDatabase.blocks[i].textures[2], typeof (Texture2D), false);
			blockDatabase.blocks[i].textures[3] = (Texture2D) EditorGUILayout.ObjectField("East Texture", blockDatabase.blocks[i].textures[3], typeof (Texture2D), false);
			blockDatabase.blocks[i].textures[4] = (Texture2D) EditorGUILayout.ObjectField("Front Texture", blockDatabase.blocks[i].textures[4], typeof (Texture2D), false);
			blockDatabase.blocks[i].textures[5] = (Texture2D) EditorGUILayout.ObjectField("Back Texture", blockDatabase.blocks[i].textures[5], typeof (Texture2D), false);
			if (GUILayout.Button("Delete"))
			{
				blockDatabase.blocks.RemoveAt(i);
				return;
			}
        }
		GUILayout.EndScrollView();

		if (GUILayout.Button("Add new"))
		{
			BlockData block = new BlockData();
			block.textures = new Texture2D[6];
			block.texturePosition = new BlockData.TexturePosition[6];
			block.solid = new bool[6];
			block.ID = blockDatabase.blocks.Count;
			blockDatabase.blocks.Add(block);
		}


        //TODO indicate uncompiled through graphic or color
        if (GUILayout.Button("Compile Database"))
        {
            BuildAtlas();
            AssetDatabase.SaveAssets();
        }
    }

    void BuildAtlas() {
		Texture2D texture = new Texture2D(2048, 2048, TextureFormat.RGBA32, true);

		int x = 0;
		int y = 0;
        for (int i = 0; i < blockDatabase.blocks.Count; i++)
        {
			BlockData block = blockDatabase.blocks[i];
			for(int j = 0; j < 6; j++) {
				AddTexture(texture, block, (BlockInstance.Direction)j, ref x, ref y);
			}
			//TODO skip null for air
			//TODO skip sides using same texture using a dictionary probably

        }
		string filePath = "Assets/Sandbox/Zack/TempShit/Atlas.png";
		byte[] bytes = texture.EncodeToPNG();
		FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		BinaryWriter writer = new BinaryWriter(stream);
		writer.Write(bytes);
		writer.Close();
		stream.Close();
		DestroyImmediate(texture);
		AssetDatabase.Refresh();
		
		AssetDatabase.LoadAssetAtPath<Material>("Assets/Scripts/NewVoxels/Materials/tiles 2.mat").mainTexture = AssetDatabase.LoadAssetAtPath<Texture>(filePath);
    }

	void AddTexture(Texture2D atlas, BlockData block, BlockInstance.Direction direction, ref int x, ref int y) {
		atlas.SetPixels(x*32, y*32, 32, 32, block.textures[(int)direction].GetPixels());
		block.texturePosition[(int)direction] = new BlockData.TexturePosition(x,y);
		x++;
		if(x >= 64) {
			y++;
			x = 0;
		}
	}
}
