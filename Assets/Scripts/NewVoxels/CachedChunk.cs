using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CachedChunk {

    //TODO sub worlds

    public readonly World world;
    public readonly WorldPos position;
    public readonly ushort[,,] blockIds = new ushort[ChunkInstance.CHUNK_SIZE, ChunkInstance.CHUNK_SIZE, ChunkInstance.CHUNK_SIZE];
    //public readonly LightLevel[,,] LightLevels = new LightLevel[Chunk.Size, Chunk.Size, Chunk.Size];
    public readonly Dictionary<WorldPos, BlockData> blockDatas = new Dictionary<WorldPos, BlockData>();

    public bool IsEmpty => min.x == ChunkInstance.CHUNK_SIZE;

    public WorldPos min = new WorldPos(ChunkInstance.CHUNK_SIZE, ChunkInstance.CHUNK_SIZE, ChunkInstance.CHUNK_SIZE);
    public WorldPos max = new WorldPos(-1, -1, -1);

    internal CachedChunk(World world, WorldPos position)
    {
        this.world = world;
        this.position = position;
    }

    public CachedChunk(World world, WorldPos position, BinaryReader reader) : this(world, position)
    {
        min = new WorldPos(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
        max = new WorldPos(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());

        for (var x = min.x; x <= max.x; x++)
            for (var y = min.y; y <= max.y; y++)
                for (var z = min.z; z <= max.z; z++)
                {
                    blockIds[x, y, z] = reader.ReadUInt16();
                    //LightLevels[x, y, z] = LightLevel.FromBinary(reader.ReadUInt16());
                }

        var blockDataCount = reader.ReadInt32();
        for (var i = 0; i < blockDataCount; i++)
        {
            WorldPos blockDataPos = Vector3iChunk.FromBinary(reader.ReadUInt16());
            var blockData = BlockData.ReadFromStream(reader);

            blockDatas.Add(blockDataPos, blockData);
        }
    }

    public void SetBlock(int x, int y, int z, BlockData block)
    {
        if (blockIds[x, y, z] == block.ID) return;

        blockIds[x, y, z] = block.ID;

        if (x < min.x) min.x = x;
        if (y < min.y) min.y = y;
        if (z < min.z) min.z = z;
        if (x > max.x) max.x = x;
        if (y > max.y) max.y = y;
        if (z > max.z) max.z = z;
    }

    public BlockData GetBlock(int x, int y, int z)
    {
        if (x < min.x || x > max.x ||
            y < min.y || y > max.y ||
            z < min.z || z > max.z)
            return BlockLoader.GetBlock(0);

        return null;
        //return GameRegistry.BlockRegistry[blockIds[x, y, z]];

        //TODO fix this shit
    }
}
