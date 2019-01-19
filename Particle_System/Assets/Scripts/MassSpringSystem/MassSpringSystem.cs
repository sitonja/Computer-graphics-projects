using System.Collections;
using UnityEngine;

public class MassSpringSystem : MonoBehaviour {
    public float mass = 10.0f;
    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    public float k = 2.0f;
    public float damping = 5;
    public Vector3 initialForce = new Vector3(2.0f , 0, 0);
    public Vector3 initialVelocity = new Vector3(5.0f, 0, 0); 
    
    public GameObject wall;
    public GameObject nodeGameobject;

    private ParticleModel node;
    private Vector3 initialPosition;
    private Vector3 currentForce;
    private void Start()
    {
        currentForce = initialForce;
        initialPosition = new Vector3(0, 0.5f, 0);
        node = new ParticleModel(nodeGameobject, 0);
        node.velocity = initialVelocity;
        StartCoroutine(AnimateSimpleSpring());

    }

    private IEnumerator AnimateSimpleSpring()
    {
        while(true)
        {
            Vector3 displacement = -initialPosition + nodeGameobject.transform.position;
            Vector3 force = -k * displacement + initialForce - damping * node.velocity;
            Vector3 acceleration = force / mass;
            node.velocity += acceleration * Time.deltaTime;

            
            Vector3 distance = Time.deltaTime * node.velocity;
            nodeGameobject.transform.position += distance;            
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }


    void OnDrawGizmos()
    {
        if (wall != null && nodeGameobject != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(wall.transform.position, nodeGameobject.transform.position);
        }
    }
}
