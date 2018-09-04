using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotImageController : MonoBehaviour
{
	private float lifetime = 0.2f;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Time.deltaTime >= lifetime)
		{
			Destroy (gameObject);
		}
		else
		{
			lifetime -= Time.deltaTime;
		}
	}
}