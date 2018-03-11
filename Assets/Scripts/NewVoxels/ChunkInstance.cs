using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(MeshFilter))]
//[RequireComponent(typeof(MeshRenderer))]
//[RequireComponent(typeof(MeshCollider))]

public class ChunkInstance {

    public BlockInstance[, ,] blocks = new BlockInstance[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];

    public const int CHUNK_SIZE = 16;

    public World world;
    public WorldPos position;

    public bool update = false;

    public System.DateTime Time;

    //TODO this is an optimization step to only iterate from min to max. Implement this
    //public WorldPos min = new WorldPos(ChunkInstance.CHUNK_SIZE, ChunkInstance.CHUNK_SIZE, ChunkInstance.CHUNK_SIZE);
    //public WorldPos max = new WorldPos(-1, -1, -1);

    public ChunkInstance(World world, WorldPos position)
    {
        this.world = world;
        this.position = position;

        Time = System.DateTime.Now;
    }

    internal ChunkInstance(CachedChunk cachedChunk) : this(cachedChunk.world, cachedChunk.position)
    {
        //blocks = cachedChunk.blockIds;
        //_lightLevels = cachedChunk.LightLevels;
        //_blockDatas = cachedChunk.blockDatas;
        //min = cachedChunk.min;
        //max = cachedChunk.max;
    }

    public void Update() {
        //TODO implement
    }

    public BlockInstance GetBlock(int x, int y, int z)
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

    public void SetBlock(int x, int y, int z, BlockData block, bool setChanged = true)
    {
        if (InRange(x) && InRange(y) && InRange(z))
        {
            BlockInstance newBlock;
            newBlock = blocks[x, y, z];
            if (newBlock == null)
            {
                newBlock = new BlockInstance();
            }
            newBlock.ID = block.ID;
            newBlock.changed = setChanged;

            //TODO whatever else I need to do to set the block instance
            blocks[x, y, z] = newBlock;

            //TODO metadata
        }
        else
        {
            world.SetBlock(position.x + x, position.y + y, position.z + z, block);
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