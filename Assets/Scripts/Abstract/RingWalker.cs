using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;
using System;

public abstract class RingWalker : RingObject
{
	[HideInInspector]
    public bool isFacingRight;
	public bool Grounded { get; protected set; }

	[Header("Grounded")]
	public LayerMask groundedRayMask = 1;
	public Vector2 groundedOffset;
	[Range(0, 1)]
	public float groundedRadius = 0.25f;

	[Header("Health")]
	public int health = 20;
	public int maxHealth = 20;
	public bool IsDead { get; private set; }
	public float HealthPercentage { get { return maxHealth == 0 ? 0 : (float)health / maxHealth; } }

	[Header("Misc.")]
	[SerializeField]
	private Vector2 m_headOffset = Vector2.up * 2;
	public Vector3 HeadPosition { get { return transform.TransformPoint(m_headOffset); } }

	[Header("Animations")]
	public Animator animBody;
	public SpriteRenderer spriteBody;

	[SerializeField, HideInInspector]
	protected bool initialSpriteFlip;

	[Header("Movement")]
	public float velocityTerminal = 8;
	public float velocityJump = 15;


	public Rigidbody Body { get; private set; }

	protected virtual void Start()
	{
		initialSpriteFlip = spriteBody.flipX;
	}

	protected virtual void Awake() {
		Body = GetComponent<Rigidbody>();
	}

	/// <summary>
	/// Raycasts and checks if the object is touching ground.
	/// </summary>
	protected bool IsGrounded()
	{
		Collider[] colliders = Physics.OverlapSphere(transform.TransformPoint(groundedOffset), groundedRadius, groundedRayMask, QueryTriggerInteraction.Ignore);
		foreach (Collider col in colliders) {
			if (col.transform.IsChildOf(transform) == false)
				return true;
		}
		return false;
	}

	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.color = IsGrounded() ? Color.red : Color.cyan;
		Gizmos.DrawWireSphere(transform.TransformPoint(groundedOffset), groundedRadius);

		Vector3 pos = transform.position;
		Vector3 head = HeadPosition;
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(pos, pos.SetY(head.y));
		Gizmos.DrawLine(pos.SetY(head.y), head);
		Gizmos.DrawWireCube(head, Vector3.one.SetY(0) * 0.5f);
	}

	protected virtual void FixedUpdate()
	{
		// Physics update
		Grounded = IsGrounded();

		Vector3 position = Body.position;

		// Set position but with interpolation
		Body.MovePosition(RingPosition(position));

		// Look towards the center
		Body.MoveRotation(RingRotation(position));
	}

	protected void ParentUpdate(bool moveAnim, bool movePhysx, float horizontal)
	{
		spriteBody.flipX = !isFacingRight ^ initialSpriteFlip;

		if (movePhysx)
			Body.velocity = (transform.right.xz() * horizontal * velocityTerminal).x_y(Body.velocity.y);

		// Update animators
		animBody.SetFloat("Speed", Body.velocity.xz().magnitude);
		animBody.SetBool("Moving", moveAnim);
		animBody.SetBool("Grounded", Grounded);
	}

	public static void CalculateRotation(float inAngle, out float outAngle, out bool facingRight)
    {
        facingRight = inAngle < 90 || inAngle > 270;
        outAngle = facingRight ? inAngle : inAngle - 180;
    }

	public virtual void Damage(int damage)
	{
		if (IsDead) return;
		health -= damage;
		IsDead = health <= 0;
		GameGUI.GameGUI.CreateDamagePopup(HeadPosition, damage);
	}

}
