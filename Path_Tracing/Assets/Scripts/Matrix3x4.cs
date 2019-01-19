using UnityEngine;

public class Matrix3x4 {

    private Matrix matrix;


    public Matrix3x4(float[][] matrix)
    {
        this.matrix = new Matrix(matrix);
    }

    public Matrix3x4(Vector4[] vectors)
    {
        matrix = new Matrix(ConvertVector4ToArray(vectors));
    }

    private float[][] ConvertVector4ToArray(Vector4[] vectors)
    {
        float[][] matrix = new float[4][];

        for (int i = 0; i < 3; i++)
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
        return new Vector3(col[0], col[1], col[2]);
    }

    public float Get(int row, int col)
    {
        return matrix.GetValue(row, col);
    }
}
