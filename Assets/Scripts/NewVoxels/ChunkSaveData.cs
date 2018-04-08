using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MessagePack;

[MessagePackObject]
public class ChunkSaveData
{
    [Key(0)]
    public Dictionary<WorldPos, BlockInstanceData> blocks;
//    int[] blocks

    public ChunkSaveData() {
        blocks = new Dictionary<WorldPos, BlockInstanceData>();
    }

    public ChunkSaveData(ChunkInstance chunk)
    {
        blocks = new Dictionary<WorldPos, BlockInstanceData>();
        for (int x = 0; x < ChunkInstance.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < ChunkInstance.CHUNK_SIZE; y++)
            {
                for (int z = 0; z < ChunkInstance.CHUNK_SIZE; z++)
                {
                    //if (!chunk.blockInstanceData[x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE].changed)
                        //continue;

                    //WorldPos pos = new WorldPos(x, y, z);
                    //blocks.Add(pos, chunk.blocks[x, y, z]);
                }
            }
        }
    }
}