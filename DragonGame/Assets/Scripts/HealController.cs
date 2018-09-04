using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealController : MonoBehaviour {

	private bool hasWaited = false;

	void Start () {
		
	}
	
	void Update ()
	{
		if (!hasWaited) 
		{
			StartCoroutine (PlayAnim (3.5f));
		} 
		else 
		{
			DestroyObject (gameObject);
		}
	}

	private IEnumerator PlayAnim(float time)
	{
		if (!hasWaited) 
		{
			yield return new WaitForSeconds (time);
			hasWaited = true;
		}
	}

}
