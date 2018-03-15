using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshUtility : MonoBehaviour {

	public static MeshData CreateMeshFromSprite(Sprite sprite) {
		MeshData meshData = new MeshData();
		for(int x = 0; x < sprite.texture.width; x++) {
			for(int y = 0; y < sprite.texture.height; y++) {
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

		if(y < texture.height - 1) {
			if(texture.GetPixel(x, y+1).a > 0) {

			} else {
                meshData = GetFaceData(texture, BlockData.Direction.Up, x, y, meshData);
			}
		}

        if(y > 0) {
            if(texture.GetPixel(x, y-1).a > 0) {

            } else {
                meshData = GetFaceData(texture, BlockData.Direction.Down, x, y, meshData);
            }
        }

        meshData = GetFaceData(texture, BlockData.Direction.South, x, y, meshData);

        meshData = GetFaceData(texture, BlockData.Direction.North, x, y, meshData);

        if(x < texture.width - 1) {
            if(texture.GetPixel(x + 1, y).a > 0) {

            } else {
                meshData = GetFaceData(texture, BlockData.Direction.East, x, y, meshData);
            }
        }

        if(x > 0) {
            if(texture.GetPixel(x - 1, y).a > 0) {

            } else {
                meshData = GetFaceData(texture, BlockData.Direction.West, x, y, meshData);
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

    static MeshData GetFaceData (Texture2D texture, BlockData.Direction direction, int x, int y, MeshData meshData)
	{
        float size = 1f / 32f;
        switch (direction)
        {
//            case BlockInstance.Direction.Up:
//                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, 0.5f));
//                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, 0.5f));
//                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, -0.5f));
//                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, -0.5f));
//                break;
//            case BlockInstance.Direction.Down:
//                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, -0.5f));
//                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, -0.5f));
//                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, 0.5f));
//                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, 0.5f));
//                break;
//            case BlockInstance.Direction.North:
//                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, 0.5f));
//                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, 0.5f));
//                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, 0.5f));
//                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, 0.5f));
//                break;
//            case BlockInstance.Direction.East:
//                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, -0.5f));
//                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, -0.5f));
//                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, 0.5f));
//                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, 0.5f));
//                break;
//            case BlockInstance.Direction.South:
//                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, -0.5f));
//                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, -0.5f));
//                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, -0.5f));
//                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, -0.5f));
//                break;
//            case BlockInstance.Direction.West:
//                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, 0.5f));
//                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, 0.5f));
//                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, -0.5f));
//                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, -0.5f));
//                break;

            case BlockData.Direction.Up:
                meshData.AddVertex(new Vector3(x*size - size/2f, y*size + size/2f, size/2f));
                meshData.AddVertex(new Vector3(x*size + size/2f, y*size + size/2f, size/2f));
                meshData.AddVertex(new Vector3(x*size + size/2f, y*size + size/2f, -size/2f));
                meshData.AddVertex(new Vector3(x*size - size/2f, y*size + size/2f, -size/2f));
                break;
            case BlockData.Direction.Down:
                meshData.AddVertex(new Vector3(x*size - size/2f, y*size - size/2f, -size/2f));
                meshData.AddVertex(new Vector3(x*size + size/2f, y*size - size/2f, -size/2f));
                meshData.AddVertex(new Vector3(x*size + size/2f, y*size - size/2f, size/2f));
                meshData.AddVertex(new Vector3(x*size - size/2f, y*size - size/2f, size/2f));
                break;
            case BlockData.Direction.North:
                meshData.AddVertex(new Vector3(x*size + size/2f, y*size - size/2f, size/2f));
                meshData.AddVertex(new Vector3(x*size + size/2f, y*size + size/2f, size/2f));
                meshData.AddVertex(new Vector3(x*size - size/2f, y*size + size/2f, size/2f));
                meshData.AddVertex(new Vector3(x*size - size/2f, y*size - size/2f, size/2f));
                break;
            case BlockData.Direction.East:
                meshData.AddVertex(new Vector3(x*size + size/2f, y*size - size/2f, -size/2f));
                meshData.AddVertex(new Vector3(x*size + size/2f, y*size + size/2f, -size/2f));
                meshData.AddVertex(new Vector3(x*size + size/2f, y*size + size/2f, size/2f));
                meshData.AddVertex(new Vector3(x*size + size/2f, y*size - size/2f, size/2f));
                break;
            case BlockData.Direction.South:
                meshData.AddVertex(new Vector3(x*size - size/2f, y*size - size/2f, -size/2f));
                meshData.AddVertex(new Vector3(x*size - size/2f, y*size + size/2f, -size/2f));
                meshData.AddVertex(new Vector3(x*size + size/2f, y*size + size/2f, -size/2f));
                meshData.AddVertex(new Vector3(x*size + size/2f, y*size - size/2f, -size/2f));
                break;
            case BlockData.Direction.West:
                meshData.AddVertex(new Vector3(x*size - size/2f, y*size - size/2f, size/2f));
                meshData.AddVertex(new Vector3(x*size - size/2f, y*size + size/2f, size/2f));
                meshData.AddVertex(new Vector3(x*size - size/2f, y*size + size/2f, -size/2f));
                meshData.AddVertex(new Vector3(x*size - size/2f, y*size - size/2f, -size/2f));
                break;
        }
		

		meshData.AddQuadTriangles();

        float tileSize = 1f / 32f;
        Vector2[] UVs = new Vector2[4];

        UVs[0] = new Vector2(tileSize * x + tileSize, tileSize * y);
        UVs[1] = new Vector2(tileSize * x + tileSize, tileSize * y + tileSize);
        UVs[2] = new Vector2(tileSize * x, tileSize * y + tileSize);
        UVs[3] = new Vector2(tileSize * x, tileSize * y);
        meshData.AddUVs(UVs);
		return meshData;
	}
}
