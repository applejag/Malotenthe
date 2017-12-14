using System;
using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public sealed class PlayerController : RingWalker {

	[Range(0,5)]
	public float velocityRunningMultiplier = 2;
	[Range(0,1)]
	public float velocityShootingMultiplier = 0.5f;

	[Header("Shooting")]
	public Weapon weapon;

	private float lastGroundedVelocityMultiplier;

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
		bool running = Input.GetButton("Run") && Grounded;
		bool horiZero = Mathf.Approximately(horizontal, 0);
		bool jump = Input.GetButtonDown("Jump");

		bool fireBegan = Input.GetButtonDown("Fire");
		bool fireEnded = Input.GetButtonUp("Fire");
		
		/**
		 *	MOVEMENT
		*/

		float velocityMultiplier = 1;

		if (running)
		{
			velocityMultiplier *= velocityRunningMultiplier;
		}

		if (weapon.IsShooting)
		{
			velocityMultiplier *= velocityShootingMultiplier;
		}

		if (!horiZero && !weapon.IsShooting)
		{
			isFacingRight = horizontal > 0;
		}

		if (jump && Grounded)
		{
			Body.velocity = Body.velocity.SetY(VelocityJumpForce);
		}

		if (Grounded)
		{
			lastGroundedVelocityMultiplier = velocityMultiplier;
		}

		horizontal *= lastGroundedVelocityMultiplier;

		/**
		 *	VISUAL UPDATE
		*/

		// Rotate gun
		float weaponAngle = isFacingRight ? 0 : 180;
		weapon.SetRotation(weaponAngle, isFacingRight);

		animBody.SetBool("Running", running);

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

		if (IsDead)
			this.enabled = false;
	}
}
