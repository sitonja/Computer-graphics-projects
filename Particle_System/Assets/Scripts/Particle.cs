using UnityEngine;

public class Particle : MonoBehaviour {
	void Update () {
        Vector3 lookAtVector = transform.position - Camera.main.transform.position;
        transform.rotation = Quaternion.LookRotation(lookAtVector);
	}
}
