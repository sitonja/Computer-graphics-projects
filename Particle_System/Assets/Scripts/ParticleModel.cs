using UnityEngine;

public class ParticleModel {
    public Vector3 velocity;
    public GameObject particle;
    public int index;
    public ParticleModel(Vector3 velocity, GameObject particle, int index) 
    {
        this.index = index;
        this.velocity = velocity;
        this.particle = particle;
    }

    public ParticleModel(GameObject particle, int index)
    {
        this.index = index;
        this.particle = particle;
    }

    public override bool Equals(object obj)
    {
        var model = obj as ParticleModel;
        return model != null &&
               index == model.index;
    }

    public override int GetHashCode()
    {
        return -1982729373 + index.GetHashCode();
    }
}
