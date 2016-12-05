using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ParticleProperties;

public class Particle : IParticles
{
	Vector3 vecPosition; //position += velocity * Time.deltaTime
	Vector3 vecVelocity; //velocity += a * Time.deltaTime
	Vector3 vecAcceleration; //acceleration = (1/m)f
	float fMass; //mass = p/v
	Vector3 vecMomentum; //momentum = mv
	Vector3 vecForce; //force

	public List<Particle> allInstances;

	public bool isKinematic;

	public Particle() { }

	public Particle(Vector3 pos, Vector3 vel, float m)
	{
		vecPosition = pos;
		vecVelocity = vel;
		fMass = m;
		vecForce = Vector3.zero;
		vecMomentum = Vector3.zero;
	}

	public Vector3 Position
	{
		get { return vecPosition; }
		set { vecPosition = value; }
	}

	public Vector3 Velocity
	{
		get { return vecVelocity; }
		set { vecVelocity = value; }
	}

	public Vector3 Force
	{
		get { return vecForce; }
		set { vecForce = value; }
	}

	public float Mass
	{
		get { return fMass; }
		set { fMass = value; }
	}

	//public void addForce(Vector3 forces)
	//{
	//	Force += forces;
	//}

	public bool addForce(Vector3 forces)
	{
		if(forces.magnitude > 0.0)
		{
			Force += forces;
			return true;
		}
		else
		{
			Force = Force;
			return false;
		}
	}

	public Vector3 particleUpdate()
	{
		if (isKinematic == true)
		{
			Force = Vector3.zero;
		}

		else
		{
			vecAcceleration = (1.0f / fMass) * Force;
			vecVelocity += (vecAcceleration * Time.fixedDeltaTime);
			vecVelocity = Vector3.ClampMagnitude(vecVelocity, vecVelocity.magnitude);
			vecPosition += vecVelocity * Time.fixedDeltaTime;
		}
		
		return vecPosition;
		
	}
}
