using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SpringBehavior : MonoBehaviour
{
	//public GameObject camera;

	List<Particle> allPoints;
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
		allPoints = new List<Particle>();
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
		foreach (Particle i in allPoints)
		{
			i.Force = Vector3.down * Gravity * i.Mass;
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
			allObjects[i].transform.position = allPoints[i].Position;
		}

		foreach(Particle j in allPoints)
		{
			j.particleUpdate();
		}
	}

	public void spawnParticles(int w, int h)
	{
		float x = 0.0f, y = 0.0f;

		for (int i = 0; i < h; i++)
		{
			for (int j = 0; j < w; j++)
			{
				GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);

				Particle p = new Particle(new Vector3(x, -y, 0), Vector3.zero, 1.0f);
				go.transform.position = p.Position;
				allObjects.Add(go);
				go.name = "Particle::" + (allObjects.Count - 1).ToString();
				allPoints.Add(p);

				x += Rest;
			}
			x = 0.0f;
			y += Rest;
		}
		allPoints[0].isKinematic = true;
		allPoints[w - 1].isKinematic = true;
		allPoints[allPoints.Count - 1].isKinematic = true;
		allPoints[allPoints.Count - w].isKinematic = true;
	}

	public void generateSprings()
	{
		for (int i = 0; i < allPoints.Count; i++)
		{
			if ((i + 1) % width > i % width)
			{
				SpringDamper sdRight = new SpringDamper(Spring, Damping, Rest, allPoints[i], allPoints[i + 1]);
				allJoints.Add(sdRight);
			}

			if (i + width < allPoints.Count)
			{
				SpringDamper sdDown = new SpringDamper(Spring, Damping, Rest, allPoints[i], allPoints[i + width]);
				allJoints.Add(sdDown);
			}

			if ((i + 1) % width > i % width && i + width + 1 < allPoints.Count)
			{
				SpringDamper sdRD = new SpringDamper(Spring, Damping, diagonalRest, allPoints[i], allPoints[i + width + 1]);
				allJoints.Add(sdRD);
			}

			if (i + width < allPoints.Count && i - 1 >= 0  && (i - 1) % width < i % width)
			{
				SpringDamper sdLD = new SpringDamper(Spring, Damping, diagonalRest, allPoints[i], allPoints[i + width - 1]);
				allJoints.Add(sdLD);
			}
		}
	}

	public void generateSurfaces()
	{
		for (int i = 0; i < allPoints.Count; i++)
		{
			if (i % width != width - 1 && i + width < allPoints.Count)
			{
				Triangle t = new Triangle(allPoints[i], allPoints[i + 1], allPoints[i + width]);
				foreach (SpringDamper sd in allJoints)
				{
					if ((sd.P1 == t.P1 && sd.P2 == t.P2) || (sd.P1 == t.P2 && sd.P2 == t.P2))
					{
						t.SD1 = sd;
					}

					else if ((sd.P1 == t.P2 && sd.P2 == t.P3) || (sd.P1 == t.P3 && sd.P2 == t.P3))
					{
						t.SD2 = sd;
					}

					else if ((sd.P1 == t.P3 && sd.P2 == t.P1) || (sd.P2 == t.P3))
					{
						t.SD3 = sd;
					}
				}
				allSurfaces.Add(t);
			}

			if (i >= width && i + 1 < allPoints.Count && i % width != width - 1)
			{
				Triangle temp = new Triangle(allPoints[i], allPoints[i + 1], allPoints[i - width + 1]);
				allSurfaces.Add(temp);
			}
		}
	}

	public void placeCamera()
	{
		Vector3 cameraPosition = Vector3.zero;
		foreach (Particle i in allPoints)
		{
			cameraPosition += i.Position;
		}
		cameraPosition = cameraPosition / allPoints.Count;
		cameraPosition.z = -(width * height);
		Camera.main.transform.position = cameraPosition;
	}
	
	public int FindIndex(List<Particle> PointList, Particle aPoint)
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