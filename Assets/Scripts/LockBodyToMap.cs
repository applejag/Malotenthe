using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

[RequireComponent(typeof(Rigidbody)), DisallowMultipleComponent]
public class LockBodyToMap : RingWalker {

	public const float RADIUS = 50f;

	private void FixedUpdate() {
		Vector2 normalized = body.position.xz().normalized;

		// Set position but with interpolation
		body.MovePosition((normalized * RADIUS).x_y(body.position.y));
		
		// Look towards the center
		body.MoveRotation(Quaternion.LookRotation(-normalized.x_y(0)));
	}

	public static Vector3 LockPosition(Vector3 vec) {
		return (vec.xz().normalized * RADIUS).x_y(vec.y);
	}

}
