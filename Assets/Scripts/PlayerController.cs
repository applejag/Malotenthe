using System;
using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using GameGUI;
using UnityEngine;
using UnityEngine.Serialization;

public sealed class PlayerController : RingWalker {

	[Range(0,5)]
	public float velocityRunningMultiplier = 2;
	[Range(0,1)]
	public float velocityShootingMultiplier = 0.5f;

	private float lastGroundedVelocityMultiplier;

	protected override void Awake() {
		base.Awake();
		
		isFacingRight = true;
	}

	private void OnEnable()
	{
		weapon.WeaponFired += OnWeaponFired;
		EventDeath += OnEventDeath;
	}

	private void OnDisable()
	{
		weapon.WeaponFired -= OnWeaponFired;
		EventDeath -= OnEventDeath;
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

		ParentUpdate(
			moveAnim: !horiZero, 
			movePhysx: true, 
			horizontal: horizontal);
	}

	private void OnEventDeath(int damage, object source)
	{
		this.enabled = false;
		DiedGUI.FadeInGameOverScreen(GetComponent<WalkerStatistics>());
	}

}
