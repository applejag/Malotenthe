using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {

	public Transform shootPoint;
	public Vector2 bulletDirection = Vector3.right;

	public abstract void OnInputBegan();
	public abstract void OnInputEnded();

	public Animator Anim { get; private set; }
	protected bool IsFacingRight { get; private set; }

	public event WeaponEvent WeaponFired;
	public event WeaponReloadEvent WeaponReloading;
	public event WeaponEvent WeaponReloaded;

	public delegate void WeaponEvent(Weapon source);
	public delegate void WeaponReloadEvent(Weapon source, float reloadTime);

	protected void OnWeaponFired()
	{
		if (WeaponFired != null) WeaponFired(this);
	}

	protected void OnWeaponReloading(float reloadTime)
	{
		if (WeaponReloading != null) WeaponReloading(this, reloadTime);
	}

	protected void OnWeaponReloaded()
	{
		if (WeaponReloaded != null) WeaponReloaded(this);
	}

	protected virtual void Awake()
	{
		Anim = GetComponent<Animator>();
	}

	public void SpawnBullet(GameObject prefab)
	{
		Vector3 position = GetShootPosition();
		Vector3 direction = GetShootDirection(position);
		Quaternion rotation = Quaternion.LookRotation(direction);

		Instantiate(prefab, position, rotation);

		Anim.SetTrigger("Shoot");

		OnWeaponFired();
	}

	public virtual void SetRotation(float lookAngle, bool facingRight)
	{
	    IsFacingRight = facingRight;

		// Move gun
		Vector3 localPos = transform.localPosition;
		localPos.x = IsFacingRight ? Mathf.Abs(localPos.x) : -Mathf.Abs(localPos.x);
		transform.localPosition = localPos;

		transform.localEulerAngles = new Vector3(0, 0, lookAngle);

		if (shootPoint && shootPoint != transform) {
			Vector3 position = GetShootPosition();
			Vector3 direction = GetShootDirection(position);
			Quaternion rotation = Quaternion.LookRotation(direction);

			shootPoint.position = position;
			shootPoint.rotation = rotation;
		}
	}

	protected Vector3 GetShootPosition()
	{
		return shootPoint ? shootPoint.position : transform.position;
	}

	protected Vector3 GetShootDirection(Vector3 position)
	{
		Vector3 direction = transform.TransformDirection(bulletDirection);
		return RingObject.RingProjectDirection(position, direction).normalized;
	}

#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Vector3 position = GetShootPosition();
		Vector3 direction = GetShootDirection(position);
		Gizmos.DrawRay(position, direction);
		Gizmos.DrawWireSphere(position+direction, 0.05f);
		Gizmos.color = new Color(1,0,0,0.5f);
		Gizmos.DrawSphere(position, 0.05f);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(position, 0.05f);
	}
#endif

}
