using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    //square detail property
    public int detail;
    public bool setEdges;
    public Material defaultMaterial;
    // texture properties
    public Material mat;
    public float scale = 2.0f;
    public float resolution = 0.08f;
    public bool detectEdges;
    public int edgeInc;
    public Texture2D[] textures;
    public TextureTypes selected = TextureTypes.DONUT;

    // Physics properties
    public float dt = 1.0f / 60;
    public Vector3 gravity = new Vector3(0, -9.81f);
    public float damping = 30f;
    public float mass = 1.0f;
    public float stiffness = 100.0f;
    public float friction;
    public float staticFriction;
    public int iterations = 1;
    // Collision property
    public bool collide = false;
    public GameObject[] collideRectangles;
    public GameObject[] collisionGameobjects;
    public GameObject testPlane;

    // Collision rectangle
    public float width = 1.0f;
    public float height = 1.0f;
    private Rectangle[] rectangles;
    public float[] angle;
    // Mouse property
    public float mouseStrength;
    private int MouseIndex = -1;
    private Vector3 mousePos;

    // triangle models
    private DelaunayTriangulation delaunay = new DelaunayTriangulation();
    private IList<Triangle> triangles;
    private Node[] nodes;
    MeshGenerator meshGenerator = new MeshGenerator();
    // explosive force
    public float explosiveForce;

    // Camera properties
    private Vector3 cameraPos;
    public enum TextureTypes
    {
        DONUT = 0, ARMADILLO = 1, BUNNY = 2, SQUARE = 3
    }
    private float nodeMass;

    void Start()
    {
        collisionGameobjects = GameObject.FindGameObjectsWithTag("CollisionObject");
        if (!selected.Equals(TextureTypes.SQUARE))
        {
            triangles = InitializeTrianglesFromTexture();
            GetComponent<MeshRenderer>().material = mat;
        }
        else
        {
            triangles = InitializeSqureTriangles();
            GetComponent<MeshRenderer>().material = defaultMaterial;
        }

        Mesh mesh = meshGenerator.ConvertTrianglesToMesh(triangles, scale);
        GetComponent<MeshFilter>().mesh = mesh;

        nodeMass = mass / mesh.vertices.Length;
        nodes = InitializeNodes(mesh.vertices, mesh.triangles);
        InitializeRectangleGameobjects();
        CreateCollisionRectangles();
        StartCoroutine(UpdateScene(mesh.triangles, nodes));
    }
    private void MoveNodes(Vector3 position)
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].position += position; 
            nodes[i].initialPosition += position; 
        }
    }

    private void CreateCollisionRectangles()
    {
        for (int i = 0; i < collisionGameobjects.Length; i++)
        {
            Vector3 center = collisionGameobjects[i].transform.position;
            Vector3 scale = collisionGameobjects[i].transform.localScale;
            Vector3 A = new Vector3(center.x - scale.x/2, center.y - scale.y/2);
            Vector3 B = new Vector3(center.x - scale.x/2, center.y + scale.y/2);
            Vector3 C = new Vector3(center.x + scale.x/2, center.y + scale.y/2);
            Vector3 D = new Vector3(center.x + scale.x/2, center.y - scale.y/2);

            float rotation = collisionGameobjects[i].transform.localEulerAngles.z;
            rectangles[collideRectangles.Length + i] = new Rectangle(A, rotation, scale.x, scale.y);
        }
    }

    private IList<Triangle> InitializeTrianglesFromTexture()
    {
        IList<Triangle> triangles;
        Texture2D texture = textures[(int)selected];
        mat.SetTexture("_MainTex", texture);
        ImageConverter converter = new ImageConverter(texture, resolution, scale);
        ICollection<Vector3> points = converter.CreatePointsfromImage(detectEdges, edgeInc);
        Vector3[] pointsArr = new Vector3[points.Count];
        points.CopyTo(pointsArr, 0);
        triangles = delaunay.Triangulate(pointsArr);
        triangles = converter.CleanTriangleList(triangles);
        return triangles;

    }

    private IList<Triangle> InitializeSqureTriangles()
    {
        IList<Triangle> triangles;
        mat = new Material(Shader.Find("Sprites/Default"));
        MockPointsGenerator mockPointsGenerator = new MockPointsGenerator();
        Vector3[] points = mockPointsGenerator.CreateSquarePoints(detail, setEdges);
        triangles = delaunay.Triangulate(points);
        return triangles;
    }

    private Node[] InitializeNodes(Vector3[] vertices, int[] triangles)
    {
        Node[] nodes = new Node[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            nodes[i] = new Node(i, vertices[i]);
        }
        for (int i = 0; i < triangles.Length; i+=3)
        {
            Node node1 = nodes[triangles[i]];
            Node node2 = nodes[triangles[i+1]];
            Node node3 = nodes[triangles[i+2]];

            node1.AddNode(node2);
            node1.AddNode(node3);

            node2.AddNode(node3);
            node2.AddNode(node1);

            node3.AddNode(node1);
            node3.AddNode(node2);
        }
        return nodes;
    }

    private void InitializeRectangleGameobjects()
    {
        rectangles = new Rectangle[collideRectangles.Length + collisionGameobjects.Length];
        for (int i = 0; i < collideRectangles.Length; i++)
        {
            Renderer renderer = collideRectangles[i].GetComponent<Renderer>();
            renderer.material = defaultMaterial;
            rectangles[i] = new Rectangle(collideRectangles[i].transform.position, angle[i], width, height);
            Mesh mesh = meshGenerator.CreateRectangleMesh(rectangles[i]);
            collideRectangles[i].GetComponent<MeshFilter>().mesh = mesh;
        }
    }

    private IEnumerator UpdateScene(int[] meshTriangles, Node[] nodes)
    {
        while (true)
        {
            UpdateMouse();
            UpdateNodes();
            UpdateExplosiveForce();
            UpdateStiffness();
            UpdateFriction();
            Animate(nodes);
            UpdateCamera();
            yield return new WaitForSeconds(dt);
        }
    }

    private void UpdateFriction()
    {
        if (Input.GetKeyUp("f"))
        {
            staticFriction = staticFriction != 0.08f ? 0.08f : 0.02f;
            friction = friction != 0.05f ? 0.05f : 0.01f;
        }
    }

    private void UpdateStiffness()
    {
        if (Input.GetKeyUp("s"))
        {
            if (stiffness != 50)
            {
                stiffness = 50;
            } else
            {
                stiffness = 10;
            }
        }
    }

    private void UpdateExplosiveForce()
    {
        if (Input.GetKeyUp("space"))
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].AddVelocity(new Vector3(0, explosiveForce));
                nodes[i].Displacement = nodes[i].position;
                nodes[i].position += (nodes[i].Velocity * dt);
            }
        }
    }
    private void UpdateCamera()
    {
        Camera camera = Camera.main;
        camera.transform.position = cameraPos;
    }

    private void UpdateMouse()
    {
        MouseIndex = -1;
        if (Input.GetMouseButton(0))
        {
            Vector3 p = Input.mousePosition;
            p = Camera.main.ScreenToWorldPoint(p);

            mousePos = new Vector3(p.x, p.y);
            MouseIndex = FindClosestNode();
        }
    }

    private void UpdateNodes()
    {
        for (int currentIteration = 0; currentIteration < iterations; currentIteration++)
        {
            for (int i = 0; i < nodes.Length; i ++)
            {
                CalculateForces(nodes[i]);
            }

        for (int i = 0; i < nodes.Length; i++)
        {
            CalculateCollision(nodes[i]);
        }
    }
    }

    private void CalculateCollision(Node node)
    {
        if (testPlane != null && collide && IsCollidingWithBoundingBox(node.position))
        {
            node.position = node.Displacement;
            Vector3 velocity = node.Velocity;
     
            velocity.y = Mathf.Max(velocity.y, 0.0f);
            
            if (Mathf.Abs(velocity.x) > staticFriction)
            {
                velocity += -(velocity.normalized)* friction;
            }
            else
            {
                velocity.x = 0.0f;
            }
            node.Velocity = velocity;
            node.position += (node.Velocity * dt);
            node.Displacement = (node.Velocity * dt);
        }


        Rectangle rectangle;
        if (collide && (rectangle = IsCollidingWithRec(node.position)) != null)
        {
            Vector3 closestNormal = rectangle.GetClosestNormal(node.position);
            Vector3 edge = Vector2.Perpendicular(closestNormal);
            node.position = node.Displacement;

            // set node velocity
            Vector3 nodeVelocityY = new Vector3(0, node.Velocity.y);
            Vector3 nodePerpendicularVelocityY = Vector3.Project(nodeVelocityY, -closestNormal.normalized);
            Vector3 nodeParalelVelocityY = Vector3.Project(nodeVelocityY, edge);

            Vector3 nodeVelocityX = new Vector3(node.Velocity.x, 0);
            Vector3 nodePerpendicularVelocityX = Vector3.Project(nodeVelocityX, -closestNormal.normalized);
            Vector3 nodeParalelVelocityX = Vector3.Project(nodeVelocityX, edge);

            Vector3 nodePerpendicularVelocity = nodePerpendicularVelocityX + nodePerpendicularVelocityY;
            Vector3 nodeParalelVelocity = nodeParalelVelocityX + nodeParalelVelocityY;

            if (Vector3.Dot(nodePerpendicularVelocity, closestNormal) < 0.0f)
            {
                nodePerpendicularVelocity = Vector3.zero;
            }
            if (nodeParalelVelocity.magnitude > staticFriction)
            {
                nodeParalelVelocity += -(nodeParalelVelocity.normalized)* friction;

            }
            else
            {
                nodeParalelVelocity = Vector3.zero;
            }

            Vector3 velocity = nodeParalelVelocity + nodePerpendicularVelocity;
            Vector3 gravityVelocity = -CalculateVelocity(gravity * nodeMass);
            node.Velocity = velocity + gravityVelocity;
            node.position += (node.Velocity * dt);
            node.Displacement = (node.Velocity * dt);
        }

    }

    private Node CalculateForces(Node node)
    {
        if (node.Stationary) return node;
        Vector3 extForce = gravity * nodeMass - damping * node.Velocity;
        Vector3 force = Vector3.zero;
        List<Node> connectedNodes = node.connectedNodes;
        for (int i = 0; i < connectedNodes.Count; i++)
        {
            Vector3 currentLenght = connectedNodes[i].position - node.position;
            Vector3 dampingForce = damping * (Vector3.Project(connectedNodes[i].Velocity - node.Velocity, currentLenght));
            Vector3 springForce = stiffness * (currentLenght.magnitude - Node.GetInitialLenght(node, connectedNodes[i])) * (currentLenght / currentLenght.magnitude) + dampingForce;
            force += springForce;
        }

        Vector3 mouseForce = MoveByMouseDrag();

        force += extForce + mouseForce;
        Vector3 velocity = CalculateVelocity(force);
        Vector3 displacement = CalculateDisplacement(force);

        // Update Nodes
        node.AddVelocity(velocity);
        node.Displacement = node.position;
        node.position += (node.Velocity * dt);

        return node;
    }

    private Vector3 CalculateDisplacement(Vector3 force)
    {
        return CalculateVelocity(force) * dt;        
    }

    private Vector3 CalculateVelocity(Vector3 force)
    {
        return (force / nodeMass) * dt;
    }

    private void Animate(Node[] nodes)
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        float posX = 0;
        for (int i = 0; i < nodes.Length; i++)
        {
            vertices[i] = nodes[i].position;
            posX += nodes[i].position.x;
        }
        cameraPos = new Vector3(posX / vertices.Length, cameraPos.y,-10);
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private bool IsCollidingWithBoundingBox(Vector3 point)
    {
            Renderer planeRenderer = testPlane.GetComponent<Renderer>();
            Bounds planeBounds = planeRenderer.bounds;

            if (planeBounds.Contains(point))
            {
                return true;
            }
        return false;      
    }

    private Rectangle IsCollidingWithRec(Vector3 point)
    {
        Rectangle collidingRec = null;
        for (int i = 0; i < rectangles.Length; i++)
        {
            if (rectangles[i].IsColliding(point))
            {
                collidingRec = rectangles[i];
            }
        }
        return collidingRec;
    }

    private Vector3 MoveByMouseDrag()
    {

        if (MouseIndex != -1)
        {
            Vector3 pq = mousePos - nodes[MouseIndex].position;
            Vector3 stretch = mouseStrength * pq.normalized;
            Vector3 damp = -damping * Vector3.Dot(pq.normalized, nodes[MouseIndex].Velocity) * pq.normalized;

            return stretch + damp;
        }
        return Vector3.zero;
    }

    int FindClosestNode()
    {
        float minDistSq = float.PositiveInfinity;
        int minIndex = -1;

        for (int i = 0; i < nodes.Length; ++i)
        {
            float d = (mousePos - nodes[i].position).magnitude;

            if (d < minDistSq)
            {
                minDistSq = d;
                minIndex = i;
            }
        }
        return minIndex;

    }
}
