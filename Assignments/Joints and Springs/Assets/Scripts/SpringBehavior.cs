using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SpringBehavior : MonoBehaviour
{
	public GameObject go1;

	List<ApplyParticle> allPoints;
	List<SpringDamper> allJoints;
	List<Triangle> allSurfaces;
	List<GameObject> allObjects;
	public float fGravity = 5.0f;
	public int height, width;

	[Range(0.0f, 100.0f)]
	public float fSpring;
	[Range(0.0f, 10.0f)]
	public float fDamping;
	public float Rest = 4;
	float diagonalRest;
	[Range(0.01f, 10.0f)]
	public float fStrength;

	public bool bWind;

	// Use this for initialization
	void Start ()
	{
		allPoints = new List<ApplyParticle>();
		allJoints = new List<SpringDamper>();
		allSurfaces = new List<Triangle>();
		allObjects = new List<GameObject>();

		spawnParticles(width, height);
		generateSprings();
		generateSurfaces();
		placeCamera();

	}

	// Update is called once per frame
	void Update()
	{
		foreach (SpringDamper i in allJoints)
		{
			i.SpringConstant = Spring;
			i.DampingFactor = Damping;
			i.RestLength = Rest;
			diagonalRest = Mathf.Sqrt(Mathf.Pow(Rest, 2) * 2);
		}
	}

	void FixedUpdate()
	{
		foreach (ApplyParticle i in allPoints)
		{
			i.temporary.Force = Vector3.down * Gravity * i.temporary.Mass;
		}

		foreach (SpringDamper i in allJoints)
		{
			i.ComputeForce();
			i.Draw();
		}

		foreach (Triangle j in allSurfaces)
		{
			if(Wind == true)
			{
				j.Aerodynamics(Vector3.forward * Strength);
			}
			
		}
	}

	void LateUpdate()
	{
		for(int i = 0; i < allPoints.Count; i++)
		{
			allObjects[i].transform.position = allPoints[i].temporary.Position;
		}

		foreach(ApplyParticle j in allPoints)
		{
			j.temporary.particleUpdate();
		}
	}

	public void spawnParticles(int w, int h)
	{
		float x = 0.0f, y = 0.0f;
		
		for (int i = 0; i < h; i++)
		{
			for (int j = 0; j < w; j++)
			{
				GameObject Temp = Instantiate(go1, new Vector3(x, -y, 0), new Quaternion()) as GameObject;

				ApplyParticle testing = Temp.GetComponent<ApplyParticle>();
				allObjects.Add(Temp);
				testing.temporary = new Particle(new Vector3(x, -y, 0), Vector3.zero, 1);
				allPoints.Add(testing.GetComponent<ApplyParticle>());

				x += Rest;
			}
			x = 0.0f;
			y += Rest;
		}
		allPoints[0].temporary.isKinematic = true;
		allPoints[w - 1].temporary.isKinematic = true;
		allPoints[allPoints.Count - 1].temporary.isKinematic = true;
		allPoints[allPoints.Count - w].temporary.isKinematic = true;
	}

	public void generateSprings()
	{
		foreach(ApplyParticle i in allPoints)
		{
			int springIndex = FindIndex(allPoints, i);
			i.temporary.allInstances = new List<Particle>();

			if ((springIndex + 1) % width > springIndex % width)
			{
				i.temporary.allInstances.Add(allPoints[springIndex + 1].temporary);
				SpringDamper sdRight = new SpringDamper(Spring, Damping, Rest, i.temporary, allPoints[springIndex + 1].temporary);
				allJoints.Add(sdRight);
			}
			if (springIndex + width < allPoints.Count)
			{
				i.temporary.allInstances.Add(allPoints[springIndex + width].temporary);
				SpringDamper sdDown = new SpringDamper(Spring, Damping, Rest, i.temporary, allPoints[springIndex + width].temporary);
				allJoints.Add(sdDown);
			}
			if ((springIndex + 1) % width > springIndex % width && springIndex + width + 1 < allPoints.Count)
			{
				i.temporary.allInstances.Add(allPoints[springIndex + width + 1].temporary);
				SpringDamper sdRD = new SpringDamper(Spring, Damping, diagonalRest, i.temporary, allPoints[springIndex + width + 1].temporary);
				allJoints.Add(sdRD);
			}
			if (springIndex + width - 1 < allPoints.Count && springIndex - 1 >= 0 && (springIndex - 1) % width < springIndex % width)
			{
				i.temporary.allInstances.Add(allPoints[springIndex + width - 1].temporary);
				SpringDamper sdLD = new SpringDamper(Spring, Damping, diagonalRest, i.temporary, allPoints[springIndex + width - 1].temporary);
				allJoints.Add(sdLD);
			}
		}
	}

	public void generateSurfaces()
	{
		foreach(ApplyParticle i in allPoints)
		{
			int triIndex = FindIndex(allPoints, i);

			if (triIndex % width != width - 1 && triIndex + width < allPoints.Count)
			{
				Triangle t = new Triangle(allPoints[triIndex], allPoints[triIndex + 1], allPoints[triIndex + width]);
				foreach(SpringDamper sd in allJoints)
				{
					if ((sd.P1 == t.P1 && sd.P2 == t.P2) || (sd.P1 == t.P2 && sd.P2 == t.P1))
					{
						t.SD1 = sd;
					}
					else if ((sd.P1 == t.P2 && sd.P2 == t.P3) || (sd.P1 == t.P3 && sd.P2 == t.P2))
					{
						t.SD2 = sd;
					}
					else if((sd.P1 == t.P3 && sd.P2 == t.P1) || (sd.P1 == t.P1 && sd.P2 == t.P3))
					{
						t.SD3 = sd;
					}
				}
				allSurfaces.Add(t);
			}
		}

		foreach(ApplyParticle i in allPoints)
		{
			int triIndex = FindIndex(allPoints, i);
			Triangle t;

			if(triIndex >= width && triIndex + 1 < allPoints.Count && triIndex % width != width - 1)
			{
				t = new Triangle(allPoints[triIndex], allPoints[triIndex + 1], allPoints[triIndex - width + 1]);
				allSurfaces.Add(t);
			}
		}
	}

	public void placeCamera()
	{
		Vector3 cameraPosition = Vector3.zero;
		foreach (ApplyParticle i in allPoints)
		{
			cameraPosition += i.temporary.Position;
		}
		cameraPosition = cameraPosition / allPoints.Count;
		cameraPosition.z = -(width * height);
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

	public bool Wind
	{
		get { return bWind; }
		set { bWind = value; }
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