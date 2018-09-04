using System;
using UnityEngine;
using UnityEngine.UI;

public enum Actions
{
	Attack,
	Heal,
	TailAttack,
	FireAttack,
	Block,

	None
	//TODO Other abilities
}

public class PlayerBattleScript : MonoBehaviour, ProjectileHit
{
	private const float fireAttackMult = 2;
	private const float slashMult = 1;
	private const float tailAttackMult = 1.5f;
	private const float healMult = 0.65f;
	private const float blockRatio = 0.5f;

	private bool fireballHit = false;
	private bool waitingOnFireball = false;

	public Entity entity;
	private GameObject[] enemies;
	public Actions action = Actions.None;

	public GameObject healParticleObject;
	public GameObject fireObject;

	private Animator animator;
	private bool attacking = false;
	private Transform myTransform = null;
	private Vector3 initialPosition;
	private Quaternion initialRotation;

	private GameObject targetedEnemy = null;
	private EnemyBattleScript targetedEnemyBattleScript = null;
	private Transform targetedEnemyTransform = null;
	private float elapsedAttackTime = 0;

	public GameObject stunParticles;
	GameObject stunGO;

	private const float approachTime = 2.0f;

	private float entryTime;
	private bool back;
	private bool stunned = false;
	private bool actionSelected = false;
	private bool blocking = false;

	private const int HealCooldown = 1;
	private const int TailAttackCooldown = 2;
	private const int FireAttackCooldown = 3;

	// TODO ------------------------------------------------------
	private const int BlockAttackCooldown = 0;
	private const int SlashAttackCooldown = 0;
	// TODO ------------------------------------------------------

	private int currentHealCooldown = 0;
	private int currentTailAttackCooldown = 0;
	private int currentFireAttackCooldown = 0;

	// TODO ------------------------------------------------------
	private int currentBlockAttackCooldown = 0;
	private int currentSlashAttackCooldown = 0;
	// TODO ------------------------------------------------------

	DragonSoundPlayer ds;

	private void Start()
	{
		entity = Battle.playerEntity;
		animator = GetComponentInChildren<Animator>();
		myTransform = GetComponent<Transform>();
		ds = gameObject.GetComponentInChildren<DragonSoundPlayer> ();
		stunGO = null;
	}

	public void Update()
	{
		AnimatorStateInfo clip = animator.GetCurrentAnimatorStateInfo(0);
		if(clip.IsName("Pain") && clip.normalizedTime > 1f)
		{
			animator.SetBool("Pain", false);
		}
	}

