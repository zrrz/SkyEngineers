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
                    for (int z = min.x, n3 = max.z; z <= n3; z++)
                    {
                        int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                        int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                        if ((chunk.chunkData.blockIds[index] & 1 << 14) > 0) //If I'm solid then lets look
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
                                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));

                                    meshData.AddQuadTriangles();
                                    meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.Up));
                                }
                            }

                            if (y != 0)
                            {
                                id = chunk.chunkData.blockIds[index - ChunkInstance.CHUNK_SIZE];
                                if ((id & 1 << 14) == 0)
                                {
                                    UnityEngine.Profiling.Profiler.BeginSample("Alloc down Face");
                                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

                                    meshData.AddQuadTriangles();
                                    meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.Down));
                                    UnityEngine.Profiling.Profiler.EndSample();
                                }
                            }

                            if (z != ChunkInstance.CHUNK_SIZE - 1)
                            {
                                id = chunk.chunkData.blockIds[index + ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE];
                                if ((id & 1 << 14) == 0)
                                {
                                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

                                    meshData.AddQuadTriangles();
                                    meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.North));
                                }
                            }

                            if (z != 0)
                            {
                                id = chunk.chunkData.blockIds[index - ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE];
                                if ((id & 1 << 14) == 0)
                                {
                                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));

                                    meshData.AddQuadTriangles();
                                    meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.South));
                                }
                            }

                            if (x != ChunkInstance.CHUNK_SIZE - 1)
                            {
                                id = chunk.chunkData.blockIds[index + 1];
                                if ((id & 1 << 14) == 0)
                                {
                                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));

                                    meshData.AddQuadTriangles();
                                    meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.East));
                                }
                            }

                            if (x != 0)
                            {
                                id = chunk.chunkData.blockIds[index - 1];
                                if ((id & 1 << 14) == 0)
                                {
                                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));

                                    meshData.AddQuadTriangles();
                                    meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.West));
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
                if(westChunk == null) {
                    for (int y = min.y, n2 = max.y; y <= n2; y++)
                    {
                        for (int z = min.x, n3 = max.z; z <= n3; z++)
                        {
                            int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                            int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                            if ((chunk.chunkData.blockIds[index] & 1 << 14) > 0) //If I'm solid then lets look
                            {
                                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
                                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
                                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));

                                meshData.AddQuadTriangles();
                                meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.West));
                            }
                        }
                    }
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
                                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));

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
                    for (int y = min.y, n2 = max.y; y <= n2; y++)
                    {
                        for (int z = min.x, n3 = max.z; z <= n3; z++)
                        {
                            int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                            int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                            if ((chunk.chunkData.blockIds[index] & 1 << 14) > 0) //If I'm solid then lets look
                            {
                                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
                                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));

                                meshData.AddQuadTriangles();
                                meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.East));
                            }
                        }
                    }
                }
                else
                {
                    for (int y = min.y, n2 = max.y; y <= n2; y++)
                    {
                        for (int z = min.x, n3 = max.z; z <= n3; z++)
                        {
                            int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                            int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                            if ((chunk.chunkData.blockIds[index] & 1 << 14) > 0) //If I'm solid then lets look
                            {
                                int newIndex = index - (ChunkInstance.CHUNK_SIZE - 1);
                                ushort id = eastChunk.chunkData.blockIds[newIndex];
                                if ((id & 1 << 14) == 0)
                                {
                                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));

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
                    for (int x = min.x, n2 = max.x; x <= n2; x++)
                    {
                        for (int z = min.x, n3 = max.z; z <= n3; z++)
                        {
                            int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                            int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                            if ((chunk.chunkData.blockIds[index] & 1 << 14) > 0) //If I'm solid then lets look
                            {
                                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
                                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
                                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
                                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

                                meshData.AddQuadTriangles();
                                meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.Down));
                            }
                        }
                    }
                }
                else
                {
                    for (int x = min.x, n2 = max.x; x <= n2; x++)
                    {
                        for (int z = min.x, n3 = max.z; z <= n3; z++)
                        {
                            int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                            int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                            if ((chunk.chunkData.blockIds[index] & 1 << 14) > 0) //If I'm solid then lets look
                            {
                                int newIndex = index + ((ChunkInstance.CHUNK_SIZE - 1) * ChunkInstance.CHUNK_SIZE);
                                ushort id = downChunk.chunkData.blockIds[newIndex];
                                if ((id & 1 << 14) == 0)
                                {
                                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

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
                    for (int x = min.x, n2 = max.x; x <= n2; x++)
                    {
                        for (int z = min.x, n3 = max.z; z <= n3; z++)
                        {
                            int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                            int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                            if ((chunk.chunkData.blockIds[index] & 1 << 14) > 0) //If I'm solid then lets look
                            {
                                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));

                                meshData.AddQuadTriangles();
                                meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.Up));
                            }
                        }
                    }
                }
                else
                {
                    for (int x = min.x, n2 = max.x; x <= n2; x++)
                    {
                        for (int z = min.x, n3 = max.z; z <= n3; z++)
                        {
                            int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                            int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                            if ((chunk.chunkData.blockIds[index] & 1 << 14) > 0) //If I'm solid then lets look
                            {
                                int newIndex = index - ((ChunkInstance.CHUNK_SIZE - 1) * ChunkInstance.CHUNK_SIZE);
                                ushort id = downChunk.chunkData.blockIds[newIndex];
                                if ((id & 1 << 14) == 0)
                                {
                                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));

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
                    for (int x = min.x, n2 = max.x; x <= n2; x++)
                    {
                        for (int y = min.y, n3 = max.y; y <= n3; y++)
                        {
                            int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                            int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                            if ((chunk.chunkData.blockIds[index] & 1 << 14) > 0) //If I'm solid then lets look
                            {
                                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
                                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
                                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));

                                meshData.AddQuadTriangles();
                                meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.South));
                            }
                        }
                    }
                }
                else
                {
                    for (int x = min.x, n2 = max.x; x <= n2; x++)
                    {
                        for (int y = min.y, n3 = max.y; y <= n3; y++)
                        {
                            int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                            int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                            if ((chunk.chunkData.blockIds[index] & 1 << 14) > 0) //If I'm solid then lets look
                            {
                                ushort id = southChunk.chunkData.blockIds[index + (ChunkInstance.CHUNK_SIZE - 1) * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE];
                                if ((id & 1 << 14) == 0)
                                {
                                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));

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
                    for (int x = min.x, n2 = max.x; x <= n2; x++)
                    {
                        for (int y = min.y, n3 = max.y; y <= n3; y++)
                        {
                            int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                            int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                            if ((chunk.chunkData.blockIds[index] & 1 << 14) > 0) //If I'm solid then lets look
                            {
                                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
                                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

                                meshData.AddQuadTriangles();
                                meshData.AddUVs(BlockData.FaceUVs(currentBlockID, BlockData.Direction.North));
                            }
                        }
                    }
                }
                else
                {
                    for (int x = min.x, n2 = max.x; x <= n2; x++)
                    {
                        for (int y = min.y, n3 = max.y; y <= n3; y++)
                        {
                            int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                            int currentBlockID = chunk.chunkData.blockIds[index] & 0x3FFF;
                            if ((chunk.chunkData.blockIds[index] & 1 << 14) > 0) //If I'm solid then lets look
                            {
                                ushort id = northChunk.chunkData.blockIds[index - (ChunkInstance.CHUNK_SIZE - 1) * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE];
                                if ((id & 1 << 14) == 0)
                                {
                                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

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
