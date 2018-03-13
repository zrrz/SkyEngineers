using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RenderNearbyChunks : MonoBehaviour
{
//    static WorldPos[] chunkPositions = {   new WorldPos( 0, 0,  0), new WorldPos(-1, 0,  0), new WorldPos( 0, 0, -1), new WorldPos( 0, 0,  1), new WorldPos( 1, 0,  0),
//                             new WorldPos(-1, 0, -1), new WorldPos(-1, 0,  1), new WorldPos( 1, 0, -1), new WorldPos( 1, 0,  1), new WorldPos(-2, 0,  0),
//                             new WorldPos( 0, 0, -2), new WorldPos( 0, 0,  2), new WorldPos( 2, 0,  0), new WorldPos(-2, 0, -1), new WorldPos(-2, 0,  1),
//                             new WorldPos(-1, 0, -2), new WorldPos(-1, 0,  2), new WorldPos( 1, 0, -2), new WorldPos( 1, 0,  2), new WorldPos( 2, 0, -1),
//                             new WorldPos( 2, 0,  1), new WorldPos(-2, 0, -2), new WorldPos(-2, 0,  2), new WorldPos( 2, 0, -2), new WorldPos( 2, 0,  2),
//                             new WorldPos(-3, 0,  0), new WorldPos( 0, 0, -3), new WorldPos( 0, 0,  3), new WorldPos( 3, 0,  0), new WorldPos(-3, 0, -1),
//                             new WorldPos(-3, 0,  1), new WorldPos(-1, 0, -3), new WorldPos(-1, 0,  3), new WorldPos( 1, 0, -3), new WorldPos( 1, 0,  3),
//                             new WorldPos( 3, 0, -1), new WorldPos( 3, 0,  1), new WorldPos(-3, 0, -2), new WorldPos(-3, 0,  2), new WorldPos(-2, 0, -3),
//                             new WorldPos(-2, 0,  3), new WorldPos( 2, 0, -3), new WorldPos( 2, 0,  3), new WorldPos( 3, 0, -2), new WorldPos( 3, 0,  2),
//                             new WorldPos(-4, 0,  0), new WorldPos( 0, 0, -4), new WorldPos( 0, 0,  4), new WorldPos( 4, 0,  0), new WorldPos(-4, 0, -1),
//                             new WorldPos(-4, 0,  1), new WorldPos(-1, 0, -4), new WorldPos(-1, 0,  4), new WorldPos( 1, 0, -4), new WorldPos( 1, 0,  4),
//                             new WorldPos( 4, 0, -1), new WorldPos( 4, 0,  1), new WorldPos(-3, 0, -3), new WorldPos(-3, 0,  3), new WorldPos( 3, 0, -3),
//                             new WorldPos( 3, 0,  3), new WorldPos(-4, 0, -2), new WorldPos(-4, 0,  2), new WorldPos(-2, 0, -4), new WorldPos(-2, 0,  4),
//                             new WorldPos( 2, 0, -4), new WorldPos( 2, 0,  4), new WorldPos( 4, 0, -2), new WorldPos( 4, 0,  2), new WorldPos(-5, 0,  0),
//                             new WorldPos(-4, 0, -3), new WorldPos(-4, 0,  3), new WorldPos(-3, 0, -4), new WorldPos(-3, 0,  4), new WorldPos( 0, 0, -5),
//                             new WorldPos( 0, 0,  5), new WorldPos( 3, 0, -4), new WorldPos( 3, 0,  4), new WorldPos( 4, 0, -3), new WorldPos( 4, 0,  3),
//                             new WorldPos( 5, 0,  0), new WorldPos(-5, 0, -1), new WorldPos(-5, 0,  1), new WorldPos(-1, 0, -5), new WorldPos(-1, 0,  5),
//                             new WorldPos( 1, 0, -5), new WorldPos( 1, 0,  5), new WorldPos( 5, 0, -1), new WorldPos( 5, 0,  1), new WorldPos(-5, 0, -2),
//                             new WorldPos(-5, 0,  2), new WorldPos(-2, 0, -5), new WorldPos(-2, 0,  5), new WorldPos( 2, 0, -5), new WorldPos( 2, 0,  5),
//                             new WorldPos( 5, 0, -2), new WorldPos( 5, 0,  2), new WorldPos(-4, 0, -4), new WorldPos(-4, 0,  4), new WorldPos( 4, 0, -4),
//                             new WorldPos( 4, 0,  4), new WorldPos(-5, 0, -3), new WorldPos(-5, 0,  3), new WorldPos(-3, 0, -5), new WorldPos(-3, 0,  5),
//                             new WorldPos( 3, 0, -5), new WorldPos( 3, 0,  5), new WorldPos( 5, 0, -3), new WorldPos( 5, 0,  3), new WorldPos(-6, 0,  0),
//                             new WorldPos( 0, 0, -6), new WorldPos( 0, 0,  6), new WorldPos( 6, 0,  0), new WorldPos(-6, 0, -1), new WorldPos(-6, 0,  1),
//                             new WorldPos(-1, 0, -6), new WorldPos(-1, 0,  6), new WorldPos( 1, 0, -6), new WorldPos( 1, 0,  6), new WorldPos( 6, 0, -1),
//                             new WorldPos( 6, 0,  1), new WorldPos(-6, 0, -2), new WorldPos(-6, 0,  2), new WorldPos(-2, 0, -6), new WorldPos(-2, 0,  6),
//                             new WorldPos( 2, 0, -6), new WorldPos( 2, 0,  6), new WorldPos( 6, 0, -2), new WorldPos( 6, 0,  2), new WorldPos(-5, 0, -4),
//                             new WorldPos(-5, 0,  4), new WorldPos(-4, 0, -5), new WorldPos(-4, 0,  5), new WorldPos( 4, 0, -5), new WorldPos( 4, 0,  5),
//                             new WorldPos( 5, 0, -4), new WorldPos( 5, 0,  4), new WorldPos(-6, 0, -3), new WorldPos(-6, 0,  3), new WorldPos(-3, 0, -6),
//                             new WorldPos(-3, 0,  6), new WorldPos( 3, 0, -6), new WorldPos( 3, 0,  6), new WorldPos( 6, 0, -3), new WorldPos( 6, 0,  3),
//                             new WorldPos(-7, 0,  0), new WorldPos( 0, 0, -7), new WorldPos( 0, 0,  7), new WorldPos( 7, 0,  0), new WorldPos(-7, 0, -1),
//                             new WorldPos(-7, 0,  1), new WorldPos(-5, 0, -5), new WorldPos(-5, 0,  5), new WorldPos(-1, 0, -7), new WorldPos(-1, 0,  7),
//                             new WorldPos( 1, 0, -7), new WorldPos( 1, 0,  7), new WorldPos( 5, 0, -5), new WorldPos( 5, 0,  5), new WorldPos( 7, 0, -1),
//                             new WorldPos( 7, 0,  1), new WorldPos(-6, 0, -4), new WorldPos(-6, 0,  4), new WorldPos(-4, 0, -6), new WorldPos(-4, 0,  6),
//                             new WorldPos( 4, 0, -6), new WorldPos( 4, 0,  6), new WorldPos( 6, 0, -4), new WorldPos( 6, 0,  4), new WorldPos(-7, 0, -2),
//                             new WorldPos(-7, 0,  2), new WorldPos(-2, 0, -7), new WorldPos(-2, 0,  7), new WorldPos( 2, 0, -7), new WorldPos( 2, 0,  7),
//                             new WorldPos( 7, 0, -2), new WorldPos( 7, 0,  2), new WorldPos(-7, 0, -3), new WorldPos(-7, 0,  3), new WorldPos(-3, 0, -7),
//                             new WorldPos(-3, 0,  7), new WorldPos( 3, 0, -7), new WorldPos( 3, 0,  7), new WorldPos( 7, 0, -3), new WorldPos( 7, 0,  3),
//                             new WorldPos(-6, 0, -5), new WorldPos(-6, 0,  5), new WorldPos(-5, 0, -6), new WorldPos(-5, 0,  6), new WorldPos( 5, 0, -6),
//                             new WorldPos( 5, 0,  6), new WorldPos( 6, 0, -5), new WorldPos( 6, 0,  5) };

    public World world;
//
    List<WorldPos> updateList = new List<WorldPos>();
    List<WorldPos> buildList = new List<WorldPos>();

    int timer = 0;
    readonly Dictionary<WorldPos, ChunkRenderer> chunkRenderers = new Dictionary<WorldPos, ChunkRenderer>();

    [SerializeField]
    GameObject chunkPrefab;

    public int renderDistance = 4;

    // Update is called once per frame
    void Update()
    {
        if (DeleteChunkRenderers())
            return;

        FindChunksToLoad();
        LoadAndRenderChunks();
    }

    void FindChunksToLoad()
    {
        //Get the position of this gameobject to generate around
        WorldPos playerPos = new WorldPos(
            Mathf.FloorToInt(transform.position.x / ChunkInstance.CHUNK_SIZE) * ChunkInstance.CHUNK_SIZE,
            Mathf.FloorToInt(transform.position.y / ChunkInstance.CHUNK_SIZE) * ChunkInstance.CHUNK_SIZE,
            Mathf.FloorToInt(transform.position.z / ChunkInstance.CHUNK_SIZE) * ChunkInstance.CHUNK_SIZE
            );

        //If there aren't already chunks to generate
        if (updateList.Count == 0)
        {

            for (int x = -renderDistance; x < renderDistance; x++)
            {
                for (int y = -renderDistance; y < renderDistance; y++)
                {
                    for (int z = -renderDistance; z < renderDistance; z++)
                    {
                        WorldPos newChunkPos = new WorldPos(
                            x * ChunkInstance.CHUNK_SIZE + playerPos.x,
                            y * ChunkInstance.CHUNK_SIZE + playerPos.y,
                            z * ChunkInstance.CHUNK_SIZE + playerPos.z
                        );

                        //If no chunk data available, ignore.
                        if (world.GetChunk(newChunkPos.x, newChunkPos.y, newChunkPos.z) == null)
                        {
                            Debug.LogError("No world data at " + newChunkPos.ToString());
                            continue;
                        }

                        ChunkRenderer newChunk;
                        chunkRenderers.TryGetValue(newChunkPos, out newChunk);

                        //If the chunk already exists and it's already
                        //rendered or in queue to be rendered continue
                        if (newChunk != null
                            && (newChunk.rendered || updateList.Contains(newChunkPos)))
                            continue;


                        //Otherwise let's build it
                        buildList.Add(newChunkPos);
                        updateList.Add(newChunkPos);

//                        return; //This is what is throttling chunk loading

//                        //load a column of chunks in this position
//                        for (int y = -4; y < 4; y++) {
//                            for (int x = newChunkPos.x - Chunk.CHUNK_SIZE; x <= newChunkPos.x + Chunk.CHUNK_SIZE; x += Chunk.CHUNK_SIZE) {
//                                for (int z = newChunkPos.z - Chunk.CHUNK_SIZE; z <= newChunkPos.z + Chunk.CHUNK_SIZE; z += Chunk.CHUNK_SIZE) {
//                                    if (world.GetChunk(x, y * Chunk.CHUNK_SIZE, z) == null) {
//                                        //                                Debug.LogError("No chunk. Skipping");
//                                        //                                continue;
//                                    }
//                                    else
//                                    {
//                                        buildList.Add(new WorldPos(
//                                            x, y * Chunk.CHUNK_SIZE, z));
//                                    }
//                                    if (world.GetChunk(newChunkPos.x, y * Chunk.CHUNK_SIZE, newChunkPos.z) == null)
//                                    {
//                                        Debug.LogError("No chunk. Skipping");
//                                    }
//                                    else
//                                    {
//                                        updateList.Add(new WorldPos(
//                                            x, y * Chunk.CHUNK_SIZE, z));
//                                    }
//                                }
//                            }
//
//                        }

                    }
                }
            }
        }
    }

    void LoadAndRenderChunks()
    {
        if (buildList.Count != 0)
        {
            for (int i = 0; i < buildList.Count && i < 64; i++)
            {
                BuildChunkRenderer(buildList[0]);
                buildList.RemoveAt(0);
            }

            //If chunks were built return early
            return;
        }

        if (updateList.Count != 0)
        {
			for (int i = 0; i < updateList.Count && i < 64; i++)
			{
                WorldPos pos = updateList[0];
                ChunkRenderer chunkRenderer;
                if (chunkRenderers.TryGetValue(pos, out chunkRenderer))
                {
                    chunkRenderer.chunk.update = true;
                }
                    
            	updateList.RemoveAt(0);
			}
        }
    }

    void BuildChunkRenderer(WorldPos pos)
    {
        //Instantiate the chunk at the coordinates using the chunk prefab
        GameObject newChunkObject = Instantiate(
            chunkPrefab, new Vector3(pos.x, pos.y, pos.z),
            Quaternion.Euler(Vector3.zero)
        ) as GameObject;

        ChunkRenderer chunkRenderer = newChunkObject.GetComponent<ChunkRenderer>();
        chunkRenderer.chunk = world.GetChunk(pos.x, pos.y, pos.z);
        if (chunkRenderer.chunk == null)
        {
            Debug.LogError("No chunk at " + pos.x + " " + pos.y + " " + pos.z);
        }
        chunkRenderers.Add(pos, newChunkObject.GetComponent<ChunkRenderer>());
        chunkRenderer.rendered = true;
    }

    bool DeleteChunkRenderers()
    {
        if (timer == 10)
        {
            List<WorldPos> chunkRenderersToDelete = new List<WorldPos>();
            foreach (var chunkRenderer in chunkRenderers)
            {
                float distance = Vector3.Distance(
                    new Vector3(chunkRenderer.Value.chunk.position.x, chunkRenderer.Value.chunk.position.y, chunkRenderer.Value.chunk.position.z),
                    new Vector3(transform.position.x, transform.position.y, transform.position.z));

                if (distance > (Mathf.CeilToInt(1.74f * (renderDistance)) + 1) * ChunkInstance.CHUNK_SIZE)
                {
                    chunkRenderersToDelete.Add(chunkRenderer.Key);
                }
            }

            foreach (WorldPos chunkKeys in chunkRenderersToDelete) {
                Destroy(chunkRenderers[chunkKeys].gameObject);
                chunkRenderers.Remove(chunkKeys);
            }

            timer = 0;
            return true;
        }

        timer++;
        return false;
    }
}