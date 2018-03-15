using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using Assets.Scripts.Utilities;
using System;
using System.Threading;
using UnityEngine.Jobs;
using System.Linq;

public class World : MonoBehaviour
{

    public List<Anchor> anchors = new List<Anchor>();

    /// <summary>
    /// The loaded chunk instances.
    /// </summary>
    public Dictionary<int, ChunkInstance> loadedChunks;

    public string worldName = "world"; //Maybe move to some IO file handling place?

    public static World instance;

    //public GameObject fogPrefab;

    public Vector3 spawnPoint;

    private Thread _unloadThread;
    private Thread _loadThread;
    private Thread _updateThread;

    /// <summary>
    /// If game is exiting and needs to save
    /// </summary>
    private bool _unloaded;

    /// <summary>
    /// How long a chunk persists when not near any anchors
    /// </summary>
    public static readonly TimeSpan ChunkLifetime = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Thread adds cachedChunks. Main thread iterates and creates chunk instances.
    /// </summary>
    private readonly Dictionary<int, CachedChunk> _chunksReadyToAdd = new Dictionary<int, CachedChunk>();

    /// <summary>
    //Not threaded. Threads add to set. Main thread iterates and deletes.
    /// </summary>
    private readonly HashSet<int> _chunksReadyToRemove = new HashSet<int>();

    /// <summary>
    //To instantiate the graphics object
    /// </summary>
    //private readonly Queue<ChunkInstance> _chunksReadyToCreateGraphics = new Queue<ChunkInstance>();

    /// <summary>
    ///Chunks that are populated by anchors. Might be unnecessary?
    /// </summary>
    private readonly HashSet<int> _populatedChunks = new HashSet<int>();

    //Threaded
    private readonly Queue<ChunkInstance> _queuedChunkUpdates = new Queue<ChunkInstance>();

