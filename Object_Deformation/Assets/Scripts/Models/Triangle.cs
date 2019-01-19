using System.Collections.Generic;
using UnityEngine;

public class Triangle  {

    public Vector3 A { get; set; }
    public Vector3 B { get; set; }
    public Vector3 C { get; set; }

    public Edge E1 { get; set; }
    public Edge E2 { get; set; }
    public Edge E3 { get; set; }
    
    public float InitialArea { get; set; }
    public float CurrentArea { get; set; }
    public Vector3 Center { get; set; }

    public Triangle(Vector3 a, Vector3 b, Vector3 c)
    {
        if (!IsWindingCorrect(a, b, c))
        {
            A = c;
            B = b;
            C = a;
        } else
        {
            A = a;
            B = b;
            C = c;
        }

        E1 = new Edge(A, B);
        E2 = new Edge(B, C);
        E3 = new Edge(C, A);

        SetCenter();
        CalculateTriangleArea(A, B, C);
        InitialArea = CurrentArea;
    }

    private void CalculateTriangleArea(Vector3 a, Vector3 b, Vector3 c)
    {
        float area = ((a.x*(b.y - c.y) + b.x*(c.y-a.y) + c.x *(a.y - b.y))/2);
        CurrentArea = Mathf.Abs(area);
    }

    public void SetCenter()
    {
        Center = (A + B + C) / 3;
    }
    private bool IsWindingCorrect(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 ab = b - a;
        Vector3 bc = c - b;
        Vector3 surfaceNormal = Vector3.Cross(ab, bc);

        return Vector3.Dot(surfaceNormal, -Vector3.forward) >= 0;
    }

    public IList<Edge> GetEdges()
    {
        List<Edge> edges = new List<Edge>
        {
            E1,
            E2,
            E3
        };

        return edges;
    }

    public bool ContainsEdge(Edge edge)
    {
        if (edge.Equals(E1) || edge.Equals(E2) || edge.Equals(E3))
        {
            return true;
        }
        return false;
    }

    public IList<Edge> GetUnsharedEdges(Triangle triangle)
    {
        IList<Edge> edges = triangle.GetEdges();
        IList<Edge> unsharedEdges = new List<Edge>();
        for (int i = 0; i < edges.Count; i++)
        {
            if (!ContainsEdge(edges[i]))
            {
                unsharedEdges.Add(edges[i]);
            }
        }
        return unsharedEdges;
    }

  

    public bool ContainsVertex(Vector3 vertex)
    {
        return vertex.Equals(A) || vertex.Equals(B) || vertex.Equals(C);
    }

    public bool ContainsVertex(Triangle triangle)
    {
        return ContainsVertex(triangle.A) || ContainsVertex(triangle.B) || ContainsVertex(triangle.C);
    }

    public bool ContainsVertex(IList<Vector3> points)
    {
        int i = 0;
        foreach (Vector3 point in points)
        {
            if (ContainsVertex(point))
            {
                i++;
                if (i >= 3)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsInsideCircumcircle(Vector3 point)
    {
        Vector3 P1 = A;
        Vector3 P2 = B;
        Vector3 P3 = C;
        float radius = ((P1 - P2).magnitude * (P2 - P3).magnitude * (P3 - P1).magnitude) / (2.0f * Vector3.Cross(P1-P2, P2-P3).magnitude);

        float alpha = CalculateCoefficient(P2, P3, P1);
        float beta = CalculateCoefficient(P1, P3, P2);
        float sigma = CalculateCoefficient(P1, P2, P3);
        Vector3 center = alpha * P1 + beta * P2 + sigma * P3;

        float distance = (point - center).magnitude;
        return distance < radius;
    }

    private float CalculateCoefficient(Vector3 P1, Vector3 P2, Vector3 P3)
    {
        return (Vector3.SqrMagnitude(P1 - P2) * Vector3.Dot((P3 - P1), (P3 - P2))) / (2.0f * Vector3.SqrMagnitude(Vector3.Cross((P1-P2), (P2-P3))));
    }

    public bool IsValid()
    {
        return !((A.x == B.x && B.x == C.x && A.x == C.x) || (A.y == B.y && A.y == B.y && A.y == C.y));
    }


    public override bool Equals(object obj)
    {
        var triangle = obj as Triangle;
        if (triangle == null) return false;

        Vector3[] points = new Vector3[]
        {
            triangle.A, triangle.B, triangle.C
        };

        for (int i = 0; i < 3; i++)
        {
            if (!ContainsVertex(points[i]))
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 793064651;
        hashCode = hashCode * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(A);
        hashCode = hashCode * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(B);
        hashCode = hashCode * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(C);
        return hashCode;
    }

    public void SetTriangle(Node node1, Node node2, Node node3)
    {
        A = node1.position;
        B = node2.position;
        C = node3.position;
        SetCenter();
        CalculateTriangleArea(A, B , C);
    }

    public bool IsInside(Vector3 p)
    {
        float s = A.y * C.x - A.x * C.y + (C.y - A.y) * p.x + (A.x - C.x) * p.y;
        float t = A.x * B.y - A.y * B.x + (A.y - B.y) * p.x + (B.x - A.x) * p.y;

        if ((s < 0) != (t < 0))
            return false;

        float area = CurrentArea;

        return s > 0 && t > 0 && (s + t) <= area;
    }
}
