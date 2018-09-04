using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	// Variables
	private Camera cam;
	private GameObject[] cars;
	private float margin = 6;

	// Use this for initialization
	void Start ()
	{
		cam = GetComponent<Camera> ();
		cars = GameObject.FindGameObjectsWithTag ("Player");
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		float maxOffsetX = 0;
		float maxOffsetY = 0;

		Vector3 medianPosition = new Vector3 ();

		int inactives = 0;
		foreach (GameObject car in cars)
		{
			if (car.activeSelf == false)
			{
				inactives++;
				continue;
			}

			medianPosition += car.transform.position;

			float currOffsetX = Mathf.Abs (transform.position.x - car.transform.position.x);
			maxOffsetX = maxOffsetX > currOffsetX ? maxOffsetX : currOffsetX;

			float currOffsetY = Mathf.Abs(transform.position.y - car.transform.position.y) * cam.aspect;
			maxOffsetY = maxOffsetY > currOffsetY ? maxOffsetY : currOffsetY;
		}

		int activesCars = cars.Length - inactives;
		medianPosition = medianPosition / activesCars;
		transform.position = new Vector3 (medianPosition.x , medianPosition.y, -10);

		float offset = maxOffsetX > maxOffsetY ? maxOffsetX : maxOffsetY;
		offset = offset / 2 + margin;

		if (activesCars > 1)
		{
			float size = offset < margin ? margin : offset;
			cam.orthographicSize = size;
		}
	}
}
