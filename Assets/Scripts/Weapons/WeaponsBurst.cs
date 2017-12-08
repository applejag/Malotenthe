using System;
using System.Collections;
using System.Collections.Generic;
using GameGUI;
using UnityEngine;

public sealed class WeaponsBurst : Weapon
{
	public GameObject bulletPrefab;

	[Header("Burst settings")]
	public float bulletsPerSecond = 10;
	public int bulletsPerBurst = 3;
	public float reloadTime;

	private bool isFiring;

	public override void OnInputBegan()
	{
		if (isFiring == false)
			StartCoroutine(ShootBurst());
	}

	public override void OnInputEnded()
	{
		// Do nofin
	}

	IEnumerator ShootBurst()
	{
		isFiring = true;

		for (int i = 0; i < bulletsPerBurst; i++) {
			SpawnBullet(bulletPrefab);
			yield return new WaitForSeconds(1 / bulletsPerSecond);
		}

		OnWeaponReloading(reloadTime);

		yield return new WaitForSeconds(reloadTime);
		isFiring = false;

		OnWeaponReloaded();
	}
}
