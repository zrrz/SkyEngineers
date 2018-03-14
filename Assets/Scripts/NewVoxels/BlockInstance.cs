﻿using UnityEngine;
using System.Collections;
using System;
using MessagePack;

[MessagePackObject]
public class BlockInstance
{
	public enum Direction { Up, Down, East, West, North, South,};

    [Key(0)]
    public int ID = 0; //ID of corresponding BlockData this is an instance of
    [IgnoreMember]
    public bool changed = true;

    public static float tileSize = 1f/64f;

    //Base block constructor
    public BlockInstance()
    {

    }

    public virtual MeshData GetBlockdata
     (ChunkInstance chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.useRenderDataForCol = true;

        if (!BlockLoader.GetBlock(chunk.GetBlock(x, y + 1, z)).IsSolid(Direction.Down))
        {
            meshData = FaceDataUp(chunk, x, y, z, meshData);
        }

        if (!BlockLoader.GetBlock(chunk.GetBlock(x, y - 1, z)).IsSolid(Direction.Up))
        {
            meshData = FaceDataDown(chunk, x, y, z, meshData);
        }

        if (!BlockLoader.GetBlock(chunk.GetBlock(x, y, z + 1)).IsSolid(Direction.South))
        {
            meshData = FaceDataNorth(chunk, x, y, z, meshData);
        }

        if (!BlockLoader.GetBlock(chunk.GetBlock(x, y, z - 1)).IsSolid(Direction.North))
        {
            meshData = FaceDataSouth(chunk, x, y, z, meshData);
        }

        if (!BlockLoader.GetBlock(chunk.GetBlock(x + 1, y, z)).IsSolid(Direction.West))
        {
            meshData = FaceDataEast(chunk, x, y, z, meshData);
        }

        if (!BlockLoader.GetBlock(chunk.GetBlock(x - 1, y, z)).IsSolid(Direction.East))
        {
            meshData = FaceDataWest(chunk, x, y, z, meshData);
        }

        return meshData;
    }

    protected virtual MeshData FaceDataUp
        (ChunkInstance chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));

        meshData.AddQuadTriangles();
        meshData.AddUVs(FaceUVs(Direction.Up));
        return meshData;
    }

    protected virtual MeshData FaceDataDown
        (ChunkInstance chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

        meshData.AddQuadTriangles();
        meshData.AddUVs(FaceUVs(Direction.Down));
        return meshData;
    }

    protected virtual MeshData FaceDataNorth
        (ChunkInstance chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

        meshData.AddQuadTriangles();
        meshData.AddUVs(FaceUVs(Direction.North));
        return meshData;
    }

    protected virtual MeshData FaceDataEast
        (ChunkInstance chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));

        meshData.AddQuadTriangles();
        meshData.AddUVs(FaceUVs(Direction.East));
        return meshData;
    }

    protected virtual MeshData FaceDataSouth
        (ChunkInstance chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));

        meshData.AddQuadTriangles();
        meshData.AddUVs(FaceUVs(Direction.South));
        return meshData;
    }

    protected virtual MeshData FaceDataWest
        (ChunkInstance chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));

        meshData.AddQuadTriangles();
        meshData.AddUVs(FaceUVs(Direction.West));

        return meshData;
    }

    public virtual BlockData.TexturePosition TexturePosition(Direction direction)
    {
//        Tile tile = new Tile();
//        tile.x = 0;
//        tile.y = 0;
//
//        return tile;
//
//
		if(BlockLoader.GetBlock(ID).texturePosition != null && BlockLoader.GetBlock(ID).texturePosition.Length > (int)direction)
			return BlockLoader.GetBlock(ID).texturePosition[(int)direction];
		else 
        	return new BlockData.TexturePosition(0,0);
//        return BlockDatabase.GetBlock(ID).texturePosition[(int)direction];
    }

    public virtual Vector2[] FaceUVs(Direction direction)
    {
        Vector2[] UVs = new Vector2[4];
        BlockData.TexturePosition tilePos = TexturePosition(direction);

        UVs[0] = new Vector2(tileSize * tilePos.x + tileSize,
            tileSize * tilePos.y);
        UVs[1] = new Vector2(tileSize * tilePos.x + tileSize,
            tileSize * tilePos.y + tileSize);
        UVs[2] = new Vector2(tileSize * tilePos.x,
            tileSize * tilePos.y + tileSize);
        UVs[3] = new Vector2(tileSize * tilePos.x,
            tileSize * tilePos.y);

        return UVs;
    }

    public virtual bool IsSolid(Direction direction)
    {
        return BlockLoader.GetBlock(ID).solid[(int)direction];
//        switch (direction)
//        {
//            case Direction.north:
//                return true;
//            case Direction.east:
//                return true;
//            case Direction.south:
//                return true;
//            case Direction.west:
//                return true;
//            case Direction.up:
//                return true;
//            case Direction.down:
//                return true;
//            default:
//                Debug.LogError("No case");
//                return false;
//        }

//        return false;
    }

}