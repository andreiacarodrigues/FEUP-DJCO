using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainController : MonoBehaviour
{
    public bool active = true;
	private WorldSceneController wsc;

	// Use this for initialization
	void Start ()
	{
		wsc = FindObjectOfType<WorldSceneController>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void OnCollisionEnter(Collision collision)
    {
        if(!active)
        {
            return;
        }

        if(collision.gameObject.CompareTag("Player"))
        {
            // Fully Heal Player
            Battle.playerEntity.CurrentHealth = Battle.playerEntity.Stats.MaxHealth;
	        wsc.UpdateHealth();

            // Save Stats
            SaveLoadController.FountainsUsedList.Add(gameObject.name);
            SaveLoadController.Save();

			wsc.SavedGame.gameObject.SetActive (true);
			StartCoroutine (RemoveSavedGameInfo(3f));

            // Disable
            active = false;
        }
    }

	private IEnumerator RemoveSavedGameInfo(float time)
	{
		yield return new WaitForSeconds (time);
		wsc.SavedGame.gameObject.SetActive (false);
	}
}