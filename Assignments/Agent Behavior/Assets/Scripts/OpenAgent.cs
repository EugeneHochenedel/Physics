using UnityEngine;
using System.Collections;
using Boid;

public class OpenAgent : MonoBehaviour
{
    public Agent bond;
    public float Mass;

	// Use this for initialization
	void Start ()
    {
        bond = new Agent(Mass);
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
        bond.velocityUpdate();
        transform.position = bond.Position;
	}
}
