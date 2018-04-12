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

        if(!meshData.isClear)
            meshData.Clear();

        UnityEngine.Profiling.Profiler.BeginSample("Iterate block mesh data");
        if (!chunk.chunkData.IsEmpty)
        {
            WorldPos min = chunk.chunkData.min;
            WorldPos max = chunk.chunkData.max;
            for (int x = min.x; x <= max.x; x++) 
            {
                for (int y = min.y; y <= max.y; y++)
                {
                    for (int z = min.x; z <= max.z; z++)
                    {
                        int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                        if ((chunk.chunkData.blockIds[index] & 1 << 14) > 0) //If I'm solid then lets look
                        {
                            if (x >= 1 && x < ChunkInstance.CHUNK_SIZE - 1 && y >= 1 && y < ChunkInstance.CHUNK_SIZE - 1 && z >= 1 && z < ChunkInstance.CHUNK_SIZE - 1)
                            {
                                //Inside chunk so less safety checks
                                ushort id = chunk.chunkData.blockIds[index - ChunkInstance.CHUNK_SIZE];
                                //int solid = id & 1 << 14;
                                //int blockID = id & ~(1 << 14);
                                //Debug.log
                                if ((id & 1 << 14) > 0)
                                {
                                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));

                                    meshData.AddQuadTriangles();
                                    meshData.AddUVs(BlockData.FaceUVs(id & ~(1 << 14), BlockData.Direction.Up));
                                }

                                id = chunk.chunkData.blockIds[index + ChunkInstance.CHUNK_SIZE];
                                if ((id & 1 << 14) > 0)
                                {
                                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

                                    meshData.AddQuadTriangles();
                                    meshData.AddUVs(BlockData.FaceUVs(id & ~(1 << 14), BlockData.Direction.Down));
                                }
                                id = chunk.chunkData.blockIds[index - ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE];
                                if ((id & 1 << 14) > 0)
                                {
                                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

                                    meshData.AddQuadTriangles();
                                    meshData.AddUVs(BlockData.FaceUVs(id & ~(1 << 14), BlockData.Direction.North));
                                }
                                id = chunk.chunkData.blockIds[index + ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE];
                                if ((id & 1 << 14) > 0)
                                {
                                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));

                                    meshData.AddQuadTriangles();
                                    meshData.AddUVs(BlockData.FaceUVs(id & ~(1 << 14), BlockData.Direction.South));
                                }
                                id = chunk.chunkData.blockIds[index - 1];
                                if ((id & 1 << 14) > 0)
                                {
                                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));

                                    meshData.AddQuadTriangles();
                                    meshData.AddUVs(BlockData.FaceUVs(id & ~(1 << 14), BlockData.Direction.East));
                                }
                                id = chunk.chunkData.blockIds[index + 1];
                                if ((id & 1 << 14) > 0)
                                {
                                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));

                                    meshData.AddQuadTriangles();
                                    meshData.AddUVs(BlockData.FaceUVs(id & ~(1 << 14), BlockData.Direction.West));
                                }
                            }
                            else
                            {
								//if (chunk.chunkData.blockIds[index] != 0)
								//{
                                //int blockID = chunk.chunkData.blockIds[index] & ~(1 << 14);
								//meshData = BlockLoader.GetBlock(blockID).GetBlockdata(chunk, x, y, z, meshData);
								//}
                            } 
                        }
                    }
                }
            }
            //for (int x = 0; x < ChunkInstance.CHUNK_SIZE; x++)
            //{
            //    for (int y = 0; y < ChunkInstance.CHUNK_SIZE; y++)
            //    {
            //        for (int z = 0; z < ChunkInstance.CHUNK_SIZE; z++)
            //        {
            //            int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
            //            if (chunk.chunkData.blockIds[index] != 0)
            //            {
            //                meshData = BlockLoader.GetBlock(chunk.chunkData.blockIds[index]).GetBlockdata(chunk, x, y, z, meshData);
            //            }
            //        }
            //    }
            //}
        }

        //Just check on if its lists to see if its still empty.
        if (meshData.triangles.Count > 0)
            meshData.isClear = false;
        
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
