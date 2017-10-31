using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using Assets.Scripts.Utilities;
using System;

public class World : MonoBehaviour {

    public Dictionary<int, Chunk> chunks;// = new Dictionary<int, Chunk>();

    public string worldName = "world";

	public static GameManager instance;

	public GameObject fogPrefab;

	public Vector3 spawnPoint;

    public const int WORLD_WIDTH_IN_CHUNKS = 8;

	void Start () {
        GenerateWorld();
//        Debug.LogError(System.mar( BlockInstance));


//        yield return new WaitForSeconds(2f);
//        RaycastHit hit;
//        Debug.DrawRay(new Vector3(1f, 300f, 0f), Vector3.down, Color.red, 5f);
//        if(Physics.Raycast(new Vector3(1f, 3000f, 1f), Vector3.down, out hit, 3000f, LayerMask.GetMask("Blocks"))) {
//            spawnPoint = new Vector3Int((int)hit.point.x, (int)hit.point.y, (int)hit.point.z);
//        } else {
//            Debug.LogError("Can't find spawnpoint");
//        }
//		Serialization.LoadPlayer(FindObjectOfType<PlayerData>());
	}

//    public override void DownloadWorld(RpcArgs args)
//    {
//        if (networkObject.IsServer)
//        {
//            Debug.LogError("I don't think server should be calling this");
//        }
//
//        System.Byte[] bytes = args.GetNext<System.Byte[]>();
//        chunks = (Dictionary<int, Chunk>)ByteArrayUtils.ByteArrayToObject(bytes);
//    }

    public void GenerateWorld() {
        chunks = new Dictionary<int, Chunk>();
        for (int y = -4; y < 4; y++) {
            for (int x = -WORLD_WIDTH_IN_CHUNKS; x < WORLD_WIDTH_IN_CHUNKS; x++) {
                for (int z = -WORLD_WIDTH_IN_CHUNKS; z < WORLD_WIDTH_IN_CHUNKS; z++) {
                    WorldPos worldPos = new WorldPos(x*Chunk.CHUNK_SIZE, y*Chunk.CHUNK_SIZE, z*Chunk.CHUNK_SIZE);

                    Chunk newChunk = new Chunk();
//                    newChunkObject.layer = LayerMask.NameToLayer("Blocks");
                    newChunk.pos = worldPos;
                    newChunk.world = this;

                    //Add it to the chunks dictionary with the position as the key
                    chunks.Add(worldPos.GetHashCode(), newChunk);

                    if (Serialization.LoadChunk(newChunk))
                    {
                        //Load instead of gen
                    }
                    else
                    {
                        var terrainGen = new TerrainGenerator();
                        newChunk = terrainGen.ChunkGen(newChunk);
                    }
//                    newChunk.SetBlocksUnmodified();
                }
            }
        }

    }

//    public void CreateChunk(int x, int y, int z)
//    {
//        WorldPos worldPos = new WorldPos(x, y, z);
//
//        //Instantiate the chunk at the coordinates using the chunk prefab
//        GameObject newChunkObject = Instantiate(
//                        chunkPrefab, new Vector3(x, y, z),
//                        Quaternion.Euler(Vector3.zero)
//                    ) as GameObject;
//
//        Chunk newChunk = newChunkObject.GetComponent<Chunk>();
//		newChunkObject.layer = LayerMask.NameToLayer("Blocks");
//        newChunk.pos = worldPos;
//        newChunk.world = this;
//
//        //Add it to the chunks dictionary with the position as the key
//        chunks.Add(worldPos.GetHashCode(), newChunk);
//
//        var terrainGen = new TerrainGenerator();
//        newChunk = terrainGen.ChunkGen(newChunk);
//
//        newChunk.SetBlocksUnmodified();
//
//        Serialization.LoadChunk(newChunk);
//    }

    void OnApplicationQuit() {
        SaveWorld();
    }

    public void SaveWorld() {
//		Serialization.SavePlayer(FindObjectOfType<PlayerData>());
        foreach (Chunk chunk in chunks.Values)
        {
            Serialization.SaveChunk(chunk);
        }
    }

    //Don't destroy chunks anymore
//    public void DestroyChunk(WorldPos pos)
//    {
//        Chunk chunk = null;
//        int hash = pos.GetHashCode();
//        if (chunks.TryGetValue(hash, out chunk))
//        {
//            Serialization.SaveChunk(chunk);
//            Object.Destroy(chunk.gameObject);
//            chunks.Remove(hash);
//        }
//    }

    public Chunk GetChunk(int x, int y, int z)
    {
        int hash = WorldPos.GenerateHashCode(
            Mathf.FloorToInt(x / (float)Chunk.CHUNK_SIZE) * Chunk.CHUNK_SIZE,
            Mathf.FloorToInt(y / (float)Chunk.CHUNK_SIZE) * Chunk.CHUNK_SIZE,
            Mathf.FloorToInt(z / (float)Chunk.CHUNK_SIZE) * Chunk.CHUNK_SIZE
               );

        Chunk containerChunk = null;

        chunks.TryGetValue(hash, out containerChunk);

        return containerChunk;
    }

    public BlockInstance GetBlock(int x, int y, int z)
    {
        Chunk containerChunk = GetChunk(x, y, z);

        if (containerChunk != null)
        {
            BlockInstance block = containerChunk.GetBlock(
                x - containerChunk.pos.x,
                y - containerChunk.pos.y,
                z - containerChunk.pos.z);

            return block;
        }
        else
        {
//            Debug.LogError("Block isn't in containerChunk", this);
            return new BlockInstance();
//            return BlockDatabase.GetBlock(0);
        }

    }

    public void SetBlock(int x, int y, int z, BlockData block)
    {
        Chunk chunk = GetChunk(x, y, z);

        if (chunk != null)
        {
            chunk.SetBlock(x - chunk.pos.x, y - chunk.pos.y, z - chunk.pos.z, block);
            chunk.update = true;

            UpdateIfEqual(x - chunk.pos.x, 0, new WorldPos(x - 1, y, z));
            UpdateIfEqual(x - chunk.pos.x, Chunk.CHUNK_SIZE - 1, new WorldPos(x + 1, y, z));
            UpdateIfEqual(y - chunk.pos.y, 0, new WorldPos(x, y - 1, z));
            UpdateIfEqual(y - chunk.pos.y, Chunk.CHUNK_SIZE - 1, new WorldPos(x, y + 1, z));
            UpdateIfEqual(z - chunk.pos.z, 0, new WorldPos(x, y, z - 1));
            UpdateIfEqual(z - chunk.pos.z, Chunk.CHUNK_SIZE - 1, new WorldPos(x, y, z + 1));
        
        }
    }

    void UpdateIfEqual(int value1, int value2, WorldPos pos)
    {
        if (value1 == value2)
        {
            Chunk chunk = GetChunk(pos.x, pos.y, pos.z);
            if (chunk != null)
                chunk.update = true;
        }
    }
}
