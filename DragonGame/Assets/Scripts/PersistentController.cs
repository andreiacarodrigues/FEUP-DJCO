using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentController : MonoBehaviour {

	private static Object instance = null;

	void Start () {
		if(!SingletonInit())
			return;
	}

	bool SingletonInit()
	{
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
			return false;
		}

		DontDestroyOnLoad(gameObject);
		return true;
	}
}
