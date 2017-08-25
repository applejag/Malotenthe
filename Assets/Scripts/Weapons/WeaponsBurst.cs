using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsBurst : Weapon
{
	public GameObject bulletPrefab;

	[Header("Burst settings")]
	public float bulletsPerSecond = 10;
	public int bulletsPerBurst = 3;

	private bool isFiring;

	public override void OnFireBegan()
	{
		if (isFiring == false)
			StartCoroutine(ShootBurst());
	}

	public override void OnFireEnded()
	{
		// Do nofin
	}

	void SpawnBullet()
	{
		Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);

		Anim.SetTrigger("Shoot");
	}

	IEnumerator ShootBurst()
	{
		isFiring = true;

		for (int i = 0; i < bulletsPerBurst; i++) {
			SpawnBullet();
			yield return new WaitForSeconds(1 / bulletsPerSecond);
		}

		isFiring = false;
	}
}