	public bool Turn()
	{
		if(blocking)
		{
			blocking = false;
		}

		if(stunned)
		{
			stunned = false;
			return true;
		}

		if (stunGO != null) 
		{
			Destroy (stunGO);
			stunGO = null;
		}

		if(action == Actions.None) return false;
		actionSelected = true;
		bool finished;
		switch(action)
		{
			case Actions.Attack:
				finished = Attack();
				break;
			case Actions.Heal:
				currentHealCooldown = HealCooldown;
				finished = Heal();
				break;
			case Actions.TailAttack:
				currentTailAttackCooldown = TailAttackCooldown;
				finished = TailAttack();
				break;
			case Actions.FireAttack:
				currentFireAttackCooldown = FireAttackCooldown;
				finished = FireAttack();
				break;
			case Actions.Block:
				finished = Block();
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		if(finished)
		{
			if(currentHealCooldown > 0)
			{
				currentHealCooldown--;
			}

			if(currentTailAttackCooldown > 0)
			{
				currentTailAttackCooldown--;
			}

			if(currentFireAttackCooldown > 0)
			{
				currentFireAttackCooldown--;
			}

			actionSelected = false;
			action = Actions.None;
		}

		return finished;
	}

	private bool Block()
	{
		blocking = true;
		return true;
	}

	private bool FireAttack()
	{
		if(waitingOnFireball)
		{
			if(fireballHit)
			{
				waitingOnFireball = false;
				fireballHit = false;
				return true;
			}

			return false;
		}

		if(!attacking)
		{
			elapsedAttackTime = 0;
			attacking = true;
			back = false;
			entryTime = 0;
		}

		animator.SetBool("Fire", true);
		AnimatorStateInfo clip = animator.GetCurrentAnimatorStateInfo(0);
		if(clip.IsName("Fire") && clip.normalizedTime > 0.80f)
		{
			Vector3 firePos = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
			GameObject fire = Instantiate(fireObject, firePos, transform.rotation);
			waitingOnFireball = true;
			FireController fireController = fire.GetComponent<FireController>();
			fireController.SetTarget(targetedEnemy);
			fireController.SetParent(gameObject);
			fireController.SetCallbackObject(this);
			fireController.SetDamage((int)(entity.Stats.Attack * fireAttackMult));
			fireController.Init();
			animator.SetBool("Fire", false);
			attacking = false;
		}

		return false;
	}

	private bool TailAttack()
	{
		if(!attacking)
		{
			elapsedAttackTime = 0;
			attacking = true;
			back = false;
			entryTime = 0;
			initialPosition = myTransform.position;
			initialRotation = myTransform.rotation;
		}

		elapsedAttackTime += Time.deltaTime;
		animator.SetBool("Moving", true);
		print(back);

		if(!back)
		{
			float ratio = Mathf.Clamp(elapsedAttackTime / approachTime, 0, 0.8f);
			myTransform.position = Vector3.Lerp(initialPosition, targetedEnemyTransform.position, ratio);

			if(ratio >= 0.8f)
			{
				animator.SetBool("Moving", false);
				animator.SetBool("TailAttack", true);
				if(entryTime == 0)
				{
					entryTime = Time.time;
				}

				AnimatorStateInfo clip = animator.GetCurrentAnimatorStateInfo(0);
				if(clip.IsName("TailAttack") && clip.normalizedTime > 0.95f)
				{
					targetedEnemyBattleScript.TakeDamage((int)(entity.Stats.Attack * tailAttackMult));
					animator.SetBool("TailAttack", false);
					back = true;
					myTransform.eulerAngles = myTransform.eulerAngles + 180f * Vector3.up;
					elapsedAttackTime = 0;
					entryTime = 0;
				}
			}
		}
		else
		{
			float ratio = Mathf.Clamp(elapsedAttackTime / approachTime + 0.2f, 0.2f, 1.0f);
			myTransform.position = Vector3.Lerp(targetedEnemyTransform.position, initialPosition, ratio);
			if(ratio >= 1.0f)
			{
				myTransform.rotation = initialRotation; //TODO maybe smooth rotation?
				animator.SetBool("Moving", false);
				attacking = false;
				return true;
			}
		}

		return false;
	}

	private bool Heal()
	{
		animator.SetBool("Heal", true);
		if(entryTime == 0)
		{
			entryTime = Time.time;
		}

		AnimatorStateInfo clip = animator.GetCurrentAnimatorStateInfo(0);
		if(clip.IsName("Heal") && clip.normalizedTime > 0.95f)
		{
			animator.SetBool("Heal", false);
			Vector3 healPos = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
			Instantiate(healParticleObject, healPos, transform.rotation);
			TakeDamage(-(int)(entity.Stats.Attack * healMult));
			elapsedAttackTime = 0;
			entryTime = 0;
			return true;
		}

		return false;
	}

	private bool Attack()
	{
		if(!attacking)
		{
			elapsedAttackTime = 0;
			attacking = true;
			back = false;
			entryTime = 0;
			initialPosition = myTransform.position;
			initialRotation = myTransform.rotation;
		}

		elapsedAttackTime += Time.deltaTime;
		animator.SetBool("Moving", true);
		print(back);

		if(!back)
		{
			float ratio = Mathf.Clamp(elapsedAttackTime / approachTime, 0, 0.8f);
			myTransform.position = Vector3.Lerp(initialPosition, targetedEnemyTransform.position, ratio);

			if(ratio >= 0.8f)
			{
				animator.SetBool("Moving", false);
				animator.SetBool("Attack", true);
				if(entryTime == 0)
				{
					entryTime = Time.time;
				}

				AnimatorStateInfo clip = animator.GetCurrentAnimatorStateInfo(0);
				bool isName = clip.IsName("Attack");
				var normieTime = clip.normalizedTime;
				bool normieTimeOkay = normieTime > 0.95f;
				if(isName && normieTimeOkay)
				{
					targetedEnemyBattleScript.TakeDamage((int)(entity.Stats.Attack * slashMult));
					animator.SetBool("Attack", false);
					back = true;
					elapsedAttackTime = 0;
					myTransform.eulerAngles = myTransform.eulerAngles + 180f * Vector3.up;
					entryTime = 0;
				}
			}
		}
		else
		{
			float ratio = Mathf.Clamp(elapsedAttackTime / approachTime + 0.2f, 0.2f, 1.0f);
			myTransform.position = Vector3.Lerp(targetedEnemyTransform.position, initialPosition, ratio);
			if(ratio >= 1.0f)
			{
				myTransform.rotation = initialRotation; //TODO maybe smooth rotation?
				animator.SetBool("Moving", false);
				attacking = false;
				return true;
			}
		}

		return false;
	}

	public void SetEnemiesGameObjects(GameObject[] enemies)
	{
		this.enemies = enemies;
	}

	public void TakeDamage(int value)
	{
		if(blocking)
		{
			entity.CurrentHealth -= (int)(value * blockRatio);
		}
		else
		{
			entity.CurrentHealth -= value;
		}


		if(value > 0)
		{
			if(blocking)
			{
				ds.PlaySoundScript ("event:/Characters/Combat/Dragon/char_dragon_block", 0, 0);
				animator.Play("Block");
			}
			else
			{
				animator.SetBool("Pain", true);
			}
		}

		if(entity.CurrentHealth > entity.Stats.MaxHealth)
		{
			entity.CurrentHealth = entity.Stats.MaxHealth;
		}

		if(entity.CurrentHealth <= 0)
		{
			animator.Play("Die");
			Battle.KillEnemy(gameObject);
		}
	}

	public void Stun()
	{
		if (!blocking) 
		{
			stunned = true;
			if (stunGO == null) 
			{
				Vector3 stunPos = new Vector3 (transform.position.x, transform.position.y + 1.5f, transform.position.z - 0.23f);
				stunGO = Instantiate (stunParticles, stunPos, transform.rotation);
			}
		}
	}

	public void SetSkillAndTarget(int skill, int target)
	{
		Debug.Log(skill + " " + target);
		switch(skill)
		{
			case 0:
				if(currentFireAttackCooldown == 0)
				action = Actions.FireAttack;
				break;
			case 1:
				if(currentHealCooldown == 0)
				action = Actions.Heal;
				break;
			case 2:
				if(currentBlockAttackCooldown == 0)
				action = Actions.Block;
				break;
			case 3:
				if(currentTailAttackCooldown == 0)
				action = Actions.TailAttack;
				break;
			case 4:
				if(currentSlashAttackCooldown == 0)
				action = Actions.Attack;
				break;
		}

		if(skill != 1 && skill != 2)
		{
			targetedEnemy = enemies[target];
			targetedEnemyBattleScript = targetedEnemy.GetComponent<EnemyBattleScript>();
			targetedEnemyTransform = targetedEnemy.GetComponent<Transform>();
		}
	}

	public void ProjectileHit()
	{
		fireballHit = true;
	}

	void OnParticleCollision(GameObject other)
	{
		if(other.CompareTag("Fire"))
		{
			FireController controller = other.GetComponent<FireController>();
			if(controller.GetParent() != gameObject)
			{
				TakeDamage(controller.GetDamage());
				DestroyObject(other.gameObject);
			}
		}

		if(other.gameObject.CompareTag("Stun"))
		{
			FireController controller = other.gameObject.GetComponent<FireController>();
			if(controller.GetParent() != gameObject)
			{
				Stun();
				TakeDamage(controller.GetDamage());
				DestroyObject(other.gameObject);
			}
		}
	}

	public int[] AbilityCooldowns()
	{
		return new int[] {FireAttackCooldown, HealCooldown, BlockAttackCooldown, TailAttackCooldown, SlashAttackCooldown};
	}

	public int[] CurrentAbilityCooldowns()
	{
		return new int[] {currentFireAttackCooldown, currentHealCooldown, currentBlockAttackCooldown, currentTailAttackCooldown, currentSlashAttackCooldown};
	}

	public float Disable()
	{
		var clip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
		if(clip.name != "Die") return -1;
		return clip.length;
	}
}