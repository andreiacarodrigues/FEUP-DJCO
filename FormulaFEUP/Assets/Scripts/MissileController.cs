using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : MonoBehaviour
{
	public GameObject explosion;
	private SoundController soundController;
	private float movementSpeed = 12.5f;
	private float lifetime = 5f;
	private static float knockbackPower = 400f;
	private float extraKnockbackPower = 0f;
	private GameObject parentCar;
	private Transform missileTarget = null;
	float speedFactor = 2.5f;

	// Use this for initialization
	void Start ()
	{
		soundController = FindObjectOfType<SoundController> ().GetComponent<SoundController> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if((missileTarget != null) && (lifetime <= 4.75f))
		{
			Vector3 vectorToTarget =  transform.position - missileTarget.position;

			float speed = speedFactor;
			if (vectorToTarget.magnitude < 2f)
			{
				speed *= 4;
			}

			float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
			Quaternion q = Quaternion.AngleAxis(angle + 90f, Vector3.forward);
			transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * speed);
		}

		transform.position += transform.up * Time.deltaTime * movementSpeed;

		if (Time.deltaTime >= lifetime)
		{
			Destroy (gameObject);
		}
		else
		{
			lifetime -= Time.deltaTime;
		}
	}

	public static void Configure(Difficulty difficulty)
	{
		switch (difficulty)
		{
		case Difficulty.Easy:

			knockbackPower = 200f;

			break;

		case Difficulty.Medium:

			knockbackPower = 400f;

			break;

		case Difficulty.Hard:

			knockbackPower = 600f;

			break;
		}
	}

	// Events
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject == parentCar)
		{
			return;
		}

		CarController carController = other.gameObject.GetComponent<CarController> ();
		if (carController == null)
		{
			return;
		}

		float carMult = 1;
		if (parentCar != null)
		{
			carMult = 1.5f;
		}

		Instantiate (explosion, transform.position, Quaternion.identity);
		soundController.PlayImpact ();

		if(carController.GetMissileProtection())
		{
			Destroy(gameObject);
			return;
		}

		float newKnockbackPower = knockbackPower + extraKnockbackPower;
		if (carController.isOnRoad ())
		{
			other.gameObject.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (transform.up.x, transform.up.y) * newKnockbackPower * carMult);
		}
		else
		{
			other.gameObject.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (transform.up.x, transform.up.y) * newKnockbackPower/3f * carMult);
		}
		Destroy (gameObject);
	}

	// Getters and Setters
	public GameObject getParentCar()
	{
		return parentCar;
	}

	public void setParentCar(GameObject parentCar)
	{
		this.parentCar = parentCar;
	}

	public void SetExtraKnockbackPower(float knockbackPower)
	{
		extraKnockbackPower = knockbackPower;
	}

	public void SetTarget(Transform target)
	{
		missileTarget = target;
	}
}