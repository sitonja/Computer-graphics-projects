using UnityEngine;

public class Point3d {
    private float x;
    private float y;
    private float z;

    public Point3d()
    {
        x = 0;
        y = 0;
        z = 0;
    }

    public Point3d(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }


    public override bool Equals(object obj)
    {
        var d = obj as Point3d;
        return d != null &&
               x == d.x &&
               y == d.y &&
               z == d.z;
    }

    public override int GetHashCode()
    {
        var hashCode = 373119288;
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        hashCode = hashCode * -1521134295 + z.GetHashCode();
        return hashCode;
    }

    public Vector3 vector3
    {
        get
        {
            return new Vector3(x, y, z);
        }
    }
}
