using UnityEngine;
using System.Collections;

public static class EditTerrain
{
    public static WorldPos GetBlockPos(Vector3 pos)
    {
        WorldPos blockPos = new WorldPos(
            Mathf.RoundToInt(pos.x),
            Mathf.RoundToInt(pos.y),
            Mathf.RoundToInt(pos.z)
            );

        return blockPos;
    }

    public static WorldPos GetBlockPos(RaycastHit hit, bool adjacent = false)
    {
        Vector3 pos = new Vector3(
            MoveWithinBlock(hit.point.x, hit.normal.x, adjacent),
            MoveWithinBlock(hit.point.y, hit.normal.y, adjacent),
            MoveWithinBlock(hit.point.z, hit.normal.z, adjacent)
            );

        return GetBlockPos(pos);
    }

    static float MoveWithinBlock(float pos, float norm, bool adjacent = false)
    {
        if (pos - (int)pos == 0.5f || pos - (int)pos == -0.5f)
        {
            if (adjacent)
            {
                pos += (norm / 2);
            }
            else
            {
                pos -= (norm / 2);
            }
        }

        return pos;
    }

	public static bool BreakBlock(RaycastHit hit, bool adjacent = false) {
        ChunkRenderer chunkRenderer = hit.collider.GetComponent<ChunkRenderer>();
		if (chunkRenderer == null)
			return false;

		WorldPos pos = GetBlockPos(hit, adjacent);

        ushort blockID = chunkRenderer.chunk.world.GetBlock(pos.x, pos.y, pos.z) ;
        BlockLoader.GetBlock(blockID).Break(new Vector3(pos.x, pos.y, pos.z));
        chunkRenderer.chunk.world.SetBlockID(pos.x, pos.y, pos.z, 0);

		return true;
	}

    public static bool PlaceBlock(RaycastHit hit, ushort blockID, bool adjacent = false) {
        ChunkRenderer chunkRenderer = hit.collider.GetComponent<ChunkRenderer>();
        if (chunkRenderer == null)
            return false;

//        hit.point += hit.normal;
        WorldPos pos = GetBlockPos(hit, adjacent);

        chunkRenderer.chunk.world.SetBlockID(pos.x, pos.y, pos.z, blockID);

        return true;
    }

    public static bool SetBlock(RaycastHit hit, ushort blockID, BlockData blockData, bool adjacent = false)
    {
        ChunkInstance chunk = hit.collider.GetComponent<ChunkInstance>();
        if (chunk == null)
            return false;

        WorldPos pos = GetBlockPos(hit, adjacent);

        chunk.world.SetBlockID(pos.x, pos.y, pos.z, blockID);
        //chunk.world.SetBlockData(pos.x, pos.y, pos.z, blockData);

        return true;
    }

    public static int GetBlockID(RaycastHit hit, bool adjacent = false)
    {
        ChunkInstance chunk = hit.collider.GetComponent<ChunkInstance>();
        if (chunk == null)
            return 0;

        WorldPos pos = GetBlockPos(hit, adjacent);


        return chunk.world.GetBlock(pos.x, pos.y, pos.z);
    }
}