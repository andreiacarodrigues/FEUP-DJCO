using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Battle : MonoBehaviour
{
	public static EnemyData[] Enemies;
	public static readonly PlayerEntity playerEntity = new PlayerEntity();
    public static bool won = true;
	private static bool playerDead = false;
    public static string worldObjName = "";

    private static float animTime = 0;

	static Battle()
	{
		playerEntity.InitStats("Dragon", 1);
	}

	public Transform PlayerTransform;
	public Transform[] EnemiesTransforms3 = new Transform[3];
	public Transform[] EnemiesTransforms2 = new Transform[2];
	private static Transform[] enemyTransforms;

	public SlidingBar healthBar;

	public GameObject PlayerPrefab;
	public GameObject MagePrefab;
	public GameObject PrincePrefab;

	private int enemiesAmount;

	private static List<GameObject> enemiesGameObjects = new List<GameObject>();
	private static List<EnemyBattleScript> enemiesScripts = new List<EnemyBattleScript>();
	private static bool[] alive;

	public static BattleUIController battleUIScript;
	public GameObject battleUIObject;

	public static PlayerBattleScript playerScript;
	public static GameObject playerObject;

	private static bool playerTurn = true;
	private static bool xpNotCounted = true;

	private int enemyIndex = 0;
	// Use this for initialization
	void Start()
	{
        won = false;
		xpNotCounted = true;
		enemiesGameObjects.Clear();
		enemiesScripts.Clear();
		playerTurn = true;

		enemiesAmount = Enemies.Length;
		GameObject player = Instantiate(PlayerPrefab, PlayerTransform);
		playerObject = player;
		playerScript = player.GetComponent<PlayerBattleScript>();
		switch(enemiesAmount)
		{
			case 1:
			case 3:
				enemyTransforms = EnemiesTransforms3;
				break;
			case 2:
				enemyTransforms = EnemiesTransforms2;
				break;
			default:
				throw new Exception("EnemiesAmount invalid");
		}

		for(int i = 0; i < enemiesAmount; i++)
		{
			GameObject enemy;
			if(Enemies[i].MageType == MageType.Prince)
			{
				enemy = Instantiate(PrincePrefab, enemyTransforms[i]);
			}
			else
			{
				enemy = Instantiate(MagePrefab, enemyTransforms[i]);
			}
			EnemyBattleScript enemyScript = enemy.GetComponent<EnemyBattleScript>();
			enemiesGameObjects.Add(enemy);
			enemiesScripts.Add(enemyScript);
		}

		for(int i = 0; i < enemiesAmount; i++)
		{
			GameObject enemy = enemiesGameObjects[i];
			EnemyBattleScript enemyScript = enemy.GetComponent<EnemyBattleScript>();
			Canvas canvas = enemyTransforms[i].gameObject.GetComponentInChildren<Canvas>();
			SlidingBar slidingBar = enemyTransforms[i].gameObject.GetComponentInChildren<SlidingBar>();
			enemyScript.Init(Enemies[i].Level, Enemies[i].MageType, enemiesGameObjects.ToArray(), slidingBar, canvas);
		}

		alive = new bool[enemiesAmount];
		for(int i = 0; i < alive.Length; i++)
		{
			alive[i] = true;
		}

		playerScript.SetEnemiesGameObjects(enemiesGameObjects.ToArray());

		battleUIScript = battleUIObject.GetComponent<BattleUIController> ();
		battleUIScript.SetBattleScript (playerScript);

		battleUIScript.SetEnemiesGameObjects (enemiesGameObjects.ToArray());

		healthBar.SetMaxValue(playerEntity.Stats.MaxHealth);
	}

	void Update()
	{
		healthBar.SetCurValue(playerEntity.CurrentHealth);

		bool allDead = true;
		foreach(bool var in alive)
		{
			if(var)
			{
				allDead = false;
			}
		}

		if(playerTurn)
		{
			if(playerScript.Turn())
			{
				playerTurn = false;
				battleUIScript.UpdateUI ();
			}
		}
		else
		{
			bool done;
			if(alive[enemyIndex])
			{
				GameObject enemy = enemiesGameObjects[enemyIndex];
				EnemyBattleScript enemyScript = enemy.GetComponent<EnemyBattleScript>();
				if(enemyScript.Ready())
				{
					done = enemyScript.Turn();
					battleUIScript.EnemiesTurn();
				}
				else
				{
					done = false;
				}
			}
			else
			{
				done = true;
			}
			if(done)
			{
				enemyIndex++;
				if(enemyIndex >= enemiesGameObjects.Count)
				{
					battleUIScript.ResetTurn();
					playerTurn = true;
					enemyIndex = 0;
				}
			}
		}

		if(playerDead)
		{
			float animTime = playerScript.Disable();
			if(animTime > 0)
			{
				battleUIScript.BattleResults(false);
				playerDead = false;
				Invoke("MoveToForestScene", animTime);
			}
		}

		if(allDead)
		{
			if(xpNotCounted)
			{
				int sum = 0;
				foreach(GameObject enemy in enemiesGameObjects)
				{
					EnemyBattleScript enemyScript = enemy.GetComponent<EnemyBattleScript>();
					sum += enemyScript.entity.Stats.Experience;
				}

				playerEntity.CurrentExperience += sum;
				while(playerEntity.CurrentExperience >= playerEntity.Stats.Experience)
				{
					int remainder = playerEntity.CurrentExperience - playerEntity.Stats.Experience;
					playerEntity.LevelUp();
					playerEntity.CurrentExperience += remainder;
				}

				won = true;

				SaveLoadController.data.CurrHealth = playerEntity.CurrentHealth;
				SaveLoadController.data.CurrExp = playerEntity.CurrentExperience;
				SaveLoadController.data.CurrLevel = playerEntity.Stats.Level;
				battleUIScript.BattleResults (true);

				Invoke("MoveToForestScene", animTime);
				xpNotCounted = false;
			}
		}
	}

    private void MoveToForestScene()
    {
        SceneManager.LoadScene("forest_scene");
    }

	public static void KillEnemy(GameObject enemy)
	{
		if(enemy == playerObject)
		{
			playerDead = true;
		}

		for(int i = 0; i < enemiesGameObjects.Count; i++)
		{
			GameObject enemyObj = enemiesGameObjects[i];
			if(enemy == enemyObj)
			{
				animTime = enemyObj.GetComponent<EnemyBattleScript>().Disable ();

				alive[i] = false;
				for(int j = 0; j < enemiesScripts.Count; j++)
				{
					if(alive[j])
					{
						enemiesScripts[j].Kill(i);
					}
				}
				break;
			}
		}
	}

	public static int[] GetAliveIndices()
	{
		List<int> ret = new List<int>();
		for(int i = 0; i < alive.Length; i++)
		{
			if(alive[i])
			{
				ret.Add(i);
			}
		}

		return ret.ToArray();
	}
}