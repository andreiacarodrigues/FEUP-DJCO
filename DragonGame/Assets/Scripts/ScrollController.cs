using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollController : MonoBehaviour
{
    public int ScrollNumber;

	StoryController sc;
	DragonSoundPlayer dp;

	void Start () 
	{
		sc = FindObjectOfType<StoryController> ();
		dp = GameObject.FindGameObjectWithTag ("Player").GetComponentInChildren<DragonSoundPlayer> ();
	}

	void OnTriggerEnter(Collider other)
	{
		dp.PlaySoundScript ("event:/Characters/Misc/char_dragon_scroll_pick_up", 0, 0);
		sc.StoryMode (ScrollNumber);
        SaveLoadController.ScrollsCaughtList.Add(gameObject.transform.parent.gameObject.name);
        Destroy (gameObject.transform.parent.gameObject);
	}
}