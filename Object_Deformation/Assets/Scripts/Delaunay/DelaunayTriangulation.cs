using System.Collections.Generic;
using UnityEngine;

public class DelaunayTriangulation
{

    // Bowyer-Watson algorithm
    public IList<Triangle> Triangulate(Vector3[] pointList)
    {
        List<Triangle> triangulation = new List<Triangle>();
        Triangle superTriangle = CreateBoundingTriangle(pointList);
        triangulation.Add(superTriangle);
        for (int i = 0; i < pointList.Length; i++)
        {
            
            List<Triangle> badTriangles = new List<Triangle>();
            for (int j = 0; j < triangulation.Count; j++)
            {
                Triangle triangle = triangulation[j];

                if (triangle.IsInsideCircumcircle(pointList[i]))
                {
                    badTriangles.Add(triangle);
                }

            }
            ICollection<Edge> polygon = new HashSet<Edge>();

            if (badTriangles.Count == 1)
            {
                Triangle triangle = badTriangles[0];
                IList<Edge> edges = triangle.GetEdges();
                foreach (Edge edge in edges)
                {
                    polygon.Add(edge);
                }
            }
            for (int j = 0; j < badTriangles.Count; j++)
            {
                Triangle triangle = badTriangles[j];
                foreach (Edge edge in triangle.GetEdges()) {
                    ICollection<Triangle> triangleList = new HashSet<Triangle>(badTriangles);
                    triangleList.Remove(triangle);
                    if (edge.IsEdgeUnshared(triangleList))
                    {
                        polygon.Add(edge);
                    }
                    
                }

            }
            //remove each triangle in badTriangles from triangulation
            foreach (Triangle triangle in badTriangles)
            {
                //remove triangle from triangulation
                triangulation.Remove(triangle);
            }
            foreach (Edge edge in polygon)
            {
                Triangle newTri = new Triangle(pointList[i], edge.A, edge.B);
                if (newTri.IsValid() && !triangulation.Contains(newTri))
                {
                    triangulation.Add(newTri);
                }
            }
        }
        triangulation = triangulation.FindAll(triangle =>  triangle.ContainsVertex(pointList));
        return triangulation;
    }

    private Triangle CreateBoundingTriangle(Vector3[] pointList)
    {
        Vector2 lowerBound = Vector2.positiveInfinity;
        Vector2 upperBound = Vector2.negativeInfinity;

        for (int i = 0; i < pointList.Length; i++)
        {
            if (pointList[i].x > upperBound.x) upperBound.x = pointList[i].x;
            if (pointList[i].y > upperBound.y) upperBound.y = pointList[i].y;

            if (pointList[i].x < lowerBound.x) lowerBound.x = pointList[i].x;
            if (pointList[i].y < lowerBound.y) lowerBound.y = pointList[i].y;
        }
        Vector2 boxLenght = upperBound - lowerBound;

        Vector2 triPoint1 = lowerBound;
        Vector2 triPoint2 = new Vector2(lowerBound.x + boxLenght.x * 2.0f, lowerBound.y);
        Vector2 triPoint3 = new Vector2(lowerBound.x, lowerBound.y + boxLenght.y * 2.0f);

        Triangle boundingTriangle = new Triangle(triPoint1, triPoint2, triPoint3);
        return boundingTriangle;
    }
}
