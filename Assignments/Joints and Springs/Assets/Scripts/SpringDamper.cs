using UnityEngine;
using System.Collections;

public class SpringDamper
{
	public float SpringConstant, DampingFactor; //Ks, Kd
	public float RestLength; //l0;

	public Particle P1, P2;

	public SpringDamper()
	{
	}

	public SpringDamper(float Ks, float Kd, float L0, Particle first, Particle second)
	{
		SpringConstant = Ks;
		DampingFactor = Kd;
		RestLength = L0;
		P1 = first;
		P2 = second;
	}

	public void ComputeForce()
	{
		Vector3 eStar = P2.Position - P1.Position;
		float l = eStar.magnitude;
		Vector3 e = eStar / l;

		float vel1 = Vector3.Dot(e, P1.Velocity);
		float vel2 = Vector3.Dot(e, P2.Velocity);

		float SpringForceLinear = -SpringConstant * (RestLength - l);
		float DamperForce = -DampingFactor * (vel1 - vel2);

		Vector3 SpringForce1 = (SpringForceLinear + DamperForce) * e;
		Vector3 SpringForce2 = -SpringForce1;

		P1.addForce(SpringForce1);
		P2.addForce(SpringForce2);
	}

	//public void Draw()
	//{
	//	Debug.DrawLine(P1.Position, P2.Position, Color.black);
	//}

	public bool threadTearing(float tF)
	{
		if ((P2.Position - P1.Position).magnitude > (RestLength * tF) / (0.03f * SpringConstant))
		{
			if (P2.allInstances.Contains(P1))
			{
				P2.allInstances.Remove(P1);
			}
			if (P1.allInstances.Contains(P2))
			{
				P1.allInstances.Remove(P2);
			}
			return true;
		}
		return false;
	}
}