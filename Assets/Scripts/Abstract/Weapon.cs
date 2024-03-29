﻿using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {

	public RingWalker owner;
	public Animator gunAnim;
	public Transform shootPoint;
	public Vector2 bulletDirection = Vector3.right;

	public abstract void OnInputBegan();
	public abstract void OnInputEnded();
	public abstract IEnumerator OnShootCoroutine();

	public bool IsShooting { get; private set; }
	public bool IsReloading { get; private set; }
	public bool CanShoot { get { return !IsReloading && !IsShooting; } }
	protected bool IsFacingRight { get; private set; }

	public event WeaponEvent WeaponFired;
	public event WeaponReloadEvent WeaponReloading;
	public event WeaponReloadEvent WeaponReloaded;

	public delegate void WeaponEvent(Weapon source);
	public delegate void WeaponReloadEvent(Weapon source, float reloadTime);

	private void Reset()
	{
		owner = GetComponentInParent<RingWalker>();
		gunAnim = GetComponentInChildren<Animator>();
		shootPoint = gunAnim ? gunAnim.transform : transform;
	}

	private void Start()
	{
		if (owner == null)
			Debug.LogWarningFormat(this, "Weapon {0} ({1}) is missing an owner!", transform.GetPath(), GetType().Name);
	}

	public void SpawnBullet(GameObject prefab)
	{
		Vector3 position = GetShootPosition();
		Vector3 direction = GetShootDirection(position);
		Quaternion rotation = Quaternion.LookRotation(direction);

		GameObject clone = Instantiate(prefab, position, rotation);

		var bullet = clone.GetComponent<Bullet>();
		if (bullet)
			bullet.owner = owner;
		else
			Debug.LogError("Unable to find bullet script!", this);

		gunAnim.SetTrigger("Shoot");

		if (WeaponFired != null) WeaponFired(this);
	}

	public void TryStartShootCycleCoroutine()
	{
		if (CanShoot)
			StartCoroutine(ShootCoroutine());
	}
	
	public void TryReloadCoroutine(float reloadTime)
	{
		if (!IsReloading)
			StartCoroutine(ReloadCoroutine(reloadTime));
	}

	private IEnumerator ShootCoroutine()
	{
		IsShooting = true;
		yield return StartCoroutine(OnShootCoroutine());
		IsShooting = false;
	}

	private IEnumerator ReloadCoroutine(float reloadTime)
	{
		IsReloading = true;
		if (WeaponReloading != null) WeaponReloading(this, reloadTime);
		yield return new WaitForSeconds(reloadTime);
		IsReloading = false;
		if (WeaponReloaded != null) WeaponReloaded(this, reloadTime);
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

			shootPoint.position = position;
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
