using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : RingWalker {

	[Header("Movement")]
	public float velocityAcceleration = 600;
	public float velocityDeacceleration = 300;
	public float velocityTerminal = 8;

	[Header("Shooting")]
	public BoxCollider shootingTrigger;
	public GameObject bulletPrefab;
	public Vector2 bulletOffset;
	public Vector2 bulletDirection = Vector2.right;

	[Header("Health")]
	public int health = 20;
	public int maxHealth = 20;

	[Header("Animations")]
	public Animator animBody;
	public SpriteRenderer spriteBody;

	private PlayerController player;

	protected override void Awake()
	{
		base.Awake();

		player = FindObjectOfType<PlayerController>();
	}

	private void Update()
	{
		/**
		 *	READ INPUT
		*/

		float horizontal = player != null ? (Mathf.DeltaAngle(RingDegrees(transform.position), RingDegrees(player.transform.position)) < 0 ? -1 : 1) : 0;
		bool horiZero = Mathf.Approximately(horizontal, 0);
		IsFacingRight = horizontal > 0;

		AnimatorStateInfo animState = animBody.GetCurrentAnimatorStateInfo(0);
		bool isFiring = animState.IsTag("Shooting");
		bool isMoving = animState.IsTag("Moving");
		bool inRange = isFiring == false && HittingPlayer();

		/**
		 *	MOVEMENT
		*/

		if (Grounded) {
			if (!isMoving) {
				// Try counteract the movement.
				Body.AddForce(-Body.velocity.SetY(0) * Time.deltaTime * velocityDeacceleration);
			} else {
				Body.AddForce(transform.right * horizontal * velocityAcceleration * Time.deltaTime);

				Body.velocity = Vector2.ClampMagnitude(Body.velocity.xz(), velocityTerminal).x_y(Body.velocity.y);
			}
		}

		/**
		 *	VISUAL UPDATE
		*/

		animBody.SetFloat("Speed", Body.velocity.xz().magnitude);
		animBody.SetBool("Moving", !horiZero);
		animBody.SetBool("Grounded", Grounded);
		animBody.SetBool("In Range", inRange);

		if (!horiZero) {
			spriteBody.flipX = !IsFacingRight;
		}
	}

	private bool HittingPlayer()
	{
		if (!player) return false;

		Vector3 localPos = shootingTrigger.transform.localPosition;
		localPos.x = IsFacingRight ? Mathf.Abs(localPos.x) : -Mathf.Abs(localPos.x);
		shootingTrigger.transform.localPosition = localPos;

		Vector3 center = shootingTrigger.transform.TransformPoint(shootingTrigger.center);
		Vector3 halfSize = shootingTrigger.transform.TransformVector(shootingTrigger.size * 0.5f);
		Quaternion rotation = shootingTrigger.transform.rotation;

		Collider[] colliders = Physics.OverlapBox(center, halfSize, rotation);
		foreach (Collider col in colliders) {
			if (col.transform.IsChildOf(player.transform))
				return true;
		}
		return false;
	}

	private void AnimationEvent_Shoot()
	{
		print(transform.GetPath() + " fired!");
		//Vector3 position = RingPosition(transform.TransformPoint(IsFacingRight ? bulletOffset.x : -bulletOffset.x, bulletOffset.y, 0));
		//Vector3 direction = RingProjectDirection(position, transform.TransformDirection(IsFacingRight ? bulletDirection.x : -bulletDirection.x, bulletDirection.y, 0));
		//Quaternion rotation = Quaternion.LookRotation(direction);

		//Instantiate(bulletPrefab, position, rotation);
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();

		Gizmos.color = Color.red;
		if (UnityEditor.EditorApplication.isPlaying && IsFacingRight == false) {
			Vector3 position = RingPosition(transform.TransformPoint(bulletOffset.FlipX()));
			Vector3 direction = RingProjectDirection(position, transform.TransformDirection(bulletDirection).FlipX()).normalized;
			Gizmos.DrawRay(position, direction);
		} else {
			Vector3 position = RingPosition(transform.TransformPoint(bulletOffset));
			Vector3 direction = RingProjectDirection(position, transform.TransformDirection(bulletDirection)).normalized;
			Gizmos.DrawRay(position, direction);
		}
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
