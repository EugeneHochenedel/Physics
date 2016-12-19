using UnityEngine;
using System.Collections;

public class Triangle
{
	//Fields
	private float fCrossSectionalArea; //a
	private Vector3 vecSurfaceNormal, vecRelativeVelocity; //n, v

	private Particle P1, P2, P3;
	private SpringDamper SD1, SD2, SD3;

	public Triangle() { }

	public Triangle(ApplyParticle first, ApplyParticle second, ApplyParticle third)
	{
		P1 = first.particle;
		P2 = second.particle;
		P3 = third.particle;
	}

	public Triangle(Particle first, Particle second, Particle third)
	{
		P1 = first;
		P2 = second;
		P3 = third;
	}

	//Properties
	public float CrossSection
	{
		get { return fCrossSectionalArea; }
		set { fCrossSectionalArea = value; }
	}

	public Vector3 SurfaceNorm
	{
		get { return vecSurfaceNormal; }
		set { vecSurfaceNormal = value; }
	}

	public Vector3 RelativeVel
	{
		get { return vecRelativeVelocity; }
		set { vecRelativeVelocity = value; }
	}

	public Particle FirstPoint
	{
		get { return P1; }
		set { P1 = value; }
	}

	public Particle SecondPoint
	{
		get { return P2; }
		set { P2 = value; }
	}

	public Particle ThirdPoint
	{
		get { return P3; }
		set { P3 = value; }
	}

	public SpringDamper FirstLine
	{
		get { return SD1; }
		set { SD1 = value; }
	}
	public SpringDamper SecondLine
	{
		get { return SD2; }
		set { SD2 = value; }
	}
	public SpringDamper ThirdLine
	{
		get { return SD3; }
		set { SD3 = value; }
	}

	//Function
	public void Aerodynamics(Vector3 airVelocity)
	{
		Vector3 averageVelocity = (P1.Velocity + P2.Velocity + P3.Velocity) / 3;
		
		vecRelativeVelocity = averageVelocity - airVelocity;

		Vector3 crossNorm = Vector3.Cross(P2.Position - P1.Position, P3.Position - P1.Position);
		vecSurfaceNormal = crossNorm / crossNorm.magnitude;
		
		float a0 = 0.5f * crossNorm.magnitude;
		fCrossSectionalArea = a0 * (Vector3.Dot(vecRelativeVelocity, vecSurfaceNormal) / vecRelativeVelocity.magnitude);
		
		float Aero = 1.0f * Mathf.Pow(vecRelativeVelocity.magnitude, 2.0f);
		Vector3 Dynamics = 1.0f * fCrossSectionalArea * vecSurfaceNormal;

		Vector3 AeroForce = -0.5f * (Aero * Dynamics);

		P1.addForce(AeroForce / 3);
		P2.addForce(AeroForce / 3);
		P3.addForce(AeroForce / 3);
	}
}
// (|v|^2) * a * n = ((|v| * (v . n*)) / (2 * |n|)) * n*
// (|v|^2) * a * n = ((vecRelativeVelocity.magnitude * Vector3.dot(vecRelativeVelocity, crossNorm)) / (2 * crossNorm.magnitude)) * crossNorm
// n* = crossNorm