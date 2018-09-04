using System;
using System.Collections;
using UnityEngine;

public class EnemyBattleScript : MonoBehaviour, ProjectileHit
{
	private const float fireAttackMult = 1;
	private const float stunAttackMult = 0.5f;
	private const float healMult = 0.75f;
	private const float princeMult = 1;

	public string TypeIdentifier;
	private MageType mageType;

	public Texture texture1;
	public Texture texture2;
	public Texture texture3;

	public readonly Entity entity = new Entity();
	private PlayerBattleScript playerScript;
	private GameObject player;
	private GameObject[] enemies;
	private EnemyBattleScript[] enemyScripts;
	private SlidingBar slidingBarScript;
	private Canvas canvas;
	private bool healing = false;
	private bool[] alive;
	GameObject target;
	EnemyBattleScript targetScript;
	private bool ready = false;

	private bool waitingOnHit = false;
	private bool hit = false;
	private int stunCooldown = 0;
	private int healCooldown = 0;

	public GameObject fireObject;
	public GameObject healParticleObject;
	public GameObject stunObject;

	private Animator animator;

	private bool attacking = false;
	private float elapsedAttackTime = 0;
	private bool back = false;
	private float entryTime = 0;
	private const float approachTime = 2.0f;
	private Vector3 initialPosition;
	private Quaternion initialRotation;

	bool fireSound;
	bool princeAttack;

	DragonSoundPlayer ds;

	public void Start()
	{
		animator = GetComponentInChildren<Animator>();
		ds = gameObject.GetComponentInChildren<DragonSoundPlayer> ();
		fireSound = true;
		princeAttack = true;
		ready = true;
	}

	public void Update()
	{
		AnimatorStateInfo clip = animator.GetCurrentAnimatorStateInfo(0);
		if(clip.IsName("Pain") && clip.normalizedTime > 1f)
		{
			animator.SetBool("Pain", false);
		}
	}

	public void Init(int level, MageType mageType, GameObject[] enemies, SlidingBar slidingBar, Canvas canvas)
	{
		entity.InitStats(TypeIdentifier, level);
		Debug.Log(entity.Stats.Attack);
		Debug.Log(entity.Stats.MaxHealth);
		playerScript = Battle.playerScript;
		player = Battle.playerObject;

		this.mageType = mageType;
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
			case MageType.Prince:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		this.enemies = enemies;
		enemyScripts = new EnemyBattleScript[enemies.Length];
		for(int i = 0; i < enemies.Length; i++)
		{
			enemyScripts[i] = enemies[i].GetComponent<EnemyBattleScript>();
		}

		alive = new bool[enemies.Length];
		for(int i = 0; i < alive.Length; i++)
		{
			alive[i] = true;
		}

		slidingBarScript = slidingBar;
		slidingBar.SetMaxValue(entity.Stats.MaxHealth);
		slidingBar.SetCurValue(entity.CurrentHealth);

		this.canvas = canvas;
		this.canvas.enabled = true;
	}

	public void TakeDamage(int value)
	{
		entity.CurrentHealth -= value;
		if(value > 0)
		{
			if(!(mageType == MageType.Prince))
				ds.PlaySoundScript ("event:/Characters/Combat/Mage/char_mage_damage", 0, 0);
			else
				ds.PlaySoundScript ("event:/Characters/Combat/Prince/char_prince_damage", 0, 0);
			animator.SetBool("Pain", true);
			
		}

		if(entity.CurrentHealth > entity.Stats.MaxHealth)
		{
			entity.CurrentHealth = entity.Stats.MaxHealth;
		}

		slidingBarScript.SetCurValue(entity.CurrentHealth);

		if(entity.CurrentHealth <= 0)
		{
			Battle.KillEnemy(gameObject);
		}
	}

	public bool Turn()
	{
		bool finished;
		switch(mageType)
		{
			case MageType.Normal:
				finished = Attack();
				break;
			case MageType.Stun:
				if(stunCooldown > 0)
				{
					finished = Attack();
				}
				else
				{
					finished = Stun();
				}
				break;
			case MageType.Heal:
				if(healCooldown > 0 || !ExistsWoundedEnemy())
				{
					finished = Attack();
				}
				else
				{
					finished = Heal();
				}

				break;
			case MageType.Prince:
				finished = AttackPrince();
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		if(finished)
		{
			if(stunCooldown > 0)
			{
				stunCooldown--;
			}

			if(healCooldown > 0)
			{
				healCooldown--;
			}
		}
		return finished;
	}

	public bool AttackPrince()
	{
		if(!attacking)
		{
			elapsedAttackTime = 0;
			attacking = true;
			back = false;
			entryTime = 0;
			initialPosition = transform.position;
			initialRotation = transform.rotation;
		}

		elapsedAttackTime += Time.deltaTime;
		animator.SetBool("Moving", true);
		print(back);

		if(!back)
		{
			float ratio = Mathf.Clamp(elapsedAttackTime / approachTime, 0, 0.8f);
			transform.position = Vector3.Lerp(initialPosition, player.transform.position, ratio);

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

				if (princeAttack && isName && normieTime > 0.5f) 
				{
					ds.PlaySoundScript ("event:/Characters/Combat/Prince/char_prince_damage", 0, 0);
					princeAttack = false;
				}

				if(isName && normieTimeOkay)
				{
					playerScript.TakeDamage((int)(entity.Stats.Attack * princeMult));
					animator.SetBool ("Attack", false);
					back = true;
					elapsedAttackTime = 0;
					transform.eulerAngles = transform.eulerAngles + 180f * Vector3.up;
					entryTime = 0;
					princeAttack = true;
				}
			}
		}
		else
		{
			float ratio = Mathf.Clamp(elapsedAttackTime / approachTime + 0.2f, 0.2f, 1.0f);
			transform.position = Vector3.Lerp(player.transform.position, initialPosition, ratio);
			if(ratio >= 1.0f)
			{
				transform.rotation = initialRotation; //TODO maybe smooth rotation?
				animator.SetBool("Moving", false);
				attacking = false;
				return true;
			}
		}

		return false;
	}

