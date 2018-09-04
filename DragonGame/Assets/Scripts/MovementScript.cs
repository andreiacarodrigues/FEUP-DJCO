using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
	private float horizontal;
	private float vertical;

	public Transform JumpPos;
	public LayerMask WhatIsGround;
    private int jumpCharges = 2;
	private float jumpRadius = 0.1f;

	private new Rigidbody rigidbody;
	private Animator animator;

	private CameraScript cameraScript;

	private Vector3 forward;
	private Vector3 right;

	private float moveForce = 800f;
	private float maxSpeed = 8f;

	private bool movePlayer;

	public GameObject fire;
	public GameObject heal;

	private bool playingAnim = false;

	private void Start()
	{
		rigidbody = GetComponent<Rigidbody>();
		animator = GetComponentInChildren<Animator> ();
		cameraScript = FindObjectOfType<Camera>().GetComponent<CameraScript>();
		movePlayer = true;
	}

	private void Update()
	{
        ProcessMovement();
    }

	private void FixedUpdate()
	{
        if (movePlayer) 
		{
			forward = cameraScript.CurrentForwardProjection;
			right = cameraScript.CurrentRightProjection;

			rigidbody.AddForce ((horizontal * right + vertical * forward) * moveForce * Time.deltaTime);
			if (rigidbody.velocity.magnitude > maxSpeed) 
			{
				rigidbody.velocity *= maxSpeed / rigidbody.velocity.magnitude;
			}

			if (rigidbody.velocity.magnitude > 0.25f) 
			{
				Vector3 velocity = rigidbody.velocity;
				velocity.y = 0;

				if (velocity != Vector3.zero) 
				{	
					transform.rotation = Quaternion.LookRotation (velocity);
				}
			}
				
			if (!animator.GetBool ("Jumping")) {
				animator.SetBool ("Moving", rigidbody.velocity.magnitude > 0.25f ? true : false);
			}
		}

        /*

		// isto são as animações para a batalha

		// fire

		if (Input.GetKeyDown (KeyCode.F) && !playingAnim)
		{
			if (!animator.GetBool ("Fire")) 
			{
				animator.SetBool ("Fire", true);
				playingAnim = true;
				GameObject createdFire = Instantiate (fire, transform.position, transform.rotation);
				FireController fireController = createdFire.GetComponent<FireController> ();
			}
		}

		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Fire") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f) 
		{
			if (animator.GetBool ("Fire")) {
				animator.SetBool ("Fire", false);
				playingAnim = false;
			}
		}

		// heal

		if (Input.GetKeyDown (KeyCode.H) && !playingAnim)
		{
			if (!animator.GetBool ("Heal")) 
			{
				animator.SetBool ("Heal", true);
				playingAnim = true;
				Vector3 healPos = new Vector3(transform.position.x, 1f, transform.position.z);
				Instantiate (heal, healPos, transform.rotation);
			}
		}

		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Heal") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f) 
		{
			if (animator.GetBool ("Heal")) 
			{
				animator.SetBool ("Heal", false);
				playingAnim = false;
			}
		}

		// block

		if (Input.GetKeyDown (KeyCode.B) && !playingAnim)
		{
			if (!animator.GetBool ("Block")) 
			{
				animator.SetBool ("Block", true);
				playingAnim = true;
			}
		}

		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Block") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f) 
		{
			if (animator.GetBool ("Block")) 
			{
				animator.SetBool ("Block", false);
				playingAnim = false;
			}
		}

		// die

		if (Input.GetKeyDown (KeyCode.D) && !playingAnim)
		{
			if (!animator.GetBool ("Die")) 
			{
				animator.SetBool ("Die", true);
				playingAnim = true;
			}
		}

		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Die") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f) 
		{
			if (animator.GetBool ("Die")) 
			{
				animator.SetBool ("Die", false);
				playingAnim = false;
			}
		}

		// slash

		if (Input.GetKeyDown (KeyCode.V) &&  !playingAnim)
		{
			if (!animator.GetBool ("Slash")) 
			{
				animator.SetBool ("Slash", true);
				playingAnim = true;
			}
		}

		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Slash") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f) 
		{
			if (animator.GetBool ("Slash")) 
			{
				animator.SetBool ("Slash", false);
				playingAnim = false;
			}
		}

		// pain

		if (Input.GetKeyDown (KeyCode.P) && !playingAnim)
		{
			if (!animator.GetBool ("Pain")) 
			{
				animator.SetBool ("Pain", true);
				playingAnim = true;
			}
		}

		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Pain") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f) 
		{
			if (animator.GetBool ("Pain")) 
			{
				animator.SetBool ("Pain", false);
				playingAnim = false;
			}
		}

		// tail

		if (Input.GetKeyDown (KeyCode.T) && !playingAnim)
		{
			if (!animator.GetBool ("Tail")) 
			{
				animator.SetBool ("Tail", true);
				playingAnim = true;
			}
		}

		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Tail") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f) 
		{
			if (animator.GetBool ("Tail")) 
			{
				animator.SetBool ("Tail", false);
				playingAnim = false;
			}
		}

    */
			
		animator.SetBool("Moving", rigidbody.velocity.magnitude > 0.25f);
	}

    public void ProcessMovement()
    {
        if (movePlayer)
        {
            bool isGrounded = Physics.OverlapSphere(JumpPos.position, jumpRadius, WhatIsGround).Length > 0;
            animator.SetBool("Grounded", isGrounded);

            if (isGrounded)
            {
                animator.SetBool("Jumping", false);
                animator.SetBool("DoubleJumping", false);
                animator.SetBool("Flying", false);
                jumpCharges = 2;
            }

            if (jumpCharges > 0 && Input.GetKeyDown(KeyCode.Space))
            {
                jumpCharges--;
               
                rigidbody.AddForce(new Vector3(0, 1000, 0));

                if (jumpCharges == 1)
                {
                    transform.parent.transform.position += new Vector3(0, jumpRadius, 0);
                    animator.SetBool("Jumping", true);
                }
                else if (jumpCharges == 0)
                {
                    animator.SetBool("DoubleJumping", true);
                    animator.SetBool("Jumping", false);
                }
            }
            else if (!isGrounded && Input.GetKey(KeyCode.Space) && rigidbody.velocity.y <= 0 && jumpCharges == 0)
            {
                rigidbody.AddForce(new Vector3(0, 600f * Time.deltaTime, 0));
                animator.SetBool("Flying", true);
                animator.SetBool("DoubleJumping", false);
            }

            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }
    }

	public void SetMovePlayer(bool newState)
	{
		movePlayer = newState;
	}
}