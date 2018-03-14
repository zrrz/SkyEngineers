using UnityEngine;
using System.Collections;
using SimplexNoise;

public class TerrainGenerator
{

    float stoneBaseHeight = -24;
    float stoneBaseNoise = 0.05f;
    float stoneBaseNoiseHeight = 4;

    float stoneMountainHeight = 48;
    float stoneMountainFrequency = 0.008f;
    float stoneMinHeight = -12;

    float dirtBaseHeight = 1;
    float dirtNoise = 0.04f;
    float dirtNoiseHeight = 3;

    float caveFrequency = 0.025f;
    int caveSize = 7;

    float treeFrequency = 0.2f;
    int treeDensity = 3;

    public CachedChunk ChunkGen(CachedChunk chunk)
    {
        for (int x = chunk.position.x - 3; x < chunk.position.x + ChunkInstance.CHUNK_SIZE + 3; x++) //Change this line
        {
            for (int z = chunk.position.z - 3; z < chunk.position.z + ChunkInstance.CHUNK_SIZE + 3; z++)//and this line
            {
                chunk = ChunkColumnGen(chunk, x, z);
            }
        }
        return chunk;
    }

    public CachedChunk ChunkColumnGen(CachedChunk chunk, int x, int z)
    {
        int stoneHeight = Mathf.FloorToInt(stoneBaseHeight);
        stoneHeight += GetNoise(x, 0, z, stoneMountainFrequency, Mathf.FloorToInt(stoneMountainHeight));

        if (stoneHeight < stoneMinHeight)
            stoneHeight = Mathf.FloorToInt(stoneMinHeight);

        stoneHeight += GetNoise(x, 0, z, stoneBaseNoise, Mathf.FloorToInt(stoneBaseNoiseHeight));

        int dirtHeight = stoneHeight + Mathf.FloorToInt(dirtBaseHeight);
        dirtHeight += GetNoise(x, 100, z, dirtNoise, Mathf.FloorToInt(dirtNoiseHeight));

        for (int y = chunk.position.y - 8; y < chunk.position.y + ChunkInstance.CHUNK_SIZE; y++)
        {
            //Get a value to base cave generation on
            int caveChance = GetNoise(x, y, z, caveFrequency, 100);

            if (y <= stoneHeight && caveSize < caveChance)
            {
                SetBlock(x, y, z, BlockLoader.GetBlock(1), chunk);
            }
            else if (y <= dirtHeight && caveSize < caveChance)
            {
                SetBlock(x, y, z, BlockLoader.GetBlock(2), chunk);

                if (y == dirtHeight && GetNoise(x, 0, z, treeFrequency, 100) < treeDensity)
                    CreateTree(x, y + 1, z, chunk);
            }
            else
            {
                SetBlock(x, y, z, BlockLoader.GetBlock(0), chunk);
            }

        }

        return chunk;
    }

    void CreateTree(int x, int y, int z, CachedChunk chunk)
    {
        //create leaves
        for (int xi = -2; xi <= 2; xi++)
        {
            for (int yi = 4; yi <= 8; yi++)
            {
                for (int zi = -2; zi <= 2; zi++)
                {
                    SetBlock(x + xi, y + yi, z + zi, BlockLoader.GetBlock(3), chunk, true);
                }
            }
        }

        //create trunk
        for (int yt = 0; yt < 6; yt++)
        {
            SetBlock(x, y + yt, z, BlockLoader.GetBlock(4), chunk, true);
        }
    }

    public static void SetBlock(int x, int y, int z, BlockData block, CachedChunk chunk, bool replaceBlocks = false)
    {
        x -= chunk.position.x;
        y -= chunk.position.y;
        z -= chunk.position.z;

        if (ChunkInstance.InRange(x) && ChunkInstance.InRange(y) && ChunkInstance.InRange(z))
        {
            if (replaceBlocks || chunk.blockIds[x, y, z] == 0)
            {
                chunk.SetBlock(x, y, z, block);
            }
        }
    }

    public static int GetNoise(int x, int y, int z, float scale, int max)
    {
        return Mathf.FloorToInt((Noise.Generate(x * scale, y * scale, z * scale) + 1f) * (max / 2f));
    }
}