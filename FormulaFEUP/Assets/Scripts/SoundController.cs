using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
	public AudioClip impact;
	public AudioClip shot;
	public AudioClip powerup;

	private AudioSource source;

	// Use this for initialization
	void Start ()
	{
		source = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void PlayImpact()
	{
		source.PlayOneShot (impact, 0.2f);
	}

	public void PlayShot()
	{
		source.PlayOneShot (shot, 0.2f);
	}

	public void PlayPowerup()
	{
		source.PlayOneShot (powerup, 0.2f);
	}
}