using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSceneController : MonoBehaviour
{
	public SlidingBar HealthBar;
	public SlidingBar ExperienceBar;
	public TextMeshProUGUI LevelLabel;
	public TextMeshProUGUI SavedGame;

	void Awake()
	{
		SavedGame.gameObject.SetActive (false);

        if(!Battle.won)
        {
            SaveLoadController.Load();
        }
        else if(Battle.worldObjName != "" && Battle.won)
        {
            SaveLoadController.EnemiesDefeatedList.Add(Battle.worldObjName);
            if(Battle.worldObjName == "Prince")
            {
                SceneManager.LoadScene("end_game");
            }
        }

        SaveLoadController.SaveData data = SaveLoadController.data;

        if (SaveLoadController.data != null)
        {
            GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(data.PositionX, data.PositionY, data.PositionZ);
            Battle.playerEntity.InitStats("Dragon", data.CurrLevel);
            Battle.playerEntity.CurrentExperience = data.CurrExp;
            Battle.playerEntity.CurrentHealth = data.CurrHealth;

            StoryController.SetNumberScrollsFound(data.ScrollsCaughtCount);

            foreach(FountainController fountain in FindObjectsOfType<FountainController>())
            {
                int i = Array.IndexOf(data.FountainsUsed, fountain.name);

                if(i > -1)
                {
                    fountain.active = false;
                }
            }

            foreach(EnemyWorldScript enemy in FindObjectsOfType<EnemyWorldScript>())
            {
                int i = Array.IndexOf(data.EnemiesDefeated, enemy.name);

                if (i > -1 || (enemy.name == Battle.worldObjName && Battle.won))
                {
                    Destroy(enemy.gameObject);
                }
            }

            foreach(ScrollController scrollSub in FindObjectsOfType<ScrollController>())
            {
                GameObject scroll = scrollSub.transform.parent.gameObject;

                int i = Array.IndexOf(data.ScrollsCaught, scroll.name);

                if (i > -1)
                {
                    Destroy(scroll.gameObject);
                }
            }
        }
        else
        {
            SaveLoadController.ResetValues();
            SaveLoadController.Save();
        }

		HealthBar.SetMaxValue(Battle.playerEntity.Stats.MaxHealth);
		HealthBar.SetCurValue(Battle.playerEntity.CurrentHealth);

		ExperienceBar.SetMaxValue(Battle.playerEntity.Stats.Experience);
		ExperienceBar.SetCurValue(Battle.playerEntity.CurrentExperience);

		LevelLabel.text = "Level " + Battle.playerEntity.Stats.Level;
	}

    public void UpdateHealth()
    {
        HealthBar.SetCurValue(Battle.playerEntity.CurrentHealth);
    }
}