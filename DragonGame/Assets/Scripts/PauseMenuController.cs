using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

public class PauseMenuController : MonoBehaviour
{
	DragonSoundPlayer ds;
	private GameObject player;

	/* Controllers */
	private CameraScript cs;
	private MovementScript ms;

	/* Audio */
	public AudioMixer audioMixer;

	/* Pause And Setting Menus */
	private GameObject pauseMenu;
	private GameObject settingsMenu;

	/* Dropdowns */
	private Resolution[] resolutions;
	private TMP_Dropdown resolutionsDropdown;
	private TMP_Dropdown qualityDropdown;

	/* Toogle */
	private Toggle fullscreenToggle;

	/* Slider */
	private Slider volumeSlider;

	/* Is Paused */
	private bool paused;

	void Start () 
	{
		ds = GameObject.FindObjectOfType<DragonSoundPlayer> ();
		Scene currentScene = SceneManager.GetActiveScene ();
		if (currentScene.name != "end_game") 
		{
			cs = GameObject.FindObjectOfType<CameraScript> ();
			ms = GameObject.FindObjectOfType<MovementScript> ();
			player = GameObject.FindGameObjectWithTag ("Player");
	
			/* Resolutions Dropdown Initialization */

			resolutionsDropdown = GameObject.FindGameObjectWithTag ("ResolutionsDropdown").GetComponent<TMP_Dropdown>();
			resolutionsDropdown.ClearOptions ();

			resolutions = Screen.resolutions;
			List<string> options = new List<string> ();
			int currentResolutionValue = 0;

			for(int i = 0; i < resolutions.Length; i++)
			{
				string option = resolutions [i].width + " x " + resolutions [i].height;
				options.Add (option);

				if (resolutions [i].width == Screen.currentResolution.width &&
					resolutions [i].height == Screen.currentResolution.height) 
				{
					currentResolutionValue = i;
				}
			}

			resolutionsDropdown.AddOptions (options);
			resolutionsDropdown.value = currentResolutionValue;
			resolutionsDropdown.RefreshShownValue ();

			/* Quality Dropdown Initialization */

			qualityDropdown = GameObject.FindGameObjectWithTag ("QualityDropdown").GetComponent<TMP_Dropdown>();
			qualityDropdown.value = QualitySettings.GetQualityLevel ();
			qualityDropdown.RefreshShownValue ();

			fullscreenToggle = GameObject.FindGameObjectWithTag ("FullscreenToggle").GetComponent<Toggle>();
			fullscreenToggle.isOn = Screen.fullScreen;

			float volume = 0;
			FMODUnity.RuntimeManager.GetBus ("bus:/").getVolume (out volume, out volume);

			volumeSlider = GameObject.FindGameObjectWithTag ("VolumeSlider").GetComponent<Slider>();
			volumeSlider.value = volume;


			/* Hides Pause and Setting Menus */

			pauseMenu = GameObject.FindGameObjectWithTag ("Pause");
			settingsMenu = GameObject.FindGameObjectWithTag ("Settings");

			if (currentScene.name != "main_menu_scene") 
			{
				pauseMenu.SetActive (false);
			}

			settingsMenu.SetActive(false);

	        if(currentScene.name == "main_menu_scene" && !File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
	        {
	            GameObject.FindGameObjectWithTag("ResumeButton").GetComponent<Button>().interactable = false;
	        }
		}
    }

	void Update () 
	{
		Scene currentScene = SceneManager.GetActiveScene ();
		if((currentScene.name != "main_menu_scene" || currentScene.name != "end_game"))
		{
			if (Input.GetKeyUp (KeyCode.Escape))
			{
				if (paused)
				{
					ds.PlaySoundScript ("event:/UI/ui_pausemenu_exit", 0, 0);
					Resume();
				}
				else
				{
					Pause();
				}
			}
		}
	}

	public void Pause()
	{
		if (!paused) 
		{
			ds.PlaySoundScript ("event:/UI/ui_pausemenu_enter", 0, 0);
			paused = true;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			//Time.timeScale = 0;

			cs.SetMoveCamera (false);
			ms.SetMovePlayer (false);

			Rigidbody rb = player.GetComponent<Rigidbody>();
			rb.velocity = Vector3.zero;
		
			pauseMenu.SetActive (true);
		} 
		else 
		{
			ds.PlaySoundScript ("event:/UI/ui_click", 0, 0);
			settingsMenu.SetActive (false);
			pauseMenu.SetActive(true);
		}
	}

	private void Resume()
	{
		if(paused)
		paused = false;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		//Time.timeScale = 1;

		cs.SetMoveCamera (true);
		ms.SetMovePlayer (true);

		pauseMenu.SetActive(false);
		settingsMenu.SetActive (false);
	}

	public void Settings()
	{
		ds.PlaySoundScript ("event:/UI/ui_pausemenu_enter", 0, 0);
		pauseMenu.SetActive(false);
		settingsMenu.SetActive (true);
	}

	public void ExitToMenu()
	{
		ds.PlaySoundScript ("event:/UI/ui_click", 0, 0);
        Battle.won = true;
        Battle.worldObjName = "";
        Battle.playerEntity.CurrentExperience = 0;
        Battle.playerEntity.CurrentHealth = 0;
        Battle.playerEntity.InitStats("Dragon", 1);
        SaveLoadController.ResetValues();
		SceneManager.LoadScene ("main_menu_scene");
	}

	public void ExitGame()
	{
		ds.PlaySoundScript ("event:/UI/ui_click", 0, 0);
		Application.Quit ();
	}
		
	public bool IsPaused()
	{
		return paused;
	}

	public void SetPaused(bool paused)
	{
		this.paused = paused;
	}

	public void SetVolume(float volume)
	{
		ds.PlaySoundScript ("event:/UI/ui_click", 0, 0);
		FMODUnity.RuntimeManager.GetBus ("bus:/").setVolume (volume);

		//audioMixer.SetFloat ("Volume", volume);
	}

	public void SetQuality(int qualityIndex)
	{
		ds.PlaySoundScript ("event:/UI/ui_click", 0, 0);
		QualitySettings.SetQualityLevel (qualityIndex);
	}

	public void SetFullscreen(bool isFullscreen)
	{
		ds.PlaySoundScript ("event:/UI/ui_click", 0, 0);
		Screen.fullScreen = isFullscreen;
	}

	public void SetResolution(int resolutionIndex)
	{
		ds.PlaySoundScript ("event:/UI/ui_click", 0, 0);
		Resolution resolution = resolutions [resolutionIndex];
		Screen.SetResolution (resolution.width, resolution.height, Screen.fullScreen);
	}

	public void NewGame()
	{
		ds.PlaySoundScript ("event:/UI/ui_click", 0, 0);
        SaveLoadController.FountainsUsedList = new List<string>();
        SaveLoadController.EnemiesDefeatedList = new List<string>();
        SaveLoadController.ScrollsCaughtList = new List<string>();

        SceneManager.LoadScene ("forest_scene");
	}

	public void ResumeGame()
	{
		ds.PlaySoundScript ("event:/UI/ui_click", 0, 0);
        bool success = SaveLoadController.Load();
        if(success)
        {
            SceneManager.LoadScene("forest_scene");
        }
        else
        {
            Debug.Log("No Load Data");
        }
	}

	public void BackToMainMenu()
	{
		ds.PlaySoundScript ("event:/UI/ui_click", 0, 0);
		pauseMenu.SetActive(true);
		settingsMenu.SetActive (false);
	}
}