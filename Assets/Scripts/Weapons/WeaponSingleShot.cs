using System;
using System.Collections;
using System.Collections.Generic;
using GameGUI;
using UnityEngine;

public sealed class WeaponSingleShot : Weapon
{
	public GameObject bulletPrefab;
	public float bulletsPerSecond = 10;

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
		SpawnBullet(bulletPrefab);
		TryReloadCoroutine(1 / bulletsPerSecond);
		yield return new WaitForSeconds(0.5f / bulletsPerSecond);
	}
}