    [NonSerialized]
    public int CHUNK_LOAD_DISTANCE = 16; //-half to +half

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Instance of World already exists in scene. Disabling", this);
            this.enabled = false;
            return;
        }
        instance = this;
    }

    void Start()
    {
        loadedChunks = new Dictionary<int, ChunkInstance>();

        _unloadThread = new Thread(UnloadThread) { Name = "Unload Thread" };
        _loadThread = new Thread(LoadThread) { Name = "Load Thread" };
        _updateThread = new Thread(UpdateThread) { Name = "Update Thread" };

        _unloadThread.Start();
        _loadThread.Start();
        _updateThread.Start();
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

    public void GenerateWorld()
    {
        for (int y = -4; y < 4; y++)
        {
            for (int x = -CHUNK_LOAD_DISTANCE / 2; x < CHUNK_LOAD_DISTANCE / 2; x++)
            {
                for (int z = -CHUNK_LOAD_DISTANCE / 2; z < CHUNK_LOAD_DISTANCE / 2; z++)
                {
                    GenerateChunk(x * ChunkInstance.CHUNK_SIZE, y * ChunkInstance.CHUNK_SIZE, z * ChunkInstance.CHUNK_SIZE);
                }
            }
        }
    }

    //void Update() {
    //    UpdateWorld();

    //    for(int i = 0; i < 6 && chunksToLoad.Count > 0; i++)
    //    {
    //        Dictionary<int, Vector3Int>.Enumerator enumerator = chunksToLoad.GetEnumerator();
    //        enumerator.MoveNext();
    //        Vector3Int pos = enumerator.Current.Value;
    //        chunksToLoad.Remove(enumerator.Current.Key);
    //        ChunkLoad(pos);
    //    }
    //}

    //void UpdateWorld() {
    //    //TODO switch to go through all chunks and update, load, or unload if near Anchor
    //    foreach (Anchor anchor in anchors)
    //    {
    //        WorldPos pos = new WorldPos(
    //            Mathf.FloorToInt(anchor.position.x / Chunk.CHUNK_SIZE) * Chunk.CHUNK_SIZE,
    //            Mathf.FloorToInt(anchor.position.y / Chunk.CHUNK_SIZE) * Chunk.CHUNK_SIZE,
    //            Mathf.FloorToInt(anchor.position.z / Chunk.CHUNK_SIZE) * Chunk.CHUNK_SIZE
    //        );
    //        for (int x = -CHUNK_LOAD_DISTANCE/2; x < CHUNK_LOAD_DISTANCE/2; x++) {
    //            for (int y = -CHUNK_LOAD_DISTANCE/2; y < CHUNK_LOAD_DISTANCE/2; y++) {
    //                for (int z = -CHUNK_LOAD_DISTANCE/2; z < CHUNK_LOAD_DISTANCE/2; z++) {
    //                    //TODO dont update a chunk twice in a frame. Just send.
    //                    int xPos = pos.x + (x*Chunk.CHUNK_SIZE);
    //                    int yPos = pos.y + (y*Chunk.CHUNK_SIZE);
    //                    int zPos = pos.z + (z*Chunk.CHUNK_SIZE);
    //                    Chunk chunk = GetChunk(xPos, yPos, zPos);
    //                    if (chunk != null)
    //                    {
    //                        chunk.Update();
    //                    }
    //                    else
    //                    {
    //                        GenerateChunk(xPos, yPos, zPos);
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    public void Update()
    {
        if (_unloaded) return;

        //Update entities
        //foreach (var playerEntity in PlayerEntities)
        //{
        //    playerEntity.Update();
        //}
        //foreach (var entity in Entities)
        //{
        //    entity.Update();
        //}

        lock (_chunksReadyToRemove)
        {
            while (_chunksReadyToRemove.Count > 0)
            {
                var chunkPos = _chunksReadyToRemove.First();

                ChunkInstance chunk;
                if (loadedChunks.TryGetValue(chunkPos, out chunk))
                {
                    lock (loadedChunks)
                    {
                        loadedChunks.Remove(chunkPos);
                    }
                    _populatedChunks.Remove(chunkPos);
                    //chunk.Dispose();
                }

                _chunksReadyToRemove.Remove(chunkPos);
            }
        }

        lock (_chunksReadyToAdd)
        {
            while (_chunksReadyToAdd.Count > 0)
            {
                var entry = _chunksReadyToAdd.First();
                lock (loadedChunks)
                {
                    if (!loadedChunks.ContainsKey(entry.Key))
                        loadedChunks.Add(entry.Key, new ChunkInstance(entry.Value));
                    else Debug.Log("Chunk has already been loaded! " + entry.Key);
                }
                _populatedChunks.Add(entry.Key);
                _chunksReadyToAdd.Remove(entry.Key);
            }
        }

        //while (_chunksReadyToCreateGraphics.Count > 0)
            //lock (_chunksReadyToCreateGraphics)
            //{
            //    //_chunksReadyToUpload.Dequeue().Upload();
            //}

    }

    public void Unload()
    {
        _unloaded = true;

        Debug.Log("Waiting for threads to finish...");
        while (_loadThread.IsAlive || _unloadThread.IsAlive || _updateThread.IsAlive)
            Thread.Sleep(100);

        Debug.Log("Saving world...");
        foreach (var entry in loadedChunks)
            Serialization.SaveChunk(entry.Value);
        Debug.Log("World saved");
    }


    private void UpdateThread()
    {
        //WorldPos blockPos;
        ChunkInstance chunk;

        while (!_unloaded)
        {
            //while (_queuedLightUpdates.Count > 0)
            //{
            //    lock (_queuedLightUpdates)
            //    {
            //        blockPos = _queuedLightUpdates.Dequeue();
            //    }

            //    UpdateLightValues(blockPos);
            //}

            while (_queuedChunkUpdates.Count > 0)
            {
                lock (_queuedChunkUpdates)
                {
                    chunk = _queuedChunkUpdates.Dequeue();
                }
                chunk.Update();
                //lock (_chunksReadyToCreateGraphics)
                //{
                //    if (!_chunksReadyToCreateGraphics.Contains(chunk))
                //        _chunksReadyToCreateGraphics.Enqueue(chunk);
                //}
            }

            //if (_queuedChunkUpdatesLp.Count <= 0) continue;
            //lock (_queuedChunkUpdatesLp)
            //{
            //    chunk = _queuedChunkUpdatesLp.Dequeue();
            //}

            //chunk.Update();

            //lock (_chunksReadyToUploadLp)
            //{
            //    if (!_chunksReadyToUploadLp.Contains(chunk)) _chunksReadyToUploadLp.Enqueue(chunk);
            //}

            Thread.Sleep(1);
        }
    }

    private void UnloadThread()
    {
        while (!_unloaded)
        {
            List<ChunkInstance> chunksToUnload;

            lock (loadedChunks)
            {
                //TODO fix closure
                chunksToUnload =
                    loadedChunks.Where(
                        pair =>
                            DateTime.Now - pair.Value.Time > ChunkLifetime &&
                        !_chunksReadyToRemove.Contains(pair.Key.GetHashCode())).Select(pair => pair.Value).ToList();
            }

            foreach (var chunk in chunksToUnload)
            {
                Serialization.SaveChunk(chunk);
                lock (_chunksReadyToRemove)
                {
                    _chunksReadyToRemove.Add(chunk.position.GetHashCode());
                }
            }

            Thread.Sleep(1000);
        }
    }

    private void LoadThread()
    {
        var chunksWaitingForNeighbours = new HashSet<WorldPos>();

        while (!_unloaded)
        {
            //Remove chunks waiting that have been unloaded
            //TODO fix closure
            var chunksToRemove =
                chunksWaitingForNeighbours.Where(
                    chunkPos =>
                    !_populatedChunks.Contains(chunkPos.GetHashCode()) && !_chunksReadyToAdd.ContainsKey(chunkPos.GetHashCode()) ||
                    _populatedChunks.Contains(chunkPos.GetHashCode()) && !loadedChunks.ContainsKey(chunkPos.GetHashCode())).ToList();
            chunksToRemove.ForEach(chunkPos => chunksWaitingForNeighbours.Remove(chunkPos));

            //Update chunks waiting for neighbours
            //TODO fix closure
            var chunksToUpdate =
                chunksWaitingForNeighbours.Where(
                        chunkPos =>
                    BlockFaceHelper.Faces.All(face => _populatedChunks.Contains((chunkPos + face.GetNormali()).GetHashCode()))
                ).ToList();
            foreach (var chunkPos in chunksToUpdate)
            {
                QueueChunkUpdate(chunkPos/*, true*/);
                chunksWaitingForNeighbours.Remove(chunkPos);
            }

            var playerChunksLists = new List<List<WorldPos>>();
            foreach (var anchor in anchors)
            {
                var anchorChunksToLoad = new List<WorldPos>();
                var anchorChunk = ChunkInWorld(new WorldPos(anchor.position));


            //for (int x = -CHUNK_LOAD_DISTANCE/2; x < CHUNK_LOAD_DISTANCE/2; x++) {
            //    for (int y = -CHUNK_LOAD_DISTANCE/2; y < CHUNK_LOAD_DISTANCE/2; y++) {
            //        for (int z = -CHUNK_LOAD_DISTANCE/2; z < CHUNK_LOAD_DISTANCE/2; z++) {
            //            //TODO dont update a chunk twice in a frame. Just send.
            //            int xPos = pos.x + (x*Chunk.CHUNK_SIZE);
            //            int yPos = pos.y + (y*Chunk.CHUNK_SIZE);
            //            int zPos = pos.z + (z*Chunk.CHUNK_SIZE);
            //            Chunk chunk = GetChunk(xPos, yPos, zPos);
            //            if (chunk != null)
            //            {
            //                chunk.Update();
            //            }
            //            else
            //            {
            //                GenerateChunk(xPos, yPos, zPos);
            //            }
            //        }
            //    }
            //}

                //Load 7x7x7 around player
                for (var x = -3; x <= 3; x++)
                    for (var y = -3; y <= 3; y++)
                        for (var z = -3; z <= 3; z++)
                        {
                            var chunkPos = anchorChunk + (new WorldPos(x, y, z) * ChunkInstance.CHUNK_SIZE);
                            if (anchorChunksToLoad.Contains(chunkPos))
                                continue;
                            if (_populatedChunks.Contains(chunkPos.GetHashCode()) || _chunksReadyToAdd.ContainsKey(chunkPos.GetHashCode()))
                            {
                                //Reset chunk time so it will not be unloaded
                                ChunkInstance chunk;
                                if (loadedChunks.TryGetValue(chunkPos.GetHashCode(), out chunk))
                                    chunk.Time = DateTime.Now;
                                continue;
                            }

                            anchorChunksToLoad.Add(chunkPos);
                        }


                //Load 61x3x61 terrain if overworld
                //heightmap(x*Chunk.Size + Chunk.Size/2, z*Chunk.Size + Chunk.Size/2)

                var height = 0 + ChunkInstance.CHUNK_SIZE / 2;
                var heightMapChunkY = ChunkInWorld(0, height, 0).y;
                for (var x = -30; x <= 30; x++)
                    for (var y = -1; y <= 1; y++)
                        for (var z = -30; z <= 30; z++)
                        {
                            if (x <= 3 && x >= -3 && y <= 3 && y >= -3 && z <= 3 && z >= -3) continue;

                            var chunkPos = new WorldPos(anchorChunk.x + x, heightMapChunkY + y, anchorChunk.z + z);

                            if (_populatedChunks.Contains(chunkPos.GetHashCode()) || _chunksReadyToAdd.ContainsKey(chunkPos.GetHashCode()))
                            {
                                //Reset chunk time so it will not be unloaded
                                ChunkInstance chunk;
                                if (loadedChunks.TryGetValue(chunkPos.GetHashCode(), out chunk))
                                    chunk.Time = DateTime.Now;
                                continue;
                            }

                            anchorChunksToLoad.Add(chunkPos);
                        }


                //TODO fix closure
                anchorChunksToLoad.Sort(
                    (v0, v1) =>
                    (int)(v0.ToVector3() - anchorChunk.ToVector3()).sqrMagnitude -
                    (int)(v1.ToVector3() - anchorChunk.ToVector3()).sqrMagnitude);

                //Cap player chunk load tasks to 16
                if (anchorChunksToLoad.Count > 16)
                    anchorChunksToLoad.RemoveRange(16, anchorChunksToLoad.Count - 16);

                playerChunksLists.Add(anchorChunksToLoad);
            }

            var merged = ExtensionHelper.ZipMerge(playerChunksLists.ToArray());
            foreach (var chunkPos in merged)
            {
                var cachedChunk = LoadChunk(chunkPos);

                if (cachedChunk.IsEmpty)
                {
                    //Empty chunks dont need to be added to LoadedChunks
                    lock (_populatedChunks)
                    {
                        if (_populatedChunks.Contains(chunkPos.GetHashCode()))
                        {
                            Debug.LogError("_populatedChunks ALREADY CONTAINS KEY");
                        }
                        else
                        {
                            _populatedChunks.Add(chunkPos.GetHashCode());
                        }
                    }
                }
                else
                {
                    lock (_chunksReadyToAdd)
                    {
                        if (_chunksReadyToAdd.ContainsKey(chunkPos.GetHashCode()))
                        {
                            Debug.LogError("_chunksReadyToAdd ALREADY CONTAINS KEY");
                        }
                        else
                        {
                            _chunksReadyToAdd.Add(chunkPos.GetHashCode(), cachedChunk);
                        }
                    }
                    chunksWaitingForNeighbours.Add(chunkPos);
                }
            }

            Thread.Sleep(10);
        }
    }

    public void QueueChunkUpdate(WorldPos chunkPos/*, bool lowPrioriity*/)
    {
        ChunkInstance chunk;
        if (loadedChunks.TryGetValue(chunkPos.GetHashCode(), out chunk))
            QueueChunkUpdate(chunk/*, lowPrioriity*/);
    }

    public void QueueChunkUpdate(ChunkInstance chunk/*, bool lowPrioriity*/)
    {
        var queue = _queuedChunkUpdates;
        lock (queue)
        {
            if (!queue.Contains(chunk)) queue.Enqueue(chunk);
        }
    }


    Dictionary<int, Vector3Int> chunksToLoad = new Dictionary<int, Vector3Int>();

    /// <summary>
    /// Load from disk or create chunk.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="z">The z coordinate.</param>
    void GenerateChunk(int x, int y, int z)
    {
        //        Thread thread = new Thread(() => ChunkLoad(new Vector3Int(x, y, z)));// new Thread(new ThreadStart(delegate { ChunkLoad(new Vector3Int(x, y, z));}));
        //        thread.Start();
        Vector3Int pos = new Vector3Int(x, y, z);
        if (!chunksToLoad.ContainsKey(pos.GetHashCode()))
            chunksToLoad.Add(pos.GetHashCode(), pos);
        //        ChunkLoad(new Vector3Int(x, y, z));
    }

    void ChunkLoad(Vector3Int pos)
    {
        WorldPos worldPos = new WorldPos(pos.x, pos.y, pos.z);
		CachedChunk cachedChunk = Serialization.LoadChunk(this, worldPos);
        ChunkInstance newChunk = null;
        if(cachedChunk == null) {
            newChunk = new ChunkInstance(this, worldPos);
        } else {
            newChunk = new ChunkInstance(cachedChunk);
            //        newChunkObject.layer = LayerMask.NameToLayer("Blocks");
        }
		lock (loadedChunks)
		{
			loadedChunks.Add(worldPos.GetHashCode(), newChunk);
		}


        //if ()
        //{
        //    //Load instead of gen
        //}
        //else
        //{
        //    var terrainGen = new TerrainGenerator();
        //    newChunk = terrainGen.ChunkGen(newChunk);
        //}
        //        newChunk.SetBlocksUnmodified();
    }

    private CachedChunk LoadChunk(WorldPos position)
    {
        //return null;
        var chunk = Serialization.LoadChunk(this, position);
        if (chunk != null) 
            return chunk;

        chunk = new CachedChunk(this, position);
        var terrainGen = new TerrainGenerator();
        chunk = terrainGen.ChunkGen(chunk);

        return chunk;


        ////TODO: implement terrain gen
        //chunk = new CachedChunk(this, position);
        //var worldMin = position * ChunkInstance.CHUNK_SIZE;
        //var worldMax = worldMin + new WorldPos(ChunkInstance.CHUNK_SIZE - 1, ChunkInstance.CHUNK_SIZE - 1, ChunkInstance.CHUNK_SIZE - 1);

        //for (var x = 0; x < ChunkInstance.CHUNK_SIZE; x++)
        //    for (var z = 0; z < ChunkInstance.CHUNK_SIZE; z++)
        //    {

        //        var height = OpenSimplexNoise.Generate((worldMin.x + x) * 0.06f, (worldMin.z + z) * 0.06f) * 5;
        //        height += OpenSimplexNoise.Generate((worldMin.x + x) * 0.1f, (worldMin.z + z) * 0.1f) * 2;
        //        //height += OpenSimplexNoise.Generate((worldMin.X + x) * 0.005f, (worldMin.Z + z) * 0.005f) * 10;
        //        height = 0;


        //    for (var y = 0; y < ChunkInstance.CHUNK_SIZE; y++)
        //            if (worldMin.Y + y <= height)
        //                chunk.SetBlock(x, y, z, (worldMin.Y + y == (int)height) ? GameRegistry.GetBlock("Vanilla:Grass") : GameRegistry.GetBlock("Vanilla:Dirt"));

        //        /*
        //    for (var y = 0; y < Chunk.Size; y++)
        //    {
        //        var density = (OpenSimplexNoise.Generate((worldMin.X + x) * 0.045f, (worldMin.Y + y) * 0.075f, (worldMin.Z + z) * 0.045f) +1)*30;
        //        if (density > 13+15)
        //        {
        //            chunk.SetBlock(x,y,z, GameRegistry.GetBlock("Vanilla:Stone"));
        //                if(chunk.GetBlock(x,y-1,z).RegistryKey == "Vanilla:Grass")
        //                    chunk.SetBlock(x,y-1,z, GameRegistry.GetBlock("Vanilla:Dirt"));
        //        }
        //        else if (density > 10+15)
        //        {
        //            chunk.SetBlock(x, y, z,
        //                chunk.GetBlock(x, y + 1, z).IsOpaqueFullBlock(this, new Vector3i(x, y + 1, z))
        //                    ? GameRegistry.GetBlock("Vanilla:Dirt")
        //                    : GameRegistry.GetBlock("Vanilla:Grass"));
        //                if (chunk.GetBlock(x, y - 1, z).RegistryKey == "Vanilla:Grass")
        //                    chunk.SetBlock(x, y - 1, z, GameRegistry.GetBlock("Vanilla:Dirt"));
        //            }
        //    }
        //    */
        //    }


        //return chunk;
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

    void OnApplicationQuit()
    {
        SaveWorld();
        Unload();
    }

    public void SaveWorld()
    {
        //		Serialization.SavePlayer(FindObjectOfType<PlayerData>());
        foreach (ChunkInstance chunk in loadedChunks.Values)
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

    public ChunkInstance GetChunk(int x, int y, int z)
    {
        int hash = WorldPos.GenerateHashCode(
            Mathf.FloorToInt(x / (float)ChunkInstance.CHUNK_SIZE) * ChunkInstance.CHUNK_SIZE,
            Mathf.FloorToInt(y / (float)ChunkInstance.CHUNK_SIZE) * ChunkInstance.CHUNK_SIZE,
            Mathf.FloorToInt(z / (float)ChunkInstance.CHUNK_SIZE) * ChunkInstance.CHUNK_SIZE
               );

        ChunkInstance containerChunk = null;

        loadedChunks.TryGetValue(hash, out containerChunk);

        return containerChunk;
    }

    public ushort GetBlock(int x, int y, int z)
    {
        ChunkInstance containerChunk = GetChunk(x, y, z);

        if (containerChunk != null)
        {
            ushort blockID = containerChunk.GetBlock(
                x - containerChunk.position.x,
                y - containerChunk.position.y,
                z - containerChunk.position.z);

            return blockID;
        }
        else
        {
            //            Debug.LogError("Block isn't in containerChunk", this);
            return 0;
            //            return BlockDatabase.GetBlock(0);
        }

    }

    public BlockInstanceData GetBlockInstanceData(int x, int y, int z)
    {
        ChunkInstance containerChunk = GetChunk(x, y, z);

        if (containerChunk != null)
        {
            BlockInstanceData blockInstance = containerChunk.GetBlockInstanceData(
                x - containerChunk.position.x,
                y - containerChunk.position.y,
                z - containerChunk.position.z);

            return blockInstance;
        }
        else
        {
            //            Debug.LogError("Block isn't in containerChunk", this);
            //return 0;
            return null;
        }

    }

    public void SetBlockID(int x, int y, int z, ushort blockID)
    {
        ChunkInstance chunk = GetChunk(x, y, z);

        if (chunk != null)
        {
            chunk.SetBlock(x - chunk.position.x, y - chunk.position.y, z - chunk.position.z, blockID);
            chunk.update = true;

            UpdateIfEqual(x - chunk.position.x, 0, new WorldPos(x - 1, y, z));
            UpdateIfEqual(x - chunk.position.x, ChunkInstance.CHUNK_SIZE - 1, new WorldPos(x + 1, y, z));
            UpdateIfEqual(y - chunk.position.y, 0, new WorldPos(x, y - 1, z));
            UpdateIfEqual(y - chunk.position.y, ChunkInstance.CHUNK_SIZE - 1, new WorldPos(x, y + 1, z));
            UpdateIfEqual(z - chunk.position.z, 0, new WorldPos(x, y, z - 1));
            UpdateIfEqual(z - chunk.position.z, ChunkInstance.CHUNK_SIZE - 1, new WorldPos(x, y, z + 1));

        }
    }

    void UpdateIfEqual(int value1, int value2, WorldPos pos)
    {
        if (value1 == value2)
        {
            ChunkInstance chunk = GetChunk(pos.x, pos.y, pos.z);
            if (chunk != null)
                chunk.update = true;
        }
    }

    public static WorldPos ChunkInWorld(WorldPos v) => ChunkInWorld(v.x, v.y, v.z);

    public static WorldPos ChunkInWorld(int x, int y, int z)
    {
        return new WorldPos (
            Mathf.FloorToInt(x / (float)ChunkInstance.CHUNK_SIZE) * ChunkInstance.CHUNK_SIZE,
            Mathf.FloorToInt(y / (float)ChunkInstance.CHUNK_SIZE) * ChunkInstance.CHUNK_SIZE,
            Mathf.FloorToInt(z / (float)ChunkInstance.CHUNK_SIZE) * ChunkInstance.CHUNK_SIZE
               );

        //return new WorldPos(
        //    x < 0 ? (x + 1) / ChunkInstance.CHUNK_SIZE - 1 : x / ChunkInstance.CHUNK_SIZE,
        //    y < 0 ? (y + 1) / ChunkInstance.CHUNK_SIZE - 1 : y / ChunkInstance.CHUNK_SIZE,
        //    z < 0 ? (z + 1) / ChunkInstance.CHUNK_SIZE - 1 : z / ChunkInstance.CHUNK_SIZE
        //);
    }
}