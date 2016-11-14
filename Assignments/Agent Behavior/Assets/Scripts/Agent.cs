using UnityEngine;
using Boid;

public class Agent : IBoid
{
    private Vector3 vecVelocity;
    private Vector3 vecPos;
    private float fMass;

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
}
