using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class BattleUIController : MonoBehaviour
{
	private DragonSoundPlayer ds;

	private PlayerBattleScript battleScript;

	public TextMeshProUGUI playerInfo;

	public GameObject[] skillsIndicators;

	public GameObject[] skillOverlay;

	private GameObject[] enemiesGameObjects;

	private GameObject[] enemiesIndicators;

	public TextMeshProUGUI battleResults;

//	public GameObject indicator;
	private Vector3 indicatorInitPos;
	private int curTurn;
	private int curSkill;
	private int curEnemy;


	void Start()
	{
		curSkill = 0;
		curTurn = 0;
		curEnemy = 0;
		ActivateIndicator (0);
		battleResults.gameObject.SetActive (false);
		ds = gameObject.GetComponentInChildren<DragonSoundPlayer> ();
	}
		
	void Update()
	{
		if (curTurn == 0) 
		{
			playerInfo.text = "Choose a skill!";
			if (battleScript.CurrentAbilityCooldowns () [curSkill] > 0) 
			{
				while (battleScript.CurrentAbilityCooldowns () [curSkill] > 0) 
				{
					curSkill++;
				}

				GetNextAvailableSkill (curSkill, false);
			}

			if (Input.GetKeyDown (KeyCode.UpArrow)) 
			{
				ds.PlaySoundScript ("event:/UI/ui_hover", 0, 0);

				curSkill -= 1;

				if (curSkill < 0) 
				{
					curSkill = 4;
				}

				GetNextAvailableSkill (curSkill, true);
			}

			if (Input.GetKeyDown (KeyCode.DownArrow)) 
			{
				ds.PlaySoundScript ("event:/UI/ui_hover", 0, 0);

				curSkill += 1;

				if (curSkill > 4) 
				{
					curSkill = 0;
				}

				GetNextAvailableSkill (curSkill, false);
			}


			if (Input.GetKeyDown (KeyCode.Return)) 
			{
				ds.PlaySoundScript ("event:/UI/ui_open", 0, 0);

				int[] indices = Battle.GetAliveIndices ();
				curEnemy = indices [0];
				if (curSkill != 1 && curSkill != 2)
					enemiesIndicators [curEnemy].SetActive (true);

				ActivateIndicator ();
				curTurn++;
			}
		} 
		else if (curTurn == 1) 
		{
			if (curSkill == 1 || curSkill == 2) 
			{
				battleScript.SetSkillAndTarget (curSkill, 0);
				curTurn++;
			} 
			else 
			{
				playerInfo.text = "Choose a target!";

				if (Input.GetKeyDown (KeyCode.RightArrow)) 
				{

					ds.PlaySoundScript ("event:/UI/ui_hover", 0, 0);

					if (enemiesIndicators.Length == 3) 
					{
						curEnemy += 1;
						if (curEnemy > 2) 
						{
							curEnemy = 0;
						}

						GetNextEnemy (enemiesIndicators.Length, curEnemy, true);
					} 
					else if (enemiesIndicators.Length == 2) 
					{
						curEnemy += 1;
						if (curEnemy > 1) 
						{
							curEnemy = 0;
						}

						GetNextEnemy (enemiesIndicators.Length, curEnemy, true);
					}
				}

				if (Input.GetKeyDown (KeyCode.LeftArrow)) 
				{

					ds.PlaySoundScript ("event:/UI/ui_hover", 0, 0);

					if (enemiesIndicators.Length == 3) 
					{
						curEnemy -= 1;
						if (curEnemy < 0) 
						{
							curEnemy = 2;
						}

						GetNextEnemy (enemiesIndicators.Length, curEnemy, false);
					} 
					else if (enemiesIndicators.Length == 2) 
					{
						curEnemy -= 1;

						if (curEnemy < 0) 
						{
							curEnemy = 1;
						}

						GetNextEnemy (enemiesIndicators.Length, curEnemy, false);
					}
				}

				if (Input.GetKeyDown (KeyCode.Return)) 
				{
					ds.PlaySoundScript ("event:/UI/ui_open", 0, 0);

					enemiesIndicators [curEnemy].SetActive (false);
					battleScript.SetSkillAndTarget (curSkill, curEnemy);
					curTurn++;
				}
			}
		} 
		else if(curTurn == 2)
		{
			if (curSkill == 1 || curSkill == 2) 
			{
				playerInfo.text = "Protect!";
			}
			else
				playerInfo.text = "Attack!";
		}
		else
		{
			playerInfo.text = "The enemies are attacking! You must wait for your turn.";
		}
	}

	private void ActivateIndicator(int num = -1)
	{
		for (int i = 0; i < skillsIndicators.Length; i++) 
		{
			skillsIndicators [i].SetActive (false);
		}

		if (num != -1) 
		{
			skillsIndicators [num].SetActive (true);
		}
	}


	public void UpdateUI()
	{
		for (int i = 0; i < battleScript.CurrentAbilityCooldowns ().Length; i++) 
		{
			if (battleScript.CurrentAbilityCooldowns () [i] > 0) 
			{
				skillOverlay [i].SetActive (true);
				TextMeshProUGUI text = skillOverlay [i].GetComponentInChildren<TextMeshProUGUI> ();
				text.text = battleScript.CurrentAbilityCooldowns () [i] + "";
			}
			else
				skillOverlay [i].SetActive (false);
		}
	}

	public void SetBattleScript(PlayerBattleScript p)
	{
		battleScript = p;
		UpdateUI ();
	}

	public void SetEnemiesGameObjects(GameObject[] e)
	{
		enemiesGameObjects = e;
		enemiesIndicators = new GameObject[e.Length];

		for(int i = 0; i < e.Length; i++)
		{
			EnemyBattleScript eScript = e[i].GetComponent<EnemyBattleScript>();
			Canvas eCanvas = eScript.GetCanvas();
			enemiesIndicators[i] = eCanvas.GetComponent<EnemyBattleIndicator>().GetIndicator();

			enemiesIndicators[i].SetActive(false);
		}
	}

	public void EnemiesTurn()
	{
		curTurn = 4;
	}

	public void ResetTurn()
	{
		curSkill = 0;
		GetNextAvailableSkill(curSkill, false);
		curTurn = 0;
	}

	public void BattleResults(bool won)
	{
		if (won) 
		{
			battleResults.text = "Battle Successful!";
		} 
		else 
		{
			battleResults.text = "Battle Unsuccessful! Try again when you're stronger!";
		}
		battleResults.gameObject.SetActive (true);
	}

	private void GetNextEnemy(int total, int enemy, bool right)
	{
		if(enemy > total - 1)
		{
			GetNextEnemy(total, 0, right);
		}
		else if(enemy < 0)
		{
			GetNextEnemy(total, total - 1, right);
		}
		else if(!Battle.GetAliveIndices().Contains(enemy))
		{
			if(right)
			{
				GetNextEnemy(total, enemy + 1, true);
			}
			else
			{
				GetNextEnemy(total, enemy - 1, false);
			}
		}
		else
		{
			for(int i = 0; i < total; i++)
			{
				enemiesIndicators[i].SetActive(false);
			}

			enemiesIndicators[enemy].SetActive(true);
		}
	}

	private void GetNextAvailableSkill(int skill, bool up)
	{
		if(up)
		{
			if(skill < 0)
			{
				GetNextAvailableSkill(4, up);
				return;
			}

			Debug.Log(skill);
			if(battleScript.CurrentAbilityCooldowns()[skill] > 0)
			{
				GetNextAvailableSkill(skill - 1, up);
			}
			else
			{
				ActivateIndicator (skill);
				curSkill = skill;
			}
		}
		else
		{
			Debug.Log("Cur Skill: " + skill);
			if(skill > 4)
			{
				GetNextAvailableSkill(0, up);
				return;
			}

			if(battleScript.CurrentAbilityCooldowns()[skill] > 0)
			{
				GetNextAvailableSkill(skill + 1, up);
			}
			else
			{
				ActivateIndicator (skill);
				curSkill = skill;
			}
		}
	}
}