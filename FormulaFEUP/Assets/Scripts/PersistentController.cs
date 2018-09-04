using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentController : MonoBehaviour
{
	private static Object instance = null;

	// Variables
	public GameMode GameMode;
	public Difficulty Difficulty;
	public Dictionary<Player, Character> PlayerCharacters;
	public int numberOfLaps;

	public string winner;
	public float time;

	// Use this for initialization
	void Start()
	{
		if(!SingletonInit())
			return;
	}

	// Singleton pattern init
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