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

	public override void OnInputBegan()
	{
		TryStartShootCycleCoroutine();
	}

	public override void OnInputEnded()
	{
		// Do nofin
	}

	public override IEnumerator ShootCoroutine()
	{
		for (int i = 0; i < bulletsPerBurst; i++) {
			SpawnBullet(bulletPrefab);
			yield return new WaitForSeconds(1 / bulletsPerSecond);
		}
	}
}
