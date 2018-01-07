using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;
using System;
using UnityEngine.Serialization;

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
	[SingleLayer]
	public int deadLayerMask = 1;
	public float HealthPercentage { get { return maxHealth == 0 ? 0 : (float)health / maxHealth; } }

	[Header("Shooting")]
	public Weapon weapon;

	[Header("Misc.")]
	[SerializeField]
	protected Vector2 m_headOffset = Vector2.up * 2;
	public Vector3 HeadPosition { get { return transform.TransformPoint(m_headOffset); } }

	[Header("Animations")]
	public Animator animBody;
	public SpriteRenderer spriteBody;

	[SerializeField, HideInInspector]
	protected bool initialSpriteFlip;

	[Header("Movement")]
	public float velocityTerminal = 8;
	[FormerlySerializedAs("velocityJump")]
	public float velocityJumpHeight = 15;

	private float lastVelocityJumpForce = -1;
	public float VelocityJumpForce
	{
		get
		{
			if (lastVelocityJumpForce < 0)
			{
				float g = Physics.gravity.y;
				float h = velocityJumpHeight;
				float t = Mathf.Sqrt(-2 * h / g);
				return lastVelocityJumpForce = -g * t;
			}
			else
				return lastVelocityJumpForce;
		}
	}

	public Rigidbody Body { get; private set; }

	public delegate void DamageEvent(int damage, object source);

	public event DamageEvent EventDamageTaken;
	public event DamageEvent EventDeath;
	public event DamageEvent EventDamageDealt;
	public event DamageEvent EventKilled;

	protected virtual void Start()
	{
		initialSpriteFlip = spriteBody.flipX;
	}

	protected virtual void Awake() {
		Body = GetComponent<Rigidbody>();
	}

	protected virtual void OnValidate()
	{
		lastVelocityJumpForce = VelocityJumpForce;
	}

	/// <summary>
	/// Raycasts and checks if the object is touching ground.
	/// </summary>
	protected bool IsGrounded()
	{
		Collider[] colliders = Physics.OverlapSphere(
			position: transform.TransformPoint(groundedOffset), 
			radius: groundedRadius, 
			layerMask: groundedRayMask, 
			queryTriggerInteraction: QueryTriggerInteraction.Ignore);

		foreach (Collider col in colliders) {
			if (col.transform.IsChildOf(transform) == false)
				return true;
		}
		return false;
	}

	protected virtual void OnDrawGizmos()
	{
		Gizmos.color = IsGrounded() ? Color.red : Color.cyan;
		Gizmos.DrawWireSphere(transform.TransformPoint(groundedOffset), groundedRadius);

		Vector3 pos = transform.position;
		Vector3 head = HeadPosition;
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(pos, pos.SetY(head.y));
		Gizmos.DrawLine(pos.SetY(head.y), head);
		Gizmos.DrawWireCube(head, new Vector3(0.5f, 0, 0.5f));

		Gizmos.color = new Color(1, 1, 0, 0.25f);
		Gizmos.DrawLine(pos, new Vector3(0, pos.y, 0));
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

	public void Damage(int damage, object source)
	{
		if (IsDead) return;
		health -= damage;
		IsDead = health <= 0;
		GameGUI.GameGUI.CreateDamagePopup(HeadPosition, damage);

		if (IsDead)
		{
			animBody.SetBool("Dead", true);
			gameObject.SetLayerRecursive(deadLayerMask);
		} 
		else
			animBody.SetTrigger("Hit");

		// Event : damage taken
		if (EventDamageTaken != null)
			EventDamageTaken(damage, source);

		var bullet = source as Bullet;
		RingWalker bsource = bullet ? bullet.owner : null;
		Debug.LogFormat(this, "{0} took {1} damage from {2}", name, damage, bsource ? bsource.name : "<NULL>");
		if (bsource)
		{
			// Event : damage dealt
			if (bsource.EventDamageDealt != null)
				bsource.EventDamageDealt(damage, this);

			if (bsource.EventKilled != null && IsDead)
				bsource.EventKilled(damage, this);
		}

		// Event : died
		if (EventDeath != null && IsDead)
			EventDeath(damage, source);
	}


}
