using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
	public float turretRange = 10f;
	private GameObject[] cars;
	private TurretWeaponController missileSpawner;
	private Transform turretBase;

	// Use this for initialization
	void Start ()
	{
		missileSpawner = transform.GetChild (1).gameObject.GetComponent<TurretWeaponController> ();
		turretBase = transform.GetChild (0);
	}

	// Update is called once per frame
	void Update ()
	{
		cars = GameObject.FindGameObjectsWithTag ("Player");
		GameObject closerCar = null;

		float minDistance = 0;

		if (cars.Length == 1)
		{
			GameObject car = cars [0];
			minDistance = (transform.position - car.transform.position).magnitude;
			closerCar = car;
		}
		else
		{
			foreach (GameObject car in cars)
			{
				float distance = (transform.position - car.transform.position).magnitude;

				if ((closerCar == null) || (distance < minDistance))
				{
					closerCar = car;
					minDistance = distance;
				}
			}
		}
			
		if (minDistance < turretRange && minDistance > 1f)
		{
			missileSpawner.SetInRange (true);
		}
		else
		{
			missileSpawner.SetInRange (false);
		}

		if (minDistance > 1f)
		{
			Quaternion rotation = Quaternion.LookRotation (closerCar.transform.position - transform.position, transform.TransformDirection (-Vector3.forward));
			transform.rotation = new Quaternion (0, 0, rotation.z, rotation.w);
			turretBase.transform.rotation = Quaternion.Euler (0, 0, 0);
		}
	}
}