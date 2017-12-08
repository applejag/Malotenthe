using System;
using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerController : RingWalker {

	[Header("Movement")]
	public float velocityAcceleration = 600;
	public float velocityDeacceleration = 300;
	public float velocityTerminal = 8;
	public float velocityJump = 15;
	public bool snappyInput = true;
	public bool snappyMovement = true;

	[Header("Shooting")]
	public Weapon weapon;

	[Header("Animations")]
	public Animator animBody;
	public SpriteRenderer spriteBody;

	[SerializeField, HideInInspector]
	private bool initialSpriteFlip;

	protected override void Awake() {
		base.Awake();
		
		isFacingRight = true;
	}

	private void Start()
	{
		initialSpriteFlip = spriteBody.flipX;
	}

	private void OnEnable()
	{
		weapon.WeaponFired += OnWeaponFired;
	}

	private void OnDisable()
	{
		weapon.WeaponFired -= OnWeaponFired;
	}

	private void OnWeaponFired(Weapon source)
	{
		animBody.SetTrigger("Shoot");
	}

	private void Update() {

		/**
		 *	READ INPUT
		*/

		float horizontal = snappyInput ? Input.GetAxisRaw("Horizontal") : Input.GetAxis("Horizontal");
		bool horiZero = Mathf.Approximately(horizontal, 0);
		bool jump = Input.GetButtonDown("Jump");

		bool fireBegan = Input.GetButtonDown("Fire1");
		bool fireEnded = Input.GetButtonUp("Fire1");
		
		/**
		 *	MOVEMENT
		*/

		if (!horiZero)
		{
			isFacingRight = horizontal > 0;
		}

		if (snappyMovement)
		{
			Body.velocity = (transform.right.xz() * horizontal * velocityTerminal).x_y(Body.velocity.y);
		}
		else
		{ 
			if (horiZero)
			{
				// Try counteract the movement.
				Body.AddForce(-Body.velocity.SetY(0) * Time.deltaTime * velocityDeacceleration);
			}
			else
			{
				Body.AddForce(transform.right * horizontal * velocityAcceleration * Time.deltaTime);

				Body.velocity = Vector2.ClampMagnitude(Body.velocity.xz(), velocityTerminal).x_y(Body.velocity.y);
			}
		}

		if (jump && Grounded) {
			Body.velocity = Body.velocity.SetY(velocityJump);
		}

		/**
		 *	VISUAL UPDATE
		*/

		// Update animators
		animBody.SetFloat("Speed", Body.velocity.xz().magnitude);
		animBody.SetBool("Moving", !horiZero);
		animBody.SetBool("Grounded", Grounded);

		// Rotate gun
		float weaponAngle = isFacingRight ? 0 : 180;
		weapon.SetRotation(weaponAngle, isFacingRight);
		spriteBody.flipX = !isFacingRight ^ initialSpriteFlip;

		/**
		 *	SHOOTING
		*/

		if (weapon) {
			if (fireBegan) {
				// Fire
				weapon.OnInputBegan();
			}
			if (fireEnded) {
				weapon.OnInputEnded();
			}
		}

	}

	//private static float AngleTowardsMouse(Vector3 from)
	//{
	//	return ((Input.mousePosition.xy() - Camera.main.WorldToScreenPoint(from).xy()).ToDegrees() + 360) % 360;
	//}

	public override void Damage(int damage)
	{
		base.Damage(damage);

		if (Dead) {
			animBody.SetBool("Dead", true);

			foreach (Collider col in GetComponentsInChildren<Collider>())
				col.enabled = false;

			Body.isKinematic = true;
			this.enabled = false;

		} else
			animBody.SetTrigger("Hit");
	}
}
