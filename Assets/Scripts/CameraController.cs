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

		float targetDeg = RingDegrees(player.transform.position);
		float currentDeg = RingDegrees(transform.position);

		// Look-ahead factor
		targetDeg += player.Body.velocity.xz().magnitude * (player.FacingRight ? 1 : -1);
		
		float newDeg = Mathf.LerpAngle(currentDeg, targetDeg, angleSpeed * Time.deltaTime);
		
		transform.position = RingPosition(newDeg, RingData.Radius + dist, y);
		transform.rotation = RingRotation(transform.position);
	}

}
