using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Particle
{
	//Fields
	private Vector3 vecPosition; //position += velocity * Time.deltaTime
	private Vector3 vecVelocity; //velocity += a * Time.deltaTime
	private Vector3 vecAcceleration; //acceleration = (1/m)f
	private float fMass; //mass = p/v
	private Vector3 vecForce; //force

	public List<Particle> allInstances;

	public bool isKinematic;

	public Particle() { }

	public Particle(Vector3 pos, Vector3 vel, float m)
	{
		vecPosition = pos;
		vecVelocity = vel;
		fMass = m;
		vecForce = Vector3.zero;
	}

	//Properties
	public Vector3 Position
	{
		get { return vecPosition; }
		set { vecPosition = value; }
	}

	public Vector3 Velocity
	{
		get { return Vector3.ClampMagnitude(vecVelocity, 20); }
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

	//Functions
	public bool addForce(Vector3 forces)
	{
		if(forces.magnitude > 0.0)
		{
			Force += forces;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Calculates how the position of this particle is changing,
	/// Based on if it's kinematic, and its velocity.
	/// The velocity is determined by the force being applied to this particle
	/// And the resulting acceleration.
	/// </summary>
	/// <returns>position of this particle</returns>
	public Vector3 particleUpdate()
	{
		if (isKinematic == true)
		{
			vecForce = Vector3.zero;
		}

		else
		{
			vecAcceleration = (1.0f / fMass) * Vector3.ClampMagnitude(vecForce, 25);
			vecVelocity += (vecAcceleration * Time.fixedDeltaTime);
			vecPosition += vecVelocity * Time.fixedDeltaTime;
		}
		
		return vecPosition;	
	}

	//Comments from the instructor
	//why use a property vs a field?
	//when to use?
	//if you don't know then at the very least be consistent

	//Difference between Time.deltaTime and Time.fixedDeltaTime
	//Time.deltaTime respects a time scale
	//Time.fixedDeltaTime is a pre-set amount of time between frames
}
