using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Chunk : MonoBehaviour {

    public BlockInstance[, ,] blocks = new BlockInstance[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];

    public const int CHUNK_SIZE = 16;
    public bool update = false;
    public bool rendered;

    MeshFilter filter;
    MeshCollider coll;

    public World world;
    public WorldPos pos;

    void Start()
    {
        filter = gameObject.GetComponent<MeshFilter>();
        coll = gameObject.GetComponent<MeshCollider>();
    }

    //Update is called once per frame
    void Update()
    {
        if (update)
        {
            update = false;
            UpdateChunk();
        }
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

    public void SetBlock(int x, int y, int z, BlockData block)
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
            newBlock.changed = true;

            //TODO whatever else I need to do to set the block instance
            blocks[x, y, z] = newBlock;
        }
        else
        {
            world.SetBlock(pos.x + x, pos.y + y, pos.z + z, block);
        }
    }

    public void SetBlocksUnmodified()
    {
        foreach (BlockInstance block in blocks)
        {
            block.changed = false;
        }
    }

    // Updates the chunk based on its contents
    void UpdateChunk()
    {
        rendered = true;
        MeshData meshData = new MeshData();

        for (int x = 0; x < CHUNK_SIZE; x++)
        {
            for (int y = 0; y < CHUNK_SIZE; y++)
            {
                for (int z = 0; z < CHUNK_SIZE; z++)
                {
                    if(blocks[x, y, z].ID != 0)
                        meshData = blocks[x, y, z].GetBlockdata(this, x, y, z, meshData);
                }
            }
        }

        RenderMesh(meshData);
    }

    // Sends the calculated mesh information
    // to the mesh and collision components
    void RenderMesh(MeshData meshData)
    {
        filter.mesh.Clear();
        filter.mesh.vertices = meshData.vertices.ToArray();
        filter.mesh.triangles = meshData.triangles.ToArray();

        filter.mesh.uv = meshData.uv.ToArray();
        filter.mesh.RecalculateNormals();

        coll.sharedMesh = null;
        Mesh mesh = new Mesh();
        mesh.vertices = meshData.colVertices.ToArray();
        mesh.triangles = meshData.colTriangles.ToArray();
        mesh.RecalculateNormals();

        coll.sharedMesh = mesh;
    }

}