using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator {

    public static Mesh CreateSquareGameobject()
    {
        float width = 1;
        float height = 1;

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4];

        vertices[0] = new Vector3(-width, -height);
        vertices[1] = new Vector3(-width, height);
        vertices[2] = new Vector3(width, height);
        vertices[3] = new Vector3(width, -height);

        mesh.vertices = vertices;

        mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        return mesh;
    }
	
    public Mesh ConvertTrianglesToMesh(IList<Triangle> triangles, float scale = 1)
    {
        IList<Vector3> vertices = new List<Vector3>();
        for (int i = 0; i < triangles.Count; i++)
        {
            if (!vertices.Contains(triangles[i].A))
            {
                vertices.Add(triangles[i].A);
            }
            if (!vertices.Contains(triangles[i].B))
            {
                vertices.Add(triangles[i].B);
            }
            if (!vertices.Contains(triangles[i].C))
            {
                vertices.Add(triangles[i].C);
            }
        }


        int[] meshTriangles = new int[triangles.Count * 3]; 


        for (int i = 0; i < triangles.Count; i++)
        {
            meshTriangles[i * 3] = vertices.IndexOf(triangles[i].A);
            meshTriangles[i * 3 + 1] = vertices.IndexOf(triangles[i].B);
            meshTriangles[i * 3 + 2] = vertices.IndexOf(triangles[i].C);
        }

        Mesh mesh = new Mesh();
        Vector3[] meshVertices = new Vector3[vertices.Count];
        vertices.CopyTo(meshVertices, 0);
        mesh.vertices = meshVertices;
        mesh.triangles = meshTriangles;
        mesh.uv = VectorUtil.Vector3ToVector2(mesh.vertices);
        mesh.vertices = ScaleVertices(mesh.vertices, scale);
        mesh.RecalculateNormals();
        return mesh;
    }
    private Vector3[] ScaleVertices(Vector3[] vertices, float scale)
    {
        Vector3[] result = new Vector3[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            result[i] = vertices[i] * scale;
        }
        return result;
    }

    public Mesh CreateRectangleMesh(Rectangle rectangle)
    {
        int[] triangles = new int[6] { 0, 1, 2, 0, 2, 3};

        Vector3[] vertices = rectangle.localPoints;

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;

        mesh.triangles = triangles;
        return mesh;
    }
}
