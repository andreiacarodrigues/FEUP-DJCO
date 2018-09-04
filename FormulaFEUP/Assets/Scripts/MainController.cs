using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{
	private PersistentController pc;

	// Use this for initialization
	void Start ()
	{
		pc = FindObjectOfType<PersistentController>();
	}
		
	// Update is called once per frame
	void Update ()
	{
		
	}

	// Button Clicks
	public void SinglePlayer()
	{
		pc.GameMode = GameMode.SinglePlayer;
		SceneManager.LoadScene ("PickPlayerScene");
	}

	public void Hotseat()
	{
		pc.GameMode = GameMode.Hotseat;
		SceneManager.LoadScene ("PickPlayerScene");
	}

	public void Online()
	{
		// TODO
	}

	public void Back()
	{
		SceneManager.LoadScene ("TitleScene");
	}
}