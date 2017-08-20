using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : RingObject {

	public float angleSpeed = 90;
	public float dist = 12;
	public float y = 5;
	[Range(0,1)]
	public float lookAheadFactor = 0.5f;

	private PlayerController player;

	void Awake() {
		player = FindObjectOfType<PlayerController>();

		enabled = player != null;
	}

	private void Update() {

		float targetDeg = RingDegrees(player.transform.position);
		float currentDeg = RingDegrees(transform.position);

		// Look-ahead factor
		targetDeg += player.Body.velocity.xz().magnitude * (player.IsFacingRight ? 1 : -1) * lookAheadFactor;
		
		float newDeg = Mathf.LerpAngle(currentDeg, targetDeg, angleSpeed * Time.deltaTime);
		
		transform.position = RingPosition(newDeg, RingData.Radius + dist, y);
		transform.rotation = RingRotation(transform.position);
	}

}
