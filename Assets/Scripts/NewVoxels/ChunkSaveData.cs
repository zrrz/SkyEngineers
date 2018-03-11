using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MessagePack;

[MessagePackObject]
public class ChunkSaveData
{
    [Key(0)]
    public Dictionary<WorldPos, BlockInstance> blocks;
//    int[] blocks

    public ChunkSaveData() {
        blocks = new Dictionary<WorldPos, BlockInstance>();
    }

    public ChunkSaveData(ChunkInstance chunk)
    {
        blocks = new Dictionary<WorldPos, BlockInstance>();
        for (int x = 0; x < ChunkInstance.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < ChunkInstance.CHUNK_SIZE; y++)
            {
                for (int z = 0; z < ChunkInstance.CHUNK_SIZE; z++)
                {
                    if (!chunk.blocks[x, y, z].changed)
                        continue;

                    WorldPos pos = new WorldPos(x, y, z);
                    blocks.Add(pos, chunk.blocks[x, y, z]);
                }
            }
        }
    }
}