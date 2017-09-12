using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {

	public const int CHUNK_WIDTH = 16;
	public const int CHUNK_HEIGHT = 16;

	Block[,,] blocks;

	public GameObject fogPrefab;

	void Start () {
		blocks = new Block[CHUNK_WIDTH, CHUNK_HEIGHT, CHUNK_WIDTH];

		//TODO spawn blockers all around edges for test
		for(int x = 0; x < CHUNK_WIDTH; x++) {
			for(int z = 0; z < CHUNK_WIDTH; z++) {
				PlaceBlock(BlockDatabase.GetBlock(0), x, 0, z);
			}
		}

		for(int x = 0; x < CHUNK_WIDTH; x++) {
			for(int z = 0; z < CHUNK_WIDTH; z++) {
				PlaceBlock(BlockDatabase.GetBlock(0), x, CHUNK_HEIGHT -1, z);
			}
		}

		for(int x = 0; x < CHUNK_WIDTH; x++) {
			for(int y = 0; y < CHUNK_HEIGHT; y++) {
				PlaceBlock(BlockDatabase.GetBlock(0), x, y, 0);
			}
		}

		for(int x = 0; x < CHUNK_WIDTH; x++) {
			for(int y = 0; y < CHUNK_HEIGHT; y++) {
				PlaceBlock(BlockDatabase.GetBlock(0), x, y, CHUNK_WIDTH-1);
			}
		}

		for(int z = 0; z < CHUNK_WIDTH; z++) {
			for(int y = 0; y < CHUNK_HEIGHT; y++) {
				PlaceBlock(BlockDatabase.GetBlock(0), 0, y, z);
			}
		}

		for(int z = 0; z < CHUNK_WIDTH; z++) {
			for(int y = 0; y < CHUNK_HEIGHT; y++) {
				PlaceBlock(BlockDatabase.GetBlock(0), CHUNK_WIDTH-1, y, z);
			}
		}

		//TODO spawn a tree at the player spawn point
		//Shitty tree
		PlaceBlock(BlockDatabase.GetBlock(1),CHUNK_WIDTH/2, 1, CHUNK_WIDTH/2);
		PlaceBlock(BlockDatabase.GetBlock(1),CHUNK_WIDTH/2, 2, CHUNK_WIDTH/2);
		PlaceBlock(BlockDatabase.GetBlock(1),CHUNK_WIDTH/2, 3, CHUNK_WIDTH/2);
		for(int x = CHUNK_WIDTH/2 - 1; x < CHUNK_WIDTH/2 + 2; x++) {
			for(int y = 4; y < 6; y++) {
				for(int z = CHUNK_WIDTH/2 - 1; z < CHUNK_WIDTH/2 + 2; z++) {
					PlaceBlock(BlockDatabase.GetBlock(2), x, y, z);
				}
			}
		}

		//TODO Add player spawn point

//		StartCoroutine(UpdateFoo());

		Instantiate(fogPrefab, transform.InverseTransformPoint(new Vector3(0f, -16f, 0f)), Quaternion.Euler(-90f, 0f, 0f));
	}

	//TODO call from World.cs depending on anchors
//	IEnumerator UpdateFoo () {
	void Update() {
////		while(true) {
////			yield return new WaitForSeconds(0.25f);
			UpdateChunk();
////		}
	}

	//TODO rotate based on player view
	void PlaceBlock(Block block, int x, int y, int z) {
//		if(block.GetComponent<Block>() != null) {
		if(blocks[x,y,z] == null) {
			blocks[x,y,z] = ((GameObject)Instantiate(block.gameObject, transform.InverseTransformPoint(new Vector3(x, y, z)), Quaternion.identity, transform)).GetComponent<Block>();
		} else {
//			Debug.LogError("Already a [" + blocks[x,y,z].name + "] block at: (" + x + "," + y + "," + z + ")!");
		}
//		} else {
//			Debug.LogError("Must be a block stupid");
//		}
	}


	/// <summary>
	/// Gets the neighbors in this order: +x, -x, +y, -y, +z, -z
	/// </summary>
	/// <returns>The neighbors.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	Block[] GetNeighbors(int x, int y, int z) {
		Block[] neighbors = new Block[6];
		if(x < CHUNK_WIDTH-1) neighbors[0] = blocks[x+1,y,z];
		if(x > 0) neighbors[1] = blocks[x-1,y,z];
		if(y < CHUNK_HEIGHT) neighbors[2] = blocks[x,y+1,z];
		if(y > 0) neighbors[3] = blocks[x,y-1,z];
		if(z < CHUNK_WIDTH-1) neighbors[4] = blocks[x,y,z+1];
		if(z > 0) neighbors[5] = blocks[x,y,z-1];
		return neighbors;
	}

	//TODO call this sometime
	void UpdateChunk() {
		for(int x = 0; x < CHUNK_WIDTH; x++) {
			for(int y = 0; y < CHUNK_HEIGHT; y++) {
				for(int z = 0; z < CHUNK_WIDTH; z++) {
					//TODO Update blocks
					if(blocks[x,y,z]) {
						blocks[x,y,z].BlockUpdate();
					}
				}
			}
		}
	}
}
