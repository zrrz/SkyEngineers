using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshData
{
    public List<Vector3> vertices;
    public List<int> triangles;
    public List<Vector2> uv;

    public List<Vector3> colVertices;
    public List<int> colTriangles;

    public bool useRenderDataForCol;

    public MeshData() {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uv = new List<Vector2>();
        colVertices = new List<Vector3>();
        colTriangles = new List<int>();
        useRenderDataForCol = true;
    }

    public void Clear() {
        vertices.Clear();
        triangles.Clear();
        uv.Clear();
        colVertices.Clear();
        colTriangles.Clear();
    }

    public void AddQuadTriangles()
    {
        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);

        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);

        if (useRenderDataForCol)
        {
            colTriangles.Add(colVertices.Count - 4);
            colTriangles.Add(colVertices.Count - 3);
            colTriangles.Add(colVertices.Count - 2);
            colTriangles.Add(colVertices.Count - 4);
            colTriangles.Add(colVertices.Count - 2);
            colTriangles.Add(colVertices.Count - 1);
        }
    }

    public void AddUVs(Vector2[] newUVs) {
        uv.AddRange(newUVs);
    }

    public void AddVertex(Vector3 vertex)
    {
        vertices.Add(vertex);

        if (useRenderDataForCol)
        {
            colVertices.Add(vertex);
        }
    }

    //public void AddTriangle(int tri)
    //{
        //triangles.Add(tri);

        //if (useRenderDataForCol)
        //{
        //    colTriangles.Add(tri - (vertices.Count - colVertices.Count));
        //}
    //}
}