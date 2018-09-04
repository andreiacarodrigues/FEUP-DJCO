using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum GameMode
{
	SinglePlayer,
	Hotseat,
	Online
}

public class GameController : MonoBehaviour
{
	private PersistentController pc;

	// Variables
	public CarController blueCar;
	public CarController redCar;
	public GameMode gameMode;

	private CheckpointsController cc;

	// UI
	public TextMeshProUGUI lapsBlue;
	public TextMeshProUGUI lapsRed;

	public TextMeshProUGUI checkpointsBlue;
	public TextMeshProUGUI checkpointsRed;

	public int maxLaps;

	public Image timerBlue;
	public Image timerRed;
	public TextMeshProUGUI timerBlueTxt;
	public TextMeshProUGUI timerRedTxt;
	public CarWeaponController cwcBlue;
	public CarWeaponController cwcRed;

	public Image PlayerBlueLidav;
	public Image PlayerBlueMagusto;
	public Image PlayerRedLidav;
	public Image PlayerRedMagusto;

	public GameObject secondPlayerUI;

	// needed for initial countdown
	private bool countdownDone;
	public TextMeshProUGUI countdownText;

	public TextMeshProUGUI timeText;
	private float time;

	public Sprite[] powerups;
	public Image redPowerup;
	public Image bluePowerup;


	// Use this for initialization
	void Start ()
	{
		pc = FindObjectOfType<PersistentController> ();

		if (pc != null)
		{
			gameMode = pc.GameMode;
			maxLaps = pc.numberOfLaps;
			MissileController.Configure (pc.Difficulty);
		}

		if (gameMode == GameMode.SinglePlayer)
		{
			redCar.gameObject.SetActive (false);
			secondPlayerUI.gameObject.SetActive (false);

			if (pc.PlayerCharacters [Player.Blue] == Character.Lidav) 
			{
				PlayerBlueLidav.gameObject.SetActive (true);
			}
			else
			{
				PlayerBlueMagusto.gameObject.SetActive (true);
			}

			PowerUpController[] PowerUpObjects = FindObjectsOfType<PowerUpController> ();
			foreach(PowerUpController PowerUpObject in PowerUpObjects)
			{
				Destroy (PowerUpObject.gameObject);
			}

		}
		if (gameMode == GameMode.Hotseat) 
		{
			if (pc.PlayerCharacters [Player.Red] == Character.Lidav) 
			{
				PlayerRedLidav.gameObject.SetActive (true);
			}
			if (pc.PlayerCharacters [Player.Blue] == Character.Lidav) 
			{
				PlayerBlueLidav.gameObject.SetActive (true);
			}

			if (pc.PlayerCharacters [Player.Red] == Character.MagustoPousa) 
			{
				PlayerRedMagusto.gameObject.SetActive (true);
			}
			if (pc.PlayerCharacters [Player.Blue] == Character.MagustoPousa) 
			{
				PlayerBlueMagusto.gameObject.SetActive (true);
			}
		}

		cc = FindObjectOfType<CheckpointsController> ();

		// needed for initial countdown
		countdownDone = false;

		// needed for initial countdown
		time = 0;

		UpdateUIInitial ();
	}

	void Update () 
	{
		if (countdownDone) 
		{
			UpdateUI ();
			time += Time.deltaTime;
		} 
		else 
		{
			if (time >= 4) 
			{
				countdownDone = true;
				countdownText.gameObject.SetActive (false);
				time = 0;
			} 
			else 
			{
				if ((int)time == 3)
				{
					countdownText.text = "GO!";
				}
				else 
				{
					countdownText.text = (3 - (int)time).ToString ();
				}
				time += Time.deltaTime;
			}
		}
	}

	private void UpdateUIInitial()
	{
		lapsBlue.text = cc.GetCurrBlueLap () + "/" + maxLaps;
		lapsRed.text = cc.GetCurrRedLap () + "/" + maxLaps;

		checkpointsBlue.text = cc.GetCurrBlueCheckpoint () + "/" + cc.GetTotalCheckpoints ();
		checkpointsRed.text = cc.GetCurrRedCheckpoint () + "/" + cc.GetTotalCheckpoints ();

		timerBlueTxt.text = "READY";
		timerBlue.fillAmount = 1;

		timerRedTxt.text = "READY";
		timerRed.fillAmount = 1;

		timeText.text = "0s";
	}

	private void UpdateUI()
	{
		lapsBlue.text = cc.GetCurrBlueLap () + "/" + maxLaps;
		lapsRed.text = cc.GetCurrRedLap () + "/" + maxLaps;

		checkpointsBlue.text = cc.GetCurrBlueCheckpoint () + "/" + cc.GetTotalCheckpoints ();
		checkpointsRed.text = cc.GetCurrRedCheckpoint () + "/" + cc.GetTotalCheckpoints ();

		float cooldownBlue = cwcBlue.GetCooldownTimer ();
		if (cooldownBlue > 0f) 
		{
			timerBlueTxt.text = (cooldownBlue).ToString("0.0");
			timerBlue.fillAmount = (.8f - cooldownBlue) / .8f;
		} 
		else 
		{
			timerBlueTxt.text = "READY";
			timerBlue.fillAmount = 1;
		}

		float cooldownRed = cwcRed.GetCooldownTimer ();
		if (cooldownRed > 0f) 
		{
			timerRedTxt.text = (cooldownRed).ToString("0.0");
			timerRed.fillAmount = (.8f - cooldownRed) / .8f;
		} 
		else 
		{
			timerRedTxt.text = "READY";
			timerRed.fillAmount = 1;
		}

		if ((time / 60) > 1) {
			timeText.text = (int)(time/60) + "m " + (time % 60).ToString ("0.0") + "s";
		} else {
			timeText.text = (time).ToString ("0.0") + "s";
		}

		if (redCar.getActivePowerUp () != PowerUpType.None)
		{
			Debug.Log ("UI RED: " + redCar.getActivePowerUp ());
		}
		
		if (redCar.getActivePowerUp() == PowerUpType.None) 
		{
			redPowerup.gameObject.SetActive (false);
		} 
		else 
		{
			redPowerup.sprite = powerups [(int)(redCar.getActivePowerUp())];
			redPowerup.gameObject.SetActive (true);
		}

		if (blueCar.getActivePowerUp () != PowerUpType.None)
		{
			Debug.Log ("UI BLUE: " + blueCar.getActivePowerUp ());
		}

		if (blueCar.getActivePowerUp() == PowerUpType.None) 
		{
			bluePowerup.gameObject.SetActive (false);
		} 
		else 
		{
			bluePowerup.sprite = powerups [(int)(blueCar.getActivePowerUp())];
			bluePowerup.gameObject.SetActive (true);
		}

	}

	public float GetTime()
	{
		return time;
	}
}