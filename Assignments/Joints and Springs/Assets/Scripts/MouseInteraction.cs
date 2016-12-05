using UnityEngine;
using System.Collections;

public class MouseInteraction : MonoBehaviour
{
	public GameObject selected;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		dragging();
		anchoring();
	}

	void dragging()
	{
		if (Input.GetButton("Fire1"))
		{
			if (Project() != null && Project().GetComponent<ApplyParticle>() != null)
			{
				selected = Project();

				selected.GetComponent<ApplyParticle>().particle.Force = Vector3.zero;
				selected.GetComponent<ApplyParticle>().particle.Velocity = Vector3.zero;

				Vector3 cursor = Input.mousePosition;
				cursor.z = -Camera.main.transform.position.z;

				Vector3 area = Camera.main.ScreenToWorldPoint(cursor);
				area.z = selected.transform.position.z;

				selected.GetComponent<ApplyParticle>().particle.Position = area;
				selected.transform.position = area;
			}
		}

		if (Input.GetButtonUp("Fire1"))
		{
			selected = null;
		}
	}
	void anchoring()
	{
		if (Input.GetButtonDown("Fire2"))
		{
			if (Project() != null && Project().GetComponent<ApplyParticle>() != null)
			{
				selected = Project();

				//Checks if isKinematic is true.
				//If isKinematic is true then sets it to false and vice versa
				selected.GetComponent<ApplyParticle>().particle.isKinematic = (selected.GetComponent<ApplyParticle>().particle.isKinematic == true) ? false : true;
			}
		}

		if (Input.GetButtonUp("Fire2"))
		{
			selected = null;
		}
	}

	public GameObject Project()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit = new RaycastHit();
		Physics.Raycast(ray.origin, ray.direction, out hit);

		if (hit.transform != null)
		{
			return hit.transform.gameObject;
		}
		return null;
	}
}
