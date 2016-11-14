using UnityEngine;
using System.Collections;

public class Destination : MonoBehaviour
{
    public GameObject targetPre;
    public int agentCount;
    public float maxDistance;
    public float minimumMass;
    public float maximumMass;
    [Range(0.1f, 1.5f)]
    public float steeringBehavior;
    public float fRad;

	// Use this for initialization
	void Start ()
    {
        steeringBehavior = 1;
        for(int i = 0; i < agentCount; i++)
        {

        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
