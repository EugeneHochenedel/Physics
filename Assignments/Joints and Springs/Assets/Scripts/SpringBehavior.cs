using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SpringBehavior : MonoBehaviour
{
	public GameObject clothBoid;

	private List<ApplyParticle> allPoints;
	private List<SpringDamper> allJoints;
	private List<Triangle> allSurfaces;
	private List<LineRenderer> allLines;

	[Range(0.1f, 9.8f)]
	public float fGravity;

	public int height, width;

	[Range(0.0f, 50.0f)]
	public float fSpring;
	[Range(0.0f, 10.0f)]
	public float fDamping;
	public float Rest = 4;

	[Range(0.01f, 10.0f)]
	public float fStrength;
	[Range(0.0f, 20.0f)]
	public float tearPoint;

	public bool bWind;
	public bool bGravity;

	// Use this for initialization
	private void Awake ()
	{
		allPoints = new List<ApplyParticle>();
		allJoints = new List<SpringDamper>();
		allSurfaces = new List<Triangle>();
		allLines = new List<LineRenderer>();

		spawnParticles(width, height);
		generateSprings();
		generateSurfaces();
		placeCamera();
	}

	// Update is called once per frame
	private void Update()
	{
		foreach (SpringDamper i in allJoints)
		{
			i.SpringConstant = Spring;
			i.DampingFactor = Damping;
			i.RestLength = Rest;
		}	
	}

	private void FixedUpdate()
	{
		List<SpringDamper> secondaryDamper = new List<SpringDamper>();
		List<Triangle> secondarySurface = new List<Triangle>();

		foreach (SpringDamper sd in allJoints)
		{
			secondaryDamper.Add(sd);
		}

		foreach(Triangle t in allSurfaces)
		{
			secondarySurface.Add(t);
		}

		foreach (ApplyParticle i in allPoints)
		{
			if(GravTog == false)
			{
				i.particle.Force = Vector3.zero;
			}
			else
			{
				i.particle.Force = Vector3.down * Gravity * (Gravity * 0.5f) * i.particle.Mass;
			}
		}

		foreach (SpringDamper j in secondaryDamper)
		{
			j.ComputeForce();
			clothTearing(j);
		}

		foreach(Triangle k in secondarySurface)
		{
			if(Wind == true)
			{
				if (!allJoints.Contains(k.FirstLine) || !allJoints.Contains(k.SecondLine) || !allJoints.Contains(k.ThirdLine))
				{
					allSurfaces.Remove(k);
				}
				else
				{
					k.Aerodynamics(Vector3.forward * Strength);
				}
			}
		}
	}

	private void LateUpdate()
	{
		foreach (ApplyParticle i in allPoints)
		{
			Bounds(i);

			i.transform.position = i.particle.Position;
			i.particle.particleUpdate();
		}

		foreach (SpringDamper i in allJoints)
		{
			int linkIndex = FindIndex(allJoints, i);
			allLines[linkIndex].SetPosition(0, allJoints[linkIndex].partOne.Position);
			allLines[linkIndex].SetPosition(1, allJoints[linkIndex].partTwo.Position);
		}
	}

	public void spawnParticles(int w, int h)
	{
		float x = 0.0f, y = 0.0f;
		
		for (int i = 0; i < h; i++)
		{
			for (int j = 0; j < w; j++)
			{
				GameObject go = Instantiate(clothBoid, new Vector3(x, -y, 0), new Quaternion()) as GameObject;

				ApplyParticle spawned = go.GetComponent<ApplyParticle>();
				
				spawned.particle = new Particle(new Vector3(x, -y, 0), Vector3.zero, 5);
				allPoints.Add(spawned);
				go.name = "Particle " + (allPoints.Count - 1).ToString();
				spawned.transform.parent = transform;

				x += Rest;
			}
			x = 0.0f;
			y += Rest;
		}
		allPoints[0].particle.isKinematic = true;
		allPoints[w - 1].particle.isKinematic = true;
		allPoints[allPoints.Count - 1].particle.isKinematic = true;
		allPoints[allPoints.Count - w].particle.isKinematic = true;
	}

	public void generateSprings()
	{
		foreach(ApplyParticle i in allPoints)
		{
			int springIndex = FindIndex(allPoints, i);
			i.particle.allInstances = new List<Particle>();

			if ((springIndex + 1) % width > springIndex % width)
			{
				i.particle.allInstances.Add(allPoints[springIndex + 1].particle);
				SpringDamper sdRight = new SpringDamper(Spring, Damping, Rest, i.particle, allPoints[springIndex + 1].particle);
				allJoints.Add(sdRight);
			}

			if (springIndex + width < allPoints.Count)
			{
				i.particle.allInstances.Add(allPoints[springIndex + width].particle);
				SpringDamper sdDown = new SpringDamper(Spring, Damping, Rest, i.particle, allPoints[springIndex + width].particle);
				allJoints.Add(sdDown);
			}

			if ((springIndex + 1) % width > springIndex % width && springIndex + width + 1 < allPoints.Count)
			{
				i.particle.allInstances.Add(allPoints[springIndex + width + 1].particle);
				SpringDamper sdRD = new SpringDamper(Spring, Damping, Rest, i.particle, allPoints[springIndex + width + 1].particle);
				allJoints.Add(sdRD);
			}

			if (springIndex + width - 1 < allPoints.Count && springIndex - 1 >= 0 && (springIndex - 1) % width < springIndex % width)
			{
				i.particle.allInstances.Add(allPoints[springIndex + width - 1].particle);
				SpringDamper sdLD = new SpringDamper(Spring, Damping, Rest, i.particle, allPoints[springIndex + width - 1].particle);
				allJoints.Add(sdLD);
			}
		}

		foreach(SpringDamper i in allJoints)
		{
			int sdIndex = FindIndex(allJoints, i);
			GameObject linkDraw = new GameObject();
			LineRenderer lr = linkDraw.AddComponent<LineRenderer>();

			linkDraw.transform.position = (allJoints[sdIndex].partOne.Position + allJoints[sdIndex].partTwo.Position) / 2;

			lr.SetPosition(0, allJoints[sdIndex].partOne.Position);
			lr.SetPosition(1, allJoints[sdIndex].partTwo.Position);
			lr.material.color = Color.black;
			lr.SetWidth(0.25f, 0.25f);
			allLines.Add(lr);
			linkDraw.name = "Link " + (allLines.Count).ToString();
			lr.transform.parent = transform;
		}
	}

	public void generateSurfaces()
	{
		foreach(ApplyParticle i in allPoints)
		{
			int triIndex = FindIndex(allPoints, i);
			Triangle surf;

			if (triIndex % width != width - 1 && triIndex + width < allPoints.Count)
			{
				surf = new Triangle(allPoints[triIndex], allPoints[triIndex + 1], allPoints[triIndex + width]);
				foreach(SpringDamper sd in allJoints)
				{
					if ((sd.partOne == surf.FirstPoint && sd.partTwo == surf.SecondPoint) || (sd.partOne == surf.SecondPoint && sd.partTwo == surf.FirstPoint))
					{
						surf.FirstLine = sd;
					}
					if ((sd.partOne == surf.SecondPoint && sd.partTwo == surf.ThirdPoint) || (sd.partOne == surf.ThirdPoint && sd.partTwo == surf.SecondPoint))
					{
						surf.SecondLine = sd;
					}
					if((sd.partOne == surf.ThirdPoint && sd.partTwo == surf.FirstPoint) || (sd.partOne == surf.FirstPoint && sd.partTwo == surf.ThirdPoint))
					{
						surf.ThirdLine = sd;
					}
				}
				allSurfaces.Add(surf);
			}

			else if(triIndex >= width && triIndex + 1 < allPoints.Count && triIndex % width != width - 1)
			{
				surf = new Triangle(allPoints[triIndex], allPoints[triIndex + 1], allPoints[triIndex - width + 1]);
				allSurfaces.Add(surf);
			}
		}
	}

	public void clothTearing(SpringDamper torn)
	{
		if ((torn.partTwo.Position - torn.partOne.Position).magnitude > (Rest * tearPoint) / (0.03f * fSpring))
		{
			if ((torn.partTwo.allInstances.Contains(torn.partOne)))
			{
				torn.partTwo.allInstances.Remove(torn.partOne);
				Destroy(allLines[allJoints.IndexOf(torn)].gameObject);
				allLines.Remove(allLines[allJoints.IndexOf(torn)]);
				allJoints.Remove(torn);
			}
			if (torn.partOne.allInstances.Contains(torn.partTwo))
			{
				torn.partOne.allInstances.Remove(torn.partTwo);
				Destroy(allLines[allJoints.IndexOf(torn)].gameObject);
				allLines.Remove(allLines[allJoints.IndexOf(torn)]);
				allJoints.Remove(torn);
			}
		}
	}

	private void Bounds(ApplyParticle blocked)
	{
		float forceX = blocked.particle.Force.x, forceY = blocked.particle.Force.y, forceZ = blocked.particle.Force.z;
		float velX = blocked.particle.Velocity.x, velY = blocked.particle.Velocity.y, velZ = blocked.particle.Velocity.z;

		if (Camera.main.WorldToScreenPoint(blocked.particle.Position).x < 15.0f)
		{
			if(blocked.particle.Force.x < 0.0f)
			{
				blocked.particle.Force = new Vector3(0, forceY, forceZ);
			}
			blocked.particle.Velocity = new Vector3(-velX, velY, velZ) * 0.65f;
		}

		if (Camera.main.WorldToScreenPoint(blocked.particle.Position).x > Screen.width - 15.0f)
		{
			if (blocked.particle.Force.x > 0.0f)
			{
				blocked.particle.Force = new Vector3(0, forceY, forceZ);
			}
			blocked.particle.Velocity = new Vector3(-velX, velY, velZ) * 0.65f;
		}

		if (Camera.main.WorldToScreenPoint(blocked.particle.Position).y < 15.0f)
		{
			if (blocked.particle.Force.y < 0.0f)
			{
				blocked.particle.Force = new Vector3(forceX, 0, forceZ);
			}
			blocked.particle.Velocity = new Vector3(velX, -velY, velZ) * 0.65f;
		}

		if (Camera.main.WorldToScreenPoint(blocked.particle.Position).y > Screen.height - 15.0f)
		{
			if (blocked.particle.Force.y > 0.0f)
			{
				blocked.particle.Force = new Vector3(forceX, 0, forceZ);
			}
			blocked.particle.Velocity = new Vector3(velX, -velY, velZ) * 0.65f;
		}
	}

	public void placeCamera()
	{
		Vector3 cameraPosition = Vector3.zero;
		foreach (ApplyParticle i in allPoints)
		{
			cameraPosition += i.particle.Position;
		}
		cameraPosition = cameraPosition / allPoints.Count;
		cameraPosition.z = -(width * height) * 2;
		Camera.main.transform.position = cameraPosition;
	}
	
	public int FindIndex(List<ApplyParticle> PointList, ApplyParticle aPoint)
	{
		int index = 0;

		for (int i = 0; i < PointList.Count; i++)
		{
			if (PointList[i] == aPoint)
			{
				index = i;
				break;
			}
		}
		return index;
	}

	public int FindIndex(List<SpringDamper> SpringList, SpringDamper aSpringDamper)
	{
		int index = 0;

		for (int i = 0; i < SpringList.Count; i++)
		{
			if (SpringList[i] == aSpringDamper)
			{
				index = i;
				break;
			}
		}
		return index;
	}

	public int FindIndex(List<Triangle> TriangleList, Triangle aTriangle)
	{
		int index = 0;

		for (int i = 0; i < TriangleList.Count; i++)
		{
			if (TriangleList[i] == aTriangle)
			{
				index = i;
				break;
			}
		}
		return index;
	}

	//Allows for UI elements to access values in this class
	public bool Wind
	{
		get { return bWind; }
		set { bWind = value; }
	}
	public bool GravTog
	{
		get { return bGravity; }
		set { bGravity = value; }
	}
	public float Spring
	{
		get { return fSpring; }
		set { fSpring = value; }
	}
	public float Damping
	{
		get { return fDamping; }
		set { fDamping = value; }
	}
	public float Gravity
	{
		get { return fGravity; }
		set { fGravity = value; }
	}
	public float Strength
	{
		get { return fStrength; }
		set { fStrength = value; }
	}
}