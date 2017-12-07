using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using GameGUI;
using UnityEngine;

public class EnemyController : RingWalker {

	[Header("Movement")]
	public float velocityAcceleration = 600;
	public float velocityDeacceleration = 300;
	public float velocityTerminal = 8;

	[Header("Shooting")]
	public GameObject bulletPrefab;
	public Vector2 bulletOffset;
	public Vector2 bulletDirection = Vector2.right;
	public float bulletRange = 5;

	[Header("Animations")]
	public Animator animBody;
	public SpriteRenderer spriteBody;
	public SpriteRenderer spriteHead;
	public SpriteRenderer spriteBackleg;

    private PlayerController player;
    private bool aimAtPlayer = true;
	private Healthbar healthbar;

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
		isFacingRight = horizontal > 0;

		AnimatorStateInfo animState = animBody.GetCurrentAnimatorStateInfo(0);
		bool isFiring = animState.IsTag("Shooting");
		bool isMoving = animState.IsTag("Moving");
		bool inRange = isFiring == false && PlayerInRange();

	    float angleTowardsPlayer = AngleTowardsPlayer();
	    bool playerRightFromMe;
        CalculateRotation(angleTowardsPlayer, out angleTowardsPlayer, out playerRightFromMe);

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

	    if (isFiring)
	    {
            if (aimAtPlayer)
    	        spriteHead.transform.localEulerAngles = new Vector3(0, 0, angleTowardsPlayer);
	        spriteHead.flipX = 
            spriteBody.flipX = 
	        spriteBackleg.flipX = !playerRightFromMe;
	    }
        else if (!horiZero)
	    {
	        aimAtPlayer = true;
		    spriteBackleg.flipX =
            spriteHead.flipX =
            spriteBody.flipX = !isFacingRight;
		}
	}

    float AngleTowardsPlayer()
    {
        if (!player) return 0;
        Vector3 direction = transform.InverseTransformDirection(player.transform.position - transform.position);
        return (direction.xy().ToDegrees() + 360) % 360;
    }

    private bool PlayerInRange()
	{
#if UNITY_EDITOR
		player = player ?? FindObjectOfType<PlayerController>();
#endif
		if (!player) return false;

		Vector3 playerPos = player.transform.position;
		float dist = Vector3.Distance(playerPos, transform.position);

		return dist <= bulletRange;
	}

	private void AnimationEvent_Shoot()
	{
	    if (bulletPrefab == null) return;

	    Vector3 position = GetShootPosition();
	    Vector3 direction = GetShootDirection(position);
        Quaternion rotation = Quaternion.LookRotation(direction);

        Instantiate(bulletPrefab, position, rotation);
    }

    private void AnimationEvent_ContinueAiming()
    {
        aimAtPlayer = true;
    }

    private void AnimationEvent_StopAiming()
    {
        aimAtPlayer = false;
    }

    protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();

		Gizmos.color = Color.red;
	    Vector3 position = GetShootPosition();
	    Vector3 direction = GetShootDirection(position);
        Gizmos.DrawRay(position, direction);

		Gizmos.color = PlayerInRange() ? Color.red : Color.cyan;
		Gizmos.DrawWireSphere(transform.position, bulletRange);
	}

	public override void Damage(int damage)
	{
		base.Damage(damage);

		if (healthbar == null)
			healthbar = GameGUI.GameGUI.CreateHealthbar(this);
		healthbar.UpdateSliderFromWalkerHealth();

		if (Dead) {
			animBody.SetBool("Dead", true);

			foreach (Collider col in GetComponentsInChildren<Collider>())
				col.enabled = false;
			
			Body.isKinematic = true;
			this.enabled = false;

		} else
			animBody.SetTrigger("Hit");
	}


    protected Vector3 GetShootPosition()
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying == false) return RingPosition(spriteHead.transform.TransformPoint(bulletOffset));
#endif
        if (isFacingRight) return RingPosition(spriteHead.transform.TransformPoint(bulletOffset));
        else return RingPosition(spriteHead.transform.TransformPoint(bulletOffset.FlipX()));
    }

    protected Vector3 GetShootDirection(Vector3 position)
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying == false) return RingProjectDirection(position, spriteHead.transform.TransformDirection(bulletDirection)).normalized;
#endif
        if (isFacingRight) return RingProjectDirection(position, spriteHead.transform.TransformDirection(bulletDirection)).normalized;
        else return RingProjectDirection(position, spriteHead.transform.TransformDirection(bulletDirection.FlipX())).normalized;
    }

}
