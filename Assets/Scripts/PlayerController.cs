using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : RingWalker {

	[Header("Movement")]
	public float velocityAcceleration = 50;
	public float velocityDeacceleration = 200;
	public float velocityTerminal = 1;
	public float velocityJump = 5;
	
	[Header("Grounded")]
	public LayerMask groundedRayMask = 1;
	public float groundedRayLength = 2;
	public float groundedRaySkin = .5f;

	[Header("Shooting")]
	public GameObject bulletPrefab;

	private SpriteRenderer sprite;

	protected override void Awake() {
		base.Awake();

		sprite = GetComponentInChildren<SpriteRenderer>();
		facingRight = true;
	}

	private void Update() {

		/**
		 *	READ INPUT
		*/

		float horizontal = Input.GetAxis("Horizontal");
		bool horiZero = Mathf.Approximately(horizontal, 0);
		bool jump = Input.GetButtonDown("Jump");

		bool grounded = false;
		Ray ray = new Ray(transform.position + Vector3.up * groundedRaySkin, Vector3.down);
		if (Physics.Raycast(ray, groundedRayLength + groundedRaySkin, groundedRayMask)) {
			grounded = true;
		}

		bool fire = Input.GetButtonDown("Fire1");

		/**
		 *	MOVEMENT
		*/

		if (horiZero && grounded) {
			// Try counteract the movement.
			body.AddForce(-body.velocity.SetY(0) * Time.deltaTime * velocityDeacceleration);
		} else {
			body.AddForce(transform.right * horizontal * velocityAcceleration * Time.deltaTime);

			body.velocity = Vector2.ClampMagnitude(body.velocity.xz(), velocityTerminal).x_y(body.velocity.y);
		}

		if (jump && grounded) {
			body.velocity = body.velocity.SetY(velocityJump);
		}

		/**
		 *	VISUAL UPDATE
		*/

		if (!horiZero) {
			sprite.flipX = horizontal < 0;
			facingRight = !sprite.flipX;
		}

		/**
		 *	SHOOTING
		*/

		if (fire) {
			Vector3 forward = facingRight ? transform.right : -transform.right;
			var clone = Instantiate(bulletPrefab, LockBodyToMap.LockPosition(transform.position + forward * 2), Quaternion.LookRotation(forward));
			var c_body = clone.GetComponent<Rigidbody>();

			c_body.AddForce(forward * 50, ForceMode.Impulse);
		}

	}

}
