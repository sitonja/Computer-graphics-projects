
using UnityEngine;

public class MockPointsGenerator {
    private float width;
    private float height;

    public MockPointsGenerator()
    {
        width = 1;
        height = 1;
    }

    public MockPointsGenerator(float width, float height)
    {
        this.width = width;
        this.height = height;
    }

    public Vector3[] CreateSquarePoints(int detail, bool edge = false)
    {
        Vector3[] vertices = CreateSquarePoints();
        int numberOfPoints = detail * detail;
        Vector3[] additionVertices = new Vector3[numberOfPoints];
        for (int i = 0; i < detail; i++)
        {
            for (int j = 0; j < detail; j++)
            {
                additionVertices[i * detail + j] = new Vector3(-width + (2*width/(detail+1))*(j+1), -height + (2*height/(detail+1))*(i+1));
            }
        }

        if (edge)
        {
            Vector3[] edges = new Vector3[detail*4];

            for (int i = 0; i < detail; i++)
            {
                    edges[i*2] = new Vector3(-width + (2 * width / (detail + 1)) * (i + 1), height);
                    edges[i*2+1] = new Vector3(-width + (2 * width / (detail + 1)) * (i + 1), -height);
            }
            for (int i = 0; i < detail; i++)
            {
                edges[i*2+2*detail] = new Vector3(-width, -height + (2 * height / (detail + 1)) * (i + 1));
                edges[i*2+2*detail+1] = new Vector3(width, -height + (2 * height / (detail + 1)) * (i + 1));
            }
            vertices = ArrayUtil.AppendVector3Array(vertices, edges);
        }
        vertices = ArrayUtil.AppendVector3Array(vertices, additionVertices);
        return vertices;
    }
    public Vector3[] CreateSquarePoints()
    {
        Vector3[] vertices = new Vector3[4];

        vertices[0] = new Vector3(-width, -height);
        vertices[1] = new Vector3(-width, height);
        vertices[2] = new Vector3(width, height);
        vertices[3] = new Vector3(width, -height);
        return vertices;
    }


}
