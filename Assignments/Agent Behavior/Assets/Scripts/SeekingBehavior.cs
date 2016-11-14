using UnityEngine;
using System.Collections;

public class SeekingBehavior : MonoBehaviour
{
    OpenAgent j;

    public Transform targetPosition;
    Vector3 Steer;
    Vector3 targetVelocity;
    public float Seek;

	// Use this for initialization
    void Start()
    {
        j = gameObject.GetComponent<OpenAgent>();
    }
	void FixedUpdate ()
    {
        targetVelocity = (targetPosition.position - transform.position).normalized;
        Steer = (targetVelocity - j.bond.Velocity).normalized * Seek;
        j.bond.Velocity += Steer / j.bond.Mass;

        if (j.bond.Velocity.magnitude > 5)
        {
            j.bond.Velocity = j.bond.Velocity.normalized;
        }
    }
}

//public vector3 Force
//F = vec3.right
//position = position + (velocity / mass)
//seek = (desiredVelocity - currentVelocity).normalized
//steering *= seek + avoid + ...
//velocity = velocity + steering