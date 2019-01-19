
using System;
using System.Collections.Generic;
using UnityEngine;

public class BSpline
{

    private Matrix4x4 matrix;
    private Matrix3x4 tangentMatrix;

    private Vector3[] points;

    public Vector3[] Points
    {
        get
        {
            return points;
        }

        set
        {
            points = value;
        }
    }

    public Vector3[] TangentPoints
    {
        get
        {
            return tangentPoints;
        }

        set
        {
            tangentPoints = value;
        }
    }

    private Vector3[] tangentPoints;

    public BSpline()
    {
        // dodaj zadanu matricu B
        Vector4 row1 = new Vector4(-1, 3, -3, 1);
        Vector4 row2 = new Vector4(3, -6, 3, 0);
        Vector4 row3 = new Vector4(-3, 0, 3, 0);
        Vector4 row4 = new Vector4(1, 4, 1, 0);
        Vector4[] rows = { row1, row2, row3, row4 };
        matrix = new Matrix4x4(rows);
        // matrica tangente B'
        tangentMatrix = new Matrix3x4(InitializeTangentMatrix());
    }

    private float[][] InitializeTangentMatrix()
    {
        float[] row1 = { -1, 3, -3, 1 };
        float[] row2 = { 2, -4, 2, 0 };
        float[] row3 = { -1, 0, 1, 0 };
        float[][] matrix = { row1, row2, row3 };
        return matrix;
    }

    public Vector3[] CalculateSegment(Vector3[] vectors)
    {
        const int iterations = 100;
        Vector3[] resultPoints = new Vector3[iterations];

        for (int i = 0; i < iterations; i++)
        {
            // računaj matrice
            float t = (float)i / iterations;
            Vector4 tVector = new Vector4(Mathf.Pow(t, 3), Mathf.Pow(t, 2), Mathf.Pow(t, 1), 1) / 6;
            Vector4 vector4 = MatrixHelper.MultiplyMatrices(tVector, matrix);

            Matrix4x3 pointsMatrix4x3 = new Matrix4x3(vectors);

            Vector3 point = MatrixHelper.MultiplyMatrices(vector4, pointsMatrix4x3);
            resultPoints[i] = point;
        }
        return resultPoints;
    }

    public Vector3[] CalculateBSpline(Vector3[] vectors)
    {
        const int iterations = 100;
        // broj segmenata
        int segmentNumber = vectors.Length - 3;
        Vector3[] resultPoints = new Vector3[segmentNumber * iterations];
        Vector3[] tangentPoints = new Vector3[segmentNumber * iterations];

        for (int i = 0; i < segmentNumber; i++)
        {
            // dohvati 4 kontrolne točke
            Vector3[] points = GetSubarray(vectors, i, 4);
            // izračunaj segment
            Vector3[] segmentPoints = CalculateSegment(points);
            // izračunaj točke tangente
            Vector3[] segmentTangentPoints = CalculateTangent(points);

            for (int j = 0; j < iterations; j++)
            {
                resultPoints[i * iterations + j] = segmentPoints[j];
                tangentPoints[i * iterations + j] = segmentTangentPoints[j];
            }
        }

        points = resultPoints;
        this.TangentPoints = tangentPoints;

        return resultPoints;
    }

    private Vector3[] GetSubarray(Vector3[] array, int startIndex, int range)
    {
        Vector3[] vectors = new Vector3[range];
        for (int i = 0; i < range; i++)
        {
            vectors[i] = array[startIndex + i];
        }
        return vectors;
    }

    public Vector3[] CalculateTangent(Vector3[] vectors)
    {
        const int iterations = 100;
        Vector3[] resultPoints = new Vector3[iterations];

        for (int i = 0; i < iterations; i++)
        {
            // računaj matrice
            float t = (float)i / iterations;
            Vector3 tVector = new Vector3(Mathf.Pow(t, 2), t, 1) / 2;

            Vector4 vector4 = MatrixHelper.MultiplyMatrices(tVector, tangentMatrix);

            Matrix4x3 pointsMatrix4x3 = new Matrix4x3(vectors);

            Vector3 point = MatrixHelper.MultiplyMatrices(vector4, pointsMatrix4x3);
            resultPoints[i] = point;
        }
        return resultPoints;
    }
}
