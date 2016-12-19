using UnityEngine;
using System.Collections;

public class SpringDamper
{
	//Fields
	private float fSpringConstant, fDampingFactor; //Ks, Kd
	private float fRestLength; //l0;

	private Particle P1, P2;

	public SpringDamper() { }

	public SpringDamper(float Ks, float Kd, float L0, Particle first, Particle second)
	{
		fSpringConstant = Ks;
		fDampingFactor = Kd;
		fRestLength = L0;
		P1 = first;
		P2 = second;
	}

	//Properties
	public float SpringConstant
	{
		get { return fSpringConstant; }
		set { fSpringConstant = value; }
	}

	public float DampingFactor
	{
		get { return fDampingFactor; }
		set { fDampingFactor = value; }
	}

	public float RestLength
	{
		get { return fRestLength; }
		set { fRestLength = value; }
	}

	public Particle partOne
	{
		get { return P1; }
		set { P1 = value; }
	}

	public Particle partTwo
	{
		get { return P2; }
		set { P2 = value; }
	}

	//Functions
	public void ComputeForce()
	{
		Vector3 eStar = P2.Position - P1.Position;
		float l = eStar.magnitude;
		Vector3 e = eStar / l;

		float vel1 = Vector3.Dot(e, P1.Velocity);
		float vel2 = Vector3.Dot(e, P2.Velocity);

		float SpringForceLinear = -fSpringConstant * (fRestLength - l);
		float DamperForce = -fDampingFactor * (vel1 - vel2);

		Vector3 SpringForce1 = (SpringForceLinear + DamperForce) * e;
		Vector3 SpringForce2 = -SpringForce1;

		P1.addForce(SpringForce1);
		P2.addForce(SpringForce2);
	}

	//Comment from the instructor
	//remember to let classes represent an "is a" relationship
}