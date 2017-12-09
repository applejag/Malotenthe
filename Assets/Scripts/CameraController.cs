using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : RingObject {

	public float dist = 12;
	public float y = 5;

	private PlayerController player;

	void Awake() {
		player = FindObjectOfType<PlayerController>();

		this.enabled = player != null;
	}

	private void Update() {

		float targetDeg = RingDegrees(player.transform.position);

		transform.position = RingPositionY(targetDeg, RingData.Radius + dist, y);
		transform.rotation = RingRotation(transform.position);
	}

}
