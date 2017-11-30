using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using Assets.Scripts.Utilities;
using System;
using System.Threading;

public class World : MonoBehaviour {

    public List<Anchor> anchors = new List<Anchor>();

    public Dictionary<int, Chunk> chunks;// = new Dictionary<int, Chunk>();

    public string worldName = "world";

    public static World instance;

	public GameObject fogPrefab;

	public Vector3 spawnPoint;

//    public const int WORLD_WIDTH_IN_CHUNKS = 8;

    [NonSerialized]
    public int CHUNK_LOAD_DISTANCE = 16; //-half to +half

    void Awake() {
        if (instance != null)
        {
            Debug.LogError("Instance of World already exists in scene. Disabling", this);
            this.enabled = false;
            return;
        }
        instance = this;
    }

	void Start () {
        chunks = new Dictionary<int, Chunk>();
//        GenerateWorld();
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
        for (int y = -4; y < 4; y++) {
            for (int x = -CHUNK_LOAD_DISTANCE/2; x < CHUNK_LOAD_DISTANCE/2; x++) {
                for (int z = -CHUNK_LOAD_DISTANCE/2; z < CHUNK_LOAD_DISTANCE/2; z++) {
                    GenerateChunk(x * Chunk.CHUNK_SIZE, y * Chunk.CHUNK_SIZE, z * Chunk.CHUNK_SIZE);
                }
            }
        }
    }

    void Update() {
        UpdateWorld();

        for(int i = 0; i < 6 && chunksToLoad.Count > 0; i++)
        {
            Dictionary<int, Vector3Int>.Enumerator enumerator = chunksToLoad.GetEnumerator();
            enumerator.MoveNext();
            Vector3Int pos = enumerator.Current.Value;
            chunksToLoad.Remove(enumerator.Current.Key);
            ChunkLoad(pos);
        }
    }

    void UpdateWorld() {
        //TODO switch to go through all chunks and update, load, or unload if near Anchor
        foreach (Anchor anchor in anchors)
        {
            WorldPos pos = new WorldPos(
                Mathf.FloorToInt(anchor.position.x / Chunk.CHUNK_SIZE) * Chunk.CHUNK_SIZE,
                Mathf.FloorToInt(anchor.position.y / Chunk.CHUNK_SIZE) * Chunk.CHUNK_SIZE,
                Mathf.FloorToInt(anchor.position.z / Chunk.CHUNK_SIZE) * Chunk.CHUNK_SIZE
            );
            for (int x = -CHUNK_LOAD_DISTANCE/2; x < CHUNK_LOAD_DISTANCE/2; x++) {
                for (int y = -CHUNK_LOAD_DISTANCE/2; y < CHUNK_LOAD_DISTANCE/2; y++) {
                    for (int z = -CHUNK_LOAD_DISTANCE/2; z < CHUNK_LOAD_DISTANCE/2; z++) {
                        //TODO dont update a chunk twice in a frame. Just send.
                        int xPos = pos.x + (x*Chunk.CHUNK_SIZE);
                        int yPos = pos.y + (y*Chunk.CHUNK_SIZE);
                        int zPos = pos.z + (z*Chunk.CHUNK_SIZE);
                        Chunk chunk = GetChunk(xPos, yPos, zPos);
                        if (chunk != null)
                        {
                            chunk.Update(); 
                        }
                        else
                        {
                            GenerateChunk(xPos, yPos, zPos);
                        }
                    }
                }
            }
        }
    }

    Dictionary<int, Vector3Int> chunksToLoad = new Dictionary<int, Vector3Int>();

    /// <summary>
    /// Load from disk or create chunk.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="z">The z coordinate.</param>
    void GenerateChunk(int x, int y, int z) {
//        Thread thread = new Thread(() => ChunkLoad(new Vector3Int(x, y, z)));// new Thread(new ThreadStart(delegate { ChunkLoad(new Vector3Int(x, y, z));}));
//        thread.Start();
        Vector3Int pos = new Vector3Int(x, y, z);
        if(!chunksToLoad.ContainsKey(pos.GetHashCode()))
            chunksToLoad.Add(pos.GetHashCode(), pos);
//        ChunkLoad(new Vector3Int(x, y, z));
    }

    void ChunkLoad(Vector3Int pos) {
        WorldPos worldPos = new WorldPos(pos.x, pos.y, pos.z);
        Chunk newChunk = new Chunk();
//        newChunkObject.layer = LayerMask.NameToLayer("Blocks");
        newChunk.pos = worldPos;
        newChunk.world = this;

        //Add it to the chunks dictionary with the position as the key
        lock (chunks)
        {
            chunks.Add(worldPos.GetHashCode(), newChunk);
        }

        if (Serialization.LoadChunk(newChunk))
        {
            //Load instead of gen
        }
        else
        {
            var terrainGen = new TerrainGenerator();
            newChunk = terrainGen.ChunkGen(newChunk);
        }
        //        newChunk.SetBlocksUnmodified();
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
