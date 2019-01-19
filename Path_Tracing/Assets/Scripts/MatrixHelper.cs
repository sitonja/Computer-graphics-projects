
using UnityEngine;

public static class MatrixHelper {
    public static float MultiplyVector4(Vector4 vector1, Vector4 vector2)
    {
        float result = 0;
        for (int i = 0; i < 4; i++)
        {
            result += vector1[i] * vector2[i];
        }
        return result;
    }

    public static Vector3 MultiplyMatrices(Vector4 vector4, Matrix4x3 matrix4X3)
    {
        float x = MultiplyVector4(vector4, matrix4X3.GetColumn(0));
        float y = MultiplyVector4(vector4, matrix4X3.GetColumn(1));
        float z = MultiplyVector4(vector4, matrix4X3.GetColumn(2));
        return new Vector3(x, y, z);
    }

    public static Vector4 MultiplyMatrices(Vector4 vector4, Matrix4x4 matrix)
    {
        float x = MultiplyVector4(vector4, matrix.GetColumn(0));
        float y = MultiplyVector4(vector4, matrix.GetColumn(1));
        float z = MultiplyVector4(vector4, matrix.GetColumn(2));
        float w = MultiplyVector4(vector4, matrix.GetColumn(3));
        return new Vector4(x, y, z, w);
    }

    public static Vector4 MultiplyMatrices(Vector3 vector3, Matrix3x4  matrix)
    {
        Vector4 result = new Vector4();
        for (int i = 0; i < 4; i++)
        {
            result[i] = MultiplyVector3(vector3, matrix.GetColumn(i));
        }
        
        return new Vector4(result[0], result[1], result[2], result[3]);
    }

    public static float MultiplyVector3(Vector3 vector1, Vector3 vector2)
    {
        float result = 0;
        for (int i = 0; i < 3; i++)
        {
            result += vector1[i] * vector2[i];
        }
        return result;
    }
}