	public bool Attack()
	{
		if(waitingOnHit)
		{
			if(hit)
			{
				waitingOnHit = false;
				hit = false;
				return true;
			}

			return false;
		}

		animator.SetBool("Fire", true);
		AnimatorStateInfo clip = animator.GetCurrentAnimatorStateInfo(0);

		if (fireSound && !waitingOnHit && !animator.GetBool("Pain")) {
			ds.PlaySoundScript ("event:/Characters/Combat/Mage/char_mage_attack", 0, 0);
			fireSound = false;
		}

		if(clip.IsName("Fire") && clip.normalizedTime > 0.80f)
		{
			Vector3 firePos = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
			GameObject fire = Instantiate(fireObject, firePos, transform.rotation);
			waitingOnHit = true;
			FireController fireController = fire.GetComponent<FireController>();
			fireController.SetTarget(player);
			fireController.SetParent(gameObject);
			fireController.SetCallbackObject(this);
			fireController.SetDamage((int)(entity.Stats.Attack * fireAttackMult));
			fireController.Init();
			animator.SetBool("Fire", false);
			fireSound = true;
		}

		return false;
	}

	public bool Heal()
	{
		Debug.Log("Start heal.");
		if(!healing)
		{
			Debug.Log("Not Healing.");
			int min = 0;
			for(int i = 0; i < enemies.Length; i++)
			{
				if(!alive[i]) continue;
				Debug.Log("Chose" + i);
				min = (int)enemyScripts[i].entity.CurrentHealth;
				target = enemies[i];
				targetScript = enemyScripts[i];
				break;
			}
			for(int i = 1; i < enemies.Length; i++)
			{
				if(!alive[i]) continue;
				int health = (int)enemyScripts[i].entity.CurrentHealth;
				if(health < min)
				{
					Debug.Log("Chose " + i);
					min = health;
					target = enemies[i];
					targetScript = enemyScripts[i];
				}
			}

			healing = true;
		}

		animator.SetBool("Fire", true);

		AnimatorStateInfo clip = animator.GetCurrentAnimatorStateInfo(0);
		if(clip.IsName("Fire") && clip.normalizedTime > 0.95f)
		{
			Vector3 healPos = new Vector3(target.transform.position.x, target.transform.position.y + 1f, target.transform.position.z);
			Instantiate(healParticleObject, healPos, transform.rotation);
			ds.PlaySoundScript ("event:/Characters/Combat/Dragon/char_dragon_heal", 0, 0);
			targetScript.TakeDamage(-(int)(entity.Stats.Attack * healMult));
			animator.SetBool("Fire", false);
			healing = false;
			healCooldown = 2;
			return true;
		}

		return false;
	}

	public bool Stun()
	{
		if(waitingOnHit)
		{
			if(hit)
			{
				waitingOnHit = false;
				hit = false;
				stunCooldown = 3;
				return true;
			}

			return false;
		}

		animator.SetBool("Fire", true);
		AnimatorStateInfo clip = animator.GetCurrentAnimatorStateInfo(0);
		if(clip.IsName("Fire") && clip.normalizedTime > 0.80f)
		{
			Vector3 firePos = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
			GameObject fire = Instantiate(stunObject, firePos, transform.rotation);
			ds.PlaySoundScript ("event:/Characters/Combat/Mage/char_mage_attack", 0, 0);
			waitingOnHit = true;
			FireController fireController = fire.GetComponent<FireController>();
			fireController.SetTarget(player);
			fireController.SetParent(gameObject);
			fireController.SetCallbackObject(this);
			fireController.SetDamage((int)(entity.Stats.Attack * stunAttackMult));
			fireController.Init();
			animator.SetBool("Fire", false);
		}

		return false;
	}

	private bool ExistsWoundedEnemy()
	{
		for(int i = 0; i < alive.Length; i++)
		{
			if(alive[i])
			{
				if(enemyScripts[i].entity.CurrentHealth < enemyScripts[i].entity.Stats.MaxHealth)
				{
					return true;
				}
			}
		}

		return false;
	}

	void OnParticleCollision(GameObject other)
	{
		if (other.tag == "Fire")
		{
			FireController controller = other.GetComponent<FireController>();
			if(controller.GetParent() != gameObject)
			{
				TakeDamage(controller.GetDamage());
				DestroyObject(other.gameObject);
			}
		}
	}

	public void ProjectileHit()
	{
		hit = true;
	}

	public Canvas GetCanvas()
	{
		return canvas;
	}

	public void Kill(int i)
	{
		alive[i] = false;
	}

	public float Disable()
	{
        animator.Play("Die");
        float animTime = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        StartCoroutine (DisableEnemy(animTime));
        return animTime;
	}

	private IEnumerator DisableEnemy(float delayTime)
	{
		yield return new WaitForSeconds (delayTime);
		transform.parent.gameObject.SetActive(false);
	}

	public bool Ready()
	{
		return ready;
	}
}