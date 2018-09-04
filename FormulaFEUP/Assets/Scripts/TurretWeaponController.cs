using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretWeaponController : MonoBehaviour
{
	public GameObject missile;
	public GameObject shotImage;
	private bool inRange = false;
	private float cooldown = 3f;
	private float cooldownTimer = 1.5f;
	public SoundController soundController;

	private float time;

	// Use this for initialization
	void Start ()
	{
		time = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(time >= 4f)
		{
			if (cooldownTimer > 0)
			{
				cooldownTimer -= Time.deltaTime;
				if (cooldownTimer < 0)
				{
					cooldownTimer = 0;
				}
			}

			if (cooldownTimer == 0 && inRange)
			{
				// Missile
				Transform parent = gameObject.transform.parent;
				Instantiate (missile, transform.position, parent.rotation);
				cooldownTimer = cooldown;

				// Shot Image
				GameObject shotImageCreated = Instantiate (shotImage, transform.position, parent.rotation * Quaternion.Euler(0, 0, 180));
				shotImageCreated.gameObject.transform.parent = gameObject.transform;

				// Shot Sound
				soundController.PlayShot();
			}
		}
		else
		{
			time += Time.deltaTime;
		}
	}

	// Getters and Setters
	public void SetInRange(bool inRange)
	{
		this.inRange = inRange;
	}
}