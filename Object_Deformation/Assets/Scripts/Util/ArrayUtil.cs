using UnityEngine;

public class ArrayUtil {
    public static Vector3[] AppendVector3Array(Vector3[] v1, Vector3[] v2)
    {
        int arrLenght = v1.Length + v2.Length;
        Vector3[] arr = new Vector3[arrLenght];

        for (int i = 0; i < arrLenght; i++)
        {
            if (i < v1.Length)
            {
                arr[i] = v1[i];
            } else
            {
                arr[i] = v2[i - v1.Length];
            }
        }
        return arr;
    }
}
