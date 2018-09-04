using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class FireController : MonoBehaviour {

	GameObject target;
	float speedFactor = 2.5f;
	private float movementSpeed = 10f;
	private bool hasWaited = false;
	private GameObject parent;
	private ProjectileHit callback;
	private Vector3 targetPos;
	private int damage;

	public void Init()
	{
		transform.position = new Vector3(parent.transform.position.x, parent.transform.position.y + 1.5f, parent.transform.position.z);
		Debug.Log (parent.transform.position.x + " " + parent.transform.position.y + " " + parent.transform.position.z);
		targetPos = new Vector3(target.transform.position.x, target.transform.position.y + 1.5f, target.transform.position.z);
	}
	
	void Update ()
	{
		if (false)
		{
			StartCoroutine (PlayAnim (.5f));
		} 
		else 
		{
			float step = movementSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, 
				targetPos, step);
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

	public void SetTarget(GameObject varTarget)
	{
		target = varTarget;
	}

	public void SetParent(GameObject parParent)
	{
		parent = parParent;
	}

	public void SetCallbackObject(ProjectileHit ph)
	{
		callback = ph;
	}

	public GameObject GetParent()
	{
		return parent;
	}

	public void SetDamage(int damage)
	{
		this.damage = damage;
	}

	public int GetDamage()
	{
		callback.ProjectileHit();
		return damage;
	}
}
