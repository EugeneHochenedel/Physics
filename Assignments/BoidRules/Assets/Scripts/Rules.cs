using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rules : MonoBehaviour
{
    public GameObject anAgent;

    public Vector3 velocity;
    public float mass = 30;

   

    int boidCount = 10;
    float maxSpawnDistance = 12.5f;

    Vector3 Cohesion;
    Vector3 Dispersion;
    Vector3 Alignment;

    [Range(0.1f, 1.0f)]
    public float Rule1Ex;
    [Range(0.0f, 1.0f)]
    public float Rule2Ex;
    [Range(0.0f, 0.35f)]
    public float Rule3Ex;

    List<Rules> allBoids;

	// Use this for initialization
	void Start ()
    {
        allBoids = new List<Rules>();
        Vector3 pos = Vector3.zero;
        for(int i = 0; i < boidCount; i++)
        {
            pos.x = Random.Range(-maxSpawnDistance, maxSpawnDistance);
            pos.y = Random.Range(-maxSpawnDistance, maxSpawnDistance);
            pos.z = Random.Range(-maxSpawnDistance, maxSpawnDistance);

            GameObject spawned = Instantiate(anAgent, pos, new Quaternion()) as GameObject;

            Rules law = spawned.GetComponent<Rules>();
            law.velocity = law.transform.position.normalized;
            law.mass = Random.Range(0.1f, mass);
            law.transform.parent = transform;

            allBoids.Add(this);
        }
	}

    /*Vector3 AllRules(Rules boidRule)
    {
        Vector3 percievedCenter = Vector3.zero;
        Vector3 displace = Vector3.zero;
        Vector3 perceivedVelocity = Vector3.zero;

        foreach (Rules i in allBoids)
        {
            if (i != boidRule)
            {
                percievedCenter += i.transform.position;
                perceivedVelocity += i.velocity;
                if ((i.transform.position - boidRule.transform.position).magnitude < 2 * Rule2Ex)
                {
                    displace -= (i.transform.position - boidRule.transform.position);
                }
            }
        }
        percievedCenter = percievedCenter / (allBoids.Count - 1);
        perceivedVelocity = perceivedVelocity / (allBoids.Count - 1);

        Cohesion = (percievedCenter - boidRule.transform.position).normalized / Rule1Ex;
        Dispersion = displace.normalized;
        Alignment = (perceivedVelocity - boidRule.velocity).normalized * Rule3Ex;

        return Cohesion + Dispersion + Alignment;
    }*/
	
    Vector3 CohesionCalc(Rules boidRule)
    {
        Vector3 percievedCenter = Vector3.zero;
        
        foreach(Rules i in allBoids)
        {
            if (i != boidRule)
            {
                percievedCenter += i.transform.position;
            }
        }
        percievedCenter = percievedCenter / (allBoids.Count - 1);

        return (percievedCenter - boidRule.transform.position).normalized / 100;
    }

    Vector3 DispersionCalc(Rules boidRule)
    {
        Vector3 displace = Vector3.zero;

        foreach (Rules i in allBoids)
        {
            if (i != boidRule)
            {
                if ((i.transform.position - boidRule.transform.position).magnitude < 2)
                {
                    displace -= (i.transform.position - boidRule.transform.position);
                }
            }
        }

        return displace.normalized;
    }

    Vector3 AlignmentCalc(Rules boidRule)
    {
        Vector3 perceivedVelocity = Vector3.zero;

        foreach (Rules i in allBoids)
        {
            if (i != boidRule)
            {
                perceivedVelocity += i.velocity;
            }
        }
        perceivedVelocity = perceivedVelocity / (allBoids.Count - 1);

        return (perceivedVelocity - boidRule.velocity).normalized / 8;
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        foreach(Rules i in allBoids)
        {
            Cohesion = CohesionCalc(i) * Rule1Ex;

            Dispersion = DispersionCalc(i) * Rule2Ex;

            Alignment = AlignmentCalc(i) * Rule3Ex;

            //i.velocity += Cohesion + Dispersion + Alignment;
        }
        velocity += Cohesion + Dispersion + Alignment;
        
	}

    void LateUpdate()
    {
        //velocity += Cohesion + Dispersion + Alignment;

        transform.position += velocity.normalized;
        transform.forward = velocity.normalized;
    }
}
