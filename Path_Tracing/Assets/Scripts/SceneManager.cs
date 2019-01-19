using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {
    private const string BSplineFilePath = "Assets/bSpline.obj";

    public GameObject plane;
    public float deltaTime = 0.5f;
    public bool showTangents = false;
    private Vector3[] points;
    private Vector3[] bSplinePoints;
    private Vector3[] tangentPoints;

    private GameObject createdPlane;
    Quaternion[] rotations;

    // Use this for initialization
    void Start () {
        //stvori toček b-krivulje
        InitializePoints();
        // stvori objekt
        createdPlane = Instantiate(plane, bSplinePoints[0], Quaternion.identity);
        // izračunaj orijentaciju
        InitializeRotation(createdPlane);
        StartCoroutine(Move(createdPlane));
    }

    private void InitializeRotation(GameObject gameObject)
    {
        rotations = new Quaternion[tangentPoints.Length];
        for (int i = 0; i < tangentPoints.Length; i++)
        {
            // početna orijentacija
            Vector3 s = gameObject.transform.forward;
            // ciljna orijentacija
            Vector3 e = tangentPoints[i];

            // izračunaj os = s x e
            float x = s.y * e.z - e.y * s.z;
            float y = -(s.x * e.z - e.x * s.z);
            float z = s.x * e.y - s.y * e.x;
            Vector3 os = new Vector3(x, y, z);

            // izračunaj kut prema formuli
            float angle = Mathf.Acos(MatrixHelper.MultiplyVector3(s, e) / (s.sqrMagnitude* e.sqrMagnitude));
            // kut u stupnjevima
            float angleDegrees = angle * 180 / Mathf.PI;
            rotations[i] = Quaternion.AngleAxis(angleDegrees, os);
        }
    }
	
	private void InitializePoints()
    {
        // čitaj iz datoteke kontrolne točke b-krivulje
        List<Vector3> points = PointsReader.ReadFile(BSplineFilePath);
        this.points = points.ToArray();
        BSpline bSpline = new BSpline();
        // izračunaj točke b-krivulje
        bSplinePoints = bSpline.CalculateBSpline(points.ToArray());
        tangentPoints = bSpline.TangentPoints;
    }

	void Update () {
	}
    // animiraj objekt
    private IEnumerator Move(GameObject gameObject)
    {
        for (int i = 0; i < bSplinePoints.Length; i++)
        {
            gameObject.transform.position = bSplinePoints[i];
            gameObject.transform.rotation = rotations[i];
            yield return new WaitForSeconds(deltaTime);
        }
    }


    private void DrawLines(List<Vector3> points)
    {
        Gizmos.color = Color.blue;

        for (int i = 0; i < points.Count - 1; i++)
        {
            Gizmos.DrawSphere(points[i], 0.5f);
            Gizmos.DrawLine(points[i], points[i+1]);
        }
        Gizmos.DrawSphere(points[points.Count - 1], 0.5f);
    }

    private void DrawBSpline(Vector3[] points)
    {
        Gizmos.color = Color.red;

        for (int i = 0; i < points.Length - 1; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
    }

    private void DrawTangents(Vector3[] startPoints, Vector3[] orientations)
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < startPoints.Length; i++)
        {
            Gizmos.DrawRay(startPoints[i], orientations[i]);
        }
    }
    private void OnDrawGizmos()
    {
        List<Vector3> points = PointsReader.ReadFile(BSplineFilePath);
        DrawLines(points);
        BSpline bSpline = new BSpline();

        bSplinePoints = bSpline.CalculateBSpline(points.ToArray());
        DrawBSpline(bSplinePoints);
        if (showTangents)
        {
            DrawTangents(bSplinePoints, bSpline.TangentPoints);
        }
    }
}
 