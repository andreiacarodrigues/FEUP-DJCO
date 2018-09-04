using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Position
{
	Ahead,
	Tied,
	Behind
}

public class CheckpointsController : MonoBehaviour
{
	// Variables
	public CheckpointController[] checkpoints;

	public int currBlue;
	public int currRed;

	public int currBlueLap;
	public int currRedLap;

	private float winOffsetTime = 1f;

	private PersistentController pc;
	public GameController gameController;

	// Use this for initialization
	void Start ()
	{
		pc = FindObjectOfType<PersistentController> ();

		currBlue = 0;
		currRed = 0;

		currBlueLap = 0;
		currRedLap = 0;

		if (GameObject.FindGameObjectsWithTag ("Player").Length == 1)
		{
			checkpoints [0].state = CheckpointState.Blue;
		}
		else
		{
			checkpoints [0].state = CheckpointState.Mixed;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	// Functions
	public void CheckpointReached(string color)
	{
		if (color == "Blue")
		{
			currBlue++;
			if (currBlue == checkpoints.Length)
			{
				currBlue = 0;
				currBlueLap++;

				if (currBlueLap == gameController.maxLaps)
				{
					if (pc != null)
					{
						if (pc.winner != "Red")
						{
							pc.winner = "Blue";
						}
					}
					Invoke ("MoveToVictoryScreen", winOffsetTime);
				}
			}

			CheckpointController newCheckpoint = checkpoints [currBlue];

			if (newCheckpoint.state == CheckpointState.None)
			{
				newCheckpoint.state = CheckpointState.Blue;
			}
			else if (newCheckpoint.state == CheckpointState.Red)
			{
				newCheckpoint.state = CheckpointState.Mixed;
			}

		}
		else if (color == "Red")
		{
			currRed++;
			if (currRed == checkpoints.Length)
			{
				currRed = 0;
				currRedLap++;

				if (currRedLap == gameController.maxLaps)
				{
					if (pc != null)
					{
						if (pc.winner != "Blue")
						{
							pc.winner = "Red";
						}
					}
					Invoke ("MoveToVictoryScreen", winOffsetTime);
				}
			}

			CheckpointController newCheckpoint = checkpoints [currRed];

			if (newCheckpoint.state == CheckpointState.None)
			{
				newCheckpoint.state = CheckpointState.Red;
			}
			else if (newCheckpoint.state == CheckpointState.Blue)
			{
				newCheckpoint.state = CheckpointState.Mixed;
			}
		}
	}


	public int GetCurrBlueLap()
	{
		return currBlueLap;
	}

	public int GetCurrRedLap()
	{
		return currRedLap;
	}

	public int GetCurrBlueCheckpoint()
	{
		return currBlue;
	}

	public int GetCurrRedCheckpoint()
	{
		return currRed;
	}

	public int GetTotalCheckpoints()
	{
		return checkpoints.Length;
	}

	private void MoveToVictoryScreen()
	{
		if (pc != null)
		{
			pc.time = gameController.GetTime () - winOffsetTime;
		}

		SceneManager.LoadScene("WinScene");
	}

	public Position PlayerPosition(Player player)
	{
		Player playerAhead;
		int redCheckpoints = currRedLap * GetTotalCheckpoints() + currRed;
		int blueCheckpoints = currBlueLap * GetTotalCheckpoints() + currBlue;
		if(redCheckpoints > blueCheckpoints)
		{
			playerAhead = Player.Red;
		}
		else if(blueCheckpoints > redCheckpoints)
		{
			playerAhead = Player.Blue;
		}
		else
		{
			return Position.Tied;
		}

		return player == playerAhead ? Position.Ahead : Position.Behind;
	}
}