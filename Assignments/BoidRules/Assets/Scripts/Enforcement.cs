using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enforcement : MonoBehaviour
{
    public GameObject anAgent;

    int boidCount = 25;

    public Vector3 target;

    Vector3 Cohesion;
    Vector3 Dispersion;
    Vector3 Alignment;

    Vector3 Tendency;

    Vector3 Boundary;

    [Range(0.0f, 1.0f)]
    public float Rule1Ex, Rule2Ex, Rule3Ex, Rule5Ex;

    Vector3 pos;

    List<Agent> allAgents;

    void Awake()
    {
        allAgents = new List<Agent>();
        pos = Vector3.zero;
        for(int i = 0; i < boidCount; i++)
        {
            pos.x = Random.Range(-40, 40);
            pos.y = Random.Range(-40, 40);
            pos.z = Random.Range(-40, 40);

            GameObject spawned = Instantiate(anAgent, pos, new Quaternion()) as GameObject;

            Agent parts = spawned.GetComponent<Agent>();
            parts.Position = pos;
            parts.Velocity = parts.Position.normalized;
            parts.transform.parent = transform;

            allAgents.Add(parts);
        }
    }

    public float Rule1Strength
    {
        get { return Rule1Ex; }
        set { Rule1Ex = value; }
    }

    public float Rule2Strength
    {
        get { return Rule2Ex; }
        set { Rule2Ex = value; }
    }

    public float Rule3Strength
    {
        get { return Rule3Ex; }
        set { Rule3Ex = value; }
    }

    public float Rule5Strength
    {
        get { return Rule5Ex; }
        set { Rule5Ex = value; }
    }

    Vector3 CohesionRule(Agent eachAgent)
    {
        Vector3 perceivedCenter = Vector3.zero;
        foreach (Agent i in allAgents)
        {
            if (i != eachAgent)
            {
                perceivedCenter += i.transform.position;
            }
        }
        perceivedCenter = perceivedCenter / (allAgents.Count - 1);
        return (perceivedCenter - eachAgent.Position).normalized / 50;
    }

    Vector3 DispersionRule(Agent eachAgent)
    {
        Vector3 displace = Vector3.zero;

        foreach (Agent i in allAgents)
        {
            if (i != eachAgent)
            {
                if ((i.transform.position - eachAgent.Position).magnitude < 30)
                {
                    displace -= (i.transform.position - eachAgent.Position);
                }
            }
        }
        return displace.normalized;
    }

    Vector3 AlignmentRule(Agent eachAgent)
    {
        Vector3 perceivedVelocity = Vector3.zero;

        foreach (Agent i in allAgents)
        {
            if (i != eachAgent)
            {
                perceivedVelocity += i.Velocity;
            }
        }
        perceivedVelocity = perceivedVelocity / (allAgents.Count - 1);

        Mathf.Clamp01(perceivedVelocity.x);
        Mathf.Clamp01(perceivedVelocity.y);
        Mathf.Clamp01(perceivedVelocity.z);

        return (perceivedVelocity - eachAgent.Velocity).normalized / 8;
    }

    Vector3 TendencyRule(Agent eachAgent)
    {
        Vector3 tendTowards = (target - eachAgent.Position).normalized / 100;

        return tendTowards;
    }

    Vector3 BoundaryRule(Agent eachAgent)
    {
        Vector3 limitation = Vector3.zero;
        if (eachAgent.Position.x > 40)
        {
            limitation.x -= 1;
        }
        else if (eachAgent.Position.x < -40)
        {
            limitation.x += 1;
        }
        if (eachAgent.Position.y > 40)
        {
            limitation.y -= 1;
        }
        else if (eachAgent.Position.y < -40)
        {
            limitation.y += 1;
        }
        if (eachAgent.Position.z > 40)
        {
            limitation.z -= 1;
        }
        else if (eachAgent.Position.z < -40)
        {
            limitation.z += 1;
        }
        return limitation;
    }

    void SpeedRule(Agent eachAgent)
    {
        float vlim = 0.5f;

        if (eachAgent.Velocity.magnitude > vlim)
        {
            eachAgent.Velocity = (eachAgent.Velocity / eachAgent.Velocity.magnitude).normalized * vlim;
        }
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        foreach (Agent i in allAgents)
        {
            Cohesion = CohesionRule(i) * Rule1Strength;
            Dispersion = DispersionRule(i) * Rule2Strength;
            Alignment = AlignmentRule(i) * Rule3Strength;
            Tendency = TendencyRule(i) * Rule5Strength;
            Boundary = BoundaryRule(i);
            if (Rule5Ex == 0)
            {
                i.Velocity += Cohesion + Dispersion + Alignment + Boundary;
            }

            else
            {
                i.Velocity += Cohesion + Dispersion + Alignment + Tendency + Boundary;
            }
            //i.Velocity += Cohesion + Dispersion + Alignment + Tendency + Boundary;
            SpeedRule(i);
            //i.bond.Position = i.bond.Position + i.bond.Velocity;
        }
    }
}
