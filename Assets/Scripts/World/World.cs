using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

	Chunk[,] chunks;

	const int WORLD_WIDTH = 2;
	const int WORLD_HEIGHT = 2;

	//TEMP
	public GameObject fogPrefab;

	public Vector3 spawnPoint;

	void Start () {
		chunks = new Chunk[WORLD_WIDTH,WORLD_HEIGHT];
		for(int i = 0; i < WORLD_WIDTH; i++) {
			for(int j = 0; j < WORLD_HEIGHT; j++) {
				GameObject chunk = new GameObject("Chunk[" + i + " " + j + "]");
				chunk.transform.parent = transform;
				chunk.transform.position = new Vector3(i * Chunk.CHUNK_WIDTH - (WORLD_WIDTH*Chunk.CHUNK_WIDTH/2), 0f, j * Chunk.CHUNK_HEIGHT - (WORLD_HEIGHT*Chunk.CHUNK_HEIGHT/2));
				chunk.AddComponent<Chunk>();
				chunk.GetComponent<Chunk>().fogPrefab = fogPrefab;
			}
		}
	}
	
	void Update () {
		
	}
}
