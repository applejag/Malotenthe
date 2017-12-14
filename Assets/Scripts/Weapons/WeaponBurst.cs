using System;
using System.Collections;
using System.Collections.Generic;
using GameGUI;
using UnityEngine;

public sealed class WeaponBurst : Weapon
{
	public float reloadTime = 2;
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

	public override IEnumerator OnShootCoroutine()
	{
		for (int i = 0; i < bulletsPerBurst; i++) {
			SpawnBullet(bulletPrefab);
			yield return new WaitForSeconds(1 / bulletsPerSecond);
		}
		TryReloadCoroutine(reloadTime);
	}
}
