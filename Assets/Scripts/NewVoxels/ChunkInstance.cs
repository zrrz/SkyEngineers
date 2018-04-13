using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent(typeof(MeshFilter))]
//[RequireComponent(typeof(MeshRenderer))]
//[RequireComponent(typeof(MeshCollider))]

public class ChunkInstance {

    //public ushort[] blockIds = new ushort[CHUNK_SIZE * CHUNK_SIZE * CHUNK_SIZE];
    public BlockInstanceData[] blockInstanceData = new BlockInstanceData[CHUNK_SIZE * CHUNK_SIZE * CHUNK_SIZE];
    //public readonly Dictionary<Vector3iChunk, BlockInstanceData> blockInstanceData = new Dictionary<Vector3iChunk, BlockInstanceData>();

    public const int CHUNK_SIZE = 16;

    public readonly CachedChunk chunkData;

    public bool update = false;

    public bool needsSaving = false;

    public System.DateTime Time;

    //I'm duplicating them to save some calls to chunkData. Maybe an optimization?
    public readonly World world;
    public readonly WorldPos position;

    /**
     * Whether this Chunk has any Entities and thus requires saving on every tick
     */
    private bool hasEntities;


    //public ChunkInstance(World world, WorldPos position)
    //{
    //    this.world = world;
    //    this.position = position;

    //    Time = System.DateTime.Now;
    //}

    public ChunkInstance(CachedChunk cachedChunk)/* : this(cachedChunk.world, cachedChunk.position)*/
    {
        chunkData = cachedChunk;
        world = cachedChunk.world;
        position = cachedChunk.position;
        //TODO fix
        //for (int i = 0, n = CHUNK_SIZE*CHUNK_SIZE*CHUNK_SIZE; i < n; i++) {
        //    blockInstanceData[i] = new BlockInstanceData(cachedChunk.blockIds[i], cachedChunk.blockIds[i] != 0);
        //}
        //blockIds = cachedChunk.blockIds;

        //_lightLevels = cachedChunk.LightLevels;
        //blockDatas = cachedChunk.blockDatas;
		//world = cachedChunk.world;
        //position = cachedChunk.position;

        Time = System.DateTime.Now;

        //ChunksLoadedVisualizer.SetChunkLoadedState(position, true, true);
    }

    //~ChunkInstance() {
        //ChunksLoadedVisualizer.SetChunkLoadedState(position, false);
    //}

    public void Update() {
        //TODO implement
    }

    /// <summary>
    /// Gets the block with extra data except ID stripped out.
    /// </summary>
    /// <returns>The block identifier.</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="z">The z coordinate.</param>
    public ushort GetBlockID(int x, int y, int z)
    {
        //UnityEngine.Profiling.Profiler.BeginSample("Instance GetBlock");
        ushort blockID;
        if (x >= 0 && x < CHUNK_SIZE && y >= 0 && y < CHUNK_SIZE && z >= 0 && z < CHUNK_SIZE)
            blockID = chunkData.blockIds[x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE];
        else
            blockID = chunkData.world.GetBlock(position.x + x, position.y + y, position.z + z);

        blockID &= 0x3FFF; //Strip out anything but first bits

        //UnityEngine.Profiling.Profiler.EndSample();
        return blockID;
    }

    /// <summary>
    /// Gets the block with extra data included
    /// </summary>
    /// <returns>The block.</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="z">The z coordinate.</param>
    public ushort GetBlock(int x, int y, int z)
    {
        //UnityEngine.Profiling.Profiler.BeginSample("Instance GetBlock");
        ushort blockID;
        if (x >= 0 && x < CHUNK_SIZE && y >= 0 && y < CHUNK_SIZE && z >= 0 && z < CHUNK_SIZE)
            blockID = chunkData.blockIds[x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE];
        else
            blockID = chunkData.world.GetBlock(position.x + x, position.y + y, position.z + z);

        //UnityEngine.Profiling.Profiler.EndSample();
        return blockID;
    }

    public BlockInstanceData GetBlockInstanceData(int x, int y, int z)
    {
        //TODO check how expensive this is and only do in debug depending
        if (x >= 0 && x < CHUNK_SIZE && y >= 0 && y < CHUNK_SIZE && z >= 0 && z < CHUNK_SIZE)
            return blockInstanceData[x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE];
        return chunkData.world.GetBlockInstanceData(position.x + x, position.y + y, position.z + z);
    }

    //public static bool InRange(int index)
    //{
    //    if (index < 0 || index >= CHUNK_SIZE)
    //        return false;

    //    return true;
    //}

    public void SetBlock(int x, int y, int z, ushort blockID, bool setChanged = true)
    {
        if (x >= 0 && x < CHUNK_SIZE && y >= 0 && y < CHUNK_SIZE && z >= 0 && z < CHUNK_SIZE)
        {
            //TODO whatever else I need to do to set the block instance
            chunkData.blockIds[x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE] = blockID;

            //TODO metadata

            needsSaving = true;
            update = true;
        }
        else
        {
            chunkData.world.SetBlockID(position.x + x, position.y + y, position.z + z, blockID);
        }
    }

//    public void SetBlocksUnmodified()
//    {
//        foreach (BlockInstance block in blocks)
//        {
//            block.changed = false;
//        }
//    }
}