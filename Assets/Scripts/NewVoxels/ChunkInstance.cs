using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent(typeof(MeshFilter))]
//[RequireComponent(typeof(MeshRenderer))]
//[RequireComponent(typeof(MeshCollider))]

public class ChunkInstance {

    public int[, ,] blocks = new int[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];
    public readonly Dictionary<Vector3iChunk, BlockInstance> blockDatas = new Dictionary<Vector3iChunk, BlockInstance>();

    public const int CHUNK_SIZE = 16;

    public World world;
    public WorldPos position;

    public bool update = false;

    public bool needsSaving = false;

    public System.DateTime Time;

    public WorldPos min = new WorldPos(ChunkInstance.CHUNK_SIZE, ChunkInstance.CHUNK_SIZE, ChunkInstance.CHUNK_SIZE);
    public WorldPos max = new WorldPos(-1, -1, -1);

    public ChunkInstance(World world, WorldPos position)
    {
        this.world = world;
        this.position = position;

        Time = System.DateTime.Now;
    }

    internal ChunkInstance(CachedChunk cachedChunk) : this(cachedChunk.world, cachedChunk.position)
    {
        //TODO fix
        for (int x = 0; x < CHUNK_SIZE; x++) {
            for (int y = 0; y < CHUNK_SIZE; y++)
            {
                for (int z = 0; z < CHUNK_SIZE; z++)
                {
                    SetBlock(x, y, z, cachedChunk.blockIds[x, y, z]);
                }
            }
        }

        //_lightLevels = cachedChunk.LightLevels;
        //blockDatas = cachedChunk.blockDatas;
		world = cachedChunk.world;
        position = cachedChunk.position;
        min = cachedChunk.min;
        max = cachedChunk.max;
    }

    public void Update() {
        //TODO implement
    }

    public int GetBlock(int x, int y, int z)
    {
        //TODO check how expensive this is and only do in debug depending
        if (InRange(x) && InRange(y) && InRange(z))
            return blocks[x, y, z];
        return world.GetBlock(position.x + x, position.y + y, position.z + z);
    }

    public static bool InRange(int index)
    {
        if (index < 0 || index >= CHUNK_SIZE)
            return false;

        return true;
    }

    public void SetBlock(int x, int y, int z, int blockID, bool setChanged = true)
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
            blocks[x, y, z] = blockID;

            //TODO metadata
        }
        else
        {
            world.SetBlock(position.x + x, position.y + y, position.z + z, blockID);
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