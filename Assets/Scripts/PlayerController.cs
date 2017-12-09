using System;
using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerController : RingWalker {

	[Header("Movement")]
	public float velocityTerminal = 8;
	public float velocityJump = 15;
	[Range(0,1)]
	public float velocityShootingPenalty = 0.5f;

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
		if (Grounded)
			animBody.SetTrigger("Shoot");
	}

	private void Update() {

		/**
		 *	READ INPUT
		*/

		float horizontal = Input.GetAxisRaw("Horizontal");
		bool horiZero = Mathf.Approximately(horizontal, 0);
		bool jump = Input.GetButtonDown("Jump");

		bool fireBegan = Input.GetButtonDown("Fire1");
		bool fireEnded = Input.GetButtonUp("Fire1");
		
		/**
		 *	MOVEMENT
		*/

		if (weapon.IsShooting)
		{
			horizontal *= velocityShootingPenalty;
		}

		if (!horiZero && !weapon.IsShooting)
		{
			isFacingRight = horizontal > 0;
		}

		Body.velocity = (transform.right.xz() * horizontal * velocityTerminal).x_y(Body.velocity.y);

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
	
	public override void Damage(int damage)
	{
		base.Damage(damage);

		if (IsDead) {
			animBody.SetBool("Dead", true);
			
			this.enabled = false;
		} else
			animBody.SetTrigger("Hit");
	}
}
