using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkRenderer : MonoBehaviour {

//    public bool update = false;
    public bool rendered;

    MeshFilter filter;
    MeshCollider coll;

    public ChunkInstance chunk;

    public MeshData meshData = new MeshData();

    void Start()
    {
        filter = gameObject.GetComponent<MeshFilter>();
        coll = gameObject.GetComponent<MeshCollider>();
    }

    //void Update()
    //{
    //    if (chunk == null)
    //        return;
    //    if (chunk.update)
    //    {
    //        chunk.update = false;
    //        UpdateChunk();
    //    }
    //}

    // Updates the chunk based on its contents
    void UpdateChunk()
    {
        rendered = true;
        meshData.Clear();

        for (int x = 0; x < ChunkInstance.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < ChunkInstance.CHUNK_SIZE; y++)
            {
                for (int z = 0; z < ChunkInstance.CHUNK_SIZE; z++)
                {
                    if(chunk.blockIds[x, y, z] != 0)
                        meshData = BlockLoader.GetBlock(chunk.blockIds[x,y,z]).GetBlockdata(chunk, x, y, z, meshData);
                }
            }
        }

        RenderMesh();
    }

    // Sends the calculated mesh information
    // to the mesh and collision components
    void RenderMesh()
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
