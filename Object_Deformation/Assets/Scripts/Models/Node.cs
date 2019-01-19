
using System.Collections.Generic;
using UnityEngine;

public class Node {
    private Vector3 force;
    public Vector3 Force
    {
        get { return force; }
        set
        {
            force = value;
        }
    }

    private Vector3 velocity;
    public Vector3 Velocity {
        get { return velocity; }
        set { velocity = value; } }
    public Vector3 Displacement { get; set; }
    private int index;
    public bool Stationary { get; set; }
    public Vector3 initialPosition;
    public Vector3 position;

    public List<Node> connectedNodes;

    public Node(int index, Vector3 initialPosition)
    {
        connectedNodes = new List<Node>();
        this.index = index;
        Velocity = new Vector3();
        Force = new Vector3();
        Displacement = new Vector3();
        this.initialPosition = initialPosition;
        position = initialPosition;
    }

    public void AddNode(Node node1)
    {
        connectedNodes.Add(node1);
    }

    public override bool Equals(object obj)
    {
        var node = obj as Node;
        return node != null &&
               index == node.index;
    }

    public override int GetHashCode()
    {
        return -1982729373 + index.GetHashCode();
    }

    public void AddVelocity(Vector3 velocity)
    {
        Velocity += velocity;
    }

    public void AddDisplacement(Vector3 displacement)
    {
        Displacement += displacement;
    }

    public void SetStationary(bool stationary)
    {
        Stationary = stationary;
    }

    public Vector3 GetDisplacement()
    {
        return (-initialPosition + position);
    }

    public static float GetInitialLenght(Node node1, Node node2)
    {
        return (node1.initialPosition - node2.initialPosition).magnitude;
    }

    public void SetForceX( float forceX)
    {
        force.x = forceX;
    }

    public void SetForceY(float forceY)
    {
        force.y = forceY;
    }

    public void SetVelocityX(float velocityX)
    {
        velocity.x = velocityX;
    }

    public void SetVelocityY(float velocityY)
    {
        velocity.y = velocityY;
    }
}
