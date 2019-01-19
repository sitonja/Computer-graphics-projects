
using UnityEngine;

public class Rectangle {
    public float Height { get; set; }
    public float Width { get; set; }

    private float angle;
    public float Angle {
        get { return angle; }
        set {
                angle = value;
        }
    }
    public Vector3 A { get; set; }
    public Vector3 B { get; set; }
    public Vector3 C { get; set; }
    public Vector3 D { get; set; }

    private Vector3 center;
    public Vector3 Center
    {
        get
        {
            return center;
        }

        set
        {
            center = value;
        }
    }
    public Vector3[] normals;
    public Vector3[] localPoints;
    public Rectangle()
    {
        A = new Vector3();
        Height = 1;
        Width = 1;
        B = new Vector3(A.x, A.y + Height);
        C = new Vector3(A.x + Width, A.y + Height);
        D = new Vector3(A.x + Width, A.y);
        SetCenter();
        SetNormals();
    }

    public Rectangle(Vector3 position)
    {
        A = new Vector3(position.x, position.y);
        Height = 1;
        Width = 1;
        B = new Vector3(A.x, A.y + Height);
        C = new Vector3(A.x + Width, A.y + Height);
        D = new Vector3(A.x + Width, A.y);
        SetCenter();
        SetNormals();
    }

    public Rectangle(Vector3 position, float angle, float width, float height)
    {
        A = new Vector3();
        Height = height;
        Width = width;
        B = new Vector3(A.x, A.y + Height);
        C = new Vector3(A.x + Width, A.y + Height);
        D = new Vector3(A.x + Width, A.y);

        SetCenter();
        RotateCenter(angle);
        SetLocalPoints();
        Move(position);
        SetNormals();
    }



    public Rectangle(Vector3 A, Vector3 B, Vector3 C, Vector3 D, float rotation)
    {
        this.A = A;
        this.B = B;
        this.C = C;
        this.D = D;
        center.x = (A.x + D.x) / 2;
        center.y = (A.y + D.y) / 2;
        RotateCenter(angle);
        SetNormals();
    }

    private void SetLocalPoints()
    {
        localPoints = new Vector3[4]
        {
            A, B, C, D
        };
    }

    public void Move(Vector3 position)
    {
        A += position;
        B += position;
        C += position;
        D += position;
    }

    private void SetCenter()
    {
        center.x = A.x + Width / 2;
        center.y = A.y + Height / 2;
    }

    public void SetNormals()
    {
        Vector3 AB = B - A;
        Vector3 BC = C - B;
        Vector3 CD = D - C;
        Vector3 DA = A - D;
        normals = new Vector3[4]
        {
            
            new Vector3(-AB.y , AB.x),
            new Vector3(-BC.y , BC.x),
            new Vector3(-CD.y , CD.x),
            new Vector3(-DA.y , DA.x)};
    }

    public void Rotate(float angle)
    {
        Angle = angle;

        B = Quaternion.Euler(0, 0, angle) * B;
        C = Quaternion.Euler(0, 0, angle) * C;
        D = Quaternion.Euler(0, 0, angle) * D;
    }
    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angle)
    {
        return Quaternion.Euler(0, 0, angle) * (point - pivot) + pivot;
    }
    public void RotateCenter(float angle)
    {
        Angle = angle;
        A = RotatePointAroundPivot(A, center, angle);
        B = RotatePointAroundPivot(B, center, angle);
        C = RotatePointAroundPivot(C, center, angle);
        D = RotatePointAroundPivot(D, center, angle);
    }

    //0 <= dot(AB, AM) <= dot(AB, AB) &&
    //0 <= dot(BC, BM) <= dot(BC, BC)
    public bool IsColliding(Vector3 point)
    {
        Vector3 AP = point - A;
        Vector3 BP = point - B;
        Vector3 AB = B - A;
        Vector3 BC = C - B;
        Vector3 CD = D - C;
        Vector3 DA = A - D;
        float dotABAP = Vector3.Dot(AB, AP);
        float dotABAB = Vector3.Dot(AB, AB);
        float dotBCBP = Vector3.Dot(BC, BP);
        float dotBCBC = Vector3.Dot(BC, BC);


        return 0 <= dotABAP && dotABAP <= dotABAB && 0 <= dotBCBP && dotBCBP <= dotBCBC;
    }

    public Vector3 GetClosestNormal(Vector3 point)
    {
        Vector3[] edges = new Vector3[4] { B-A, C-B, D-C, A-D };
        Vector3[] rectPoints = new Vector3[4] { A, B, C, D};
        float minDist = float.PositiveInfinity;
        Vector3 closestEdge = edges[0];

        for (int i = 0; i < edges.Length; i++)
        {
            Vector3 w = (point - rectPoints[i]);
            Vector3 vl = edges[i];
            Vector3 ul = edges[i].normalized;

            float distance = (w - Vector3.Dot(w, ul) * ul).magnitude;
            if (distance < minDist)
            {
                minDist = distance;
                closestEdge = edges[i]; 
            }
        }
        return Vector2.Perpendicular(closestEdge).normalized;
    }
}
