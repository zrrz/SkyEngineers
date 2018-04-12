using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

public class BlockLoader : MonoBehaviour {

	[SerializeField]
    BlockDatabaseScriptableObject blockDatabase;

	Dictionary<int, BlockData> blocks;

	static BlockLoader instance;

	void Awake() {
		if(instance != null) {
			Debug.LogError("Already a BlockLoader in scene. Disabling", this);
			this.enabled = false;
			return;
		}
		instance = this;

		InitializeBlocks();
	}

	void InitializeBlocks() {
		blocks = new Dictionary<int, BlockData>();
        for(int i = 0; i < blockDatabase.blocks.Count; i++) {
            if(blocks.ContainsKey(blockDatabase.blocks[i].ID)) {
				Debug.LogError("DUPLICATE BLOCK ID. SKIPPING BLOCK");
			} else {
                blocks.Add(blockDatabase.blocks[i].ID, blockDatabase.blocks[i]);
			}
		}
	}

    //[MethodImpl(256)]
	public static BlockData GetBlock(int ID) {
        //return instance.blocks[ID];
		BlockData block;
		if(instance.blocks.TryGetValue(ID, out block)) {
			return block;
		} else {
			Debug.LogError("Block ID: " + ID + " not found.");
			return null;
		}
	}
}
