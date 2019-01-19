using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemManager : MonoBehaviour {
    public float spawnRate = 0.5f;
    public float mass = 55;
    public float duration = 1;
    public int max = 10;
    public float velocity = 1f;
    public Vector3 gravity = new Vector3 (0, -9.81f, 0);
    public bool loop = false;
    public float bounce = 10.0f;
    public float damping = 2.0f;
    public GameObject particle;

    public GameObject collisionPlane; 

    private List<ParticleModel> particles = new List<ParticleModel>();

    private int deadParticles = 0;
    private Vector3 groundForce;

    void Awake()
    {
        groundForce = -gravity * mass;
        // object pooling
        for (int i = 0; i < max; i++)
        {
            GameObject particleGameobject = Instantiate(particle);
            particleGameobject.SetActive(false);
            ParticleModel particleModel = new ParticleModel(Random.onUnitSphere * velocity, particleGameobject, i);
            particles.Add(particleModel);
        }
    }
    // Use this for initialization
    void Start () {
        StartCoroutine(CreateParticles());
        StartCoroutine(AnimateParticle());
    }

    private IEnumerator CreateParticles()
    {
        int particlesCreated = 0;
        while (particlesCreated < max || loop)
        {
            ParticleModel currentParticle = CreateParticleModel();

            StartCoroutine(CheckParticleLifeCycle(currentParticle.index));

            if (!loop)
            {
                particlesCreated++;
            }
            yield return new WaitForSeconds(spawnRate);
        }
    }

    private IEnumerator AnimateParticle()
    {
        while(deadParticles < max || loop)
        {
            yield return new WaitUntil(() => IsParticleActive());
            List<ParticleModel> activeParticles = GetActiveParticles();


            for (int i = 0; i < activeParticles.Count; i++)
            {
                Vector3 force = gravity * mass - damping * activeParticles[i].velocity;
                Vector3 velocity = activeParticles[i].velocity + (force / mass) * Time.deltaTime;
                Vector3 distance = Time.deltaTime * velocity;
                Vector3 nextPosition = activeParticles[i].particle.transform.position + distance;
                if (IsColliding(activeParticles[i].particle))
                {
                    force = Vector3.Reflect(activeParticles[i].velocity, collisionPlane.transform.up) * bounce * mass - damping * activeParticles[i].velocity ;
                    velocity = (force / mass) * Time.deltaTime;
                    distance = Time.deltaTime * velocity;
                    nextPosition = activeParticles[i].particle.transform.position + distance;
                }
                activeParticles[i].velocity = velocity;
                activeParticles[i].particle.transform.position = nextPosition;
            }

            yield return new WaitForSeconds(Time.deltaTime);

        }
    }
    private IEnumerator CheckParticleLifeCycle(int particleIndex)
    {
        yield return new WaitForSeconds(duration);
        particles[particleIndex].particle.SetActive(false);

        if (!loop)
        {
            deadParticles++;
        }
    }


    private List<ParticleModel> GetActiveParticles()
    {
        List<ParticleModel> activeParticles = particles.FindAll( particle => particle.particle.activeSelf);
        return activeParticles;
    }

    private ParticleModel GetHiddenParticle()
    {
        return particles.Find( particleModel => !particleModel.particle.activeSelf);
    }

    private bool IsParticleActive()
    {
        return particles.Exists(particle => particle.particle.activeSelf);
    }

    private bool AllParticlesActive()
    {
        return !particles.Exists(particle => !particle.particle.activeSelf);
    }

    private ParticleModel CreateParticleModel()
    {
        ParticleModel currentParticle;
        if (AllParticlesActive())
        {
            GameObject particleGameobject = Instantiate(particle);
            currentParticle = new ParticleModel(Random.onUnitSphere * velocity, particleGameobject, particles.Count);
            particles.Add(currentParticle);

        } else
        {
            currentParticle = GetHiddenParticle();
        }
        currentParticle.particle.transform.position = transform.position;
        currentParticle.velocity = Random.onUnitSphere * velocity;
        currentParticle.particle.SetActive(true);

        return currentParticle;
    }

    private bool IsColliding(GameObject particle)
    {
        Renderer particleRenderer = particle.GetComponent<Renderer>();
        Bounds particleBounds = particleRenderer.bounds;

        Renderer planeRenderer = collisionPlane.GetComponent<Renderer>();
        Bounds planeBounds = planeRenderer.bounds;
        

        return planeBounds.Intersects(particleBounds);
    }
}
