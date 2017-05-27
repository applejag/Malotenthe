using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : RingWalker {

	public float angleSpeed = 90;
	public float dist = 12;
	public float y = 5;

	private PlayerController player;

	protected override void Awake() {
		base.Awake();
		player = FindObjectOfType<PlayerController>();

		enabled = player != null;
	}

	private void Update() {

		float target = player.RingDegrees;
		target += player.body.velocity.xz().magnitude * (player.facingRight ? 1 : -1);

		this.RingDegrees = Mathf.LerpAngle(this.RingDegrees, target, angleSpeed * Time.deltaTime);
		this.RingRadius = LockBodyToMap.RADIUS + dist;
		transform.rotation = Quaternion.LookRotation(-transform.position.SetY(0));
		transform.position = transform.position.SetY(y);
	}

}
