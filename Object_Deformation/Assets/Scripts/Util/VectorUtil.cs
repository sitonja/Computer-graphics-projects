using UnityEngine;

public static class VectorUtil {

	public static Vector2[] Vector3ToVector2(Vector3[] points)
    {
        Vector2[] arr = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            arr[i] = new Vector2(points[i].x, points[i].y);
        }
        return arr;      
    }
}
