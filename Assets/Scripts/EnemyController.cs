using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using GameGUI;
using UnityEngine;

public class EnemyController : RingWalker {

	[Header("Shooting")]
	public GameObject bulletPrefab;
	public Vector2 bulletOffset;
	public Vector2 bulletDirection = Vector2.right;
	public float bulletRange = 5;

	[Header("Animations")]
	public SpriteRenderer spriteHead;
	public SpriteRenderer spriteBackleg;
	public Transform HeadTransform { get { return spriteHead ? spriteHead.transform : (spriteBody ? spriteBody.transform : transform); } }

    private PlayerController player;
    private bool aimAtPlayer;
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

		float horizontal = (player && !player.IsDead)
			? (Mathf.DeltaAngle(RingDegrees(transform.position), RingDegrees(player.transform.position)) < 0
				? -1
				: 1)
			: 0;

		AnimatorStateInfo animState = animBody.GetCurrentAnimatorStateInfo(0);
		bool isFiring = animState.IsTag("Shooting");
		bool isMoving = animState.IsTag("Moving");
		bool inRange = isFiring == false && PlayerInRange();
		bool playerAlive = player && !player.IsDead;
		bool shouldMove = !inRange && playerAlive;

		float angleTowardsPlayer = AngleTowardsPlayer();
	    bool playerRightFromMe;
        CalculateRotation(angleTowardsPlayer, out angleTowardsPlayer, out playerRightFromMe);

		/**
		 *	MOVEMENT
		*/

		if (isMoving)
			isFacingRight = horizontal > 0;

		/**
		 *	VISUAL UPDATE
		*/
		
		animBody.SetBool("In Range", inRange);

	    if (isFiring)
	    {
		    if (spriteHead && aimAtPlayer)
				spriteHead.transform.localEulerAngles = new Vector3(0, 0, angleTowardsPlayer);

		    isFacingRight = playerRightFromMe;
	    }

		if (spriteBackleg)
			spriteBackleg.flipX = !isFacingRight ^ initialSpriteFlip;

		if (spriteHead)
			spriteHead.flipX = !isFacingRight ^ initialSpriteFlip;

		/** 
		 * Update from parent class
		*/

		ParentUpdate(shouldMove, isMoving, horizontal);
	}

    float AngleTowardsPlayer()
    {
        if (!player || player.IsDead) return 0;
        Vector3 direction = transform.InverseTransformDirection(player.transform.position - transform.position);
        return (direction.xy().ToDegrees() + 360) % 360;
    }

    private bool PlayerInRange()
	{
#if UNITY_EDITOR
		player = player ?? FindObjectOfType<PlayerController>();
#endif
		if (!player || player.IsDead) return false;

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
	    if (!enabled) return;
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

		if (IsDead) {
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
        if (UnityEditor.EditorApplication.isPlaying == false)
			return RingPosition(HeadTransform.TransformPoint(bulletOffset));
#endif
        if (isFacingRight)
			return RingPosition(HeadTransform.TransformPoint(bulletOffset));
        else
			return RingPosition(HeadTransform.TransformPoint(bulletOffset.FlipX()));
    }

    protected Vector3 GetShootDirection(Vector3 position)
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying == false)
        {
	        Vector3 direction = HeadTransform.TransformDirection(bulletDirection);
	        return RingProjectDirection(position, direction).normalized;
        }
#endif
        if (isFacingRight)
        {
	        Vector3 direction = HeadTransform.TransformDirection(bulletDirection);
	        return RingProjectDirection(position, direction).normalized;
        }
        else
        {
	        Vector3 direction = HeadTransform.TransformDirection(bulletDirection.FlipX());
	        return RingProjectDirection(position, direction).normalized;
        }
    }

}
