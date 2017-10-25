using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkRenderer : MonoBehaviour {

//    public bool update = false;
    public bool rendered;

    MeshFilter filter;
    MeshCollider coll;

    public Chunk chunk;

    void Start()
    {
        filter = gameObject.GetComponent<MeshFilter>();
        coll = gameObject.GetComponent<MeshCollider>();
    }

    void Update()
    {
        if (chunk == null)
            return;
        if (chunk.update)
        {
            chunk.update = false;
            UpdateChunk();
        }
    }

    // Updates the chunk based on its contents
    void UpdateChunk()
    {
        rendered = true;
        MeshData meshData = new MeshData();

        for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
            {
                for (int z = 0; z < Chunk.CHUNK_SIZE; z++)
                {
                    if(chunk.blocks[x, y, z].ID != 0)
                        meshData = chunk.blocks[x, y, z].GetBlockdata(chunk, x, y, z, meshData);
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
        filter.mesh.SetVertices(meshData.vertices);
        filter.mesh.SetTriangles(meshData.triangles, 0);

        filter.mesh.SetUVs(0, meshData.uv);
        filter.mesh.RecalculateNormals();

        coll.sharedMesh = null;
        Mesh mesh = new Mesh();
        mesh.SetVertices(meshData.colVertices);
        mesh.SetTriangles(meshData.colTriangles, 0);
        mesh.RecalculateNormals();

        coll.sharedMesh = mesh;
    }
	
}
