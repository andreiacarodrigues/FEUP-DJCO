using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarWeaponController : MonoBehaviour
{
	public GameObject missile;
	public GameObject shotImage;
	private float cooldown = .8f;
	private float cooldownOffset = 0f;
	private float cooldownTimer = 0f;
	private float extraKnockback = 0f;
	private bool homing = false;
	private bool hasPowerUp = false;
	private float timeLeft = 0f;
	public KeyCode shootKey;
	public SoundController soundController;
	public CarController carController;
	private Transform enemy;

	// needed for initial countdown
	private float time;

	// Use this for initialization
	void Start ()
	{
		time = 0;
		GameController gameController = FindObjectOfType<GameController>();
		if(carController == gameController.blueCar)
		{
			enemy = gameController.redCar.transform;
		}
		else
		{
			enemy = gameController.blueCar.transform;
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		if(time < 4f)
		{
			time += Time.deltaTime;
			return;
		}

		if(cooldownTimer > 0)
		{
			cooldownTimer -= Time.deltaTime;
			if(cooldownTimer < 0)
			{
				cooldownTimer = 0;
			}
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

		if(Input.GetKey(shootKey) && cooldownTimer == 0)
		{
			// Shot
			Transform parent = gameObject.transform.parent;
			GameObject createdMissile = Instantiate(missile, transform.position, parent.rotation);
			MissileController missileController = createdMissile.GetComponent<MissileController>();
			missileController.setParentCar(parent.gameObject);
			missileController.SetExtraKnockbackPower(extraKnockback);
			if(homing)
			{
				missileController.SetTarget(enemy);
			}
			cooldownTimer = cooldown + cooldownOffset;

			// Shot Image
			GameObject shotImageCreated =
				Instantiate(shotImage, transform.position, parent.rotation * Quaternion.Euler(0, 0, 180));
			shotImageCreated.gameObject.transform.parent = gameObject.transform;

			// Shot Sound
			soundController.PlayShot();
		}
	}

	// Getters and Setters
	public float GetCooldownTimer()
	{
		return cooldownTimer;
	}

	public float GetCooldown()
	{
		return cooldown + cooldownOffset;
	}

	public void SetPowerUp(PowerUpType type, float powerupTime)
	{
		if(hasPowerUp)
		{
			UnsetPowerUp();
		}

		switch(type)
		{
		case PowerUpType.HigherKnockback:
			extraKnockback = 100f;
			break;
		case PowerUpType.LowerReload:
			cooldownOffset = -cooldown * 0.5f;
			break;
		case PowerUpType.HomingMissiles:
			homing = true;
			break;
		default:
			break;
		}

		timeLeft = powerupTime;
	}

	private void UnsetPowerUp()
	{
		cooldownOffset = 0;
		extraKnockback = 0;
		homing = false;
		hasPowerUp = false;
	}
}