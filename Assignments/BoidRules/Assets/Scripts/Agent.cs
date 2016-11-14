using UnityEngine;
using System.Collections;
using Boid;

public class Agent : MonoBehaviour, IBoidRules
{
    public Vector3 vecVelocity;
    public Vector3 vecPos;
    public float fMass;

    public Agent(float fM)
    {
        Velocity = new Vector3();
        Position = new Vector3();
        Mass = (fM <= 0) ? 1 : fM;
    }

    public Vector3 Velocity
    {
        get { return vecVelocity; }
        set { vecVelocity = value; }
    }
    
    public Vector3 Position
    {
        get { return vecPos; }
        set { vecPos = value; }
    }

    public float Mass
    {
        get { return fMass; }
        set { fMass = value; }
    }

    public void velocityUpdate()
    {
        Position += Velocity;
    }

    void Update()
    {
        velocityUpdate();
        transform.position = Position;
    }
}
