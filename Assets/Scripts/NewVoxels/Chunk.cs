using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Chunk {

    public BlockInstance[, ,] blocks = new BlockInstance[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];

    public const int CHUNK_SIZE = 16;

    public World world;
    public WorldPos pos;

    public bool update = false;

    public void Update() {
        //TODO implement
    }

    public BlockInstance GetBlock(int x, int y, int z)
    {
        //TODO check how expensive this is and only do in debug depending
        if (InRange(x) && InRange(y) && InRange(z))
            return blocks[x, y, z];
        return world.GetBlock(pos.x + x, pos.y + y, pos.z + z);
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
        }
        else
        {
            world.SetBlock(pos.x + x, pos.y + y, pos.z + z, block);
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