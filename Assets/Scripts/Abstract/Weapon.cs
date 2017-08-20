using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Weapon : MonoBehaviour {
	
	public Vector2 bulletOffset;
	public Vector2 bulletDirection = Vector3.right;

	public abstract void OnFireBegan();
	public abstract void OnFireEnded();

	protected SpriteRenderer Sprite { get; private set; }
	protected Animator Anim { get; private set; }
	protected bool IsFacingRight { get; private set; }

	protected virtual void Awake()
	{
		Sprite = GetComponent<SpriteRenderer>();
		Anim = GetComponent<Animator>();
	}

	public virtual void SetRotation(float angle, bool flipX)
	{
		IsFacingRight = !flipX;

		// Move gun
		Vector3 localPos = transform.localPosition;
		localPos.x = IsFacingRight ? Mathf.Abs(localPos.x) : -Mathf.Abs(localPos.x);
		transform.localPosition = localPos;

		transform.localEulerAngles = new Vector3(0, 0, IsFacingRight ? angle : angle - 180);
		Sprite.flipX = flipX;
	}

	protected virtual void OnEnable()
	{
		Sprite.enabled = true;
	}

	protected virtual void OnDisable()
	{
		Sprite.enabled = false;
	}

	protected Vector3 GetShootPosition()
	{
#if UNITY_EDITOR
		if (UnityEditor.EditorApplication.isPlaying == false) return RingObject.RingPosition(transform.TransformPoint(bulletOffset));
#endif
		if (IsFacingRight) return RingObject.RingPosition(transform.TransformPoint(bulletOffset));
		else return RingObject.RingPosition(transform.TransformPoint(bulletOffset.FlipX()));
	}

	protected Vector3 GetShootDirection(Vector3 position)
	{
#if UNITY_EDITOR
		if (UnityEditor.EditorApplication.isPlaying == false) return RingObject.RingProjectDirection(position, transform.TransformDirection(bulletDirection)).normalized;
#endif
		if (IsFacingRight) return RingObject.RingProjectDirection(position, transform.TransformDirection(bulletDirection)).normalized;
		else return RingObject.RingProjectDirection(position, transform.TransformDirection(bulletDirection.FlipX())).normalized;
	}

	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Vector3 position = GetShootPosition();
		Vector3 direction = GetShootDirection(position);
		Gizmos.DrawRay(position, direction);
	}

}
