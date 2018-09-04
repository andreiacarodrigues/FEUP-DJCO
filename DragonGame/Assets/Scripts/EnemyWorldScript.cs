using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum MageType
{
	Normal,
	Stun,
	Heal,
	Prince
}

[Serializable]
public struct EnemyData
{
	public MageType MageType;
	public int Level;
}

public class EnemyWorldScript : MonoBehaviour
{
	public MageType mageType;
	public EnemyData[] BattleEnemies;

	public Texture texture1;
	public Texture texture2;
	public Texture texture3;

	private void Start()
	{
		switch(mageType)
		{
			case MageType.Normal:
				GetComponentInChildren<Renderer>().material.mainTexture = texture3;
				break;
			case MageType.Stun:
				GetComponentInChildren<Renderer>().material.mainTexture = texture2;
				break;
			case MageType.Heal:
				GetComponentInChildren<Renderer>().material.mainTexture = texture1;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if(!other.gameObject.CompareTag("Player")) return;
        if(SaveLoadController.EnemiesDefeatedList.Contains(gameObject.name)) return;

		Battle.Enemies = BattleEnemies;
        Battle.worldObjName = gameObject.name;

        SaveLoadController.Save(false);

		SceneManager.LoadScene("battle");
	}

	private void OnValidate()
	{
		if(BattleEnemies.Length <= 3) return;
		Debug.LogWarning("BattleEnemies length max 3");
		Array.Resize(ref BattleEnemies, 3);
	}
}