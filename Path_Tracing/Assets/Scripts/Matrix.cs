
public class Matrix {
    private float[][] matrix;

    public int Height {
        get { return matrix.Length; }
    }

    public int Width
    {
        get { return matrix[0].Length; }
    }

    public Matrix(int width, int height)
    {
        matrix = new float[height][];

        for(int i = 0; i < height; i++)
        {
            matrix[i] = new float[width];
        }
    }

    public Matrix(float[][] matrix)
    {
        this.matrix = matrix;
    }

    public float[] GetColumn(int index)
    {
        float[] col = new float[matrix.Length];

        for(int i = 0; i < matrix.Length; i++)
        {
            col[i] = matrix[i][index];
        }

        return col;
    }


    public float[] GetRow(int index)
    {
        return matrix[index];
    }

    public float GetValue(int row, int col)
    {
        return matrix[row][col];
    }
}
