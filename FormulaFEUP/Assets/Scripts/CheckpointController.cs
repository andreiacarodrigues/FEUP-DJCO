using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CheckpointState
{
	Blue,
	Red,
	Mixed,
	None
}

public class CheckpointController : MonoBehaviour
{
	public CheckpointState state;
	public Sprite[] sprites;
	private CheckpointsController cc;
	private SpriteRenderer sr;

	// Use this for initialization
	void Start ()
	{
		cc = GameObject.FindObjectOfType<CheckpointsController> ();
		sr = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (state == CheckpointState.None)
		{
			sr.sprite = null;
		}
		else
		{
			
			sr.sprite = sprites[(int)state];
		}
	}

	// Events
	void OnTriggerEnter2D(Collider2D other)
	{
		string name = other.name;

		if (name == "BlueCar")
		{
			if (state == CheckpointState.Blue)
			{
				cc.CheckpointReached ("Blue");
				state = CheckpointState.None;
			}
			else if (state == CheckpointState.Mixed)
			{
				cc.CheckpointReached ("Blue");
				state = CheckpointState.Red;
			}
		}
		else if (name == "RedCar")
		{
			if (state == CheckpointState.Red)
			{
				cc.CheckpointReached ("Red");
				state = CheckpointState.None;
			}
			else if (state == CheckpointState.Mixed)
			{
				cc.CheckpointReached ("Red");
				state = CheckpointState.Blue;
			}
		}
		else
		{
			Debug.LogError ("Can't solve: " + name);
		}
	}
}