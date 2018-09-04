using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
	// Components
	private Rigidbody2D rigidbody2D;

	// Controller Variables
	public int mode;
	private KeyCode KeyUp;
	private KeyCode KeyDown;
	private KeyCode KeyLeft;
	private KeyCode KeyRight;

	// Car Variables
	private Vector2 speed;
	public float basePower;
	public float maxspeed;
	public float turnpower;
	public bool onRoad;
	public float friction;
	public float frictionOnRoad;
	public float frictionOnGrass;

	// Sound
	private AudioSource audioSource;
	public AudioClip move;

	// PowerUps
	private PowerUpType activePowerup = PowerUpType.None;
	private float basePowerOffset = 0;
	private float maxSpeedOffset = 0;
	private float turnPowerOffset = 0;
	private int shieldCharges = 0;

	private bool hasPowerUp = false;
	private float timeLeft = 0f;

	// needed for initial countdown
	private float time;

	// Initialization
	void Start ()
	{
		// Get Components
		rigidbody2D = GetComponent<Rigidbody2D>();
		audioSource = GetComponent<AudioSource> ();

		// Configure Audio
		audioSource.clip = move;
		audioSource.volume = 0.2f;

		// Configure Controllers
		ConfigureControllers();

		// Init Variables
		basePower = 900f;
		maxspeed = 6f;
		turnpower = 2f;
		onRoad = true;
		friction = 3f;
		frictionOnRoad = 3f;
		frictionOnGrass = 15f;

		// needed for initial countdown
		time = 0;
	}

	void ConfigureControllers ()
	{
		switch (mode)
		{
		case 0:

			KeyUp = KeyCode.W;
			KeyDown = KeyCode.S;
			KeyLeft = KeyCode.A;
			KeyRight = KeyCode.D;

			break;

		case 1:

			KeyUp = KeyCode.UpArrow;
			KeyDown = KeyCode.DownArrow;
			KeyLeft = KeyCode.LeftArrow;
			KeyRight = KeyCode.RightArrow;

			break;

		default:

			Debug.LogError ("UNKOWN CONFIGURATION MODE");

			break;
		}
	}

	// Update
	void Update ()
	{

	}

	// Fixed Update
	void FixedUpdate()
	{
		if(time < 4f)
		{
			time += Time.deltaTime;
			return;
		}

		if(timeLeft > 0)
		{
			timeLeft -= Time.deltaTime;
			if(timeLeft < 0)
			{
				timeLeft = 0;
				UnsetPowerUp();
			}
		}

		float power = (basePower + basePowerOffset) * Time.deltaTime;
		friction = onRoad ? frictionOnRoad : frictionOnGrass;

		speed = new Vector2(rigidbody2D.velocity.x, rigidbody2D.velocity.y);

		float maxSpeed = maxspeed + maxSpeedOffset;
		float turnPower = turnpower + turnPowerOffset;
		if(speed.magnitude > maxSpeed)
		{
			speed = speed.normalized;
			speed *= maxspeed + maxSpeed;
		}

		if(Input.GetKey(KeyUp))
		{
			rigidbody2D.AddForce(transform.up * power);
			rigidbody2D.drag = friction;
		}

		if(Input.GetKey(KeyDown))
		{
			rigidbody2D.AddForce(-transform.up * (power / 2));
			rigidbody2D.drag = friction;
		}

		if(Input.GetKey(KeyLeft))
		{
			rigidbody2D.angularVelocity = 0;
			transform.Rotate(Vector3.forward * turnPower);
		}

		if(Input.GetKey(KeyRight))
		{
			rigidbody2D.angularVelocity = 0;
			transform.Rotate(Vector3.forward * -turnPower);
		}

		if(!(Input.GetKey(KeyUp) || Input.GetKey(KeyDown)))
		{
			audioSource.Stop();
			rigidbody2D.drag = friction * 2;
		}
		else
		{
			if(!audioSource.isPlaying)
			{
				audioSource.Play();
			}
		}

	}

	// Events
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.name == "Tilemap_Road")
		{
			onRoad = true;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.name == "Tilemap_Road")
		{
			onRoad = false;
		}
	}

	// Getters and Setters
	public bool isOnRoad()
	{
		return onRoad;
	}

	public PowerUpType getActivePowerUp()
	{
		return activePowerup;
	}

	public bool GetMissileProtection()
	{
		return shieldCharges-- > 0;
	}

	public void SetPowerUp(PowerUpType type, float powerupTime)
	{
		if(hasPowerUp)
		{
			UnsetPowerUp();
		}

		activePowerup = type;

		switch(type)
		{
			case PowerUpType.SpeedUp:
				basePowerOffset = basePower * 0.5f;
				maxSpeedOffset = maxspeed;
				break;
			case PowerUpType.SpeedDown:
				basePowerOffset = -basePower * 0.5f;
				maxSpeedOffset = -maxspeed * 0.5f;
				break;
			case PowerUpType.ReverseControllers:
				turnPowerOffset = -turnpower * 2.0f;
				break;
			case PowerUpType.MissileShield:
				shieldCharges = 2;
				break;
		default:
			break;
		}

		timeLeft = powerupTime;
	}

	private void UnsetPowerUp()
	{
		basePowerOffset = 0;
		maxSpeedOffset = 0;
		turnPowerOffset = 0;
		shieldCharges = 0;
		activePowerup = PowerUpType.None;
		hasPowerUp = false;
	}
}