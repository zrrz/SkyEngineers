using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent(typeof(MeshFilter))]
//[RequireComponent(typeof(MeshRenderer))]
//[RequireComponent(typeof(MeshCollider))]

public class ChunkInstance {

    public ushort[, ,] blockIds = new ushort[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];
    public readonly Dictionary<Vector3iChunk, BlockInstanceData> blockInstanceData = new Dictionary<Vector3iChunk, BlockInstanceData>();

    public const int CHUNK_SIZE = 16;

    public World world;
    public WorldPos position;

    public bool update = false;

    public bool needsSaving = false;

    public System.DateTime Time;

    public WorldPos min = new WorldPos(CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE);
    public WorldPos max = new WorldPos(-1, -1, -1);

    //public ChunkInstance(World world, WorldPos position)
    //{
    //    this.world = world;
    //    this.position = position;

    //    Time = System.DateTime.Now;
    //}

    public ChunkInstance(CachedChunk cachedChunk)/* : this(cachedChunk.world, cachedChunk.position)*/
    {
        //TODO fix
        //for (int x = 0; x < CHUNK_SIZE; x++) {
        //    for (int y = 0; y < CHUNK_SIZE; y++)
        //    {
        //        for (int z = 0; z < CHUNK_SIZE; z++)
        //        {
        //            blockIds[x, y, z] = cachedChunk.blockIds[x, y, z];
        //            //SetBlock(x, y, z, cachedChunk.blockIds[x, y, z]);
        //        }
        //    }
        //}
        blockIds = cachedChunk.blockIds;

        //_lightLevels = cachedChunk.LightLevels;
        //blockDatas = cachedChunk.blockDatas;
		world = cachedChunk.world;
        position = cachedChunk.position;
        min = cachedChunk.min;
        max = cachedChunk.max;

        Time = System.DateTime.Now;
    }

    public void Update() {
        //TODO implement
    }

    public ushort GetBlock(int x, int y, int z)
    {
        //TODO check how expensive this is and only do in debug depending
        if (InRange(x) && InRange(y) && InRange(z))
            return blockIds[x, y, z];
        return world.GetBlock(position.x + x, position.y + y, position.z + z);
    }

    public BlockInstanceData GetBlockInstanceData(int x, int y, int z)
    {
        //TODO check how expensive this is and only do in debug depending
        if (InRange(x) && InRange(y) && InRange(z))
            return blockInstanceData[new Vector3iChunk(x, y, z)];
        return world.GetBlockInstanceData(position.x + x, position.y + y, position.z + z);
    }

    public static bool InRange(int index)
    {
        if (index < 0 || index >= CHUNK_SIZE)
            return false;

        return true;
    }

    public void SetBlock(int x, int y, int z, ushort blockID, bool setChanged = true)
    {
        if (InRange(x) && InRange(y) && InRange(z))
        {
            //BlockInstance newBlock;
            //newBlock = blocks[x, y, z];
            //if (newBlock == null)
            //{
            //    newBlock = new BlockInstance();
            //}
            //newBlock.ID = block.ID;
            //newBlock.changed = setChanged;

            //TODO whatever else I need to do to set the block instance
            blockIds[x, y, z] = blockID;

            //TODO metadata
        }
        else
        {
            world.SetBlockID(position.x + x, position.y + y, position.z + z, blockID);
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