using UnityEngine;

public class Matrix4x4 {
    private Matrix matrix;

    public Matrix4x4(Vector4[] vectors)
    {
        matrix = new Matrix(ConvertVector4ToArray(vectors));
    }

    private float[][] ConvertVector4ToArray(Vector4[] vectors)
    {
        float[][] matrix = new float[4][];

        for (int i = 0; i < 4; i++)
        {
            matrix[i] = new float[4];
            for (int j = 0; j < 4; j++)
            {
                matrix[i][j] = vectors[i][j];
            }
        }
        return matrix;
    }

    public Vector4 GetColumn(int index)
    {
        float[] col = matrix.GetColumn(index);
        return new Vector4(col[0], col[1], col[2], col[3]);
    }
}
