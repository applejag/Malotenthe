using System;
using System.Collections;
using System.Collections.Generic;
using GameGUI;
using UnityEngine;

public class WeaponsBurst : Weapon
{
	public GameObject bulletPrefab;
	public Reloadbar reloadbar;

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
			if (reloadbar) reloadbar.Blink();
			yield return new WaitForSeconds(1 / bulletsPerSecond);
		}

		if (reloadbar) reloadbar.Reloading(reloadTime);

		yield return new WaitForSeconds(reloadTime);
		isFiring = false;

		if (reloadbar) reloadbar.Reloaded();
	}
}
