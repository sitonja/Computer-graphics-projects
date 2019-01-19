using System.Collections.Generic;
using UnityEngine;

public class Edge {
    public Vector3 A { get; set; }
    public Vector3 B { get; set; }

    public Edge(Vector3 a, Vector3 b)
    {
        A = a;
        B = b;
    }

    public bool IsEdgeUnshared(ICollection<Triangle> triangles)
    {
        foreach (Triangle triangle in triangles)
        {
            if (triangle.ContainsEdge(this))
            {
                return false;
            }
        }
        return true;
    }

    public override bool Equals(object obj)
    {
        var edge = obj as Edge;
        return edge != null &&
               (A.Equals(edge.A) &&
               B.Equals(edge.B)) || (A.Equals(edge.B) &&
               B.Equals(edge.A));
    }

    public override int GetHashCode()
    {
        var hashCode = -1817952719;
        hashCode = hashCode * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(A);
        hashCode = hashCode * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(B);
        return hashCode;
    }
}
