using System;
using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerController : RingWalker {

	[Range(0,1)]
	public float velocityShootingPenalty = 0.5f;

	[Header("Shooting")]
	public Weapon weapon;

	protected override void Awake() {
		base.Awake();
		
		isFacingRight = true;
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

		if (jump && Grounded) {
			Body.velocity = Body.velocity.SetY(velocityJump);
		}

		/**
		 *	VISUAL UPDATE
		*/

		// Rotate gun
		float weaponAngle = isFacingRight ? 0 : 180;
		weapon.SetRotation(weaponAngle, isFacingRight);

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

		ParentUpdate(!horiZero, true, horizontal);
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
