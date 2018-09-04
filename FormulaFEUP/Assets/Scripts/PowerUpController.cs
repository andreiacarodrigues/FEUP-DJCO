using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum PowerUpType
{
	SpeedUp,
	SpeedDown,
	ReverseControllers,
	HigherKnockback,
	LowerReload,
	HomingMissiles,
	MissileShield,
	None
}

public class PowerUpController : MonoBehaviour
{
	private PersistentController pc;
	private GameController gameController;
	private CheckpointsController checkpointsController;
	private SpriteRenderer spriteRenderer;
	private SoundController soundController;

	private const float cooldown = 20;
	private bool inactive = false;
	private float cooldownLeft = 0;

	// Use this for initialization
	void Start ()
	{
		gameController = FindObjectOfType<GameController>();
		checkpointsController = FindObjectOfType<CheckpointsController>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		soundController = FindObjectOfType<SoundController> ().GetComponent<SoundController> ();
	}

	void Update()
	{
		if(inactive)
		{
			cooldownLeft -= Time.deltaTime;
			if(cooldownLeft <= 0)
			{
				SetActive();
			}
		}
	}

	// Events
	private void OnTriggerEnter2D(Collider2D other)
	{
		if(inactive)
		{
			return;
		}
			
		if(!other.gameObject.CompareTag("Player"))
		{
			return;
		}
		CarController carController = other.gameObject.GetComponent<CarController>();
		CarWeaponController carWeaponController = other.gameObject.GetComponentInChildren<CarWeaponController>();
		Player player;
		if(carController == gameController.blueCar)
		{
			player = Player.Blue;
		}
		else
		{
			player = Player.Red;
		}

		Position position = checkpointsController.PlayerPosition(player);

		double randomValue = Random.value;

		switch(position)
		{
			case Position.Ahead:
			if(randomValue < 0.25)
			{
				float time = 2f;
				carController.SetPowerUp(PowerUpType.SpeedUp, time);
				carWeaponController.SetPowerUp(PowerUpType.SpeedUp, time);
				Debug.Log ("Speed Up");
			}
			else if(randomValue < 0.60)
			{
				float time = 4f;
				carController.SetPowerUp(PowerUpType.SpeedDown, time);
				carWeaponController.SetPowerUp(PowerUpType.SpeedDown, time);
				Debug.Log ("Speed Down");
			}
			else if(randomValue < 0.95)
			{
				float time = 2f;
				carController.SetPowerUp(PowerUpType.ReverseControllers, time);
				carWeaponController.SetPowerUp(PowerUpType.ReverseControllers, time);
				Debug.Log ("Reverse Controllers");
			}
			else
			{
				float time = 5f;
				carController.SetPowerUp(PowerUpType.MissileShield, time);
				carWeaponController.SetPowerUp(PowerUpType.MissileShield, time);
				Debug.Log ("Missile Shield");
			}
			break;
		case Position.Tied:
			if(randomValue < 0.3)
			{
				float time = 3f;
				carController.SetPowerUp(PowerUpType.SpeedUp, time);
				carWeaponController.SetPowerUp(PowerUpType.SpeedUp, time);
				Debug.Log ("Speed Up");
			}
			else if(randomValue < 0.45)
			{
				float time = 3f;
				carController.SetPowerUp(PowerUpType.SpeedDown, time);
				carWeaponController.SetPowerUp(PowerUpType.SpeedDown, time);
				Debug.Log ("Speed Down");
			}
			else if(randomValue < 0.55)
			{
				float time = 5f;
				carController.SetPowerUp(PowerUpType.HigherKnockback, time);
				carWeaponController.SetPowerUp(PowerUpType.HigherKnockback, time);
				Debug.Log ("Higher Knockback");
			}
			else if(randomValue < 0.7)
			{
				float time = 1f;
				carController.SetPowerUp(PowerUpType.ReverseControllers, time);
				carWeaponController.SetPowerUp(PowerUpType.ReverseControllers, time);
				Debug.Log ("Reverse Controllers");
			}
			else if(randomValue < 0.9)
			{
				float time = 3f;
				carController.SetPowerUp(PowerUpType.LowerReload, time);
				carWeaponController.SetPowerUp(PowerUpType.LowerReload, time);
				Debug.Log ("Lower Reload");
			}
			else
			{
				float time = 5f;
				carController.SetPowerUp(PowerUpType.MissileShield, time);
				carWeaponController.SetPowerUp(PowerUpType.MissileShield, time);
				Debug.Log ("Missile Shield");
			}
			break;
		case Position.Behind:
			if(randomValue < 0.3)
			{
				float time = 4f;
				carController.SetPowerUp(PowerUpType.SpeedUp, time);
				carWeaponController.SetPowerUp(PowerUpType.SpeedUp, time);
				Debug.Log ("Speed Up");
			}
			else if(randomValue < 0.4)
			{
				float time = 1.5f;
				carController.SetPowerUp(PowerUpType.SpeedDown, time);
				carWeaponController.SetPowerUp(PowerUpType.SpeedDown, time);
				Debug.Log ("Speed Down");
			}
			else if(randomValue < 0.5)
			{
				float time = 5f;
				carController.SetPowerUp(PowerUpType.HigherKnockback, time);
				carWeaponController.SetPowerUp(PowerUpType.HigherKnockback, time);
				Debug.Log ("Higher Knockback");
			}
			else if(randomValue < 0.6)
			{
				float time = 1f;
				carController.SetPowerUp(PowerUpType.ReverseControllers, time);
				carWeaponController.SetPowerUp(PowerUpType.ReverseControllers, time);
				Debug.Log ("Reverse Controllers");
			}
			else if(randomValue < 0.9)
			{
				float time = 4f;
				carController.SetPowerUp(PowerUpType.LowerReload, time);
				carWeaponController.SetPowerUp(PowerUpType.LowerReload, time);
				Debug.Log ("Lower Reload");
			}
			else
			{
				float time = 2f;
				carController.SetPowerUp(PowerUpType.HomingMissiles, time);
				carWeaponController.SetPowerUp(PowerUpType.HomingMissiles, time);
				Debug.Log ("Homing Missile");
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
			
		soundController.PlayPowerup ();
		SetInactive();
	}

	private void SetInactive()
	{
		spriteRenderer.enabled = false;
		inactive = true;
		cooldownLeft = cooldown;
	}

	private void SetActive()
	{
		spriteRenderer.enabled = true;
		inactive = false;
	}
}