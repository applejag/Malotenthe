using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : RingWalker {

	[Header("Movement")]
	public float velocityAcceleration = 600;
	public float velocityDeacceleration = 300;
	public float velocityTerminal = 8;
	public float velocityJump = 15;

	[Header("Shooting")]
	public Weapon weapon;

	[Header("Health")]
	public int health = 20;
	public int maxHealth = 20;

	[Header("Animations")]
	public Animator animBody;
	//public Animator animGun;
	public SpriteRenderer spriteBody;
	//public SpriteRenderer spriteGun;

	protected override void Awake() {
		base.Awake();
		
		isFacingRight = true;
	}

	private void Update() {

		/**
		 *	READ INPUT
		*/

		float horizontal = Input.GetAxis("Horizontal");
		bool horiZero = Mathf.Approximately(horizontal, 0);
		bool jump = Input.GetButtonDown("Jump");

		bool fireBegan = Input.GetButtonDown("Fire1");
		bool fireEnded = Input.GetButtonUp("Fire1");

		float angleTowardsMouse = AngleTowardsMouse(weapon.transform.position);
		// - => right, + => left
	    float weaponAngle;
        CalculateRotation(angleTowardsMouse, out weaponAngle, out isFacingRight);

		bool runningBackwards = (horizontal > 0 && !isFacingRight) || (horizontal < 0 && isFacingRight);

		/**
		 *	MOVEMENT
		*/

		if (horiZero && Grounded) {
			// Try counteract the movement.
			Body.AddForce(-Body.velocity.SetY(0) * Time.deltaTime * velocityDeacceleration);
		} else {
			Body.AddForce(transform.right * horizontal * velocityAcceleration * Time.deltaTime);

			Body.velocity = Vector2.ClampMagnitude(Body.velocity.xz(), velocityTerminal).x_y(Body.velocity.y);
		}

		if (jump && Grounded) {
			Body.velocity = Body.velocity.SetY(velocityJump);
		}

		/**
		 *	VISUAL UPDATE
		*/

		// Update animators
		animBody.SetFloat("Speed", Body.velocity.xz().magnitude * (runningBackwards ? -1 : 1));
		animBody.SetBool("Moving", !horiZero);
		animBody.SetBool("Grounded", Grounded);

		// Rotate gun
		weapon.SetRotation(angleTowardsMouse, weaponAngle, isFacingRight);
		spriteBody.flipX = !isFacingRight;

		/**
		 *	SHOOTING
		*/

		if (weapon) {
			if (fireBegan) {
				// Fire
				weapon.OnFireBegan();
			}
			if (fireEnded) {
				weapon.OnFireEnded();
			}
		}

	}

	float AngleTowardsMouse(Vector3 from)
	{
		return ((Input.mousePosition.xy() - Camera.main.WorldToScreenPoint(from).xy()).ToDegrees() + 360) % 360;
	}

	public void Damage(int damage)
	{
		health -= damage;
		if (health <= 0) {
			animBody.SetBool("Dead", true);

			foreach (Collider col in GetComponentsInChildren<Collider>())
				col.enabled = false;

			Body.isKinematic = true;
			this.enabled = false;

		} else
			animBody.SetTrigger("Hit");
	}
}
