using UnityEngine;

public static class TriangleUtil {

    public static Vector3 GetTriangleCentroid(Vector3 a, Vector3 b, Vector3 c)
    {
        return (a + b + c) / 3;
    }

  
}
