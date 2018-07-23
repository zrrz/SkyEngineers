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

            //Debug.LogError("Min:" + min.x + ", " + min.y + ", " + min.z, this);
            //Debug.LogError("Max:" + max.x + ", " + max.y + ", " + max.z, this);

            //ChunkInstance upChunk = null;
            //if (max.y == ChunkInstance.CHUNK_SIZE - 1)
            //{
            //    upChunk = chunk.world.GetChunk(chunk.position.x, chunk.position.y + 1, chunk.position.z);
            //    if (upChunk == null)
            //        Debug.LogError("Can't find Up Chunk", this);
            //}
            //ChunkInstance downChunk = null;
            //if (min.y == 0)
            //{
            //    downChunk = chunk.world.GetChunk(chunk.position.x, chunk.position.y - 1, chunk.position.z);
            //    if (downChunk == null)
            //        Debug.LogError("Can't find Down Chunk", this);
            //}
            //ChunkInstance eastChunk = null;
            //if (max.x == ChunkInstance.CHUNK_SIZE - 1)
            //    eastChunk = chunk.world.GetChunk(chunk.position.x + ChunkInstance.CHUNK_SIZE, chunk.position.y, chunk.position.z);
            //ChunkInstance westChunk = null;
            //if (min.x == 0)
            //    westChunk = chunk.world.GetChunk(chunk.position.x + ChunkInstance.CHUNK_SIZE, chunk.position.y, chunk.position.z);
            //ChunkInstance northChunk = null;
            //if (max.z == ChunkInstance.CHUNK_SIZE - 1)
            //    northChunk = chunk.world.GetChunk(chunk.position.x, chunk.position.y, chunk.position.z + ChunkInstance.CHUNK_SIZE);
            //ChunkInstance southChunk = null;
            //if (max.z == 0)
                //southChunk = chunk.world.GetChunk(chunk.position.x, chunk.position.y, chunk.position.z - ChunkInstance.CHUNK_SIZE);


            //for (int x = Mathf.Max(min.x, 1), n1 = Mathf.Min(max.x, ChunkInstance.CHUNK_SIZE-2); x <= n1; x++) 
            //{
                //for (int y = Mathf.Max(min.y, 1), n2 = Mathf.Min(max.y, ChunkInstance.CHUNK_SIZE-2); y <= n2; y++)
                //{
                    //for (int z = Mathf.Max(min.x, 1), n3 = Mathf.Min(max.z, ChunkInstance.CHUNK_SIZE-2); z <= n3; z++)
                    //{
                        
            for (int x = min.x, n1 = max.x; x <= n1; x++)
            {
                for (int y = min.y, n2 = max.y; y <= n2; y++)
                {
                    for (int z = min.z, n3 = max.z; z <= n3; z++)
                    {
                        int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                        int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                        //if ((chunk.chunkData.blockIds[index] & 1 << 14) > 0) //If I'm solid then lets look
                        if(currentBlockID != 0)
                        {
                            //if (!(x >= 1 && x < ChunkInstance.CHUNK_SIZE - 1 && y >= 1 && y < ChunkInstance.CHUNK_SIZE - 1 && z >= 1 && z < ChunkInstance.CHUNK_SIZE - 1))
                            //{
                            //    Debug.LogError("SHOULDNT HAPPEN: " + x + "-" + y + "-" + z);
                            //}
                            UnityEngine.Profiling.Profiler.BeginSample("Render Inside");
                            ushort id = 0;
                            //Inside chunk so less safety checks
                            if (y != ChunkInstance.CHUNK_SIZE - 1)
                            {
                                id = chunk.chunkData.blockIds[index + ChunkInstance.CHUNK_SIZE];
                                if ((id & 1 << 14) == 0)
                                {
                                    UnityEngine.Profiling.Profiler.BeginSample("Alloc up Face");

                                    Vector3[] vertices = new Vector3[4];
                                    //Vector3[] vertices = MemoryPoolManager.vector3ArrayPool.Rent(4);
                                    vertices[0].Set(x - 0.5f, y + 0.5f, z + 0.5f);
                                    vertices[1].Set(x + 0.5f, y + 0.5f, z + 0.5f);
                                    vertices[2].Set(x + 0.5f, y + 0.5f, z - 0.5f);
                                    vertices[3].Set(x - 0.5f, y + 0.5f, z - 0.5f);
                                    meshData.AddVertices(vertices);
                                    //meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                                    //meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                                    //meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                                    //meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));

                                    meshData.AddQuadTriangles();
                                    meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.Up));

									UnityEngine.Profiling.Profiler.EndSample();
                                }
                            }

                            if (y != 0)
                            {
                                id = chunk.chunkData.blockIds[index - ChunkInstance.CHUNK_SIZE];
                                if ((id & 1 << 14) == 0)
                                {
                                    UnityEngine.Profiling.Profiler.BeginSample("Alloc down Face");
                                    UnityEngine.Profiling.Profiler.BeginSample("Get verts");
                                    Vector3[] vertices = new Vector3[4];
                                    //Vector3[] vertices = MemoryPoolManager.vector3ArrayPool.Rent(4);
                                    UnityEngine.Profiling.Profiler.EndSample();

                                    UnityEngine.Profiling.Profiler.BeginSample("set verts");
                                    vertices[0].Set(x - 0.5f, y - 0.5f, z - 0.5f);
                                    vertices[1].Set(x + 0.5f, y - 0.5f, z - 0.5f);
                                    vertices[2].Set(x + 0.5f, y - 0.5f, z + 0.5f);
                                    vertices[3].Set(x - 0.5f, y - 0.5f, z + 0.5f);
                                    UnityEngine.Profiling.Profiler.EndSample();

                                    UnityEngine.Profiling.Profiler.BeginSample("add verts to mesh data");
                                    meshData.AddVertices(vertices);
                                    UnityEngine.Profiling.Profiler.EndSample();

                                    //meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
                                    //meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
                                    //meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
                                    //meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

                                    UnityEngine.Profiling.Profiler.BeginSample("add quad triangles");
                                    meshData.AddQuadTriangles();
                                    UnityEngine.Profiling.Profiler.EndSample();

                                    UnityEngine.Profiling.Profiler.BeginSample("add uvs");
                                    meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.Down));
                                    UnityEngine.Profiling.Profiler.EndSample();

                                    UnityEngine.Profiling.Profiler.EndSample();
                                }
                            }

                            if (z != ChunkInstance.CHUNK_SIZE - 1)
                            {
                                id = chunk.chunkData.blockIds[index + ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE];
                                if ((id & 1 << 14) == 0)
                                {
                                    UnityEngine.Profiling.Profiler.BeginSample("Alloc north Face");

                                    Vector3[] vertices = new Vector3[4];
                                    //Vector3[] vertices = MemoryPoolManager.vector3ArrayPool.Rent(4);
                                    vertices[0].Set(x + 0.5f, y - 0.5f, z + 0.5f);
                                    vertices[1].Set(x + 0.5f, y + 0.5f, z + 0.5f);
                                    vertices[2].Set(x - 0.5f, y + 0.5f, z + 0.5f);
                                    vertices[3].Set(x - 0.5f, y - 0.5f, z + 0.5f);
                                    meshData.AddVertices(vertices);

                                    //meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
                                    //meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                                    //meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                                    //meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

                                    meshData.AddQuadTriangles();
                                    meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.North));

									UnityEngine.Profiling.Profiler.EndSample();
                                }
                            }

                            if (z != 0)
                            {
                                id = chunk.chunkData.blockIds[index - ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE];
                                if ((id & 1 << 14) == 0)
                                {
                                    UnityEngine.Profiling.Profiler.BeginSample("Alloc south Face");

                                    Vector3[] vertices = new Vector3[4];
                                    //Vector3[] vertices = MemoryPoolManager.vector3ArrayPool.Rent(4);
                                    vertices[0].Set(x - 0.5f, y - 0.5f, z - 0.5f);
                                    vertices[1].Set(x - 0.5f, y + 0.5f, z - 0.5f);
                                    vertices[2].Set(x + 0.5f, y + 0.5f, z - 0.5f);
                                    vertices[3].Set(x + 0.5f, y - 0.5f, z - 0.5f);
                                    meshData.AddVertices(vertices);

                                    //meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
                                    //meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
                                    //meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                                    //meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));

                                    meshData.AddQuadTriangles();
                                    meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.South));

									UnityEngine.Profiling.Profiler.EndSample();
                                }
                            }

                            if (x != ChunkInstance.CHUNK_SIZE - 1)
                            {
                                id = chunk.chunkData.blockIds[index + 1];
                                if ((id & 1 << 14) == 0)
                                {
                                    UnityEngine.Profiling.Profiler.BeginSample("Alloc east Face");

                                    Vector3[] vertices = new Vector3[4];
                                    //Vector3[] vertices = MemoryPoolManager.vector3ArrayPool.Rent(4);
                                    vertices[0].Set(x + 0.5f, y - 0.5f, z - 0.5f);
                                    vertices[1].Set(x + 0.5f, y + 0.5f, z - 0.5f);
                                    vertices[2].Set(x + 0.5f, y + 0.5f, z + 0.5f);
                                    vertices[3].Set(x + 0.5f, y - 0.5f, z + 0.5f);
                                    meshData.AddVertices(vertices);

                                    //meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
                                    //meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                                    //meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                                    //meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));

                                    meshData.AddQuadTriangles();
                                    meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.East));

									UnityEngine.Profiling.Profiler.EndSample();
                                }
                            }

                            if (x != 0)
                            {
                                id = chunk.chunkData.blockIds[index - 1];
                                if ((id & 1 << 14) == 0)
                                {
                                    UnityEngine.Profiling.Profiler.BeginSample("Alloc west Face");

                                    Vector3[] vertices = new Vector3[4];
                                    //Vector3[] vertices = MemoryPoolManager.vector3ArrayPool.Rent(4);
                                    vertices[0].Set(x - 0.5f, y - 0.5f, z + 0.5f);
                                    vertices[1].Set(x - 0.5f, y + 0.5f, z + 0.5f);
                                    vertices[2].Set(x - 0.5f, y + 0.5f, z - 0.5f);
                                    vertices[3].Set(x - 0.5f, y - 0.5f, z - 0.5f);
                                    meshData.AddVertices(vertices);

                                    //meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
                                    //meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                                    //meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
                                    //meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));

                                    meshData.AddQuadTriangles();
                                    meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.West));

									UnityEngine.Profiling.Profiler.EndSample();
                                }
                            }
                            UnityEngine.Profiling.Profiler.EndSample();
                        }
                    }
                }
            }

            UnityEngine.Profiling.Profiler.BeginSample("Render Edges");
            if (min.x == 0)
            {
				int x = 0;
                ChunkInstance westChunk = chunk.world.GetChunk(chunk.position.x - ChunkInstance.CHUNK_SIZE, chunk.position.y, chunk.position.z);
                if(westChunk == null) 
                {
                    //for (int y = min.y, n2 = max.y; y <= n2; y++)
                    //{
                    //    for (int z = min.x, n3 = max.z; z <= n3; z++)
                    //    {
                    //        int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                    //        int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                    //        if ((chunk.chunkData.blockIds[index] & 1 << 14) > 0) //If I'm solid then lets look
                    //        {
                    //            Vector3[] vertices = new Vector3[4];
                    //            //Vector3[] vertices = MemoryPoolManager.vector3ArrayPool.Rent(4);
                    //            vertices[0].Set(x - 0.5f, y - 0.5f, z + 0.5f);
                    //            vertices[1].Set(x - 0.5f, y + 0.5f, z + 0.5f);
                    //            vertices[2].Set(x - 0.5f, y + 0.5f, z - 0.5f);
                    //            vertices[3].Set(x - 0.5f, y - 0.5f, z - 0.5f);
                    //            meshData.AddVertices(vertices);

                    //            //meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
                    //            //meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                    //            //meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
                    //            //meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));

                    //            meshData.AddQuadTriangles();
                    //            meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.West));
                    //        }
                    //    }
                    //}
                } else {
                    for (int y = min.y, n2 = max.y; y <= n2; y++)
                    {
                        for (int z = min.x, n3 = max.z; z <= n3; z++)
                        {
                            int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                            int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                            if ((chunk.chunkData.blockIds[index] & 1 << 14) > 0) //If I'm solid then lets look
                            {
                                ushort id = westChunk.chunkData.blockIds[index + ChunkInstance.CHUNK_SIZE - 1];
                                if ((id & 1 << 14) == 0)
                                {
                                    Vector3[] vertices = new Vector3[4];
                                    //Vector3[] vertices = MemoryPoolManager.vector3ArrayPool.Rent(4);
                                    vertices[0].Set(x - 0.5f, y - 0.5f, z + 0.5f);
                                    vertices[1].Set(x - 0.5f, y + 0.5f, z + 0.5f);
                                    vertices[2].Set(x - 0.5f, y + 0.5f, z - 0.5f);
                                    vertices[3].Set(x - 0.5f, y - 0.5f, z - 0.5f);
                                    meshData.AddVertices(vertices);

                                    //meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
                                    //meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                                    //meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
                                    //meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));

                                    meshData.AddQuadTriangles();
                                    meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.West));
                                }
                            }
                        }
                    }
                }
            }

            if (max.x == ChunkInstance.CHUNK_SIZE - 1)
            {
                int x = ChunkInstance.CHUNK_SIZE - 1;
                ChunkInstance eastChunk = chunk.world.GetChunk(chunk.position.x + ChunkInstance.CHUNK_SIZE, chunk.position.y, chunk.position.z);
                if (eastChunk == null)
                {
                    //for (int y = min.y, n2 = max.y; y <= n2; y++)
                    //{
                    //    for (int z = min.x, n3 = max.z; z <= n3; z++)
                    //    {
                    //        int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                    //        int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                    //        if (currentBlockID != 0) //If I'm solid then lets look
                    //        {
                    //            Vector3[] vertices = new Vector3[4];
                    //            //Vector3[] vertices = MemoryPoolManager.vector3ArrayPool.Rent(4);
                    //            vertices[0].Set(x + 0.5f, y - 0.5f, z - 0.5f);
                    //            vertices[1].Set(x + 0.5f, y + 0.5f, z - 0.5f);
                    //            vertices[2].Set(x + 0.5f, y + 0.5f, z + 0.5f);
                    //            vertices[3].Set(x + 0.5f, y - 0.5f, z + 0.5f);
                    //            meshData.AddVertices(vertices);

                    //            //meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
                    //            //meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                    //            //meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                    //            //meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));

                    //            meshData.AddQuadTriangles();
                    //            meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.East));
                    //        }
                    //    }
                    //}
                }
                else
                {
                    for (int y = min.y, n2 = max.y; y <= n2; y++)
                    {
                        for (int z = min.x, n3 = max.z; z <= n3; z++)
                        {
                            int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                            int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                            if (currentBlockID != 0) //If I'm solid then lets look
                            {
                                int newIndex = index - (ChunkInstance.CHUNK_SIZE - 1);
                                ushort id = eastChunk.chunkData.blockIds[newIndex];
                                if ((id & 1 << 14) == 0)
                                {
                                    Vector3[] vertices = new Vector3[4];
                                    //Vector3[] vertices = MemoryPoolManager.vector3ArrayPool.Rent(4);
                                    vertices[0].Set(x + 0.5f, y - 0.5f, z - 0.5f);
                                    vertices[1].Set(x + 0.5f, y + 0.5f, z - 0.5f);
                                    vertices[2].Set(x + 0.5f, y + 0.5f, z + 0.5f);
                                    vertices[3].Set(x + 0.5f, y - 0.5f, z + 0.5f);
                                    meshData.AddVertices(vertices);

                                    //meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
                                    //meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                                    //meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                                    //meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));

                                    meshData.AddQuadTriangles();
                                    meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.East));
                                }
                            }
                        }
                    }
                }
            }

            if (min.y == 0)
            {
                int y = 0;
                ChunkInstance downChunk = chunk.world.GetChunk(chunk.position.x, chunk.position.y - ChunkInstance.CHUNK_SIZE, chunk.position.z);
                if (downChunk == null)
                {
                    //for (int x = min.x, n2 = max.x; x <= n2; x++)
                    //{
                    //    for (int z = min.x, n3 = max.z; z <= n3; z++)
                    //    {
                    //        int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                    //        int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                    //        if (currentBlockID != 0) //If I'm solid then lets look
                    //        {
                    //            Vector3[] vertices = new Vector3[4];
                    //            //Vector3[] vertices = MemoryPoolManager.vector3ArrayPool.Rent(4);
                    //            vertices[0].Set(x - 0.5f, y - 0.5f, z - 0.5f);
                    //            vertices[1].Set(x + 0.5f, y - 0.5f, z - 0.5f);
                    //            vertices[2].Set(x + 0.5f, y - 0.5f, z + 0.5f);
                    //            vertices[3].Set(x - 0.5f, y - 0.5f, z + 0.5f);
                    //            meshData.AddVertices(vertices);

                    //            //meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
                    //            //meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
                    //            //meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
                    //            //meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

                    //            meshData.AddQuadTriangles();
                    //            meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.Down));
                    //        }
                    //    }
                    //}
                }
                else
                {
                    for (int x = min.x, n2 = max.x; x <= n2; x++)
                    {
                        for (int z = min.x, n3 = max.z; z <= n3; z++)
                        {
                            int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                            int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                            if (currentBlockID != 0) //If I'm solid then lets look
                            {
                                int newIndex = index + ((ChunkInstance.CHUNK_SIZE - 1) * ChunkInstance.CHUNK_SIZE);
                                ushort id = downChunk.chunkData.blockIds[newIndex];
                                if ((id & 1 << 14) == 0)
                                {
                                    Vector3[] vertices = new Vector3[4];
                                    //Vector3[] vertices = MemoryPoolManager.vector3ArrayPool.Rent(4);
                                    vertices[0].Set(x - 0.5f, y - 0.5f, z - 0.5f);
                                    vertices[1].Set(x + 0.5f, y - 0.5f, z - 0.5f);
                                    vertices[2].Set(x + 0.5f, y - 0.5f, z + 0.5f);
                                    vertices[3].Set(x - 0.5f, y - 0.5f, z + 0.5f);
                                    meshData.AddVertices(vertices);

                                    //meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
                                    //meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
                                    //meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
                                    //meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

                                    meshData.AddQuadTriangles();
                                    meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.Down));
                                }
                            }
                        }
                    }
                }
            }

            if (max.y == ChunkInstance.CHUNK_SIZE - 1)
            {
                int y = ChunkInstance.CHUNK_SIZE - 1;
                ChunkInstance downChunk = chunk.world.GetChunk(chunk.position.x, chunk.position.y + ChunkInstance.CHUNK_SIZE, chunk.position.z);
                if (downChunk == null)
                {
                    //for (int x = min.x, n2 = max.x; x <= n2; x++)
                    //{
                    //    for (int z = min.x, n3 = max.z; z <= n3; z++)
                    //    {
                    //        int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                    //        int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                    //        if (currentBlockID != 0) //If I'm solid then lets look
                    //        {
                    //            Vector3[] vertices = new Vector3[4];
                    //            //Vector3[] vertices = MemoryPoolManager.vector3ArrayPool.Rent(4);
                    //            vertices[0].Set(x - 0.5f, y + 0.5f, z + 0.5f);
                    //            vertices[1].Set(x + 0.5f, y + 0.5f, z + 0.5f);
                    //            vertices[2].Set(x + 0.5f, y + 0.5f, z - 0.5f);
                    //            vertices[3].Set(x - 0.5f, y + 0.5f, z - 0.5f);
                    //            meshData.AddVertices(vertices);

                    //            //meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                    //            //meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                    //            //meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                    //            //meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));

                    //            meshData.AddQuadTriangles();
                    //            meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.Up));
                    //        }
                    //    }
                    //}
                }
                else
                {
                    for (int x = min.x, n2 = max.x; x <= n2; x++)
                    {
                        for (int z = min.x, n3 = max.z; z <= n3; z++)
                        {
                            int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                            int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                            if (currentBlockID != 0) //If I'm solid then lets look
                            {
                                int newIndex = index - ((ChunkInstance.CHUNK_SIZE - 1) * ChunkInstance.CHUNK_SIZE);
                                ushort id = downChunk.chunkData.blockIds[newIndex];
                                if ((id & 1 << 14) == 0)
                                {
                                    Vector3[] vertices = new Vector3[4];
                                    //Vector3[] vertices = MemoryPoolManager.vector3ArrayPool.Rent(4);
                                    vertices[0].Set(x - 0.5f, y + 0.5f, z + 0.5f);
                                    vertices[1].Set(x + 0.5f, y + 0.5f, z + 0.5f);
                                    vertices[2].Set(x + 0.5f, y + 0.5f, z - 0.5f);
                                    vertices[3].Set(x - 0.5f, y + 0.5f, z - 0.5f);
                                    meshData.AddVertices(vertices);

                                    //meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                                    //meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                                    //meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                                    //meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));

                                    meshData.AddQuadTriangles();
                                    meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.Up));
                                }
                            }
                        }
                    }
                }
            }

            if (min.z == 0)
            {
                int z = 0;
                ChunkInstance southChunk = chunk.world.GetChunk(chunk.position.x, chunk.position.y, chunk.position.z - ChunkInstance.CHUNK_SIZE);
                if (southChunk == null)
                {
                    //for (int x = min.x, n2 = max.x; x <= n2; x++)
                    //{
                    //    for (int y = min.y, n3 = max.y; y <= n3; y++)
                    //    {
                    //        int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                    //        int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                    //        if (currentBlockID != 0) //If I'm solid then lets look
                    //        {
                    //            Vector3[] vertices = new Vector3[4];
                    //            //Vector3[] vertices = MemoryPoolManager.vector3ArrayPool.Rent(4);
                    //            vertices[0].Set(x - 0.5f, y - 0.5f, z - 0.5f);
                    //            vertices[1].Set(x - 0.5f, y + 0.5f, z - 0.5f);
                    //            vertices[2].Set(x + 0.5f, y + 0.5f, z - 0.5f);
                    //            vertices[3].Set(x + 0.5f, y - 0.5f, z - 0.5f);
                    //            meshData.AddVertices(vertices);

                    //            //meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
                    //            //meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
                    //            //meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                    //            //meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));

                    //            meshData.AddQuadTriangles();
                    //            meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.South));
                    //        }
                    //    }
                    //}
                }
                else
                {
                    for (int x = min.x, n2 = max.x; x <= n2; x++)
                    {
                        for (int y = min.y, n3 = max.y; y <= n3; y++)
                        {
                            int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                            int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                            if (currentBlockID != 0) //If I'm solid then lets look
                            {
                                ushort id = southChunk.chunkData.blockIds[index + (ChunkInstance.CHUNK_SIZE - 1) * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE];
                                if ((id & 1 << 14) == 0)
                                {
                                    Vector3[] vertices = new Vector3[4];
                                    //Vector3[] vertices = MemoryPoolManager.vector3ArrayPool.Rent(4);
                                    vertices[0].Set(x - 0.5f, y - 0.5f, z - 0.5f);
                                    vertices[1].Set(x - 0.5f, y + 0.5f, z - 0.5f);
                                    vertices[2].Set(x + 0.5f, y + 0.5f, z - 0.5f);
                                    vertices[3].Set(x + 0.5f, y - 0.5f, z - 0.5f);
                                    meshData.AddVertices(vertices);

                                    //meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
                                    //meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
                                    //meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                                    //meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));

                                    meshData.AddQuadTriangles();
                                    meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.South));
                                }
                            }
                        }
                    }
                }
            }

            if (max.z == ChunkInstance.CHUNK_SIZE - 1)
            {
                int z = ChunkInstance.CHUNK_SIZE - 1;
                ChunkInstance northChunk = chunk.world.GetChunk(chunk.position.x, chunk.position.y, chunk.position.z + ChunkInstance.CHUNK_SIZE);
                if (northChunk == null)
                {
                    //for (int x = min.x, n2 = max.x; x <= n2; x++)
                    //{
                    //    for (int y = min.y, n3 = max.y; y <= n3; y++)
                    //    {
                    //        int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                    //        int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                    //        if (currentBlockID != 0) //If I'm solid then lets look
                    //        {
                    //            Vector3[] vertices = new Vector3[4];
                    //            //Vector3[] vertices = MemoryPoolManager.vector3ArrayPool.Rent(4);
                    //            vertices[0].Set(x + 0.5f, y - 0.5f, z + 0.5f);
                    //            vertices[1].Set(x + 0.5f, y + 0.5f, z + 0.5f);
                    //            vertices[2].Set(x - 0.5f, y + 0.5f, z + 0.5f);
                    //            vertices[3].Set(x - 0.5f, y - 0.5f, z + 0.5f);
                    //            meshData.AddVertices(vertices);

                    //            //meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
                    //            //meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                    //            //meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                    //            //meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

                    //            meshData.AddQuadTriangles();
                    //            meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.North));
                    //        }
                    //    }
                    //}
                }
                else
                {
                    for (int x = min.x, n2 = max.x; x <= n2; x++)
                    {
                        for (int y = min.y, n3 = max.y; y <= n3; y++)
                        {
                            int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                            int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                            if (currentBlockID != 0) //If I'm solid then lets look
                            {
                                ushort id = northChunk.chunkData.blockIds[index - (ChunkInstance.CHUNK_SIZE - 1) * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE];
                                if ((id & 1 << 14) == 0)
                                {
                                    Vector3[] vertices = new Vector3[4];
                                    //Vector3[] vertices = MemoryPoolManager.vector3ArrayPool.Rent(4);
                                    vertices[0].Set(x + 0.5f, y - 0.5f, z + 0.5f);
                                    vertices[1].Set(x + 0.5f, y + 0.5f, z + 0.5f);
                                    vertices[2].Set(x - 0.5f, y + 0.5f, z + 0.5f);
                                    vertices[3].Set(x - 0.5f, y - 0.5f, z + 0.5f);
                                    meshData.AddVertices(vertices);

                                    //meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
                                    //meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                                    //meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                                    //meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

                                    meshData.AddQuadTriangles();
                                    meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.North));
                                }
                            }
                        }
                    }
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }

        //Just check on if its lists to see if its still empty.
        if (meshData.triangles.Count > 0)
            meshData.isClear = false;
        
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("RenderMesh");
        RenderMesh();
        UnityEngine.Profiling.Profiler.EndSample();
    }

    //void GenerateSamples(uint dim, uint[] samples, float size)
    //{
    //    //TODO sampler
    //    //assert(sampler.value != nullptr);

    //    uint z_per_y_chunks = ((dim + 31)) / 32;
    //    uint y_per_x_chunks = z_per_y_chunks * dim;
    //    uint z_per_y = dim;
    //    uint y_per_x = z_per_y * dim;
    //    bool positive = false, negative = false;
    //    uint count = (y_per_x * dim);
    //    uint real_count = ((z_per_y_chunks * 32) * dim * dim + 31) / 32;
    //    const float scale = 1.0f;

    //    samples = new uint[real_count];
    //    //samples = (uint32_t*)malloc(sizeof(uint32_t) * real_count);
    //    //memset(samples, 0, sizeof(uint32_t) * real_count);
    //    float delta = size / (float)dim;
    //    const float res = sampler.world_size;

    //    float[] block_data;
    //    FastNoiseVectorSet vector_set; //TODO FastNoiseVectorSet vector_set stuff
    //    sampler.block(res, pos, new WorldPos(dim, dim, dim), delta * scale, block_data, &vector_set);

    //    Vector3 dxyz;
    //    var f = sampler.value; //Not sure if var is same as auto
    //    //auto f = sampler.value;
    //    uint s0_mask, s0;
    //    uint z_count = (dim + 32) / 32;
    //    bool mesh = false;
    //    for (uint x = 0; x < dim; x++)
    //    {
    //        for (uint y = 0; y < dim; y++)
    //        {
    //            for (uint z_block = 0; z_block < z_count; z_block++)
    //            {
    //                float* block_samples = block_data + x * y_per_x + y * z_per_y + z_block * 32;
    //                uint m = 0;
    //                uint z_max = dim - z_block * 32;
    //                if (z_max > 32)
    //                    z_max = 32;

    //                for (uint z = 0; z < z_max; z++)
    //                {
    //                    float s = block_samples[z];
    //                    if (block_samples[z] < 0.0f)
    //                        m |= (uint)(1 << (int)z);
    //                }
    //                if (m != 0)
    //                {
    //                    samples[x * y_per_x_chunks + y * z_per_y_chunks + z_block] = m;
    //                    if (m != 0xFFFFFFFF)
    //                        mesh = true;
    //                    else if (!mesh)
    //                        negative = true;
    //                }
    //                else
    //                {
    //                    if (!mesh)
    //                        positive = true;
    //                    samples[x * y_per_x_chunks + y * z_per_y_chunks + z_block] = 0;
    //                }
    //            }
    //        }
    //    }

    //    if (!mesh)
    //        contains_mesh = negative && positive;
    //    else
    //        contains_mesh = mesh;

    //    //_aligned_free(block_data);
    //}



    //void GenerateNeighborInfo(uint dim)
    //{
    //    uint[] inds;
    //    ulong[] /*__restrict*/ masks; //IDK what restrict does
    //    uint[] samples;

    //    //TODO
    //    //if (!contains_mesh)
    //        //return;

    //    uint dimm1 = dim - 1;
    //    uint count = dim * dim * dim;
    //    uint z_per_y = ((dim + 31)) / 32;
    //    uint y_per_x = z_per_y * dim;

    //    uint z_per_y8 = (dimm1 + 8) / 8;
    //    uint y_per_x8 = z_per_y8 * dim;
    //    uint count8 = z_per_y8 * y_per_x8 * dim;

    //    inds = new uint[count];
    //    //inds = (uint32_t*)malloc(sizeof(uint32_t) * count);
    //    for (int i = 0; i < inds.Length; i++)
    //        inds[i] = 0xFFFFFFFF; //TODO neccesary to init them at Max Value?
    //    //memset(inds, 0xFFFFFFFF, sizeof(uint32_t) * count);

    //    masks = new ulong[count8];
    //    //masks = (uint64_t*)malloc(sizeof(uint64_t) * count8);

    //    uint z_count = (dim + 31) / 32;

    //    for (uint x = 0; x < dimm1; x++)
    //    {
    //        for (uint y = 0; y < dimm1; y++)
    //        {
    //            const int m = 0;
    //            for (uint z_block = 0; z_block < z_count; z_block++)
    //            {
    //                //TODO
    //                uint line = samples[x * y_per_x + y * z_per_y + z_block];

    //                uint line_masks_index = x * y_per_x8 + y * z_per_y8 + z_block * 4;
    //                //uint64_t* __restrict line_masks = masks + x * y_per_x8 + y * z_per_y8 + z_block * 4;
    //                if (line == 0)
    //                {
    //                    masks[line_masks_index] = 0;
    //                    masks[line_masks_index + 1] = 0;
    //                    masks[line_masks_index + 2] = 0;
    //                    masks[line_masks_index + 3] = 0;
    //                    continue;
    //                }
    //                ulong[] local_masks = new ulong[4]{ 0, 0, 0, 0 };
    //                EDGE_LINE(0, 0, 0x1, line, z_block, z_count, m, local_masks, line_masks_index);
    //                EDGE_LINE(0, 1, 0x2, line, z_block, z_count, m, local_masks, line_masks_index);
    //                if (z_block * 32 + 1 < dim)
    //                {
    //                    EDGE_LINE(0, 2, 0x4,        line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(0, 3, 0x8,        line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(0, 4, 0x10,       line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(0, 5, 0x20,       line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(0, 6, 0x40,       line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(0, 7, 0x80,       line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 0, 0x100,      line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 1, 0x200,      line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 2, 0x400,      line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 3, 0x800,      line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 4, 0x1000,     line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 5, 0x2000,     line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 6, 0x4000,     line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 7, 0x8000,     line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 0, 0x10000,    line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 1, 0x20000,    line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 2, 0x40000,    line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 3, 0x80000,    line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 4, 0x100000,   line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 5, 0x200000,   line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 6, 0x400000,   line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 7, 0x800000,   line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 0, 0x1000000,  line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 1, 0x2000000,  line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 2, 0x4000000,  line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 3, 0x8000000,  line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 4, 0x10000000, line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 5, 0x20000000, line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 6, 0x40000000, line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 7, 0x80000000, line, z_block, z_count, m, local_masks, line_masks_index);
    //                }
    //                //if (local_masks[0] != 0)
    //                masks[line_masks_index] = local_masks[0];
    //                //if (local_masks[1] != 0)
    //                masks[line_masks_index + 1] = local_masks[1];
    //                //if (local_masks[2] != 0)
    //                masks[line_masks_index + 2] = local_masks[2];
    //                //if (local_masks[3] != 0)
    //                masks[line_masks_index + 3] = local_masks[3];
    //            }
    //        }
    //    }

    //    for (uint x = 0; x < dimm1; x++)
    //    {
    //        for (uint y = 0; y < dimm1; y++)
    //        {
    //            const int m = 2;
    //            for (uint z_block = 0; z_block < z_count; z_block++)
    //            {
    //                uint line = samples[x * y_per_x + (y + 1) * z_per_y + z_block];
    //                if (line == 0)
    //                    continue;
                    
    //                uint line_masks_index = x * y_per_x8 + y * z_per_y8 + z_block * 4;

    //                //uint64_t* __restrict line_masks = masks + x * y_per_x8 + y * z_per_y8 + z_block * 4;
    //                ulong[] local_masks = { 0, 0, 0, 0 };
    //                EDGE_LINE(0, 0, 0x1, line, z_block, z_count, m, local_masks, line_masks_index);
    //                EDGE_LINE(0, 1, 0x2, line, z_block, z_count, m, local_masks, line_masks_index);
    //                if (z_block * 32 + 1 < dim)
    //                {
    //                    EDGE_LINE(0, 2, 0x4,        line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(0, 3, 0x8,        line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(0, 4, 0x10,       line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(0, 5, 0x20,       line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(0, 6, 0x40,       line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(0, 7, 0x80,       line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 0, 0x100,      line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 1, 0x200,      line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 2, 0x400,      line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 3, 0x800,      line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 4, 0x1000,     line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 5, 0x2000,     line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 6, 0x4000,     line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 7, 0x8000,     line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 0, 0x10000,    line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 1, 0x20000,    line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 2, 0x40000,    line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 3, 0x80000,    line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 4, 0x100000,   line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 5, 0x200000,   line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 6, 0x400000,   line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 7, 0x800000,   line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 0, 0x1000000,  line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 1, 0x2000000,  line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 2, 0x4000000,  line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 3, 0x8000000,  line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 4, 0x10000000, line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 5, 0x20000000, line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 6, 0x40000000, line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 7, 0x80000000, line, z_block, z_count, m, local_masks, line_masks_index);
    //                }
    //                if (local_masks[0] != 0)
    //                    masks[line_masks_index] |= local_masks[0];
    //                if (local_masks[1] != 0)
    //                    masks[line_masks_index + 1] |= local_masks[1];
    //                if (local_masks[2] != 0)
    //                    masks[line_masks_index + 2] |= local_masks[2];
    //                if (local_masks[3] != 0)
    //                    masks[line_masks_index + 3] |= local_masks[3];
    //            }
    //        }
    //    }

    //    for (uint x = 0; x < dimm1; x++)
    //    {
    //        for (uint y = 0; y < dimm1; y++)
    //        {
    //            const int m = 4;
    //            for (uint z_block = 0; z_block < z_count; z_block++)
    //            {
    //                uint line = samples[(x + 1) * y_per_x + y * z_per_y + z_block];
    //                if (line == 0)
    //                    continue;
                    
    //                uint line_masks_index = x * y_per_x8 + y * z_per_y8 + z_block * 4;

    //                //uint64_t* __restrict line_masks = masks + x * y_per_x8 + y * z_per_y8 + z_block * 4;
    //                ulong[] local_masks = { 0, 0, 0, 0 };
    //                EDGE_LINE(0, 0, 0x1, line, z_block, z_count, m, local_masks, line_masks_index);
    //                EDGE_LINE(0, 1, 0x2, line, z_block, z_count, m, local_masks, line_masks_index);
    //                if (z_block * 32 + 1 < dim)
    //                {
    //                    EDGE_LINE(0, 2, 0x4,        line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(0, 3, 0x8,        line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(0, 4, 0x10,       line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(0, 5, 0x20,       line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(0, 6, 0x40,       line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(0, 7, 0x80,       line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 0, 0x100,      line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 1, 0x200,      line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 2, 0x400,      line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 3, 0x800,      line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 4, 0x1000,     line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 5, 0x2000,     line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 6, 0x4000,     line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 7, 0x8000,     line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 0, 0x10000,    line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 1, 0x20000,    line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 2, 0x40000,    line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 3, 0x80000,    line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 4, 0x100000,   line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 5, 0x200000,   line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 6, 0x400000,   line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 7, 0x800000,   line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 0, 0x1000000,  line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 1, 0x2000000,  line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 2, 0x4000000,  line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 3, 0x8000000,  line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 4, 0x10000000, line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 5, 0x20000000, line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 6, 0x40000000, line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 7, 0x80000000, line, z_block, z_count, m, local_masks, line_masks_index);
    //                }
    //                if (local_masks[0] != 0)
    //                    masks[line_masks_index] |= local_masks[0];
    //                if (local_masks[1] != 0)
    //                    masks[line_masks_index + 1] |= local_masks[1];
    //                if (local_masks[2] != 0)
    //                    masks[line_masks_index + 2] |= local_masks[2];
    //                if (local_masks[3] != 0)
    //                    masks[line_masks_index + 3] |= local_masks[3];
    //            }
    //        }
    //    }

    //    for (uint x = 0; x < dimm1; x++)
    //    {
    //        for (uint y = 0; y < dimm1; y++)
    //        {
    //            const int m = 6;
    //            for (uint z_block = 0; z_block < z_count; z_block++)
    //            {
    //                uint line = samples[(x + 1) * y_per_x + (y + 1) * z_per_y + z_block];
    //                if (line == 0)
    //                    continue;

    //                uint line_masks_index = x * y_per_x8 + y * z_per_y8 + z_block * 4;

    //                //uint64_t* __restrict line_masks = masks + x * y_per_x8 + y * z_per_y8 + z_block * 4;
    //                ulong[] local_masks = { 0, 0, 0, 0 };
    //                EDGE_LINE(0, 0, 0x1, line, z_block, z_count, m, local_masks, line_masks_index);
    //                EDGE_LINE(0, 1, 0x2, line, z_block, z_count, m, local_masks, line_masks_index);
    //                if (z_block * 32 + 1 < dim)
    //                {
    //                    EDGE_LINE(0, 2, 0x4,        line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(0, 3, 0x8,        line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(0, 4, 0x10,       line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(0, 5, 0x20,       line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(0, 6, 0x40,       line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(0, 7, 0x80,       line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 0, 0x100,      line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 1, 0x200,      line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 2, 0x400,      line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 3, 0x800,      line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 4, 0x1000,     line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 5, 0x2000,     line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 6, 0x4000,     line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(1, 7, 0x8000,     line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 0, 0x10000,    line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 1, 0x20000,    line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 2, 0x40000,    line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 3, 0x80000,    line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 4, 0x100000,   line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 5, 0x200000,   line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 6, 0x400000,   line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(2, 7, 0x800000,   line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 0, 0x1000000,  line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 1, 0x2000000,  line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 2, 0x4000000,  line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 3, 0x8000000,  line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 4, 0x10000000, line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 5, 0x20000000, line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 6, 0x40000000, line, z_block, z_count, m, local_masks, line_masks_index);
    //                    EDGE_LINE(3, 7, 0x80000000, line, z_block, z_count, m, local_masks, line_masks_index);
    //                }
    //                if (local_masks[0] != 0)
    //                    masks[line_masks_index] |= local_masks[0];
    //                if (local_masks[1] != 0)
    //                    masks[line_masks_index+1] |= local_masks[1];
    //                if (local_masks[2] != 0)
    //                    masks[line_masks_index+2] |= local_masks[2];
    //                if (local_masks[3] != 0)
    //                    masks[line_masks_index+3] |= local_masks[3];
    //            }
    //        }
    //    }
    //}

    //void GenerateMesh(uint dim, float size)
    //{
    //    uint dimm1 = dim - 1;
    //    uint count = dim * dim * dim;
    //    uint z_per_y = ((dim + 31)) / 32;
    //    uint y_per_x = z_per_y * dim;

    //    uint z_per_y8 = (dimm1 + 8) / 8;
    //    uint y_per_x8 = z_per_y8 * dim;
    //    uint count8 = z_per_y8 * y_per_x8 * dim;

    //    float delta = size / (float)dim;

    //    //vertices.scale = 4;//TODO
    //    //vertices.resize(16384);
    //    DualVertex temp;
    //    for (uint x = 0; x < dim - 1; x++)
    //    {
    //        for (uint y = 0; y < dim - 1; y++)
    //        {
    //            for (uint z = 0; z < dim - 1; z += 8)
    //            {
    //                ulong mask = masks[x * y_per_x8 + y * z_per_y8 + z / 8];
    //                if (mask != 0 && mask != 0xFFFFFFFFFFFFFFFF)
    //                {
    //                    for (int sub_z = 0; sub_z < 8 && z + sub_z < dim - 1; sub_z++)
    //                    {
    //                        uint index = (uint)(x * dim * dim + y * dim + z + sub_z);
    //                        ushort sub_mask = (ushort)(mask & 0xFF);
    //                        if (sub_mask != 0 && sub_mask != 255)
    //                        {
    //                            //calculate_vertex(uvec3(x, y, z + sub_z), (uint32_t)vertices.count, &temp, false, sub_mask, vec3(0,0,0));
    //                            //inds[index] = (uint32_t)vertices.count;
    //                            //vertices.push_back(temp);
    //                            CalculateCell(x, y, z + sub_z, index, sub_mask, pos, delta, z_per_y, y_per_x);
    //                        }
    //                        mask >>= 8;
    //                    }
    //                }
    //                else
    //                {
    //                }
    //            }
    //        }
    //    }
    //    vertices.shrink();
    //}

    //void CalculateCell(uint x, uint y, uint z, uint index, ushort mask, Vector3 start, float delta, int z_per_y, int z_per_x, uint dim, uint[] inds)
    //{
    //    bool solid = (mask & 1) > 0;
    //    bool solid_right = ((mask >> 4) & 1) > 0;
    //    bool solid_up = ((mask >> 2) & 1) > 0;
    //    bool solid_forward = ((mask >> 1) & 1) > 0;
    //    if (solid == solid_right && solid_right == solid_up && solid_up == solid_forward)
    //        return;

    //    float dx = start.x + (float)x * delta;
    //    float dy = start.y + (float)y * delta;
    //    float dz = start.z + (float)z * delta;

    //    uint c, x1y0z0 = 0, x0y1z0 = 0, x1y1z0 = 0, x1y1z1 = 0, x1y0z1 = 0, x0y0z1 = 0, x0y1z1 = 0;
    //    uint out_mask;

    //    if (solid != solid_right)
    //    {
    //        c = EncodeCell(x + 1, y, z, dim);
    //        x1y0z0 = inds[c];
    //        if ((int)x1y0z0 < 0)
    //            x1y0z0 = CreateVertex(c, new WorldPos(x + 1, y, z), dx + delta, dy, dz, inds);
    //    }

    //    if (solid != solid_up)
    //    {
    //        c = EncodeCell(x, y + 1, z, dim);
    //        x0y1z0 = inds[c];
    //        if ((int)x0y1z0 < 0)
    //            x0y1z0 = CreateVertex(c, new WorldPos(x, y + 1, z), dx, dy + delta, dz, inds);
    //    }

    //    if (solid != solid_right || solid != solid_up)
    //    {
    //        c = EncodeCell(x + 1, y + 1, z, dim);
    //        x1y1z0 = inds[c];
    //        if ((int)x1y1z0 < 0)
    //            x1y1z0 = CreateVertex(c, new WorldPos(x, y, z + 1), dx + delta, dy + delta, dz, inds);
    //    }

    //    c = EncodeCell(x + 1, y + 1, z + 1, dim);
    //    x1y1z1 = inds[c];
    //    if ((int)x1y1z1 < 0)
    //        x1y1z1 = CreateVertex(c, new WorldPos(x + 1, y + 1, z + 1), dx + delta, dy + delta, dz + delta, inds);

    //    if (solid != solid_right || solid != solid_forward)
    //    {
    //        c = EncodeCell(x + 1, y, z + 1, dim);
    //        x1y0z1 = inds[c];
    //        if ((int)x1y0z1 < 0)
    //            x1y0z1 = CreateVertex(c, new WorldPos(x + 1, y, z + 1), dx + delta, dy, dz + delta, inds);
    //    }

    //    if (solid != solid_forward)
    //    {
    //        c = EncodeCell(x, y, z + 1, dim);
    //        x0y0z1 = inds[c];
    //        if ((int)x0y0z1 < 0)
    //            x0y0z1 = CreateVertex(c, new WorldPos(x, y, z + 1), dx, dy, dz + delta, inds);
    //    }

    //    if (solid != solid_up || solid != solid_forward)
    //    {
    //        c = EncodeCell(x, y + 1, z + 1, dim);
    //        x0y1z1 = inds[c];
    //        if ((int)x0y1z1 < 0)
    //            x0y1z1 = CreateVertex(c, new WorldPos(x, y + 1, z + 1), dx, dy + delta, dz + delta, inds);
    //    }

    //    // Right face
    //    if (solid && !solid_right)
    //    {
    //        mesh_indexes.push_back(x1y0z0);
    //        mesh_indexes.push_back(x1y1z0);
    //        mesh_indexes.push_back(x1y1z1);
    //        mesh_indexes.push_back(x1y0z1);

    //        vertices[x1y0z0].init_valence++;
    //        vertices[x1y1z0].init_valence++;
    //        vertices[x1y1z1].init_valence++;
    //        vertices[x1y0z1].init_valence++;
    //    }
    //    else if (!solid && solid_right)
    //    {
    //        mesh_indexes.push_back(x1y1z1);
    //        mesh_indexes.push_back(x1y1z0);
    //        mesh_indexes.push_back(x1y0z0);
    //        mesh_indexes.push_back(x1y0z1);

    //        vertices[x1y1z1].init_valence++;
    //        vertices[x1y1z0].init_valence++;
    //        vertices[x1y0z0].init_valence++;
    //        vertices[x1y0z1].init_valence++;
    //    }

    //    // Top face
    //    if (!solid && solid_up)
    //    {
    //        mesh_indexes.push_back(x0y1z0);
    //        mesh_indexes.push_back(x1y1z0);
    //        mesh_indexes.push_back(x1y1z1);
    //        mesh_indexes.push_back(x0y1z1);

    //        vertices[x0y1z0].init_valence++;
    //        vertices[x1y1z0].init_valence++;
    //        vertices[x1y1z1].init_valence++;
    //        vertices[x0y1z1].init_valence++;
    //    }
    //    else if (solid && !solid_up)
    //    {
    //        mesh_indexes.push_back(x1y1z1);
    //        mesh_indexes.push_back(x1y1z0);
    //        mesh_indexes.push_back(x0y1z0);
    //        mesh_indexes.push_back(x0y1z1);

    //        vertices[x1y1z1].init_valence++;
    //        vertices[x1y1z0].init_valence++;
    //        vertices[x0y1z0].init_valence++;
    //        vertices[x0y1z1].init_valence++;
    //    }

    //    // Front face
    //    if (solid && !solid_forward)
    //    {
    //        mesh_indexes.push_back(x0y0z1);
    //        mesh_indexes.push_back(x1y0z1);
    //        mesh_indexes.push_back(x1y1z1);
    //        mesh_indexes.push_back(x0y1z1);

    //        vertices[x0y0z1].init_valence++;
    //        vertices[x1y0z1].init_valence++;
    //        vertices[x1y1z1].init_valence++;
    //        vertices[x0y1z1].init_valence++;
    //    }
    //    else if (!solid && solid_forward)
    //    {
    //        mesh_indexes.push_back(x1y1z1);
    //        mesh_indexes.push_back(x1y0z1);
    //        mesh_indexes.push_back(x0y0z1);
    //        mesh_indexes.push_back(x0y1z1);

    //        vertices[x1y1z1].init_valence++;
    //        vertices[x1y0z1].init_valence++;
    //        vertices[x0y0z1].init_valence++;
    //        vertices[x0y1z1].init_valence++;
    //    }
    //}

    //uint EncodeCell(uint x, uint y, uint z, uint dim)
    //{
    //    return x * dim * dim + y * dim + z;
    //}

    //uint CreateVertex(uint cell_index, WorldPos xyz, float x, float y, float z, uint[] inds)
    //{
    //    //DualVertex dv;
    //    //calculate_vertex((uint32_t)vertices.count, &dv, xyz, x, y, z, 0);
    //    //vertices.push_back(dv);
    //    //inds[cell_index] = dv.index;
    //    //return dv.index;

    //    //TODO
    //    return 0u;
    //}

    //void EDGE_LINE(int l_z, int z, uint b, uint line, uint z_block, uint z_count, int m, ulong[] local_masks, uint line_masks_index)
    //{
    //    if ((line & b) > 0)
    //    {
    //        if (z_block < z_count - 1 || l_z * 8 + z < 31)
    //        {
    //            ulong mask = 1ul << (int)(z * 8u + m);
    //            local_masks[l_z] |= mask;
    //            //local_masks[l_z] |= 1ull << (z * 8 + m);
    //        }
    //        if (z > 0)
    //        {
    //            ulong mask = 1ul << (int)((z - 1) * 8 + m + 1);
    //            local_masks[l_z] |= mask;
    //            //local_masks[l_z] |= 1ull << ((z - 1) * 8 + m + 1);
    //        }
    //        else if (l_z > 0)
    //        {
    //            ulong mask = 1ul << (7 * 8 + m + 1);
    //            local_masks[l_z - 1] |= mask;
    //            //local_masks[l_z - 1] |= 1ul << (7 * 8 + m + 1);
    //        }
    //        else if (z_block > 0)
    //        {
    //            ulong mask = 1ul << (7 * 8 + m + 1);
    //            local_masks[line_masks_index - 1] |= mask;
    //            //line_masks[-1] |= 1ull << (7 * 8 + m + 1);
    //        }
    //    }
    //}

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

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        if(meshData.useRenderDataForCol)
			coll.sharedMesh = mesh;
        filter.sharedMesh = mesh;
    }
}
