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

    void Awake()
    {
        filter = gameObject.GetComponent<MeshFilter>();
        coll = gameObject.GetComponent<MeshCollider>();
        //ChunksLoadedVisualizer.SetChunkLoadedState(chunk.position, true, transform);
    }

    // Updates the chunk based on its contents
    public void UpdateChunk()
    {
        if (chunk == null)
        {
            Debug.LogError("Chunk is null");
            return;
        }

        chunk.update = false;

        rendered = true;

        UnityEngine.Profiling.Profiler.BeginSample("Clear meshData");
        meshData.Clear();
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("Iterate block mesh data");
        for (int x = 0; x < ChunkInstance.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < ChunkInstance.CHUNK_SIZE; y++)
            {
                for (int z = 0; z < ChunkInstance.CHUNK_SIZE; z++)
                {
                    int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                    if (chunk.chunkData.blockIds[index] != 0)
                    {
                        meshData = BlockLoader.GetBlock(chunk.chunkData.blockIds[index]).GetBlockdata(chunk, x, y, z, meshData);
                    }
                }
            }
        }
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("RenderMesh");
        RenderMesh();
        UnityEngine.Profiling.Profiler.EndSample();
    }

    // Sends the calculated mesh information
    // to the mesh and collision components
    void RenderMesh()
    {
        //filter.mesh.Clear();
        //filter.mesh.SetVertices(meshData.vertices);
        //filter.mesh.SetTriangles(meshData.triangles, 0);

        //filter.mesh.SetUVs(0, meshData.uv);
        //filter.mesh.RecalculateNormals();

        //coll.sharedMesh = null;
        Mesh mesh = filter.sharedMesh;
        if (mesh == null)
            mesh = new Mesh();
        mesh.Clear(false);
        mesh.SetVertices(meshData.vertices);
        mesh.SetTriangles(meshData.triangles, 0);
        mesh.SetUVs(0, meshData.uv);
        mesh.RecalculateNormals();

        if(meshData.useRenderDataForCol)
            filter.sharedMesh = mesh;
        coll.sharedMesh = mesh;
    }
	
}
