using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrinceWorldController : MonoBehaviour
{
	public EnemyData[] BattleEnemies;

	private void OnCollisionEnter(Collision other)
	{
		if(!other.gameObject.CompareTag("Player")) return;
		if(SaveLoadController.EnemiesDefeatedList.Contains(gameObject.name)) return;

		Battle.Enemies = BattleEnemies;
		Battle.worldObjName = gameObject.name;

		SaveLoadController.Save(false);

		SceneManager.LoadScene("battle_castle");
	}

	private void OnValidate()
	{
		if(BattleEnemies.Length <= 3) return;
		Debug.LogWarning("BattleEnemies length max 3");
		Array.Resize(ref BattleEnemies, 3);
	}
}