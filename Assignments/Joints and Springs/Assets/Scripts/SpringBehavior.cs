using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SpringBehavior : MonoBehaviour
{
	public GameObject go1;
	//public GameObject go2;

	List<ApplyParticle> allPoints;
	List<SpringDamper> allJoints;
	List<Triangle> allSurfaces;
	List<GameObject> allObjects/*, chains*/;
	public float fGravity = 5.0f;
	public int height, width;

	[Range(0.0f, 100.0f)]
	public float fSpring;
	[Range(0.0f, 10.0f)]
	public float fDamping;
	public float Rest = 4;
	//float diagonalRest;
	[Range(0.01f, 10.0f)]
	public float fStrength;

	public bool bWind;
	public bool bGravity;

	// Use this for initialization
	void Awake ()
	{
		allPoints = new List<ApplyParticle>();
		allJoints = new List<SpringDamper>();
		allSurfaces = new List<Triangle>();
		allObjects = new List<GameObject>();

		//chains = new List<GameObject>();

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
		}
	}

	void FixedUpdate()
	{
		foreach (ApplyParticle i in allPoints)
		{
			if (bGravity == true)
			{
				i.particle.Force = Vector3.down * Gravity * i.particle.Mass;
			}
			else
			{
				i.particle.Force = Vector3.down * 0;
			}
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
		//Very expensive rendering spring dampers like this
		//foreach(GameObject i in chains)
		//{
		//	int linkIndex = FindIndex(chains, i);
		//	LineRenderer lr = chains[linkIndex].GetComponent<LineRenderer>();
		//	lr.SetPosition(0, allJoints[linkIndex].P1.Position);
		//	lr.SetPosition(1, allJoints[linkIndex].P2.Position);
		//}
		
		//foreach(GameObject k in allObjects)
		//{
		//	int pointIndex = FindIndex(allObjects, k);
		//	allObjects[pointIndex].transform.position = allPoints[pointIndex].particle.Position;
		//}

		foreach(ApplyParticle j in allPoints)
		{
			int pointIndex = FindIndex(allPoints, j);
			allObjects[pointIndex].transform.position = allPoints[pointIndex].particle.Position;
			j.particle.particleUpdate();
		}
	}

	public void spawnParticles(int w, int h)
	{
		float x = 0.0f, y = 0.0f;
		
		for (int i = 0; i < h; i++)
		{
			for (int j = 0; j < w; j++)
			{
				GameObject go = Instantiate(go1, new Vector3(x, -y, 0), new Quaternion()) as GameObject;

				ApplyParticle spawned = go.GetComponent<ApplyParticle>();
				allObjects.Add(go);
				go.name = "Particle " + (allObjects.Count - 1).ToString();
				spawned.particle = new Particle(new Vector3(x, -y, 0), Vector3.zero, 1);
				allPoints.Add(spawned.GetComponent<ApplyParticle>());

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
				//chains.Add(drawSprings(sdRight));
				allJoints.Add(sdRight);
			}
			if (springIndex + width < allPoints.Count)
			{
				i.particle.allInstances.Add(allPoints[springIndex + width].particle);
				SpringDamper sdDown = new SpringDamper(Spring, Damping, Rest, i.particle, allPoints[springIndex + width].particle);
				//chains.Add(drawSprings(sdDown));
				allJoints.Add(sdDown);
			}
			if ((springIndex + 1) % width > springIndex % width && springIndex + width + 1 < allPoints.Count)
			{
				i.particle.allInstances.Add(allPoints[springIndex + width + 1].particle);
				SpringDamper sdRD = new SpringDamper(Spring, Damping, Rest, i.particle, allPoints[springIndex + width + 1].particle);
				//chains.Add(drawSprings(sdRD));
				allJoints.Add(sdRD);
			}
			if (springIndex + width - 1 < allPoints.Count && springIndex - 1 >= 0 && (springIndex - 1) % width < springIndex % width)
			{
				i.particle.allInstances.Add(allPoints[springIndex + width - 1].particle);
				SpringDamper sdLD = new SpringDamper(Spring, Damping, Rest, i.particle, allPoints[springIndex + width - 1].particle);
				//chains.Add(drawSprings(sdLD));
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
				Triangle surf = new Triangle(allPoints[triIndex], allPoints[triIndex + 1], allPoints[triIndex + width]);
				foreach(SpringDamper sd in allJoints)
				{
					if ((sd.P1 == surf.P1 && sd.P2 == surf.P2) || (sd.P1 == surf.P2 && sd.P2 == surf.P1))
					{
						surf.SD1 = sd;
					}
					else if ((sd.P1 == surf.P2 && sd.P2 == surf.P3) || (sd.P1 == surf.P3 && sd.P2 == surf.P2))
					{
						surf.SD2 = sd;
					}
					else if((sd.P1 == surf.P3 && sd.P2 == surf.P1) || (sd.P1 == surf.P1 && sd.P2 == surf.P3))
					{
						surf.SD3 = sd;
					}
				}
				allSurfaces.Add(surf);
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

	//public GameObject drawSprings(SpringDamper temp)
	//{
	//	GameObject linkDraw = Instantiate(go2, (temp.P1.Position + temp.P2.Position) / 2.0f, new Quaternion()) as GameObject;
	//	linkDraw.name = "Link " + (chains.Count).ToString();
	//	LineRenderer lr = linkDraw.GetComponent<LineRenderer>();
	//	//lr.materials[0].color = Color.black;
	//	lr.materials[0].color = new Color(0, 0, 0, 255);
		
	//	lr.SetWidth(0.1f, 0.1f);
	//	return linkDraw;
	//}

	public void placeCamera()
	{
		Vector3 cameraPosition = Vector3.zero;
		foreach (ApplyParticle i in allPoints)
		{
			cameraPosition += i.particle.Position;
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

	public int FindIndex(List<GameObject> ObjectList, GameObject anObject)
	{
		int index = 0;

		for (int i = 0; i < ObjectList.Count; i++)
		{
			if (ObjectList[i] == anObject)
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