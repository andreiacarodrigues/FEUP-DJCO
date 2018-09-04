using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneController : MonoBehaviour {

	public void GoToMainScene()
	{
		SceneManager.LoadScene ("MainScene");
	}

	public void GoToAboutScene()
	{
		SceneManager.LoadScene ("AboutGameScene");
	}

	public void GoToHowToPlayScene()
	{
		SceneManager.LoadScene ("HowToPlayScene");
	}

	public void ExitGame()
	{
		Application.Quit ();
	}


	public void GoToTitleScene()
	{
		SceneManager.LoadScene ("TitleScene");
	}

	public void PowerupScene()
	{
		SceneManager.LoadScene ("PowerupsScene");
	}
}
