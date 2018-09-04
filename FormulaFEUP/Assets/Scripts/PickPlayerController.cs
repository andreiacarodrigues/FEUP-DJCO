using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public enum Character
{
	Lidav,
	MagustoPousa
}

public enum Player
{
	Blue,
	Red
}

public enum Difficulty
{
	Undefined,
	Easy,
	Medium,
	Hard
}

public class PickPlayerController : MonoBehaviour
{
	public Button[] difficultyButtons;
	public TMP_InputField inputLaps;
	public Button startButton;

	public TextMeshProUGUI lidavSelected;
	public TextMeshProUGUI magustoSelected;

	public TextMeshProUGUI lidavInstructions;
	public TextMeshProUGUI magustoInstructions;

	private PersistentController persistentController;

	private readonly Dictionary<Player, Character> charactersSelected = new Dictionary<Player, Character>();

	// Use this for initialization
	private void Start()
	{
		persistentController = FindObjectOfType<PersistentController>();

		if (persistentController.GameMode == GameMode.SinglePlayer)
		{
			lidavInstructions.text = "\n<#92C0FFFF>Player Blue:</color> A";
			magustoInstructions.text = "\n<#92C0FFFF> Player Blue: </color> D";
		}
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
			SetLidav(Player.Blue);
		}

		if(Input.GetKeyDown(KeyCode.D))
		{
			SetMagusto(Player.Blue);
		}

		if(persistentController.GameMode == GameMode.Hotseat)
		{
			if(Input.GetKeyDown(KeyCode.LeftArrow))
			{
				SetLidav(Player.Red);
			}

			if(Input.GetKeyDown(KeyCode.RightArrow))
			{
				SetMagusto(Player.Red);
			}
		}

		if(persistentController.GameMode == GameMode.SinglePlayer && charactersSelected.ContainsKey(Player.Blue) && persistentController.Difficulty == Difficulty.Undefined)
		{
			SetDifficultyButtons(true, Difficulty.Undefined);
		}

		if(persistentController.GameMode == GameMode.Hotseat && charactersSelected.ContainsKey(Player.Blue) &&
			charactersSelected.ContainsKey(Player.Red) && persistentController.Difficulty == Difficulty.Undefined)
		{
			
			SetDifficultyButtons(true, Difficulty.Undefined);
 		}
	}

	private void SetLidav(Player player)
	{
		if(persistentController.GameMode == GameMode.Hotseat && charactersSelected.ContainsValue(Character.Lidav))
		{
			return;
		}

		if(charactersSelected.ContainsKey(player))
		{
			var character = charactersSelected[player];
			switch(character)
			{
				case Character.Lidav:
					UnsetSelectedText(lidavSelected);
					break;
				case Character.MagustoPousa:
					UnsetSelectedText(magustoSelected);
					break;
			}
		}
		charactersSelected[player] = Character.Lidav;
		SetSelectedText(player, lidavSelected);
	}

	private void SetMagusto(Player player)
	{
		if(persistentController.GameMode == GameMode.Hotseat && charactersSelected.ContainsValue(Character.MagustoPousa))
		{
			return;
		}

		if(charactersSelected.ContainsKey(player))
		{
			var character = charactersSelected[player];
			switch(character)
			{
				case Character.Lidav:
					UnsetSelectedText(lidavSelected);
					break;
				case Character.MagustoPousa:
					UnsetSelectedText(magustoSelected);
					break;
			}
		}
		charactersSelected[player] = Character.MagustoPousa;
		SetSelectedText(player, magustoSelected);
	}

	private static void SetSelectedText(Player player, TextMeshProUGUI selectedText)
	{
		if(player == Player.Blue)
		{
			Color mycolor = new Color ();
			ColorUtility.TryParseHtmlString ("#92C0FFFF", out mycolor);

			selectedText.color = mycolor;
			selectedText.text = "Player Blue";
		}
		else
		{
			Color mycolor = new Color ();
			ColorUtility.TryParseHtmlString ("#FF9292FF", out mycolor);

			selectedText.color = mycolor;
			selectedText.text = "Player Red";
		}
	}

	private static void UnsetSelectedText(TextMeshProUGUI selectedText)
	{
		selectedText.text = "";
	}

	#region Button Controllers

	public void EasySelected()
	{
		DifficultySelected(Difficulty.Easy);

	}

	public void MediumSelected()
	{
		DifficultySelected(Difficulty.Medium);

	}

	public void HardSelected()
	{
		DifficultySelected(Difficulty.Hard);

	}

	#endregion

	private void SetDifficultyButtons(bool state, Difficulty difficulty)
	{
		if(difficulty == Difficulty.Undefined)
		{
			foreach(var difficultyButton in difficultyButtons)
			{
				difficultyButton.interactable = state;
			}
		}
		else
		{
			var chosenBtn = difficultyButtons [(int)difficulty - 1];
			foreach(var difficultyButton in difficultyButtons)
			{
				if(difficultyButton != chosenBtn)
					difficultyButton.interactable = state;
			}
		}
	}

	private void DifficultySelected(Difficulty difficulty)
	{
		persistentController.Difficulty = difficulty;
		persistentController.PlayerCharacters = charactersSelected;
		SetDifficultyButtons(false, difficulty);
		inputLaps.interactable = true;
	}

	public void InputLaps()
	{
		int laps;
		if(int.TryParse(inputLaps.text, out laps))
		{
			if(laps > 0)
			{
				persistentController.numberOfLaps = laps;
				startButton.interactable = true;
			}
			else
			{
				startButton.interactable = false;
			}
		}
		else
		{
			startButton.interactable = false;
		}
	}

	public void StartGame()
	{
		SceneManager.LoadScene("RacingScene");
	}

	public void Back()
	{
		persistentController.GameMode = GameMode.SinglePlayer;
		persistentController.Difficulty = Difficulty.Undefined;
		persistentController.numberOfLaps = 0;
		persistentController.winner = "";
		persistentController.time = 0;

		SceneManager.LoadScene("MainScene");
	}
}