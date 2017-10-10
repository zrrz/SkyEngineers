using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshUtility : MonoBehaviour {

	public static MeshData CreateMeshFromSprite(Sprite sprite) {
		MeshData meshData = new MeshData();
		for(int x = 0; x < sprite.texture.width; x++) {
			for(int y = 0; y < sprite.texture.height; x++) {
				Color color = sprite.texture.GetPixel(x,y);
				if(color.a > 0f) {
					meshData = GetPixelData(sprite.texture, x, y, meshData);
				}
			}
		}

		return meshData;

//		Mesh mesh = new Mesh();
//		mesh.SetVertices(meshData.vertices);
//		mesh.SetTriangles(meshData.triangles, 0);
//
//		mesh.SetUVs(0, meshData.uv);
//		filter.mesh.RecalculateNormals();
//
//		coll.sharedMesh = null;
//		Mesh mesh = new Mesh();
//		mesh.SetVertices(meshData.colVertices);
//		mesh.SetTriangles(meshData.colTriangles, 0);
//		mesh.RecalculateNormals();
//
//		coll.sharedMesh = mesh;
	}

	public static MeshData GetPixelData
	(Texture2D texture, int x, int y, MeshData meshData)
	{
//		meshData.useRenderDataForCol = true;

		if(y > texture.height - 1) {
			if(texture.GetPixel(x, y+1).a > 0) {

			} else {

			}
		}

		return meshData;
//		if (!chunk.GetBlock(x, y + 1, z).IsSolid(Direction.Down))
//		{
//			meshData = FaceDataUp(chunk, x, y, z, meshData);
//		}
//
//		if (!chunk.GetBlock(x, y - 1, z).IsSolid(Direction.Up))
//		{
//			meshData = FaceDataDown(chunk, x, y, z, meshData);
//		}
//
//		if (!chunk.GetBlock(x, y, z + 1).IsSolid(Direction.South))
//		{
//			meshData = FaceDataNorth(chunk, x, y, z, meshData);
//		}
//
//		if (!chunk.GetBlock(x, y, z - 1).IsSolid(Direction.North))
//		{
//			meshData = FaceDataSouth(chunk, x, y, z, meshData);
//		}
//
//		if (!chunk.GetBlock(x + 1, y, z).IsSolid(Direction.West))
//		{
//			meshData = FaceDataEast(chunk, x, y, z, meshData);
//		}
//
//		if (!chunk.GetBlock(x - 1, y, z).IsSolid(Direction.East))
//		{
//			meshData = FaceDataWest(chunk, x, y, z, meshData);
//		}
//
//		return meshData;
	}
//
//	protected virtual MeshData FaceDataUp
//	(Chunk chunk, int x, int y, int z, MeshData meshData)
//	{
//		meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
//		meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
//		meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
//		meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
//
//		meshData.AddQuadTriangles();
//		meshData.AddUVs(FaceUVs(Direction.Up));
//		return meshData;
//	}
//
//	protected virtual MeshData FaceDataDown
//	(Chunk chunk, int x, int y, int z, MeshData meshData)
//	{
//		meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
//		meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
//		meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
//		meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
//
//		meshData.AddQuadTriangles();
//		meshData.AddUVs(FaceUVs(Direction.Down));
//		return meshData;
//	}
//
//	protected virtual MeshData FaceDataNorth
//	(Chunk chunk, int x, int y, int z, MeshData meshData)
//	{
//		meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
//		meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
//		meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
//		meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
//
//		meshData.AddQuadTriangles();
//		meshData.AddUVs(FaceUVs(Direction.North));
//		return meshData;
//	}
//
//	protected virtual MeshData FaceDataEast
//	(Chunk chunk, int x, int y, int z, MeshData meshData)
//	{
//		meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
//		meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
//		meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
//		meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
//
//		meshData.AddQuadTriangles();
//		meshData.AddUVs(FaceUVs(Direction.East));
//		return meshData;
//	}
//
//	protected virtual MeshData FaceDataSouth
//	(Chunk chunk, int x, int y, int z, MeshData meshData)
//	{
//		meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
//		meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
//		meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
//		meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
//
//		meshData.AddQuadTriangles();
//		meshData.AddUVs(FaceUVs(Direction.South));
//		return meshData;
//	}
//
//	protected virtual MeshData FaceDataWest
//	(Chunk chunk, int x, int y, int z, MeshData meshData)
//	{
//		meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
//		meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
//		meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
//		meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
//
//		meshData.AddQuadTriangles();
//		meshData.AddUVs(FaceUVs(Direction.West));
//
//		return meshData;
//	}
}
