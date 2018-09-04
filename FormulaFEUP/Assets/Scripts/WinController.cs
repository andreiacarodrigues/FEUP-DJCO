using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class WinController : MonoBehaviour
{
	private PersistentController pc;

	private string winner;
	private Character winnerCharacter;
	private float time;

	public GameObject racerMagusto;
	public GameObject racerLidav;
	public GameObject blueAnnouncer;
	public GameObject redAnnouncer;
	public TextMeshProUGUI timeAnnouncer;

	// Use this for initialization
	void Start ()
	{
		pc = FindObjectOfType<PersistentController> ();

		time = pc.time;
		winner = pc.winner;

		if (pc.GameMode == GameMode.SinglePlayer)
		{
			winnerCharacter = pc.PlayerCharacters [Player.Blue];
		}
		else
		{
			Player player;

			if (winner == "Blue")
			{
				player = Player.Blue;
				blueAnnouncer.SetActive(true);
			}
			else if(winner == "Red")
			{
				player = Player.Red;
				redAnnouncer.SetActive(true);
			}
			else
			{
				Debug.Log ("Shouldn't be here");
				return;
			}

			winnerCharacter = pc.PlayerCharacters [player];

			if (winnerCharacter == Character.Lidav) 
			{
				racerLidav.SetActive(true);
			} 
			else 
			{
				racerMagusto.SetActive(true);
			}
			time = pc.time;
		}

		if ((time / 60) > 1) 
		{
			timeAnnouncer.text = "Total Time: " + (int)(time/60) + " m " + (time % 60).ToString ("0.0") + " s";
		} 
		else 
		{
			timeAnnouncer.text = "Total Time: " + (time).ToString ("0.0") + " s";
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void BackToMainMenu()
	{
		pc.GameMode = GameMode.SinglePlayer;
		pc.Difficulty = Difficulty.Undefined;
		pc.numberOfLaps = 0;
		pc.winner = "";
		pc.time = 0;

		SceneManager.LoadScene("TitleScene");
	}
}