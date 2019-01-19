
using UnityEngine;

public class Matrix4x3 {
    private Matrix matrix;
    

    public Matrix4x3(float[][] matrix)
    {
        this.matrix = new Matrix(matrix);
    }

    public Matrix4x3(Vector3[] vectors)
    {
        matrix = new Matrix(ConvertVector3ToArray(vectors));
    }

   private float[][] ConvertVector3ToArray(Vector3[] vectors)
    {
        float[][] matrix = new float[4][];

        for (int i = 0; i < 4; i++)
        {
            matrix[i] = new float[3];
            for (int j = 0; j < 3; j++)
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

    public float Get(int row, int col) {
        return matrix.GetValue(row, col);
    }
}
