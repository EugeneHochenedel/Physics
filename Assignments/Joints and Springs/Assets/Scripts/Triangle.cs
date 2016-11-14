using UnityEngine;
using System.Collections;

public class Triangle
{
	//public float Density, DragCoefficient; //rho, Cd both are constants
	public float CrossSectionalArea; //a
	public Vector3 SurfaceNormal, RelativeVelocity; //n, v

	public Particle P1, P2, P3;
	public SpringDamper SD1, SD2, SD3;

	public Triangle() { }

	public Triangle(Particle first, Particle second, Particle third)
	{
		P1 = first;
		P2 = second;
		P3 = third;
	}

	public void Aerodynamics(Vector3 airVelocity)
	{
		Vector3 averageVelocity = (P1.Velocity + P2.Velocity + P3.Velocity) / 3;
		//Vector3 airVelocity = Vector3.forward * 0.5f;
		RelativeVelocity = averageVelocity - airVelocity;

		Vector3 crossNorm = Vector3.Cross(P2.Position - P1.Position, P3.Position - P1.Position);
		SurfaceNormal = crossNorm / crossNorm.magnitude;

		float a0 = 0.5f * crossNorm.magnitude;
		CrossSectionalArea = a0 * (Vector3.Dot(RelativeVelocity, SurfaceNormal) / RelativeVelocity.magnitude);
		
		float Aero = 1.0f * Mathf.Pow(RelativeVelocity.magnitude, 2.0f);
		Vector3 Dynamics = 1.0f * CrossSectionalArea * SurfaceNormal;

		Vector3 AeroForce = -0.5f * (Aero * Dynamics);

		P1.addForce(AeroForce / 3);
		P2.addForce(AeroForce / 3);
		P3.addForce(AeroForce / 3);
	}
}
// (|v|^2) * a * n = ((|v| * (v . n*)) / (2 * |n|)) * n*
// n* = crossNorm