using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldWorld : MonoBehaviour {

	OldChunk[,] chunks;

	const int WORLD_WIDTH = 2;
	const int WORLD_HEIGHT = 2;

	//TEMP
	public GameObject fogPrefab;

	public Vector3 spawnPoint;

	void Start () {
        chunks = new OldChunk[WORLD_WIDTH,WORLD_HEIGHT];
		for(int i = 0; i < WORLD_WIDTH; i++) {
			for(int j = 0; j < WORLD_HEIGHT; j++) {
				GameObject chunk = new GameObject("Chunk[" + i + " " + j + "]");
				chunk.transform.parent = transform;
                chunk.transform.position = new Vector3(i * OldChunk.CHUNK_WIDTH - (WORLD_WIDTH*OldChunk.CHUNK_WIDTH/2), 0f, j * OldChunk.CHUNK_HEIGHT - (WORLD_HEIGHT*OldChunk.CHUNK_HEIGHT/2));
                chunk.AddComponent<OldChunk>();
                chunk.GetComponent<OldChunk>().fogPrefab = fogPrefab;
			}
		}
	}
	
	void Update () {
		
	}
}
